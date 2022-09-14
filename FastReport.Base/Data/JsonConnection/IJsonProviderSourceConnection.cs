namespace FastReport.Data.JsonConnection
{
    /// <summary>
    /// Provider for getting a json object fron connection source
    /// </summary>
    public interface IJsonProviderSourceConnection
    {
        /// <summary>
        /// Returns JsonBase object from connection source specific by tableDataSource
        /// </summary>
        /// <param name="tableDataSource"></param>
        /// <returns></returns>
        JsonBase GetJson(TableDataSource tableDataSource);
    }
}