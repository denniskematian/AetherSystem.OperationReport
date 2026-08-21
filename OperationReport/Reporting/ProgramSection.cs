namespace AetherSystem.OperationReport.Reporting;

public sealed class ProgramSection
{
    public ProgramSection(
        DateTime startedAt,
        DateTime finishedAt,
        string programType,
        ProgramSteps programSteps,
        IReadOnlyList<ProgramParameter> parameters,
        bool isReleased,
        string? startedBy,
        IReadOnlyList<ProgramMessage> messages)
    {
        ArgumentException.ThrowIfNullOrEmpty(programType);
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(messages);
        ArgumentNullException.ThrowIfNull(programSteps);

        var parameterItems = parameters.ToArray();
        var messageItems = messages.ToArray();

        if (finishedAt < startedAt)
            throw new ArgumentOutOfRangeException(
                nameof(finishedAt),
                finishedAt,
                "Finish time cannot be earlier than start time.");

        StartedAt = startedAt;
        FinishedAt = finishedAt;
        ProgramType = programType;
        ProgramSteps = programSteps;
        Parameters = parameterItems;
        IsReleased = isReleased;
        StartedBy = string.IsNullOrWhiteSpace(startedBy) ? null : startedBy;
        Messages = messageItems;
    }

    public DateTime StartedAt { get; }
    public DateTime FinishedAt { get; }
    public TimeSpan Duration => FinishedAt - StartedAt;
    public string ProgramType { get; }
    public ProgramSteps ProgramSteps { get; }
    public IReadOnlyList<ProgramParameter> Parameters { get; }
    public bool IsReleased { get; }
    public string? StartedBy { get; }
    public IReadOnlyList<ProgramMessage> Messages { get; }
}