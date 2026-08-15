using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class DashboardViewModel : ObservableObject
{
    public IReadOnlyList<DashboardContent> Pages { get; }

    [ObservableProperty] 
    public partial DashboardContent CurrentPage { get; set; }

    public DashboardViewModel(DashboardContext context)
    {
        Pages = [
            new ChartPreviewViewModel(context),
            new SampleDataViewModel(context),
            new ReportViewModel(context),
        ];

        CurrentPage = Pages[0];
    }

    [RelayCommand]
    private void ChangePage()
    {
        
    }
}