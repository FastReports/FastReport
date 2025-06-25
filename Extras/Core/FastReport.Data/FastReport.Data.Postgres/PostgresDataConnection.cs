using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    public partial class PostgresDataConnection : DataConnectionBase
    {
        /// <summary>
        /// Specified, system schemas should be shown or not
        /// </summary>
        public static bool EnableSystemSchemas { get; set; }

        private void GetDBObjectNames(string name, List<string> list)
        {
            DataTable schema = GetSchema(name);

            GetDBObjectNamesShared(list, schema);
        }

        private async Task GetDBObjectNamesAsync(string name, List<string> list, CancellationToken cancellationToken)
        {
            DataTable schema = await GetSchemaAsync(name, cancellationToken);

            GetDBObjectNamesShared(list, schema);
        }

        private static void GetDBObjectNamesShared(List<string> list, DataTable schema)
        {
            foreach (DataRow row in schema.Rows)
            {
                string schemaName = row["TABLE_SCHEMA"].ToString();
                if (!EnableSystemSchemas && (schemaName == "pg_catalog" || schemaName == "information_schema"))
                    continue;
                list.Add(schemaName + ".\"" + row["TABLE_NAME"].ToString() + "\"");
            }
        }

        private DataTable ExecuteSelect(string selectCommand)
        {
            var connection = GetConnection();
            try
            {
                OpenConnection(connection);

                var dataset = new DataSet();
                var adapter = new NpgsqlDataAdapter(selectCommand, connection as NpgsqlConnection);
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

        private async Task<DataTable> ExecuteSelectAsync(string selectCommand, CancellationToken cancellationToken = default)
        {
            var connection = GetConnection();
            try
            {
                await OpenConnectionAsync(connection, cancellationToken);

                var dataset = new DataSet();
                var adapter = new NpgsqlDataAdapter(selectCommand, connection as NpgsqlConnection);
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


        private static int MapTypes(string data_type)
        {
            var parenIndex = data_type.IndexOf('(');
            if (parenIndex > -1)
                data_type = data_type.Substring(0, parenIndex);

            NpgsqlDbType result = NpgsqlDbType.Unknown;

            switch (data_type)
            {
                // Numeric types
                case "smallint": result = NpgsqlDbType.Smallint; break;
                case "integer":
                case "int": result = NpgsqlDbType.Integer; break;
                case "bigint": result = NpgsqlDbType.Bigint; break;
                case "real": result = NpgsqlDbType.Real; break;
                case "double precision": result = NpgsqlDbType.Double; break;
                case "numeric": result = NpgsqlDbType.Numeric; break;
                case "money": result = NpgsqlDbType.Money; break;

                // Text types
                case "text": result = NpgsqlDbType.Text; break;
                case "xml": result = NpgsqlDbType.Xml; break;
                case "character varying":
                case "varchar": result = NpgsqlDbType.Varchar; break;
                case "character": result = NpgsqlDbType.Char; break;
                case "name": result = NpgsqlDbType.Name; break;
                case "refcursor": result = NpgsqlDbType.Refcursor; break;
                case "citext": result = NpgsqlDbType.Citext; break;
                case "jsonb": result = NpgsqlDbType.Jsonb; break;
                case "json": result = NpgsqlDbType.Json; break;
                case "jsonpath": result = NpgsqlDbType.JsonPath; break;

                // Date/time types
                case "timestamp without time zone":
                case "timestamp": result = NpgsqlDbType.Timestamp; break;
                case "timestamp with time zone":
                case "timestamptz": result = NpgsqlDbType.TimestampTz; break;
                case "date": result = NpgsqlDbType.Date; break;
                case "time without time zone":
                case "timetz": result = NpgsqlDbType.Time; break;
                case "time with time zone":
                case "time": result = NpgsqlDbType.TimeTz; break;
                case "interval": result = NpgsqlDbType.Interval; break;

                // Network types
                case "cidr": result = NpgsqlDbType.Cidr; break;
                case "inet": result = NpgsqlDbType.Inet; break;
                case "macaddr": result = NpgsqlDbType.MacAddr; break;
                case "macaddr8": result = NpgsqlDbType.MacAddr8; break;

                // Full-text search types
                case "tsquery": result = NpgsqlDbType.TsQuery; break;
                case "tsvector": result = NpgsqlDbType.TsVector; break;

                // Geometry types
                case "box": result = NpgsqlDbType.Box; break;
                case "circle": result = NpgsqlDbType.Circle; break;
                case "line": result = NpgsqlDbType.Line; break;
                case "lseg": result = NpgsqlDbType.LSeg; break;
                case "path": result = NpgsqlDbType.Path; break;
                case "point": result = NpgsqlDbType.Point; break;
                case "polygon": result = NpgsqlDbType.Polygon; break;

                // LTree types
                case "lquery": result = NpgsqlDbType.LQuery; break;
                case "ltree": result = NpgsqlDbType.LTree; break;
                case "ltxtquery": result = NpgsqlDbType.LTxtQuery; break;

                // UInt types
                case "oid": result = NpgsqlDbType.Oid; break;
                case "xid": result = NpgsqlDbType.Xid; break;
                case "xid8": result = NpgsqlDbType.Xid8; break;
                case "cid": result = NpgsqlDbType.Cid; break;
                case "regtype": result = NpgsqlDbType.Regtype; break;
                case "regconfig": result = NpgsqlDbType.Regconfig; break;

                // Misc types
                case "boolean":
                case "bool": result = NpgsqlDbType.Boolean; break;
                case "bytea": result = NpgsqlDbType.Bytea; break;
                case "uuid": result = NpgsqlDbType.Uuid; break;
                case "bit varying":
                case "varbit": result = NpgsqlDbType.Varbit; break;
                case "bit": result = NpgsqlDbType.Bit; break;
                case "hstore": result = NpgsqlDbType.Hstore; break;

                case "geometry": result = NpgsqlDbType.Geometry; break;
                case "geography": result = NpgsqlDbType.Geography; break;

                // Built-in range types
                case "int4range": result = NpgsqlDbType.IntegerRange; break;
                case "int8range": result = NpgsqlDbType.BigIntRange; break;
                case "numrange": result = NpgsqlDbType.NumericRange; break;
                case "tsrange": result = NpgsqlDbType.TimestampRange; break;
                case "tstzrange": result = NpgsqlDbType.TimestampTzRange; break;
                case "daterange": result = NpgsqlDbType.DateRange; break;

                // Built-in multirange types
                case "int4multirange": result = NpgsqlDbType.IntegerMultirange; break;
                case "int8multirange": result = NpgsqlDbType.BigIntMultirange; break;
                case "nummultirange": result = NpgsqlDbType.NumericMultirange; break;
                case "tsmultirange": result = NpgsqlDbType.TimestampMultirange; break;
                case "tstzmultirange": result = NpgsqlDbType.TimestampTzMultirange; break;
                case "datemultirange": result = NpgsqlDbType.DateMultirange; break;

                // Internal types
                case "int2vector": result = NpgsqlDbType.Int2Vector; break;
                case "oidvector": result = NpgsqlDbType.Oidvector; break;
                case "pg_lsn": result = NpgsqlDbType.PgLsn; break;
                case "tid": result = NpgsqlDbType.Tid; break;
                case "char": result = NpgsqlDbType.InternalChar; break;

                default:
                    {
                        if (data_type.EndsWith("[]", StringComparison.Ordinal))
                        {
                            NpgsqlDbType elementNpgsqlDbType = (NpgsqlDbType)MapTypes(data_type.Substring(0, data_type.Length - 2));
                            result = elementNpgsqlDbType != NpgsqlDbType.Unknown ? elementNpgsqlDbType | NpgsqlDbType.Array : NpgsqlDbType.Unknown; // e.g. ranges
                        }
                        break;
                    }
            }

            return (int)result;
        }    

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("Tables", list);
            GetDBObjectNames("Views", list);
            
            if (list.Count == 0)
            {
                string selectCommand =
                    "SELECT n.nspname as \"Schema\", " +
                    "c.relname as \"Name\", " +
                    "CASE c.relkind WHEN 'r' THEN 'table' WHEN 'v' THEN 'view' WHEN 'i' THEN 'index' WHEN 'S' THEN 'sequence' WHEN 's' THEN 'special' WHEN 'f' THEN 'foreign table' END as \"Type\", " +
                    "pg_catalog.pg_get_userbyid(c.relowner) as \"Owner\" " +
                    "FROM pg_catalog.pg_class c " +
                         "LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace " +
                    "WHERE c.relkind IN ('r', 'v', '') " +
                          "AND n.nspname <> 'pg_catalog' " +
                          "AND n.nspname <> 'information_schema' " +
                          "AND n.nspname !~'^pg_toast' " +
                      "AND pg_catalog.pg_table_is_visible(c.oid) " +
                    "ORDER BY 1,2; ";

                var schema = ExecuteSelect(selectCommand);
                if (schema != null)
                {
                    foreach (DataRow row in schema.Rows)
                    {
                        list.Add(row["Name"].ToString());
                    }
                }
            }

            return list.ToArray();
        }

        public override async Task<string[]> GetTableNamesAsync(CancellationToken cancellationToken = default)
        {
            List<string> list = new List<string>();
            await GetDBObjectNamesAsync("Tables", list, cancellationToken);
            await GetDBObjectNamesAsync("Views", list, cancellationToken);

            if (list.Count == 0)
            {
                string selectCommand =
                    "SELECT n.nspname as \"Schema\", " +
                    "c.relname as \"Name\", " +
                    "CASE c.relkind WHEN 'r' THEN 'table' WHEN 'v' THEN 'view' WHEN 'i' THEN 'index' WHEN 'S' THEN 'sequence' WHEN 's' THEN 'special' WHEN 'f' THEN 'foreign table' END as \"Type\", " +
                    "pg_catalog.pg_get_userbyid(c.relowner) as \"Owner\" " +
                    "FROM pg_catalog.pg_class c " +
                         "LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace " +
                    "WHERE c.relkind IN ('r', 'v', '') " +
                          "AND n.nspname <> 'pg_catalog' " +
                          "AND n.nspname <> 'information_schema' " +
                          "AND n.nspname !~'^pg_toast' " +
                      "AND pg_catalog.pg_table_is_visible(c.oid) " +
                    "ORDER BY 1,2; ";

                var schema = await ExecuteSelectAsync(selectCommand, cancellationToken);
                if (schema != null)
                {
                    foreach (DataRow row in schema.Rows)
                    {
                        list.Add(row["Name"].ToString());
                    }
                }
            }

            return list.ToArray();
        }


        ///<inheritdoc/>
        public override string[] GetProcedureNames()
        {
            List<string> list = new List<string>();

            string selectCommand =
                "SELECT routine_schema As schema_name,\r\n" +
                "routine_name As procedure_name\r\n" +
                "FROM information_schema.routines\r\n" +
                "WHERE routine_type = 'FUNCTION'";

            var schema = ExecuteSelect(selectCommand);

            if (schema != null)
            {
                foreach (DataRow row in schema.Rows)
                {
                    string schemaName = row["schema_name"].ToString();
                    if (!EnableSystemSchemas && (schemaName == "pg_catalog" || schemaName == "information_schema"))
                        continue;
                    list.Add(schemaName + ".\"" + row["procedure_name"].ToString() + "\"");
                }
            }
            
            return list.ToArray();
        }

        public override async Task<string[]> GetProcedureNamesAsync(CancellationToken cancellationToken = default)
        {
            List<string> list = new List<string>();

            string selectCommand =
                "SELECT routine_schema As schema_name,\r\n" +
                "routine_name As procedure_name\r\n" +
                "FROM information_schema.routines\r\n" +
                "WHERE routine_type = 'FUNCTION'";

            var schema = await ExecuteSelectAsync(selectCommand, cancellationToken);

            if (schema != null)
            {
                foreach (DataRow row in schema.Rows)
                {
                    string schemaName = row["schema_name"].ToString();
                    if (!EnableSystemSchemas && (schemaName == "pg_catalog" || schemaName == "information_schema"))
                        continue;
                    list.Add(schemaName + ".\"" + row["procedure_name"].ToString() + "\"");
                }
            }

            return list.ToArray();
        }

        ///<inheritdoc/>
        public override TableDataSource CreateProcedure(string tableName)
        {
            CreateProcedureBeforeCommand(tableName, out ProcedureDataSource table, out string selectCommand);

            var schema = ExecuteSelect(selectCommand);
            CreateProcedureShared(table, schema);

            return table;
        }

        public override async Task<TableDataSource> CreateProcedureAsync(string tableName, CancellationToken cancellationToken = default)
        {
            CreateProcedureBeforeCommand(tableName, out ProcedureDataSource table, out string selectCommand);

            var schema = await ExecuteSelectAsync(selectCommand, cancellationToken);
            CreateProcedureShared(table, schema);

            return table;
        }

        private static void CreateProcedureBeforeCommand(string tableName, out ProcedureDataSource table, out string selectCommand)
        {
            string schemaName = "public";
            string procName = tableName;

            string[] parts = tableName.Split('.');
            if (parts.Length == 2)
            {
                schemaName = parts[0];
                procName = parts[1];
            }

            procName = procName.Replace("\"", "");

            table = new ProcedureDataSource()
            {
                Enabled = true,
                SelectCommand = tableName
            };
            selectCommand = "select proc.specific_schema as procedure_schema,\r\n" +
                "       proc.specific_name,\r\n" +
                "       proc.routine_name as procedure_name,\r\n" +
                "       proc.external_language,\r\n" +
                "       args.parameter_name,\r\n" +
                "       args.parameter_mode,\r\n" +
                "       args.data_type\r\n" +
                "from information_schema.routines proc\r\n" +
                "left join information_schema.parameters args\r\n" +
                "          on proc.specific_schema = args.specific_schema\r\n" +
                "          and proc.specific_name = args.specific_name\r\n" +
                "where proc.routine_schema not in ('pg_catalog', 'information_schema')\r\n" +
                "      and proc.routine_type = 'FUNCTION'\r\n" +
                "      and proc.specific_schema = '" + schemaName + "'\r\n" +
                "      and proc.routine_name = '" + procName + "'\r\n" +
                "order by procedure_schema,\r\n" +
                "         specific_name,\r\n" +
                "         procedure_name,\r\n" +
                "         args.ordinal_position;";
        }

        private static void CreateProcedureShared(ProcedureDataSource table, DataTable schema)
        {
            if (schema != null)
            {
                foreach (DataRow row in schema.Rows)
                {
                    var direction = ParameterDirection.Input;
                    switch (row["parameter_mode"].ToString())
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
                        // skip completely: it's a result table's column
                        default:
                            continue;
                    }

                    table.Parameters.Add(new ProcedureParameter()
                    {
                        Name = row["parameter_name"].ToString(),
                        DataType = MapTypes(row["data_type"].ToString()),
                        Direction = direction
                    });
                }
            }
        }


        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

        /// <inheritdoc/>
        protected override string GetConnectionStringWithLoginInfo(string userName, string password)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(ConnectionString);

            builder.Username = userName;
            builder.Password = password;

            return builder.ToString();
        }

        /// <inheritdoc/>
        public override Type GetConnectionType()
        {
            return typeof(NpgsqlConnection);
        }

        /// <inheritdoc/>
        public override Type GetParameterType()
        {
            return typeof(NpgsqlDbType);
        }

        /// <inheritdoc/>
        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
            CommandParameterCollection parameters)
        {
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(selectCommand, connection as NpgsqlConnection);
            foreach (CommandParameter p in parameters)
            {
                NpgsqlParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (NpgsqlDbType)p.DataType, p.Size);
                if (p.Value is Variant value)
                {
                    if (value.Type == typeof(string))
                        parameter.Value = VariantToClrType(value, (NpgsqlDbType)p.DataType);
                    else
                        parameter.Value = value.ToType(value.Type);
                }
                else
                    parameter.Value = p.Value;
            }
            return adapter;
        }

        private static object VariantToClrType(Variant value, NpgsqlDbType type)
        {
            if (value.ToString() == "")
                return null;

            switch (type)
            {
                case NpgsqlDbType.Bigint:
                    {
                        long val = 0;
                        long.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Boolean:
                    {
                        bool val = false;
                        bool.TryParse(value.ToString(), out val);
                        if (value.ToString() == "1")
                            val = true;
                        return val;
                    }
                case NpgsqlDbType.Double:
                    {
                        double val = 0;
                        double.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Timestamp:
                case NpgsqlDbType.TimestampTz:
                case NpgsqlDbType.Date:
                    {
                        DateTime val = DateTime.Now;
                        DateTime.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Time:
                case NpgsqlDbType.Interval:
                    {
                        TimeSpan val = TimeSpan.Zero;
                        TimeSpan.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.TimeTz:
                    {
                        DateTimeOffset val = DateTimeOffset.Now;
                        DateTimeOffset.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Money:
                case NpgsqlDbType.Numeric:
                    {
                        decimal val = 0;
                        decimal.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Real:
                    {
                        float val = 0;
                        float.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Integer:
                    {
                        int val = 0;
                        int.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Oid:
                case NpgsqlDbType.Xid:
                case NpgsqlDbType.Cid:
                    {
                        uint val = 0;
                        uint.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Tid:
                    {
                        string[] values = value.ToString().Replace("(", "").Replace(")", "").Split(',');

                        uint val1 = 0;
                        uint.TryParse(values[0], out val1);

                        ushort val2 = 0;
                        ushort.TryParse(values[1], out val2);

                        return new NpgsqlTid(val1, val2);
                    }
                case NpgsqlDbType.Uuid:
                    {
                        Guid val = Guid.Empty;
                        Guid.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Smallint:
                    {
                        short val = 0;
                        short.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.InternalChar:
                    {
                        byte val = 0;
                        byte.TryParse(value.ToString(), out val);
                        return val;
                    }
                case NpgsqlDbType.Box:
                    {
                        try
                        {
                            return PostgresTypesParsers.ParseBox(value.ToString());
                        }
                        catch
                        {
                            return new NpgsqlBox();
                        }
                    }
                case NpgsqlDbType.Circle:
                    {
                        try
                        {
                            return PostgresTypesParsers.ParseCircle(value.ToString());
                        }
                        catch
                        {
                            return new NpgsqlCircle();
                        }
                    }
                case NpgsqlDbType.Line:
                    {
                        try
                        {
                            return PostgresTypesParsers.ParseLine(value.ToString());
                        }
                        catch
                        {
                            return new NpgsqlLine();
                        }
                    }
                case NpgsqlDbType.Polygon:
                    {
                        try
                        {
                            return PostgresTypesParsers.ParsePolygon(value.ToString());
                        }
                        catch
                        {
                            return new NpgsqlPolygon();
                        }
                    }
                case NpgsqlDbType.Path:
                    {
                        try
                        {
                            return PostgresTypesParsers.ParsePath(value.ToString());
                        }
                        catch
                        {
                            return new NpgsqlPath();
                        }
                    }
                case NpgsqlDbType.LSeg:
                    {
                        try
                        {
                            return PostgresTypesParsers.ParseLSeg(value.ToString());
                        }
                        catch
                        {
                            return new NpgsqlLSeg();
                        }
                    }
                case NpgsqlDbType.Point:
                    {
                        try
                        {
                            return PostgresTypesParsers.ParsePoint(value.ToString());
                        }
                        catch
                        {
                            return new NpgsqlPoint();
                        }
                    }
                case NpgsqlDbType.Cidr:
                    {
                        try
                        {
                            return new NpgsqlCidr(value.ToString());
                        }
                        catch
                        {
                            return new NpgsqlCidr();
                        }
                    }
                case NpgsqlDbType.Inet:
                    {
                        try
                        {
                            return new NpgsqlInet(value.ToString());
                        }
                        catch
                        {
                            return new NpgsqlInet();
                        }
                    }
                case NpgsqlDbType.MacAddr:
                    {
                        try
                        {
                            return PhysicalAddress.Parse(value.ToString());
                        }
                        catch
                        {
                            return PhysicalAddress.None;
                        }
                    }
                case NpgsqlDbType.TsQuery:
                    {
                        try
                        {
                            return NpgsqlTsQuery.Parse(value.ToString());
                        }
                        catch
                        {
                            return null;
                        }
                    }
                case NpgsqlDbType.TsVector:
                    {
                        try
                        {
                            return NpgsqlTsVector.Parse(value.ToString());
                        }
                        catch
                        {
                            return null;
                        }
                    }
                 case NpgsqlDbType.Bytea:
                    {
                        string val = value.ToString();
                        if (val.Length % 2 != 0)
                        {
                            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", val));
                        }

                        byte[] data = new byte[val.Length / 2];
                        for (int index = 0; index < data.Length; index++)
                        {
                            string byteValue = val.Substring(index * 2, 2);
                            data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        }

                        return data;
                    }
                case NpgsqlDbType.Array:
                case NpgsqlDbType.Range:
                case NpgsqlDbType.Hstore:
                case NpgsqlDbType.Oidvector:
                case NpgsqlDbType.MacAddr8:
                case NpgsqlDbType.Int2Vector:
                    throw new NotImplementedException();
                default:
                    return value.ToString();
            }
        }

        public PostgresDataConnection()
        {
            CanContainProcedures = true;
            AppContext.SetSwitch("Npgsql.EnableStoredProcedureCompatMode", true);
        }
    }
}
