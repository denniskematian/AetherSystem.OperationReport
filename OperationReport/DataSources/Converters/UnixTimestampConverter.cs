namespace AetherSystem.OperationReport.DataSources.Converters;

internal class UnixTimestampConverter(long ticksPerUnit, TimeSpan offset) : ITimestampConverter
{
    public DateTime ToDateTime(IConvertible value)
    {
        var ticks = checked(Convert.ToInt64(value) * ticksPerUnit + offset.Ticks);
        return new DateTime(ticks);
    }

    public IConvertible FromDateTime(DateTime value)
    {
        return (value - offset).Ticks / ticksPerUnit;
    }
}