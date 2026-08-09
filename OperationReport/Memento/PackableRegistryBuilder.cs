using MemoryPack;
using MemoryPack.Formatters;

namespace AetherSystem.OperationReport.Memento;

public sealed class PackableRegistryBuilder
{
    private readonly HashSet<Type> _sourceTypes = [];
    private readonly HashSet<Type> _targetTypes = [];
    private readonly List<IPacker> _packers = [];

    public void Add(IPacker packer)
    {
        CheckSourceType(packer.SourceType);
        CheckTargetType(packer.TargetType);

        _sourceTypes.Add(packer.SourceType);
        _targetTypes.Add(packer.TargetType);
        _packers.Add(packer);
    }

    public PackableRegistry Build()
    {
        List<(ushort, Type)> packableTypes = [
            (0, typeof(CollectionPack)),
            .._packers.Index().Select(i => ((ushort)(i.Index + 1), i.Item.TargetType))
        ];

        var registry = new DynamicUnionFormatter<IPackableRecord>([
            ..packableTypes
        ]);

        MemoryPackFormatterProvider.Register(registry);
        return new PackableRegistry(_packers.AsReadOnly());
    }

    private void CheckSourceType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (_sourceTypes.Contains(type))
            throw new InvalidOperationException($"Source type {type} is already registered.");

        if (Nullable.GetUnderlyingType(type) is not null)
            throw new InvalidOperationException($"Source type {type} can't be nullable value types.");
    }

    private void CheckTargetType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (_targetTypes.Contains(type))
            throw new InvalidOperationException($"Target type {type} is already registered.");

        if (Nullable.GetUnderlyingType(type) is not null)
            throw new InvalidOperationException($"Target type {type} can't be nullable value types.");

        if (type.GetInterfaces().All(t => t != typeof(IPackableRecord)))
            throw new InvalidOperationException($"Target type {type} must implements {nameof(IPackableRecord)}.");

        if (!type.GetInterfaces()
                .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IMemoryPackable<>)))
            throw new InvalidOperationException($"Target type {type} must implements {nameof(IMemoryPackable<>)}.");
    }
}