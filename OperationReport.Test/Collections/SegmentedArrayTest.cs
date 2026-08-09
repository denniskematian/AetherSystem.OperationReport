using System.Collections;
using AetherSystem.OperationReport.Collections;

namespace OperationReport.Test.Collections;

public class SegmentedArrayTest
{
    [Fact]
    public void ResizeSegment_CreatesSegmentsWithExpectedSizeAndStorage()
    {
        var array = new SegmentedArray<int>(3);

        array.ResizeSegment(2);
        var segments = array.GetArraySegments();

        Assert.Equal(2, array.SegmentSize);
        Assert.Equal(3, segments.Count);
        Assert.All(segments, segment => Assert.Equal(2, segment.Length));

        segments[0].Span[0] = 10;
        segments[0].Span[1] = 11;
        segments[1].Span[0] = 20;
        segments[1].Span[1] = 21;
        segments[2].Span[0] = 30;
        segments[2].Span[1] = 31;

        Assert.Equal([10, 11], array.GetSegmentReference(0));
        Assert.Equal(10, array.GetSegmentReference(0)[0]);
        Assert.Equal(11, array.GetSegmentReference(0)[1]);
        Assert.Equal([20, 21], array.GetSegmentReference(1));
        Assert.Equal(20, array.GetSegmentReference(1)[0]);
        Assert.Equal(21, array.GetSegmentReference(1)[1]);
        Assert.Equal([30, 31], array.GetSegmentReference(2));
        Assert.Equal(30, array.GetSegmentReference(2)[0]);
        Assert.Equal(31, array.GetSegmentReference(2)[1]);
        
        var transposedSegments = array.GetTransposedSegments([0, 1, 2], 0, 3);
        Assert.Equal([10, 20, 30], transposedSegments[0]);
        Assert.Equal([11, 21, 31], transposedSegments[1]);
        
        Assert.Equal(10, transposedSegments[0][0]);
        Assert.Equal(20, transposedSegments[0][1]);
        Assert.Equal(30, transposedSegments[0][2]);
        Assert.Equal(11, transposedSegments[1][0]);
        Assert.Equal(21, transposedSegments[1][1]);
        Assert.Equal(31, transposedSegments[1][2]);
    }
    
    [Fact]
    public void ResizeSegment_ShouldShiftSegment()
    {
        var array = new SegmentedArray<int>(3);

        array.ResizeSegment(2);
        var segments = array.GetArraySegments();
        segments[0].Span[0] = 10;
        segments[0].Span[1] = 11;
        segments[1].Span[0] = 20;
        segments[1].Span[1] = 21;
        segments[2].Span[0] = 30;
        segments[2].Span[1] = 31;

        array.ResizeSegment(3);
        Assert.Equal([10, 11, 20], array.GetSegmentReference(0));
        Assert.Equal([21, 30, 31], array.GetSegmentReference(1));
        Assert.Equal([0, 0, 0], array.GetSegmentReference(2));
        
        array.ResizeSegment(8);
        Assert.Equal([10, 11, 20, 21, 30, 31, 0, 0], array.GetSegmentReference(0));
        Assert.Equal([0, 0, 0, 0, 0, 0, 0, 0], array.GetSegmentReference(1));
        Assert.Equal([0, 0, 0, 0, 0, 0, 0, 0], array.GetSegmentReference(2));

        array.ResizeSegment(3);
        Assert.Equal([10, 11, 20], array.GetSegmentReference(0));
        Assert.Equal([21, 30, 31], array.GetSegmentReference(1));
        Assert.Equal([0, 0, 0], array.GetSegmentReference(2));
    }
    
    [Fact]
    public void Segment_GetItem_ShouldThrowsOutOfRangeIndex()
    {
        var array = new SegmentedArray<int>(3);

        array.ResizeSegment(2);
        var segments = array.GetArraySegments();

        segments[0].Span[0] = 10;
        segments[0].Span[1] = 11;
        segments[1].Span[0] = 20;
        segments[1].Span[1] = 21;
        segments[2].Span[0] = 30;
        segments[2].Span[1] = 31;

        var segment = array.GetSegmentReference(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => segment[2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => segment[-1]);
    }
    
    [Fact]
    public void TransposedSegment_GetItem_ShouldThrowsOutOfRangeIndex()
    {
        var array = new SegmentedArray<int>(3);

        array.ResizeSegment(2);
        var segments = array.GetArraySegments();

        segments[0].Span[0] = 10;
        segments[0].Span[1] = 11;
        segments[1].Span[0] = 20;
        segments[1].Span[1] = 21;
        segments[2].Span[0] = 30;
        segments[2].Span[1] = 31;

        var transposedSegments = array.GetTransposedSegments([0, 1, 2], 0, 3);
        var segment = transposedSegments[1];
        Assert.Throws<ArgumentOutOfRangeException>(() => segment[3]);
        Assert.Throws<ArgumentOutOfRangeException>(() => segment[-1]);
    }

    [Fact]
    public void GetTransposedSegments_ReturnsValuesByOffsetAcrossSelectedSegments()
    {
        var array = new SegmentedArray<int>(3);
        array.ResizeSegment(3);

        var segments = array.GetArraySegments();
        for (var segment = 0; segment < segments.Count; segment++)
        {
            for (var offset = 0; offset < segments[segment].Length; offset++)
            {
                segments[segment].Span[offset] = segment * 10 + offset;
            }
        }

        var transposed = array.GetTransposedSegments([2, 0, 1], offset: 1, count: 3);

        Assert.Equal(2, transposed.Count);
        Assert.Equal([21, 1, 11], transposed[0]);
        Assert.Equal([22, 2, 12], transposed[1]);
    }

    [Fact]
    public void GetTransposedSegments_DoesNotReturnOffsetsBeyondSegmentSize()
    {
        var array = new SegmentedArray<int>(2);
        array.ResizeSegment(3);

        var transposed = array.GetTransposedSegments([0, 1], offset: 2, count: 5);

        Assert.Single(transposed);
        Assert.Equal(2, transposed[0].Count);
    }

    [Fact]
    public void NonGenericGetEnumerable_MatchGenericEnumeratorType()
    {
        var array = new SegmentedArray<int>(3);
        array.ResizeSegment(3);

        var segment = array.GetSegmentReference(0);
        // ReSharper disable once GenericEnumeratorNotDisposed
        Assert.IsType<IEnumerator<int>>(((IEnumerable)segment).GetEnumerator(), exactMatch: false);
        
        var transposed = array.GetTransposedSegments([0], offset: 0, count: 3);
        // ReSharper disable once GenericEnumeratorNotDisposed
        Assert.IsType<IEnumerator<int>>(((IEnumerable)transposed[0]).GetEnumerator(), exactMatch: false);
    }
}
