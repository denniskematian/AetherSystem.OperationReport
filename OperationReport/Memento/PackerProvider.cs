namespace AetherSystem.OperationReport.Memento;

public sealed class PackerProvider : IPackerProvider
{
    private readonly Dictionary<Type, IPacker> _packers;

    public PackerProvider(IEnumerable<IPacker> packers)
    {
        _packers = [];
        foreach (var packer in packers)
        {
            _packers.Add(packer.SourceType, packer);
            if(packer.SourceType != packer.TargetType)
                _packers.Add(packer.TargetType, packer);
        }
    }

    private IPacker GetPacker(Type sourceType)
    {
        return !_packers.TryGetValue(sourceType, out var result)
               ? throw new InvalidOperationException($"No packer found for type {sourceType}.")
               : result;
    }

    private IPacker GetUnpacker(Type targetType)
    {
        return !_packers.TryGetValue(targetType, out var result)
            ? throw new InvalidOperationException($"No unpacker found for type {targetType}.")
            : result;
    }

    public IPackableRecord Pack(object value)
    {
        return GetPacker(value.GetType()).Pack(value, this);
    }
    
    public object Unpack(IPackableRecord record)
    {
        return GetUnpacker(record.GetType()).Unpack(record, this);
    }
    
    public T Pack<T>(object value) where T : IPackableRecord
    {
        return (T)GetPacker(value.GetType()).Pack(value, this);
    }
    
    public T? PackNullable<T>(object? value) where T : IPackableRecord
    {
        return value is null ? default : (T)GetPacker(value.GetType()).Pack(value, this);
    }
    
    public T Unpack<T>(IPackableRecord record)
    {
        return (T)GetUnpacker(record.GetType()).Unpack(record, this);
    }
    
    public T? UnpackNullable<T>(IPackableRecord? record)
    {
        return record is null ? default : (T)GetUnpacker(record.GetType()).Unpack(record, this);
    }
}