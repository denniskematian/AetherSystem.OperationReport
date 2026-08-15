using System.Diagnostics.CodeAnalysis;

namespace AetherSystem.OperationReport.Timestamps;

public sealed record StringTimestampFormat([StringSyntax("DateTimeFormat")] string Format = "O") : ITimestampFormat
{
    public ITimestampComparer Comparer { get; } = new TimestampComparer(TimestampResolution.HundredNanoseconds);
    public ITimestampConverter Converter { get; } = new StringTimestampConverter(Format);
}