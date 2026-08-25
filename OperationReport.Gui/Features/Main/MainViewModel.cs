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

    [ObservableProperty] public partial string TitleText { get; private set; } = Title;
    [ObservableProperty] public partial object CurrentContent { get; private set; }
    
    public PackableRegistry Registry { get; }

    public MainViewModel()
    {
        CurrentContent = new BlankViewModel();
        Registry = Facades.CreateMementoRegistry();
    }
    
    [RelayCommand]
    private async Task CreatePreset()
    {
        var result = Facades.DialogService.OpenPresetDialog();
        if(result.IsFailed)
            return;
        
        var lastContent = CurrentContent;
        try
        {
            Registry.Put(nameof(PresetConfig), result.Value);
            
            var chartConfig = ChartConfig.CreateDefault(result.Value.SampleReferences);
            await InitializeDashboard(result.Value, chartConfig, "New Preset");
        }
        catch(Exception ex)
        {
            Facades.DialogService.ErrorDialog("Unable to create preset.\r\n" + ex.Message);
            CurrentContent = lastContent;
        }
    }

    [RelayCommand]
    private async Task OpenPreset()
    {
        var result = Facades.DialogService.OpenPresetFileDialog();
        if(result.IsFailed)
            return;

        var lastContent = CurrentContent;
        CurrentContent = new LoadingViewModel();
        try
        {
            await using (var stream = result.Value.OpenRead())
            {
                await Registry.LoadAsync(stream, true);
            }

            var presetConfig = Registry.Get<PresetConfig>(nameof(PresetConfig))
                               ?? throw new InvalidOperationException("Preset config not found");

            var chartConfig = Registry.Get<ChartConfig>(nameof(ChartConfig))
                              ?? ChartConfig.CreateDefault(presetConfig.SampleReferences);

            await InitializeDashboard(presetConfig, chartConfig, result.Value.Name);
        }
        catch (Exception ex)
        {
            Facades.DialogService.ErrorDialog("Unable to load preset.\r\n" + ex.Message);
            CurrentContent = lastContent;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSavePreset))]
    private async Task SavePreset()
    {
        var result = Facades.DialogService.SavePresetFileDialog();
        if(result.IsFailed)
            return;

        var tempFile = new FileInfo(Path.GetTempFileName());
        try
        {
            await using (var file = tempFile.Create())
            {
                await Registry.SaveAsync(file);
            }

            var existingFile = result.Value;
            if(existingFile.Exists)
                existingFile.Delete();
            
            tempFile.MoveTo(existingFile.FullName);
            TitleText = Title + " - " + result.Value.Name;
        }
        catch(Exception ex)
        {
            Facades.DialogService.ErrorDialog("Unable to save preset.\r\n" + ex.Message);
            if(tempFile.Exists)
                tempFile.Delete();
        }
    }
    
    private bool CanSavePreset()
    {
        return Registry.ContainsKey(nameof(PresetConfig));
    }

    private async Task InitializeDashboard(PresetConfig presetConfig, ChartConfig chartConfig, string name)
    {
        SavePresetCommand.NotifyCanExecuteChanged();
        TitleText = Title + " - " + name;

        var dashboardContext = new DashboardContext(presetConfig, chartConfig, Registry);
        CurrentContent = new DashboardViewModel(dashboardContext);
        var filterQuery = Registry.Get<SampleFilterQuery>(nameof(SampleFilterQuery));
        if(filterQuery is not null)
            await dashboardContext.ApplyFilterAsync(filterQuery);

        await dashboardContext.DiscoverFilterQueries();
    }
}