namespace AetherSystem.OperationReport.DataSources.Schema;

public record Column
{
    public string Name { get; }
    public ColumnType Type { get; }
    
    public Column(string Name, ColumnType Type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        if(!Enum.IsDefined(Type))
            throw new ArgumentException($"ColumnType ({(int)Type}) is not defined.", nameof(Type));

        this.Name = Name;
        this.Type = Type;
    }
}