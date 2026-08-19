using AetherSystem.OperationReport.Internals;
using AetherSystem.OperationReport.Memento;
using MemoryPack;
using ScottPlot;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class LinePatternPacker : Packer<LinePattern, LinePatternPacker.Record>
{
    [MemoryPackable]
    public partial record Record(LinePatternEnum LinePatternEnum) : IPackableRecord;

    public override Record Pack(LinePattern unpacked, IPackerProvider provider)
    {
        if (!Enum.TryParse<LinePatternEnum>(unpacked.Name, out var linePatternEnum))
            throw new InvalidOperationException($"Invalid LinePatternEnum name {unpacked.Name}");

        return new Record(linePatternEnum);
    }

    public override LinePattern Unpack(Record packed, IPackerProvider provider)
    {
        return packed.LinePatternEnum switch
        {
            LinePatternEnum.Solid => LinePattern.Solid,
            LinePatternEnum.Dashed => LinePattern.Dashed,
            LinePatternEnum.Dotted => LinePattern.Dotted,
            _ => ExceptionUtils.ThrowInvalidEnumArgument<LinePattern>(packed.LinePatternEnum)
        };
    }

    public enum LinePatternEnum
    {
        Solid,
        Dashed,
        Dotted
    }
}