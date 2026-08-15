using ScottPlot;

namespace AetherSystem.OperationReport.Charting;

public sealed class SeriesConfig
{
    public required string Column { get; init; }
    public required bool IsVisible { get; set; }
    public required AxisPosition AxisPosition { get; set; }
    public required string Label { get; set; }
    public required ColorInfo Color { get; set; }
    public required LinePattern LinePattern { get; set; }
}