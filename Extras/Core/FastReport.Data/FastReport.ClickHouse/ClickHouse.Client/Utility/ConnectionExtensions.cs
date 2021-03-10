using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using ClickHouse.Client.ADO.Adapters;

namespace ClickHouse.Client.Utility
{
    public static class ConnectionExtensions
    {
        public static async Task<int> ExecuteStatementAsync(this DbConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        public static async Task<object> ExecuteScalarAsync(this DbConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return await command.ExecuteScalarAsync().ConfigureAwait(false);
        }

        public static async Task<DbDataReader> ExecuteReaderAsync(this DbConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return await command.ExecuteReaderAsync().ConfigureAwait(false);
        }

        public static DataTable ExecuteDataTable(this DbConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            using var adapter = new ClickHouseDataAdapter();
            command.CommandText = sql;
            adapter.SelectCommand = command;
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }
    }
}
