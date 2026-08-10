using AetherSystem.OperationReport.DataSources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public sealed partial class ObservableDataSourceInfo : ObservableObject
{
    [ObservableProperty] public partial string FilePath { get; set; } = string.Empty;
    [ObservableProperty] public partial FileType FileType { get; set; } = FileType.Sqlite;
    
    public DataSourceInfo ToDataSourceInfo() => new(FilePath, FileType);
}