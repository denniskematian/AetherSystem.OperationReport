using System.Runtime.CompilerServices;
using AetherSystem.OperationReport.DataSources.Schema;

namespace AetherSystem.OperationReport.DataSources.Csv;

public class DataSourceAdapter(DataSourceInfo info) : CsvAdapter(info.FilePath), IDataSourceAdapter
{
    public async IAsyncEnumerable<Table> GetTablesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var csvReader = await CreateCsvReader();

        if (!await csvReader.ReadAsync() || csvReader.Parser.Record is not { } header)
            throw new InvalidOperationException("Unable to read header of CSV file.");

        var columnTypes = new ColumnType[header.Length];
        Array.Fill(columnTypes, ColumnType.Integer);

        var isAllString = false;
        while (await csvReader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (csvReader.Parser.Record is not { } row)
                continue;

            if (isAllString) continue;

            var isAllFieldString = true;
            for (var i = 0; i < header.Length; i++)
                switch (columnTypes[i])
                {
                    case ColumnType.Integer when int.TryParse(row[i], out _):
                        isAllFieldString = false;
                        continue;
                    case ColumnType.Integer when double.TryParse(row[i], out _):
                        isAllFieldString = false;
                        columnTypes[i] = ColumnType.Real;
                        continue;
                    case ColumnType.Integer:
                        columnTypes[i] = ColumnType.Text;
                        break;
                    case ColumnType.Real when double.TryParse(row[i], out _):
                        isAllFieldString = false;
                        continue;
                    case ColumnType.Real:
                        columnTypes[i] = ColumnType.Text;
                        break;
                }

            isAllString = isAllFieldString;
        }

        var columns = new Column[header.Length];
        for (var i = 0; i < header.Length; i++) columns[i] = new Column(header[i], columnTypes[i]);

        var tableName = Path.GetFileNameWithoutExtension(info.FilePath);
        yield return new Table(tableName, columns.AsReadOnly());
    }
}