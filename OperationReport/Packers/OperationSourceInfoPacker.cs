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
            (TablePacker.Record)provider.Pack(unpacked.Table),
            (TimestampColumnPacker.Record)provider.Pack(unpacked.TimestampColumn),
            (ColumnPacker.Record)provider.Pack(unpacked.CommentColumn));
    }

    public override OperationSourceInfo Unpack(Record packed, IPackerProvider provider)
    {
        return new OperationSourceInfo(
            packed.FilePath,
            packed.FileType,
            (Table)provider.Unpack(packed.Table),
            (TimestampColumn)provider.Unpack(packed.TimestampColumn),
            (Column)provider.Unpack(packed.CommentColumn));
    }

    [MemoryPackable]
    public sealed partial record Record(
        string FilePath,
        FileType FileType,
        TablePacker.Record Table,
        TimestampColumnPacker.Record TimestampColumn,
        ColumnPacker.Record CommentColumn) : IPackableRecord;
}