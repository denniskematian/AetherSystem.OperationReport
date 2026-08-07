using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record OperationSourceInfo : DataSourceInfo
{
    public Table Table { get; }
    public DateTimeColumn TimestampColumn { get; }
    public Column CommentColumn { get; }
    
    public OperationSourceInfo(
        string filePath,
        FileType type,
        Table table,
        DateTimeColumn timestampColumn,
        Column commentColumn) : base(filePath, type)
    {
        if(table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, timestampColumn)))
            throw new ArgumentException($"Timestamp column '{timestampColumn.Name}' not found in table");

        if(table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, commentColumn)))
            throw new ArgumentException($"Comment column '{commentColumn.Name}' not found in table");

        if(commentColumn.Type is not ColumnType.Text)
            throw new ArgumentException($"Comment column '{commentColumn.Name}' must be of type text");

        Table = table;
        TimestampColumn = timestampColumn;
        CommentColumn = commentColumn;
    }

    public int TimestampColumnIndex => Table.IndexOf(TimestampColumn);
    public int CommentColumnIndex => Table.IndexOf(CommentColumn);
}