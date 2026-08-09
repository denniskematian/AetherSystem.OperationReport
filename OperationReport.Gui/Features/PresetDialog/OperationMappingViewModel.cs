using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Timestamps;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public partial class OperationMappingViewModel : ObservableObject, IPresetDialogContent
{
    private const string PreferredTable = "data";
    private const string PreferredTimestampColumn = "timestamp";
    private const string PreferredCommentColumn = "comment";

    private readonly PresetDialogViewModel _parent;
    public PresetDialogContext Context { get; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OperationTextColumns))]
    public partial Table? OperationTable { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OperationTimestampResolutionRequired))]
    public partial Column? OperationTimestampColumn { get; set; }

    [ObservableProperty]
    public partial Column? OperationCommentColumn { get; set; }
    
    [ObservableProperty]
    public partial TimestampResolution OperationTimestampResolution { get; set; } = TimestampResolution.Second;
    public bool OperationTimestampResolutionRequired => OperationTimestampColumn is { Type: ColumnType.Real or ColumnType.Integer };

    public IReadOnlyList<Column> OperationTextColumns
        => OperationTable?.Columns.Where(i => i.Type == ColumnType.Text).ToArray() ?? [];

    /// <inheritdoc/>
    public OperationMappingViewModel(PresetDialogViewModel parent, PresetDialogContext context)
    {
        _parent = parent;
        Context = context;
        InitializePreferredTable();
    }

    [RelayCommand(CanExecute = nameof(CanProceed))]
    private Task Proceed()
    {
        try
        {
            if(OperationTable is null)
                throw new InvalidOperationException("Operation table is not selected");
            
            if(OperationTimestampColumn is null)
                throw new InvalidOperationException("Operation timestamp column is not selected");
            
            if(OperationCommentColumn is null)
                throw new InvalidOperationException("Operation comment column is not selected");

            Context.PresetConfigBuilder.WithOperationDataSource(
                OperationTable,
                OperationTimestampColumn,
                OperationTimestampResolution,
                OperationCommentColumn);

            _parent.NextPage();
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    private bool CanProceed()
    {
        return OperationTable is not null
            && OperationTimestampColumn is not null
            && OperationCommentColumn is not null;
    }

    partial void OnOperationTableChanged(Table? value)
    {
        if (value is null) return;
        
        InitializePreferredCommentColumn(value);
        InitializePreferredTimestampColumn(value);
    }

    private void InitializePreferredTable()
    {
        OperationTable = Context.OperationTables
            .OrderByDescending(i => i.Name == PreferredTable)
            .ThenByDescending(i => i.Name.StartsWith(PreferredTable))
            .ThenByDescending(i => i.Name.Contains(PreferredTable))
            .ThenByDescending(i => i.Name.EndsWith(PreferredTable))
            .FirstOrDefault();
    }

    private void InitializePreferredTimestampColumn(Table table)
    {
        OperationTimestampColumn = table.Columns.Count == 0
            ? null
            : (
                from column in table.Columns
                orderby column.Name == PreferredTimestampColumn descending,
                    column.Name.StartsWith(PreferredTimestampColumn) descending,
                    column.Name.EndsWith(PreferredTimestampColumn) descending,
                    column.Name.Contains(PreferredTimestampColumn) descending
                select column).FirstOrDefault();
    }

    private void InitializePreferredCommentColumn(Table table)
    {
        OperationCommentColumn = table.Columns.Count == 0
            ? null
            : (
                from column in table.Columns
                orderby column.Name == PreferredCommentColumn descending,
                    column.Name.StartsWith(PreferredCommentColumn) descending,
                    column.Name.EndsWith(PreferredCommentColumn) descending,
                    column.Name.Contains(PreferredCommentColumn) descending
                select column).FirstOrDefault();
    }

    // public void Restore(IMementoProxy registry)
    // {
    //     if(registry.TryGet(nameof(OperationTable), out var table))
    //         OperationTable = (Table)table;
    //
    //     if(registry.TryGet(nameof(OperationCommentColumn), out var commentColumn))
    //         OperationCommentColumn = (Column)commentColumn;
    //
    //     if (registry.TryGet(nameof(OperationTimestampColumn), out var timestampColumn))
    //     {
    //         var column = (DateTimeColumn)timestampColumn;
    //         OperationTimestampColumn = new Column(column.Name, column.Type);
    //         OperationTimestampResolution = column.Resolution;
    //     }
    // }
    //
    // public void Capture(IMementoProxy registry)
    // {
    //     registry.Put(nameof(OperationTable), OperationTable);
    //     registry.Put(nameof(OperationCommentColumn), OperationCommentColumn);
    //
    //     DateTimeColumn? timestampColumn = null;
    //     if (OperationTimestampColumn is not null)
    //     {
    //         timestampColumn = new DateTimeColumn(
    //             OperationTimestampColumn.Name,
    //             OperationTimestampColumn.Type,
    //             Resolution: OperationTimestampResolution);
    //     }
    //
    //     registry.Put(nameof(OperationTimestampColumn), timestampColumn);
    // }
}