using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to MS SQL database.
    /// </summary>
    /// <example>This example shows how to add a new connection to the report.
    /// <code>
    /// Report report1;
    /// MsSqlDataConnection conn = new MsSqlDataConnection();
    /// conn.ConnectionString = "your_connection_string";
    /// report1.Dictionary.Connections.Add(conn);
    /// conn.CreateAllTables();
    /// </code>
    /// </example>
    public partial class MsSqlDataConnection : DataConnectionBase
    {
        /// <inheritdoc/>
        protected override string GetConnectionStringWithLoginInfo(string userName, string password)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);

            builder.IntegratedSecurity = false;
            builder.UserID = userName;
            builder.Password = password;

            return builder.ToString();
        }

        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            // already quoted? keep in mind GetDBObjectNames and non-dbo schema!
            if (!value.EndsWith("\""))
                value = "\"" + value + "\"";
            return value;
        }

        /// <inheritdoc/>
        public override Type GetConnectionType()
        {
            return typeof(SqlConnection);
        }

        /// <inheritdoc/>
        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
          CommandParameterCollection parameters)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(selectCommand, connection as SqlConnection);
            foreach (CommandParameter p in parameters)
            {
                SqlParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (SqlDbType)p.DataType, p.Size);
                object value = p.Value;
                parameter.Direction = p.Direction;

                if ((SqlDbType)p.DataType == SqlDbType.UniqueIdentifier && (value is Variant || value is String))
                    value = new Guid(value.ToString());

                if (value is Variant v && v.Type == typeof(string))
                {
                    value = VariantToClrType(v, (SqlDbType)p.DataType);
                }

                parameter.Value = value ?? DBNull.Value;
            }
            return adapter;
        }

        /// <inheritdoc/>
        public override Type GetParameterType()
        {
            return typeof(SqlDbType);
        }

        private void GetDBObjectNames(string name, List<string> list)
        {
            DataTable schema = null;
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);
                schema = conn.GetSchema("Tables", new string[] { null, null, null, name });
            }
            finally
            {
                DisposeConnection(conn);
            }

            foreach (DataRow row in schema.Rows)
            {
                string tableName = row["TABLE_NAME"].ToString();
                string schemaName = row["TABLE_SCHEMA"].ToString();
                if (String.Compare(schemaName, "dbo") == 0)
                    list.Add(tableName);
                else
                    list.Add(schemaName + ".\"" + tableName + "\"");
            }
        }

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("BASE TABLE", list);
            GetDBObjectNames("VIEW", list);
            return list.ToArray();
        }

        private object VariantToClrType(Variant value, SqlDbType type)
        {
            if (value.ToString() == "")
                return null;

            switch (type)
            {
                case SqlDbType.BigInt:
                    {
                        long val = 0;
                        long.TryParse(value.ToString(), out val);
                        return val;
                    }

                case SqlDbType.Bit:
                    {
                        bool val = false;
                        bool.TryParse(value.ToString(), out val);
                        return val;
                    }
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    {
                        if (value.ToString() == "")
                            return null;
                        return value.ToString();
                    }

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    {
                        DateTime val = DateTime.Now;
                        DateTime.TryParse(value.ToString(), out val);
                        return val;
                    }

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    {
                        decimal val = 0;
                        decimal.TryParse(value.ToString(), out val);
                        return val;
                    }

                case SqlDbType.Real:
                case SqlDbType.Float:
                    {
                        float val = 0;
                        float.TryParse(value.ToString(), out val);
                        return val;
                    }

                case SqlDbType.Int:
                    {
                        int val = 0;
                        int.TryParse(value.ToString(), out val);
                        return val;
                    }

                case SqlDbType.UniqueIdentifier:
                    {
                        Guid val = Guid.Empty;
                        Guid.TryParse(value.ToString(), out val);
                        return val;
                    }

                case SqlDbType.SmallInt:
                    {
                        short val = 0;
                        short.TryParse(value.ToString(), out val);
                        return val;
                    }

                case SqlDbType.TinyInt:
                    {
                        byte val = 0;
                        byte.TryParse(value.ToString(), out val);
                        return val;
                    }

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return value;

                case SqlDbType.Structured:
                    throw new NotImplementedException();

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    throw new NotImplementedException();

                case SqlDbType.DateTimeOffset:
                    {
                        DateTimeOffset val = DateTimeOffset.Now;
                        DateTimeOffset.TryParse(value.ToString(), out val);
                        return val;
                    }

                default:
                    return value;
            }
        }

        ///<inheritdoc/>
        public override string[] GetProcedureNames()
        {
            List<string> list = new List<string>();
            DataTable schema = null;
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);
                schema = conn.GetSchema("Procedures");
            }
            finally
            {
                DisposeConnection(conn);
            }

            foreach (DataRow row in schema.Rows)
            {
                string tableName = row["SPECIFIC_NAME"].ToString();
                string schemaName = row["SPECIFIC_SCHEMA"].ToString();
                if (tableName.StartsWith("dt_"))
                    continue;

                if (String.Compare(schemaName, "dbo") == 0)
                    list.Add(tableName);
                else
                    list.Add(schemaName + ".\"" + tableName + "\"");

            }
            return list.ToArray();
        }

        ///<inheritdoc/>
        public override TableDataSource CreateProcedure(string tableName)
        {
            ProcedureDataSource table = new ProcedureDataSource();
            table.Enabled = true;
            table.SelectCommand = tableName;
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);
                var schemaParameters = conn.GetSchema("ProcedureParameters", new string[] { null, null, tableName });
                foreach (DataRow row in schemaParameters.Rows)
                {
                    ParameterDirection direction = ParameterDirection.Input;
                    switch (row["PARAMETER_MODE"].ToString())
                    {
                        case "IN":
                            direction = ParameterDirection.Input;
                            table.Enabled = false;
                            break;
                        case "INOUT":
                            direction = ParameterDirection.InputOutput;
                            table.Enabled = false;
                            break;
                        case "OUT":
                            direction = ParameterDirection.Output;
                            break;
                    }
                    table.Parameters.Add(new ProcedureParameter()
                    {
                        Name = row["PARAMETER_NAME"].ToString(),
                        DataType = (int)(SqlDbType)Enum.Parse(typeof(SqlDbType), row["DATA_TYPE"].ToString(), true),
                        Direction = direction
                    });
                }
            }
            finally
            {
                DisposeConnection(conn);
            }

            return table;
        }

        public MsSqlDataConnection() : base()
        {
            CanContainProcedures = true;
        }

    }
}
