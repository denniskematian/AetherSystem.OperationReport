using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using QuestPDF.Infrastructure;

namespace AetherSystem.OperationReport.Gui;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly Lock s_exceptionLogLock = new();

    public App()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        InitializeComponent();
    }

    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogException("Unhandled UI dispatcher exception", e.Exception);

        MessageBox.Show(
            "An unexpected error occurred. The application will try to continue running.\n\n" +
            $"Details were written to:\n{GetExceptionLogPath()}",
            "Unexpected Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.Handled = true;
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        LogException(
            e.IsTerminating
                ? "Unhandled fatal application exception"
                : "Unhandled application exception",
            e.ExceptionObject as Exception,
            e.ExceptionObject);
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogException("Unobserved task exception", e.Exception);
        e.SetObserved();
    }

    private static void LogException(string title, Exception? exception, object? exceptionObject = null)
    {
        try
        {
            var builder = new StringBuilder();

            builder.AppendLine("============================================================");
            builder.AppendLine(title);
            builder.AppendLine($"Timestamp: {DateTimeOffset.Now:O}");
            builder.AppendLine();

            if (exception is not null)
            {
                builder.AppendLine(exception.ToString());
            }
            else if (exceptionObject is not null)
            {
                builder.AppendLine(exceptionObject.ToString());
            }
            else
            {
                builder.AppendLine("No exception details were available.");
            }

            builder.AppendLine();

            using var _ = s_exceptionLogLock.EnterScope();
            
            var logPath = GetExceptionLogPath();
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.AppendAllText(logPath, builder.ToString());
        }
        catch
        {
            // Never allow logging failures to trigger another unhandled exception.
        }
    }

    private static string GetExceptionLogPath()
    {
        var logDirectory = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Logs");

        var dt = DateTime.Now.ToString("yyMMdd-HHmmss");
        return Path.Combine(logDirectory, dt + "-unhandled-exceptions.log");
    }
}