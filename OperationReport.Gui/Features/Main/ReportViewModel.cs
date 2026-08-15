using MahApps.Metro.IconPacks;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public class ReportViewModel : DashboardContent
{
    public ReportViewModel(DashboardContext context) : base(context)
    {
        Title = "Batch Document";
        Icon = new PackIconMaterial { Kind = PackIconMaterialKind.FilePdfBox };
    }

    protected override void DataCollectorUpdated()
    {
    }
}