#undef FRCORE

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using FastReport.Data;
#if FRCORE
using System.Linq;
using Microsoft.Data.Sqlite;
#else
using System.Data.SQLite;
#endif

namespace FastReport.Data
{
    public partial class SQLiteDataConnection : DataConnectionBase
    {
        private void GetDBObjectNames(string name, List<string> list)
        {
            DataTable schema = null;
            DbConnection connection = GetConnection();
            try
            {
                OpenConnection(connection);
                schema = connection.GetSchema(name);
            }
            finally
            {
              DisposeConnection(connection);
            }
            foreach (DataRow row in schema.Rows)
            {
                list.Add(row["TABLE_NAME"].ToString());
            }
        }

        public override string[] GetTableNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("Tables", list);
            GetDBObjectNames("Views", list);
            return list.ToArray();
        }

        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return "`" + value + "`";
        }

        protected override string GetConnectionStringWithLoginInfo(string userName, string password)
        {
#if FRCORE
            var builder = new SqliteConnectionStringBuilder(ConnectionString);
#else
            var builder = new SQLiteConnectionStringBuilder(ConnectionString);
            builder.Password = password;
#endif

            return builder.ToString();
        }

        public override Type GetConnectionType()
        {
#if FRCORE
            return typeof(SqliteConnection);
#else
            return typeof(SQLiteConnection);
#endif
        }

#if !FRCORE
        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
            CommandParameterCollection parameters)
        {
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(selectCommand, connection as SQLiteConnection);
            foreach (CommandParameter p in parameters)
            {
                SQLiteParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (DbType)p.DataType, p.Size);
                parameter.Value = p.Value;
            }
            return adapter;
        }
#else

        public new SqliteConnection GetConnection() => new SqliteConnection(ConnectionString);

        private string PrepareSelectCommand(string selectCommand, string tableName, DbConnection connection)
        {
            if (String.IsNullOrEmpty(selectCommand))
            {
                selectCommand = "select * from " + QuoteIdentifier(tableName, connection);
            }
            return selectCommand;
        }

        private IEnumerable<DataColumn> GetColumns(SqliteDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                yield return new DataColumn(reader.GetName(i), typeof(String));
            }
        }

        public override void FillTableSchema(DataTable table, string selectCommand,
  CommandParameterCollection parameters)
        {
            SqliteConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);

                // prepare select command
                selectCommand = PrepareSelectCommand(selectCommand, table.TableName, conn);

                SqliteCommand command = conn.CreateCommand();
                command.CommandText = selectCommand;

                // read the table schema
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var clmns = GetColumns(reader);
                    table.Columns.AddRange(clmns.ToArray());
                }
            }
            finally
            {
                DisposeConnection(conn);
            }
        }

        public override void FillTableData(DataTable table, string selectCommand,
     CommandParameterCollection parameters)
        {
            SqliteConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);

                // prepare select command
                selectCommand = PrepareSelectCommand(selectCommand, table.TableName, conn);

                SqliteCommand command = conn.CreateCommand();
                command.CommandText = selectCommand;
                command.CommandTimeout = CommandTimeout;

                // read the table
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    table.Clear();
                    while (reader.Read())
                    {
                        var newRow = table.Rows.Add();
                        foreach (DataColumn col in table.Columns)
                        {
                            newRow[col.ColumnName] = reader[col.ColumnName].ToString();
                        }
                    }
                }
            }
            finally
            {
                DisposeConnection(conn);
            }
        }
#endif

        public override Type GetParameterType()
        {
            return typeof(DbType);
        }
    }
}
