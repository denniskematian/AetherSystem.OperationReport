using System.ComponentModel;
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
    public void Constructor_ShouldThrowsIfUnspecifiedNumericType(ColumnType columnType)
    {
        Assert.Throws<ArgumentException>(() => new DateTimeColumn(
            name: "columnName",
            type: columnType,
            resolution: DateTimeResolution.Unspecified));
    }
    
    [Theory]
    [InlineData((DateTimeResolution)(-1))]
    [InlineData((DateTimeResolution)int.MinValue)]
    [InlineData((DateTimeResolution)int.MaxValue)]
    public void Constructor_ShouldThrowsIfUndefinedResolution(DateTimeResolution resolution)
    {
        Assert.Throws<InvalidEnumArgumentException>(() => new DateTimeColumn("columnName", ColumnType.Real, resolution));
    }
}