using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Gui.Options;
using ScottPlot;

namespace AetherSystem.OperationReport.Gui.Features.ChartSettingDialog;

public static class Options
{
    public static IReadOnlyList<IOption<MarkerShape>> MarkerShapeOptions { get; } = [
        new EnumOption<MarkerShape>(MarkerShape.Eks),
        new EnumOption<MarkerShape>(MarkerShape.Cross),
        new Option<MarkerShape>("Diamond", MarkerShape.FilledDiamond),
        new Option<MarkerShape>("Square", MarkerShape.FilledSquare),
        new Option<MarkerShape>("Circle", MarkerShape.FilledCircle),
        new Option<MarkerShape>("Triangle", MarkerShape.FilledTriangleUp),
    ];

    public static IReadOnlyList<AxisPosition> AxisPositionOptions { get; } = [
        AxisPosition.Left,
        AxisPosition.Right,
    ];

    public static IReadOnlyList<LinePattern> LinePatternOptions { get; } = [
        LinePattern.Solid,
        LinePattern.Dashed,
        LinePattern.Dotted,
    ];

    public static IReadOnlyList<ColorInfo> ColorOptions => ColorInfo.Colors;
}