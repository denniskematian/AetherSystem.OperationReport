using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.Collections;

public sealed class DataCollector
{
    private readonly ISampleTableAdapter _sampleTableAdapter;
    private readonly IOperationTableAdapter _operationTableAdapter;
    private readonly Dictionary<string, int> _sampleColumnIndexLookup;

    private readonly SegmentedArray<double> _sampleArrayPool;
    private readonly SegmentedArray<double> _operationArrayPool;
    private readonly List<string> _operationComments = [];
    
    private readonly SemaphoreSlim _lock = new(1, 1);
    
    public IReadOnlyList<double> SampleTimestamps => _sampleArrayPool.GetSegmentReference(0);
    public IReadOnlyList<double> OperationTimestamps => _operationArrayPool.GetSegmentReference(0);

    public DataCollector(
        ISampleTableAdapter sampleTableAdapter,
        IOperationTableAdapter operationTableAdapter)
    {
        _sampleTableAdapter = sampleTableAdapter;
        _operationTableAdapter = operationTableAdapter;
        
        var segmentCount = sampleTableAdapter.SampleColumns.Count + 1;
        _sampleArrayPool = new SegmentedArray<double>(segmentCount);
        _operationArrayPool = new SegmentedArray<double>(segmentCount);
        
        _sampleColumnIndexLookup = _sampleTableAdapter.SampleColumns.Index()
            .ToDictionary(c => c.Item.Name, c => c.Index + 1);
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

    public PageResult<Sample> GetSamplePage(PageRequest request)
    {
        using (LockHandle.Lock(_lock))
        {
            var totalCount = _sampleArrayPool.SegmentSize;
            var (offset, length, page) = request.ComputeRange(totalCount);
            if (length == 0)
                return new PageResult<Sample>(request.PageSize);

            var samples = new Sample[length];
            var timestampSegment = _sampleArrayPool.GetSegmentReference(0);
            var indexes = Enumerable.Range(1, _sampleTableAdapter.SampleColumns.Count).ToArray();

            var transposedSegments = _sampleArrayPool
                .GetTransposedSegments(indexes, offset, length);

            for (int i = 0; i < length; i++)
            {
                var timestamp = DateTime.FromOADate(timestampSegment[offset + i]);
                samples[i] = new Sample(timestamp, transposedSegments[i]);
            }

            return new PageResult<Sample>(page, request.PageSize, totalCount, samples);
        }
    }

    public PageResult<OperationSample> GetOperationPage(PageRequest request, IEnumerable<int> indexes)
    {
        using (LockHandle.Lock(_lock))
        {
            var totalCount = _operationArrayPool.SegmentSize;
            var (offset, length, page) = request.ComputeRange(totalCount);
            if (length == 0)
                return new PageResult<OperationSample>(request.PageSize);

            var operations = new OperationSample[length];
            var timestampSegment = _operationArrayPool.GetSegmentReference(0);
            var indexesArray = indexes.Select(i => i + 1).ToArray();
            var transposedSegments = _operationArrayPool
                .GetTransposedSegments(indexesArray, offset, length);

            for (int i = 0; i < length; i++)
            {
                var timestamp = DateTime.FromOADate(timestampSegment[offset + i]);
                var comment = _operationComments[offset + i];
                operations[i] = new OperationSample(timestamp, comment, transposedSegments[i]);
            }

            return new PageResult<OperationSample>(page, request.PageSize, totalCount, operations);
        }
    }

    public async Task UpdateDataAsync(SampleFilterQuery sampleFilter, CancellationToken cancellationToken = default)
    {
        using (await LockHandle.LockAsync(_lock, cancellationToken))
        {
            await UpdateSampleDataAsync(sampleFilter, cancellationToken);
            await UpdateOperationDataAsync(sampleFilter, cancellationToken);
        }
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
        _operationComments.Clear();
        if(count == 0)
            return;

        var index = 0;
        var segments = _operationArrayPool.GetArraySegments();
        var buffer = new double[_sampleColumnIndexLookup.Count];
        await foreach (var operation in _operationTableAdapter.EnumerateAsync(filterQuery, cancellationToken))
        {
            var oaTimestamp = operation.Timestamp.ToOADate();
            _operationComments.Add(operation.Comment);
            GetSampleValuesAt(oaTimestamp, buffer);
            segments[0].Span[index] = oaTimestamp;
            for (int i = 0; i < buffer.Length; i++)
            {
                segments[i + 1].Span[index] = buffer[i];
            }

            index++;
        }
    }

    private void GetSampleValuesAt(double oaTimestamp, Span<double> buffer)
    {
        var segments = _sampleArrayPool.GetArraySegments();

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
        for (int i = 0; i < buffer.Length; i++)
        {
            var segment = segments[i + 1].Span;
            buffer[i] = double.Lerp(segment[index - 1], segment[index], weight);
        }
    }

    private class LockHandle(SemaphoreSlim semaphore) : IDisposable
    {
        public static async Task<LockHandle> LockAsync(SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            return new LockHandle(semaphore);
        }

        public static LockHandle Lock(SemaphoreSlim semaphore)
        {
            semaphore.Wait();
            return new LockHandle(semaphore);
        }

        public void Dispose() => semaphore.Release();
    }
        
}