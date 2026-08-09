using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using AetherSystem.OperationReport.ValueObjects;

namespace AetherSystem.OperationReport.Gui.Converters;

public sealed class ColorToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is ColorRgb color
            ? new SolidColorBrush(Color.FromArgb(255, color.R, color.G, color.B))
            : Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}