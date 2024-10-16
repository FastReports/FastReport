using FastReport.Data;

namespace FastReport
{
    internal interface IContainDataSource
    {
         void UpdateDataSourceRef(DataSourceBase newRefDatasource);
    }
}
