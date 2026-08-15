using System.Windows;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public sealed partial class ChartPreviewViewModel : DashboardContent
{
    public ChartPreviewViewModel(DashboardContext context) : base(context)
    {
        Title = "Chart";
        Icon = new PackIconMaterial { Kind = PackIconMaterialKind.ChartBar };
    }

    [RelayCommand]
    private void ChangeChartConfig()
    {
        var controller = Context.ChartController;
        if(controller is null)
            return;

        var result = Facades.DialogService.OpenPlotConfigDialog(Context.ChartConfig);
        if(!result.IsSuccess)
            return;
        
        Context.UpdateChartConfig(result.Value);
    }

    protected override void DataCollectorUpdated()
    {
        var controller = Context.ChartController;
        if(controller is not null)
            Application.Current.Dispatcher.Invoke(() => controller.Refresh());
    }
}