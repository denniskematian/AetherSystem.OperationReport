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
                .Select(log => (OperationSamplePacker.Record)provider.Pack(log))
                .ToArray(),
            unpacked.OperationLogLabels
                .Select(label => (OperationLogLabelPacker.Record)provider.Pack(label))
                .ToArray());
    }

    public override ProgramSteps Unpack(Record packed, IPackerProvider provider)
    {
        return new ProgramSteps(
            packed.OperationLogs
                .Select(log => (OperationSample)provider.Unpack(log))
                .ToArray(),
            packed.OperationLogLabels
                .Select(label => (OperationLogLabel)provider.Unpack(label))
                .ToArray());
    }

    [MemoryPackable]
    public sealed partial record Record(
        IReadOnlyList<OperationSamplePacker.Record> OperationLogs,
        IReadOnlyList<OperationLogLabelPacker.Record> OperationLogLabels) : IPackableRecord;
}
