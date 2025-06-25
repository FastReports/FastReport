﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    partial class LinterDataConnection : DataConnectionBase
    {
        DbProviderFactory factory;

        private void GetDBObjectNames(string name, string columnName, List<string> list)
        {
            DataTable schema = null;
            DbConnection connection = GetConnection();
            connection.ConnectionString = ConnectionString;
            try
            {
                OpenConnection(connection);
                DbConnectionStringBuilder builder = factory.CreateConnectionStringBuilder();
                builder.ConnectionString = ConnectionString;
                schema = connection.GetSchema(name, new string[] { (string)builder["UserID"] });
            }
            finally
            {
                DisposeConnection(connection);
            }

            GetDBObjectNames(name, columnName, list, schema);
        }

        private async Task GetDBObjectNamesAsync(string name, string columnName, List<string> list, CancellationToken cancellationToken)
        {
            DataTable schema = null;
            DbConnection connection = GetConnection();
            connection.ConnectionString = ConnectionString;
            try
            {
                await OpenConnectionAsync(connection, cancellationToken);
                DbConnectionStringBuilder builder = factory.CreateConnectionStringBuilder();
                builder.ConnectionString = ConnectionString;
                schema = await connection.GetSchemaAsync(name, new string[] { (string)builder["UserID"] }, cancellationToken);
            }
            finally
            {
                await DisposeConnectionAsync(connection);
            }

            GetDBObjectNames(name, columnName, list, schema);
        }

        private static void GetDBObjectNames(string name, string columnName, List<string> list, DataTable schema)
        {
            foreach (DataRow row in schema.Rows)
            {
                string tableName = row[columnName].ToString();
                string schemaName = row["TABLE_SCHEM" + (name == "Views" ? "A" : "")].ToString();
                if (schemaName.Contains("SYSTEM"))
                    list.Add(tableName);
                else
                    list.Add(schemaName + ".\"" + tableName + "\"");
            }
        }

        public override string[] GetTableNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("Tables", "TABLE_NAME", list);
            GetDBObjectNames("Views", "TABLE_NAME", list);
            return list.ToArray();
        }

        public override async Task<string[]> GetTableNamesAsync(CancellationToken cancellationToken)
        {
            List<string> list = new List<string>();
            await GetDBObjectNamesAsync("Tables", "TABLE_NAME", list, cancellationToken);
            await GetDBObjectNamesAsync("Views", "TABLE_NAME", list, cancellationToken);
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
            DbConnectionStringBuilder builder = factory.CreateConnectionStringBuilder();

            builder.ConnectionString = ConnectionString;
            builder["UserID"] = userName;
            builder["Password"] = password;

            return builder.ToString();
        }


        public override Type GetConnectionType()
        {
            return factory.CreateConnection().GetType();
        }

        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
          CommandParameterCollection parameters)
        {
            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = factory.CreateCommand();
            adapter.SelectCommand.Connection = connection;
            adapter.SelectCommand.CommandText = selectCommand;

            foreach (CommandParameter p in parameters)
            {
                adapter.SelectCommand.Parameters.Add(p.Value);
            }

            return adapter;
        }

        public LinterDataConnection()
        {
            try
            {
                Init();
            }
            catch 
            {
                factory = null;
            }
            IsSqlBased = true;
        }

        private void Init()
        {
            factory = DbProviderFactories.GetFactory("System.Data.LinterClient");
        }
    }
}
