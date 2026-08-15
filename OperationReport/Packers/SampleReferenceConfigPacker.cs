using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.ValueObjects;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class SampleReferenceConfigPacker : Packer<SampleReferenceConfig, SampleReferenceConfigPacker.Record>
{
    public override Record Pack(SampleReferenceConfig unpacked, IPackerProvider provider)
    {
        return new Record(unpacked.Column, unpacked.IsIncluded, unpacked.Index, unpacked.Label);
    }

    public override SampleReferenceConfig Unpack(Record packed, IPackerProvider provider)
    {
        return new SampleReferenceConfig
        {
            Column = packed.Column,
            IsIncluded = packed.IsIncluded,
            Index = packed.Index,
            Label = packed.Label
        };
    }

    [MemoryPackable]
    public sealed partial record Record(string Column, bool IsIncluded, int Index, string Label) : IPackableRecord;
}