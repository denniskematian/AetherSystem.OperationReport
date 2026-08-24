using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Reporting;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class SignaturePacker : Packer<Signature, SignaturePacker.Record>
{
    public override Record Pack(Signature unpacked, IPackerProvider provider) =>
        new(unpacked.Name, unpacked.ImagePath, unpacked.SignedAt);

    public override Signature Unpack(Record packed, IPackerProvider provider) =>
        new(packed.Name, packed.ImagePath, packed.SignedAt);

    [MemoryPackable]
    public sealed partial record Record(string Name, string ImagePath, DateTime SignedAt) : IPackableRecord;
}
