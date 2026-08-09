using CommunityToolkit.Mvvm.ComponentModel;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class BlankViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PlaceholderText { get; set; } = "Data source is not configured.\r\nPlease click new or open preset to continue.";
}