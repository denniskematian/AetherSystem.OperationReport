using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources;

public interface IReferenceTableAdapter : IDisposable, IAsyncDisposable
{
    IAsyncEnumerable<SampleReference> EnumerateAsync(CancellationToken cancellationToken = default);
}