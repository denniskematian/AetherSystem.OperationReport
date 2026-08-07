using System.ComponentModel;
using AetherSystem.OperationReport.Internals;

namespace AetherSystem.OperationReport.DataSources.Schema;

public record DateTimeColumn : Column
{
    public DateTimeResolution Resolution { get; }
    public TimeSpan Offset { get; }
    
    public DateTimeColumn(
        string name,
        ColumnType type,
        DateTimeResolution resolution = DateTimeResolution.Unspecified,
        TimeSpan offset = default) : base(name, type)
    {
        ExceptionUtils.ThrowIfUndefined(resolution);
        if(type is ColumnType.Real or ColumnType.Integer && resolution is not (DateTimeResolution.Milliseconds or DateTimeResolution.Seconds))
            throw new ArgumentException($"DateTimeResolution must Real or Integer for {type} column.", nameof(resolution));

        Resolution = resolution;
        Offset = offset;
    }
}