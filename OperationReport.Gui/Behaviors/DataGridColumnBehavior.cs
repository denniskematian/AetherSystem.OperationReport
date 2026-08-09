using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.Gui.Behaviors;

public static class DataGridColumnsBehavior
{
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.RegisterAttached(
            "Columns",
            typeof(IEnumerable),
            typeof(DataGridColumnsBehavior),
            new PropertyMetadata(null, OnColumnsChanged));

    public static void SetColumns(DependencyObject element, IEnumerable value)
    {
        element.SetValue(ColumnsProperty, value);
    }

    public static IEnumerable GetColumns(DependencyObject element)
    {
        return (IEnumerable)element.GetValue(ColumnsProperty);
    }

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid dataGrid)
            return;

        BuildColumns(dataGrid, e.NewValue as IEnumerable);

        if (e.NewValue is INotifyCollectionChanged notifyCollection)
            notifyCollection.CollectionChanged += (_, _) => { BuildColumns(dataGrid, GetColumns(dataGrid)); };
    }

    private static void BuildColumns(DataGrid dataGrid, IEnumerable? columns)
    {
        dataGrid.Columns.Clear();

        if (columns is null)
            return;

        var index = 0;
        foreach (var item in columns)
        {
            if (item is not Column column)
                continue;

            var binding = new Binding($"[{index++}]");
            binding.Mode = BindingMode.OneWay;
            if (column is TimestampColumn)
                binding.StringFormat = "yyyy-MM-dd HH:mm:ss.fff";
            else if (column.Type is ColumnType.Real) binding.StringFormat = "0.###";

            var gridColumn = new DataGridTextColumn
            {
                Header = column.Name,
                Binding = binding
            };

            if (column is { Type: ColumnType.Integer or ColumnType.Real } and not TimestampColumn)
            {
                var style = new Style(typeof(TextBlock));

                var alignment = new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right);
                style.Setters.Add(alignment);

                gridColumn.ElementStyle = style;
            }

            dataGrid.Columns.Add(gridColumn);
        }
    }
}