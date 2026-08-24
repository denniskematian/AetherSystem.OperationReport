using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Reporting;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class OperationLogLabelPacker : Packer<OperationLogLabel, OperationLogLabelPacker.Record>
{
    public override Record Pack(OperationLogLabel unpacked, IPackerProvider provider) =>
        new(unpacked.Id, unpacked.Label);

    public override OperationLogLabel Unpack(Record packed, IPackerProvider provider) =>
        new(packed.Id, packed.Label);

    [MemoryPackable]
    public sealed partial record Record(int Id, string Label) : IPackableRecord;
}
