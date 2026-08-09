using System.Windows;
using AetherSystem.OperationReport.DataSources;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace AetherSystem.OperationReport.Gui;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // public new static App Current => (App)Application.Current;
    //
    // public IServiceProvider Services { get; } = ConfigureServices();
    //
    // private static ServiceProvider ConfigureServices()
    // {
    //     var serviceCollection = new ServiceCollection();
    //
    //     // serviceCollection.AddTransient<IDialogService, DialogService>();
    //     serviceCollection.AddTransient<IDataSourceAdapterFactory, DataSourceAdapterFactory>();
    //
    //     // serviceCollection.AddTransient<MainViewModel>();
    //     // serviceCollection.AddTransient<PlotConfigFactory>();
    //
    //     return serviceCollection.BuildServiceProvider();
    // }

    public App()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        InitializeComponent();
    }
}