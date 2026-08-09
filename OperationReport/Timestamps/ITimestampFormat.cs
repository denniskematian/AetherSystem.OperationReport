namespace AetherSystem.OperationReport.Timestamps;

public interface ITimestampFormat
{
    ITimestampComparer Comparer { get; }
    ITimestampConverter Converter { get; }
}
