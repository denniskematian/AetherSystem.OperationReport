namespace AetherSystem.OperationReport.Timestamps;

public sealed record StringTimestampFormat : ITimestampFormat
{
    public ITimestampComparer Comparer { get; } = new TimestampComparer(TimestampResolution.HundredNanoseconds);
    public ITimestampConverter Converter { get; }
    
    public StringTimestampFormat(string format)
    {
        Converter = new StringTimestampConverter(format);
    }
}