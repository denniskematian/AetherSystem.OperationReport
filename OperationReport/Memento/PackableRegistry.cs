using System.Collections.Concurrent;
using MemoryPack;

namespace AetherSystem.OperationReport.Memento;

public class PackableRegistry(IReadOnlyList<IPacker> packers)
{
    private readonly ConcurrentDictionary<string, IPackableRecord> _registry = [];

    public bool IsDirty { get; private set; }
    
    public void Put<T>(string key, T value) where T : notnull
    {
        var packer = GetPacker(typeof(T));
        _registry[key] = packer.Pack(value);
        IsDirty = true;
    }
    
    public void PutCollection<T>(string key, IEnumerable<T> items) where T : notnull
    {
        var packer = GetPacker(typeof(T));
        var collection = new CollectionPack([
            ..items.Select(item => packer.Pack(item))
        ]);
        _registry[key] = packer.Pack(collection);
        IsDirty = true;
    }

    public void Delete(string key)
    {
        if (_registry.Remove(key, out _))
            IsDirty = true;
    }

    public void Clear()
    {
        _registry.Clear();
        IsDirty = true;
    }
    
    public T? Get<T>(string key) where T : notnull
    {
        if (!_registry.TryGetValue(key, out var record))
        {
            return default;
        }

        var packer = GetPacker(typeof(T));
        return (T)packer.Unpack(record);
    }
    
    public IEnumerable<T> GetCollection<T>(string key) where T : notnull
    {
        if (!_registry.TryGetValue(key, out var record))
            return [];
        
        if(record is not CollectionPack collection)
            throw new InvalidOperationException($"The record with key '{key}' is not a collection.");

        var packer = GetPacker(typeof(T));
        return collection.Items.Select(item => (T)packer.Unpack(item));
    }
    
    public bool ContainsKey(string key) => _registry.ContainsKey(key);

    public async Task LoadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if(!stream.CanRead)
            throw new ArgumentException("Stream is not readable.");
        
        var packableRegistry = await MemoryPackSerializer.DeserializeAsync<Dictionary<string, IPackableRecord>>(stream, cancellationToken: cancellationToken)
                               ?? throw new ArgumentException("Stream is not readable.");
        
        foreach (var (key, value) in packableRegistry)
            _registry[key] = value;
    }

    public async Task SaveAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if(!stream.CanWrite)
            throw new ArgumentException("Stream is not writable.");

        await MemoryPackSerializer.SerializeAsync(stream, _registry, cancellationToken: cancellationToken);
        IsDirty = false;
    }

    private IPacker GetPacker(Type sourceType)
    {
        return packers.First(i => i.SourceType == sourceType);
    }
}