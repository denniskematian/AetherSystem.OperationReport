using AetherSystem.OperationReport.DataSources.Csv;
using AetherSystem.OperationReport.DataSources.Sqlite;

namespace AetherSystem.OperationReport.DataSources;

public interface IDataSourceAdapterFactory
{
    IDataSourceAdapter CreateDataSourceAdapter(DataSourceInfo info);
    ISampleTableAdapter CreateSampleTableAdapter(SampleDataSourceInfo info);
    IOperationTableAdapter CreateOperationTableAdapter(OperationSourceInfo info);
    IReferenceTableAdapter CreateReferenceTableAdapter(ReferenceSourceInfo info);
}