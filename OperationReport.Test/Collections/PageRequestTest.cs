using AetherSystem.OperationReport.Collections;

namespace OperationReport.Test.Collections;

public class PageRequestTest
{
    [Fact]
    public void Constructor_ThrowsForInvalidPageOrPageSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PageRequest(0, 100));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PageRequest(1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PageRequest(-1, -1));
    }

    [Fact]
    public void All_ReturnsRequestForAllItems()
    {
        var request = PageRequest.All;

        Assert.Equal(1, request.Page);
        Assert.Equal(int.MaxValue, request.PageSize);
    }

    [Fact]
    public void ComputeRange_ReturnsRequestedPageRange()
    {
        var request = new PageRequest(page: 3, pageSize: 10);

        Assert.Equal((20, 10, 3), request.ComputeRange(totalCount: 35));
    }

    [Fact]
    public void ComputeRange_ReturnsPartialRangeForLastPage()
    {
        var request = new PageRequest(page: 4, pageSize: 10);

        Assert.Equal((30, 5, 4), request.ComputeRange(totalCount: 35));
    }

    [Fact]
    public void ComputeRange_ClampsPageToLastPage()
    {
        var request = new PageRequest(page: 99, pageSize: 10);

        Assert.Equal((30, 5, 4), request.ComputeRange(totalCount: 35));
    }

    [Fact]
    public void ComputeRange_ReturnsEmptyRangeForEmptyCollection()
    {
        var request = new PageRequest(page: 2, pageSize: 10);

        Assert.Equal((0, 0, 1), request.ComputeRange(totalCount: 0));
    }

    [Fact]
    public void ComputeRange_ThrowsForNegativeTotalCount()
    {
        var request = new PageRequest(1, 100);

        Assert.Throws<ArgumentOutOfRangeException>(() => request.ComputeRange(-1));
    }
}
