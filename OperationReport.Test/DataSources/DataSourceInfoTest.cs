using System.ComponentModel;
using AetherSystem.OperationReport.DataSources;

namespace OperationReport.Test.DataSources;

public class DataSourceInfoTest
{
    [Theory]
    [InlineData("data.db", FileType.Sqlite)]
    [InlineData("data.csv", FileType.Csv)]
    public void Constructor_ShouldInitializesCorrectly(string filePath, FileType fileType)
    {
        var value = new DataSourceInfo(filePath, fileType);
        Assert.Equal(filePath, value.FilePath);
        Assert.Equal(fileType, value.FileType);
    }

    [Theory]
    [InlineData((FileType)int.MinValue)]
    [InlineData((FileType)int.MaxValue)]
    public void Constructor_ShouldThrowsIfUndefinedFileType(FileType fileType)
    {
        Assert.Throws<InvalidEnumArgumentException>(() => new DataSourceInfo("data.db", fileType));
    }
    
    // public static TheoryData<Column> ReferenceSourceInfo_ShouldThrowsIfIdNotExists_TestData()
    // {
    //     return
    //     [
    //         new Column("column4", ColumnType.Text),
    //         new Column("column1 ", ColumnType.Text),
    //         new Column(" column1", ColumnType.Text),
    //         new Column(" column1 ", ColumnType.Text),
    //         new Column("column1", ColumnType.Integer),
    //         new Column("column2", ColumnType.Real),
    //         new DateTimeColumn("column1", ColumnType.Text),
    //     ];
    // }

    [Theory]
    [InlineData((FileType)(-1))]
    [InlineData((FileType)int.MinValue)]
    [InlineData((FileType)int.MaxValue)]
    public void DataSourceInfo_ShouldThrowsIfUndefinedType(FileType type)
    {
        Assert.Throws<InvalidEnumArgumentException>(() => new DataSourceInfo("test", type));
    }
    
    // [Fact]
    // public void ReferenceSourceInfo_ShouldInitializesCorrectly()
    // {
    //     var table = new Table("table", [
    //         new Column("id", ColumnType.Integer),
    //         new Column("label", ColumnType.Text),
    //     ]);
    //     
    //     var value = new ReferenceSourceInfo("data.db", FileType.Sqlite, table, table.Columns[0], table.Columns[1]);
    //     Assert.Equal(table.Columns[0], value.IdColumn);
    //     Assert.Equal(table.Columns[1], value.LabelColumn);
    //     Assert.Equal(table, value.Table);
    // }
    //
    // [Fact]
    // public void ReferenceSourceInfo_ShouldNotThrowsIfEquals()
    // {
    //     var table = new Table("table", [
    //         new Column("id", ColumnType.Integer),
    //         new Column("label", ColumnType.Text),
    //     ]);
    //     
    //     var value = new ReferenceSourceInfo("data.db", FileType.Sqlite, table, table.Columns[0], table.Columns[1]);
    //     Assert.Equal(value.IdColumn, new Column("id", ColumnType.Integer));
    //     Assert.Equal(value.LabelColumn, new Column("label", ColumnType.Text));
    // }
    
    // [Theory]
    // [InlineData("column4", ColumnType.Text)]
    // [InlineData("column1 ", ColumnType.Text)]
    // [InlineData(" column1", ColumnType.Text)]
    // [InlineData(" column1 ", ColumnType.Text)]
    // [InlineData("column1", ColumnType.Text)]
    // [InlineData("column1", ColumnType.Text)]
    // [InlineData("column1", ColumnType.Text)]
    // public void ReferenceSourceInfo_ShouldThrowsIfIdNotExists(string columnName, ColumnType columnType)
    // {
    //     var table = new Table("table", [
    //         new Column("column1", ColumnType.Text),
    //         new Column("column2", ColumnType.Text),
    //         new Column("column3", ColumnType.Text),
    //     ]);
    //     
    //     var column = new Column("column", ColumnType.Integer);
    //
    //     Assert.Throws<ArgumentException>(() =>
    //         new ReferenceSourceInfo("data.db", FileType.Sqlite, table, table.Columns[columnIndex], table.Columns[1]));
    // }
    //
    // [Theory]
    // [InlineData(0)]
    // [InlineData(2)]
    // [InlineData(3)]
    // [InlineData(5)]
    // public void ReferenceSourceInfo_ShouldThrowsIfLabelNotText(int columnIndex)
    // {
    //     var table = new Table("table", [
    //         new Column("real", ColumnType.Real),
    //         new Column("text", ColumnType.Text),
    //         new Column("integer", ColumnType.Integer),
    //         new DateTimeColumn("date_real", ColumnType.Real, DateTimeResolution.Seconds),
    //         new DateTimeColumn("date_text", ColumnType.Text),
    //         new DateTimeColumn("date_integer", ColumnType.Integer, DateTimeResolution.Milliseconds),
    //     ]);
    //
    //     Assert.Throws<ArgumentException>(() =>
    //         new ReferenceSourceInfo("data.db", FileType.Sqlite, table, table.Columns[2], table.Columns[columnIndex]));
    // }
}