using AetherSystem.OperationReport.Entities;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class OperationSamplePacker : Packer<OperationSample, OperationSamplePacker.Record>
{
    public override Record Pack(OperationSample unpacked, IPackerProvider provider) =>
        new(unpacked.Timestamp, unpacked.Comment, unpacked.Values);

    public override OperationSample Unpack(Record packed, IPackerProvider provider) =>
        new(packed.Timestamp, packed.Comment, packed.Values.ToArray());

    [MemoryPackable]
    public sealed partial record Record(
        DateTime Timestamp,
        string Comment,
        IReadOnlyList<double> Values) : IPackableRecord;
}
