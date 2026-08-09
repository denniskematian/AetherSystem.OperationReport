namespace AetherSystem.OperationReport.Timestamps;

public sealed record UnixTimestampFormat : ITimestampFormat
{
    public ITimestampComparer Comparer { get; }
    public ITimestampConverter Converter { get; }
    
    public UnixTimestampFormat(TimestampResolution resolution, TimeSpan offset = default)
    {
        Comparer = new TimestampComparer(resolution);
        Converter = new UnixTimestampConverter(resolution, offset);
    }
}