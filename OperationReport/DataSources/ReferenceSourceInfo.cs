using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources;

public sealed record ReferenceSourceInfo : DataSourceInfo
{
    public Table Table { get; }
    public Column IdColumn { get; }
    public Column LabelColumn { get; }
    
    public ReferenceSourceInfo(
        string filePath,
        FileType type,
        Table table,
        Column idColumn,
        Column labelColumn) : base(filePath, type)
    {
        if(table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, idColumn)))
            throw new ArgumentException("ID column not found in table.");
        
        if(table.Columns.All(column => !ColumnComparer.NameAndType.Equals(column, labelColumn)))
            throw new ArgumentException("Label column not found in table.");
        
        Table = table;
        IdColumn = idColumn;
        LabelColumn = labelColumn;
    }

    public int IdColumnIndex => Table.IndexOf(IdColumn);
    public int LabelColumnIndex => Table.IndexOf(LabelColumn);
}