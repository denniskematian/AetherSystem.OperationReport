namespace AetherSystem.OperationReport.Memento;

public interface IPackerProvider
{
    IPackableRecord Pack(object value);
    object Unpack(IPackableRecord record);
}