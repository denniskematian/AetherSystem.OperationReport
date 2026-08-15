using AetherSystem.OperationReport.DataSources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public abstract partial class DashboardContent : ObservableObject
{
    private SampleFilterQuery? _filterQuery;

    protected DashboardContent(DashboardContext context)
    {
        Context = context;
        context.DataCollectorUpdated += (_, _) => DataCollectorUpdated();
    }

    public DashboardContext Context { get; }

    public string Title { get; protected init; } = string.Empty;

    public PackIconBase Icon { get; protected init; } = new PackIconMaterial();

    [RelayCommand]
    private async Task ChangeFilter()
    {
        var filter = Facades.DialogService.OpenFilterQueryDialog(
            Context.FilterQuery,
            canEditBatchNumber: Context.PresetConfig.SampleDataSource.HasBatchNumberColumn);

        if (!filter.IsSuccess)
            return;

        if(_filterQuery == filter.Value)
            return;

        await Context.ApplyFilterAsync(filter.Value);
        DataCollectorUpdated();
        _filterQuery = filter.Value;
    }

    protected abstract void DataCollectorUpdated();
}