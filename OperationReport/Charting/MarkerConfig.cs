using ScottPlot;

namespace AetherSystem.OperationReport.Charting;

public sealed class MarkerConfig
{
    public required string Column { get; set; }
    public required bool IsVisible { get; set; }
    public required MarkerShape Shape { get; set; }
    public required ColorInfo Color { get; set; }
}