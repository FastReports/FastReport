namespace FastReport.Data
{
    /// <summary>
    /// Datasource for stored procedure.
    /// </summary>
    partial class CsvDataConnection : DataConnectionBase
    {
        /// <summary>
        /// Does nothing
        /// </summary>
        private string CheckForChangeConnection(CsvConnectionStringBuilder builder)
        {
            ConnectionString = builder.ToString();
            return ConnectionString;
        }
    }
}