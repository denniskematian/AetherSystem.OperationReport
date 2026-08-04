using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources.Csv;

public class OperationTableAdapter(OperationSourceInfo info) : CsvAdapter(info.FilePath), IOperationTableAdapter
{
    private int _timestampColumnIndex = -1;
    private int _commentColumnIndex = -1;

    public async Task<int> CountAsync(FilterQuery filterQuery, CancellationToken cancellationToken = default)
    {
        using var csvReader = await CreateCsvReader();

        var count = 0;
        var tsColumnIndex = GetTimestampColumnIndex();
        var resolver = new TimestampResolver(info.TimestampColumn);
        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;

            var timestamp = resolver.ToDateTime(row[tsColumnIndex]);
            if(!IsMatchFilter(filterQuery, info.TimestampColumn, timestamp))
                continue;

            count++;
        }

        return count;
    }

    public async IAsyncEnumerable<Operation> EnumerateAsync(
        FilterQuery filterQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var csvReader = await CreateCsvReader();
        var resolver = new TimestampResolver(info.TimestampColumn);
        var timestampColumnIndex = GetTimestampColumnIndex();
        var commentColumnIndex = GetCommentColumnIndex();
        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;
            
            var timestamp = resolver.ToDateTime(row[timestampColumnIndex]);
            if(!IsMatchFilter(filterQuery, info.TimestampColumn, timestamp))
                continue;
            
            var comment = row[commentColumnIndex];
            yield return new Operation(timestamp, comment);
        }
    }

    private int GetTimestampColumnIndex()
    {
        if (_timestampColumnIndex < 0)
        {
            _timestampColumnIndex = info.Table.Columns.Index().First(tc => tc.Item.Name == info.TimestampColumn.Name).Index;
        }

        return _timestampColumnIndex;
    }

    private int GetCommentColumnIndex()
    {
        if (_commentColumnIndex < 0)
        {
            _commentColumnIndex = info.Table.Columns.Index().First(tc => tc.Item.Name == info.CommentColumn.Name).Index;
        }

        return _commentColumnIndex;
    }
}