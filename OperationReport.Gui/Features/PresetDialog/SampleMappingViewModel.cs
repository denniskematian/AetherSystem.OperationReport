using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Gui.Options;
using AetherSystem.OperationReport.Timestamps;
using AetherSystem.OperationReport.ValueObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public sealed partial class SampleMappingViewModel : ObservableObject, IPresetDialogContent
{
    private readonly PresetDialogViewModel _parent;

    private const string PreferredTable = "data";
    private const string PreferredTimestampColumn = "timestamp";
    private const string PreferredColumnFormat = "data_format_";

    public PresetDialogContext Context { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SampleRealColumns))]
    [NotifyPropertyChangedFor(nameof(BatchNumberColumnOptions))]
    public partial Table? SampleTable { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SampleTimestampResolutionRequired))]
    public partial Column? SampleTimestampColumn { get; set; }

    [ObservableProperty]
    public partial TimestampResolution SampleTimestampResolution { get; set; } = TimestampResolution.Second;
    public bool SampleTimestampResolutionRequired => SampleTimestampColumn is { Type: ColumnType.Real or ColumnType.Integer };

    [ObservableProperty] 
    public partial Column? SampleBatchNumberColumn { get; set; }
    
    [ObservableProperty] 
    public partial IReadOnlyList<SampleReferenceConfig> SampleReferences { get; private set; } = [];
    
    private IReadOnlyList<Column> SampleIntegerColumns
        => SampleTable?.Columns.Where(i => i.Type == ColumnType.Integer).ToArray() ?? [];

    public IReadOnlyList<Option<Column?>> BatchNumberColumnOptions 
        => Options.WithNone(SampleIntegerColumns, i => i.Name);

    public IReadOnlyList<Column> SampleRealColumns
        => SampleTable?.Columns.Where(i => i.Type == ColumnType.Real).ToArray() ?? [];

    public SampleMappingViewModel(PresetDialogViewModel parent, PresetDialogContext context)
    {
        _parent = parent;
        Context = context;
        InitializePreferredTable();
    }

    [RelayCommand(CanExecute = nameof(CanProceed))]
    private Task Proceed()
    {
        if(SampleTable is null)
            throw new InvalidOperationException("Sample table is not set");

        if(SampleTimestampColumn is null)
            throw new InvalidOperationException("Sample timestamp column is not set");
        
        Context.PresetConfigBuilder.WithSampleDataSource(
            SampleReferences,
            SampleTable,
            SampleTimestampColumn,
            SampleTimestampResolution,
            SampleBatchNumberColumn);
        
        _parent.NextPage();
        return Task.CompletedTask;
    }

    private bool CanProceed()
    {
        return SampleTable is not null
               && SampleTimestampColumn is not null
               && SampleReferences.Any(i => i.IsIncluded);
    }

    partial void OnSampleTableChanged(Table? value)
    {
        if (value is null)
            return;

        InitializePreferredTimestampColumn(value);
        InitializePreferredSampleReferences(value);
    }

    private void InitializePreferredTable()
    {
        SampleTable = Context.SampleTables
            .OrderByDescending(i => i.Name == PreferredTable)
            .ThenByDescending(i => i.Name.StartsWith(PreferredTable))
            .ThenByDescending(i => i.Name.Contains(PreferredTable))
            .ThenByDescending(i => i.Name.EndsWith(PreferredTable))
            .FirstOrDefault();
    }

    private void InitializePreferredTimestampColumn(Table sampleTable)
    {
        SampleTimestampColumn = sampleTable.Columns.Count == 0
            ? null
            : (
                from column in sampleTable.Columns
                orderby column.Name == PreferredTimestampColumn descending,
                    column.Name.StartsWith(PreferredTimestampColumn) descending,
                    column.Name.EndsWith(PreferredTimestampColumn) descending,
                    column.Name.Contains(PreferredTimestampColumn) descending
                select column).FirstOrDefault();
    }
    
    private void InitializePreferredSampleReferences(Table sampleTable)
    {
        List<SampleReferenceConfig> sampleReferences = [];
        var references = Context.SampleReferences;
        foreach (var column in sampleTable.Columns)
        {
            if(column.Type is not (ColumnType.Integer or ColumnType.Real)) 
                continue;

            var isIncluded = column.Name.StartsWith(PreferredColumnFormat);
            if (!isIncluded || !int.TryParse(column.Name.AsSpan(PreferredColumnFormat.Length), out var id))
            {
                id = -1;
                isIncluded = false;
            }

            var label = references.FirstOrDefault(i => i.Id == id)?.Label ?? column.Name;
            var sampleReference = new SampleReferenceConfig
            {
                Column = column.Name,
                IsIncluded = isIncluded,
                Index = id,
                Label = label,
            };
            
            sampleReferences.Add(sampleReference);
        }

        SampleReferences = sampleReferences;
    }
}