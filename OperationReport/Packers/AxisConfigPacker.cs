using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class AxisConfigPacker : Packer<AxisConfig, AxisConfigPacker.Record>
{
    [MemoryPackable]
    public partial record Record(bool IsVisible, string Label) : IPackableRecord;

    public override Record Pack(AxisConfig unpacked, IPackerProvider provider)
    {
        return new Record(unpacked.IsVisible, unpacked.Label);
    }

    public override AxisConfig Unpack(Record packed, IPackerProvider provider)
    {
        return new AxisConfig
        {
            IsVisible = packed.IsVisible,
            Label = packed.Label
        };
    }
}