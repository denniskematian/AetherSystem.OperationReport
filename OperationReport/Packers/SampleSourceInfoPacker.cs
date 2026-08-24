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
            provider.Pack<TablePacker.Record>(unpacked.Table),
            provider.Pack<TimestampColumnPacker.Record>(unpacked.TimestampColumn),
            provider.PackNullable<ColumnPacker.Record>(unpacked.BatchNumberColumn),
            unpacked.SampleColumns.Select(provider.Pack<ColumnPacker.Record>).ToArray());
    }

    public override SampleSourceInfo Unpack(Record packed, IPackerProvider provider)
    {
        return new SampleSourceInfo(
            packed.FilePath,
            packed.FileType,
            provider.Unpack<Table>(packed.Table),
            provider.Unpack<TimestampColumn>(packed.TimestampColumn),
            provider.UnpackNullable<Column>(packed.BatchNumberColumn),
            packed.SampleColumns.Select(provider.Unpack<Column>).ToArray());
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