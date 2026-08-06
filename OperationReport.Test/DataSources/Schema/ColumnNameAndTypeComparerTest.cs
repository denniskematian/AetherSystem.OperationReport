using AetherSystem.OperationReport.DataSources.Schema;

namespace OperationReport.Test.DataSources.Schema;

public class ColumnNameAndTypeComparerTest
{
    public static TheoryData<Column, Column, bool> EqualityTestRows()
    {
        return new TheoryData<Column, Column, bool>()
        {
            { new Column("column1", ColumnType.Integer), new Column("column1", ColumnType.Integer), true },
            { new Column("column2", ColumnType.Real), new Column("column2", ColumnType.Real), true },
            { new Column("column3", ColumnType.Text), new Column("column3", ColumnType.Text), true },
            { new Column("column1", ColumnType.Integer), new Column("column3", ColumnType.Text), false },
            { new Column("column2", ColumnType.Real), new Column("column1", ColumnType.Integer), false },
            { new Column("column3", ColumnType.Text), new Column("column2", ColumnType.Real), false },
            { new Column("column1", ColumnType.Integer), new Column("column1", ColumnType.Text), false },
            { new Column("column2", ColumnType.Real), new Column("column2", ColumnType.Integer), false },
            { new Column("column3", ColumnType.Text), new Column("column3", ColumnType.Real), false },
        };
    }
    
    public static TheoryData<Column, Column, int> ComparisonTestRows()
    {
        return new TheoryData<Column, Column, int>()
        {
            // test equality
            { new Column("column1", ColumnType.Integer), new Column("column1", ColumnType.Integer), 0 },
            { new Column("column2", ColumnType.Real), new Column("column2", ColumnType.Real), 0 },
            { new Column("column3", ColumnType.Text), new Column("column3", ColumnType.Text), 0 },
            // test name
            { new Column("column1", ColumnType.Integer), new Column("column2", ColumnType.Integer), -1 },
            { new Column("column2", ColumnType.Real), new Column("column3", ColumnType.Real), -1 },
            { new Column("column3", ColumnType.Text), new Column("column4", ColumnType.Text), -1 },
            { new Column("column1", ColumnType.Integer), new Column("column", ColumnType.Integer), 1 },
            { new Column("column2", ColumnType.Real), new Column("column1", ColumnType.Real), 1 },
            { new Column("column3", ColumnType.Text), new Column("column2", ColumnType.Text), 1 },
            // test type
            { new Column("column1", ColumnType.Text), new Column("column1", ColumnType.Text), 0 },
            { new Column("column1", ColumnType.Text), new Column("column1", ColumnType.Integer), -1 },
            { new Column("column1", ColumnType.Text), new Column("column1", ColumnType.Real), -1 },
            { new Column("column1", ColumnType.Integer), new Column("column1", ColumnType.Text), 1 },
            { new Column("column1", ColumnType.Integer), new Column("column1", ColumnType.Integer), 0 },
            { new Column("column1", ColumnType.Integer), new Column("column1", ColumnType.Real), -1 },
            { new Column("column1", ColumnType.Real), new Column("column1", ColumnType.Text), 1 },
            { new Column("column1", ColumnType.Real), new Column("column1", ColumnType.Integer), 1 },
            { new Column("column1", ColumnType.Real), new Column("column1", ColumnType.Real), 0 },
        };
    }
    
    [Fact]
    public void Equals_ShouldReturnsFalseIfEitherNull()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.False(ColumnComparer.NameAndType.Equals(null, column));
        Assert.False(ColumnComparer.NameAndType.Equals(column, null));
    }
    
    [Fact]
    public void Equals_ShouldReturnsTrueIfSameReference()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.True(ColumnComparer.NameAndType.Equals(column, column));
    }
    
    [Fact]
    public void Equals_ShouldReturnsTrueIfBothNull()
    {
        Assert.True(ColumnComparer.NameAndType.Equals(null, null));
    }
    
    [Theory]
    [MemberData(nameof(EqualityTestRows))]
    public void Equals_ShouldCompareEqualityByNameAndType(Column column1, Column column2, bool expected)
    {
        Assert.Equal(expected, ColumnComparer.NameAndType.Equals(column1, column2));
    }
    
    [Fact]
    public void Compare_ShouldComparableWithNull()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.Equal(-1, ColumnComparer.NameAndType.Compare(null, column));
        Assert.Equal(1, ColumnComparer.NameAndType.Compare(column, null));
    }
    
    [Theory]
    [MemberData(nameof(ComparisonTestRows))]
    public void Compare_ShouldCompareByNameAndType(Column column1, Column column2, int expected)
    {
        Assert.Equal(expected, ColumnComparer.NameAndType.Compare(column1, column2));
        Assert.Equal(-expected, ColumnComparer.NameAndType.Compare(column2, column1));
    }
    
    [Fact]
    public void Compare_ShouldReturnsZeroSameReference()
    {
        var column = new Column("column", ColumnType.Text);
        Assert.Equal(0, ColumnComparer.NameAndType.Compare(column, column));
    }
    
    [Fact]
    public void Compare_ShouldReturnsZeroBothNull()
    {
        Assert.Equal(0, ColumnComparer.NameAndType.Compare(null, null));
    }

    [Theory]
    [InlineData("column1", ColumnType.Text)]
    [InlineData("column2", ColumnType.Integer)]
    [InlineData("column3", ColumnType.Real)]
    [InlineData("column4", ColumnType.Text)]
    public void GetHashCode_ShouldEqualsByColumnAndName(string columnName, ColumnType type)
    {
        var column = new Column(columnName, type);
        Assert.Equal(
            HashCode.Combine(string.GetHashCode(column.Name, StringComparison.Ordinal), type.GetHashCode()),
            ColumnComparer.NameAndType.GetHashCode(column));
    }
}