namespace AetherSystem.OperationReport.DataSources.Schema;

public record Table(string Name, IReadOnlyList<Column> Columns);