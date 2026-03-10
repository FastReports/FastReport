using FastReport.Utils.Json;

namespace FastReport.Data.JsonConnection
{
    /// <summary>
    /// Provider for getting a json object fron connection source
    /// </summary>
    public interface IJsonProviderSourceConnection
    {
        /// <summary>
        /// Returns JsonBase object from connection source by specified tableDataSource
        /// </summary>
        /// <param name="tableDataSource"></param>
        /// <returns></returns>
        JsonBase GetJson(TableDataSource tableDataSource);

        /// <summary>
        /// Returns JsonBase object (schema part) from connection source by specified tableDataSource
        /// </summary>
        /// <param name="tableDataSource"></param>
        /// <returns></returns>
        JsonSchema GetJsonSchema(TableDataSource tableDataSource);


        /// <summary>
        /// Return information about simple sturcture by specified tableDataSource
        /// </summary>
        /// <param name="tableDataSource"></param>
        /// <returns></returns>
        bool IsJsonSimpleStructure(TableDataSource tableDataSource);
    }
}