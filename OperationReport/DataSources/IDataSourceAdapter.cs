using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public interface IDataSourceAdapter : IDisposable, IAsyncDisposable
{
    IAsyncEnumerable<Table> GetTablesAsync(CancellationToken cancellationToken = default);
}