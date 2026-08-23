using System.Globalization;
using AetherSystem.OperationReport.Charting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AetherSystem.OperationReport.Reporting;

public sealed class DocumentController
{
    private const string BorderColor = "#606060";
    private const string MutedColor = "#555555";
    private const string AlternateRowColor = "#F2F2F2";

    private readonly BatchDocument _data;
    private readonly ChartController _chartController;

    public DocumentController(BatchDocument data, ChartController chartController)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(chartController);

        _data = data;
        _chartController = chartController;
    }

    public async Task WritePdfAsync(Stream output, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(output);

        if (!output.CanWrite)
            throw new ArgumentException("The output stream must be writable.", nameof(output));

        cancellationToken.ThrowIfCancellationRequested();

        await Task.Run(
            () => CreateDocument().GeneratePdf(output),
            cancellationToken).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();
    }

    private IDocument CreateDocument()
    {
        return Document.Create(document =>
        {
            document.Page(ComposeReport);
        }).WithMetadata(new DocumentMetadata
        {
            Title = _data.Title,
            Author = _data.CompanyName,
            Subject = $"Batch {_data.BatchNumber}",
            Creator = nameof(DocumentController),
            CreationDate = _data.GeneratedAt,
            ModifiedDate = _data.GeneratedAt,
        });
    }

    private void ComposeReport(PageDescriptor page)
    {
        page.Size(PageSizes.A4.Landscape());
        page.PageColor(Colors.White);
        page.MarginHorizontal(15, Unit.Millimetre);
        page.MarginVertical(8, Unit.Millimetre);
        page.DefaultTextStyle(style => style.FontFamily(Fonts.Lato).FontSize(8).FontColor(Colors.Grey.Darken3));

        page.Header().Element(ComposeHeader);
        page.Content().PaddingTop(5, Unit.Millimetre).Column(column =>
        {
            column.Item().Element(ComposeOverview);
            column.Item().PageBreak();
            column.Item().Element(ComposeProgramProcess);
            column.Item().PageBreak();
            column.Item().Element(ComposeMessages);
        });
        page.Footer().Element(ComposeFooter);
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text(_data.Title).FontSize(16).SemiBold();
                    left.Item().Text(text =>
                    {
                        text.Span("Program no.:  ").SemiBold();
                        text.Span(_data.ProgramNumber);
                    });
                    left.Item().Text(text =>
                    {
                        text.Span("Batch no.:  ").SemiBold();
                        text.Span(_data.BatchNumber);
                    });
                });

                row.ConstantItem(43, Unit.Millimetre)
                    .Height(18, Unit.Millimetre)
                    .AlignRight()
                    .Image(_data.CompanyLogoPath)
                    .FitArea();
            });

            column.Item().Row(row =>
            {
                row.RelativeItem();
                row.AutoItem().Text(text =>
                {
                    text.Span("Serial-No.:  ").SemiBold();
                    text.Span(_data.SerialNumber);
                });
            });

            column.Item().BorderBottom(0.6f).BorderColor(BorderColor);
        });
    }
    

    private void ComposeFooter(IContainer container)
    {
        container.PaddingTop(2, Unit.Millimetre)
            .BorderTop(0.6f)
            .BorderColor(BorderColor)
            .Row(row =>
            {
                row.RelativeItem().AlignLeft().Text(text =>
                {
                    text.Span("page ");
                    text.CurrentPageNumber();
                    text.Span(" from ");
                    text.TotalPages();
                });

                row.RelativeItem().AlignCenter().Text(_data.CompanyName);
                row.RelativeItem().AlignRight().Text(_data.GeneratedAt.ToString("dd.MM.yyyy HH:mm:ss"));
            });
    }

    private void ComposeOverview(IContainer container)
    {
        container.Column(column =>
        {
            column.Item()
                .MinHeight(142, Unit.Millimetre)
                .Image(_chartController.GetPrintableChartPng())
                .FitArea();

            column.Item().PaddingTop(3, Unit.Millimetre).Row(row =>
            {
                row.RelativeItem().Element(element => ComposeSignature(element, "Done/operated by:", _data.OperatorSignature));
                row.ConstantItem(8, Unit.Millimetre);
                row.RelativeItem().Element(element => ComposeSignature(element, "Checked by:", _data.OfficerSignature));
            });
        });
    }

    private void ComposeProgramProcess(IContainer container)
    {
        var section = _data.ProgramSection;
        var steps = section.ProgramSteps;

        container.Column(column =>
        {
            column.Spacing(5);
            column.Item().Text("Program process").FontSize(15).SemiBold();

            column.Item().Row(row =>
            {
                row.RelativeItem().Text($"Program start:  {section.StartedAt:dd.MM.yyyy HH:mm:ss}");
                row.RelativeItem().Text($"Program end:  {section.FinishedAt:dd.MM.yyyy HH:mm:ss}");
                row.AutoItem().Text($"Program duration:  {FormatDuration(section.Duration)} h");
            });

            column.Item().Element(element => ComposeOperationTable(element, steps));

            column.Item().PaddingTop(6, Unit.Millimetre).ShowEntire().Row(row =>
            {
                row.RelativeItem(2).Element(element => ComposeParameters(element, section));
                row.ConstantItem(10, Unit.Millimetre);
                row.RelativeItem(3).Element(element => ComposeStatistics(element, steps));
            });

            column.Item().PaddingTop(7, Unit.Millimetre).ShowEntire().Element(element => ComposeRelease(element, section));
        });
    }
    
    private void ComposeMessages(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(7);
            column.Item().Text("Messages").FontSize(15).SemiBold();

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(48, Unit.Millimetre);
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    HeaderCell(header.Cell(), "Date / Time");
                    HeaderCell(header.Cell(), "Message");
                });

                for (var index = 0; index < _data.ProgramSection.Messages.Count; index++)
                {
                    var message = _data.ProgramSection.Messages[index];
                    DataCell(table.Cell(), message.Timestamp.ToString("dd.MM.yyyy HH:mm:ss"), index, true);
                    DataCell(table.Cell(), message.Message, index);
                }
            });
        });
    }

    private static void ComposeSignature(
        IContainer container,
        string caption,
        Signature? signature)
    {
        container.Row(row =>
        {
            row.ConstantItem(39, Unit.Millimetre).AlignBottom().PaddingBottom(5).Text(caption).FontSize(9);
            row.RelativeItem().Column(column =>
            {
                column.Item().Height(10, Unit.Millimetre).AlignCenter().AlignMiddle().Element(imageContainer =>
                {
                    if (signature?.ImagePath is not null)
                        imageContainer.Image(signature.ImagePath).FitArea();
                });

                column.Item().BorderBottom(0.5f).BorderColor(BorderColor).AlignCenter().Text(signature?.Name ?? string.Empty);
                column.Item().AlignCenter().Text(signature is null
                    ? "Date / Signature"
                    : $"{signature.SignedAt:dd.MM.yyyy HH:mm:ss} / Signature").FontSize(6.5f);
            });
        });
    }

    private static string FormatDuration(TimeSpan duration)
    {
        return $"{(int)duration.TotalHours:00}:{duration.Minutes:00}:{duration.Seconds:00}";
    }

    private static void ComposeParameters(IContainer container, ProgramSection section)
    {
        container.Column(column =>
        {
            column.Spacing(3);
            column.Item().Text("Program parameters").FontSize(14).SemiBold();

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn(1.5f);
                });

                ParameterRow(table, "Program type", section.ProgramType);

                foreach (var parameter in section.Parameters)
                    ParameterRow(table, parameter.Name, parameter.Value);
            });
        });
    }

    private static void ParameterRow(TableDescriptor table, string name, string value)
    {
        table.Cell().Padding(2).Text($"{name}:");
        table.Cell()
            .Border(0.4f)
            .BorderColor(BorderColor)
            .Padding(2)
            .AlignCenter()
            .Text(value);
    }

    private static void ComposeStatistics(IContainer container, ProgramSteps steps)
    {
        container.Column(column =>
        {
            column.Spacing(3);
            column.Item().Text("MinMaxAvg values").FontSize(14).SemiBold();

            if (steps.OperationLogs.Count == 0)
            {
                column.Item().Text("No operation log data.").Italic().FontColor(MutedColor);
                return;
            }

            if (steps.OperationLogLabels.Count == 0)
            {
                column.Item().Text("No operation values selected.").Italic().FontColor(MutedColor);
                return;
            }

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(14, Unit.Millimetre);
                    foreach (var _ in steps.OperationLogLabels)
                        columns.RelativeColumn();
                });

                HeaderCell(table.Cell(), string.Empty);
                foreach (var label in steps.OperationLogLabels)
                    HeaderCell(table.Cell(), label.Label);

                StatisticsRow(table, "MIN", steps.MinValues);
                StatisticsRow(table, "MAX", steps.MaxValues);
                StatisticsRow(table, "AVG", steps.AvgValues);
            });
        });
    }

    private static void HeaderCell(IContainer container, string value)
    {
        container.BorderBottom(0.7f)
            .BorderColor(BorderColor)
            .Padding(2)
            .AlignCenter()
            .AlignMiddle()
            .Text(value)
            .SemiBold()
            .FontSize(7);
    }

    private static void StatisticsRow(TableDescriptor table, string name, IReadOnlyList<double> values)
    {
        HeaderCell(table.Cell(), name);
        foreach (var value in values)
            DataCell(table.Cell(), FormatNumber(value), 0, true);
    }

    private static void DataCell(IContainer container, string value, int rowIndex, bool center = false)
    {
        var cell = container
            .Background(rowIndex % 2 == 0 ? Colors.White : AlternateRowColor)
            .BorderBottom(0.35f)
            .BorderColor(BorderColor)
            .PaddingHorizontal(2)
            .PaddingVertical(2);

        if (center)
            cell = cell.AlignCenter();

        cell.AlignMiddle().Text(value).FontSize(7);
    }

    private static string FormatNumber(double value)
    {
        return value.ToString("0.0##", CultureInfo.CurrentCulture);
    }
    
    private void ComposeRelease(IContainer container, ProgramSection section)
    {
        container.Border(0.6f).BorderColor(BorderColor).Padding(6, Unit.Millimetre).Column(column =>
        {
            column.Spacing(4);
            column.Item().Row(row =>
            {
                row.ConstantItem(65, Unit.Millimetre).Text("Process released:");
                row.ConstantItem(7, Unit.Millimetre)
                    .Height(7, Unit.Millimetre)
                    .Border(0.6f)
                    .BorderColor(BorderColor)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text(section.IsReleased ? "X" : string.Empty)
                    .SemiBold();
            });

            column.Item().Row(row =>
            {
                row.ConstantItem(65, Unit.Millimetre).Text("Start program by:");
                row.RelativeItem().BorderBottom(0.5f).BorderColor(BorderColor).AlignCenter().Text(section.StartedBy ?? string.Empty);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Element(element => ComposeSignature(element, "Done/operated by:", _data.OperatorSignature));
                row.ConstantItem(8, Unit.Millimetre);
                row.RelativeItem().Element(element => ComposeSignature(element, "Checked by:", _data.OfficerSignature));
            });
        });
    }

    private static void ComposeOperationTable(IContainer container, ProgramSteps steps)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(13, Unit.Millimetre);
                columns.ConstantColumn(22, Unit.Millimetre);
                columns.RelativeColumn(3.4f);

                foreach (var _ in steps.OperationLogLabels)
                    columns.RelativeColumn();
            });

            table.Header(header =>
            {
                HeaderCell(header.Cell(), "Step:");
                HeaderCell(header.Cell(), "Time");
                HeaderCell(header.Cell(), "Description");

                foreach (var label in steps.OperationLogLabels)
                    HeaderCell(header.Cell(), label.Label);
            });

            if (steps.OperationLogs.Count == 0)
            {
                DataCell(
                    table.Cell().ColumnSpan((uint)(3 + steps.OperationLogLabels.Count)),
                    "No operation log data",
                    0,
                    true);
                return;
            }

            for (var rowIndex = 0; rowIndex < steps.OperationLogs.Count; rowIndex++)
            {
                var log = steps.OperationLogs[rowIndex];
                DataCell(table.Cell(), (rowIndex + 1).ToString(CultureInfo.CurrentCulture), rowIndex, true);
                DataCell(table.Cell(), log.Timestamp.ToString("HH:mm:ss"), rowIndex, true);
                DataCell(table.Cell(), log.Comment, rowIndex);

                foreach (var label in steps.OperationLogLabels)
                    DataCell(table.Cell(), FormatNumber(log.Values[label.Id]), rowIndex, true);
            }
        });
    }
}