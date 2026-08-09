using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.DataSources.Schema;
using AetherSystem.OperationReport.Timestamps;
using AetherSystem.OperationReport.ValueObjects;

namespace AetherSystem.OperationReport.Gui.Features.PresetDialog;

public sealed class PresetConfigBuilder(PresetDialogContext context)
{
    private SampleSourceInfo? _sampleDataSource;
    private OperationSourceInfo? _operationDataSource;
    private IReadOnlyList<SampleReferenceConfig>? _sampleReferences;
    
    public PresetConfigBuilder WithSampleDataSource(SampleSourceInfo dataSource)
    {
        _sampleDataSource = dataSource;
        return this;
    }
    
    public PresetConfigBuilder WithOperationDataSource(
        Table table,
        Column timestampColumn,
        TimestampResolution resolution,
        Column commentColumn)
    {
        ITimestampFormat timestampFormat = timestampColumn.Type switch
        {
            ColumnType.Integer => new UnixTimestampFormat(resolution),
            ColumnType.Real => new FractionalUnixTimestampFormat(resolution),
            ColumnType.Text => new StringTimestampFormat(),
            _ => throw new InvalidOperationException("Unsupported timestamp column type")
        };
            
        var tsColumn = new TimestampColumn(
            timestampColumn.Name, 
            timestampColumn.Type,
            timestampFormat);
        
        _operationDataSource = new OperationSourceInfo(
            context.OperationDataSource.FilePath,
            context.OperationDataSource.FileType,
            table, tsColumn, commentColumn);

        return this;
    }
    
    public PresetConfigBuilder WithSampleReferences(IReadOnlyList<SampleReferenceConfig> references)
    {
        _sampleReferences = references;
        return this;
    }
    
    public PresetConfig Build()
    {
        if(_sampleDataSource == null)
            throw new InvalidOperationException("SampleDataSource is not set");
        
        if(_operationDataSource == null)
            throw new InvalidOperationException("OperationDataSource is not set");
        
        if(_sampleReferences == null)
            throw new InvalidOperationException("SampleReferences is not set");

        return new PresetConfig
        {
            OperationDataSource = _operationDataSource,
            SampleDataSource = _sampleDataSource,
            SampleReferences = _sampleReferences
        };
    }
}