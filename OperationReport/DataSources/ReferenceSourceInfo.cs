using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record ReferenceSourceInfo : DataSourceInfo
{
    public Table Table { get; }
    public Column IdColumn { get; }
    public Column LabelColumn { get; }
    
    public ReferenceSourceInfo(
        string FilePath,
        FileType Type,
        Table Table,
        Column IdColumn,
        Column LabelColumn) : base(FilePath, Type)
    {
        if(Table.Columns.All(column => column.Name != IdColumn.Name))
            throw new ArgumentException("ID column not found in table");
        
        if(Table.Columns.All(column => column.Name != LabelColumn.Name))
            throw new ArgumentException("Label column not found in table");
        
        this.Table = Table;
        this.IdColumn = IdColumn;
        this.LabelColumn = LabelColumn;
    }
}