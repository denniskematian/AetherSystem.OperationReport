namespace AetherSystem.OperationReport.Entities;

public class SampleReference
{
    public int Id { get; }
    public string Label { get; }

    public SampleReference(int id, string label)
    {
        ArgumentNullException.ThrowIfNull(label);
        Id = id;
        Label = label;
    }
}