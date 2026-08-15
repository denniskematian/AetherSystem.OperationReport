using System.IO;
using System.Windows;
using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.ValueObjects;
using FluentResults;

namespace AetherSystem.OperationReport.Gui.Services;

public interface IDialogService
{
    Result<FileInfo> OpenFileDialog(string filter);
    Result<FileInfo> SaveFileDialog(string filter);
    Result ConfirmDialog(string message, string caption = "Confirm", MessageBoxImage icon = MessageBoxImage.None);
    void ErrorDialog(string message, string caption = "Error");
    Result<PresetConfig> OpenPresetDialog();
    Result<SampleFilterQuery> OpenFilterQueryDialog(SampleFilterQuery? query, bool canEditBatchNumber = true);
    Result<ChartConfig> OpenPlotConfigDialog(ChartConfig config);
}