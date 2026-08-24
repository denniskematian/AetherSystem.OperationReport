using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Reporting;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class ProgramParameterPacker : Packer<ProgramParameter, ProgramParameterPacker.Record>
{
    public override Record Pack(ProgramParameter unpacked, IPackerProvider provider) =>
        new(unpacked.Name, unpacked.Value);

    public override ProgramParameter Unpack(Record packed, IPackerProvider provider) =>
        new(packed.Name, packed.Value);

    [MemoryPackable]
    public sealed partial record Record(string Name, string Value) : IPackableRecord;
}
