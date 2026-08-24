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
            provider.Pack<ProgramSectionPacker.Record>(unpacked.ProgramSection),
            unpacked.GeneratedAt,
            provider.PackNullable<SignaturePacker.Record>(unpacked.OperatorSignature),
            provider.PackNullable<SignaturePacker.Record>(unpacked.OfficerSignature));
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
            provider.Unpack<ProgramSection>(packed.ProgramSection),
            packed.GeneratedAt,
            provider.UnpackNullable<Signature>(packed.OperatorSignature),
            provider.UnpackNullable<Signature>(packed.OfficerSignature));
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
