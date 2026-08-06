using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources.Csv;

public class ReferenceTableAdapter(ReferenceSourceInfo info) : CsvAdapter(info.FilePath), IReferenceTableAdapter
{
    private readonly int _idColumnIndex = info.IdColumnIndex;
    private readonly int _labelColumnIndex = info.LabelColumnIndex;

    public async IAsyncEnumerable<SampleReference> EnumerateAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var csvReader = await CreateCsvReader();
        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;
            
            var id = Convert.ToInt32(row[_idColumnIndex]);
            var label = row[_labelColumnIndex];
            yield return new SampleReference(id, label);
        }
    }
}