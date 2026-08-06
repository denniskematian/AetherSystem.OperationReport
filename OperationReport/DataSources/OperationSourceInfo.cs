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
        if(Table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, TimestampColumn)))
            throw new ArgumentException($"Timestamp column '{TimestampColumn.Name}' not found in table");

        if(Table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, CommentColumn)))
            throw new ArgumentException($"Comment column '{CommentColumn.Name}' not found in table");

        if(CommentColumn.Type is not ColumnType.Text)
            throw new ArgumentException($"Comment column '{CommentColumn.Name}' must be of type text");

        this.Table = Table;
        this.TimestampColumn = TimestampColumn;
        this.CommentColumn = CommentColumn;
    }

    public int TimestampColumnIndex => Table.IndexOf(TimestampColumn);
    public int CommentColumnIndex => Table.IndexOf(CommentColumn);
}