namespace AetherSystem.OperationReport.DataSources.Schema;

public sealed class ColumnNameComparer : IComparer<Column>, IEqualityComparer<Column>
{
    public static readonly ColumnNameComparer Instance = new();
    
    private ColumnNameComparer()
    {
    }
    
    public int Compare(Column? x, Column? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        return string.Compare(x?.Name, y?.Name, StringComparison.Ordinal);
    }

    public bool Equals(Column? x, Column? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x?.GetType() != y?.GetType()) return false;
        return string.Equals(x?.Name, y?.Name, StringComparison.Ordinal);
    }

    public int GetHashCode(Column obj)
    {
        return obj.Name.GetHashCode();
    }
}