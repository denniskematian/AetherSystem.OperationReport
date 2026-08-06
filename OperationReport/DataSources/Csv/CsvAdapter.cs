using System.Globalization;
using CsvHelper;

namespace AetherSystem.OperationReport.DataSources.Csv;

public abstract class CsvAdapter(string filePath)
{
    protected async Task<CsvReader> CreateCsvReader()
    {
        var reader = new StreamReader(filePath);
        var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture, leaveOpen: false);
        await CheckHeaderAsync(csvReader);
        
        return csvReader;
    }

    private static async Task CheckHeaderAsync(CsvReader reader)
    {
        if (!await reader.ReadAsync() || reader.Parser.Record is null)
            throw new InvalidOperationException("Unable to read header of CSV file.");
    }
}