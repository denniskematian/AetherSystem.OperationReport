using System.IO;
using AetherSystem.OperationReport.ValueObjects;
using FluentResults;

namespace AetherSystem.OperationReport.Gui.Services;

public static class DialogServiceExtension
{
    extension(IDialogService dialogService)
    {
        public Result<FileInfo> OpenFileDialog()
        {
            return dialogService.OpenFileDialog("All Files (*.*)|*.*");
        }

        public Result<FileInfo> OpenDbFileDialog()
        {
            return dialogService.OpenFileDialog("SQLite File (*.db)|*.db|All Files (*.*)|*.*");
        }

        public Result<FileInfo> OpenCsvFileDialog()
        {
            return dialogService.OpenFileDialog("CSV File (*.csv)|*.csv|All Files (*.*)|*.*");
        }

        public Result<FileInfo> OpenJsonFileDialog()
        {
            return dialogService.OpenFileDialog("JSON File (*.json)|*.json|All Files (*.*)|*.*");
        }

        public Result<FileInfo> OpenPresetFileDialog()
        {
            return dialogService.OpenFileDialog("Preset File (*.mem)|*.mem|All Files (*.*)|*.*");
        }

        public Result<FileInfo> SavePresetFileDialog()
        {
            return dialogService.SaveFileDialog("Preset File (*.mem)|*.mem|All Files (*.*)|*.*");
        }

        // public Result<PresetConfig> OpenPresetDialog()
        // {
        //     return dialogService.OpenPresetDialog(new PackableMementoRegistry());
        // }
    }
}