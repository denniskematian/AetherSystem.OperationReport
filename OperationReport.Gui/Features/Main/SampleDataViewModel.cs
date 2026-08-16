using System.Collections.ObjectModel;
using AetherSystem.OperationReport.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class SampleDataViewModel : DashboardContent
{
    private const int PageSize = 100;
    private int MaxPage => (int)Math.Ceiling(TotalRows / (double)PageSize);
    
    public ObservableCollection<ObservableSample> Rows { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    public partial int TotalRows { get; private set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    public partial int CurrentPage { get; set; }
    
    public SampleDataViewModel(DashboardContext context) : base(context)
    {
        Title = "Sample Data";
        Icon = new PackIconMaterial { Kind = PackIconMaterialKind.Table };
    }

    [RelayCommand(CanExecute = nameof(CanNextPage))]
    private void NextPage()
    {
        ChangePage(CurrentPage + 1);
    }

    private bool CanNextPage()
    {
        return CurrentPage < MaxPage;
    }

    [RelayCommand(CanExecute = nameof(CanPreviousPage))]
    private void PreviousPage()
    {
        ChangePage(CurrentPage - 1);
    }

    private bool CanPreviousPage()
    {
        return CurrentPage > 1;
    }
    
    [RelayCommand]
    private void ChangePage(int page)
    {
        var pageRequest = new PageRequest(page, PageSize);
        var pageResult = Context.DataCollector.GetSamplePage(pageRequest);
        var items = pageResult.Items;

        var index = 0;
        for(; index < Rows.Count && index < items.Count; index++)
            Rows[index].Update(items[index]);

        for (; index < items.Count; index++)
        {
            var sample = items[index];
            var sampleRow = new ObservableSample(sample.Timestamp, sample.Values);
            Rows.Add(sampleRow);
        }

        while(Rows.Count > items.Count)
            Rows.RemoveAt(items.Count);
        
        CurrentPage = pageResult.Page;
        TotalRows = pageResult.TotalCount;
    }

    protected override void DataCollectorUpdated()
    {
        ChangePage(1);
    }
}