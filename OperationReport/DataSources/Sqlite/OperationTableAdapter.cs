using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Converters;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Entities;
using SqlKata;

namespace AetherSystem.OperationReport.DataSources.Sqlite;

public class OperationTableAdapter(OperationSourceInfo sourceInfo) 
    : SqliteAdapter(sourceInfo.FilePath), IOperationTableAdapter
{
    private readonly ITimestampConverter _timestampConverter = TimestampConverter.ForColumn(sourceInfo.TimestampColumn);

    public async Task<int> CountAsync(
        FilterQuery filterQuery,
        CancellationToken cancellationToken = default)
    {
        var query = CreateFilterQuery(filterQuery).AsCount();
        await using var command = CreateExecutableCommand(query);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async IAsyncEnumerable<Operation> EnumerateAsync(
        FilterQuery filterQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = CreateFilterQuery(filterQuery)
            .Select(sourceInfo.TimestampColumn.Name, sourceInfo.CommentColumn.Name)
            .OrderBy(sourceInfo.TimestampColumn.Name);

        await using var command = CreateExecutableCommand(query);
        var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var timestamp = _timestampConverter.ToDateTime(reader.GetValue(0));
            var comment = reader.GetString(1);
            yield return new Operation(timestamp, comment);
        }
    }

    private Query CreateFilterQuery(FilterQuery filterQuery)
    {
        var table = sourceInfo.Table.Name;
        var query = new Query(table);
        
        if (filterQuery.From is not null)
        {
            var value = _timestampConverter.FromDateTime(filterQuery.From.Value);
            query = query.Where(sourceInfo.TimestampColumn.Name, ">=", value);
        }

        if (filterQuery.To is not null)
        {
            var value = _timestampConverter.FromDateTime(filterQuery.To.Value);
            query = query.Where(sourceInfo.TimestampColumn.Name, "<=", value);
        }

        return query;
    }
}