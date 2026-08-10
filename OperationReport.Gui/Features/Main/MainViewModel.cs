using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class MainViewModel : ObservableObject
{
    private const string Title = "Sensor Report";
    [ObservableProperty] public partial string TitleText { get; private set; } = Title;
    [ObservableProperty] public partial object CurrentContent { get; private set; }

    public MainViewModel()
    {
        CurrentContent = new BlankViewModel();
    }
    
    [RelayCommand]
    private async Task CreatePreset()
    {
        var result = Facades.DialogService.OpenPresetDialog();
        if(result.IsFailed)
            return;
        CurrentContent = new LoadingViewModel();
        await Task.Delay(1000);
        CurrentContent = new BlankViewModel();
    }

    [RelayCommand]
    private async Task OpenPreset()
    {
        
    }

    [RelayCommand]
    private async Task SavePreset()
    {
        
    }
}