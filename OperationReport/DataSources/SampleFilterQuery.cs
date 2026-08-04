namespace AetherSystem.OperationReport.DataSources;

public record SampleFilterQuery(DateTimeOffset? From, DateTimeOffset? To, int? BatchNumber) 
    : FilterQuery(From, To);