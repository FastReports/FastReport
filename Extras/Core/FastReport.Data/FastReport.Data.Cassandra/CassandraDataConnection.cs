using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using Cassandra;
using Cassandra.Mapping;

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

        /// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {
            if (session == null)
                InitConnection();

            base.CreateTable(source);
        }

        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            foreach (var column in session.Execute($"Select * from {table.TableName} LIMIT 1").Columns)
            {
                table.Columns.Add(column.Name, column.Type);
            }
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

        public CassandraDataConnection()
        {
            IsSqlBased = false;
        }

        private void InitConnection()
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
            session = cluster.Connect(keyspace);
        }
    }
}
//docker run --name dev-cassandra -d -p 9042:9042 cassandra:latest