using CommunityToolkit.Mvvm.ComponentModel;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class LoadingViewModel : ObservableObject
{
    public string LoadingText { get; set; } = "Loading...";
}