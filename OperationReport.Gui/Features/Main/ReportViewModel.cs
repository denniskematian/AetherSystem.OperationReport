using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public partial class ReportViewModel : DashboardContent
{
    private const string ImageFilter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
    private const string PdfFilter = "PDF document (*.pdf)|*.pdf";

    [ObservableProperty] public partial string ProgramNumber { get; set; } = string.Empty;
    [ObservableProperty] public partial string BatchNumber { get; set; } = string.Empty;
    [ObservableProperty] public partial string ReportTitle { get; set; } = "Batch Report";
    [ObservableProperty] public partial string SerialNumber { get; set; } = string.Empty;
    [ObservableProperty] public partial string CompanyName { get; set; } = string.Empty;
    [ObservableProperty] public partial string CompanyLogoPath { get; private set; } = string.Empty;
    [ObservableProperty] public partial string ProgramType { get; set; } = string.Empty;
    [ObservableProperty] public partial string StartedBy { get; set; } = string.Empty;
    [ObservableProperty] public partial bool IsReleased { get; set; }
    public ObservableCollection<ProgramParameterInput> Parameters { get; } = [];
    public ObservableCollection<ProgramMessageInput> Messages { get; } = [];
    
    public ReportViewModel(DashboardContext context) : base(context)
    {
        Title = "Batch Document";
        Icon = new PackIconMaterial { Kind = PackIconMaterialKind.FilePdfBox };
    }

    protected override void DataCollectorUpdated()
    {
    }

    [RelayCommand]
    private void SelectCompanyLogo()
    {
        var result = Facades.DialogService.OpenFileDialog(ImageFilter);
        if (result.IsSuccess)
            CompanyLogoPath = result.Value.FullName;
    }

    [RelayCommand]
    private void AddParameter() => Parameters.Add(new ProgramParameterInput());

    [RelayCommand]
    private void RemoveParameter(ProgramParameterInput? parameter)
    {
        if (parameter is not null)
            Parameters.Remove(parameter);
    }

    [RelayCommand]
    private void AddMessage() => Messages.Add(new ProgramMessageInput { Timestamp = DateTime.Now });

    [RelayCommand]
    private void RemoveMessage(ProgramMessageInput? message)
    {
        if (message is not null)
            Messages.Remove(message);
    }
}

public sealed partial class ProgramParameterInput : ObservableObject
{
    [ObservableProperty] public partial string Name { get; set; } = string.Empty;
    [ObservableProperty] public partial string Value { get; set; } = string.Empty;
    [ObservableProperty] public partial string Unit { get; set; } = string.Empty;

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Name) &&
        string.IsNullOrWhiteSpace(Value) &&
        string.IsNullOrWhiteSpace(Unit);
}

public sealed partial class ProgramMessageInput : ObservableObject
{
    [ObservableProperty] public partial DateTime Timestamp { get; set; } = DateTime.Now;
    [ObservableProperty] public partial string Message { get; set; } = string.Empty;
}