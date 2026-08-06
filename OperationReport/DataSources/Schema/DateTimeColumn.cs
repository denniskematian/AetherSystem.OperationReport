namespace AetherSystem.OperationReport.DataSources.Schema;

public record DateTimeColumn : Column
{
    public DateTimeResolution Resolution { get; }
    public TimeSpan Offset { get; }
    
    public DateTimeColumn(
        string Name,
        ColumnType Type,
        DateTimeResolution Resolution = DateTimeResolution.Unspecified,
        TimeSpan Offset = default) : base(Name, Type)
    {
        if(!Enum.IsDefined(Resolution))
            throw new ArgumentException($"DateTimeResolution ({(int)Resolution}) is not defined.", nameof(Resolution));
        
        if(Type is ColumnType.Real or ColumnType.Integer && Resolution is DateTimeResolution.Unspecified)
            throw new ArgumentException($"DateTimeResolution ({(int)Resolution}) must for {Type} column.", nameof(Resolution));

        this.Resolution = Resolution;
        this.Offset = Offset;
    }
}