namespace AetherSystem.OperationReport.Timestamps;

public sealed record FractionalUnixTimestampFormat(TimestampResolution Resolution, TimeSpan Offset = default) : ITimestampFormat
{
    public ITimestampComparer Comparer { get; } = new TimestampComparer(TimestampResolution.Microsecond);
    public ITimestampConverter Converter { get; } = new FractionalUnixTimestampConverter(Resolution, Offset);
}