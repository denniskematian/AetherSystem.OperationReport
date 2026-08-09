namespace AetherSystem.OperationReport.Gui.Options;

public interface IOption<out T>
{
    string Name { get; }
    T Value { get; }
}