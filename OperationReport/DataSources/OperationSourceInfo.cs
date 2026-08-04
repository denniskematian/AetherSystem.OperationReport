using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record OperationSourceInfo(
    string FilePath, 
    FileType Type,
    Table Table, 
    DateTimeColumn TimestampColumn,
    Column CommentColumn)
    : DataSourceInfo(FilePath, Type);