using AetherSystem.OperationReport.Gui.Options;
using AetherSystem.OperationReport.Timestamps;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public static class Options
{
    public static IReadOnlyList<EnumOption<TimestampResolution>> TimestampResolutions => [
        new(TimestampResolution.Second),
        new(TimestampResolution.Millisecond),
    ];
}