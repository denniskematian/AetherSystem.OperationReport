using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Memento;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class SampleFilterQueryPacker : Packer<SampleFilterQuery, SampleFilterQueryPacker.Record>
{
    public override Record Pack(SampleFilterQuery unpacked, IPackerProvider provider)
    {
        return new Record(unpacked.From, unpacked.To, unpacked.BatchNumber);
    }

    public override SampleFilterQuery Unpack(Record packed, IPackerProvider provider)
    {
        return new SampleFilterQuery(
            packed.From,
            packed.To,
            packed.BatchNumber);
    }

    [MemoryPackable]
    public sealed partial record Record(
        DateTime? From,
        DateTime? To,
        int? BatchNumber) : IPackableRecord;
}