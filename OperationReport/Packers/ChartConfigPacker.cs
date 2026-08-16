using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class ChartConfigPacker : Packer<ChartConfig, ChartConfigPacker.Record>
{
    [MemoryPackable]
    public partial record Record(
        AxisConfigPacker.Record LeftAxis,
        AxisConfigPacker.Record RightAxis,
        IReadOnlyList<SeriesConfigPacker.Record> Series,
        MarkerConfigPacker.Record OperationMarker,
        bool ShowDateInBottomTicks,
        AxisRangePacker.Record? LeftAxisRange,
        AxisRangePacker.Record? RightAxisRange,
        AxisRangePacker.Record? BottomAxisRange) : IPackableRecord;

    public override Record Pack(ChartConfig unpacked, IPackerProvider provider)
    {
        return new Record(
            (AxisConfigPacker.Record)provider.Pack(unpacked.LeftAxis),
            (AxisConfigPacker.Record)provider.Pack(unpacked.RightAxis),
            unpacked.Series.Select(s => (SeriesConfigPacker.Record)provider.Pack(s)).ToArray(),
            (MarkerConfigPacker.Record)provider.Pack(unpacked.OperationMarker),
            unpacked.ShowDateInBottomTicks,
            unpacked.LeftAxisRange is null ? null : (AxisRangePacker.Record)provider.Pack(unpacked.LeftAxisRange),
            unpacked.RightAxisRange is null ? null : (AxisRangePacker.Record)provider.Pack(unpacked.RightAxisRange),
            unpacked.BottomAxisRange is null ? null : (AxisRangePacker.Record)provider.Pack(unpacked.BottomAxisRange));
    }

    public override ChartConfig Unpack(Record packed, IPackerProvider provider)
    {
        return new ChartConfig
        {
            LeftAxis = (AxisConfig)provider.Unpack(packed.LeftAxis),
            RightAxis = (AxisConfig)provider.Unpack(packed.RightAxis),
            Series = packed.Series.Select(s => (SeriesConfig)provider.Unpack(s)).ToArray(),
            OperationMarker = (MarkerConfig)provider.Unpack(packed.OperationMarker),
            ShowDateInBottomTicks = packed.ShowDateInBottomTicks,
            LeftAxisRange = packed.LeftAxisRange is null ? null : (AxisRange)provider.Unpack(packed.LeftAxisRange),
            RightAxisRange = packed.RightAxisRange is null ? null : (AxisRange)provider.Unpack(packed.RightAxisRange),
            BottomAxisRange = packed.BottomAxisRange is null ? null : (AxisRange)provider.Unpack(packed.BottomAxisRange),
        };
    }
}