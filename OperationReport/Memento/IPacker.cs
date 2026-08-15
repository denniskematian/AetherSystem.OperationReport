namespace AetherSystem.OperationReport.Memento;

public interface IPacker
{
    Type SourceType { get; }
    Type TargetType { get; }

    object Unpack(IPackableRecord packed, IPackerProvider provider);
    IPackableRecord Pack(object unpacked, IPackerProvider provider);
}