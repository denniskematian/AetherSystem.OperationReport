namespace AetherSystem.OperationReport.ValueObjects;

public readonly record struct ColorRgb(byte R, byte G, byte B)
{
    public byte Red => R;
    public byte Green => G;
    public byte Blue => B;

    public ColorHsl ToColorHsl()
    {
        const float tolerance = 1f / 1023;
        var red = R / 255f;
        var green = G / 255f;
        var blue = B / 255f;
        var maximum = float.Max(red, float.Max(green, blue));
        var minimum = float.Min(red, float.Min(green, blue));
        var chroma = maximum - minimum;
        var lightness = (maximum + minimum) / 2f;

        if (chroma == 0f)
            return new ColorHsl(0f, 0f, lightness);

        var saturation = chroma / (1f - float.Abs(2f * lightness - 1f));
        var hue = maximum switch
        {
            _ when float.Abs(maximum - red) < tolerance => 60f * (((green - blue) / chroma) % 6f),
            _ when float.Abs(maximum - green) < tolerance => 60f * ((blue - red) / chroma + 2f),
            _ => 60f * ((red - green) / chroma + 4f)
        };

        if (hue < 0d)
            hue += 360f;

        return new ColorHsl(hue, saturation, lightness);
    }

    public string ToHexString() => $"#{R:X2}{G:X2}{B:X2}";

    public static explicit operator ColorHsl(ColorRgb color) => color.ToColorHsl();
}