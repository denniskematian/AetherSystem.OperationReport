using MemoryPack;

namespace AetherSystem.OperationReport.Memento;

[MemoryPackable]
public sealed partial class CollectionPack(IReadOnlyCollection<IPackableRecord> items) 
    : IPackableRecord
{
    public readonly IReadOnlyCollection<IPackableRecord> Items = items;
}