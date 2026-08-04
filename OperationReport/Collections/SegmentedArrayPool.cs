using System.Collections;

namespace AetherSystem.OperationReport.Collections;


public class SegmentedArrayPool<T> where T : unmanaged
{
    private readonly int _segmentCount;
    private readonly ArrayView[] _segments;
    private int _segmentSize;
    private T[] _pool = [];

    public SegmentedArrayPool(int segmentCount)
    {
        _segmentCount = segmentCount;
        _segments = new ArrayView[_segmentCount];
        for (int i = 0; i < _segmentCount; i++)
        {
            _segments[i] = new ArrayView(this);
        }
    }

    public IReadOnlyList<Memory<T>> GetArraySegments()
    {
        return [.._segments.Select(s => s.GetArraySegment())];
    }

    public IReadOnlyList<T> GetSegmentReference(int segment)
    {
        return _segments[segment];
    }

    public void ResizeSegment(int segmentSize)
    {
        EnsureCapacity(segmentSize * _segmentCount);
        _segmentSize = segmentSize;
        for (int i = 0; i < _segmentCount; i++)
        {
            var segment = _segments[i];
            segment.Count = _segmentSize;
            segment.Offset = i * _segmentSize;
        }
    }
    
    private void EnsureCapacity(int capacity)
    {
        if(_pool.Length >= capacity)
            return;

        if(!int.IsPow2(capacity))
            capacity = 2 << int.Log2(capacity);

        _pool = new T[capacity];
    }

    private class ArrayView(SegmentedArrayPool<T> parent) : IReadOnlyList<T>
    {
        public int Count { get; set; }
        public int Offset { get; set; }

        public ArraySegment<T> GetArraySegment()
        {
            return new ArraySegment<T>(parent._pool, Offset, Count);
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var segment = GetArraySegment();
            return segment.GetEnumerator();
        }

        public T this[int index] => parent._pool[Offset + index];
    }
}