using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class ColumnPacker : Packer<Column, ColumnPacker.Record>
{
    public override Record Pack(Column unpacked, IPackerProvider provider)
    {
        return new Record(unpacked.Name, unpacked.Type);
    }

    public override Column Unpack(Record packed, IPackerProvider provider)
    {
        return new Column(packed.Name, packed.Type);
    }

    [MemoryPackable]
    public sealed partial record Record(string Name, ColumnType Type) : IPackableRecord;
}