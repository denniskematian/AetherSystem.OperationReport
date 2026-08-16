using ScottPlot;

namespace AetherSystem.OperationReport.Charting;

public sealed record ColorInfo(string Name, Color Value)
{
    public static IReadOnlyList<ColorInfo> Colors => field ??= GenerateColor();

    private static IReadOnlyList<ColorInfo> GenerateColor()
    {
        return [ .. 
            from color in GenerateRainbow(36)
                .Concat(GenerateRainbow(12, .4f))
                .Concat(GenerateGrayScale(8))
            orderby color.Luminance, color.Hue
            select new ColorInfo(color.ToHex(), color)
        ];
    }
    
    private static IEnumerable<Color> GenerateRainbow(int count, float lightness = .5f)
    {
        return ScottPlot.Colors.Rainbow(count)
            .Select(color => color.WithLightness(lightness));
    }
    
    private static IEnumerable<Color> GenerateGrayScale(int count)
    {
        var step = (256f - 63f) / count;
        for (int i = 0; i < count; i++)
        {
            var value = (byte)float.Round(step * i);
            yield return new Color(value, value, value);
        }
    }
}