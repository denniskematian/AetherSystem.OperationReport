using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public interface IDataSourceAdapter
{
    IAsyncEnumerable<Table> GetTablesAsync(CancellationToken cancellationToken = default);
}