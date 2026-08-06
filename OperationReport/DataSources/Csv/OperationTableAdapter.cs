using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Converters;
using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources.Csv;

public class OperationTableAdapter(OperationSourceInfo info) : CsvAdapter(info.FilePath), IOperationTableAdapter
{
    private readonly int _timestampColumnIndex = info.TimestampColumnIndex;
    private readonly int _commentColumnIndex = info.CommentColumnIndex;
    private readonly ITimestampConverter _timestampConverter = TimestampConverter.ForColumn(info.TimestampColumn);

    public async Task<int> CountAsync(FilterQuery filterQuery, CancellationToken cancellationToken = default)
    {
        using var csvReader = await CreateCsvReader();

        var count = 0;
        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;

            var timestamp = _timestampConverter.ToDateTime(row[_timestampColumnIndex]);
            if(!IsMatchFilter(filterQuery, timestamp))
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
        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;

            var timestamp = _timestampConverter.ToDateTime(row[_timestampColumnIndex]);
            if(!IsMatchFilter(filterQuery, timestamp))
                continue;
            
            var comment = row[_commentColumnIndex];
            yield return new Operation(timestamp, comment);
        }
    }

    private bool IsMatchFilter(FilterQuery filterQuery, DateTime timestamp)
    {
        if (filterQuery.From.HasValue)
        {
            var from = _timestampConverter.ToDateTime(filterQuery.From.Value);
            if(timestamp < from) return false;
        }
        
        if (filterQuery.To.HasValue)
        {
            var to = _timestampConverter.ToDateTime(filterQuery.To.Value);
            if(timestamp > to) return false;
        }
        
        return true;
    }
}