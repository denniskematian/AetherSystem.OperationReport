using System.ComponentModel;
using AetherSystem.OperationReport.Timestamps;

namespace OperationReport.Test.Timestamps;

public class TimestampComparerTest
{
    private const int TestDataCount = 100;
    
    [Fact]
    public void Constructor_ThrowsIfResolutionIsInvalid()
    {
        Assert.Throws<InvalidEnumArgumentException>(() => new TimestampComparer((TimestampResolution)8));
        Assert.Throws<InvalidEnumArgumentException>(() => new TimestampComparer((TimestampResolution)int.MinValue));
        Assert.Throws<InvalidEnumArgumentException>(() => new TimestampComparer((TimestampResolution)int.MaxValue));
    }

    [Theory]
    [MemberData(nameof(EqualityTestData))]
    public void Equals_ShouldTestWithPrecision(DateTime value, TimestampResolution resolution, DateTime comparand)
    {
        var comparer = new TimestampComparer(resolution);
        Assert.True(comparer.Equals(value, comparand));
    }

    [Theory]
    [MemberData(nameof(EqualityTestData))]
    public void Equals_ShouldComparesEqualityWithPrecision(DateTime value, TimestampResolution resolution, DateTime comparand)
    {
        var comparer = new TimestampComparer(resolution);
        Assert.Equal(0, comparer.Compare(value, comparand));
    }

    [Theory]
    [MemberData(nameof(EqualityTestData))]
    public void Equals_ShouldComputesHashCodeWithPrecision(DateTime value, TimestampResolution resolution, DateTime comparand)
    {
        var comparer = new TimestampComparer(resolution);
        Assert.Equal(comparer.GetHashCode(value), comparer.GetHashCode(comparand));
    }

    public static TheoryData<DateTime, TimestampResolution, DateTime> EqualityTestData()
    {
        var testData = new TheoryData<DateTime, TimestampResolution, DateTime>();
        var tickStart = DateTime.Parse("2088-08-08T00:00:00").Ticks;
        const long tickRange = 300 * TimeSpan.TicksPerDay;
        for(var i = 0; i < TestDataCount; i++)
        {
            var resolution = (TimestampResolution)(i % 7);
            var tickPerUnit = TimestampUtils.TicksPerUnit(resolution);
            var ticks = (long)(Random.Shared.NextDouble() * tickRange) + tickStart;
            var baseTicks = ticks / tickPerUnit * tickPerUnit;

            long prevTickPerUnit;
            if(resolution == TimestampResolution.HundredNanoseconds)
                prevTickPerUnit = 0;
            else 
                prevTickPerUnit = TimestampUtils.TicksPerUnit(resolution - 1);

            var testTicks = baseTicks + Random.Shared.NextInt64(prevTickPerUnit, tickPerUnit);

            var dt = new DateTime(baseTicks);
            var testDt = new DateTime(testTicks);
            testData.Add(dt, resolution, testDt);
        }

        return testData;
    }
}