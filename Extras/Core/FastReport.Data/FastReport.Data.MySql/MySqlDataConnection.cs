using System;
using System.Collections.Generic;
using System.Data.Common;
using MySqlConnector;
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

                if (p.Value is Variant value)
                {
                    //https://mysqlconnector.net/troubleshooting/parameter-types/
                    if (value.Type == typeof(string))
                        parameter.Value = VariantToClrType(value, (MySqlDbType)p.DataType);
                    else
                        parameter.Value = value.ToType(value.Type);
                }
                else
                    parameter.Value = p.Value;
            }
            return adapter;
        }

        private object VariantToClrType(Variant value, MySqlDbType type)
        {
            if (value.ToString() == "")
                return null;

            switch (type)
            {
                case MySqlDbType.Int64:
                    {
                        long val = 0;
                        long.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.UInt64:
                    {
                        ulong val = 0;
                        ulong.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.Bit:
                case MySqlDbType.Bool:
                    {
                        bool val = false;
                        bool.TryParse(value.ToString(), out val);
                        if (value.ToString() == "1")
                            val = true;
                        return val;
                    }
                case MySqlDbType.Double:
                    {
                        double val = 0;
                        double.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.DateTime:
                case MySqlDbType.Date:
                    {
                        DateTime val = DateTime.Now;
                        DateTime.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.Time:
                case MySqlDbType.Timestamp:
                    {
                        TimeSpan val = TimeSpan.Zero;
                        TimeSpan.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.Decimal:
                    {
                        decimal val = 0;
                        decimal.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.Float:
                    {
                        float val = 0;
                        float.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.Int24:
                case MySqlDbType.UInt24:
                case MySqlDbType.Int32:
                    {
                        int val = 0;
                        int.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.UInt32:
                    {
                        uint val = 0;
                        uint.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.Guid:
                    {
                        Guid val = Guid.Empty;
                        Guid.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.Int16:
                    {
                        short val = 0;
                        short.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.UInt16:
                    {
                        ushort val = 0;
                        ushort.TryParse(value.ToString(), out val);
                        return val;
                    }
                case MySqlDbType.Byte:
                case MySqlDbType.UByte:
                    {
                        byte val = 0;
                        byte.TryParse(value.ToString(), out val);
                        return val;
                    }
                default:
                    return value.ToString();
            }
        }
    }
}
