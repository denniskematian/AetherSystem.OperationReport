using System.Globalization;
using AetherSystem.OperationReport.Collections;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using ScottPlot.Plottables.Interactive;
using ScottPlot.TickGenerators;

namespace AetherSystem.OperationReport.Charting;

public sealed class ChartController
{
    private readonly Dictionary<string, SignalXY> _seriesPlottableLookup = [];
    private readonly List<SeriesConfig> _seriesConfigs = [];
    private readonly Plot _plot;
    private readonly DataCollector _dataCollector;
    private bool _showDateInBottomTicks;
    
    private SignalXY? _operationPlottable;
    private InteractiveRectangle? _interactiveRectangle;
    private string _currentOperationColumn = string.Empty;

    /// <summary>
    /// Gets the current bounds of the interactive rectangle, including changes
    /// made by dragging or resizing it.
    /// </summary>
    public CoordinateRect? InteractiveRectangleBounds => _interactiveRectangle?.Rect;

    public bool HasInteractiveRectangle => _interactiveRectangle is not null;

    /// <summary>Gets the rectangle currently visible on the primary plot axes.</summary>
    public CoordinateRect? CurrentViewBounds
    {
        get
        {
            var limits = _plot.Axes.GetLimits();
            return limits.HasArea ? limits.Rect : null;
        }
    }

    public ChartController(
        Plot plot,
        DataCollector dataCollector,
        bool showDateInBottomTicks = true)
    {
        plot.Clear();
        plot.Legend.Orientation = Orientation.Horizontal;
        plot.Legend.Alignment = Alignment.UpperCenter;
        plot.Axes.DateTimeTicksBottom();

        _plot = plot;
        _dataCollector = dataCollector;
        _showDateInBottomTicks = showDateInBottomTicks;
        ConfigureBottomTickLabels(showDateInBottomTicks);
    }

    public void UpdateConfiguration(ChartConfig config)
    {
        ConfigureYAxis(_plot.Axes.Left, config.LeftAxis);
        ConfigureYAxis(_plot.Axes.Right, config.RightAxis);
        
        ConfigureAxisRange(_plot.Axes.Left, config.LeftAxisRange);
        ConfigureAxisRange(_plot.Axes.Right, config.RightAxisRange);
        ConfigureAxisRange(_plot.Axes.Bottom, config.BottomAxisRange);
        
        var comparer = SeriesConfigColumnComparer.Instance;
        var added = config.Series.Except(_seriesConfigs, comparer);
        var removed = _seriesConfigs.Except(config.Series, comparer);
        
        foreach (var seriesConfig in added)
        {
            var dataSource = _dataCollector.GetSampleDataSource(seriesConfig.Column);
            var sampleSignal = new SignalXYSourceGenericList<double, double>(
                _dataCollector.SampleTimestamps, dataSource);

            var plottable = _plot.Add.SignalXY(sampleSignal);
            _seriesPlottableLookup.Add(seriesConfig.Column, plottable);
            _seriesConfigs.Add(seriesConfig);
        }

        foreach (var seriesConfig in config.Series)
        {
            var plottable = _seriesPlottableLookup[seriesConfig.Column];
            plottable.LegendText = seriesConfig.Label;
            plottable.Axes.YAxis = GetAxis(seriesConfig.AxisPosition);
            plottable.LinePattern = seriesConfig.LinePattern;
            plottable.IsVisible = seriesConfig.IsVisible;
            plottable.Color = seriesConfig.Color.Value;
        }

        foreach (var seriesConfig in removed)
        {
            var plottable = _seriesPlottableLookup[seriesConfig.Column];
            plottable.IsVisible = false;
        }

        if (config.OperationMarker.IsVisible)
        {
            if (_operationPlottable is null || _currentOperationColumn != config.OperationMarker.Column)
            {
                var column = config.OperationMarker.Column;
                var sampleSignal = new SignalXYSourceGenericList<double, double>(
                    _dataCollector.OperationTimestamps, _dataCollector.GetOperationDataSource(column));

                if (_operationPlottable is not null)
                    _plot.PlottableList.Remove(_operationPlottable);
                
                _operationPlottable = _plot.Add.SignalXY(sampleSignal);
                _currentOperationColumn = column;
            }

            _operationPlottable.LineWidth = 0;
            _operationPlottable.MarkerSize = 10;
            _operationPlottable.MarkerColor = config.OperationMarker.Color.Value;
            _operationPlottable.MarkerShape = config.OperationMarker.Shape;
        }
        else
        {
            _operationPlottable?.IsVisible = false;
        }
        
        ConfigureBottomTickLabels(config.ShowDateInBottomTicks);
        
        Refresh(false);
    }
    
