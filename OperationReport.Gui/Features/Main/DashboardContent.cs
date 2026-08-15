using System.ComponentModel;
using System.Windows;
using AetherSystem.OperationReport.DataSources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public abstract partial class DashboardContent(DashboardContext context) : ObservableObject
{
    private SampleFilterQuery? _filterQuery;
    public DashboardContext Context { get; } = context;

    public string Title { get; protected init; } = string.Empty;

    public PackIconBase Icon { get; protected init; } = new PackIconMaterial();

    [RelayCommand]
    private async Task ChangeFilter()
    {
        var filter = Facades.DialogService.OpenFilterQueryDialog(
            Context.FilterQuery,
            canEditBatchNumber: Context.PresetConfig.SampleDataSource.BatchNumberColumn is not null);

        if (!filter.IsSuccess)
            return;

        if(_filterQuery == filter.Value)
            return;

        await Context.ApplyFilterAsync(filter.Value);
        ChangeFilter(filter.Value);
        _filterQuery = filter.Value;
    }

    protected virtual void ChangeFilter(SampleFilterQuery filterQuery)
    {
    }
}