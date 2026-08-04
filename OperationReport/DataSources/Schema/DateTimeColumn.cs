namespace AetherSystem.OperationReport.DataSources.Schema;

public record DateTimeColumn(
    string Name,
    ColumnType Type,
    DateTimeResolution Resolution = DateTimeResolution.Unspecified,
    TimeSpan Offset = default)
    : Column(Name, Type);