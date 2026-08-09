using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Entities;
using SqlKata;
using Column = AetherSystem.OperationReport.DataSources.Schema.Column;

namespace AetherSystem.OperationReport.DataSources.Sqlite;

public class SampleTableAdapter(SampleSourceInfo sourceInfo)
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