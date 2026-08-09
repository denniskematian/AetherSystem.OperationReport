using AetherSystem.OperationReport.ValueObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public partial class PresetDialogViewModel : DialogViewModel<PresetConfig>
{
    private readonly IPresetDialogContent[] _pages;
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
        _pages = [
            new DefineDataSourceViewModel(this, _context),
            new ReferenceMappingViewModel(this, _context),
            new OperationMappingViewModel(this, _context),
        ];

        CurrentContent =  _pages[0];
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

        if (_currentPageIndex >= _pages.Length)
        {
            var presetConfig = _context.PresetConfigBuilder.Build();
            Complete(presetConfig);
            return;
        }

        PreviousText = _currentPageIndex == 0 ? "Cancel" : "Previous";
        NextText = _currentPageIndex == _pages.Length - 1 ? "Confirm" : "Next";

        // var next = GetCurrentPage();
        // next.Restore(_mementoRegistry.CreateProxy(next.GetType()));
        CurrentContent = _pages[_currentPageIndex];
    }
}