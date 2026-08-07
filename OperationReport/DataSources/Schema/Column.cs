using AetherSystem.OperationReport.Internals;

namespace AetherSystem.OperationReport.DataSources.Schema;

public record Column
{
    public string Name { get; }
    public ColumnType Type { get; }
    
    public Column(string name, ColumnType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ExceptionUtils.ThrowIfUndefined(type);

        Name = name;
        Type = type;
    }
}