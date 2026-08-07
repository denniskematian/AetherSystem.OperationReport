using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AetherSystem.OperationReport.Internals;

internal static class ExceptionUtils
{
    [DoesNotReturn]
    public static void ThrowUndefinedEnumException(Enum enumValue)
        => throw new InvalidEnumArgumentException(StringResource.UndefinedEnum(enumValue));
    
    [DoesNotReturn]
    public static T ThrowUndefinedEnumException<T>(Enum enumValue, T? value = default)
        => throw new InvalidEnumArgumentException(StringResource.UndefinedEnum(enumValue));

    public static void ThrowIfUndefined<T>(T enumValue) where T : Enum
    {
        if(!Enum.IsDefined(typeof(T), enumValue))
            throw new InvalidEnumArgumentException(StringResource.UndefinedEnum(enumValue));
    }
    
    public static void ThrowIfContainsNull(IEnumerable? obj, [CallerArgumentExpression("obj")] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(obj, paramName);
        foreach(var item in obj)
            ArgumentNullException.ThrowIfNull(item, paramName);
    }

    public static void ThrowIfEmpty(IEnumerable? obj, [CallerArgumentExpression("obj")] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(obj, paramName);
        foreach (var _ in obj)
            return;

        throw new ArgumentException(StringResource.EmptyCollection(paramName), paramName);
    }
}