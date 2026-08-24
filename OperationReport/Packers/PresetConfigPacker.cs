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
            provider.Pack<SampleSourceInfoPacker.Record>(unpacked.SampleDataSource),
            provider.Pack<OperationSourceInfoPacker.Record>(unpacked.OperationDataSource),
            unpacked.SampleReferences.Select(provider.Pack<SampleReferenceConfigPacker.Record>).ToArray());
    }

    public override PresetConfig Unpack(Record packed, IPackerProvider provider)
    {
        return new PresetConfig
        {
            SampleDataSource = provider.Unpack<SampleSourceInfo>(packed.SampleSourceInfo),
            OperationDataSource = provider.Unpack<OperationSourceInfo>(packed.OperationSourceInfo),
            SampleReferences = packed.SampleReferences.Select(provider.Unpack<SampleReferenceConfig>).ToArray(),
        };
    }

    [MemoryPackable]
    public sealed partial record Record(
        SampleSourceInfoPacker.Record SampleSourceInfo,
        OperationSourceInfoPacker.Record OperationSourceInfo,
        IReadOnlyList<SampleReferenceConfigPacker.Record> SampleReferences) : IPackableRecord;
}