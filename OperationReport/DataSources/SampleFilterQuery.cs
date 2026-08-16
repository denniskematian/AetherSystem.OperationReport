namespace AetherSystem.OperationReport.DataSources;

public record SampleFilterQuery(DateTime? From, DateTime? To, int? BatchNumber) : FilterQuery(From, To)
{
    public override string ToString()
    {
        var label = From.HasValue && To.HasValue 
            ? $"Range {From:G} - " + (To.Value.Date == From.Value.Date ? $"{To:T}" : $"{To:G}") 
            : From.HasValue 
                ? $"From {From:G}" 
                : To.HasValue 
                    ? $"To {To:G}" 
                    : string.Empty;
        
        if (BatchNumber.HasValue)
            label = string.IsNullOrEmpty(label) 
                ? $"Batch {BatchNumber}"
                : $"{label}, Batch {BatchNumber}";

        return label;
    }
}