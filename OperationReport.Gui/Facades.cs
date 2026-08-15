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
        var builder = new PackableRegistryBuilder();
        builder.Add(new PresetConfigPacker());
        builder.Add(new ColumnPacker());
        builder.Add(new TimestampColumnPacker());
        builder.Add(new TablePacker());
        builder.Add(new DataSourceInfoPacker());
        builder.Add(new SampleReferenceConfigPacker());
        builder.Add(new SampleSourceInfoPacker());
        builder.Add(new OperationSourceInfoPacker());
        
        return builder.Build();
    }
}