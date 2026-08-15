using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Internals;
using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Timestamps;
using MemoryPack;

namespace AetherSystem.OperationReport.Packers;

public sealed partial class TimestampColumnPacker : Packer<TimestampColumn, TimestampColumnPacker.Record>
{
    public override Record Pack(TimestampColumn unpacked, IPackerProvider provider)
    {
        var (resolution, offset) = unpacked.Format switch
        {
            UnixTimestampFormat format => (format.Resolution, format.Offset),
            FractionalUnixTimestampFormat format => (format.Resolution, format.Offset),
            StringTimestampFormat => (TimestampResolution.HundredNanoseconds, TimeSpan.Zero),
            _ => throw new InvalidOperationException($"Invalid timestamp format: {unpacked.Format}"),
        };
        
        var stringFormat = (unpacked.Format as StringTimestampFormat)?.Format;

        return new Record(unpacked.Name, unpacked.Type, resolution, offset, stringFormat);
    }

    public override TimestampColumn Unpack(Record packed, IPackerProvider provider)
    {
        var format = packed.Type switch
        {
            ColumnType.Integer => new UnixTimestampFormat(packed.Resolution, packed.Offset),
            ColumnType.Real => new FractionalUnixTimestampFormat(packed.Resolution, packed.Offset),
            ColumnType.Text => new StringTimestampFormat(packed.Format ?? "O"),
            _ => ExceptionUtils.ThrowInvalidEnumArgument<ITimestampFormat>(packed.Type),
        };
        
        return new TimestampColumn(packed.Name, packed.Type, format);
    }

    [MemoryPackable]
    public sealed partial record Record(
        string Name,
        ColumnType Type,
        TimestampResolution Resolution,
        TimeSpan Offset,
        string? Format) : IPackableRecord;
}