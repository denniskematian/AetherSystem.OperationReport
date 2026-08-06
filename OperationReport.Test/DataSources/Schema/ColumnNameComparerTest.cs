using AetherSystem.OperationReport.DataSources.Schema;

namespace OperationReport.Test.DataSources.Schema;

public class ColumnNameComparerTest
{
    [Theory]
    [InlineData("column1", "column2")]
    [InlineData("column2", "column1")]
    [InlineData("column1", "column1")]
    [InlineData("column2", "column2")]
    public void ShouldCompareColumnsByTheirNames(string columnName1, string columnName2)
    {
        var column1 = new Column(columnName1, ColumnType.Text);
        var column2 = new Column(columnName2, ColumnType.Text);
        var comparer = ColumnNameComparer.Instance;
        Assert.Equal(
            string.Compare(columnName1, columnName2, StringComparison.Ordinal), 
            comparer.Compare(column1, column2));
        
        Assert.Equal(
            string.Equals(columnName1, columnName2, StringComparison.Ordinal), 
            comparer.Equals(column1, column2));
    }
    
    [Fact]
    public void ShouldEqualsIfSameReference()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.Equal(0, ColumnNameComparer.Instance.Compare(column, column));
        Assert.True(ColumnNameComparer.Instance.Equals(column, column));
    }
    
    [Theory]
    [InlineData("column1")]
    [InlineData("column2")]
    [InlineData("column3")]
    [InlineData("column4")]
    public void GetHashCode_ShouldEqualsByColumnName(string columnName)
    {
        var column = new Column(columnName, ColumnType.Text);
        Assert.Equal(columnName.GetHashCode(), ColumnNameComparer.Instance.GetHashCode(column));
    }

    [Fact]
    public void ShouldReturnsFalseIfOneOfTheColumnIsNull()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.False(ColumnNameComparer.Instance.Equals(column, null));
        Assert.False(ColumnNameComparer.Instance.Equals(null, column));
    }

    [Fact]
    public void ShouldReturnsFalseIfNotTheSameType()
    {
        var column = new Column("column", ColumnType.Text);
        var dtColumn = new DateTimeColumn("column", ColumnType.Text);
        Assert.False(ColumnNameComparer.Instance.Equals(column, dtColumn));
    }
}