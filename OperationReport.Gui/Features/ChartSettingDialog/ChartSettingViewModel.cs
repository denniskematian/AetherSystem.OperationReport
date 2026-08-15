using AetherSystem.OperationReport.Charting;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.ChartSettingDialog;

public partial class ChartSettingViewModel : DialogViewModel<ChartConfig>
{
    [ObservableProperty] public required partial AxisConfig LeftAxis { get; set; }
    [ObservableProperty] public required partial AxisConfig RightAxis { get; set; }
    [ObservableProperty] public required partial IReadOnlyList<SeriesConfig> Series { get; set; }
    [ObservableProperty] public required partial MarkerConfig OperationMarker { get; set; }
    [ObservableProperty] public required partial bool ShowDateInBottomTicks { get; set; }
    
    public IReadOnlyList<string> SeriesColumns => Series.Select(s => s.Column).ToArray();
    
    [RelayCommand]
    private void Confirm()
    {
        Complete(new ChartConfig()
        {
            LeftAxis = LeftAxis,
            RightAxis = RightAxis,
            Series = Series,
            OperationMarker = OperationMarker,
            ShowDateInBottomTicks = ShowDateInBottomTicks
        });
    }
}