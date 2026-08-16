using AetherSystem.OperationReport.Charting;
using AetherSystem.OperationReport.Memento;
using MemoryPack;
using ScottPlot;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class ColorInfoPacker : Packer<ColorInfo, ColorInfoPacker.Record>
{
    [MemoryPackable]
    public partial record Record(byte R, byte G, byte B) : IPackableRecord;

    public override Record Pack(ColorInfo unpacked, IPackerProvider provider)
    {
        return new Record(unpacked.Value.R, unpacked.Value.G, unpacked.Value.B);
    }

    public override ColorInfo Unpack(Record packed, IPackerProvider provider)
    {
        var color = new Color(packed.R, packed.G, packed.B);
        return new ColorInfo(color.ToHex(), color);
    }
}