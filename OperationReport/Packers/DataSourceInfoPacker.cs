using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class DataSourceInfoPacker : Packer<DataSourceInfo, DataSourceInfoPacker.Record>
{
    public override Record Pack(DataSourceInfo unpacked, IPackerProvider provider)
    {
        return new Record(unpacked.FilePath, unpacked.FileType);
    }

    public override DataSourceInfo Unpack(Record packed, IPackerProvider provider)
    {
        return new DataSourceInfo(packed.FilePath, packed.FileType);
    }

    [MemoryPackable]
    public sealed partial record Record(string FilePath, FileType FileType) : IPackableRecord;
}