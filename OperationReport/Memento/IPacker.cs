using AetherSystem.OperationReport.DataSources.Schema;
using MemoryPack;

namespace AetherSystem.OperationReport.Memento;

public interface IPacker
{
    Type SourceType { get; }
    Type TargetType { get; }

    object Unpack(IPackableRecord unpacked);
    IPackableRecord Pack(object packed);
}

public abstract class Packer<TSource, TTarget> : IPacker 
    where TTarget : IMemoryPackable<TTarget>, IPackableRecord
    where TSource : notnull
{
    object IPacker.Unpack(IPackableRecord unpacked) => Unpack((TTarget)unpacked);
    IPackableRecord IPacker.Pack(object packed) => Pack((TSource)packed);
    
    public Type SourceType => typeof(TSource);
    public Type TargetType => typeof(TTarget);

    public abstract TTarget Pack(TSource packed);
    public abstract TSource Unpack(TTarget unpacked);
}
