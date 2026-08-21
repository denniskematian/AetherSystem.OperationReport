namespace AetherSystem.OperationReport.Reporting;

public sealed class ProgramParameter
{
    public ProgramParameter(string name, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(value);
        Name = name;
        Value = value;
    }

    public string Name { get; }
    public string Value { get; }
}