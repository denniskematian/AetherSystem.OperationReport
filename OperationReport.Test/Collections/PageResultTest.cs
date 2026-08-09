using AetherSystem.OperationReport.Collections;

namespace OperationReport.Test.Collections;

public class PageResultTest
{
    [Fact]
    public void Constructor_PreservesPageMetadataAndItems()
    {
        var items = new[] { "first", "second" };

        var result = new PageResult<string>(page: 2, pageSize: 10, totalCount: 25, items);

        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(items, result.Items);
    }

    [Fact]
    public void EmptyConstructor_CreatesEmptyFirstPage()
    {
        var result = new PageResult<int>(pageSize: 20);

        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public void Constructor_WithNullItems_ExposesEmptyItems()
    {
        var result = new PageResult<int>(1, 10, 0, items: null!);

        Assert.Empty(result.Items);
    }
}
