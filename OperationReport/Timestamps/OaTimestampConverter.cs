namespace AetherSystem.OperationReport.Timestamps;

public sealed record OaTimestampConverter : ITimestampConverter
{
    public DateTime ToDateTime(object value)
    {
        return DateTime.FromOADate(Convert.ToDouble(value));
    }

    public object FromDateTime(DateTime value)
    {
        return value.ToOADate();
    }
}