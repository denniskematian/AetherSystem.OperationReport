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
            provider.Pack<AxisConfigPacker.Record>(unpacked.LeftAxis),
            provider.Pack<AxisConfigPacker.Record>(unpacked.RightAxis),
            unpacked.Series.Select(provider.Pack<SeriesConfigPacker.Record>).ToArray(),
            provider.Pack<MarkerConfigPacker.Record>(unpacked.OperationMarker),
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
            LeftAxis = provider.Unpack<AxisConfig>(packed.LeftAxis),
            RightAxis = provider.Unpack<AxisConfig>(packed.RightAxis),
            Series = packed.Series.Select(provider.Unpack<SeriesConfig>).ToArray(),
            OperationMarker = provider.Unpack<MarkerConfig>(packed.OperationMarker),
            ShowDateInBottomTicks = packed.ShowDateInBottomTicks,
            LeftAxisLimit = packed.LeftAxisLimit,
            RightAxisLimit = packed.RightAxisLimit,
            BottomAxisLimit = packed.BottomAxisLimit,
            PrintArea = packed.PrintArea,
        };
    }
}