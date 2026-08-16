using System.Collections.ObjectModel;
using System.Windows;
using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Collections;
using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.DataSources.Sqlite;
using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.ValueObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class DashboardContext : ObservableObject
{
    public PackableRegistry Registry { get; }
    public DataCollector DataCollector { get; }
    public PresetConfig PresetConfig { get; }
    public ChartConfig ChartConfig { get; private set; }
    public ChartController? ChartController { get; private set; }
    
    [ObservableProperty]
    public partial SampleFilterQuery? FilterQuery { get; set; }

    public ObservableCollection<SampleFilterQuery> FilterQueries { get; } = [];
    
    public event EventHandler? DataCollectorUpdated;
    
    public DashboardContext(PresetConfig presetConfig, ChartConfig chartConfig, PackableRegistry registry)
    {
        PresetConfig = presetConfig;
        ChartConfig = chartConfig;
        Registry = registry;
        
        var sampleDataAdapter = Facades.DataSourceFactory.CreateSampleTableAdapter(presetConfig.SampleDataSource);
        var operationDataAdapter = Facades.DataSourceFactory.CreateOperationTableAdapter(presetConfig.OperationDataSource);
        DataCollector = new DataCollector(sampleDataAdapter, operationDataAdapter);
    }
    
    public async Task DiscoverFilterQueries()
    {
        if(PresetConfig.SampleDataSource.FileType is not FileType.Sqlite)
            return;

        var sampleDataAdapter = Facades.DataSourceFactory
                .CreateSampleTableAdapter(PresetConfig.SampleDataSource) as SampleTableAdapter;
        
        if(sampleDataAdapter is null)
            return;

        await Task.Run(async () =>
        {
            var filterQueries = await sampleDataAdapter
                .DiscoverActiveSignalRangesAsync()
                .ToArrayAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var query in filterQueries)
                    FilterQueries.Add(query);
            });
        });
    }
    
    public async Task ApplyFilterAsync(SampleFilterQuery? query)
    {
        if(query is null)
            return;

        FilterQueries.Remove(query);
        FilterQueries.Insert(0, query);
        FilterQuery = query;

        await DataCollector.UpdateDataAsync(query);
    }

    partial void OnFilterQueryChanged(SampleFilterQuery? value)
    {
        if(value is null)
            return;

        Application.Current.Dispatcher.Invoke(async () =>
        {
            await DataCollector.UpdateDataAsync(value);
            DataCollectorUpdated?.Invoke(this, EventArgs.Empty);
        });
    }

    public void InitializeChart(Plot plot)
    {
        ChartController = new ChartController(plot, DataCollector);
        ChartController.UpdateConfiguration(ChartConfig);
    }
    
    public void UpdateChartConfig(ChartConfig chartConfig)
    {
        if(ChartController is null)
            return;

        ChartConfig = chartConfig;
        ChartController.UpdateConfiguration(ChartConfig);
    }
}