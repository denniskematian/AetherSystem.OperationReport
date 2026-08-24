using AetherSystem.OperationReport.Entities;
using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Reporting;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class ProgramStepsPacker : Packer<ProgramSteps, ProgramStepsPacker.Record>
{
    public override Record Pack(ProgramSteps unpacked, IPackerProvider provider)
    {
        return new Record(
            unpacked.OperationLogs
                .Select(provider.Pack<OperationSamplePacker.Record>)
                .ToArray(),
            unpacked.OperationLogLabels
                .Select(provider.Pack<OperationLogLabelPacker.Record>)
                .ToArray());
    }

    public override ProgramSteps Unpack(Record packed, IPackerProvider provider)
    {
        return new ProgramSteps(
            packed.OperationLogs
                .Select(provider.Unpack<OperationSample>)
                .ToArray(),
            packed.OperationLogLabels
                .Select(provider.Unpack<OperationLogLabel>)
                .ToArray());
    }

    [MemoryPackable]
    public sealed partial record Record(
        IReadOnlyList<OperationSamplePacker.Record> OperationLogs,
        IReadOnlyList<OperationLogLabelPacker.Record> OperationLogLabels) : IPackableRecord;
}
