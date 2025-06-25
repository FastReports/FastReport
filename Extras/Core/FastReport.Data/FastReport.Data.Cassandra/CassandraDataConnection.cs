using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;

namespace FastReport.Data.Cassandra
{
    public partial class CassandraDataConnection : DataConnectionBase
    {
        string keyspace; 
        Cluster cluster;
        ISession session;

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            return (string[])cluster.Metadata.GetTables(keyspace);
        }

        public override Task<string[]> GetTableNamesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetTableNames());
        }

        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

        /// <inheritdoc/>
        public override void CreateAllTables(bool initSchema)
        {
            if (session == null)
                InitConnection();
            base.CreateAllTables(initSchema);
        }

        public override async Task CreateAllTablesAsync(bool initSchema, CancellationToken cancellationToken = default)
        {
            if (session == null)
                await InitConnectionAsync();
            await base.CreateAllTablesAsync(initSchema, cancellationToken);
        }

        /// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {
            if (session == null)
                InitConnection();

            base.CreateTable(source);
        }

        public override async Task CreateTableAsync(TableDataSource source, CancellationToken cancellationToken = default)
        {
            if (session == null)
                await InitConnectionAsync();

            await base.CreateTableAsync(source, cancellationToken);
        }

        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            foreach (var column in session.Execute($"Select * from {table.TableName} LIMIT 1").Columns)
            {
                table.Columns.Add(column.Name, column.Type);
            }
        }

        public override Task FillTableSchemaAsync(DataTable table, string selectCommand, CommandParameterCollection parameters, CancellationToken cancellationToken)
        {
            FillTableSchema(table, selectCommand, parameters);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            foreach (var item in session.Execute($"Select * from {table.TableName}"))
            {
                DataRow row = table.NewRow();
                row.ItemArray = item.ToArray();
                table.Rows.Add(row);
            }
        }

        public override Task FillTableDataAsync(DataTable table, string selectCommand, CommandParameterCollection parameters, CancellationToken cancellationToken = default)
        {
            FillTableData(table, selectCommand, parameters);
            return Task.CompletedTask;
        }

        public CassandraDataConnection()
        {
            IsSqlBased = false;
        }

        private void InitConnection()
        {
            InitConnectionShared();
            session = cluster.Connect(keyspace);
        }

        private async Task InitConnectionAsync()
        {
            InitConnectionShared();
            session = await cluster.ConnectAsync(keyspace);
        }

        private void InitConnectionShared()
        {
            CassandraConnectionStringBuilder connStrBuilder = new CassandraConnectionStringBuilder(ConnectionString);
            Builder clusterBuilder = Cluster.Builder();

            if (connStrBuilder.Username != null && connStrBuilder.Username != ""
                && connStrBuilder.Password != "" && connStrBuilder.Password != null)
                clusterBuilder.WithCredentials(connStrBuilder.Username, connStrBuilder.Password);

            foreach (var item in connStrBuilder.ContactPoints)
            {
                clusterBuilder.AddContactPoint(item);
            }

            keyspace = connStrBuilder.DefaultKeyspace;
            cluster = clusterBuilder.Build();
        }
    }
}
//docker run --name dev-cassandra -d -p 9042:9042 cassandra:latest