using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.DataSources.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public partial class ReferenceMappingViewModel : ObservableObject, IPresetDialogContent
{
    private const string PreferredReferenceTable = "data_format";
    private readonly PresetDialogViewModel _parent;

    public PresetDialogContext Context { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ReferenceIntegerColumns))]
    [NotifyPropertyChangedFor(nameof(ReferenceTextColumns))]
    public partial Table? ReferenceTable { get; set; }
    [ObservableProperty] public partial Column? ReferenceIdColumn { get; set; }
    [ObservableProperty] public partial Column? ReferenceLabelColumn { get; set; }
    
    public IReadOnlyList<Column> ReferenceIntegerColumns
        => ReferenceTable?.Columns.Where(i => i.Type == ColumnType.Integer).ToArray() ?? [];

    public IReadOnlyList<Column> ReferenceTextColumns
        => ReferenceTable?.Columns.Where(i => i.Type == ColumnType.Text).ToArray() ?? [];

    /// <inheritdoc/>
    public ReferenceMappingViewModel(PresetDialogViewModel parent, PresetDialogContext context)
    {
        _parent = parent;
        Context = context;
        ReferenceTable = context.ReferenceTables
            .OrderByDescending(i => i.Name == PreferredReferenceTable)
            .ThenByDescending(i => i.Name.StartsWith(PreferredReferenceTable))
            .ThenByDescending(i => i.Name.EndsWith(PreferredReferenceTable))
            .ThenByDescending(i => i.Name.Contains(PreferredReferenceTable))
            .FirstOrDefault();
    }

    partial void OnReferenceTableChanged(Table? value)
    {
        if (value is null) return;
        ReferenceIdColumn = ReferenceIntegerColumns.FirstOrDefault();
        ReferenceLabelColumn = ReferenceTextColumns.FirstOrDefault();
    }

    [RelayCommand(CanExecute = nameof(CanProceed))]
    private async Task Proceed()
    {
        if(ReferenceTable is null)
            throw new InvalidOperationException("Reference table is not selected");
        
        if(ReferenceIdColumn is null)
            throw new InvalidOperationException("Reference id column is not selected");
        
        if(ReferenceLabelColumn is null)
            throw new InvalidOperationException("Reference label column is not selected");
        
        var dataSource = new ReferenceSourceInfo(
            Context.ReferenceDataSource.FilePath,
            Context.ReferenceDataSource.FileType,
            ReferenceTable,
            ReferenceIdColumn,
            ReferenceLabelColumn);
        
        await Context.InitializeReferenceAsync(dataSource);

        _parent.NextPage();
    }
    
    private bool CanProceed() => 
        ReferenceTable is not null 
        && ReferenceIdColumn is not null 
        && ReferenceLabelColumn is not null;

    // public void Restore(IMementoProxy registry)
    // {
    //     if(registry.TryGet(nameof(ReferenceTable), out var table))
    //         ReferenceTable = (Table)table;
    //     
    //     if(registry.TryGet(nameof(ReferenceIdColumn), out var idColumn))
    //         ReferenceIdColumn = (Column)idColumn;
    //     
    //     if(registry.TryGet(nameof(ReferenceLabelColumn), out var labelColumn))
    //         ReferenceLabelColumn = (Column)labelColumn;
    // }
    //
    // public void Capture(IMementoProxy registry)
    // {
    //     registry.Put(nameof(ReferenceTable), ReferenceTable);
    //     registry.Put(nameof(ReferenceIdColumn), ReferenceIdColumn);
    //     registry.Put(nameof(ReferenceLabelColumn), ReferenceLabelColumn);
    // }
}