using System.ComponentModel;
using AetherSystem.OperationReport.Timestamps;

namespace OperationReport.Test.Timestamps;

public class FractionalUnixTimestampConverterTest
{
    private const int TestDataCount = 100;
    
    [Fact]
    public void Constructor_ThrowsIfResolutionIsInvalid()
    {
        Assert.Throws<InvalidEnumArgumentException>(() => new FractionalUnixTimestampConverter((TimestampResolution)8, TimeSpan.Zero));
        Assert.Throws<InvalidEnumArgumentException>(() => new FractionalUnixTimestampConverter((TimestampResolution)int.MinValue, TimeSpan.Zero));
        Assert.Throws<InvalidEnumArgumentException>(() => new FractionalUnixTimestampConverter((TimestampResolution)int.MaxValue, TimeSpan.Zero));
    }
    
    [Theory]
    [MemberData(nameof(TestData))]
    public void ToDateTime_ShouldPreciseUpToMicroseconds(double value, TimestampResolution resolution, DateTime expected)
    {
        var converter = new FractionalUnixTimestampConverter(resolution, TimeSpan.Zero);
        var dateTime = converter.ToDateTime(value);

        Assert.Equal(expected, dateTime, TimeSpan.FromMicroseconds(1));
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void FromDateTime_ShouldPreciseUpToResolutionUnit(double expected, TimestampResolution resolution, DateTime value)
    {
        var converter = new FractionalUnixTimestampConverter(resolution, TimeSpan.Zero);
        var unix = (double)converter.FromDateTime(value);
        var tolerance = 10_000d / TimestampUtils.TicksPerUnit(resolution);
        
        Assert.Equal(expected, unix, tolerance);
    }

    public static TheoryData<double, TimestampResolution, DateTime> TestData()
    {
        var testData = new TheoryData<double, TimestampResolution, DateTime>();
        var tickStart = DateTime.Parse("2088-08-08T00:00:00").Ticks;
        const long tickRange = 300 * TimeSpan.TicksPerDay;
        for(var i = 0; i < TestDataCount; i++)
        {
            var resolution = (TimestampResolution)(i % 7);
            var ticks = Random.Shared.NextDouble() * tickRange + tickStart;
            var dt = new DateTime((long)ticks);

            var unix = (ticks - DateTime.UnixEpoch.Ticks) / TimestampUtils.TicksPerUnit(resolution);
            testData.Add(unix, resolution, dt);
        }

        return testData;
    }
}