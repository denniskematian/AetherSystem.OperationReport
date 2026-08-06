using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record SampleDataSourceInfo : DataSourceInfo
{
    public Table Table { get; init; }
    public DateTimeColumn TimestampColumn { get; init; }
    public Column? BatchNumberColumn { get; init; }
    public IReadOnlyList<Column> SampleColumns { get; init; }
    
    public SampleDataSourceInfo(
        string FilePath, 
        FileType Type,
        Table Table,
        DateTimeColumn TimestampColumn,
        Column? BatchNumberColumn,
        IReadOnlyList<Column> SampleColumns) : base(FilePath, Type)
    {
        if(Table.Columns.All(column => column.Name != TimestampColumn.Name))
            throw new ArgumentException($"Timestamp column '{TimestampColumn.Name}' not found in table");

        if (BatchNumberColumn is not null)
        {
            if(Table.Columns.All(column => column.Name != BatchNumberColumn.Name))
                throw new ArgumentException($"Batch number '{BatchNumberColumn.Name}' column not found in table");
            
            if(BatchNumberColumn.Type is not ColumnType.Integer)
                throw new ArgumentException($"Batch number column '{BatchNumberColumn.Name}' must be of type integer");
        }

        foreach (var sampleColumn in SampleColumns)
        {
            if(Table.Columns.All(column => column.Name != sampleColumn.Name))
                throw new ArgumentException($"Sample column '{sampleColumn.Name}' not found in table");
        }
        
        this.Table = Table;
        this.TimestampColumn = TimestampColumn;
        this.BatchNumberColumn = BatchNumberColumn;
        this.SampleColumns = SampleColumns;
    }
}