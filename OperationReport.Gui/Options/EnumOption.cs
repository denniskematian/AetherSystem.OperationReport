namespace AetherSystem.OperationReport.Gui.Options;

public record EnumOption<T>(T Value) : IOption<T> where T : Enum
{
    public string Name => Value.ToString();
}