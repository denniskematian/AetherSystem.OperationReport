namespace AetherSystem.OperationReport.DataSources.Converters;

internal class Iso8601DateTimeConverter(string format) : ITimestampConverter<string>
{
    public DateTime ToDateTime(string value)
    {
        return DateTime.ParseExact(value, format, null);
    }

    public DateTime ToDateTime(object value)
    {
        return ToDateTime(Convert.ToString(value)!);
    }

    object ITimestampConverter.FromDateTime(DateTime value)
    {
        return FromDateTime(value);
    }

    public string FromDateTime(DateTime value)
    {
        return value.ToString(format);
    }
}