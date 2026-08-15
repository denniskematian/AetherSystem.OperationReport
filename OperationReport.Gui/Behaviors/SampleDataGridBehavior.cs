using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AetherSystem.OperationReport.Entities;
using AetherSystem.OperationReport.ValueObjects;

namespace AetherSystem.OperationReport.Gui.Behaviors;

public static class SampleDataGridBehavior
{
    public static readonly DependencyProperty SampleSourceProperty =
        DependencyProperty.RegisterAttached(
            "SampleSource",
            typeof(IEnumerable<SampleReferenceConfig>),
            typeof(SampleDataGridBehavior),
            new PropertyMetadata(null, OnSampleSourceChanged));

    public static void SetSampleSource(DependencyObject element, IEnumerable<SampleReferenceConfig> value)
    {
        element.SetValue(SampleSourceProperty, value);
    }

    public static IEnumerable<SampleReferenceConfig> GetSampleSource(DependencyObject element)
    {
        return (IEnumerable<SampleReferenceConfig>)element.GetValue(SampleSourceProperty);
    }

    private static void OnSampleSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid dataGrid)
            return;

        BuildColumns(dataGrid, e.NewValue as IEnumerable<SampleReferenceConfig>);

        if (e.NewValue is INotifyCollectionChanged notifyCollection)
            notifyCollection.CollectionChanged += (_, _) => { BuildColumns(dataGrid, GetSampleSource(dataGrid)); };
    }

    private static void BuildColumns(DataGrid dataGrid, IEnumerable<SampleReferenceConfig>? references)
    {
        dataGrid.Columns.Clear();

        if (references is null)
            return;

        var timestampColumn = new DataGridTextColumn
        {
            Header = nameof(Sample.Timestamp),
            Binding = new Binding(nameof(Sample.Timestamp))
            {
                Mode = BindingMode.OneWay,
                StringFormat = "yyyy-MM-dd HH:mm:ss.fff"
            },
            ElementStyle = new Style(typeof(TextBlock))
            {
                Setters =
                {
                    new Setter(TextBlock.PaddingProperty, new Thickness(2, 1, 2, 1))
                }
            }
        };
        
        dataGrid.Columns.Add(timestampColumn);
        var sampleStyle = new Style(typeof(TextBlock))
        {
            Setters =
            {
                new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right),
                new Setter(TextBlock.PaddingProperty, new Thickness(2, 1, 2, 1))
            }
        };
        
        var index = 0;
        foreach (var reference in references.Where(i => i.IsIncluded))
        {
            var column = new DataGridTextColumn
            {
                Header = reference.Label,
                Binding = new Binding($"{nameof(Sample.Values)}[{index}]")
                {
                    Mode = BindingMode.OneWay,
                    StringFormat = "0.###"
                },
                ElementStyle = sampleStyle
            };
            dataGrid.Columns.Add(column);
            index++;
        }
    }
}