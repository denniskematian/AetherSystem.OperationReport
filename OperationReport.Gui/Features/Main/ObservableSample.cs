using System.Collections.ObjectModel;
using AetherSystem.OperationReport.Entities;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AetherSystem.OperationReport.Gui.Features.Main;

public sealed partial class ObservableSample : ObservableObject
{
    [ObservableProperty]
    public partial DateTime Timestamp { get; set; }
    public ObservableCollection<double> Values { get; }
    
    public ObservableSample(int valuesCount)
    {
        Values = [
            ..Enumerable.Repeat(0, valuesCount)
        ];
    }
    
    public ObservableSample(DateTime timestamp, IEnumerable<double> values)
    {
        Timestamp = timestamp;
        Values = [..values];
    }

    public void Update(Sample sample)
    {
        Timestamp = sample.Timestamp;
        for(var i = 0; i < sample.Values.Count; i++)
            Values[i] = sample.Values[i];
    }
}