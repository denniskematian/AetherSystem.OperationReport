using MemoryPack;
using MemoryPack.Formatters;

namespace AetherSystem.OperationReport.Memento;

public sealed class PackableRegistryBuilder
{
    private readonly HashSet<Type> _types = [];
    private readonly List<IPacker> _packers = [];

    private void Add(IPacker packer)
    {
        CheckSourceType(packer.SourceType);
        CheckTargetType(packer.TargetType);

        _types.Add(packer.SourceType);
        _types.Add(packer.TargetType);
        _packers.Add(packer);
    }
    
    public PackableRegistryBuilder Add<T>() where T : IPacker, new()
    {
        Add(new T());
        return this;
    }

    public PackableRegistryBuilder AddPackable<T>() where T : IMemoryPackable<T>, IPackableRecord
    {
        var type = typeof(T);
        if (!_types.Add(type))
            throw new InvalidOperationException($"Type {type} is already registered.");

        _packers.Add(new RecordPacker<T>());
        return this;
    }

    public IPackerProvider Build()
    {
        List<(ushort, Type)> packableTypes = [
            (0, typeof(CollectionPack)),
            .._packers.Index().Select(i => ((ushort)(i.Index + 1), i.Item.TargetType))
        ];

        var registry = new DynamicUnionFormatter<IPackableRecord>([
            ..packableTypes
        ]);

        MemoryPackFormatterProvider.Register(registry);
        return new PackerProvider(_packers);
    }

    private void CheckSourceType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (_types.Contains(type))
            throw new InvalidOperationException($"Type {type} is already registered.");

        if (Nullable.GetUnderlyingType(type) is not null)
            throw new InvalidOperationException($"Source type {type} can't be nullable value types.");
    }

    private void CheckTargetType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (_types.Contains(type))
            throw new InvalidOperationException($"Type {type} is already registered.");

        if (Nullable.GetUnderlyingType(type) is not null)
            throw new InvalidOperationException($"Target type {type} can't be nullable value types.");

        if (type.GetInterfaces().All(t => t != typeof(IPackableRecord)))
            throw new InvalidOperationException($"Target type {type} must implements {nameof(IPackableRecord)}.");

        if (!type.GetInterfaces()
                .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IMemoryPackable<>)))
            throw new InvalidOperationException($"Target type {type} must implements {nameof(IMemoryPackable<>)}.");
    }
}