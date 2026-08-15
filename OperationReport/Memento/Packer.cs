using MemoryPack;

namespace AetherSystem.OperationReport.Memento;

public abstract class Packer<TSource, TTarget> : IPacker 
    where TTarget : IMemoryPackable<TTarget>, IPackableRecord
    where TSource : notnull
{
    object IPacker.Unpack(IPackableRecord packed, IPackerProvider provider) => Unpack((TTarget)packed, provider);
    IPackableRecord IPacker.Pack(object unpacked, IPackerProvider provider) => Pack((TSource)unpacked, provider);
    
    public Type SourceType => typeof(TSource);
    public Type TargetType => typeof(TTarget);

    public abstract TTarget Pack(TSource unpacked, IPackerProvider provider);
    public abstract TSource Unpack(TTarget packed, IPackerProvider provider);
}