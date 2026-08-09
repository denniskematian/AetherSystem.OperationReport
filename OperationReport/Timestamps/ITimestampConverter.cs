namespace AetherSystem.OperationReport.Timestamps;

public interface ITimestampConverter
{
    DateTime ToDateTime(object value);
    object FromDateTime(DateTime value);
}