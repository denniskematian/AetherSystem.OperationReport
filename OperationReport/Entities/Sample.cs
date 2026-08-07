using AetherSystem.OperationReport.Internals;

namespace AetherSystem.OperationReport.Entities;

public class Sample
{
    public DateTime Timestamp { get; }
    public IReadOnlyList<double> Values { get; }
    
    public Sample(DateTime timestamp, IReadOnlyList<double> values)
    {
        ExceptionUtils.ThrowIfEmpty(values);
        Timestamp = timestamp;
        Values = values;
    }
}