using System.ComponentModel;

namespace AetherSystem.OperationReport.DataSources.Schema;

public class TimestampResolver(DateTimeColumn column)
{
    public DateTime ToDateTime(object value)
    {
        return column.Type switch
        {
            ColumnType.Text => Convert.ToDateTime(value),
            ColumnType.Real => RealToDateTime(column.Resolution, Convert.ToDouble(value)),
            ColumnType.Integer => IntegerToDateTime(column.Resolution, Convert.ToInt64(value)),
            // dotcover disable
            _ => throw new InvalidEnumArgumentException($"Invalid ColumnType {(int)column.Type}."),
            // dotcover enable
        };
    }

    public long ToUnixTimestamp(DateTimeOffset value)
    {
        value = value.Subtract(column.Offset);
        return column.Resolution switch
        {
            DateTimeResolution.Milliseconds => value.ToUnixTimeMilliseconds(),
            DateTimeResolution.Seconds => value.ToUnixTimeSeconds(),
            // dotcover disable
            DateTimeResolution.Unspecified => throw new InvalidEnumArgumentException("Unspecified DateTimeResolution."),
            _ => throw new InvalidEnumArgumentException($"Invalid DateTimeResolution {(int)column.Resolution}.")
            // dotcover enable
        };
    }

    public static DateTime RealToDateTime(DateTimeResolution resolution, double value)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => FromFractionalMilliseconds(value),
            DateTimeResolution.Seconds => FromFractionalSeconds(value),
            DateTimeResolution.Unspecified => throw new InvalidEnumArgumentException("Unspecified DateTimeResolution."),
            _ => throw new InvalidEnumArgumentException($"Invalid DateTimeResolution {(int)resolution}.")
        };
    }

    public static DateTime IntegerToDateTime(DateTimeResolution resolution, long value)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => DateTimeOffset.FromUnixTimeMilliseconds(value).UtcDateTime,
            DateTimeResolution.Seconds => DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime,
            DateTimeResolution.Unspecified => throw new InvalidEnumArgumentException("Unspecified DateTimeResolution."),
            _ => throw new InvalidEnumArgumentException($"Invalid DateTimeResolution {(int)resolution}.")
        };
    }

    private static DateTime FromFractionalSeconds(double unixSeconds)
    {
        var ticks = checked((long)double.Round(unixSeconds * TimeSpan.TicksPerSecond));
        return new DateTime(ticks);
    }

    private static DateTime FromFractionalMilliseconds(double unixMilliseconds)
    {
        var ticks = checked((long)double.Round(unixMilliseconds * TimeSpan.TicksPerMillisecond));
        return new DateTime(ticks);
    }
}