namespace AetherSystem.OperationReport.DataSources.Converters;

internal class FractionalUnixTimestampConverter(double ticksPerUnit, TimeSpan offset) : ITimestampConverter<double>
{
    public DateTime ToDateTime(double value)
    {
        var ticks = checked((long)double.Round(value * ticksPerUnit + offset.Ticks));
        return new DateTime(ticks);
    }

    public DateTime ToDateTime(object value)
    {
        return ToDateTime(Convert.ToDouble(value));
    }

    object ITimestampConverter.FromDateTime(DateTime value)
    {
        return FromDateTime(value);
    }

    public double FromDateTime(DateTime value)
    {
        return (value - offset).Ticks / ticksPerUnit;
    }
}