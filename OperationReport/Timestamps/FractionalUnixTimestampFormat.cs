namespace AetherSystem.OperationReport.Timestamps;

public sealed record FractionalUnixTimestampFormat : ITimestampFormat
{
    public ITimestampComparer Comparer { get; } 
    public ITimestampConverter Converter { get; }

    public FractionalUnixTimestampFormat(TimestampResolution resolution, TimeSpan offset)
    {
        Comparer = new TimestampComparer(TimestampResolution.Microsecond);
        Converter = new FractionalUnixTimestampConverter(resolution, offset);
    }
}