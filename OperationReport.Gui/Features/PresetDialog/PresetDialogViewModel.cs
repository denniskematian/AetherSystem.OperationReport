using AetherSystem.OperationReport.ValueObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public partial class PresetDialogViewModel : DialogViewModel<PresetConfig>
{
    private readonly IReadOnlyList<Type> _pageTypes = [
        typeof(DefineDataSourceViewModel),
        typeof(ReferenceMappingViewModel),
        typeof(OperationMappingViewModel),
        typeof(SampleMappingViewModel),
    ];
    
    private readonly PresetDialogContext _context;
    private int _currentPageIndex;
    
    [ObservableProperty] public partial string TitleText { get; private set; } = "New Preset";

    [ObservableProperty]
    public partial IPresetDialogContent CurrentContent { get; private set; }

    [ObservableProperty] public partial string PreviousText { get; private set; } = "Cancel";
    [ObservableProperty] public partial string NextText { get; private set; } = "Next";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProcessing))]
    public partial string? ProcessingMessage { get; set; }
    public bool IsProcessing => !string.IsNullOrEmpty(ProcessingMessage);

    public PresetDialogViewModel()
    {
        _context = new PresetDialogContext();
        CurrentContent =  CreateContent();
    }

    [RelayCommand]
    public void PreviousPage()
    {
        _currentPageIndex--;
        UpdateContent();
    }

    public void NextPage()
    {
        _currentPageIndex++;
        // var registry = _mementoRegistry.CreateProxy(CurrentContent.GetType());
        // CurrentContent.Capture(registry);
        UpdateContent();
    }

    private void UpdateContent()
    {
        if (_currentPageIndex < 0)
        {
            SetCancel();
            return;
        }

        if (_currentPageIndex >= _pageTypes.Count)
        {
            var presetConfig = _context.PresetConfigBuilder.Build();
            Complete(presetConfig);
            return;
        }

        PreviousText = _currentPageIndex == 0 ? "Cancel" : "Previous";
        NextText = _currentPageIndex == _pageTypes.Count - 1 ? "Confirm" : "Next";

        CurrentContent = CreateContent();
    }

    private IPresetDialogContent CreateContent()
    {
        var content = Activator.CreateInstance(_pageTypes[_currentPageIndex], this, _context) as IPresetDialogContent;
        return content ?? throw new InvalidOperationException("Failed to create instance of IPresetDialogContent");
    }
}