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
    public void ShouldInitializesWithCorrectValues(string name, ColumnType type)
    {
        var column = new Column(name, type);
        Assert.Equal(column.Name, name);
        Assert.Equal(column.Type, type);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("\r\n\n\r")]
    public void ShouldThrowsIfEmptyName(string columnName)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Column(columnName, ColumnType.Real));
    }
    
    [Theory]
    [InlineData((ColumnType)(-1))]
    [InlineData((ColumnType)int.MinValue)]
    [InlineData((ColumnType)int.MaxValue)]
    public void ShouldThrowsIfUndefinedType(ColumnType columnType)
    {
        Assert.Throws<ArgumentException>(() => new Column("columnName", columnType));
    }
}