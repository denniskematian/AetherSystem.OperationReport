using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AetherSystem.OperationReport.Timestamps;

public sealed record StringTimestampConverter : ITimestampConverter
{
    private readonly string _format;

    public StringTimestampConverter([StringSyntax("DateTimeFormat")] string format = "O")
    {
        _format = format;
    }

    public DateTime ToDateTime(object value)
    {
        var str = Convert.ToString(value);
        ArgumentNullException.ThrowIfNull(str);
        return DateTime.ParseExact(str, _format, DateTimeFormatInfo.InvariantInfo);
    }

    public object FromDateTime(DateTime value)
    {
        return value.ToString(_format, DateTimeFormatInfo.InvariantInfo);
    }
}