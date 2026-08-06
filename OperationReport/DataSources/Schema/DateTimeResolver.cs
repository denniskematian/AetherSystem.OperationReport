using System.ComponentModel;

namespace AetherSystem.OperationReport.DataSources.Schema;

public static class DateTimeResolver
{
    public static string ToDateTimeString(DateTime dateTime)
    {
        return dateTime.ToString("O");
    }

    public static long ToUnixTime(DateTime dateTime, DateTimeResolution resolution)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => dateTime.Ticks / TimeSpan.TicksPerMillisecond,
            DateTimeResolution.Seconds => dateTime.Ticks / TimeSpan.TicksPerSecond,
            DateTimeResolution.Unspecified => throw new InvalidEnumArgumentException("Invalid DateTimeResolution value (Unspecified)."),
            _ => throw new InvalidEnumArgumentException($"Invalid DateTimeResolution value ({(int)resolution}).")
        };
    }

    public static double ToFractionalUnixTime(DateTime dateTime, DateTimeResolution resolution)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => (double)dateTime.Ticks / TimeSpan.TicksPerMillisecond,
            DateTimeResolution.Seconds => (double)dateTime.Ticks / TimeSpan.TicksPerSecond,
            DateTimeResolution.Unspecified => throw new InvalidEnumArgumentException("Invalid DateTimeResolution value (Unspecified)."),
            _ => throw new InvalidEnumArgumentException($"Invalid DateTimeResolution value ({(int)resolution}).")
        };
    }
    
    public static DateTime ToDateTime(string dateTimeString)
    {
        return DateTime.Parse(dateTimeString);
    }

    public static DateTime ToDateTime(long unixTime, DateTimeResolution resolution)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => FromMilliseconds(unixTime),
            DateTimeResolution.Seconds => FromSeconds(unixTime),
            DateTimeResolution.Unspecified => throw new InvalidEnumArgumentException("Invalid DateTimeResolution value (Unspecified)."),
            _ => throw new InvalidEnumArgumentException($"Invalid DateTimeResolution value ({(int)resolution}).")
        };
    }

    public static DateTime ToDateTime(double unixTime, DateTimeResolution resolution)
    {
        return resolution switch
        {
            DateTimeResolution.Milliseconds => FromMilliseconds(unixTime),
            DateTimeResolution.Seconds => FromSeconds(unixTime),
            DateTimeResolution.Unspecified => throw new InvalidEnumArgumentException("Invalid DateTimeResolution value (Unspecified)."),
            _ => throw new InvalidEnumArgumentException($"Invalid DateTimeResolution value ({(int)resolution}).")
        };
    }
    
    private static DateTime FromSeconds(double unixSeconds)
    {
        var ticks = checked((long)double.Round(unixSeconds * TimeSpan.TicksPerSecond));
        return new DateTime(ticks);
    }

    private static DateTime FromSeconds(long unixSeconds)
    {
        var ticks = checked(unixSeconds * TimeSpan.TicksPerSecond);
        return new DateTime(ticks);
    }

    private static DateTime FromMilliseconds(double unixMilliseconds)
    {
        var ticks = checked((long)double.Round(unixMilliseconds * TimeSpan.TicksPerMillisecond));
        return new DateTime(ticks);
    }

    private static DateTime FromMilliseconds(long unixMilliseconds)
    {
        var ticks = checked(unixMilliseconds * TimeSpan.TicksPerMillisecond);
        return new DateTime(ticks);
    }
}