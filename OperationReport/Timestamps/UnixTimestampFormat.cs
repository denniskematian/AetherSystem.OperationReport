namespace AetherSystem.OperationReport.Timestamps;

public sealed record UnixTimestampFormat(TimestampResolution Resolution, TimeSpan Offset = default) : ITimestampFormat
{
    public ITimestampComparer Comparer { get; } = new TimestampComparer(Resolution);
    public ITimestampConverter Converter { get; } = new UnixTimestampConverter(Resolution, Offset);
}