using AetherSystem.OperationReport.Timestamps;

namespace OperationReport.Test.Timestamps;

public class TimestampFormatTest
{
    [Fact]
    public void FractionalUnixTimestampFormat_ComparerShouldUseMicrosecondResolution()
    {
        var format = new FractionalUnixTimestampFormat(TimestampResolution.Second, TimeSpan.Zero);
        var comparer = new TimestampComparer(TimestampResolution.Microsecond);
        
        Assert.Equal(comparer, format.Comparer);
    }
    
    [Fact]
    public void FractionalUnixTimestampFormat_ConverterShouldMatchParameter()
    {
        var format = new FractionalUnixTimestampFormat(TimestampResolution.Second, TimeSpan.FromHours(7));
        var converter = new FractionalUnixTimestampConverter(TimestampResolution.Second, TimeSpan.FromHours(7));
        
        Assert.Equal(converter, format.Converter);
    }
    
    [Fact]
    public void UnixTimestampFormat_ComparerShouldMatchResolution()
    {
        var format = new UnixTimestampFormat(TimestampResolution.Second, TimeSpan.Zero);
        var comparer = new TimestampComparer(TimestampResolution.Second);
        
        Assert.Equal(comparer, format.Comparer);
    }
    
    [Fact]
    public void UnixTimestampFormat_ConverterShouldMatchParameter()
    {
        var format = new UnixTimestampFormat(TimestampResolution.Second, TimeSpan.FromHours(7));
        var converter = new UnixTimestampConverter(TimestampResolution.Second, TimeSpan.FromHours(7));
        
        Assert.Equal(converter, format.Converter);
    }
    
    [Fact]
    public void OaTimestampFormat_ComparerShouldUseMillisecondResolution()
    {
        var format = new OaTimestampFormat();
        var comparer = new TimestampComparer(TimestampResolution.Millisecond);
        
        Assert.Equal(comparer, format.Comparer);
    }
    
    [Fact]
    public void OaTimestampFormat_ConverterShouldUseOaConverter()
    {
        var format = new OaTimestampFormat();
        var converter = new OaTimestampConverter();
        
        Assert.Equal(converter, format.Converter);
    }
    
    [Fact]
    public void StringTimestampFormat_ComparerShouldUseExactResolution()
    {
        var format = new StringTimestampFormat("O");
        var comparer = new TimestampComparer(TimestampResolution.HundredNanoseconds);
        
        Assert.Equal(comparer, format.Comparer);
    }
    
    [Fact]
    public void StringTimestampFormat_ConverterShouldUseStringConverter()
    {
        var format = new StringTimestampFormat("O");
        var converter = new StringTimestampConverter();
        
        Assert.Equal(converter, format.Converter);
    }
}