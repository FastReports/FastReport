using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

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
                string schemaName = row["TABLE_SCHEMA"].ToString();
                if (!EnableSystemSchemas && (schemaName == "pg_catalog" || schemaName == "information_schema"))
                    continue;
                list.Add(schemaName + ".\"" + row["TABLE_NAME"].ToString() + "\"");
            }
        }

        private DataTable GetSchema(string selectCommand)
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

                var schema = GetSchema(selectCommand);
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

            var schema = GetSchema(selectCommand);
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
            string schemaName = "public";
            string procName = tableName;
            
            string[] parts = tableName.Split('.');
            if (parts.Length == 2)
            {
                schemaName = parts[0];
                procName = parts[1];
            }

            procName = procName.Replace("\"", "");

            var table = new ProcedureDataSource()
            {
                Enabled = true,
                SelectCommand = tableName
            };

            string selectCommand =
                "select proc.specific_schema as procedure_schema,\r\n" +
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

            var schema = GetSchema(selectCommand);
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
                            continue;
                    }

                    table.Parameters.Add(new ProcedureParameter()
                    {
                        Name = row["parameter_name"].ToString(),
                        DataType = (int)(NpgsqlDbType)Enum.Parse(typeof(NpgsqlDbType), row["data_type"].ToString(), true),
                        Direction = direction
                    });
                }
            }

            return table;
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
                parameter.Value = p.Value;
            }
            return adapter;
        }

        public PostgresDataConnection()
        {
            CanContainProcedures = true;
        }
    }
}
