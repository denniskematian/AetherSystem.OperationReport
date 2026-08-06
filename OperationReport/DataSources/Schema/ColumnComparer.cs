namespace AetherSystem.OperationReport.DataSources.Schema;

public interface IColumnComparer : IEqualityComparer<Column>, IComparer<Column>;

public static class ColumnComparer
{
    public static readonly IColumnComparer NameAndType = new BothComparer();
    public static readonly IColumnComparer NameOnly = new NameComparer();

    private class BothComparer : IColumnComparer
    {
        public bool Equals(Column? x, Column? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            return string.Equals(x.Name, y.Name, StringComparison.Ordinal) && x.Type == y.Type;
        }

        public int GetHashCode(Column obj)
        {
            return HashCode.Combine(
                string.GetHashCode(obj.Name, StringComparison.Ordinal), 
                (int)obj.Type);
        }

        public int Compare(Column? x, Column? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (y is null) return 1;
            if (x is null) return -1;
            var nameComparison = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (nameComparison != 0) return nameComparison;
            return x.Type.CompareTo(y.Type);
        }
    }

    private class NameComparer : IColumnComparer
    {
        public bool Equals(Column? x, Column? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            return string.Equals(x.Name, y.Name, StringComparison.Ordinal);
        }

        public int GetHashCode(Column obj)
        {
            return string.GetHashCode(obj.Name, StringComparison.Ordinal);
        }

        public int Compare(Column? x, Column? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (y is null) return 1;
            if (x is null) return -1;
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
}