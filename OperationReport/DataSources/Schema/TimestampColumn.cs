using AetherSystem.OperationReport.Timestamps;

namespace AetherSystem.OperationReport.DataSources.Schema;

public record TimestampColumn : Column
{
    public ITimestampFormat Format { get; }

    public TimestampColumn(string name, ColumnType type, ITimestampFormat format) : base(name, type)
    {
        ArgumentNullException.ThrowIfNull(format);
        Format = format;
    }
    
    public static TimestampColumn Create(Column baseColumn, ITimestampFormat format)
    {
        ArgumentNullException.ThrowIfNull(baseColumn);
        return new TimestampColumn(baseColumn.Name, baseColumn.Type, format);
    }
}