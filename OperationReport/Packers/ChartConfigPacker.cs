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
        AxisLimitPacker.Record? LeftAxisLimit,
        AxisLimitPacker.Record? RightAxisLimit,
        AxisLimitPacker.Record? BottomAxisLimit) : IPackableRecord;

    public override Record Pack(ChartConfig unpacked, IPackerProvider provider)
    {
        return new Record(
            (AxisConfigPacker.Record)provider.Pack(unpacked.LeftAxis),
            (AxisConfigPacker.Record)provider.Pack(unpacked.RightAxis),
            unpacked.Series.Select(s => (SeriesConfigPacker.Record)provider.Pack(s)).ToArray(),
            (MarkerConfigPacker.Record)provider.Pack(unpacked.OperationMarker),
            unpacked.ShowDateInBottomTicks,
            unpacked.LeftAxisLimit is null ? null : (AxisLimitPacker.Record)provider.Pack(unpacked.LeftAxisLimit),
            unpacked.RightAxisLimit is null ? null : (AxisLimitPacker.Record)provider.Pack(unpacked.RightAxisLimit),
            unpacked.BottomAxisLimit is null ? null : (AxisLimitPacker.Record)provider.Pack(unpacked.BottomAxisLimit));
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
            LeftAxisLimit = packed.LeftAxisLimit is null ? null : (AxisLimit)provider.Unpack(packed.LeftAxisLimit),
            RightAxisLimit = packed.RightAxisLimit is null ? null : (AxisLimit)provider.Unpack(packed.RightAxisLimit),
            BottomAxisLimit = packed.BottomAxisLimit is null ? null : (AxisLimit)provider.Unpack(packed.BottomAxisLimit),
        };
    }
}