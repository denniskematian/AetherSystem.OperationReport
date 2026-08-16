using System.Collections.Concurrent;
using System.IO.Hashing;
using MemoryPack;

namespace AetherSystem.OperationReport.Memento;

public class PackableRegistry(IPackerProvider provider)
{
    private readonly ConcurrentDictionary<ulong, IPackableRecord> _registry = [];

    public bool IsDirty { get; private set; }

    public event EventHandler? PreSaveEvent;
    
    public void Put<T>(ReadOnlySpan<char> key, T value) where T : notnull
    {
        _registry[HashCode(key)] = provider.Pack(value);
        IsDirty = true;
    }
    
    public void PutCollection<T>(ReadOnlySpan<char> key, IEnumerable<T> items) where T : notnull
    {
        var collection = new CollectionPack([
            ..items.Select(item => provider.Pack(item))
        ]);
        _registry[HashCode(key)] = provider.Pack(collection);
        IsDirty = true;
    }

    public void Delete(ReadOnlySpan<char> key)
    {
        if (_registry.Remove(HashCode(key), out _))
            IsDirty = true;
    }

    public void Clear()
    {
        _registry.Clear();
        IsDirty = true;
    }
    
    public T? Get<T>(ReadOnlySpan<char> key) where T : notnull
    {
        if (!_registry.TryGetValue(HashCode(key), out var record))
            return default;

        return (T)provider.Unpack(record);
    }
    
    public IEnumerable<T> GetCollection<T>(ReadOnlySpan<char> key) where T : notnull
    {
        if (!_registry.TryGetValue(HashCode(key), out var record))
            return [];
        
        if(record is not CollectionPack collection)
            throw new InvalidOperationException($"The record with key '{key}' is not a collection.");

        return collection.Items.Select(item => (T)provider.Unpack(item));
    }
    
    public bool ContainsKey(ReadOnlySpan<char> key) => _registry.ContainsKey(HashCode(key));

    public async Task LoadAsync(Stream stream, bool clearExisting, CancellationToken cancellationToken = default)
    {
        if(!stream.CanRead)
            throw new ArgumentException("Stream is not readable.");
        
        var packableRegistry = await MemoryPackSerializer.DeserializeAsync<Dictionary<ulong, IPackableRecord>>(stream, cancellationToken: cancellationToken)
                               ?? throw new ArgumentException("Stream is not readable.");
        
        if(clearExisting)
            _registry.Clear();

        foreach (var (key, value) in packableRegistry)
            _registry[key] = value;
    }

    public async Task SaveAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        PreSaveEvent?.Invoke(this, EventArgs.Empty);
        if(!stream.CanWrite)
            throw new ArgumentException("Stream is not writable.");

        await MemoryPackSerializer.SerializeAsync(stream, _registry, cancellationToken: cancellationToken);
        IsDirty = false;
    }

    private static ulong HashCode(ReadOnlySpan<char> key)
    {
        const long seed = 1727179140500896136;
        Span<byte> bytes = stackalloc byte[key.Length];
        for(var i = 0; i < key.Length; i++)
            bytes[i] = (byte)key[i];

        return XxHash64.HashToUInt64(bytes, seed);
    }
}