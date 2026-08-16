using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class AxisRangePacker : Packer<AxisRange, AxisRangePacker.Record>
{
    [MemoryPackable]
    public partial record Record(double Min, double Max) : IPackableRecord;

    public override Record Pack(AxisRange unpacked, IPackerProvider provider)
    {
        return new Record(unpacked.Min, unpacked.Max);
    }

    public override AxisRange Unpack(Record packed, IPackerProvider provider)
    {
        return new AxisRange
        {
            Min = packed.Min,
            Max = packed.Max
        };
    }
}