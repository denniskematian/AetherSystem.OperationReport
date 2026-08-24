using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class OperationSourceInfoPacker : Packer<OperationSourceInfo, OperationSourceInfoPacker.Record>
{
    public override Record Pack(OperationSourceInfo unpacked, IPackerProvider provider)
    {
        return new Record(
            unpacked.FilePath,
            unpacked.FileType,
            provider.Pack<TablePacker.Record>(unpacked.Table),
            provider.Pack<TimestampColumnPacker.Record>(unpacked.TimestampColumn),
            provider.Pack<ColumnPacker.Record>(unpacked.CommentColumn));
    }

    public override OperationSourceInfo Unpack(Record packed, IPackerProvider provider)
    {
        return new OperationSourceInfo(
            packed.FilePath,
            packed.FileType,
            provider.Unpack<Table>(packed.Table),
            provider.Unpack<TimestampColumn>(packed.TimestampColumn),
            provider.Unpack<Column>(packed.CommentColumn));
    }

    [MemoryPackable]
    public sealed partial record Record(
        string FilePath,
        FileType FileType,
        TablePacker.Record Table,
        TimestampColumnPacker.Record TimestampColumn,
        ColumnPacker.Record CommentColumn) : IPackableRecord;
}