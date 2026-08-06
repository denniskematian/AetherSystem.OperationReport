using AetherSystem.OperationReport.DataSources;

namespace OperationReport.Test.DataSources;

public class DataSourceInfoTest
{
    [Theory]
    [InlineData("data.db", FileType.Sqlite)]
    [InlineData("data.csv", FileType.Csv)]
    public void ShouldInitializesCorrectly(string filePath, FileType fileType)
    {
        var value = new DataSourceInfo(filePath, fileType);
        Assert.Equal(filePath, value.FilePath);
        Assert.Equal(fileType, value.Type);
    }
    
    [Theory]
    [InlineData((FileType)(-1))]
    [InlineData((FileType)int.MinValue)]
    [InlineData((FileType)int.MaxValue)]
    public void ShouldThrowsIfUndefinedType(FileType type)
    {
        Assert.Throws<ArgumentException>(() => new DataSourceInfo("test", type));
    }
}