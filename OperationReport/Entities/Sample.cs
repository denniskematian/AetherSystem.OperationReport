namespace AetherSystem.OperationReport.Entities;

public record Sample(DateTime Timestamp, IReadOnlyList<double> Values);