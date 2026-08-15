using AetherSystem.OperationReport.DataSources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.SeriesFilterDialog;

public partial class SeriesFilterDialogViewModel : DialogViewModel<SampleFilterQuery>
{
    [ObservableProperty] public partial DateTime StartDate { get; set; } = DateTime.Now;

    [ObservableProperty] public partial DateTime EndDate { get; set; } = DateTime.Now;

    [ObservableProperty] public partial int? BatchNumber { get; set; }

    [ObservableProperty] public partial bool IncludeBatchNumber { get; set; }

    [ObservableProperty] public partial bool CanEditBatchNumber { get; set; }

    [RelayCommand]
    private void Confirm()
    {
        Complete(new SampleFilterQuery(StartDate, EndDate, BatchNumber));
    }
}