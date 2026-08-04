namespace AetherSystem.OperationReport.DataSources.Schema;

public class TimestampResolver(DateTimeColumn column)
{
    public DateTime ToDateTime(object value)
    {
        return column.Type switch
        {
            ColumnType.Text => Convert.ToDateTime(value),
            ColumnType.Real => RealToDateTime(Convert.ToDouble(value)),
            ColumnType.Integer => IntegerToDateTime(Convert.ToInt64(value)),
            _ => throw new NotSupportedException("Unsupported column type."),
        };
    }

    public long ToUnixTimestamp(DateTimeOffset value)
    {
        value = value.Subtract(column.Offset);
        return column.Resolution switch
        {
            DateTimeResolution.Milliseconds => value.ToUnixTimeMilliseconds(),
            DateTimeResolution.Seconds => value.ToUnixTimeSeconds(),
            DateTimeResolution.Unspecified => throw new NotSupportedException("Unspecified datetime resolution."),
            _ => throw new NotSupportedException($"Unsupported datetime resolution {column.Resolution}.")
        };
    }

    private DateTime RealToDateTime(double value)
    {
        return column.Resolution switch
        {
            DateTimeResolution.Milliseconds => FromFractionalMilliseconds(value),
            DateTimeResolution.Seconds => FromFractionalSeconds(value),
            DateTimeResolution.Unspecified => throw new NotSupportedException("Unspecified datetime resolution."),
            _ => throw new NotSupportedException($"Unsupported datetime resolution {column.Resolution}.")
        };
    }

    private DateTime IntegerToDateTime(long value)
    {
        return column.Resolution switch
        {
            DateTimeResolution.Milliseconds => DateTimeOffset.FromUnixTimeMilliseconds(value).UtcDateTime,
            DateTimeResolution.Seconds => DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime,
            DateTimeResolution.Unspecified => throw new NotSupportedException("Unspecified datetime resolution."),
            _ => throw new NotSupportedException($"Unsupported datetime resolution {column.Resolution}.")
        };
    }

    private static DateTime FromFractionalSeconds(double unixSeconds)
    {
        var whole = (long)unixSeconds;
        var frac = unixSeconds - whole;

        var dto = DateTimeOffset.FromUnixTimeSeconds(whole);

        var ticks = (long)(frac * TimeSpan.TicksPerSecond);

        return dto.AddTicks(ticks).UtcDateTime;
    }

    private static DateTime FromFractionalMilliseconds(double unixMilliseconds)
    {
        var whole = (long)unixMilliseconds;
        var frac = unixMilliseconds - whole;

        var dto = DateTimeOffset.FromUnixTimeMilliseconds(whole);

        var ticks = (long)(frac * TimeSpan.TicksPerMillisecond);

        return dto.AddTicks(ticks).UtcDateTime;
    }
}