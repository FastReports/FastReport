using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data.Greenplum
{
    public partial class GreenplumDataConnection : PostgresDataConnection
    {
        /// <summary> 
        /// Determines whether the specified table belongs to the Greenplum 
        /// internal user-related objects that should not be displayed in the report designer. 
        /// </summary>
        private static bool IsUserTable(string tableName)
        {
            return tableName.StartsWith("gp_toolkit.", StringComparison.OrdinalIgnoreCase) &&
                   tableName.IndexOf("__gp_user", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override string GetConnectionId()
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(ConnectionString);
            string info = "";

            try
            {
                info = builder.Database;
            }
            catch
            {
            }

            return "Greenplum: " + info;
        }

        /// <summary> 
        /// Returns available table names excluding Greenplum internal user-related 
        /// objects from the gp_toolkit schema. 
        /// </summary>
        public override string[] GetTableNames()
        {
            string[] tables = base.GetTableNames();

            List<string> result = new List<string>();

            foreach (string table in tables)
            {
                if (!IsUserTable(table))
                    result.Add(table);
            }

            return result.ToArray();
        }

        /// <summary> 
        /// Asynchronously returns available table names excluding Greenplum internal 
        /// user-related objects from the gp_toolkit schema. 
        /// </summary>
        public override async Task<string[]> GetTableNamesAsync(CancellationToken cancellationToken = default)
        {
            string[] tables = await base.GetTableNamesAsync(cancellationToken);

            List<string> result = new List<string>();

            foreach (string table in tables)
            {
                if (!IsUserTable(table))
                    result.Add(table);
            }

            return result.ToArray();
        }
    }
}
