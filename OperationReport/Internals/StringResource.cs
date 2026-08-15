namespace AetherSystem.OperationReport.Internals;

internal static class StringResource
{
    public static string UndefinedEnum(Enum enumValue)
        => $"{enumValue.GetType().Name} value ({enumValue}) is not defined.";

    public static string? EmptyCollection(string? paramName)
    {
        paramName = paramName is null ? "collection" : $"collection {paramName}";
        return $"The {paramName} is empty.";
    }
}