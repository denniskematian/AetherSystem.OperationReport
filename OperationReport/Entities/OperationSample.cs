using AetherSystem.OperationReport.Internals;

namespace AetherSystem.OperationReport.Entities;

public class OperationSample : Operation
{
    public IReadOnlyList<double> Values { get; }
    
    public OperationSample(DateTime timestamp, string comment, IReadOnlyList<double> values) : base(timestamp, comment)
    {
        ExceptionUtils.ThrowIfEmpty(values);
        Values = values;
    }
}