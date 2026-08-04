using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources;

public interface ISampleTableAdapter
{
    IReadOnlyList<Column> SampleColumns { get; }
    DateTimeColumn TimestampColumn { get; }
    Column? BatchNumberColumn { get; }

    Task<int> CountAsync(SampleFilterQuery filterQuery, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Sample> EnumerateAsync(SampleFilterQuery filterQuery, CancellationToken cancellationToken = default);
}