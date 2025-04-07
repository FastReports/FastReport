using System.Collections.Generic;
using System.Data.Common;
using FastReport.Utils;

namespace FastReport.Data
{
    partial class DataConnectionBase
    {
        #region Private Methods

        private void FilterTables(List<string> tableNames)
        {
            // filter tables
            for (int i = 0; i < tableNames.Count; i++)
            {
                Config.FilterConnectionTablesEventArgs e = new Config.FilterConnectionTablesEventArgs(this, tableNames[i]);
                Config.OnFilterConnectionTables(this, e);
                if (e.Skip)
                {
                    tableNames.RemoveAt(i);
                    i--;
                }
            }
        }

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