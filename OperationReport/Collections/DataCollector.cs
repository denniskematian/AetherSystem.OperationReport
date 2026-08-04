using AetherSystem.OperationReport.DataSources;

namespace AetherSystem.OperationReport.Collections;

public sealed class DataCollector
{
    private readonly ISampleTableAdapter _sampleTableAdapter;
    private readonly IOperationTableAdapter _operationTableAdapter;
    private readonly Dictionary<string, int> _sampleColumnIndexLookup;

    private readonly SegmentedArrayPool<double> _sampleArrayPool;
    private readonly SegmentedArrayPool<double> _operationArrayPool;

    public DataCollector(
        ISampleTableAdapter sampleTableAdapter,
        IOperationTableAdapter operationTableAdapter)
    {
        _sampleTableAdapter = sampleTableAdapter;
        _operationTableAdapter = operationTableAdapter;
        
        var segmentCount = sampleTableAdapter.SampleColumns.Count + 1;
        _sampleArrayPool = new SegmentedArrayPool<double>(segmentCount);
        _operationArrayPool = new SegmentedArrayPool<double>(segmentCount);
        
        _sampleColumnIndexLookup = _sampleTableAdapter.SampleColumns.Index()
            .ToDictionary(c => c.Item.Name, c => c.Index);
    }

    public IReadOnlyList<double> GetSampleDataSource(string column)
    {
        var index = _sampleColumnIndexLookup[column];
        return _sampleArrayPool.GetSegmentReference(index);
    }

    public IReadOnlyList<double> GetOperationDataSource(string column)
    {
        var index = _sampleColumnIndexLookup[column];
        return _operationArrayPool.GetSegmentReference(index);
    }

    public async Task UpdateDataAsync(SampleFilterQuery sampleFilter, CancellationToken cancellationToken = default)
    {
        await UpdateSampleDataAsync(sampleFilter, cancellationToken);
        await UpdateOperationDataAsync(sampleFilter, cancellationToken);
    }

    private async Task UpdateSampleDataAsync(SampleFilterQuery sampleFilter, CancellationToken cancellationToken = default)
    {
        var count = await _sampleTableAdapter.CountAsync(sampleFilter, cancellationToken);
        _sampleArrayPool.ResizeSegment(count);
        if(count == 0)
            return;
        
        var index = 0;
        var segments = _sampleArrayPool.GetArraySegments();
        await foreach (var sample in _sampleTableAdapter.EnumerateAsync(sampleFilter, cancellationToken))
        {
            segments[0].Span[index] = sample.Timestamp.ToOADate();
            for (int i = 0; i < sample.Values.Count; i++)
            {
                segments[i + 1].Span[index] = sample.Values[i];
            }
        
            index++;
        }
    }

    private async Task UpdateOperationDataAsync(FilterQuery filterQuery, CancellationToken cancellationToken = default)
    {
        var count = await _operationTableAdapter.CountAsync(filterQuery, cancellationToken);
        _operationArrayPool.ResizeSegment(count);
        if(count == 0)
            return;

        var index = 0;
        var segments = _operationArrayPool.GetArraySegments();
        var sample = new double[_sampleColumnIndexLookup.Count];
        await foreach (var operation in _operationTableAdapter.EnumerateAsync(filterQuery, cancellationToken))
        {
            var oaTimestamp = operation.Timestamp.ToOADate();
            GetSampleValuesAt(oaTimestamp, sample);
            segments[0].Span[index] = oaTimestamp;
            for (int i = 0; i < sample.Length; i++)
            {
                segments[i + 1].Span[index] = sample[i];
            }

            index++;
        }
    }

    private void GetSampleValuesAt(double oaTimestamp, Span<double> buffer)
    {
        var segments = _sampleArrayPool.GetArraySegments();
        Span<double> result = stackalloc double[_sampleColumnIndexLookup.Count];

        var timestampSegment = segments[0].Span;
        var index = timestampSegment.BinarySearch(oaTimestamp);
        if (index >= 0)
        {
            for(int i = 0; i < _sampleColumnIndexLookup.Count; i++)
            {
                buffer[i] = segments[i + 1].Span[index];
            }

            return;
        }

        index = ~index;
        if (index == 0 || index == timestampSegment.Length)
            return;

        var lower = timestampSegment[index - 1];
        var upper = timestampSegment[index];
        var weight = (oaTimestamp - lower) / (upper - lower);
        for (int i = 0; i < result.Length; i++)
        {
            var segment = segments[i + 1].Span;
            result[i] = double.Lerp(segment[index - 1], segment[index], weight);
        }
    }
}