using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record OperationSourceInfo : DataSourceInfo
{
    public Table Table { get; }
    public DateTimeColumn TimestampColumn { get; }
    public Column CommentColumn { get; }
    
    public OperationSourceInfo(
        string FilePath,
        FileType Type,
        Table Table,
        DateTimeColumn TimestampColumn,
        Column CommentColumn) : base(FilePath, Type)
    {
        if(Table.Columns.All(column => column.Name != TimestampColumn.Name))
            throw new ArgumentException($"Timestamp column '{TimestampColumn.Name}' not found in table");

        if(Table.Columns.All(column => column.Name != CommentColumn.Name))
            throw new ArgumentException($"Comment column '{CommentColumn.Name}' not found in table");

        if(CommentColumn.Type is not ColumnType.Text)
            throw new ArgumentException($"Comment column '{CommentColumn.Name}' must be of type text");

        this.Table = Table;
        this.TimestampColumn = TimestampColumn;
        this.CommentColumn = CommentColumn;
    }
}