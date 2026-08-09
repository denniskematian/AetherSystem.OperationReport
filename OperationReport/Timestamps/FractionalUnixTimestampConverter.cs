namespace AetherSystem.OperationReport.Timestamps;

public sealed record FractionalUnixTimestampConverter : ITimestampConverter
{
    private readonly long _ticksPerUnit;
    private readonly long _offset;
    
    public FractionalUnixTimestampConverter(TimestampResolution resolution, TimeSpan offset)
    {
        _ticksPerUnit = TimestampUtils.TicksPerUnit(resolution);
        _offset = offset.Ticks + DateTime.UnixEpoch.Ticks;
    }

    public DateTime ToDateTime(object value)
    {
        checked
        {
            var ticks = (long)(Convert.ToDouble(value) * _ticksPerUnit) + _offset;
            return new DateTime(ticks);
        }
    }

    public object FromDateTime(DateTime value)
    {
        return checked((value.Ticks - _offset) / (double)_ticksPerUnit);
    }
}