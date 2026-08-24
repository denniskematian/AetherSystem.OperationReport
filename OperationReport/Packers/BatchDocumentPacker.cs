using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Reporting;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class BatchDocumentPacker : Packer<BatchDocument, BatchDocumentPacker.Record>
{
    public override Record Pack(BatchDocument unpacked, IPackerProvider provider)
    {
        return new Record(
            unpacked.ProgramNumber,
            unpacked.BatchNumber,
            unpacked.Title,
            unpacked.SerialNumber,
            unpacked.CompanyName,
            unpacked.CompanyLogoPath,
            (ProgramSectionPacker.Record)provider.Pack(unpacked.ProgramSection),
            unpacked.GeneratedAt,
            unpacked.OperatorSignature is null ? null : (SignaturePacker.Record)provider.Pack(unpacked.OperatorSignature),
            unpacked.OfficerSignature is null ? null : (SignaturePacker.Record)provider.Pack(unpacked.OfficerSignature));
    }

    public override BatchDocument Unpack(Record packed, IPackerProvider provider)
    {
        return new BatchDocument(
            packed.ProgramNumber,
            packed.BatchNumber,
            packed.Title,
            packed.SerialNumber,
            packed.CompanyName,
            packed.CompanyLogoPath,
            (ProgramSection)provider.Unpack(packed.ProgramSection),
            packed.GeneratedAt,
            packed.OperatorSignature is null ? null : (Signature)provider.Unpack(packed.OperatorSignature),
            packed.OfficerSignature is null ? null : (Signature)provider.Unpack(packed.OfficerSignature));
    }

    [MemoryPackable]
    public sealed partial record Record(
        string ProgramNumber,
        string BatchNumber,
        string Title,
        string SerialNumber,
        string CompanyName,
        string CompanyLogoPath,
        ProgramSectionPacker.Record ProgramSection,
        DateTime GeneratedAt,
        SignaturePacker.Record? OperatorSignature,
        SignaturePacker.Record? OfficerSignature) : IPackableRecord;
}
