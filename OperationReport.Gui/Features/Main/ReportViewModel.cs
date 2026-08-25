using System.Collections.ObjectModel;
using System.IO;
using AetherSystem.OperationReport.Collections;
using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Reporting;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;
using MemoryPack;

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
    [ObservableProperty] public partial string OperatorName { get; set; } = string.Empty;
    [ObservableProperty] public partial string OperatorSignaturePath { get; private set; } = string.Empty;
    [ObservableProperty] public partial string OfficerName { get; set; } = string.Empty;
    [ObservableProperty] public partial string OfficerSignaturePath { get; private set; } = string.Empty;
    [ObservableProperty] public partial string ProgramType { get; set; } = string.Empty;
    [ObservableProperty] public partial string StartedBy { get; set; } = string.Empty;
    [ObservableProperty] public partial bool IsReleased { get; set; }
    public ObservableCollection<ProgramParameterInput> Parameters { get; } = [];
    public ObservableCollection<ProgramMessageInput> Messages { get; } = [];
    
    public ReportViewModel(DashboardContext context) : base(context)
    {
        Title = "Batch Document";
        Icon = new PackIconMaterial { Kind = PackIconMaterialKind.FilePdfBox };
        
        context.Registry.PreSaveEvent += RegistryOnPreSaveEvent;
        var snapshot = context.Registry.Get<Snapshot>(nameof(ReportViewModel));
        if(snapshot is null)
            return;
        
        ProgramNumber = snapshot.ProgramNumber;
        BatchNumber = snapshot.BatchNumber;
        ReportTitle = snapshot.ReportTitle;
        SerialNumber = snapshot.SerialNumber;
        CompanyName = snapshot.CompanyName;
        CompanyLogoPath = snapshot.CompanyLogoPath;
        OperatorName = snapshot.OperatorName;
        OperatorSignaturePath = snapshot.OperatorSignaturePath;
        OfficerName = snapshot.OfficerName;
        OfficerSignaturePath = snapshot.OfficerSignaturePath;
        ProgramType = snapshot.ProgramType;
        StartedBy = snapshot.StartedBy;
        IsReleased = snapshot.IsReleased;
        
        foreach (var programParameterInput in snapshot.Parameters)
        {
            Parameters.Add(programParameterInput);
        }
        
        foreach (var programMessageInput in snapshot.Messages)
        {
            Messages.Add(programMessageInput);
        }
    }

    private void RegistryOnPreSaveEvent(object? sender, EventArgs e)
    {
        var snapshot = new Snapshot(
            ProgramNumber,
            BatchNumber,
            ReportTitle,
            SerialNumber,
            CompanyName,
            CompanyLogoPath,
            OperatorName,
            OperatorSignaturePath,
            OfficerName,
            OfficerSignaturePath,
            ProgramType,
            StartedBy,
            IsReleased,
            Parameters,
            Messages
        );

        Context.Registry.Put(nameof(ReportViewModel), snapshot);
    }

    protected override void DataCollectorUpdated()
    {
    }

    [RelayCommand]
    private async Task GeneratePdf()
    {
        if(Context.FilterQuery is null || Context.ChartController is null)
            return;
        
        var destination = Facades.DialogService.SaveFileDialog(PdfFilter);
        if (destination.IsFailed)
            return;

        try
        {
            var now = DateTime.Now;
            var programSection = BuildProgramSection(Context.FilterQuery);
            var batchDocument = new BatchDocument(
                ProgramNumber,
                BatchNumber,
                ReportTitle,
                SerialNumber,
                CompanyName,
                CompanyLogoPath,
                programSection,
                now,
                operatorSignature: new Signature(OperatorName, OperatorSignaturePath, now),
                officerSignature: new Signature(OfficerName, OfficerSignaturePath, now));

            var controller = new DocumentController(batchDocument, Context.ChartController);
            await using var output = new FileStream(
                destination.Value.FullName,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);
            await controller.WritePdfAsync(output);
            await output.FlushAsync();
            
            Facades.DialogService.ConfirmDialog("PDF generated successfully.");
        }
        catch (Exception ex)
        {
            Facades.DialogService.ErrorDialog("Unable to generate PDF.\r\n" + ex.Message);
        }
    }

    [RelayCommand]
    private void SelectCompanyLogo()
    {
        var result = Facades.DialogService.OpenFileDialog(ImageFilter);
        if (result.IsSuccess)
            CompanyLogoPath = result.Value.FullName;
    }

    [RelayCommand]
    private void SelectOperatorSignature()
    {
        var result = Facades.DialogService.OpenFileDialog(ImageFilter);
        if (result.IsSuccess)
            OperatorSignaturePath = result.Value.FullName;
    }

    [RelayCommand]
    private void SelectOfficerSignature()
    {
        var result = Facades.DialogService.OpenFileDialog(ImageFilter);
        if (result.IsSuccess)
            OfficerSignaturePath = result.Value.FullName;
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

    private ProgramSection BuildProgramSection(SampleFilterQuery filter)
    {
        var visibleSeries = Context.ChartConfig.Series.Index()
            .Where(series => series.Item.IsVisible)
            .ToArray();
        var labels = visibleSeries
            .Select((series, index) => new OperationLogLabel(index, series.Item.Label))
            .ToArray();
        var valueSources = visibleSeries
            .Select(series => series.Index)
            .ToArray();
        var logs = Context.DataCollector.GetOperationPage(PageRequest.All, valueSources).Items;

        return new ProgramSection(
            filter.From!.Value,
            filter.To!.Value,
            ProgramType,
            new ProgramSteps(logs, labels),
            Parameters
                .Where(parameter => !parameter.IsEmpty)
                .Select(parameter => new ProgramParameter(parameter.Name, parameter.Value))
                .ToArray(),
            IsReleased,
            StartedBy,
            Messages.Select(message => new ProgramMessage(message.Timestamp, message.Message)).ToArray());
    }

    [MemoryPackable]
    public partial record Snapshot(
        string ProgramNumber,
        string BatchNumber,
        string ReportTitle,
        string SerialNumber,
        string CompanyName,
        string CompanyLogoPath,
        string OperatorName,
        string OperatorSignaturePath,
        string OfficerName,
        string OfficerSignaturePath,
        string ProgramType,
        string StartedBy,
        bool IsReleased,
        IReadOnlyList<ProgramParameterInput> Parameters,
        IReadOnlyList<ProgramMessageInput> Messages) : IPackableRecord;
}

[MemoryPackable]
public sealed partial class ProgramParameterInput : ObservableObject
{
    [ObservableProperty] public partial string Name { get; set; } = string.Empty;
    [ObservableProperty] public partial string Value { get; set; } = string.Empty;

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Name) &&
        string.IsNullOrWhiteSpace(Value);
}

[MemoryPackable]
public sealed partial class ProgramMessageInput : ObservableObject
{
    [ObservableProperty] public partial DateTime Timestamp { get; set; } = DateTime.Now;
    [ObservableProperty] public partial string Message { get; set; } = string.Empty;
}