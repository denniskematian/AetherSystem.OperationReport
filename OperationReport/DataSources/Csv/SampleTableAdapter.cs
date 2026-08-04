using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources.Csv;

public class SampleTableAdapter(SampleDataSourceInfo info) : CsvAdapter(info.FilePath), ISampleTableAdapter
{
    private int _timestampColumnIndex = -1;
    private int _batchNumberColumnIndex = -1;

    public IReadOnlyList<Column> SampleColumns => info.SampleColumns;
    public DateTimeColumn TimestampColumn => info.TimestampColumn;
    public Column? BatchNumberColumn => info.BatchNumberColumn;
    
    public async Task<int> CountAsync(SampleFilterQuery filterQuery, CancellationToken cancellationToken = default)
    {
        using var csvReader = await CreateCsvReader();

        var count = 0;
        var tsColumnIndex = GetTimestampColumnIndex();
        var resolver = new TimestampResolver(TimestampColumn);
        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;

            var timestamp = resolver.ToDateTime(row[tsColumnIndex]);
            if(!IsMatchFilter(filterQuery, info.TimestampColumn, timestamp, row))
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
        var resolver = new TimestampResolver(TimestampColumn);
        var tsColumnIndex = GetTimestampColumnIndex();
        var indexes = SampleColumns
            .Select(c => info.Table.Columns.Index().First(tc => tc.Item.Name == c.Name).Index)
            .ToArray();

        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;
            
            var timestamp = resolver.ToDateTime(row[tsColumnIndex]);
            if(!IsMatchFilter(filterQuery, info.TimestampColumn, timestamp, row))
                continue;

            var sampleValues = new double[SampleColumns.Count];
            for (int i = 0; i < indexes.Length; i++)
            {
                var index = indexes[i];
                var value = row[index];
                sampleValues[i] = Convert.ToDouble(value);
            }
            
            yield return new Sample(timestamp, sampleValues.AsReadOnly());
        }
    }

    private bool IsMatchFilter(SampleFilterQuery filterQuery, DateTimeColumn infoTimestampColumn, DateTime timestamp, string[] row)
    {
        if(!base.IsMatchFilter(filterQuery, infoTimestampColumn, timestamp))
            return false;

        var batchNumberIndex = GetBatchNumberColumnIndex();
        if(filterQuery.BatchNumber is null || batchNumberIndex is null)
            return true;

        var batchNumber = Convert.ToInt32(row[batchNumberIndex.Value]);
        return batchNumber == filterQuery.BatchNumber;
    }

    private int GetTimestampColumnIndex()
    {
        if (_timestampColumnIndex < 0)
        {
            _timestampColumnIndex = info.Table.Columns.Index().First(tc => tc.Item.Name == info.TimestampColumn.Name).Index;
        }

        return _timestampColumnIndex;
    }

    private int? GetBatchNumberColumnIndex()
    {
        if(info.BatchNumberColumn is null)
            return null;

        if (_batchNumberColumnIndex < 0)
        {
            _batchNumberColumnIndex = info.Table.Columns.Index().First(tc => tc.Item.Name == info.BatchNumberColumn.Name).Index;
        }

        return _batchNumberColumnIndex;
    }
}