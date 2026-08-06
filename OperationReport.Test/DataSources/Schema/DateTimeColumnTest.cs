using AetherSystem.OperationReport.DataSources.Schema;

namespace OperationReport.Test.DataSources.Schema;

public class DateTimeColumnTest
{
    public static TheoryData<DateTimeResolution, TimeSpan> CreationData()
    {
        return new TheoryData<DateTimeResolution, TimeSpan>
        {
            { DateTimeResolution.Unspecified, TimeSpan.Zero },
            { DateTimeResolution.Milliseconds, TimeSpan.FromHours(7) },
            { DateTimeResolution.Seconds, TimeSpan.FromHours(-7) },
        };
    }
    
    [Theory]
    [MemberData(nameof(CreationData))]
    public void Constructor_InitializesWithCorrectValues(DateTimeResolution resolution, TimeSpan offset)
    {
        var column = new DateTimeColumn("columnName", ColumnType.Text, resolution, offset);
        Assert.Equal(column.Resolution, resolution);
        Assert.Equal(column.Offset, offset);
    }
    
    [Theory]
    [InlineData(ColumnType.Integer)]
    [InlineData(ColumnType.Real)]
    public void ShouldThrowsIfUnspecifiedNumericType(ColumnType columnType)
    {
        Assert.Throws<ArgumentException>(() => new DateTimeColumn(
            Name: "columnName",
            Type: columnType,
            Resolution: DateTimeResolution.Unspecified));
    }
    
    [Theory]
    [InlineData((DateTimeResolution)(-1))]
    [InlineData((DateTimeResolution)int.MinValue)]
    [InlineData((DateTimeResolution)int.MaxValue)]
    public void ShouldThrowsIfUndefinedResolution(DateTimeResolution resolution)
    {
        Assert.Throws<ArgumentException>(() => new DateTimeColumn("columnName", ColumnType.Real, resolution));
    }
}