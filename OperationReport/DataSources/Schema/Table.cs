namespace AetherSystem.OperationReport.DataSources.Schema;

public record Table
{
    public string Name { get; }
    public IReadOnlyList<Column> Columns { get; }

    public Table(string Name, IReadOnlyList<Column> Columns)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        if(Columns.Count == 0)
            throw new ArgumentException("Columns cannot be empty");
        
        var hashSet = new HashSet<Column>(Columns, ColumnComparer.NameOnly);
        if(hashSet.Count != Columns.Count)
            throw new ArgumentException("Columns cannot contain duplicates");

        this.Name = Name;
        this.Columns = Columns;
    }

    public int IndexOf(Column column)
    {
        for(int i = 0; i < Columns.Count; i++)
        {
            if(Columns[i] == column)
                return i;
        }

        return -1;
    }

    public int IndexOf(string columnName)
    {
        for(int i = 0; i < Columns.Count; i++)
        {
            if(Columns[i].Name == columnName)
                return i;
        }

        return -1;
    }
}