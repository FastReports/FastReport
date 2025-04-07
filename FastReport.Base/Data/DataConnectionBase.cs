using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using FastReport.Data.JsonConnection;
using FastReport.Utils;

namespace FastReport.Data
{
    /// <summary>
    /// The base class for all data connection components such as <see cref="MsSqlDataConnection"/>.
    /// </summary>
    /// <example>This example shows how to add a new MS Access connection to the report.
    /// <code>
    /// Report report1;
    /// MsAccessDataConnection conn = new MsAccessDataConnection();
    /// conn.DataSource = @"c:\data.mdb";
    /// report1.Dictionary.Connections.Add(conn);
    /// conn.CreateAllTables();
    /// </code>
    /// </example>
    public abstract partial class DataConnectionBase : DataComponentBase, IParent
    {
        #region Fields
        private DataSet dataSet;
        private TableCollection tables;
        private bool isSqlBased;
        private bool canContainProcedures;
        private string connectionString;
        private string connectionStringExpression;
        private bool loginPrompt;
        private int commandTimeout;
        private string lastConnectionString;
        #endregion

        #region Properties
        /// <summary>
        /// Gets an internal <b>DataSet</b> object that contains all data tables.
        /// </summary>
        [Browsable(false)]
        public DataSet DataSet
        {
            get
            {
                if (dataSet == null)
                    dataSet = CreateDataSet();
                return dataSet;
            }
        }

        /// <summary>
        /// Gets a collection of data tables in this connection.
        /// </summary>
        /// <remarks>
        /// To add a table to the connection, you must either create a new TableDataSource and add it
        /// to this collection or call the <see cref="CreateAllTables()"/> method which will add
        /// all tables available in the database.
        /// </remarks>
        [Browsable(false)]
        public TableCollection Tables
        {
            get { return tables; }
        }

