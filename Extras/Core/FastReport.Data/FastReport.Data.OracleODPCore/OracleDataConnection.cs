using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace FastReport.Data
{
    public class OracleDataConnection : DataConnectionBase
    {
        private void GetDBObjectNames(string name, string columnName, List<string> list)
        {
            DataTable schema = null;
            DbConnection connection = GetConnection();
            try
            {
                OpenConnection(connection);
                OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(connection.ConnectionString);
                schema = connection.GetSchema(name, new string[] { builder.UserID.ToUpper(), null });
            }
            finally
            {
                DisposeConnection(connection);
            }

            foreach (DataRow row in schema.Rows)
            {
                string tableName = row[columnName].ToString();
                string schemaName = row["OWNER"].ToString();
                if (String.Compare(schemaName, "SYSTEM") == 0)
                    list.Add(tableName);
                else
                    list.Add(schemaName + ".\"" + tableName + "\"");
            }
        }

        public override string[] GetTableNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("Tables", "TABLE_NAME", list);
            GetDBObjectNames("Views", "VIEW_NAME", list);
            return list.ToArray();
        }

        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            // already quoted? keep in mind GetDBObjectNames and non-SYSTEM schema!
            if (!value.EndsWith("\""))
                value = "\"" + value + "\"";
            return value;
        }

        protected override string GetConnectionStringWithLoginInfo(string userName, string password)
        {
            OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(ConnectionString);

            builder.UserID = userName;
            builder.Password = password;

            return builder.ToString();
        }

        public override Type GetConnectionType()
        {
            return typeof(OracleConnection);
        }

        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
          CommandParameterCollection parameters)
        {
            OracleDataAdapter adapter = new OracleDataAdapter(selectCommand, connection as OracleConnection);
            adapter.SelectCommand.BindByName = true;

            foreach (CommandParameter p in parameters)
            {
                OracleDbType parType = (OracleDbType)p.DataType;
                OracleParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, parType, p.Size);
                parameter.Value = p.Value;

                // if we have refcursor parameter, set its direction to output, and also
                // modify the command type to CommandType.StoredProcedure. The selectCommand must contain
                // the stored proc name only.
                if (parType == OracleDbType.RefCursor)
                {
                    parameter.Direction = ParameterDirection.Output;
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                }
            }

            return adapter;
        }

        public override Type GetParameterType()
        {
            return typeof(OracleDbType);
        }

#if !FRCORE
        public override int GetDefaultParameterType()
        {
            return (int)OracleDbType.Varchar2;
        }

        public override string GetConnectionId()
        {
            OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(ConnectionString);
            string info = "";
            try
            {
                info = builder.DataSource;
            }
            catch
            {

            }
            return "Oracle: " + info;
        }


        public override ConnectionEditors.ConnectionEditorBase GetEditor()
        {
            return new OracleConnectionEditor();
        }
#endif
    }
}