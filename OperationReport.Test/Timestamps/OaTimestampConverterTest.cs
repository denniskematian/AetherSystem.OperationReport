using AetherSystem.OperationReport.Timestamps;

namespace OperationReport.Test.Timestamps;

public class OaTimestampConverterTest
{
    private const int TestDataCount = 100;
    
    [Theory]
    [MemberData(nameof(TestData))]
    public void ToDateTime_ShouldPreciseUpToMilliseconds(double value, DateTime expected)
    {
        var converter = new OaTimestampConverter();
        var dateTime = converter.ToDateTime(value);

        Assert.Equal(expected, dateTime, TimeSpan.FromMilliseconds(1));
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void FromDateTime_ShouldPrecise(double expected, DateTime value)
    {
        var converter = new OaTimestampConverter();
        var unix = (double)converter.FromDateTime(value);
        
        Assert.Equal(expected, unix);
    }

    public static TheoryData<double, DateTime> TestData()
    {
        var testData = new TheoryData<double, DateTime>();
        var tickStart = DateTime.Parse("2088-08-08T00:00:00").Ticks;
        const long tickRange = 300 * TimeSpan.TicksPerDay;
        for(var i = 0; i < TestDataCount; i++)
        {
            var ticks = Random.Shared.NextDouble() * tickRange + tickStart;
            var dt = new DateTime((long)ticks);
            var oaDate = dt.ToOADate();

            testData.Add(oaDate, dt);
        }

        return testData;
    }
}