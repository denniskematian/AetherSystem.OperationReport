using System.ComponentModel;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class MainView
{
    private readonly MainViewModel _viewModel;
    public MainView()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
    }

    private void MainView_OnClosing(object? sender, CancelEventArgs e)
    {
        if (!_viewModel.Registry.IsDirty)
            return;

        var dialogResult = Facades.DialogService.ConfirmDialog(
            message: "Do you want to save your changes before closing?",
            caption: "Unsaved Changes");

        if (dialogResult.IsFailed)
            return;
        
        _viewModel.SavePresetCommand.Execute(null);
    }
}