using AetherSystem.OperationReport.Internals;

namespace AetherSystem.OperationReport.Timestamps;

internal static class TimestampUtils
{
    public static long TicksPerUnit(TimestampResolution resolution) => resolution switch
    {
        TimestampResolution.HundredNanoseconds => 1,
        TimestampResolution.Microsecond => TimeSpan.TicksPerMicrosecond,
        TimestampResolution.Millisecond => TimeSpan.TicksPerMillisecond,
        TimestampResolution.Second => TimeSpan.TicksPerSecond,
        TimestampResolution.Minute => TimeSpan.TicksPerMinute,
        TimestampResolution.Hour => TimeSpan.TicksPerHour,
        TimestampResolution.Day => TimeSpan.TicksPerDay,
        _ => ExceptionUtils.ThrowInvalidEnumArgument<long>(resolution)
    };
}