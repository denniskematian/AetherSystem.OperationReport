using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Memento;
using MemoryPack;
using ScottPlot;

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
        CoordinateRange? LeftAxisLimit,
        CoordinateRange? RightAxisLimit,
        CoordinateRange? BottomAxisLimit,
        CoordinateRect? PrintArea) : IPackableRecord;

    public override Record Pack(ChartConfig unpacked, IPackerProvider provider)
    {
        return new Record(
            (AxisConfigPacker.Record)provider.Pack(unpacked.LeftAxis),
            (AxisConfigPacker.Record)provider.Pack(unpacked.RightAxis),
            unpacked.Series.Select(s => (SeriesConfigPacker.Record)provider.Pack(s)).ToArray(),
            (MarkerConfigPacker.Record)provider.Pack(unpacked.OperationMarker),
            unpacked.ShowDateInBottomTicks,
            unpacked.LeftAxisLimit,
            unpacked.RightAxisLimit,
            unpacked.BottomAxisLimit,
            unpacked.PrintArea);
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
            LeftAxisLimit = packed.LeftAxisLimit,
            RightAxisLimit = packed.RightAxisLimit,
            BottomAxisLimit = packed.BottomAxisLimit,
            PrintArea = packed.PrintArea,
        };
    }
}