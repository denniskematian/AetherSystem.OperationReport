using AetherSystem.OperationReport.ValueObjects;

namespace AetherSystem.OperationReport.Internals;
using ScottPlotColor = ScottPlot.Color;
using QuestPDFColor = QuestPDF.Infrastructure.Color;

internal static class ColorUtils
{
    extension(ColorRgb color)
    {
        public ScottPlotColor ToScottPlotColor()
        {
            return new ScottPlotColor(color.R, color.G, color.B);
        }

        public QuestPDFColor ToQuestPdfColor()
        {
            return QuestPDFColor.FromRGB(color.R, color.G, color.B);
        }
    }

    extension(ColorHsl color)
    {
        public ScottPlotColor ToScottPlotColor()
        {
            return ScottPlotColor.FromHSL(color.H, color.S, color.L);
        }

        public QuestPDFColor ToQuestPdfColor()
        {
            return color.ToColorRgb().ToQuestPdfColor();
        }
    }
}