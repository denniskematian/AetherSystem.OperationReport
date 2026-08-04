namespace AetherSystem.OperationReport.DataSources;

public class DataSourceAdapterFactory : IDataSourceAdapterFactory
{
    public IDataSourceAdapter CreateDataSourceAdapter(DataSourceInfo info)
    {
        return info.Type switch
        {
            FileType.Csv => new Csv.DataSourceAdapter(info),
            FileType.Sqlite => new Sqlite.DataSourceAdapter(info),
            _ => throw new NotSupportedException($"Data source type {info.Type} is not supported.")
        };
    }

    public ISampleTableAdapter CreateSampleTableAdapter(SampleDataSourceInfo info)
    {
        return info.Type switch
        {
            FileType.Csv => new Csv.SampleTableAdapter(info),
            FileType.Sqlite => new Sqlite.SampleTableAdapter(info),
            _ => throw new NotSupportedException($"Data source type {info.Type} is not supported.")
        };
    }

    public IOperationTableAdapter CreateOperationTableAdapter(OperationSourceInfo info)
    {
        return info.Type switch
        {
            FileType.Csv => new Csv.OperationTableAdapter(info),
            FileType.Sqlite => new Sqlite.OperationTableAdapter(info),
            _ => throw new NotSupportedException($"Data source type {info.Type} is not supported.")
        };
    }

    public IReferenceTableAdapter CreateReferenceTableAdapter(ReferenceSourceInfo info)
    {
        return info.Type switch
        {
            FileType.Csv => new Csv.ReferenceTableAdapter(info),
            FileType.Sqlite => new Sqlite.ReferenceTableAdapter(info),
            _ => throw new NotSupportedException($"Data source type {info.Type} is not supported.")
        };
    }
}