namespace AetherSystem.OperationReport.DataSources.Converters;

internal class UnixTimestampConverter(long ticksPerUnit, TimeSpan offset) : ITimestampConverter<long>
{
    public DateTime ToDateTime(long value)
    {
        var ticks = checked(value * ticksPerUnit + offset.Ticks);
        return new DateTime(ticks);
    }

    public DateTime ToDateTime(object value)
    {
        return ToDateTime(Convert.ToInt64(value));
    }

    object ITimestampConverter.FromDateTime(DateTime value)
    {
        return FromDateTime(value);
    }

    public long FromDateTime(DateTime value)
    {
        return (value - offset).Ticks / ticksPerUnit;
    }
}