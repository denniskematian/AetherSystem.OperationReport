using MahApps.Metro.IconPacks;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public class ChartPreviewViewModel : DashboardContent
{
    public ChartPreviewViewModel(DashboardContext context) : base(context)
    {
        Title = "Chart";
        Icon = new PackIconMaterial { Kind = PackIconMaterialKind.ChartBar };
    }
}