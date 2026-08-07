using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.DataSources.Schema;

namespace OperationReport.Test.DataSources;

public class SampleSourceInfoTest
{
    private List<Column> GetValidColumns()
    {
        return [
            new Column("timestamp", ColumnType.Text),
            new Column("batch_number", ColumnType.Integer),
            new Column("text_column", ColumnType.Text),
            new Column("sample1", ColumnType.Real),
            new Column("sample2", ColumnType.Integer),
            new Column("sample3", ColumnType.Real),
            new Column("sample4", ColumnType.Integer),
            new Column("sample5", ColumnType.Real),
            new Column("sample6", ColumnType.Integer)
        ];
    }
    
    private List<Column> GetSampleColumns()
    {
        return [..GetValidColumns().Where(c => c.Name.StartsWith("sample"))];
    }

    [Fact]
    public void Constructor_ShouldInitializesCorrectly()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var batchNumberColumn = columns[1];
        var sampleColumns = GetSampleColumns();
        
        var value = new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns);
        Assert.Equal(table, value.Table);
        Assert.Equal(timestampColumn, value.TimestampColumn);
        Assert.Equal(batchNumberColumn, value.BatchNumberColumn);
        Assert.Equal(sampleColumns, value.SampleColumns);
        
        value = new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, null, sampleColumns);
        Assert.Equal(table, value.Table);
        Assert.Equal(timestampColumn, value.TimestampColumn);
        Assert.Null(value.BatchNumberColumn);
        Assert.Equal(sampleColumns, value.SampleColumns);
    }
    
    [Fact]
    public void Constructor_ShouldThrowsIfBatchNumberColumnIsNotExists()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var batchNumberColumn = new Column("program_no", ColumnType.Integer);
        var sampleColumns = GetSampleColumns();

        Assert.Throws<ArgumentException>(() =>
            new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns));
    }
    
    [Fact]
    public void Constructor_ShouldThrowsIfBatchNumberColumnIsNotInteger()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var batchNumberColumn = new Column("batch_number", ColumnType.Real);
        columns[1] = batchNumberColumn;
        var sampleColumns = GetSampleColumns();

        Assert.Throws<ArgumentException>(() =>
            new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns));
        
        batchNumberColumn = new Column("batch_number", ColumnType.Text);
        columns[1] = batchNumberColumn;

        Assert.Throws<ArgumentException>(() =>
            new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns));
    }
    
    [Fact]
    public void Constructor_ShouldThrowsIfTimestampColumnIsNotExists()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("datetime", ColumnType.Text);
        var batchNumberColumn = columns[1];
        var sampleColumns = GetSampleColumns();

        Assert.Throws<ArgumentException>(() =>
            new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns));
        
        timestampColumn = new DateTimeColumn("timestamp", ColumnType.Integer, DateTimeResolution.Milliseconds);
        Assert.Throws<ArgumentException>(() =>
            new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns));
    }
    
    [Fact]
    public void Constructor_ShouldThrowsIfAnySampleColumnDoesNotExists()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var batchNumberColumn = new Column("batch_number", ColumnType.Integer);

        List<Column> sampleColumns = [..GetSampleColumns(), new Column("sample99", ColumnType.Real)];
        Assert.Throws<ArgumentException>(() =>
            new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns));
    }
    
    [Fact]
    public void Constructor_ShouldThrowsIfAnyUnderlyingSampleColumnIsNotNumeric()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var batchNumberColumn = new Column("batch_number", ColumnType.Integer);
        List<Column> sampleColumns = [..GetSampleColumns(), new ("text_column", ColumnType.Text)];
        
        Assert.Throws<ArgumentException>(() =>
            new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns));
    }

    [Fact]
    public void TimestampColumnIndex_IsIndexOfColumn()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var batchNumberColumn = columns[1];
        var sampleColumns = GetSampleColumns();
        
        var value = new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns);
        
        Assert.Equal(0, value.TimestampColumnIndex);
        Assert.Equal(table.IndexOf(value.TimestampColumn), value.TimestampColumnIndex);
        Assert.Equal(table.IndexOf(table.Columns[0]), value.TimestampColumnIndex);
        Assert.Equal(table.IndexOf(new Column("timestamp", ColumnType.Text)), value.TimestampColumnIndex);
        Assert.Equal(table.IndexOf(new DateTimeColumn("timestamp", ColumnType.Text)), value.TimestampColumnIndex);
    }

    [Fact]
    public void BatchNumberColumnIndex_IsIndexOfColumn()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var batchNumberColumn = columns[1];
        var sampleColumns = GetSampleColumns();
        
        var value = new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns);
        
        Assert.Equal(1, value.BatchNumberColumnIndex);
        Assert.Equal(table.IndexOf(value.BatchNumberColumn!), value.BatchNumberColumnIndex);
        Assert.Equal(table.IndexOf(table.Columns[1]), value.BatchNumberColumnIndex);
        Assert.Equal(table.IndexOf(new Column("batch_number", ColumnType.Integer)), value.BatchNumberColumnIndex);
        Assert.Equal(table.IndexOf(new DateTimeColumn("batch_number", ColumnType.Integer, DateTimeResolution.Milliseconds)), value.BatchNumberColumnIndex);
    }

    [Fact]
    public void BatchNumberColumnIndex_IsNullIfNotProvided()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var sampleColumns = GetSampleColumns();
        
        var value = new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, null, sampleColumns);
        
        Assert.Null(value.BatchNumberColumnIndex);
    }
    
    [Fact]
    public void GetSampleColumnIndices_ShouldReturnsIndexesInTable()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var batchNumberColumn = columns[1];
        var sampleColumns = GetSampleColumns();
        
        var value = new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns);
        var indexes = value.GetSampleColumnIndices();
        Assert.Equal([3, 4, 5, 6, 7, 8], indexes);
    }

    [Fact]
    public void HasBatchNumberColumn_IsFalseIfNotProvided()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var sampleColumns = GetSampleColumns();
        
        var value = new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, null, sampleColumns);
        Assert.False(value.HasBatchNumberColumn);
    }

    [Fact]
    public void HasBatchNumberColumn_IsTrueIfProvided()
    {
        var columns = GetValidColumns();
        var table = new Table("table", columns);
        var timestampColumn = new DateTimeColumn("timestamp", ColumnType.Text);
        var batchNumberColumn = columns[1];
        var sampleColumns = GetSampleColumns();
        
        var value = new SampleSourceInfo("data.db", FileType.Sqlite, table, timestampColumn, batchNumberColumn, sampleColumns);
        Assert.True(value.HasBatchNumberColumn);
    }
}