using System.Collections.Generic;
using System.Data.Common;

namespace FastReport.Data
{
    partial class DataConnectionBase
    {
        #region Private Methods

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="tableNames"></param>
        partial void FilterTables(List<string> tableNames);

        /// <summary>
        /// Does nothing
        /// </summary>
        private DbConnection GetDefaultConnection()
        {
            return null;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="connection"></param>
        /// <returns>false</returns>
        private bool ShouldNotDispose(DbConnection connection)
        {
            return false;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        partial void ShowLoginForm(string lastConnectionString);

        #endregion Private Methods
    }
}