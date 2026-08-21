namespace AetherSystem.OperationReport.Reporting;

public sealed class Signature
{
    public Signature(string name, string imagePath, DateTime signedAt)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(imagePath);
        Name = name;
        ImagePath = imagePath;
        SignedAt = signedAt;
    }

    public string Name { get; }
    public string ImagePath { get; }
    public DateTime SignedAt { get; }
}