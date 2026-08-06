namespace AetherSystem.OperationReport.DataSources;

public interface IDataSourceAdapterFactory
{
    IDataSourceAdapter CreateDataSourceAdapter(DataSourceInfo info);
    ISampleTableAdapter CreateSampleTableAdapter(SampleDataSourceInfo info);
    IOperationTableAdapter CreateOperationTableAdapter(OperationSourceInfo info);
    IReferenceTableAdapter CreateReferenceTableAdapter(ReferenceSourceInfo info);
}