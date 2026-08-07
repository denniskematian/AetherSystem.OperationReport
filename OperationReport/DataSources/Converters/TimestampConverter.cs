using System.Diagnostics;
using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources.Converters;

public interface ITimestampConverter
{
    DateTime ToDateTime(IConvertible value);
    IConvertible FromDateTime(DateTime value);
}

public static class TimestampConverter
{
    public static ITimestampConverter ForColumn(DateTimeColumn column)
    {
        return column.Type switch
        {
            ColumnType.Integer => UnixTimestamp(column.Resolution, column.Offset),
            ColumnType.Real => FractionalUnixTimestamp(column.Resolution, column.Offset),
            ColumnType.Text => StringDateTime("O"),
            _ => throw new UnreachableException(),
        };
    }

    private static UnixTimestampConverter UnixTimestamp(DateTimeResolution resolution, TimeSpan offset)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => new UnixTimestampConverter(TimeSpan.TicksPerMillisecond, offset),
            DateTimeResolution.Seconds => new UnixTimestampConverter(TimeSpan.TicksPerSecond, offset),
            _ => throw new UnreachableException(),
        };
    }

    private static FractionalUnixTimestampConverter FractionalUnixTimestamp(DateTimeResolution resolution, TimeSpan offset)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => new FractionalUnixTimestampConverter(TimeSpan.TicksPerMillisecond, offset),
            DateTimeResolution.Seconds => new FractionalUnixTimestampConverter(TimeSpan.TicksPerSecond, offset),
            _ => throw new UnreachableException(),
        };
    }

    private static Iso8601DateTimeConverter StringDateTime(string format)
    {
        return new Iso8601DateTimeConverter(format);
    }
}