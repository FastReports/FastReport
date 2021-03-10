using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Adapters;
using ClickHouse.Client.ADO.Parameters;
using ClickHouse.Client.ADO.Readers;
using ClickHouse.Client.Types;
using ClickHouse.Client.Utility;
using FastReport.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.ClickHouse
{
    public partial class ClickHouseDataConnection : DataConnectionBase
    {
        private void GetDBObjectNames(string name, List<string> list)
        {
            DataTable schema = null;
            DbConnection connection = GetConnection();
            try
            {
                OpenConnection(connection);
                schema = connection.GetSchema(name, new string[] { connection.Database });
            }
            finally
            {
                DisposeConnection(connection);
            }
            foreach (DataRow row in schema.Rows)
            {
                list.Add(row["name"].ToString());
            }
        }
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return "\"" + value + "\"";
        }
        public override Type GetConnectionType()
        {
            return typeof(ClickHouseConnection);
        }
        public override string[] GetTableNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("Tables", list);
            return list.ToArray();
        }

        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection, CommandParameterCollection parameters)
        {
            ClickHouseDataAdapter clickHouseDataAdapter = new ClickHouseDataAdapter();
            var command = connection.CreateCommand() as ClickHouseCommand;

            foreach (CommandParameter p in parameters)
            {
                selectCommand = selectCommand.Replace($"@{p.Name}", $"{{{p.Name}:{(ClickHouseTypeCode)p.DataType}}}");
                command.AddParameter(p.Name, ((ClickHouseTypeCode)p.DataType).ToString(), p.Value);
            }
            command.CommandText = selectCommand;
            clickHouseDataAdapter.SelectCommand = command;
            return clickHouseDataAdapter;
        }
        private string PrepareSelectCommand(string selectCommand, string tableName, DbConnection connection)
        {
            if (String.IsNullOrEmpty(selectCommand))
            {
                selectCommand = "select * from " + QuoteIdentifier(tableName, connection);
            }
            return selectCommand;
        }
        private IEnumerable<DataColumn> GetColumns(ClickHouseDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnType = reader.GetFieldType(i);

                yield return new DataColumn(reader.GetName(i), columnType);
            }
        }

        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            ClickHouseConnection clickHouseConnection = GetConnection() as ClickHouseConnection;

            try
            {
                OpenConnection(clickHouseConnection);

                selectCommand = PrepareSelectCommand(selectCommand, table.TableName, clickHouseConnection);
                /*To reduce size of traffic and size of answer from ClickHouse server.
                  Because FillSchema doesn't work in this ADO.NET library.
                  LIMIT 0 gets an empy set, but we still have list of desired columns
                  Prorably can be a better way.
                 */
                selectCommand += " LIMIT 0"; 
                ClickHouseCommand clickHouseCommand = clickHouseConnection.CreateCommand();

                foreach (CommandParameter p in parameters)
                {
                    selectCommand = selectCommand.Replace($"@{p.Name}", $"{{{p.Name}:{(ClickHouseTypeCode)p.DataType}}}");
                    clickHouseCommand.AddParameter(p.Name, ((ClickHouseTypeCode)p.DataType).ToString(), p.Value);
                }
                clickHouseCommand.CommandText = selectCommand;
                using (ClickHouseDataReader reader = clickHouseCommand.ExecuteReader() as ClickHouseDataReader)
                {
                    var clms = GetColumns(reader);
                    table.Columns.AddRange(clms.ToArray());
                }
            }
            finally
            {
                DisposeConnection(clickHouseConnection);
            }
        }
    }
}