        /// <summary>
        /// Gets or sets a connection string that contains all connection parameters.
        /// </summary>
        /// <remarks>
        /// <para>To modify some parameter of the connection, use respective 
        /// <b>ConnectionStringBuilder</b> class.</para>
        /// <para><b>Security note:</b> the connection string may contain a user name/password. 
        /// This information is stored in a report file. By default, it is crypted using the standard
        /// FastReport's password. Since FastReport's source code is available to anyone who paid for it,
        /// it may be insecure to use the standard password. For more security, you should use own
        /// password. To do this, specify it in the <b>Crypter.DefaultPassword</b> property.</para>
        /// </remarks>
        /// <example>This example demonstrates how to change a connection string:
        /// <code>
        /// OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(oleDbConnection1.ConnectionString);
        /// builder.PersistSecurityInfo = false;
        /// oleDbConnection1.ConnectionString = builder.ToString();
        /// </code>
        /// </example>
        [Category("Data")]
        [Browsable(true)]
        public string ConnectionString
        {
            get
            {
                if (Report != null && Report.IsRunning && !String.IsNullOrEmpty(ConnectionStringExpression))
                {
                    string value = (string)Report.Calc(ConnectionStringExpression);
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
                return connectionString;
            }
            set
            {
                SetConnectionString(Crypter.DecryptString(value));
            }
        }

        /// <summary>
        /// Gets or sets an expression that returns a connection string.
        /// </summary>
        /// <remarks>
        /// Use this property to set the connection string dynamically. 
        /// <para/>The recommended way to do this is to define a report parameter. You can do this in the 
        /// "Data" window. Once you have defined the parameter, you can use it to pass a value 
        /// to the connection. Set the <b>ConnectionStringExpression</b> property of the
        /// connection object to the report parameter's name (so it will look like [myReportParam]).
        /// To pass a value to the report parameter from your application, use the 
        /// <see cref="Report.SetParameterValue"/> method.
        /// <note type="caution">
        /// Once you set value for this property, the <see cref="ConnectionString"/> property will be ignored 
        /// when report is run.
        /// </note>
        /// </remarks>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string ConnectionStringExpression
        {
            get
            {
                return connectionStringExpression;
            }
            set { connectionStringExpression = value; }
        }

        /// <summary>
        /// Gets or sets a value indicates if this connection is SQL-based.
        /// </summary>
        [Browsable(false)]
        public bool IsSqlBased
        {
            get { return isSqlBased; }
            set { isSqlBased = value; }
        }

        /// <summary>
        /// Gets or sets a value indicates if this connection can contain procedures.
        /// </summary>
        [Browsable(false)]
        public bool CanContainProcedures
        {
            get { return canContainProcedures; }
            set { canContainProcedures = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a login dialog appears immediately before opening a connection.
        /// </summary>
        /// <remarks>
        /// Set <b>LoginPrompt</b> to <b>true</b> to provide login dialog when establishing a connection. If this
        /// property is <b>false</b> (by default), you should provide login information (user name and password)
        /// in the <see cref="ConnectionString"/> property. Though that property is stored in a crypted form,
        /// this may be insecure. 
        /// <para/>Another way to pass login information to the connection is to use
        /// <see cref="ConnectionStringExpression"/> property that is bound to the report parameter. In that
        /// case you supply the entire connection string from your application.
        /// </remarks>
        [Category("Data")]
        [DefaultValue(false)]
        public bool LoginPrompt
        {
            get { return loginPrompt; }
            set { loginPrompt = value; }
        }

        /// <summary>
        /// Gets or sets the command timeout, in seconds.
        /// </summary>
        [Category("Data")]
        [DefaultValue(30)]
        public int CommandTimeout
        {
            get { return commandTimeout; }
            set { commandTimeout = value; }
        }

        internal List<XmlItem> TablesStructure { get; set; }
        #endregion

        #region Private Methods

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
                list.Add(row["TABLE_NAME"].ToString());
            }
        }

        protected string PrepareSelectCommand(string selectCommand, string tableName, DbConnection connection)
        {
            if (String.IsNullOrEmpty(selectCommand))
            {
                selectCommand = "select * from " + QuoteIdentifier(tableName, connection);
            }
            return selectCommand;
        }

        protected TableDataSource FindTableDataSource(DataTable table)
        {
            foreach (TableDataSource c in Tables)
            {
                if (c.Table == table)
                    return c;
            }
            return null;
        }

        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                DisposeDataSet();
        }

        /// <summary>
        /// Initializes a <b>DataSet</b> instance.
        /// </summary>
        /// <returns>The <b>DataSet</b> object.</returns>
        /// <remarks>
        /// This method is used to support FastReport infrastructure. You don't need to use it.
        /// </remarks>
        protected virtual DataSet CreateDataSet()
        {
            DataSet dataSet = new DataSet();
            dataSet.EnforceConstraints = false;
            return dataSet;
        }

        /// <summary>
        /// Disposes a <b>DataSet</b>.
        /// </summary>
        /// <remarks>
        /// This method is used to support FastReport infrastructure. You don't need to use it.
        /// </remarks>
        protected void DisposeDataSet()
        {
            if (dataSet != null)
                dataSet.Dispose();
            dataSet = null;
        }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="value">New connection string.</param>
        /// <remarks>
        /// Use this method if you need to perform some actions when the connection string is set.
        /// </remarks>
        protected virtual void SetConnectionString(string value)
        {
            connectionString = value;
        }

        /// <summary>
        /// Gets a connection string that contains username and password specified.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <remarks>
        /// Override this method to pass login information to the connection. Typical implementation
        /// must get the existing <see cref="ConnectionString"/>, merge specified login information into it
        /// and return the new value.
        /// </remarks>
        protected virtual string GetConnectionStringWithLoginInfo(string userName, string password)
        {
            return ConnectionString;
        }
        #endregion

        #region IParent Members
        /// <inheritdoc/>
        public bool CanContain(Base child)
        {
            return child is TableDataSource;
        }

