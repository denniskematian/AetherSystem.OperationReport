using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Reporting;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class ProgramSectionPacker : Packer<ProgramSection, ProgramSectionPacker.Record>
{
    public override Record Pack(ProgramSection unpacked, IPackerProvider provider)
    {
        return new Record(
            unpacked.StartedAt,
            unpacked.FinishedAt,
            unpacked.ProgramType,
            (ProgramStepsPacker.Record)provider.Pack(unpacked.ProgramSteps),
            unpacked.Parameters
                .Select(parameter => (ProgramParameterPacker.Record)provider.Pack(parameter))
                .ToArray(),
            unpacked.IsReleased,
            unpacked.StartedBy,
            unpacked.Messages
                .Select(message => (ProgramMessagePacker.Record)provider.Pack(message))
                .ToArray());
    }

    public override ProgramSection Unpack(Record packed, IPackerProvider provider)
    {
        return new ProgramSection(
            packed.StartedAt,
            packed.FinishedAt,
            packed.ProgramType,
            (ProgramSteps)provider.Unpack(packed.ProgramSteps),
            packed.Parameters
                .Select(parameter => (ProgramParameter)provider.Unpack(parameter))
                .ToArray(),
            packed.IsReleased,
            packed.StartedBy,
            packed.Messages
                .Select(message => (ProgramMessage)provider.Unpack(message))
                .ToArray());
    }

    [MemoryPackable]
    public sealed partial record Record(
        DateTime StartedAt,
        DateTime FinishedAt,
        string ProgramType,
        ProgramStepsPacker.Record ProgramSteps,
        IReadOnlyList<ProgramParameterPacker.Record> Parameters,
        bool IsReleased,
        string? StartedBy,
        IReadOnlyList<ProgramMessagePacker.Record> Messages) : IPackableRecord;
}
