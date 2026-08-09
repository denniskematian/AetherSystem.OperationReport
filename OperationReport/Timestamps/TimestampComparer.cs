namespace AetherSystem.OperationReport.Timestamps;

public sealed record TimestampComparer : ITimestampComparer
{
    private readonly long _ticksPerUnit;

    public TimestampComparer(TimestampResolution resolution)
    {
        _ticksPerUnit = TimestampUtils.TicksPerUnit(resolution);
    }

    public bool Equals(DateTime x, DateTime y)
    {
        return x.Ticks / _ticksPerUnit == y.Ticks / _ticksPerUnit;
    }

    public int GetHashCode(DateTime value)
    {
        return (value.Ticks / _ticksPerUnit).GetHashCode();
    }

    public int Compare(DateTime x, DateTime y)
    {
        return (x.Ticks / _ticksPerUnit).CompareTo(y.Ticks / _ticksPerUnit);
    }
}