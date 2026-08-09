using System.ComponentModel;
using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Entities;
using AetherSystem.OperationReport.ValueObjects;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public sealed class PresetDialogContext
{
    public IReadOnlyList<Table> SampleTables { get; private set; } = [];
    public IReadOnlyList<Table> ReferenceTables { get; private set; } = [];
    public IReadOnlyList<Table> OperationTables { get; private set; } = [];
    public IReadOnlyList<SampleReference> SampleReferences { get; private set; } = [];

    public ObservableDataSourceInfo SampleDataSource { get; } = new();
    public ObservableDataSourceInfo OperationDataSource { get; } = new();
    public ObservableDataSourceInfo ReferenceDataSource { get; } = new();
    
    public PresetConfigBuilder PresetConfigBuilder => field ??= new PresetConfigBuilder(this);

    public async Task InitializeSampleTablesAsync(DataSourceInfo dataSource)
    {
        await using var adapter = Facades.DataSourceFactory.CreateDataSourceAdapter(dataSource);
        SampleTables = await adapter.GetTablesAsync().ToArrayAsync();
    }

    public async Task InitializeReferenceTablesAsync(DataSourceInfo dataSource)
    {
        await using var adapter = Facades.DataSourceFactory.CreateDataSourceAdapter(dataSource);
        ReferenceTables = await adapter.GetTablesAsync().ToArrayAsync();
    }

    public async Task InitializeOperationTablesAsync(DataSourceInfo dataSource)
    {
        await using var adapter = Facades.DataSourceFactory.CreateDataSourceAdapter(dataSource);
        OperationTables = await adapter.GetTablesAsync().ToArrayAsync();
    }

    public async Task InitializeReferenceAsync(ReferenceSourceInfo dataSource)
    {
        await using var adapter = Facades.DataSourceFactory.CreateReferenceTableAdapter(dataSource);
        SampleReferences = await adapter.EnumerateAsync().ToArrayAsync();
    }
}