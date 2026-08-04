namespace AetherSystem.OperationReport.Collections;

public record PageRequest
{
    public int Page { get; }
    public int PageSize { get; }
    
    public static PageRequest All => new(1, int.MaxValue);

    public PageRequest(int page, int pageSize = 100)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        
        Page = page;
        PageSize = pageSize;
    }

    public (int Offset, int Length) ComputeRange(int totalCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalCount);
        if (totalCount == 0) return (0, 0);
        
        var maxPage = DivCeil(totalCount, PageSize);
        var page = int.Clamp(Page, 1, maxPage);
        var offset = (page - 1) * PageSize;
        var length = int.Min(PageSize, totalCount - offset);
        return (offset, length);
    }
    
    private static int DivCeil(int a, int b) => a / b + (a % b == 0 ? 0 : 1);
}