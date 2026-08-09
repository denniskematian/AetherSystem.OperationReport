using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.Gui.Services;
using AetherSystem.OperationReport.Memento;

namespace AetherSystem.OperationReport.Gui;

public static class Facades
{
    public static IDataSourceAdapterFactory DataSourceFactory => 
        LazyInitializer.EnsureInitialized(ref field, () => new DataSourceAdapterFactory());
    
    public static IDialogService DialogService => 
        LazyInitializer.EnsureInitialized(ref field, () => new DialogService());
    
    public static PackableRegistry MementoRegistry => 
        LazyInitializer.EnsureInitialized(ref field, CreateMementoRegistry);

    private static PackableRegistry CreateMementoRegistry()
    {
        var builder = new PackableRegistryBuilder();
        
        return builder.Build();
    }
}