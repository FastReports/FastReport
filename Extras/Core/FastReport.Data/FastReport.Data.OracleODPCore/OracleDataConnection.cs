using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.Data
{
    public class OracleDataConnection : DataConnectionBase
    {
        private void GetDBObjectNames(string name, string columnName, List<string> list, bool ignoreShema = false)
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

            GetDBObjectNamesShared(columnName, list, ignoreShema, schema);
        }

        private async Task GetDBObjectNamesAsync(string name, string columnName, List<string> list, bool ignoreShema = false, CancellationToken cancellationToken = default)
        {
            DataTable schema = null;
            DbConnection connection = GetConnection();
            try
            {
                await OpenConnectionAsync(connection, cancellationToken);
                OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(connection.ConnectionString);
                schema = await connection.GetSchemaAsync(name, new string[] { builder.UserID.ToUpper(), null }, cancellationToken);
            }
            finally
            {
                await DisposeConnectionAsync(connection);
            }

            GetDBObjectNamesShared(columnName, list, ignoreShema, schema);
        }

        private static void GetDBObjectNamesShared(string columnName, List<string> list, bool ignoreShema, DataTable schema)
        {
            foreach (DataRow row in schema.Rows)
            {
                string tableName = row[columnName].ToString();
                string schemaName = row["OWNER"].ToString();
                if (String.Compare(schemaName, "SYSTEM") == 0 || ignoreShema)
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

        public override async Task<string[]> GetTableNamesAsync(CancellationToken cancellationToken = default)
        {
            List<string> list = new List<string>();
            await GetDBObjectNamesAsync("Tables", "TABLE_NAME", list, cancellationToken: cancellationToken);
            await GetDBObjectNamesAsync("Views", "VIEW_NAME", list, cancellationToken: cancellationToken);
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

        public override string[] GetProcedureNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("Procedures", "OBJECT_NAME", list, true);
            return list.ToArray();
        }

        public override async Task<string[]> GetProcedureNamesAsync(CancellationToken cancellationToken = default)
        {
            List<string> list = new List<string>();
            await GetDBObjectNamesAsync("Procedures", "OBJECT_NAME", list, true, cancellationToken);
            return list.ToArray();
        }

        private DataTable GetSchema(string selectCommand)
        {
            var connection = GetConnection();
            try
            {
                OpenConnection(connection);

                var dataset = new DataSet();
                var adapter = new OracleDataAdapter(selectCommand, connection as OracleConnection);
                adapter.Fill(dataset);

                if (dataset.Tables.Count > 0)
                    return dataset.Tables[0];
            }
            finally
            {
                DisposeConnection(connection);
            }

            return null;
        }

        private async Task<DataTable> GetSchemaAsync(string selectCommand, CancellationToken cancellationToken = default)
        {
            var connection = GetConnection();
            try
            {
                await OpenConnectionAsync(connection, cancellationToken);

                var dataset = new DataSet();
                var adapter = new OracleDataAdapter(selectCommand, connection as OracleConnection);
                adapter.Fill(dataset);

                if (dataset.Tables.Count > 0)
                    return dataset.Tables[0];
            }
            finally
            {
                await DisposeConnectionAsync(connection);
            }

            return null;
        }

        public override TableDataSource CreateProcedure(string tableName)
        {
            ProcedureDataSource table = new ProcedureDataSource();
            table.Enabled = true;
            table.SelectCommand = tableName;
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);
                var schemaParameters = GetSchema($"SELECT * FROM SYS.ALL_ARGUMENTS WHERE OBJECT_NAME='{tableName}'");
                CreateProcedureShared(table, schemaParameters);
            }
            finally
            {
                DisposeConnection(conn);
            }

            return table;
        }

        public override async Task<TableDataSource> CreateProcedureAsync(string tableName, CancellationToken cancellationToken = default)
        {
            ProcedureDataSource table = new ProcedureDataSource();
            table.Enabled = true;
            table.SelectCommand = tableName;
            DbConnection conn = GetConnection();
            try
            {
                await OpenConnectionAsync(conn, cancellationToken);
                var schemaParameters = await GetSchemaAsync($"SELECT * FROM SYS.ALL_ARGUMENTS WHERE OBJECT_NAME='{tableName}'", cancellationToken);
                CreateProcedureShared(table, schemaParameters);
            }
            finally
            {
                await DisposeConnectionAsync(conn);
            }

            return table;
        }

        private static void CreateProcedureShared(ProcedureDataSource table, DataTable schemaParameters)
        {
            foreach (DataRow row in schemaParameters.Rows)
            {
                ParameterDirection direction = ParameterDirection.Input;
                switch (row["IN_OUT"].ToString())
                {
                    case "IN":
                        direction = ParameterDirection.Input;
                        table.Enabled = false;
                        break;
                    case "IN/OUT":
                        direction = ParameterDirection.InputOutput;
                        table.Enabled = false;
                        break;
                    case "OUT":
                        direction = ParameterDirection.Output;
                        break;
                }

                if (!int.TryParse(row["DATA_PRECISION"].ToString(), out int precision))
                    precision = 0;
                if (!int.TryParse(row["DATA_SCALE"].ToString(), out int scale))
                    scale = 0;
                table.Parameters.Add(new ProcedureParameter()
                {
                    Name = row["ARGUMENT_NAME"].ToString(),
                    DataType = (int)MapTypes(row["DATA_TYPE"].ToString(), Convert.ToInt32(precision), Convert.ToInt32(scale)),
                    Direction = direction,
                });
            }
        }

        public static OracleDbType MapTypes(string dataType, int precision, int scale)
        {
            switch (dataType)
            {
                case "BFILE":
                    return OracleDbType.BFile;
                case "BINARY_FLOAT":
                    return OracleDbType.BinaryFloat;
                case "BINARY_DOUBLE":
                    return OracleDbType.BinaryDouble;
                case "BLOB":
                    return OracleDbType.Blob;
                case "BOOLEAN":
                case "PL/SQL BOOLEAN":
                    return OracleDbType.Boolean;
                case "byte":
                    return OracleDbType.Byte;
                case "CHAR":
                    return OracleDbType.Char;
                case "CLOB":
                    return OracleDbType.Clob;
                case "DATE":
                    return OracleDbType.Date;
                case "NUMBER":
                case "INTEGER":
                case "FLOAT":
                    return ConvertNumberToOraDbType(precision, scale);
                case "INTERVAL DAY TO SECOND":
                    return OracleDbType.IntervalDS;
                case "INTERVAL YEAR TO MONTH":
                    return OracleDbType.IntervalYM;
                case "LONG":
                    return OracleDbType.Long;
                case "LONG RAW":
                    return OracleDbType.BFile;
                case "NCHAR":
                    return OracleDbType.NChar;
                case "NCLOB":
                    return OracleDbType.NClob;
                case "NVARCHAR2":
                    return OracleDbType.NVarchar2;
                case "RAW":
                    return OracleDbType.Raw;
                case "REF CURSOR":
                    return OracleDbType.RefCursor;
                case "TIMESTAMP":
                    return OracleDbType.TimeStamp;
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                    return OracleDbType.TimeStampLTZ;
                case "TIMESTAMP WITH TIME ZONE":
                    return OracleDbType.TimeStampTZ;
                case "VARCHAR2":
                    return OracleDbType.Varchar2;
                case "OPAQUE/XMLTYPE":
                case "XMLTYPE":
                    return OracleDbType.XmlType;
            }

            return OracleDbType.Varchar2;
        }

        private static OracleDbType ConvertNumberToOraDbType(int precision, int scale)
        {
            OracleDbType result = OracleDbType.Decimal;
            if (scale <= 0 && precision - scale < 5)
            {
                result = OracleDbType.Int16;
            }
            else if (scale <= 0 && precision - scale < 10)
            {
                result = OracleDbType.Int32;
            }
            else if (scale <= 0 && precision - scale < 19)
            {
                result = OracleDbType.Int64;
            }
            else if (precision < 8 && ((scale <= 0 && precision - scale <= 38) || (scale > 0 && scale <= 44)))
            {
                result = OracleDbType.Single;
            }
            else if (precision < 16)
            {
                result = OracleDbType.Double;
            }

            return result;
        }

        public OracleDataConnection()
        {
            CanContainProcedures = true;
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