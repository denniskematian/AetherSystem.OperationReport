using AetherSystem.OperationReport.Memento;
using MemoryPack;
using ScottPlot;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class LinePatternPacker : Packer<LinePattern, LinePatternPacker.Record>
{
    [MemoryPackable]
    public partial record Record(float[] Intervals, float Phase, string Name) : IPackableRecord;

    public override Record Pack(LinePattern unpacked, IPackerProvider provider)
    {
        return new Record(unpacked.Intervals, unpacked.Phase, unpacked.Name);
    }

    public override LinePattern Unpack(Record packed, IPackerProvider provider)
    {
        return new LinePattern(packed.Intervals, packed.Phase, packed.Name);
    }
}