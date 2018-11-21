using System;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using FastReport.Utils;
using System.IO;
using System.Drawing.Design;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a datasource based on <b>DataTable</b> class.
    /// </summary>
    /// <example>This example shows how to add a new table to the existing connection:
    /// <code>
    /// Report report1;
    /// DataConnectionBase conn = report1.Dictionary.Connections.FindByName("Connection1");
    /// TableDataSource table = new TableDataSource();
    /// table.TableName = "Employees";
    /// table.Name = "Table1";
    /// conn.Tables.Add(table);
    /// </code>
    /// </example>
    public partial class TableDataSource : DataSourceBase
    {
        #region Fields
        private DataTable table;
        private string tableName;
        private string selectCommand;
        private CommandParameterCollection parameters;
        private bool storeData;
        private bool ignoreConnection;
        private string qbSchema;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the underlying <b>DataTable</b> object.
        /// </summary>
        [Browsable(false)]
        public DataTable Table
        {
            get { return table; }
            set { table = value; }
        }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        [Category("Data")]
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        /// <summary>
        /// Gets or sets SQL "select" command.
        /// </summary>
        /// <remarks>
        /// If this command contains parameters, you should specify them in the <see cref="Parameters"/>
        /// property.
        /// </remarks>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.SqlEditor, FastReport", typeof(UITypeEditor))]
        public string SelectCommand
        {
            get { return selectCommand; }
            set { selectCommand = value; }
        }

        /// <summary>
        /// Gets a collection of parameters used by "select" command.
        /// </summary>
        /// <remarks>
        /// You must set up this property if the SQL query that you've specified in the 
        /// <see cref="SelectCommand"/> property contains parameters.
        /// <para/>You can pass a value to the SQL parameter in two ways.
        /// <para/>The right way is to define a report parameter. You can do this in the 
        /// "Data" window. Once you have defined the parameter, you can use it to pass a value 
        /// to the SQL parameter. To do this, set the SQL parameter's <b>Expression</b> property
        /// to the report parameter's name (so it will look like [myReportParam]).
        /// To pass a value to the report parameter from your application, use the 
        /// <see cref="Report.SetParameterValue"/> method.
        /// <para/>The other way (unrecommended) is to find a datasource object and set its parameter from a code:
        /// <code>
        /// TableDataSource ds = report.GetDataSource("My DataSource Name") as TableDataSource;
        /// ds.Parameters[0].Value = 10;
        /// </code>
        /// This way is not good because you hardcode the report object's name.
        /// </remarks>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.CommandParametersEditor, FastReport", typeof(UITypeEditor))]
        public CommandParameterCollection Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <summary>
        /// Gets or sets the parent <see cref="DataConnectionBase"/> object.
        /// </summary>
        [Browsable(false)]
        public DataConnectionBase Connection
        {
            get { return IgnoreConnection ? null : Parent as DataConnectionBase; }
            set { Parent = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether it is necessary to store table data in a report file.
        /// </summary>
        [Category("Data")]
        [DefaultValue(false)]
        public bool StoreData
        {
            get { return storeData; }
            set { storeData = value; }
        }

        /// <summary>
        /// Gets or sets the table data.
        /// </summary>
        /// <remarks>
        /// This property is for internal use only.
        /// </remarks>
        [Browsable(false)]
        public string TableData
        {
            get
            {
                string result = "";
                if (Table == null && Connection != null)
                {
                    Connection.CreateTable(this);
                    Connection.FillTable(this);
                }

                if (Table != null)
                {
                    using (DataSet tempDs = new DataSet())
                    {
                        DataTable tempTable = Table.Copy();
                        tempDs.Tables.Add(tempTable);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            tempDs.WriteXml(stream, XmlWriteMode.WriteSchema);
                            result = Convert.ToBase64String(stream.ToArray());
                        }
                        tempTable.Dispose();
                    }
                }

                return result;
            }
            set
            {
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(value)))
                using (DataSet tempDs = new DataSet())
                {
                    tempDs.ReadXml(stream);
                    table = tempDs.Tables[0];
                    Reference = table;
                    tempDs.Tables.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// If set, ignores the Connection (always returns null). Needed when we replace the
        /// existing connection-based datasource with datatable defined in an application.
        /// </summary>
        internal bool IgnoreConnection
        {
            get { return ignoreConnection; }
            set { ignoreConnection = value; }
        }

        /// <summary>
        /// Gets or sets the query builder schema.
        /// </summary>
        /// <remarks>
        /// This property is for internal use only.
        /// </remarks>
        [Browsable(false)]
        public string QbSchema
        {
            get { return qbSchema; }
            set { qbSchema = value; }
        }
        #endregion

        #region Private Methods
        private Column CreateColumn(DataColumn column)
        {
            Column c = new Column();
            c.Name = column.ColumnName;
            c.Alias = column.Caption;
            c.DataType = column.DataType;
            c.SetBindableControlType(c.DataType);
            return c;
        }

        private void DeleteTable()
        {
            if (Connection != null)
                Connection.DeleteTable(this);
        }

        private void CreateColumns()
        {
            Columns.Clear();
            if (Table != null)
            {
                foreach (DataColumn column in Table.Columns)
                {
                    Column c = CreateColumn(column);
                    Columns.Add(c);
                }
            }
        }

        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DeleteTable();
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected override object GetValue(Column column)
        {
            if (column == null)
                return null;

            if (column.Tag == null)
                column.Tag = Table.Columns.IndexOf(column.Name);

            return CurrentRow == null ? null : ((DataRow)CurrentRow)[(int)column.Tag];
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void InitSchema()
        {
            if (Connection != null)
            {
                if (!StoreData)
                {
                    Connection.CreateTable(this);
                    if (Table.Columns.Count == 0)
                        Connection.FillTableSchema(Table, SelectCommand, Parameters);
                }
            }
            else
                table = Reference as DataTable;

            if (Columns.Count == 0)
                CreateColumns();

            foreach (Column column in Columns)
            {
                column.Tag = null;
            }
        }

        /// <inheritdoc/>
        public override void LoadData(ArrayList rows)
        {
            if (Connection != null)
            {
                if (!StoreData)
                    Connection.FillTable(this);
            }
            else
            {
                TryToLoadData();
            }

            if (Table == null)
                throw new DataTableException(Alias);

            // custom load data via Load event
            OnLoad();

            bool needReload = ForceLoadData || rows.Count == 0 || Parameters.Count > 0;
            if (needReload)
            {
                // fill rows
                rows.Clear();
                foreach (DataRow row in Table.Rows)
                {
                    if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                        rows.Add(row);
                }
            }
        }

        /// <summary>
        /// Refresh the table schema.
        /// </summary>
        public void RefreshTable()
        {
            DeleteTable();
            InitSchema();
            RefreshColumns(true);
        }

        internal void RefreshColumns(bool enableNew)
        {
            if (Table != null)
            {
                // add new columns
                foreach (DataColumn column in Table.Columns)
                {
                    if (Columns.FindByName(column.ColumnName) == null)
                    {
                        Column c = CreateColumn(column);
                        c.Enabled = enableNew;
                        Columns.Add(c);
                    }
                }
                // delete obsolete columns
                int i = 0;
                while (i < Columns.Count)
                {
                    Column c = Columns[i];
                    if (!c.Calculated && !Table.Columns.Contains(c.Name))
                        c.Dispose();
                    else
                        i++;
                }
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            if (!String.IsNullOrEmpty(TableName))
                writer.WriteStr("TableName", TableName);
            if (!String.IsNullOrEmpty(SelectCommand))
                writer.WriteStr("SelectCommand", SelectCommand);
            if (!String.IsNullOrEmpty(QbSchema))
                writer.WriteStr("QbSchema", QbSchema);
            if (StoreData)
            {
                writer.WriteBool("StoreData", true);
                writer.WriteStr("TableData", TableData);
            }
        }

        /// <inheritdoc/>
        public override void SetParent(Base value)
        {
            base.SetParent(value);
            SetFlags(Flags.CanEdit, Connection != null);
        }

        /// <inheritdoc/>
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            foreach (CommandParameter par in Parameters)
            {
                par.ResetLastValue();
            }
        }
        #endregion

        #region IParent Members
        /// <inheritdoc/>
        public override bool CanContain(Base child)
        {
            return base.CanContain(child) || child is CommandParameter;
        }

        /// <inheritdoc/>
        public override void GetChildObjects(ObjectCollection list)
        {
            base.GetChildObjects(list);
            foreach (CommandParameter p in Parameters)
            {
                list.Add(p);
            }
        }

        /// <inheritdoc/>
        public override void AddChild(Base child)
        {
            if (child is CommandParameter)
                Parameters.Add(child as CommandParameter);
            else
                base.AddChild(child);
        }

        /// <inheritdoc/>
        public override void RemoveChild(Base child)
        {
            if (child is CommandParameter)
                Parameters.Remove(child as CommandParameter);
            else
                base.RemoveChild(child);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDataSource"/> class with default settings.
        /// </summary>
        public TableDataSource()
        {
            TableName = "";
            SelectCommand = "";
            QbSchema = "";
            parameters = new CommandParameterCollection(this);
        }
    }
}