using AetherSystem.OperationReport.Timestamps;

namespace OperationReport.Test.Timestamps;

public class StringTimestampConverterTest
{
    private const int TestDataCount = 100;
    
    [Theory]
    [MemberData(nameof(TestData))]
    public void ToDateTime_ShouldMatch(string value, DateTime expected)
    {
        var converter = new StringTimestampConverter();
        var dateTime = converter.ToDateTime(value);

        Assert.Equal(expected, dateTime);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void FromDateTime_ShouldMatch(string expected, DateTime value)
    {
        var converter = new StringTimestampConverter();
        var unix = (string)converter.FromDateTime(value);
        
        Assert.Equal(expected, unix);
    }

    public static TheoryData<string, DateTime> TestData()
    {
        var testData = new TheoryData<string, DateTime>();
        var tickStart = DateTime.Parse("2088-08-08T00:00:00").Ticks;
        const long tickRange = 300 * TimeSpan.TicksPerDay;
        for(var i = 0; i < TestDataCount; i++)
        {
            var ticks = Random.Shared.NextDouble() * tickRange + tickStart;
            var dt = new DateTime((long)ticks);
            var str = dt.ToString("O");

            testData.Add(str, dt);
        }

        return testData;
    }
}