using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.ValueObjects;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class PresetConfigPacker : Packer<PresetConfig, PresetConfigPacker.Record>
{
    public override Record Pack(PresetConfig unpacked, IPackerProvider provider)
    {
        return new Record(
            (SampleSourceInfoPacker.Record)provider.Pack(unpacked.SampleDataSource),
            (OperationSourceInfoPacker.Record)provider.Pack(unpacked.OperationDataSource),
            unpacked.SampleReferences.Select(i => (SampleReferenceConfigPacker.Record)provider.Pack(i)).ToArray());
    }

    public override PresetConfig Unpack(Record packed, IPackerProvider provider)
    {
        return new PresetConfig
        {
            SampleDataSource = (SampleSourceInfo)provider.Unpack(packed.SampleSourceInfo),
            OperationDataSource = (OperationSourceInfo)provider.Unpack(packed.OperationSourceInfo),
            SampleReferences = packed.SampleReferences.Select(i => (SampleReferenceConfig)provider.Unpack(i)).ToArray(),
        };
    }

    [MemoryPackable]
    public sealed partial record Record(
        SampleSourceInfoPacker.Record SampleSourceInfo,
        OperationSourceInfoPacker.Record OperationSourceInfo,
        IReadOnlyList<SampleReferenceConfigPacker.Record> SampleReferences) : IPackableRecord;
}