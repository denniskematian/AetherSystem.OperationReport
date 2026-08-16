using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Memento;
using MemoryPack;
using ScottPlot;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class MarkerConfigPacker : Packer<MarkerConfig, MarkerConfigPacker.Record>
{
    [MemoryPackable]
    public partial record Record(
        string Column,
        bool IsVisible,
        MarkerShape Shape,
        ColorInfoPacker.Record Color) : IPackableRecord;

    public override Record Pack(MarkerConfig unpacked, IPackerProvider provider)
    {
        return new Record(
            unpacked.Column,
            unpacked.IsVisible,
            unpacked.Shape,
            (ColorInfoPacker.Record)provider.Pack(unpacked.Color));
    }

    public override MarkerConfig Unpack(Record packed, IPackerProvider provider)
    {
        return new MarkerConfig
        {
            Column = packed.Column,
            IsVisible = packed.IsVisible,
            Shape = packed.Shape,
            Color = (ColorInfo)provider.Unpack(packed.Color)
        };
    }
}