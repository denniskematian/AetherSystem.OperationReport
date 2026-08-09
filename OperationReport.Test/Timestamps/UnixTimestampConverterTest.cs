using System.ComponentModel;
using AetherSystem.OperationReport.Timestamps;

namespace OperationReport.Test.Timestamps;

public class UnixTimestampConverterTest
{
    private const int TestDataCount = 100;
    
    [Fact]
    public void Constructor_ThrowsIfResolutionIsInvalid()
    {
        Assert.Throws<InvalidEnumArgumentException>(() => new UnixTimestampConverter((TimestampResolution)8, TimeSpan.Zero));
        Assert.Throws<InvalidEnumArgumentException>(() => new UnixTimestampConverter((TimestampResolution)int.MinValue, TimeSpan.Zero));
        Assert.Throws<InvalidEnumArgumentException>(() => new UnixTimestampConverter((TimestampResolution)int.MaxValue, TimeSpan.Zero));
    }
    
    [Theory]
    [MemberData(nameof(TestData))]
    public void ToDateTime_ShouldPreciseUpToResolutionUnit(long value, TimestampResolution resolution, DateTime expected)
    {
        var converter = new UnixTimestampConverter(resolution, TimeSpan.Zero);
        var dateTime = converter.ToDateTime(value);

        Assert.Equal(expected, dateTime, TimeSpan.FromTicks(TimestampUtils.TicksPerUnit(resolution)));
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void FromDateTime_ShouldPreciseUpToTicks(long expected, TimestampResolution resolution, DateTime value)
    {
        var converter = new UnixTimestampConverter(resolution, TimeSpan.Zero);
        var unix = (long)converter.FromDateTime(value);
        
        Assert.Equal(expected, unix);
    }
    
    public static TheoryData<long, TimestampResolution, DateTime> TestData()
    {
        var testData = new TheoryData<long, TimestampResolution, DateTime>();
        var tickStart = DateTime.Parse("2088-08-08T00:00:00").Ticks;
        const long tickRange = 300 * TimeSpan.TicksPerDay;
        for(var i = 0; i < TestDataCount; i++)
        {
            var resolution = (TimestampResolution)(i % 7);
            var ticks = (long)(Random.Shared.NextDouble() * tickRange) + tickStart;
            var dt = new DateTime(ticks);

            var unix = (ticks - DateTime.UnixEpoch.Ticks) / TimestampUtils.TicksPerUnit(resolution);
            testData.Add(unix, resolution, dt);
        }

        return testData;
    }
}