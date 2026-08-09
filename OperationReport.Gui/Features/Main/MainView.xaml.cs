namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class MainView
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}