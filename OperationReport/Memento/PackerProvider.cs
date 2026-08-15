namespace AetherSystem.OperationReport.Memento;

public sealed class PackerProvider(IReadOnlyList<IPacker> packers) : IPackerProvider
{
    private IPacker GetPacker(Type sourceType)
    {
        return packers.First(p => p.SourceType == sourceType);
    }

    private IPacker GetUnpacker(Type targetType)
    {
        return packers.First(p => p.TargetType == targetType);
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