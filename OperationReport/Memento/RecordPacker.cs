using MemoryPack;

namespace AetherSystem.OperationReport.Memento;

public class RecordPacker<T> : Packer<T, T> where T : IMemoryPackable<T>, IPackableRecord
{
    public override T Pack(T unpacked, IPackerProvider provider)
    {
        return unpacked;
    }

    public override T Unpack(T packed, IPackerProvider provider)
    {
        return packed;
    }
}