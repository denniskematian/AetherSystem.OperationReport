using System.IO;
using System.Windows;
using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Gui.Features;
using AetherSystem.OperationReport.Gui.Features.ChartSettingDialog;
using AetherSystem.OperationReport.Gui.Features.PresetDialog;
using AetherSystem.OperationReport.Gui.Features.SeriesFilterDialog;
using AetherSystem.OperationReport.ValueObjects;
using AetherSystem.SensorReport.Gui.Features.SeriesFilterDialog;
using FluentResults;
using Microsoft.Win32;

namespace AetherSystem.OperationReport.Gui.Services;

internal class DialogService : IDialogService
{
    public Result<FileInfo> OpenFileDialog(string filter)
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter
        };

        return dialog.ShowDialog() is true
            ? Result.Ok(new FileInfo(dialog.FileName))
            : Result.Fail<FileInfo>("No file selected");
    }

    public Result<FileInfo> SaveFileDialog(string filter)
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter
        };

        return dialog.ShowDialog() is true
            ? Result.Ok(new FileInfo(dialog.FileName))
            : Result.Fail<FileInfo>("No file selected");
    }

    public Result ConfirmDialog(string message, string caption = "Confirm", MessageBoxImage icon = MessageBoxImage.None)
    {
        return MessageBox.Show(message, caption, MessageBoxButton.OKCancel, icon) is MessageBoxResult.OK
            ? Result.Ok()
            : Result.Fail("Operation canceled");
    }

    public void ErrorDialog(string message, string caption = "Error")
    {
        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public Result<PresetConfig> OpenPresetDialog()
    {
        var view = new PresetDialogView();
        var viewModel = new PresetDialogViewModel();
        return ShowDialog<PresetDialogViewModel, PresetConfig>(view, viewModel);
    }

    public Result<SampleFilterQuery> OpenFilterQueryDialog(SampleFilterQuery? query, bool canEditBatchNumber = true)
    {
        var view = new SeriesFilterDialogView();
        var viewModel = new SeriesFilterDialogViewModel
        {
            StartDate = query?.From ?? DateTime.Now,
            EndDate = query?.To ?? DateTime.Now,
            BatchNumber = query?.BatchNumber,
            IncludeBatchNumber = query?.BatchNumber is not null,
            CanEditBatchNumber = canEditBatchNumber
        };
        return ShowDialog<SeriesFilterDialogViewModel, SampleFilterQuery>(view, viewModel);
    }

    public Result<ChartConfig> OpenPlotConfigDialog(ChartConfig config)
    {
        var view = new ChartSettingView();
        var viewModel = new ChartSettingViewModel
        {
            LeftAxis = config.LeftAxis,
            RightAxis = config.RightAxis,
            Series = config.Series,
            OperationMarker = config.OperationMarker,
            ShowDateInBottomTicks = config.ShowDateInBottomTicks
        };
        return ShowDialog<ChartSettingViewModel, ChartConfig>(view, viewModel);
    }

    private static Result<TResult> ShowDialog<TViewModel, TResult>(Window view, TViewModel viewModel) where TViewModel : DialogViewModel<TResult>
    {
        view.Owner = Application.Current.MainWindow;
        view.DataContext = viewModel;
        Result<TResult>? result = null;

        viewModel.CompleteHandler = CompleteHandler;
        
        return view.ShowDialog() is null || result is null
            ? Result.Fail("Operation canceled") 
            : result;

        void CompleteHandler(Result<TResult> value)
        {
            result = value;
            view.Close();
            viewModel.CompleteHandler = null;
        }
    }
}