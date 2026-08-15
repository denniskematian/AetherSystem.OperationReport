using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class SampleSourceInfoPacker : Packer<SampleSourceInfo, SampleSourceInfoPacker.Record>
{
    public override Record Pack(SampleSourceInfo unpacked, IPackerProvider provider)
    {
        return new Record(
            unpacked.FilePath,
            unpacked.FileType,
            (TablePacker.Record)provider.Pack(unpacked.Table),
            (TimestampColumnPacker.Record)provider.Pack(unpacked.TimestampColumn),
            unpacked.BatchNumberColumn is not null ? (ColumnPacker.Record)provider.Pack(unpacked.BatchNumberColumn) : null,
            unpacked.SampleColumns.Select(column => (ColumnPacker.Record)provider.Pack(column)).ToArray());
    }

    public override SampleSourceInfo Unpack(Record packed, IPackerProvider provider)
    {
        return new SampleSourceInfo(
            packed.FilePath,
            packed.FileType,
            (Table)provider.Unpack(packed.Table),
            (TimestampColumn)provider.Unpack(packed.TimestampColumn),
            packed.BatchNumberColumn is not null ? (Column)provider.Unpack(packed.BatchNumberColumn) : null,
            packed.SampleColumns.Select(column => (Column)provider.Unpack(column)).ToArray());
    }

    [MemoryPackable]
    public sealed partial record Record(
        string FilePath,
        FileType FileType,
        TablePacker.Record Table,
        TimestampColumnPacker.Record TimestampColumn,
        ColumnPacker.Record? BatchNumberColumn,
        IReadOnlyList<ColumnPacker.Record> SampleColumns) : IPackableRecord;
}