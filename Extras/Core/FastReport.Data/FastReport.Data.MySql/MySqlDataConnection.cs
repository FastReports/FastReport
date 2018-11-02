using System;
using System.Collections.Generic;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Data;

namespace FastReport.Data
{
    public partial class MySqlDataConnection : DataConnectionBase
    {
        private void GetDBObjectNames(string name, List<string> list)
        {
            DataTable schema = null;
            string databaseName = "";
            DbConnection connection = GetConnection();
            try
            {
                OpenConnection(connection);
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(ConnectionString);
                schema = connection.GetSchema(name);
                databaseName = builder.Database;
            }
            finally
            {
                DisposeConnection(connection);
            }
            foreach (DataRow row in schema.Rows)
            {
                if (String.IsNullOrEmpty(databaseName) || String.Compare(row["TABLE_SCHEMA"].ToString(), databaseName) == 0)
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
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(ConnectionString);

            builder.UserID = userName;
            builder.Password = password;

            return builder.ToString();
        }

        public override Type GetConnectionType()
        {
            return typeof(MySqlConnection);
        }

        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
          CommandParameterCollection parameters)
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter(selectCommand, connection as MySqlConnection);
            foreach (CommandParameter p in parameters)
            {
                MySqlParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (MySqlDbType)p.DataType, p.Size);
                parameter.Value = p.Value;
            }
            return adapter;
        }
    }
}
