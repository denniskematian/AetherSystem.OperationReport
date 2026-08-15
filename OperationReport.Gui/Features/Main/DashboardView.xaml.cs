using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
    {
        HamburgerMenuControl.Content = args.InvokedItem;
    }
}