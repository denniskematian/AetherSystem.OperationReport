using System.Diagnostics.CodeAnalysis;

namespace AetherSystem.OperationReport.Timestamps;

public sealed record StringTimestampFormat : ITimestampFormat
{
    public ITimestampComparer Comparer { get; } = new TimestampComparer(TimestampResolution.HundredNanoseconds);
    public ITimestampConverter Converter { get; }
    
    public StringTimestampFormat([StringSyntax("DateTimeFormat")] string format = "O")
    {
        Converter = new StringTimestampConverter(format);
    }
}