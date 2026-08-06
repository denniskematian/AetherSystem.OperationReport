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

    [Theory]
    [MemberData(nameof(CreationData))]
    public void ShouldInitializeWithCorrectNameAndColumns(string tableName, string[] columnNames)
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
    public void ShouldThrowsIfEmptyName(string tableName)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new Table(tableName, [new Column("column", ColumnType.Real)]));
    }
    
    [Fact]
    public void ShouldThrowsIfEmptyColumns()
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new Table("tableName", []));
    }
    
    [Fact]
    public void ShouldThrowsIfDuplicateColumns()
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new Table("tableName", [
                new Column("column", ColumnType.Real), 
                new Column("column", ColumnType.Integer)]));
    }
}