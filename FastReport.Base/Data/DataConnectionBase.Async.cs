using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FastReport.Utils;

namespace FastReport.Data
{
    public abstract partial class DataConnectionBase
    {

        #region Private Methods

        private async Task GetDBObjectNamesAsync(string name, List<string> list, CancellationToken cancellationToken)
        {
            var schema = await GetSchemaAsync("Tables", new string[] { null, null, null, name }, cancellationToken);
            foreach (DataRow row in schema.Rows)
            {
                list.Add(row["TABLE_NAME"].ToString());
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Fills the <see cref="Tables"/> collection with all tables available in the database.
        /// </summary>
        /// <remarks>
        /// This method does not read the table data; to do this, call the 
        /// <see cref="TableDataSource.LoadDataAsync"/> method of each table.
        /// </remarks>
        public Task CreateAllTablesAsync(CancellationToken cancellationToken = default)
        {
            return CreateAllTablesAsync(true, cancellationToken);
        }

        /// <summary>
        /// Fills the <see cref="Tables"/> collection with all tables available in the database.
        /// </summary>
        /// <param name="initSchema">Set to <b>true</b> to initialize each table's schema.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        public virtual async Task CreateAllTablesAsync(bool initSchema, CancellationToken cancellationToken = default)
        {
            List<string> tableNames = new List<string>();
            tableNames.AddRange(await GetTableNamesAsync(cancellationToken));
            FilterTables(tableNames);

            CreateAllTablesShared(tableNames);

            // init table schema
            if (initSchema)
            {
                foreach (TableDataSource table in Tables)
                {
                    await table.InitSchemaAsync(cancellationToken);
                }
            }
        }

        /// <summary>
        /// Fills the <see cref="Tables"/> collection with all procedures available in the database.
        /// </summary>
        public virtual async Task CreateAllProceduresAsync(CancellationToken cancellationToken = default)
        {
            if (!CanContainProcedures)
                return;
            List<string> procedureNames = new List<string>();
            procedureNames.AddRange(await GetProcedureNamesAsync(cancellationToken));
            FilterTables(procedureNames);

            RemoveProcedures(procedureNames);

            // now create procedures that are not created yet.
            foreach (string procName in procedureNames)
            {
                bool found = false;
                foreach (TableDataSource table in Tables)
                {
                    if (String.Compare(table.TableName, procName, true) == 0)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    var proc = await CreateProcedureAsync(procName, cancellationToken);
                    proc.Enabled = false;

                    string fixedName = procName.Replace('.', '_').Replace("[", "").Replace("]", "").Replace("\"", "");
                    if (Report != null)
                    {
                        proc.Name = Report.Dictionary.CreateUniqueName(fixedName);
                        proc.Alias = Report.Dictionary.CreateUniqueAlias(proc.Alias);
                    }
                    else
                        proc.Name = fixedName;

                    proc.TableName = procName;
                    proc.Connection = this;

                    Tables.Add(proc);
                }
            }
        }

        /// <summary>
        /// Create the stored procedure.
        /// </summary>
        public virtual async Task<TableDataSource> CreateProcedureAsync(string tableName, CancellationToken cancellationToken = default)
        {
            var schemaParameters = await GetSchemaAsync("PROCEDURE_PARAMETRS", new string[] { null, null, tableName }, cancellationToken);
            return CreateProcedureShared(tableName, schemaParameters);
        }

        /// <summary>
        /// Gets an array of table names available in the database.
        /// </summary>
        /// <returns>An array of strings.</returns>
        public virtual async Task<string[]> GetTableNamesAsync(CancellationToken cancellationToken = default)
        {
            List<string> list = new List<string>();
            await GetDBObjectNamesAsync("TABLE", list, cancellationToken);
            await GetDBObjectNamesAsync("VIEW", list, cancellationToken);
            return list.ToArray();
        }

        /// <summary>
        /// Gets an array of subroutine names available in the database.
        /// </summary>
        /// <returns>An array of strings.</returns>
        public virtual async Task<string[]> GetProcedureNamesAsync(CancellationToken cancellationToken)
        {
            var schema = await GetSchemaAsync("PROCEDURE", cancellationToken);

            return GetProcedureNamesShared(schema);
        }


        /// <summary>
        /// Opens a specified connection object.
        /// </summary>
        /// <param name="connection">Connection to open.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <remarks>
        /// Use this method to open a connection returned by the <see cref="GetConnection"/> method.
        /// <para/>This method displays a login dialog if your connection has the <see cref="LoginPrompt"/>
        /// property set to <b>true</b>. Once you have entered an user name and password in
        /// this dialog, it will remeber the entered values and will not used anymore in this report session.
        /// </remarks>
        public virtual async Task OpenConnectionAsync(DbConnection connection, CancellationToken cancellationToken = default)
        {
            if (connection.State == ConnectionState.Open)
                return;

            OpenConnectionShared(connection);

            await connection.OpenAsync(cancellationToken);
            Config.ReportSettings.OnAfterDatabaseLogin(this, new AfterDatabaseLoginEventArgs(connection));
        }

        /// <summary>
        /// Disposes a connection.
        /// </summary>
        /// <param name="connection">The connection to dispose.</param>
        public virtual async Task DisposeConnectionAsync(DbConnection connection)
        {
            if (ShouldNotDispose(connection))
                return;

            if (connection != null)
                await connection.DisposeAsync();
        }

        /// <summary>
        /// Fills the table schema.
        /// </summary>
        /// <param name="table">DataTable to fill.</param>
        /// <param name="selectCommand">The SQL select command.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <remarks>
        /// Usually you don't need to use this method. Internally it uses the <see cref="GetConnection"/> and
        /// <see cref="GetAdapter"/> methods to fill the table schema. If you create own connection component
        /// that does not use nor connection or adapter, then you need to override this method.
        /// </remarks>
        public virtual async Task FillTableSchemaAsync(DataTable table, string selectCommand,
          CommandParameterCollection parameters, CancellationToken cancellationToken = default)
        {
            DbConnection conn = GetConnection();
            try
            {
                await OpenConnectionAsync(conn, cancellationToken);

                FillTableSchemaShared(table, selectCommand, parameters, conn);
            }
            finally
            {
                await DisposeConnectionAsync(conn);
            }
        }

        /// <summary>
        /// Creates table asynchronously.
        /// For internal use only.
        /// </summary>
        public virtual async Task CreateTableAsync(TableDataSource source, CancellationToken cancellationToken = default)
        {
            if (source.Table == null)
            {
                source.Table = new DataTable(source.TableName);
                var dataSet = await GetDataSetAsync(cancellationToken);
                dataSet.Tables.Add(source.Table);
            }
        }

        /// <summary>
        /// Fills the table data.
        /// </summary>
        /// <param name="table">DataTable to fill.</param>
        /// <param name="selectCommand">The SQL select command.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <remarks>
        /// Usually you don't need to use this method. Internally it uses the <see cref="GetConnection"/> and
        /// <see cref="GetAdapter"/> methods to fill the table data. If you create own connection component
        /// that does not use nor connection or adapter, then you need to override this method.
        /// </remarks>
        public virtual async Task FillTableDataAsync(DataTable table, string selectCommand,
          CommandParameterCollection parameters, CancellationToken cancellationToken = default)
        {
            DbConnection conn = GetConnection();
            try
            {
                await OpenConnectionAsync(conn, cancellationToken);

                FillTableDataShared(table, selectCommand, parameters, conn);
            }
            finally
            {
                await DisposeConnectionAsync(conn);
            }
        }

        /// <summary>
        /// Initializes a <b>DataSet</b> instance.
        /// </summary>
        /// <returns>The <b>DataSet</b> object.</returns>
        /// <remarks>
        /// This method is used to support FastReport infrastructure. You don't need to use it.
        /// </remarks>
        protected virtual Task<DataSet> CreateDataSetAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(CreateDataSet());
        }


        public async Task<DataSet> GetDataSetAsync(CancellationToken cancellationToken = default)
        {
            if (dataSet == null)
                dataSet = await CreateDataSetAsync(cancellationToken);
            return dataSet;
        }

        internal virtual async Task FillTableAsync(TableDataSource source, CancellationToken cancellationToken)
        {
            if (source.Table != null)
            {
                bool parametersChanged = false;
                foreach (CommandParameter par in source.Parameters)
                {
                    object value = par.Value;
                    if (!Object.Equals(value, par.LastValue))
                    {
                        par.LastValue = value;
                        parametersChanged = true;
                    }
                }

                if (source.ForceLoadData || source.Table.Rows.Count == 0 || parametersChanged)
                    await FillTableDataAsync(source.Table, source.SelectCommand, source.Parameters, cancellationToken);
            }
        }

        protected async Task<DataTable> GetSchemaAsync(string collectionName, CancellationToken cancellationToken)
        {
            DbConnection conn = GetConnection();
            try
            {
                await OpenConnectionAsync(conn, cancellationToken);
                return await conn.GetSchemaAsync(collectionName, cancellationToken);
            }
            finally
            {
                await DisposeConnectionAsync(conn);
            }
        }

        protected async Task<DataTable> GetSchemaAsync(string collectionName, string[] restrictionValues, CancellationToken cancellationToken)
        {
            DbConnection conn = GetConnection();
            try
            {
                await OpenConnectionAsync(conn, cancellationToken);
                return await conn.GetSchemaAsync(collectionName, restrictionValues, cancellationToken);
            }
            finally
            {
                await DisposeConnectionAsync(conn);
            }
        }

        #endregion

    }
}