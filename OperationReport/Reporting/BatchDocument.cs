namespace AetherSystem.OperationReport.Reporting;

public class BatchDocument
{
    public BatchDocument(
        string programNumber,
        string batchNumber,
        string title,
        string serialNumber,
        string companyName,
        string companyLogoPath,
        ProgramSection programSection,
        DateTime generatedAt,
        Signature? operatorSignature,
        Signature? officerSignature)
    {
        ArgumentException.ThrowIfNullOrEmpty(batchNumber);
        ArgumentException.ThrowIfNullOrEmpty(title);
        ArgumentException.ThrowIfNullOrEmpty(serialNumber);
        ArgumentException.ThrowIfNullOrEmpty(companyName);
        ArgumentException.ThrowIfNullOrEmpty(companyLogoPath);
        ArgumentNullException.ThrowIfNull(programSection);
        
        ProgramNumber = programNumber;
        BatchNumber = batchNumber;
        Title = title;
        SerialNumber = serialNumber;
        CompanyName = companyName;
        CompanyLogoPath = companyLogoPath;
        ProgramSection = programSection;
        GeneratedAt = generatedAt;
        OperatorSignature = operatorSignature;
        OfficerSignature = officerSignature;
    }

    public string ProgramNumber { get; }
    public string BatchNumber { get; }
    public string Title { get; }
    public string SerialNumber { get; }
    public string CompanyName { get; }
    public string CompanyLogoPath { get; }
    public ProgramSection ProgramSection { get; }
    public DateTime GeneratedAt { get; }
    public Signature? OperatorSignature { get; }
    public Signature? OfficerSignature { get; }
}