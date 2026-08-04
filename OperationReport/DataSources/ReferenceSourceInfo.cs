using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record ReferenceSourceInfo(
    string FilePath,
    FileType Type,
    Table Table,
    Column IdColumn,
    Column LabelColumn)
    : DataSourceInfo(FilePath, Type);