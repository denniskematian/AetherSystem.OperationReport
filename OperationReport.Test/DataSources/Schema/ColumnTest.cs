using System.ComponentModel;
using AetherSystem.OperationReport.DataSources.Schema;

namespace OperationReport.Test.DataSources.Schema;

public class ColumnTest
{
    [Theory]
    [InlineData("columnName", ColumnType.Integer)]
    [InlineData("columnName 1", ColumnType.Real)]
    [InlineData("columnName 2", ColumnType.Text)]
    [InlineData("column Name  ", ColumnType.Text)]
    [InlineData("  column Name", ColumnType.Text)]
    [InlineData("  column Name  ", ColumnType.Text)]
    public void Constructor_ShouldInitializesWithCorrectValues(string name, ColumnType type)
    {
        var column = new Column(name, type);
        Assert.Equal(column.Name, name);
        Assert.Equal(column.Type, type);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\r\n\n\r")]
    public void Constructor_ShouldThrowsIfEmptyName(string columnName)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Column(columnName, ColumnType.Real));
    }
    
    [Theory]
    [InlineData((ColumnType)(-1))]
    [InlineData((ColumnType)int.MinValue)]
    [InlineData((ColumnType)int.MaxValue)]
    public void Constructor_ShouldThrowsIfUndefinedType(ColumnType columnType)
    {
        Assert.Throws<InvalidEnumArgumentException>(() => new Column("columnName", columnType));
    }
}