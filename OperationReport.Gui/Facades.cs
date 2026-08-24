using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Gui.Services;
using AetherSystem.OperationReport.Memento;
using AetherSystem.OperationReport.Packers;

namespace AetherSystem.OperationReport.Gui;

public static class Facades
{
    public static IDataSourceAdapterFactory DataSourceFactory => 
        LazyInitializer.EnsureInitialized(ref field, () => new DataSourceAdapterFactory());
    
    public static IDialogService DialogService => 
        LazyInitializer.EnsureInitialized(ref field, () => new DialogService());
    
    public static PackableRegistry CreateMementoRegistry() => new(PackerProvider);
    
    private static IPackerProvider PackerProvider => 
        LazyInitializer.EnsureInitialized(ref field, BuildPackerProvider);

    private static IPackerProvider BuildPackerProvider()
    {
        return new PackableRegistryBuilder()
            .Add<PresetConfigPacker>()
            .Add<ColumnPacker>()
            .Add<TimestampColumnPacker>()
            .Add<TablePacker>()
            .Add<DataSourceInfoPacker>()
            .Add<SampleReferenceConfigPacker>()
            .Add<SampleSourceInfoPacker>()
            .Add<OperationSourceInfoPacker>()
            .Add<AxisConfigPacker>()
            .Add<ChartConfigPacker>()
            .Add<ColorInfoPacker>()
            .Add<LinePatternPacker>()
            .Add<MarkerConfigPacker>()
            .Add<SeriesConfigPacker>()
            .Add<SampleFilterQueryPacker>()
            .Add<OperationSamplePacker>()
            .Add<OperationLogLabelPacker>()
            .Add<ProgramParameterPacker>()
            .Add<ProgramMessagePacker>()
            .Add<SignaturePacker>()
            .Add<ProgramStepsPacker>()
            .Add<ProgramSectionPacker>()
            .Add<BatchDocumentPacker>()
            .Build();
    }
}
