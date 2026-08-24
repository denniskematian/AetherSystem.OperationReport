namespace AetherSystem.OperationReport.Memento;

public interface IPackerProvider
{
    IPackableRecord Pack(object value);
    object Unpack(IPackableRecord record);
    
    T Pack<T>(object value) where T : IPackableRecord;
    T? PackNullable<T>(object? value) where T : IPackableRecord;
    
    T Unpack<T>(IPackableRecord record);
    T? UnpackNullable<T>(IPackableRecord? record);
}