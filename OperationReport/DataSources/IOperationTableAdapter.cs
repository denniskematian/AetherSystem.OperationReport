using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources;

public interface IOperationTableAdapter : IDisposable, IAsyncDisposable
{
    Task<int> CountAsync(FilterQuery filterQuery, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Operation> EnumerateAsync(FilterQuery filterQuery, CancellationToken cancellationToken = default);
}