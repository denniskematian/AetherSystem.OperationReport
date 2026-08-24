using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class TablePacker : Packer<Table, TablePacker.Record>
{
    public override Record Pack(Table unpacked, IPackerProvider provider)
    {
        var packedColumns = unpacked.Columns
            .Select(provider.Pack)
            .ToArray();
        
        return new Record(unpacked.Name, packedColumns);
    }

    public override Table Unpack(Record packed, IPackerProvider provider)
    {
        var unpackedColumns = packed.Columns
            .Select(provider.Unpack<Column>)
            .ToArray();
        
        return new Table(packed.Name, unpackedColumns);
    }

    [MemoryPackable]
    public sealed partial record Record(string Name, IReadOnlyList<IPackableRecord> Columns) : IPackableRecord;
}