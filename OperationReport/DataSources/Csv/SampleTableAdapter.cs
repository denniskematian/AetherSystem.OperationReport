using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Converters;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources.Csv;

public class SampleTableAdapter(SampleSourceInfo info) : CsvAdapter(info.FilePath), ISampleTableAdapter
{
    private readonly int _timestampColumnIndex = info.TimestampColumnIndex;
    private readonly int _batchNumberColumnIndex = info.BatchNumberColumnIndex ?? -1;
    private readonly ITimestampConverter _timestampConverter = TimestampConverter.ForColumn(info.TimestampColumn);

    public IReadOnlyList<Column> SampleColumns => info.SampleColumns;
    public DateTimeColumn TimestampColumn => info.TimestampColumn;
    public Column? BatchNumberColumn => info.BatchNumberColumn;
    
    public async Task<int> CountAsync(SampleFilterQuery filterQuery, CancellationToken cancellationToken = default)
    {
        using var csvReader = await CreateCsvReader();

        var count = 0;
        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;

            var timestamp = _timestampConverter.ToDateTime(row[_timestampColumnIndex]);
            if(!IsMatchFilter(filterQuery, timestamp, row))
                continue;

            count++;
        }

        return count;
    }

    public async IAsyncEnumerable<Sample> EnumerateAsync(
        SampleFilterQuery filterQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var csvReader = await CreateCsvReader();
        var indexes = info.GetSampleColumnIndices();

        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;
            
            var timestamp = _timestampConverter.ToDateTime(row[_timestampColumnIndex]);
            if(!IsMatchFilter(filterQuery, timestamp, row))
                continue;

            var sampleValues = new double[SampleColumns.Count];
            for (int i = 0; i < indexes.Count; i++)
            {
                var index = indexes[i];
                var value = row[index];
                sampleValues[i] = Convert.ToDouble(value);
            }
            
            yield return new Sample(timestamp, sampleValues.AsReadOnly());
        }
    }

    private bool IsMatchFilter(SampleFilterQuery filterQuery, DateTime timestamp, string[] row)
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

        if(filterQuery.BatchNumber is null || _batchNumberColumnIndex < 0)
            return true;

        var batchNumber = Convert.ToInt32(row[_batchNumberColumnIndex]);
        return batchNumber == filterQuery.BatchNumber;
    }
}