using System.Diagnostics;

namespace AetherSystem.OperationReport.DataSources;

public class DataSourceAdapterFactory : IDataSourceAdapterFactory
{
    public IDataSourceAdapter CreateDataSourceAdapter(DataSourceInfo info)
    {
        return info.FileType switch
        {
            FileType.Csv => new Csv.DataSourceAdapter(info),
            FileType.Sqlite => new Sqlite.DataSourceAdapter(info),
            _ => throw new UnreachableException(),
        };
    }

    public ISampleTableAdapter CreateSampleTableAdapter(SampleSourceInfo info)
    {
        return info.FileType switch
        {
            FileType.Csv => new Csv.SampleTableAdapter(info),
            FileType.Sqlite => new Sqlite.SampleTableAdapter(info),
            _ => throw new UnreachableException(),
        };
    }

    public IOperationTableAdapter CreateOperationTableAdapter(OperationSourceInfo info)
    {
        return info.FileType switch
        {
            FileType.Csv => new Csv.OperationTableAdapter(info),
            FileType.Sqlite => new Sqlite.OperationTableAdapter(info),
            _ => throw new UnreachableException(),
        };
    }

    public IReferenceTableAdapter CreateReferenceTableAdapter(ReferenceSourceInfo info)
    {
        return info.FileType switch
        {
            FileType.Csv => new Csv.ReferenceTableAdapter(info),
            FileType.Sqlite => new Sqlite.ReferenceTableAdapter(info),
            _ => throw new UnreachableException(),
        };
    }
}