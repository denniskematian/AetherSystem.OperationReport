using ScottPlot;

namespace AetherSystem.OperationReport.Charting;

public sealed record ColorInfo(string Name, Color Value)
{
    public static IReadOnlyList<ColorInfo> Colors => field ??=
    [
        .. from color in ScottPlot.Colors
            .GetNamedColors()
            .DistinctBy(i => i.Item2)
        let value = color.Item2
        where value.A == 255
        orderby value.Hue, value.Luminance
        select new ColorInfo(value.ToHex(), value)
    ];
}