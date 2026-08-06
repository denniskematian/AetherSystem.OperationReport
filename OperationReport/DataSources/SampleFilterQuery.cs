namespace AetherSystem.OperationReport.DataSources;

public record SampleFilterQuery(DateTime? From, DateTime? To, int? BatchNumber) 
    : FilterQuery(From, To);