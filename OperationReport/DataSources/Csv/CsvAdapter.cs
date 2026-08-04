using System.Globalization;
using AetherSystem.OperationReport.DataSources.Schema;
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

    protected bool IsMatchFilter(FilterQuery filterQuery, DateTimeColumn column, DateTime timestamp)
    {
        if (filterQuery.From.HasValue)
        {
            var from = filterQuery.From.Value.Add(column.Offset).LocalDateTime;
            if(timestamp < from) return false;
        }
        
        if (filterQuery.To.HasValue)
        {
            var from = filterQuery.To.Value.Add(column.Offset).LocalDateTime;
            if(timestamp < from) return false;
        }
        
        return true;
    }
}