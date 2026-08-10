using System.ComponentModel;
using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Gui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public partial class DefineDataSourceViewModel : ObservableObject, IPresetDialogContent
{
    private readonly PresetDialogViewModel _parent;
    public PresetDialogContext Context { get; }

    public DefineDataSourceViewModel(PresetDialogViewModel parent, PresetDialogContext context)
    {
        _parent = parent;
        Context = context;
        
        Context.SampleDataSource.PropertyChanged += HandleFilePathChanged;
        Context.OperationDataSource.PropertyChanged += HandleFilePathChanged;
        Context.ReferenceDataSource.PropertyChanged += HandleFilePathChanged;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ProceedCommand))]
    public partial bool SampleIncludesReferenceDataSource { get; set; } = true;

    [RelayCommand(CanExecute = nameof(CanProceed))]
    private async Task Proceed()
    {
        var sampleDataSource = Context.SampleDataSource.ToDataSourceInfo();
        await Context.InitializeSampleTablesAsync(sampleDataSource);

        if (SampleIncludesReferenceDataSource)
        {
            Context.ReferenceDataSource.FilePath = sampleDataSource.FilePath;
            Context.ReferenceDataSource.FileType = sampleDataSource.FileType;
        }
        
        var referenceDataSource = SampleIncludesReferenceDataSource
            ? sampleDataSource
            : Context.ReferenceDataSource.ToDataSourceInfo();
        
        await Context.InitializeReferenceTablesAsync(referenceDataSource);
        await Context.InitializeOperationTablesAsync(Context.OperationDataSource.ToDataSourceInfo());
        
        _parent.NextPage();
    }

    [RelayCommand]
    private void OpenFile(string name)
    {
        var source = name switch
        {
            "Sample" => Context.SampleDataSource,
            "Reference" => Context.ReferenceDataSource,
            "Operation" => Context.OperationDataSource,
            _ => throw new ArgumentOutOfRangeException(nameof(name))
        };

        var dialogResult = source.FileType switch
        {
            FileType.Sqlite => Facades.DialogService.OpenDbFileDialog(),
            FileType.Csv => Facades.DialogService.OpenCsvFileDialog(),
            _ => Facades.DialogService.OpenFileDialog()
        };
        
        if (dialogResult.IsSuccess) source.FilePath = dialogResult.Value.FullName;
    }

    private bool CanProceed()
    {
        var samplePath = Context.SampleDataSource.FilePath;
        var operationPath = Context.OperationDataSource.FilePath;
        var referencePath = Context.ReferenceDataSource.FilePath;
        
        return !string.IsNullOrEmpty(samplePath)
               && !string.IsNullOrEmpty(operationPath)
               && (SampleIncludesReferenceDataSource || !string.IsNullOrEmpty(referencePath));
    }

    private void HandleFilePathChanged(object? _, PropertyChangedEventArgs args)
    {
        if(args.PropertyName is nameof(ObservableDataSourceInfo.FilePath))
            ProceedCommand.NotifyCanExecuteChanged();
    }
}