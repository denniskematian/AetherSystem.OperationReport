using AetherSystem.OperationReport.Internals;

namespace AetherSystem.OperationReport.DataSources.Schema;

public record Table
{
    public string Name { get; }
    public IReadOnlyList<Column> Columns { get; }

    public Table(string name, IReadOnlyList<Column> columns)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ExceptionUtils.ThrowIfContainsNull(columns);
        if(columns.Count == 0)
            throw new ArgumentException("Columns cannot be empty");
        
        var hashSet = new HashSet<Column>(columns, ColumnComparer.NameOnly);
        if(hashSet.Count != columns.Count)
            throw new ArgumentException("Columns cannot contain duplicates");

        Name = name;
        Columns = columns;
    }

    public int IndexOf(Column column)
    {
        for(int i = 0; i < Columns.Count; i++)
        {
            if(ColumnComparer.NameAndType.Equals(column, Columns[i]))
                return i;
        }

        return -1;
    }

    public int IndexOf(string columnName)
    {
        for(int i = 0; i < Columns.Count; i++)
        {
            if(string.Equals(columnName, Columns[i].Name, StringComparison.Ordinal))
                return i;
        }

        return -1;
    }
}