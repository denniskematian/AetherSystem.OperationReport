namespace AetherSystem.OperationReport.Timestamps;

public sealed record OaTimestampFormat : ITimestampFormat
{
    public ITimestampComparer Comparer { get; } = new TimestampComparer(TimestampResolution.Millisecond);
    public ITimestampConverter Converter { get; } = new OaTimestampConverter();
}