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
        
        var hashSet = new HashSet<Column>(Columns, ColumnNameComparer.Instance);
        if(hashSet.Count != Columns.Count)
            throw new ArgumentException("Columns cannot contain duplicates");

        this.Name = Name;
        this.Columns = Columns;
    }
}