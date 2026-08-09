using AetherSystem.OperationReport.DataSources;

namespace AetherSystem.OperationReport.ValueObjects;

public sealed class PresetConfig
{
    public required SampleSourceInfo SampleDataSource { get; init; }
    public required OperationSourceInfo OperationDataSource { get; init; }
    public required IReadOnlyList<SampleReferenceConfig> SampleReferences { get; init; }
}