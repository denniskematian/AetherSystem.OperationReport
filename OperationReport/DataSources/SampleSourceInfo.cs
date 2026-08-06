using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record SampleSourceInfo : DataSourceInfo
{
    public Table Table { get; }
    public DateTimeColumn TimestampColumn { get; }
    public Column? BatchNumberColumn { get; }
    public IReadOnlyList<Column> SampleColumns { get; }
    
    public bool HasBatchNumberColumn => BatchNumberColumn is not null;
    
    public SampleSourceInfo(
        string FilePath, 
        FileType Type,
        Table Table,
        DateTimeColumn TimestampColumn,
        Column? BatchNumberColumn,
        IReadOnlyList<Column> SampleColumns) : base(FilePath, Type)
    {
        if(Table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, TimestampColumn)))
            throw new ArgumentException($"Timestamp column '{TimestampColumn.Name}' not found in table");

        if (BatchNumberColumn is not null)
        {
            if(Table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, BatchNumberColumn)))
                throw new ArgumentException($"Batch number '{BatchNumberColumn.Name}' column not found in table");
            
            if(BatchNumberColumn.Type is not ColumnType.Integer)
                throw new ArgumentException($"Batch number column '{BatchNumberColumn.Name}' must be of type integer");
        }

        foreach (var sampleColumn in SampleColumns)
        {
            if(Table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, sampleColumn)))
                throw new ArgumentException($"Sample column '{sampleColumn.Name}' not found in table");
        }
        
        this.Table = Table;
        this.TimestampColumn = TimestampColumn;
        this.BatchNumberColumn = BatchNumberColumn;
        this.SampleColumns = SampleColumns;
    }
    
    public int TimestampColumnIndex => Table.IndexOf(TimestampColumn);
    public int? BatchNumberColumnIndex => BatchNumberColumn is null ? null : Table.IndexOf(BatchNumberColumn);
    public IReadOnlyList<int> GetSampleColumnIndices()
    {
        return [.. SampleColumns.Select(c => Table.IndexOf(c))];
    }
}