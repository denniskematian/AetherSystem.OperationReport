using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Timestamps;

namespace OperationReport.Test.DataSources;

public class OperationSourceInfoTest
{
    private static TimestampColumn CreateTextTimestampColumn(string name = "timestamp") =>
        new TimestampColumn(name, ColumnType.Text, new StringTimestampFormat("yyyy-MM-dd HH:mm:ss"));

    private static TimestampColumn CreateIntegerTimestampColumn(string name = "timestamp") =>
        new TimestampColumn(name, ColumnType.Integer, new UnixTimestampFormat(TimestampResolution.Millisecond, TimeSpan.Zero));

    [Fact]
    public void Constructor_ShouldInitializesCorrectly()
    {
        var timestampColumn = CreateTextTimestampColumn();
        var commentColumn = new Column("comment", ColumnType.Text); 
        var table = new Table("table", [
            timestampColumn,
            new Column("comment", ColumnType.Text),
        ]);
        
        var value = new OperationSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, commentColumn);
        Assert.Equal(table.Columns[0], value.TimestampColumn);
        Assert.Equal(table.Columns[1], value.CommentColumn);
        Assert.Equal(table, value.Table);
    }
    
    [Theory]
    [InlineData("datetime", ColumnType.Text, "comment", ColumnType.Text)]
    [InlineData("timestamp", ColumnType.Integer, "comment", ColumnType.Text)]
    [InlineData("timestamp", ColumnType.Text, "label", ColumnType.Text)]
    [InlineData("timestamp", ColumnType.Text, "comment", ColumnType.Integer)]
    public void Constructor_ShouldThrowsIfColumnNotExists(
        string timestampColumnName, 
        ColumnType timestampColumnType,
        string commentColumnName, 
        ColumnType commentColumnType)
    {
        var table = new Table("table", [
            CreateTextTimestampColumn(),
            new Column("comment", ColumnType.Text),
        ]);
        
        var timestampColumn = timestampColumnType == ColumnType.Integer
            ? CreateIntegerTimestampColumn(timestampColumnName)
            : CreateTextTimestampColumn(timestampColumnName);
        var commentColumn = new Column(commentColumnName, commentColumnType); 
    
        Assert.Throws<ArgumentException>(() => new OperationSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, commentColumn));
    }
    
    [Fact]
    public void Constructor_ShouldThrowsIfLabelColumnNotText()
    {
        var table = new Table("table", [
            CreateTextTimestampColumn(),
            new Column("comment", ColumnType.Integer)
        ]);
    
        Assert.Throws<ArgumentException>(() => new OperationSourceInfo("data.db", FileType.Sqlite, table, (TimestampColumn)table.Columns[0], table.Columns[1]));
    }
    
    [Fact]
    public void TimestampColumnIndex_IsIndexOfColumn()
    {
        var table = new Table("table", [
            CreateTextTimestampColumn(),
            new Column("comment", ColumnType.Text),
        ]);
    
        var value = new OperationSourceInfo("data.db", FileType.Sqlite, table, (TimestampColumn)table.Columns[0], table.Columns[1]);
        Assert.Equal(0, value.TimestampColumnIndex);
        Assert.Equal(table.IndexOf(value.TimestampColumn), value.TimestampColumnIndex);
        Assert.Equal(table.IndexOf(table.Columns[0]), value.TimestampColumnIndex);
        Assert.Equal(table.IndexOf(new Column("timestamp", ColumnType.Text)), value.TimestampColumnIndex);
        Assert.Equal(table.IndexOf(CreateTextTimestampColumn()), value.TimestampColumnIndex);
    }
    
    [Fact]
    public void CommentColumnIndex_IsIndexOfColumn()
    {
        var table = new Table("table", [
            CreateTextTimestampColumn(),
            new Column("comment", ColumnType.Text),
        ]);
    
        var value = new OperationSourceInfo("data.db", FileType.Sqlite, table, (TimestampColumn)table.Columns[0], table.Columns[1]);
        Assert.Equal(1, value.CommentColumnIndex);
        Assert.Equal(table.IndexOf(value.CommentColumn), value.CommentColumnIndex);
        Assert.Equal(table.IndexOf(table.Columns[1]), value.CommentColumnIndex);
        Assert.Equal(table.IndexOf(new Column("comment", ColumnType.Text)), value.CommentColumnIndex);
        Assert.Equal(table.IndexOf(CreateTextTimestampColumn("comment")), value.CommentColumnIndex);
    }
}
