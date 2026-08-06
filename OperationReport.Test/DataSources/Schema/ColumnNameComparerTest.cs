using AetherSystem.OperationReport.DataSources.Schema;

namespace OperationReport.Test.DataSources.Schema;

public class ColumnNameComparerTest
{
    [Fact]
    public void Equals_ShouldReturnsFalseIfEitherNull()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.False(ColumnComparer.NameOnly.Equals(null, column));
        Assert.False(ColumnComparer.NameOnly.Equals(column, null));
    }
    
    [Fact]
    public void Equals_ShouldReturnsTrueIfSameReference()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.True(ColumnComparer.NameOnly.Equals(column, column));
    }
    
    [Fact]
    public void Equals_ShouldReturnsTrueIfBothNull()
    {
        Assert.True(ColumnComparer.NameOnly.Equals(null, null));
    }
    
    [Theory]
    [InlineData("column1", "column2")]
    [InlineData("column2", "column1")]
    [InlineData("column1", "column1")]
    [InlineData("column2", "column2")]
    [InlineData("Column1", "column2")]
    [InlineData("Column2", "column1")]
    [InlineData("Column1", "column1")]
    [InlineData("Column2", "column2")]
    public void Equals_ShouldCompareEqualityByTheirNames(string columnName1, string columnName2)
    {
        var column1 = new Column(columnName1, ColumnType.Text);
        var column2 = new Column(columnName2, ColumnType.Real);
        
        Assert.Equal(
            string.Equals(columnName1, columnName2, StringComparison.Ordinal), 
            ColumnComparer.NameOnly.Equals(column1, column2));
    }
    
    [Fact]
    public void Compare_ShouldComparableWithNull()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.Equal(-1, ColumnComparer.NameOnly.Compare(null, column));
        Assert.Equal(1, ColumnComparer.NameOnly.Compare(column, null));
    }
    
    [Theory]
    [InlineData("column1", "column2")]
    [InlineData("column2", "column1")]
    [InlineData("column1", "column1")]
    [InlineData("column2", "column2")]
    [InlineData("Column1", "column2")]
    [InlineData("Column2", "column1")]
    [InlineData("Column1", "column1")]
    [InlineData("Column2", "column2")]
    public void Compare_ShouldCompareByTheirNames(string columnName1, string columnName2)
    {
        var column1 = new Column(columnName1, ColumnType.Text);
        var column2 = new Column(columnName2, ColumnType.Real);
        
        Assert.Equal(
            string.Compare(columnName1, columnName2, StringComparison.Ordinal), 
            ColumnComparer.NameOnly.Compare(column1, column2));
    }
    
    [Fact]
    public void Compare_ShouldReturnsZeroSameReference()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.Equal(0, ColumnComparer.NameOnly.Compare(column, column));
    }
    
    [Fact]
    public void Compare_ShouldReturnsZeroBothNull()
    {
        Assert.Equal(0, ColumnComparer.NameOnly.Compare(null, null));
    }

    [Theory]
    [InlineData("column1")]
    [InlineData("column2")]
    [InlineData("column3")]
    [InlineData("column4")]
    public void GetHashCode_ShouldEqualsByColumnName(string columnName)
    {
        var column = new Column(columnName, ColumnType.Text);
        Assert.Equal(
            string.GetHashCode(column.Name, StringComparison.Ordinal),
            ColumnComparer.NameOnly.GetHashCode(column));
    }
}