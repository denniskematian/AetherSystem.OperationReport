namespace AetherSystem.OperationReport.ValueObjects;

public class SampleReferenceConfig
{
    public required string Column { get; init; }
    public required bool IsIncluded { get; set; }
    public required int Index { get; set; }
    public required string Label { get; set; }
}