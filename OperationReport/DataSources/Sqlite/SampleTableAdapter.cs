using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Entities;
using SqlKata;
using Column = AetherSystem.OperationReport.DataSources.Schema.Column;

namespace AetherSystem.OperationReport.DataSources.Sqlite;

public class SampleTableAdapter(SampleDataSourceInfo sourceInfo)
    : SqliteAdapter(sourceInfo.FilePath), ISampleTableAdapter
{
    public IReadOnlyList<Column> SampleColumns => sourceInfo.SampleColumns;
    public DateTimeColumn TimestampColumn => sourceInfo.TimestampColumn;
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
            .Select([TimestampColumn.Name, ..SampleColumns.Select(c => c.Name)]);

        await using var command = CreateExecutableCommand(query);

        var timestampResolver = new TimestampResolver(TimestampColumn);
        var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var samples = new double[SampleColumns.Count];
            var timestamp = timestampResolver.ToDateTime(reader.GetValue(0));
            for(int i = 0; i < SampleColumns.Count; i++)
                samples[i] = Convert.ToDouble(reader.GetValue(i + 1));
            yield return new Sample(timestamp, samples.AsReadOnly());
        }
    }

    private Query CreateFilterQuery(FilterQuery filterQuery)
    {
        var table = sourceInfo.Table.Name;
        var query = new Query(table);
        return base.CreateFilterQuery(query, sourceInfo.TimestampColumn, filterQuery);
    }
}