using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Schema;
using SqlKata;
using Column = AetherSystem.OperationReport.DataSources.Schema.Column;

namespace AetherSystem.OperationReport.DataSources.Sqlite;

public sealed class DataSourceAdapter(DataSourceInfo info) : SqliteAdapter(info.FilePath), IDataSourceAdapter
{
    public async IAsyncEnumerable<Table> GetTablesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = new Query("sqlite_master")
            .Select("name")
            .Where("type", "table")
            .WhereNotStarts("name", "sqlite_");

        await using var command = CreateExecutableCommand(query);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var ordinal = reader.GetOrdinal("name");
            var name = reader.GetString(ordinal);
            var columns = await GetColumnsAsync(name, cancellationToken)
                .ToArrayAsync(cancellationToken);

            yield return new Table(name, columns.AsReadOnly());
        }
    }

    private async IAsyncEnumerable<Column> GetColumnsAsync(
        string table,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var command = CreateExecutableCommand();
        command.CommandText = $"PRAGMA table_info({QuoteIdentifier(table)})";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var name = reader.GetString(reader.GetOrdinal("name"));
            var type = reader.GetString(reader.GetOrdinal("type"));
            yield return new Column(name, ParseColumnType(type));
        }
    }

    private static ColumnType ParseColumnType(string type)
    {
        return type.ToUpper() switch
        {
            "INTEGER" => ColumnType.Integer,
            "TEXT" => ColumnType.Text,
            "REAL" => ColumnType.Real,
            _ => throw new ArgumentException($"Unknown column type: {type}", nameof(type))
        };
    }
}