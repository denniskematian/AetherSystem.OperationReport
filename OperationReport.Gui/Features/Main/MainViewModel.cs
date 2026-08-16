using System.IO;
using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Gui.Services;
using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.ValueObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class MainViewModel : ObservableObject
{
    private const string Title = "Sensor Report";

    private readonly PackableRegistry _registry;

    [ObservableProperty] public partial string TitleText { get; private set; } = Title;
    [ObservableProperty] public partial object CurrentContent { get; private set; }

    public MainViewModel()
    {
        CurrentContent = new BlankViewModel();
        _registry = Facades.CreateMementoRegistry();
    }
    
    [RelayCommand]
    private async Task CreatePreset()
    {
        var result = Facades.DialogService.OpenPresetDialog();
        if(result.IsFailed)
            return;
        
        _registry.Put(nameof(PresetConfig), result.Value);
        
        var chartConfig = ChartConfig.CreateDefault(result.Value.SampleReferences);
        await InitializeDashboard(result.Value, chartConfig, "New Preset");
    }

    [RelayCommand]
    private async Task OpenPreset()
    {
        var result = Facades.DialogService.OpenPresetFileDialog();
        if(result.IsFailed)
            return;
        
        CurrentContent = new LoadingViewModel();
        await using (var stream = result.Value.OpenRead())
        {
            await _registry.LoadAsync(stream, true);
        }
        
        var presetConfig = _registry.Get<PresetConfig>(nameof(PresetConfig))
            ?? throw new InvalidOperationException("Preset config not found");

        var chartConfig = _registry.Get<ChartConfig>(nameof(ChartConfig)) 
                         ?? ChartConfig.CreateDefault(presetConfig.SampleReferences);
        
        await InitializeDashboard(presetConfig, chartConfig, result.Value.Name);
    }

    [RelayCommand(CanExecute = nameof(CanSavePreset))]
    private async Task SavePreset()
    {
        var result = Facades.DialogService.SavePresetFileDialog();
        if(result.IsFailed)
            return;

        var tempFile = new FileInfo(Path.GetTempFileName());
        await using (var file = tempFile.Create())
        {
            await _registry.SaveAsync(file);
        }

        var existingFile = result.Value;
        if(existingFile.Exists)
            existingFile.Delete();
            
        tempFile.MoveTo(existingFile.FullName);
        TitleText = Title + " - " + result.Value.Name;
    }
    
    private bool CanSavePreset()
    {
        return _registry.ContainsKey(nameof(PresetConfig));
    }

    private async Task InitializeDashboard(PresetConfig presetConfig, ChartConfig chartConfig, string name)
    {
        SavePresetCommand.NotifyCanExecuteChanged();
        TitleText = Title + " - " + name;

        var dashboardContext = new DashboardContext(presetConfig, chartConfig, _registry);
        CurrentContent = new DashboardViewModel(dashboardContext);
        var filterQuery = _registry.Get<SampleFilterQuery>(nameof(SampleFilterQuery));
        if(filterQuery is not null)
            await dashboardContext.ApplyFilterAsync(filterQuery);

        await dashboardContext.DiscoverFilterQueries();
    }
}