using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record SampleSourceInfo : DataSourceInfo
{
    public Table Table { get; }
    public TimestampColumn TimestampColumn { get; }
    public Column? BatchNumberColumn { get; }
    public IReadOnlyList<Column> SampleColumns { get; }
    
    public bool HasBatchNumberColumn => BatchNumberColumn is not null;
    
    public SampleSourceInfo(
        string filePath, 
        FileType type,
        Table table,
        TimestampColumn timestampColumn,
        Column? batchNumberColumn,
        IReadOnlyList<Column> sampleColumns) : base(filePath, type)
    {
        if(table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, timestampColumn)))
            throw new ArgumentException($"Timestamp column '{timestampColumn.Name}' not found in table");

        if (batchNumberColumn is not null)
        {
            if(table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, batchNumberColumn)))
                throw new ArgumentException($"Batch number '{batchNumberColumn.Name}' column not found in table");
            
            if(batchNumberColumn.Type is not ColumnType.Integer)
                throw new ArgumentException($"Batch number column '{batchNumberColumn.Name}' must be type of integer");
        }

        foreach (var sampleColumn in sampleColumns)
        {
            var underlyingColumn = table.Columns.FirstOrDefault(column => ColumnComparer.NameAndType.Equals(column, sampleColumn));
            if(underlyingColumn is null)
                throw new ArgumentException($"Sample column '{sampleColumn.Name}' not found in table");
            
            if(underlyingColumn.Type is not (ColumnType.Real or ColumnType.Integer))
                throw new ArgumentException($"Underlying sample column '{sampleColumn.Name}' must be type of real or integer");
        }
        
        Table = table;
        TimestampColumn = timestampColumn;
        BatchNumberColumn = batchNumberColumn;
        SampleColumns = sampleColumns;
    }
    
    public int TimestampColumnIndex => Table.IndexOf(TimestampColumn);
    public int? BatchNumberColumnIndex => BatchNumberColumn is null ? null : Table.IndexOf(BatchNumberColumn);
    public IReadOnlyList<int> GetSampleColumnIndices()
    {
        return [.. SampleColumns.Select(c => Table.IndexOf(c))];
    }
}