using System.IO;
using System.Windows;
using FluentResults;
using Microsoft.Win32;

namespace AetherSystem.OperationReport.Gui.Services;

public interface IDialogService
{
    Result<FileInfo> OpenFileDialog(string filter);
    Result<FileInfo> SaveFileDialog(string filter);
    Result ConfirmDialog(string message, string caption = "Confirm", MessageBoxImage icon = MessageBoxImage.None);
    void ErrorDialog(string message, string caption = "Error");
}

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
}