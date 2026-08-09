namespace AetherSystem.OperationReport.Gui.Options;

public record Option<T>(string Name, T Value) : IOption<T>;