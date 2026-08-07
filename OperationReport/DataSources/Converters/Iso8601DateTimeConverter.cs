namespace AetherSystem.OperationReport.DataSources.Converters;

internal class Iso8601DateTimeConverter(string format) : ITimestampConverter
{
    public DateTime ToDateTime(IConvertible value)
    {
        return DateTime.ParseExact(Convert.ToString(value)!, format, null);
    }

    public IConvertible FromDateTime(DateTime value)
    {
        return value.ToString(format);
    }
}