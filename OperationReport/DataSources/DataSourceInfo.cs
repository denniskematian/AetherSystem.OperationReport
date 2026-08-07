using AetherSystem.OperationReport.Internals;

namespace AetherSystem.OperationReport.DataSources;

public record DataSourceInfo
{
    public string FilePath { get; }
    public FileType Type { get; }
    
    public DataSourceInfo(string filePath, FileType type)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);
        ExceptionUtils.ThrowIfUndefined(type);

        FilePath = filePath;
        Type = type;
    }
}