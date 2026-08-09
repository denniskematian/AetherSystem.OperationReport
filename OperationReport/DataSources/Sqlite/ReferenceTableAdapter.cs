using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.Entities;
using SqlKata;

namespace AetherSystem.OperationReport.DataSources.Sqlite;

public sealed class ReferenceTableAdapter(ReferenceSourceInfo sourceInfo)
    : SqliteAdapter(sourceInfo.FilePath), IReferenceTableAdapter
{
    public async IAsyncEnumerable<SampleReference> EnumerateAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = new Query(sourceInfo.Table.Name)
            .Select(sourceInfo.IdColumn.Name, sourceInfo.LabelColumn.Name);

        await using var command = CreateExecutableCommand(query);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var label = reader.GetString(1);
            yield return new SampleReference(id, label);
        }
    }
}