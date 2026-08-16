using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Memento;
using MemoryPack;
using ScottPlot;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class SeriesConfigPacker : Packer<SeriesConfig, SeriesConfigPacker.Record>
{
    [MemoryPackable]
    public partial record Record(
        string Column,
        bool IsVisible,
        AxisPosition AxisPosition,
        string Label,
        ColorInfoPacker.Record Color,
        LinePatternPacker.Record LinePattern) : IPackableRecord;

    public override Record Pack(SeriesConfig unpacked, IPackerProvider provider)
    {
        return new Record(
            unpacked.Column,
            unpacked.IsVisible,
            unpacked.AxisPosition,
            unpacked.Label,
            (ColorInfoPacker.Record)provider.Pack(unpacked.Color),
            (LinePatternPacker.Record)provider.Pack(unpacked.LinePattern));
    }

    public override SeriesConfig Unpack(Record packed, IPackerProvider provider)
    {
        return new SeriesConfig()
        {
            Column = packed.Column,
            IsVisible = packed.IsVisible,
            AxisPosition = packed.AxisPosition,
            Label = packed.Label,
            Color = (ColorInfo)provider.Unpack(packed.Color),
            LinePattern = (LinePattern)provider.Unpack(packed.LinePattern),
        };
    }
}