        /// <inheritdoc/>
        public void GetChildObjects(ObjectCollection list)
        {
            foreach (TableDataSource c in Tables)
            {
                list.Add(c);
            }
        }

        /// <inheritdoc/>
        public void AddChild(Base child)
        {
            Tables.Add(child as TableDataSource);
        }

        /// <inheritdoc/>
        public void RemoveChild(Base child)
        {
            Tables.Remove(child as TableDataSource);
        }

        /// <inheritdoc/>
        public int GetChildOrder(Base child)
        {
            // we don't need to handle database objects' order.
            return 0;
        }

        /// <inheritdoc/>
        public void SetChildOrder(Base child, int order)
        {
            // do nothing
        }

        /// <inheritdoc/>
        public void UpdateLayout(float dx, float dy)
        {
            // do nothing
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Fills the <see cref="Tables"/> collection with all tables available in the database.
        /// </summary>
        /// <remarks>
        /// This method does not read the table data; to do this, call the 
        /// <see cref="TableDataSource.LoadData"/> method of each table.
        /// </remarks>
        public void CreateAllTables()
        {
            CreateAllTables(true);
        }

        /// <summary>
        /// Fills the <see cref="Tables"/> collection with all tables available in the database.
        /// </summary>
        /// <param name="initSchema">Set to <b>true</b> to initialize each table's schema.</param>
        public virtual void CreateAllTables(bool initSchema)
        {
            List<string> tableNames = new List<string>();
            tableNames.AddRange(GetTableNames());
            FilterTables(tableNames);

            // remove tables with tablename that does not exist in the connection.
            for (int i = 0; i < Tables.Count; i++)
            {
                TableDataSource table = Tables[i];
                // skip queries and procedures
                if (!String.IsNullOrEmpty(table.SelectCommand) || table is ProcedureDataSource)
                    continue;

                bool found = false;
                foreach (string tableName in tableNames)
                {
                    if (String.Compare(table.TableName, tableName, true) == 0)
                    {
                        found = true;
                        break;
                    }
                }
                // table name not found in actual tablenames. It may happen if we have edited the connection.
                if (!found)
                {
                    table.Dispose();
                    i--;
                }
            }

            int tableNumber = 0;
            // now create tables that are not created yet.
            foreach (string tableName in tableNames)
            {
                bool found = false;
                foreach (TableDataSource table in Tables)
                {
                    if (String.Compare(table.TableName, tableName, true) == 0)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    var table = new TableDataSource();
                    // Enabled = false is used to ensure that when creating a connection,
                    // none of the tables in the list of tables will be selected by default
                    table.Enabled = false;

                    string fixedTableName = tableName.Replace(".", "_").Replace("[", "").Replace("]", "").Replace("\"", "");
                    if (Report != null)
                    {
                        table.Name = Report.Dictionary.CreateUniqueName(fixedTableName);
                        table.Alias = Report.Dictionary.CreateUniqueAlias(table.Alias);
                    }
                    else
                        table.Name = fixedTableName;
                    
                    table.TableName = tableName;
                    table.Connection = this;

                    if (TablesStructure != null)
                    {
                        table.Enabled = TablesStructure[tableNumber].Properties.Where(prop => prop.Key == "Enabled").Select(res => res.Value).First() == "true";
                    }

                    Tables.Add(table);
                    tableNumber++;
                }
            }

            // init table schema
            if (initSchema)
            {
                foreach (TableDataSource table in Tables)
                {
                    table.InitSchema();
                }
            }
        }

        /// <summary>
        /// Fills the <see cref="Tables"/> collection with all procedures available in the database.
        /// </summary>
        public virtual void CreateAllProcedures()
        {
            if (!CanContainProcedures)
                return;
            List<string> procedureNames = new List<string>();
            procedureNames.AddRange(GetProcedureNames());
            FilterTables(procedureNames);

            // remove procedures that does not exist in the connection.
            for (int i = 0; i < Tables.Count; i++)
            {
                var proc = Tables[i] as ProcedureDataSource;
                if (proc == null)
                    continue;

                bool found = false;
                foreach (string procName in procedureNames)
                {
                    if (String.Compare(proc.TableName, procName, true) == 0)
                    {
                        found = true;
                        break;
                    }
                }
                // proc name not found in actual proc names. It may happen if we have edited the connection.
                if (!found)
                {
                    proc.Dispose();
                    i--;
                }
            }

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
                    var proc = CreateProcedure(procName);
                    proc.Enabled = false;

                    string fixedName = procName.Replace(".", "_").Replace("[", "").Replace("]", "").Replace("\"", "");
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
        public virtual TableDataSource CreateProcedure(string tableName)
        {
            ProcedureDataSource table = new ProcedureDataSource();
            table.Enabled = true;
            table.SelectCommand = tableName;
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);
                var schemaParameters = conn.GetSchema("PROCEDURE_PARAMETRS", new string[] { null, null, tableName });
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
                        DataType = Convert.ToInt32(row["DATA_TYPE"].ToString()),
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

        /// <summary>
        /// Creates the relations between tables. Applies to XmlDataConnection only.
        /// </summary>
        public virtual void CreateRelations()
        {
            if (Report != null)
            {
                foreach (DataRelation relation in DataSet.Relations)
                {
                    Relation rel = new Relation();
                    rel.Name = relation.RelationName;
                    rel.ParentDataSource = FindTableDataSource(relation.ParentTable);
                    rel.ChildDataSource = FindTableDataSource(relation.ChildTable);
                    string[] parentColumns = new string[relation.ParentColumns.Length];
                    string[] childColumns = new string[relation.ChildColumns.Length];
                    for (int i = 0; i < relation.ParentColumns.Length; i++)
                    {
                        parentColumns[i] = relation.ParentColumns[i].Caption;
                    }
                    for (int i = 0; i < relation.ChildColumns.Length; i++)
                    {
                        childColumns[i] = relation.ChildColumns[i].Caption;
                    }
                    rel.ParentColumns = parentColumns;
                    rel.ChildColumns = childColumns;

                    if (Report.Dictionary.Relations.FindEqual(rel) == null)
                        Report.Dictionary.Relations.Add(rel);
                    else
                        rel.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets an array of table names available in the database.
        /// </summary>
        /// <returns>An array of strings.</returns>
        public virtual string[] GetTableNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("TABLE", list);
            GetDBObjectNames("VIEW", list);
            return list.ToArray();
        }

        /// <summary>
        /// Gets an array of subroutine names available in the database.
        /// </summary>
        /// <returns>An array of strings.</returns>
        public virtual string[] GetProcedureNames()
        {
            List<string> list = new List<string>();
            DataTable schema = null;
            DbConnection conn = GetConnection();

            if (conn != null)
            {
                try
                {
                    OpenConnection(conn);
                    schema = conn.GetSchema("PROCEDURE");
                    foreach (DataRow row in schema.Rows)
                    {
                        list.Add(row["PROCEDURE_NAME"].ToString());
                    }
                }
                finally
                {
                    DisposeConnection(conn);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns a type of connection.
        /// </summary>
        /// <returns><b>Type</b> instance.</returns>
        /// <remarks>
        /// You should override this method if you developing a new connection component.
        /// <para/>If your connection component does not use data connection, you need to override 
        /// the <see cref="FillTableSchema"/> and <see cref="FillTableData"/> methods instead.
        /// </remarks>
        /// <example>Here is the example of this method implementation:
        /// <code>
        /// public override Type GetConnectionType()
        /// {
        ///   return typeof(OleDbConnection);
        /// }
        /// </code>
        /// </example>
        public virtual Type GetConnectionType()
        {
            return null;
        }

        /// <summary>
        /// Returns a connection object.
        /// </summary>
        /// <returns>The <b>DbConnection</b> instance.</returns>
        /// <remarks>Either creates a new <b>DbConnection</b> instance of type provided by the
        /// <see cref="GetConnectionType"/> method, or returns the application connection if set
        /// in the Config.DesignerSettings.ApplicationConnection.</remarks>
        public virtual DbConnection GetConnection()
        {
            Type connectionType = GetConnectionType();
            if (connectionType != null)
            {

                DbConnection connection = GetDefaultConnection();

                if (connection != null)
                    return connection;

                // create a new connection object
                connection = Activator.CreateInstance(connectionType) as DbConnection;
                connection.ConnectionString = ConnectionString;
                return connection;
            }
            return null;
        }



        /// <summary>
        /// Opens a specified connection object.
        /// </summary>
        /// <param name="connection">Connection to open.</param>
        /// <remarks>
        /// Use this method to open a connection returned by the <see cref="GetConnection"/> method.
        /// <para/>This method displays a login dialog if your connection has the <see cref="LoginPrompt"/>
        /// property set to <b>true</b>. Once you have entered an user name and password in
        /// this dialog, it will remeber the entered values and will not used anymore in this report session.
        /// </remarks>
        public virtual void OpenConnection(DbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
                return;

            if (!String.IsNullOrEmpty(lastConnectionString))
            {
                // connection string is already provided, use it and skip other logic.
                connection.ConnectionString = lastConnectionString;
            }
            else
            {
                // try the global DatabaseLogin event
                string oldConnectionString = ConnectionString;
                DatabaseLoginEventArgs e = new DatabaseLoginEventArgs(ConnectionString);
                Config.ReportSettings.OnDatabaseLogin(this, e);

                // that event may do the following:
                // - modify the ConnectionString
                // - modify the username/password
                // - there is no event handler attached to the event, so it does nothing.

                if (oldConnectionString != e.ConnectionString)
                {
                    // the connection string was modified. Set the FLastConnectionString to use it next time silently.
                    lastConnectionString = e.ConnectionString;
                }
                else
                {
                    if (!String.IsNullOrEmpty(e.UserName) || !String.IsNullOrEmpty(e.Password))
                    {
                        // the username/password was modified. Get new connection string
                        lastConnectionString = GetConnectionStringWithLoginInfo(e.UserName, e.Password);
                    }
                    else if (LoginPrompt)
                    {
                        ShowLoginForm(lastConnectionString);
                    }
                }

                // update the connection if it's not done yet
                if (!String.IsNullOrEmpty(lastConnectionString))
                    connection.ConnectionString = lastConnectionString;
            }

            connection.Open();
            Config.ReportSettings.OnAfterDatabaseLogin(this, new AfterDatabaseLoginEventArgs(connection));
        }

        /// <summary>
        /// Disposes a connection.
        /// </summary>
        /// <param name="connection">The connection to dispose.</param>
        public virtual void DisposeConnection(DbConnection connection)
        {

            if (ShouldNotDispose(connection))
                return;

            if (connection != null)
                connection.Dispose();
        }

        /// <summary>
        /// Returns a <see cref="DbDataAdapter"/> object that is specific to this connection.
        /// </summary>
        /// <param name="selectCommand">The SQL command used to fetch a table data rows.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="parameters">The select command parameters.</param>
        /// <returns>The <b>DbDataAdapter</b> object.</returns>
        /// <remarks>
        /// You should override this method if you are developing a new connection component. In this method,
        /// you need to create the adapter and set its <b>SelectCommand</b>'s parameters.
        /// <para/>If your connection does not use data adapter, you need to override 
        /// the <see cref="FillTableSchema"/> and <see cref="FillTableData"/> methods instead.
        /// </remarks>
        /// <example>Here is the example of this method implementation:
        /// <code>
        /// public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
        ///   CommandParameterCollection parameters)
        /// {
        ///   OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommand, connection as OleDbConnection);
        ///   foreach (CommandParameter p in parameters)
        ///   {
        ///     OleDbParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (OleDbType)p.DataType, p.Size);
        ///     parameter.Value = p.Value;
        ///   }
        ///   return adapter;
        /// }
        /// </code>
        /// </example>
        public virtual DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
          CommandParameterCollection parameters)
        {
            return null;
        }

        /// <summary>
        /// Gets the type of parameter that is specific to this connection.
        /// </summary>
        /// <returns>The parameter's type.</returns>
        /// <remarks>
        /// This property is used in the report designer to display available data types when you edit the
        /// connection parameters. For example, the type of OleDbConnection parameter is a <b>OleDbType</b>.
        /// </remarks>
        public virtual Type GetParameterType()
        {
            return null;
        }

        /// <summary>
        /// Quotes the specified DB identifier such as table name or column name.
        /// </summary>
        /// <param name="value">Identifier to quote.</param>
        /// <param name="connection">The opened DB connection.</param>
        /// <returns>The quoted identifier.</returns>
        public abstract string QuoteIdentifier(string value, DbConnection connection);

        /// <summary>
        /// Fills the table schema.
        /// </summary>
        /// <param name="table">DataTable to fill.</param>
        /// <param name="selectCommand">The SQL select command.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <remarks>
        /// Usually you don't need to use this method. Internally it uses the <see cref="GetConnection"/> and
        /// <see cref="GetAdapter"/> methods to fill the table schema. If you create own connection component
        /// that does not use nor connection or adapter, then you need to override this method.
        /// </remarks>
        public virtual void FillTableSchema(DataTable table, string selectCommand,
          CommandParameterCollection parameters)
        {
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);

                TableDataSource dataSource = FindTableDataSource(table);

                // prepare select command
                if (!(dataSource is ProcedureDataSource))
                    selectCommand = PrepareSelectCommand(selectCommand, table.TableName, conn);

                // read the table schema
                using (DbDataAdapter adapter = GetAdapter(selectCommand, conn, parameters))
                {
                    adapter.SelectCommand.CommandType = dataSource is ProcedureDataSource ||
                        adapter.SelectCommand.CommandType == CommandType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    adapter.SelectCommand.CommandTimeout = CommandTimeout;
                    adapter.FillSchema(table, SchemaType.Source);
                }

                foreach (Column column in dataSource.Columns)
                {
                    // Deleting only disabled columns that exist in the table.
                    if (!column.Enabled && table.Columns.Contains(column.Name))
                        table.Columns.Remove(column.Name);
                }
            }
            finally
            {
                DisposeConnection(conn);
            }
        }

        /// <summary>
        /// Fills the table data.
        /// </summary>
        /// <param name="table">DataTable to fill.</param>
        /// <param name="selectCommand">The SQL select command.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <remarks>
        /// Usually you don't need to use this method. Internally it uses the <see cref="GetConnection"/> and
        /// <see cref="GetAdapter"/> methods to fill the table data. If you create own connection component
        /// that does not use nor connection or adapter, then you need to override this method.
        /// </remarks>
        public virtual void FillTableData(DataTable table, string selectCommand,
          CommandParameterCollection parameters)
        {
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);

                TableDataSource dataSource = FindTableDataSource(table);

                // prepare select command
                if (!(dataSource is ProcedureDataSource))
                    selectCommand = PrepareSelectCommand(selectCommand, table.TableName, conn);

                // read the table
                using (DbDataAdapter adapter = GetAdapter(selectCommand, conn, parameters))
                {
                    adapter.SelectCommand.CommandType = dataSource is ProcedureDataSource ||
                        adapter.SelectCommand.CommandType == CommandType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    adapter.SelectCommand.CommandTimeout = CommandTimeout;
                    table.Clear();
                    adapter.Fill(table);

                    // copy output parameter values
                    foreach (DbParameter dp in adapter.SelectCommand.Parameters)
                    {
                        if (dp.Direction != ParameterDirection.Input)
                        {
                            foreach (CommandParameter cp in parameters)
                            {
                                if (cp.Name == dp.ParameterName)
                                {
                                    cp.Value = dp.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                DisposeConnection(conn);
            }
        }

        /// <summary>
        /// Creates table.
        /// For internal use only.
        /// </summary>
        public virtual void CreateTable(TableDataSource source)
        {
            if (source.Table == null)
            {
                source.Table = new DataTable(source.TableName);
                DataSet.Tables.Add(source.Table);
            }
        }

        internal virtual void FillTable(TableDataSource source)
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
                    FillTableData(source.Table, source.SelectCommand, source.Parameters);
            }
        }

        /// <summary>
        /// Deletes table.
        /// For internal use only.
        /// </summary>
        public virtual void DeleteTable(TableDataSource source)
        {
            if (source.Table != null)
            {
                if (dataSet != null)
                    dataSet.Tables.Remove(source.Table);
                source.Table.Dispose();
                source.Table = null;
            }
        }

        /// <summary>
        /// Clone table.
        /// For internal use only.
        /// </summary>
        public virtual void Clone()
        {
            XmlItem item = new XmlItem();
            using (FRWriter writer = new FRWriter(item))
            {
                writer.SerializeTo = SerializeTo.Clipboard;
                writer.BlobStore = new BlobStore(false);
                writer.Write(this);
            }
            using (FRReader reader = new FRReader(Report, item))
            {
                reader.DeserializeFrom = SerializeTo.Clipboard;
                reader.BlobStore = new BlobStore(false);
                var connection = Activator.CreateInstance(this.GetType()) as DataConnectionBase;
                connection.Parent = this.Parent;
                connection.SetReport(Report);
                reader.Read(connection);
                connection.CreateUniqueName();
                foreach (TableDataSource table in connection.Tables)
                    table.CreateUniqueName();
                Report.Dictionary.AddChild(connection);
            }
        }

        protected void CreateUniqueNames(DataConnectionBase copyTo)
        {
            int i = 1;
            string s;
            do
            {
                s = this.Alias + i.ToString();
                i++;
            }
            while (Report.Dictionary.FindByAlias(s) != null);
            copyTo.Alias = s;
            copyTo.Name = s;
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            writer.ItemName = ClassName;
            if (Name != "")
                writer.WriteStr("Name", Name);
            if (Restrictions != Restrictions.None)
                writer.WriteValue("Restrictions", Restrictions);
            if (!String.IsNullOrEmpty(ConnectionString))
                writer.WriteStr("ConnectionString", Crypter.EncryptString(ConnectionString));
            if (!String.IsNullOrEmpty(ConnectionStringExpression))
                writer.WriteStr("ConnectionStringExpression", ConnectionStringExpression);
            if (LoginPrompt)
                writer.WriteBool("LoginPrompt", true);
            if (CommandTimeout != 30)
                writer.WriteInt("CommandTimeout", CommandTimeout);
            SerializeDesignExt(writer);
            if (writer.SaveChildren)
            {
                foreach (TableDataSource c in Tables)
                {
                    if (c.Enabled)
                        writer.Write(c);
                }
            }
        }

        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            return new string[] { ConnectionStringExpression };
        }

        partial void SetMaskConnectionStringPassword();

        partial void SetConnectionStringBrowsableAttribute();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConnectionBase"/> class with default settings.
        /// </summary>
        public DataConnectionBase()
        {
            tables = new TableCollection(this);
            connectionString = "";
            connectionStringExpression = "";
            IsSqlBased = true;
            canContainProcedures = false;
            commandTimeout = 30;
            SetFlags(Flags.CanEdit, true);
            SetConnectionStringBrowsableAttribute();
            SetMaskConnectionStringPassword();
        }

        partial void SerializeDesignExt(FRWriter writer);
    }
}