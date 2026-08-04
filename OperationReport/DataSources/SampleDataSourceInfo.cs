using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record SampleDataSourceInfo(
    string FilePath, 
    FileType Type,
    Table Table,
    DateTimeColumn TimestampColumn,
    Column? BatchNumberColumn,
    IReadOnlyList<Column> SampleColumns)
    : DataSourceInfo(FilePath, Type);