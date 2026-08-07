namespace AetherSystem.OperationReport.DataSources.Converters;

internal class FractionalUnixTimestampConverter(double ticksPerUnit, TimeSpan offset) : ITimestampConverter
{
    public DateTime ToDateTime(IConvertible value)
    {
        var ticks = checked((long)double.Round(Convert.ToDouble(value) * ticksPerUnit + offset.Ticks));
        return new DateTime(ticks);
    }

    public IConvertible FromDateTime(DateTime value)
    {
        return (value - offset).Ticks / ticksPerUnit;
    }
}