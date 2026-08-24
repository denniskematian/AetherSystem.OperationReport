using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Reporting;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class ProgramMessagePacker : Packer<ProgramMessage, ProgramMessagePacker.Record>
{
    public override Record Pack(ProgramMessage unpacked, IPackerProvider provider) =>
        new(unpacked.Timestamp, unpacked.Message);

    public override ProgramMessage Unpack(Record packed, IPackerProvider provider) =>
        new(packed.Timestamp, packed.Message);

    [MemoryPackable]
    public sealed partial record Record(DateTime Timestamp, string Message) : IPackableRecord;
}
