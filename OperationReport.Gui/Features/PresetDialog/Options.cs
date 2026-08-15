using AetherSystem.OperationReport.Gui.Options;
using AetherSystem.OperationReport.Timestamps;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public static class Options
{
    public static IReadOnlyList<EnumOption<TimestampResolution>> TimestampResolutions => [
        new(TimestampResolution.Second),
        new(TimestampResolution.Millisecond),
    ];

    public static IReadOnlyList<Option<T?>> WithNone<T>(IEnumerable<T> values, Func<T, string> nameFactory)
        where T : class
    {
        return Enumerable.Concat(
            [new Option<T?>("(None)", null)], 
            values.Select(i => new Option<T?>(nameFactory.Invoke(i), i))
        ).ToArray();
    }
}