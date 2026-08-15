namespace AetherSystem.OperationReport.Memento;

public sealed class PackerProvider(IReadOnlyList<IPacker> packers) : IPackerProvider
{
    private IPacker GetPacker(Type sourceType)
    {
        return packers.FirstOrDefault(p => p.SourceType == sourceType)
            ?? throw new InvalidOperationException($"No packer found for type {sourceType}");
    }

    private IPacker GetUnpacker(Type targetType)
    {
        return packers.FirstOrDefault(p => p.TargetType == targetType)
            ?? throw new InvalidOperationException($"No unpacker found for type {targetType}");
    }

    public IPackableRecord Pack(object value)
    {
        return GetPacker(value.GetType()).Pack(value, this);
    }
    
    public object Unpack(IPackableRecord record)
    {
        return GetUnpacker(record.GetType()).Unpack(record, this);
    }
}