namespace AetherSystem.OperationReport.DataSources;

public record DataSourceInfo
{
    public string FilePath { get; }
    public FileType Type { get; }
    
    public DataSourceInfo(string FilePath, FileType Type)
    {
        if(!Enum.IsDefined(Type))
            throw new ArgumentException($"Invalid file type ({(int)Type})");

        this.FilePath = FilePath;
        this.Type = Type;
    }
}