    public void Refresh(bool autoScale = true)
    {
        if (_plot is { PlotControl: { } plotControl })
        {
            if(autoScale)
                _plot.Axes.AutoScale();
            plotControl.Refresh();
        }
    }

    /// <summary>
    /// Adds a red interactive rectangle inside the current view, or resets the
    /// existing rectangle to that position without replacing it.
    /// </summary>
    /// <param name="margin">
    /// Maximum inset, in axis units, applied to every side. For views smaller
    /// than twenty margin units, ten percent of that axis span is used instead.
    /// </param>
    public InteractiveRectangle AddInteractiveRectangle(double margin = 10)
    {
        if (!double.IsFinite(margin) || margin < 0)
            throw new ArgumentOutOfRangeException(nameof(margin), "Margin must be a finite non-negative number.");

        var bounds = GetInteractiveRectangleBounds(margin);
        if (_interactiveRectangle is null)
        {
            _interactiveRectangle = _plot.Add.InteractiveRectangle(bounds);
            _interactiveRectangle.LineStyle.Color = Colors.Red;
            _interactiveRectangle.LineStyle.Width = 1;
            _interactiveRectangle.FillStyle.Color = Colors.Transparent;
            _interactiveRectangle.LineStyle.Pattern = LinePattern.Dotted;
        }
        else
        {
            _interactiveRectangle.Rect = bounds;
        }

        Refresh(false);
        return _interactiveRectangle;
    }

    /// <summary>Removes the interactive rectangle if it is present.</summary>
    /// <returns><see langword="true"/> when a rectangle was removed.</returns>
    public bool RemoveInteractiveRectangle()
    {
        if (_interactiveRectangle is null)
            return false;

        _plot.PlottableList.Remove(_interactiveRectangle);
        _interactiveRectangle = null;
        Refresh(false);
        return true;
    }

    private CoordinateRect GetInteractiveRectangleBounds(double margin)
    {
        var limits = _plot.Axes.GetLimits();
        if (!limits.HasArea)
        {
            _plot.Axes.AutoScale();
            limits = _plot.Axes.GetLimits();
        }

        if (!limits.HasArea)
            throw new InvalidOperationException("The plot must have a visible area before adding an interactive rectangle.");

        var left = Math.Min(limits.Left, limits.Right);
        var right = Math.Max(limits.Left, limits.Right);
        var bottom = Math.Min(limits.Bottom, limits.Top);
        var top = Math.Max(limits.Bottom, limits.Top);
        var horizontalMargin = Math.Min(margin, (right - left) * .1);
        var verticalMargin = Math.Min(margin, (top - bottom) * .1);

        return new CoordinateRect(
            left + horizontalMargin,
            right - horizontalMargin,
            bottom + verticalMargin,
            top - verticalMargin);
    }

    private void ConfigureBottomTickLabels(bool showDateInBottomTicks)
    {
        if (showDateInBottomTicks == _showDateInBottomTicks)
            return;
        
        if (_plot.Axes.Bottom.TickGenerator is not DateTimeAutomatic dateTimeTicks)
        {
            dateTimeTicks = new DateTimeAutomatic();
            _plot.Axes.Bottom.TickGenerator = dateTimeTicks;
        }

        var format = showDateInBottomTicks
            ? "yyyy-MM-dd HH:mm:ss"
            : "HH:mm:ss";
        dateTimeTicks.LabelFormatter = timestamp =>
            timestamp.ToString(format, CultureInfo.CurrentCulture);
        
        _showDateInBottomTicks = showDateInBottomTicks;
    }
    
    private IYAxis GetAxis(AxisPosition axisPosition)
    {
        return axisPosition switch
        {
            AxisPosition.Right => _plot.Axes.Right,
            AxisPosition.Left => _plot.Axes.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(axisPosition), axisPosition, null)
        };
    }

    private static void ConfigureYAxis(IYAxis yAxis, AxisConfig config)
    {
        yAxis.IsVisible = config.IsVisible;
        if (!yAxis.IsVisible)
            return;

        yAxis.Label.Text = config.Label;
        if (yAxis.TickGenerator is not NumericAutomatic)
            yAxis.TickGenerator = new NumericAutomatic();
        yAxis.FrameLineStyle.IsVisible = false;
    }

    private static void ConfigureAxisRange(IAxis axis, AxisRange? range)
    {
        if (range is null)
            return;

        axis.Min = range.Min;
        axis.Max = range.Max;
    }

    private class SeriesConfigColumnComparer : IEqualityComparer<SeriesConfig>
    {
        public static readonly SeriesConfigColumnComparer Instance = new();

        public bool Equals(SeriesConfig? x, SeriesConfig? y)
        {
            if (ReferenceEquals(x, y)) return true;
            return x?.Column == y?.Column;
        }

        public int GetHashCode(SeriesConfig obj)
        {
            return obj.Column.GetHashCode();
        }
    }
}