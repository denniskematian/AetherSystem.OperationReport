namespace AetherSystem.OperationReport.ValueObjects;


public readonly record struct ColorHsl(float H, float S, float L)
{
    public float Hue => H;
    public float Saturation => S;
    public float Lightness => L;

    public ColorRgb ToColorRgb()
    {
        var hue = float.IsFinite(H) ? (H % 360 + 360) % 360 : 0;
        var saturation = float.IsFinite(S) ? Math.Clamp(S, 0, 1) : 0;
        var lightness = float.IsFinite(L) ? Math.Clamp(L, 0, 1) : 0;
        var chroma = (1f - Math.Abs(2f * lightness - 1f)) * saturation;
        var intermediate = chroma * (1f - Math.Abs((hue / 60f) % 2f - 1f));
        var offset = lightness - chroma / 2f;

        var (red, green, blue) = hue switch
        {
            < 60f => (chroma, intermediate, 0),
            < 120f => (intermediate, chroma, 0),
            < 180f => (0, chroma, intermediate),
            < 240f => (0, intermediate, chroma),
            < 300f => (intermediate, 0, chroma),
            _ => (chroma, 0f, intermediate)
        };

        return new ColorRgb(ToByte(red + offset), ToByte(green + offset), ToByte(blue + offset));
    }

    public static explicit operator ColorRgb(ColorHsl color) => color.ToColorRgb();

    private static byte ToByte(double value) =>
        (byte)Math.Round(Math.Clamp(value, 0d, 1d) * 255d, MidpointRounding.AwayFromZero);
}