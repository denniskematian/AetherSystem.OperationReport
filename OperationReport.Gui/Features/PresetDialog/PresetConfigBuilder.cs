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
    
    public PresetConfigBuilder WithSampleDataSource(
        IReadOnlyList<SampleReferenceConfig> references,
        Table table,
        Column timestampColumn,
        TimestampResolution resolution,
        Column? batchNumberColumn)
    {
        var sampleColumns = references
            .Where(i => i.IsIncluded)
            .Select(i => new Column(i.Column, ColumnType.Real))
            .ToArray();

        _sampleDataSource = new SampleSourceInfo(
            context.SampleDataSource.FilePath,
            context.SampleDataSource.FileType,
            table,
            CreateTimestampColumn(timestampColumn, resolution),
            batchNumberColumn,
            sampleColumns);

        _sampleReferences = references;
        
        return this;
    }
    
    public PresetConfigBuilder WithOperationDataSource(
        Table table,
        Column timestampColumn,
        TimestampResolution resolution,
        Column commentColumn)
    {
        _operationDataSource = new OperationSourceInfo(
            context.OperationDataSource.FilePath,
            context.OperationDataSource.FileType,
            table, 
            CreateTimestampColumn(timestampColumn, resolution), 
            commentColumn);

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

    private TimestampColumn CreateTimestampColumn(Column timestampColumn, TimestampResolution resolution)
    {
        ITimestampFormat timestampFormat = timestampColumn.Type switch
        {
            ColumnType.Integer => new UnixTimestampFormat(resolution),
            ColumnType.Real => new FractionalUnixTimestampFormat(resolution),
            ColumnType.Text => new StringTimestampFormat(),
            _ => throw new InvalidOperationException("Unsupported timestamp column type")
        };

        return new TimestampColumn(
            timestampColumn.Name, 
            timestampColumn.Type,
            timestampFormat);
    }
}