using System.Windows;
using System.Windows.Controls;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class ChartPreviewView : UserControl
{
    public ChartPreviewView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is not ChartPreviewViewModel viewModel)
            return;

        viewModel.Context.InitializeChart(WpfScottPlot.Plot);
    }
}