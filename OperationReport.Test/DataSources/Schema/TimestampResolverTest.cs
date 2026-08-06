using AetherSystem.OperationReport.DataSources.Schema;

namespace OperationReport.Test.DataSources.Schema;

public class TimestampResolverTest
{
    private readonly DateTimeColumn[] _columns = [
        new("column", ColumnType.Integer, DateTimeResolution.Milliseconds),
        new("column", ColumnType.Real, DateTimeResolution.Milliseconds),
        new("column", ColumnType.Integer, DateTimeResolution.Seconds),
        new("column", ColumnType.Real, DateTimeResolution.Seconds),
        new("column", ColumnType.Integer, DateTimeResolution.Milliseconds, TimeSpan.FromHours(7)),
        new("column", ColumnType.Real, DateTimeResolution.Milliseconds, TimeSpan.FromHours(7)),
        new("column", ColumnType.Integer, DateTimeResolution.Seconds, TimeSpan.FromHours(7)),
        new("column", ColumnType.Real, DateTimeResolution.Seconds, TimeSpan.FromHours(7)),
        new("column", ColumnType.Text)
    ];
}