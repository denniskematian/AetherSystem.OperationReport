using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class AxisLimitPacker : Packer<AxisLimit, AxisLimitPacker.Record>
{
    [MemoryPackable]
    public partial record Record(double Min, double Max) : IPackableRecord;

    public override Record Pack(AxisLimit unpacked, IPackerProvider provider)
    {
        return new Record(unpacked.Min, unpacked.Max);
    }

    public override AxisLimit Unpack(Record packed, IPackerProvider provider)
    {
        return new AxisLimit
        {
            Min = packed.Min,
            Max = packed.Max
        };
    }
}