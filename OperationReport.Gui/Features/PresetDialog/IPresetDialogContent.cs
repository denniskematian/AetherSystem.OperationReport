using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public interface IPresetDialogContent
{
    IAsyncRelayCommand ProceedCommand { get; }
}