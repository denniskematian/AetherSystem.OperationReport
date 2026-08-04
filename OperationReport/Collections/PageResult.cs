namespace AetherSystem.OperationReport.Collections;

public sealed class PageResult<T>
{
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public IReadOnlyList<T> Items => field ?? [];
    
    public PageResult(int page, int pageSize, int totalCount, IReadOnlyList<T> items)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items;
    }
    
    public PageResult(int pageSize)
    {
        Page = 1;
        PageSize = pageSize;
        TotalCount = 0;
    }
}
