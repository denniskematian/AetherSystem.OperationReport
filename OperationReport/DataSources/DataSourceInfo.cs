using AetherSystem.OperationReport.Internals;

namespace AetherSystem.OperationReport.DataSources;

public record DataSourceInfo
{
    public string FilePath { get; }
    public FileType FileType { get; }
    
    public DataSourceInfo(string filePath, FileType fileType)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);
        ExceptionUtils.ThrowIfUndefined(fileType);

        FilePath = filePath;
        FileType = fileType;
    }
}