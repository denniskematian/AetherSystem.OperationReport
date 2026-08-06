using System.ComponentModel;
using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources.Converters;

public interface ITimestampConverter
{
    DateTime ToDateTime(object value);
    object FromDateTime(DateTime value);
}

public interface ITimestampConverter<T> : ITimestampConverter
{
    DateTime ToDateTime(T value);
    new T FromDateTime(DateTime value);
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
            _ => throw new InvalidEnumArgumentException($"Invalid column type ({(int)column.Type}).")
        };
    }

    private static UnixTimestampConverter UnixTimestamp(DateTimeResolution resolution, TimeSpan offset)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => new UnixTimestampConverter(TimeSpan.TicksPerMillisecond, offset),
            DateTimeResolution.Seconds => new UnixTimestampConverter(TimeSpan.TicksPerSecond, offset),
            DateTimeResolution.Unspecified => throw new NotSupportedException($"Unsupported date time resolution for {resolution}."),
            _ => throw new InvalidEnumArgumentException($"Invalid date time resolution ({(int)resolution}).")
        };
    }

    private static FractionalUnixTimestampConverter FractionalUnixTimestamp(DateTimeResolution resolution, TimeSpan offset)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => new FractionalUnixTimestampConverter(TimeSpan.TicksPerMillisecond, offset),
            DateTimeResolution.Seconds => new FractionalUnixTimestampConverter(TimeSpan.TicksPerSecond, offset),
            DateTimeResolution.Unspecified => throw new NotSupportedException($"Unsupported date time resolution for {resolution}."),
            _ => throw new InvalidEnumArgumentException($"Invalid date time resolution ({(int)resolution}).")
        };
    }

    private static Iso8601DateTimeConverter StringDateTime(string format)
    {
        return new Iso8601DateTimeConverter(format);
    }
}