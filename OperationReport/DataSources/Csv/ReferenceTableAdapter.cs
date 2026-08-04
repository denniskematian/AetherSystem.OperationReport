using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.DataSources.Csv;

public class ReferenceTableAdapter(ReferenceSourceInfo info) : CsvAdapter(info.FilePath), IReferenceTableAdapter
{
    private int _idColumnIndex = -1;
    private int _labelColumnIndex = -1;
    public async IAsyncEnumerable<SampleReference> EnumerateAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var csvReader = await CreateCsvReader();
        var idColumnIndex = GetIdColumnIndex();
        var labelColumnIndex = GetLabelColumnIndex();
        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;
            
            var id = Convert.ToInt32(row[idColumnIndex]);
            var label = row[labelColumnIndex];
            yield return new SampleReference(id, label);
        }
    }

    private int GetIdColumnIndex()
    {
        if (_idColumnIndex < 0)
        {
            _idColumnIndex = info.Table.Columns.Index().First(tc => tc.Item.Name == info.IdColumn.Name).Index;
        }
    
        return _idColumnIndex;
    }
    
    private int GetLabelColumnIndex()
    {
        if (_labelColumnIndex < 0)
        {
            _labelColumnIndex = info.Table.Columns.Index().First(tc => tc.Item.Name == info.LabelColumn.Name).Index;
        }
    
        return _labelColumnIndex;
    }
}