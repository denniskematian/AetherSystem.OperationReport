namespace AetherSystem.OperationReport.Reporting;

public sealed class ProgramMessage
{
    public ProgramMessage(DateTime timestamp, string message)
    {
        ArgumentException.ThrowIfNullOrEmpty(message);
        Timestamp = timestamp;
        Message = message;
    }

    public DateTime Timestamp { get; }
    public string Message { get; }
}