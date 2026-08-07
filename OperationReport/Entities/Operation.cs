namespace AetherSystem.OperationReport.Entities;

public class Operation
{
    public DateTime Timestamp { get; }
    public string Comment { get; }
    
    public Operation(DateTime timestamp, string comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
        Timestamp = timestamp;
        Comment = comment;
    }
}