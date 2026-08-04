using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources;

public interface IReferenceTableAdapter
{
    IAsyncEnumerable<SampleReference> EnumerateAsync(CancellationToken cancellationToken = default);
}