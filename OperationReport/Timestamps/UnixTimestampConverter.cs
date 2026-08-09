namespace AetherSystem.OperationReport.Timestamps;

public sealed record UnixTimestampConverter : ITimestampConverter
{
    private readonly long _ticksPerUnit;
    private readonly long _offset;
    
    public UnixTimestampConverter(TimestampResolution resolution, TimeSpan offset)
    {
        _ticksPerUnit = TimestampUtils.TicksPerUnit(resolution);
        _offset = offset.Ticks + DateTime.UnixEpoch.Ticks;
    }

    public DateTime ToDateTime(object value)
    {
        var ticks = checked(Convert.ToInt64(value) * _ticksPerUnit + _offset);
        return new DateTime(ticks);
    }

    public object FromDateTime(DateTime value)
    {
        return checked((value.Ticks - _offset) / _ticksPerUnit);
    }
}