using AetherSystem.OperationReport.DataSources.Schema;

namespace OperationReport.Test.DataSources.Schema;

public class TableTest
{
    public static TheoryData<string, string[]> CreationData()
    {
        return new TheoryData<string, string[]>
        {
            { "tableName", ["column1", "column2"] },
            { "anotherTable", ["col1", "col2", "col3"] }
        };
    }
    
    public static TheoryData<Column, int> IndexOfData()
    {
        return new TheoryData<Column, int>
        {
            { new Column("column", ColumnType.Real), 0 },
            { new Column("column2", ColumnType.Integer), 1 },
            { new Column("column3", ColumnType.Text), 2 },
            { new Column("column4", ColumnType.Text), 3 },
            { new DateTimeColumn("column", ColumnType.Real, DateTimeResolution.Seconds), 0 },
            { new DateTimeColumn("column2", ColumnType.Integer, DateTimeResolution.Seconds), 1 },
            { new DateTimeColumn("column3", ColumnType.Text), 2 },
            { new DateTimeColumn("column4", ColumnType.Text), 3 },
            { new Column(" column ", ColumnType.Real), -1 },
            { new Column("column2 ", ColumnType.Integer), -1 },
            { new Column(" column3", ColumnType.Text), -1 },
        };
    }

    [Theory]
    [MemberData(nameof(CreationData))]
    public void Constructor_ShouldInitializeWithCorrectNameAndColumns(string tableName, string[] columnNames)
    {
        var columns = columnNames.Select(name => new Column(name, ColumnType.Text)).ToArray();
        var table = new Table(tableName, columns);
        Assert.Equal(tableName, table.Name);
        Assert.Equal(columns, table.Columns);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\r\n\n\r")]
    public void Constructor_ShouldThrowsIfEmptyName(string tableName)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new Table(tableName, [new Column("column", ColumnType.Real)]));
    }
    
    [Fact]
    public void Constructor_ShouldThrowsIfEmptyColumns()
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new Table("tableName", []));
    }
    
    [Fact]
    public void Constructor_ShouldThrowsIfDuplicateColumns()
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new Table("tableName", [
                new Column("column", ColumnType.Real), 
                new Column("column", ColumnType.Integer)]));
    }

    [Theory]
    [InlineData("column", 0)]
    [InlineData("column2", 1)]
    [InlineData("column3",2)]
    [InlineData("column4", 3)]
    [InlineData(" column", -1)]
    [InlineData("column2 ", -1)]
    [InlineData(" column3 ",-1)]
    [InlineData("column5", -1)]
    public void IndexOf_ShouldReturnsNonNegativeIfNameExists(string columnName, int expected)
    {
        var table = new Table("table", [
            new Column("column", ColumnType.Real),
            new Column("column2", ColumnType.Integer),
            new Column("column3", ColumnType.Text),
            new DateTimeColumn("column4", ColumnType.Text),
        ]);
        
        Assert.Equal(expected, table.IndexOf(columnName));
    }

    [Theory]
    [MemberData(nameof(IndexOfData))]
    public void IndexOf_ShouldReturnsNonNegativeIfExists(Column column, int expected)
    {
        var table = new Table("table", [
            new Column("column", ColumnType.Real),
            new Column("column2", ColumnType.Integer),
            new Column("column3", ColumnType.Text),
            new DateTimeColumn("column4", ColumnType.Text),
        ]);
        
        Assert.Equal(expected, table.IndexOf(column));
    }
}