using AetherSystem.OperationReport.DataSources.Schema;
using Microsoft.Data.Sqlite;
using SqlKata;
using SqlKata.Compilers;

namespace AetherSystem.OperationReport.DataSources.Sqlite;

public abstract class SqliteAdapter : IAsyncDisposable
{
    private static readonly SqliteCompiler s_compiler = new();

    private SqliteConnection? _connection;
    private readonly string _filePath;

    protected SqliteAdapter(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found", filePath);

        _filePath = filePath;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        var connection = Interlocked.Exchange(ref _connection, null);
        return connection?.DisposeAsync() ?? ValueTask.CompletedTask;
    }

    protected SqliteCommand CreateExecutableCommand(Query query)
    {
        var command = CreateExecutableCommand();
        var compiled = Compile(query);
        command.CommandText = compiled.Sql;
        for(int i = 0; i < compiled.Bindings.Count; i++)
        {
            command.Parameters.AddWithValue($"@p{i}", compiled.Bindings[i]);
        }

        return command;
    }

    private static SqlResult Compile(Query query)
    {
        return s_compiler.Compile(query);
    }

    protected SqliteCommand CreateExecutableCommand()
    {
        return LazyInitializer
            .EnsureInitialized(ref _connection, OpenConnection)
            .CreateCommand();
    }

    protected static string QuoteIdentifier(string identifier)
    {
        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }
    
    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection($"Data Source={_filePath}");
        connection.Open();
        return connection;
    }

    protected Query CreateFilterQuery(Query query, DateTimeColumn timestampColumn, FilterQuery filterQuery)
    {
        switch (timestampColumn.Type)
        {
            case ColumnType.Integer or ColumnType.Real:
            {
                var resolver = new TimestampResolver(timestampColumn);
                if (filterQuery.From.HasValue)
                {
                    var from = resolver.ToUnixTimestamp(filterQuery.From.Value);
                    query = query.Where(timestampColumn.Name, ">=", from);
                }
                
                if (filterQuery.To.HasValue)
                {
                    var to = resolver.ToUnixTimestamp(filterQuery.To.Value);
                    query = query.Where(timestampColumn.Name, "<=", to);
                }
                break;
            }

            case ColumnType.Text:
                if (filterQuery.From.HasValue)
                {
                    query = query.Where(timestampColumn.Name, ">=", filterQuery.From.Value);
                }
                
                if (filterQuery.To.HasValue)
                {
                    query = query.Where(timestampColumn.Name, "<=", filterQuery.To);
                }
                break;

            default:
                throw new NotSupportedException("Unsupported column type.");
        }

        return query;
    }
}