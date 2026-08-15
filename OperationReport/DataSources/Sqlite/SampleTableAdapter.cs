using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Entities;
using SqlKata;
using Column = AetherSystem.OperationReport.DataSources.Schema.Column;

namespace AetherSystem.OperationReport.DataSources.Sqlite;

public sealed class SampleTableAdapter(SampleSourceInfo sourceInfo)
    : SqliteAdapter(sourceInfo.FilePath), ISampleTableAdapter
{
    public IReadOnlyList<Column> SampleColumns => sourceInfo.SampleColumns;
    public TimestampColumn TimestampColumn => sourceInfo.TimestampColumn;
    public Column? BatchNumberColumn => sourceInfo.BatchNumberColumn;

    public async Task<int> CountAsync(SampleFilterQuery filterQuery, CancellationToken cancellationToken = default)
    {
        var query = CreateFilterQuery(filterQuery).AsCount();
        await using var command = CreateExecutableCommand(query);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async IAsyncEnumerable<Sample> EnumerateAsync(
        SampleFilterQuery filterQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = CreateFilterQuery(filterQuery)
            .Select([TimestampColumn.Name, ..SampleColumns.Select(c => c.Name)])
            .OrderBy(TimestampColumn.Name);

        await using var command = CreateExecutableCommand(query);

        var reader = await command.ExecuteReaderAsync(cancellationToken);
        var converter = sourceInfo.TimestampColumn.Format.Converter;
        while (await reader.ReadAsync(cancellationToken))
        {
            var samples = new double[SampleColumns.Count];
            
            var timestamp = converter.ToDateTime((IConvertible)reader.GetValue(0));
            for(int i = 0; i < SampleColumns.Count; i++)
                samples[i] = Convert.ToDouble(reader.GetValue(i + 1));
            yield return new Sample(timestamp, samples.AsReadOnly());
        }
    }
    
    public async IAsyncEnumerable<SampleFilterQuery> DiscoverActiveSignalRangesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (SampleColumns.Count == 0)
            yield break;

        const int confirmationSampleCount = 100;
        const double epsilon = 1e-5;

        var activePredicates = string.Join(
            " OR ",
            SampleColumns.Select(column => $"ABS({QuoteIdentifier(column.Name)}) >= ?"));
        var epsilonBindings = Enumerable.Repeat<object>(epsilon, SampleColumns.Count).ToArray();

        var classified = new Query(sourceInfo.Table.Name)
            .SelectRaw($"{QuoteIdentifier(TimestampColumn.Name)} AS timestamp")
            .SelectRaw(
                $"CASE WHEN {activePredicates} THEN 1 ELSE 0 END AS active",
                epsilonBindings);

        var rollingWindow =
            $"ORDER BY timestamp ROWS BETWEEN {confirmationSampleCount - 1} PRECEDING AND CURRENT ROW";

        var rolling = new Query("classified")
            .Select("timestamp")
            .SelectRaw($"MIN(timestamp) OVER ({rollingWindow}) AS edge_timestamp")
            .SelectRaw($"COUNT(*) OVER ({rollingWindow}) AS window_count")
            .SelectRaw($"SUM(active) OVER ({rollingWindow}) AS active_count");

        var transitions = new Query("rolling")
            .Select("edge_timestamp", "timestamp", "window_count", "active_count")
            .SelectRaw("LAG(active_count) OVER (ORDER BY timestamp) AS previous_active_count");

        var query = new Query("transitions")
            .With("classified", classified)
            .With("rolling", rolling)
            .With("transitions", transitions)
            .Select("edge_timestamp", "timestamp AS confirmed_at")
            .SelectRaw(
                "CASE WHEN active_count = ? THEN 'start' WHEN active_count = 0 THEN 'end' END AS edge",
                confirmationSampleCount)
            .Where("window_count", confirmationSampleCount)
            .Where(condition => condition
                .Where(start => start
                    .Where("active_count", confirmationSampleCount)
                    .WhereNot("previous_active_count", confirmationSampleCount))
                .OrWhere(end => end
                    .Where("active_count", 0)
                    .WhereNot("previous_active_count", 0)))
            .OrderBy("confirmed_at");

        await using var command = CreateExecutableCommand(query);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var format = sourceInfo.TimestampColumn.Format;
        DateTime? startTimestamp = null;

        while (await reader.ReadAsync(cancellationToken))
        {
            var timestamp = format.Converter.ToDateTime(reader.GetValue(0));
            var edge = reader.GetString(2);

            if (edge == "start")
            {
                startTimestamp ??= timestamp;
                continue;
            }

            if (edge == "end" && startTimestamp is { } from && timestamp > from)
            {
                yield return new SampleFilterQuery(from, timestamp, null);
                startTimestamp = null;
            }
        }
    }

    private Query CreateFilterQuery(FilterQuery filterQuery)
    {
        var table = sourceInfo.Table.Name;
        var query = new Query(table);
        var converter = sourceInfo.TimestampColumn.Format.Converter;
        
        if (filterQuery.From is not null)
        {
            var value = converter.FromDateTime(filterQuery.From.Value);
            query = query.Where(sourceInfo.TimestampColumn.Name, ">=", value);
        }

        if (filterQuery.To is not null)
        {
            var value = converter.FromDateTime(filterQuery.To.Value);
            query = query.Where(sourceInfo.TimestampColumn.Name, "<=", value);
        }

        return query;
    }
}