using AetherSystem.OperationReport.ValueObjects;
using ScottPlot;

namespace AetherSystem.OperationReport.Charting;

public sealed class ChartConfig
{
    public required AxisConfig LeftAxis { get; init; }
    public required AxisConfig RightAxis { get; init; }
    public required IReadOnlyList<SeriesConfig> Series { get; init; }
    public required MarkerConfig OperationMarker { get; init; }
    public required bool ShowDateInBottomTicks { get; init; }

    public AxisRange? LeftAxisRange { get; set; }
    public AxisRange? RightAxisRange { get; set; }
    public AxisRange? BottomAxisRange { get; set; }

    public static ChartConfig CreateDefault(IReadOnlyList<SampleReferenceConfig> references)
    {
        var seriesConfigs = new List<SeriesConfig>();
        var index = 0;
        foreach (var reference in references.Where(i => i.IsIncluded))
        {
            var seriesConfig = new SeriesConfig
            {
                IsVisible = true,
                AxisPosition = AxisPosition.Left,
                LinePattern = LinePattern.Solid,
                Label = reference.Label,
                Color = ColorInfo.Colors[index],
                Column = reference.Column,
            };
            
            seriesConfigs.Add(seriesConfig);
            index++;
        }

        var markerConfig = new MarkerConfig
        {
            Column = references[0].Column,
            IsVisible = false,
            Color = ColorInfo.Colors[0],
            Shape = MarkerShape.Eks,
        };

        return new ChartConfig
        {
            LeftAxis = new AxisConfig
            {
                IsVisible = true,
                Label = "Temperature"
            },
            RightAxis = new AxisConfig
            {
                IsVisible = true,
                Label = "Pressure"
            },
            Series = seriesConfigs,
            OperationMarker = markerConfig,
            ShowDateInBottomTicks = true
        };
    }
}