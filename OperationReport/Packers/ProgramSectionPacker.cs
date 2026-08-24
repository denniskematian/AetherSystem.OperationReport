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
            provider.Pack<ProgramStepsPacker.Record>(unpacked.ProgramSteps),
            unpacked.Parameters
                .Select(provider.Pack<ProgramParameterPacker.Record>)
                .ToArray(),
            unpacked.IsReleased,
            unpacked.StartedBy,
            unpacked.Messages
                .Select(provider.Pack<ProgramMessagePacker.Record>)
                .ToArray());
    }

    public override ProgramSection Unpack(Record packed, IPackerProvider provider)
    {
        return new ProgramSection(
            packed.StartedAt,
            packed.FinishedAt,
            packed.ProgramType,
            provider.Unpack<ProgramSteps>(packed.ProgramSteps),
            packed.Parameters
                .Select(provider.Unpack<ProgramParameter>)
                .ToArray(),
            packed.IsReleased,
            packed.StartedBy,
            packed.Messages
                .Select(provider.Unpack<ProgramMessage>)
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
