using QuestPDF.Infrastructure;

namespace AetherSystem.OperationReport.Gui;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        InitializeComponent();
    }
}