namespace AetherSystem.OperationReport.Entities;

public record OperationSample(DateTime Timestamp, string Comment, IReadOnlyList<double> Values)
    : Operation(Timestamp, Comment);