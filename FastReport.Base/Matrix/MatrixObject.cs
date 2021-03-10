using System;
using System.Collections.Generic;
using System.ComponentModel;
using FastReport.Table;
using FastReport.Data;
using FastReport.Utils;
using System.Drawing.Design;

namespace FastReport.Matrix
{
    /// <summary>
    /// Describes how the even style is applied to a matrix.
    /// </summary>
    public enum MatrixEvenStylePriority
    {
        /// <summary>
        /// The even style is applied to matrix rows.
        /// </summary>
        Rows,

        /// <summary>
        /// The even style is applied to matrix columns.
        /// </summary>
        Columns
    }


    /// <summary>
    /// Represents the matrix object that is used to print pivot table (also known as cross-tab).
    /// </summary>
    /// <remarks>
    /// The matrix consists of the following elements: columns, rows and data cells. Each element is
    /// represented by the <b>descriptor</b>. The <see cref="MatrixHeaderDescriptor"/> class is used
    /// for columns and rows; the <see cref="MatrixCellDescriptor"/> is used for data cells.
    /// The <see cref="Data"/> property holds three collections of descriptors - <b>Columns</b>,
    /// <b>Rows</b> and <b>Cells</b>.
    /// <para/>To create the matrix in a code, you should perform the following actions:
    /// <list type="bullet">
    ///   <item>
    ///     <description>create an instance of the <b>MatrixObject</b> and add it to the report;</description>
    ///   </item>
    ///   <item>
    ///     <description>create descriptors for columns, rows and cells and add it to the
    ///     collections inside the <see cref="Data"/> property;</description>
    ///   </item>
    ///   <item>
    ///     <description>call the <see cref="BuildTemplate"/> method to create the matrix template
    ///     that will be used to create a result;</description>
    ///   </item>
    ///   <item>
    ///     <description>modify the matrix template (change captions, set the visual appearance).</description>
    ///   </item>
    /// </list>
    /// <para/>To connect the matrix to a datasource, use the <see cref="DataSource"/> property. If
    /// this property is not set, the result matrix will be empty. In this case you may use 
    /// the <see cref="ManualBuild"/> event handler to fill the matrix.
    /// </remarks>
    /// <example>This example demonstrates how to create a matrix in a code.
    /// <code>
    /// // create an instance of MatrixObject
    /// MatrixObject matrix = new MatrixObject();
    /// matrix.Name = "Matrix1";
    /// // add it to the report title band of the first report page
    /// matrix.Parent = (report.Pages[0] as ReportPage).ReportTitle;
    /// 
    /// // create two column descriptors
    /// MatrixHeaderDescriptor column = new MatrixHeaderDescriptor("[MatrixDemo.Year]");
    /// matrix.Data.Columns.Add(column);
    /// column = new MatrixHeaderDescriptor("[MatrixDemo.Month]");
    /// matrix.Data.Columns.Add(column);
    /// 
    /// // create one row descriptor
    /// MatrixHeaderDescriptor row = new MatrixHeaderDescriptor("[MatrixDemo.Name]");
    /// matrix.Data.Rows.Add(row);
    /// 
    /// // create one data cell
    /// MatrixCellDescriptor cell = new MatrixCellDescriptor("[MatrixDemo.Revenue]", MatrixAggregateFunction.Sum);
    /// matrix.Data.Cells.Add(cell);
    /// 
    /// // connect matrix to a datasource
    /// matrix.DataSource = Report.GetDataSource("MatrixDemo");
    /// 
    /// // create the matrix template
    /// matrix.BuildTemplate();
    /// 
    /// // change the style
    /// matrix.Style = "Green";
    /// 
    /// // change the column and row total's text to "Grand Total"
    /// matrix.Data.Columns[0].TemplateTotalCell.Text = "Grand Total";
    /// matrix.Data.Rows[0].TemplateTotalCell.Text = "Grand Total";
    /// </code>
    /// </example>
    public partial class MatrixObject : TableBase
    {
        #region Fields
        private bool autoSize;
        private bool cellsSideBySide;
        private bool keepCellsSideBySide;
        private DataSourceBase dataSource;
        private string filter;
        private bool showTitle;
        private string style;
        private MatrixData data;
        private string manualBuildEvent;
        private string modifyResultEvent;
        private string afterTotalsEvent;
        private MatrixHelper helper;
        private bool saveVisible;
        private MatrixStyleSheet styleSheet;
        private object[] columnValues;
        private object[] rowValues;
        private int columnIndex;
        private int rowIndex;
        private MatrixEvenStylePriority matrixEvenStylePriority;
        private bool splitRows;
        private bool printIfEmpty;
        #endregion

        #region Properties
        /// <summary>
        /// Allows to fill the matrix in code.
        /// </summary>
        /// <remarks>
        /// In most cases the matrix is connected to a datasource via the <see cref="DataSource"/> 
        /// property. When you run a report, the matrix is filled with datasource values automatically.
        /// <para/>Using this event, you can put additional values to the matrix or even completely fill it
        /// with own values (if <see cref="DataSource"/> is set to <b>null</b>. To do this, call the
        /// <b>Data.AddValue</b> method. See the <see cref="MatrixData.AddValue(object[],object[],object[])"/>
        /// method for more details.
        /// </remarks>
        /// <example>This example shows how to fill a matrix with own values.
        /// <code>
        /// // suppose we have a matrix with one column, row and data cell.
        /// // provide 3 one-dimensional arrays with one element in each to the AddValue method
        /// Matrix1.Data.AddValue(
        ///   new object[] { 1996 },
        ///   new object[] { "Andrew Fuller" }, 
        ///   new object[] { 123.45f });
        /// Matrix1.Data.AddValue(
        ///   new object[] { 1997 },
        ///   new object[] { "Andrew Fuller" }, 
        ///   new object[] { 21.35f });
        /// Matrix1.Data.AddValue(
        ///   new object[] { 1997 },
        ///   new object[] { "Nancy Davolio" }, 
        ///   new object[] { 421.5f });
        /// 
        /// // this code will produce the following matrix:
        /// //               |  1996  |  1997  |
        /// // --------------+--------+--------+
        /// // Andrew Fuller |  123.45|   21.35|
        /// // --------------+--------+--------+
        /// // Nancy Davolio |        |  421.50|
        /// // --------------+--------+--------+
        /// </code>
        /// </example>
        public event EventHandler ManualBuild;

        /// <summary>
        /// Allows to modify the prepared matrix elements such as cells, rows, columns.
        /// </summary>
        public event EventHandler ModifyResult;

        /// <summary>
        /// Allows to modify the prepared matrix elements such as cells, rows, columns.
        /// </summary>
        public event EventHandler AfterTotals;

        /// <summary>
        /// Gets or sets a value that determines whether the matrix must calculate column/row sizes automatically.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool AutoSize
        {
            get { return autoSize; }
            set
            {
                autoSize = value;
                foreach (TableColumn column in Columns)
                {
                    column.AutoSize = AutoSize;
                }
                foreach (TableRow row in Rows)
                {
                    row.AutoSize = AutoSize;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that determines how to print multiple data cells.
        /// </summary>
        /// <remarks>
        /// This property can be used if matrix has two or more data cells. Default property value
        /// is <b>false</b> - that means the data cells will be stacked.
        /// </remarks>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool CellsSideBySide
        {
            get { return cellsSideBySide; }
            set
            {
                if (cellsSideBySide != value)
                {
                    cellsSideBySide = value;
                    if (IsDesigning)
                    {
                        foreach (MatrixCellDescriptor descr in Data.Cells)
                        {
                            descr.TemplateColumn = null;
                            descr.TemplateRow = null;
                        }

                        BuildTemplate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that the side-by-side cells must be kept together on the same page.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool KeepCellsSideBySide
        {
            get { return keepCellsSideBySide; }
            set { keepCellsSideBySide = value; }
        }

        /// <summary>
        /// Gets or sets a data source.
        /// </summary>
        /// <remarks>
        /// When you create the matrix in the designer by drag-drop data columns into it,
        /// this property will be set automatically. However you need to set it if you create 
        /// the matrix in code.
        /// </remarks>
        [Category("Data")]
        public DataSourceBase DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    if (dataSource != null)
                        dataSource.Disposed -= new EventHandler(DataSource_Disposed);
                    if (value != null)
                        value.Disposed += new EventHandler(DataSource_Disposed);
                }
                dataSource = value;
            }
        }

        /// <summary>
        /// Gets the row filter expression.
        /// </summary>
        /// <remarks>
        /// This property can contain any valid boolean expression. If the expression returns <b>false</b>,
        /// the corresponding data row will be skipped.
        /// </remarks>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show a title row.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool ShowTitle
        {
            get { return showTitle; }
            set
            {
                showTitle = value;
                if (IsDesigning)
                    BuildTemplate();
            }
        }

        /// <summary>
        /// Gets or sets a matrix style.
        /// </summary>
        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.MatrixStyleEditor, FastReport", typeof(UITypeEditor))]
        public new string Style
        {
            get { return style; }
            set
            {
                style = value;
                Helper.UpdateStyle();
            }
        }

        /// <summary>
        /// Gets or sets even style priority for matrix cells.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(MatrixEvenStylePriority.Rows)]
        public MatrixEvenStylePriority MatrixEvenStylePriority
        {
            get { return matrixEvenStylePriority; }
            set { matrixEvenStylePriority = value; }
        }

        /// <summary>
        /// Gets or sets need split rows.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool SplitRows 
        {
            get { return splitRows; }
            set { splitRows = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that empty matrix should be printed.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool PrintIfEmpty
        {
            get { return printIfEmpty; }
            set { printIfEmpty = value; }
        }

        /// <summary>
        /// Gets or sets a script method name that will be used to handle the 
        /// <see cref="ManualBuild"/> event.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ManualBuild"/> event for more details.
        /// </remarks>
        [Category("Build")]
        public string ManualBuildEvent
        {
            get { return manualBuildEvent; }
            set { manualBuildEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script method name that will be used to handle the 
        /// <see cref="ModifyResult"/> event.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ModifyResult"/> event for more details.
        /// </remarks>
        [Category("Build")]
        public string ModifyResultEvent
        {
            get { return modifyResultEvent; }
            set { modifyResultEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script method name that will be used to handle the 
        /// <see cref="AfterTotals"/> event.
        /// </summary>
        /// <remarks>
        /// See the <see cref="AfterTotals"/> event for more details.
        /// </remarks>
        [Category("Build")]
        public string AfterTotalsEvent
        {
            get { return afterTotalsEvent; }
            set { afterTotalsEvent = value; }
        }


        /// <summary>
        /// Gets the object that holds the collection of descriptors used
        /// to build a matrix.
        /// </summary>
        /// <remarks>
        /// See the <see cref="MatrixData"/> class for more details.
        /// </remarks>
        [Browsable(false)]
        public MatrixData Data
        {
            get { return data; }
        }

        /// <summary>
        /// Gets or sets array of values that describes the currently printing column.
        /// </summary>
        /// <remarks>
        /// Use this property when report is running. It can be used to highlight matrix elements
        /// depending on values of the currently printing column. To do this:
        /// <list type="bullet">
        ///   <item>
        ///     <description>select the cell that you need to highlight;</description>
        ///   </item>
        ///   <item>
        ///     <description>click the "Highlight" button on the "Text" toolbar;</description>
        ///   </item>
        ///   <item>
        ///     <description>add a new highlight condition. Use the <b>Matrix.ColumnValues</b> to 
        ///     refer to the value you need to analyze. Note: these values are arrays of <b>System.Object</b>, 
        ///     so you need to cast it to actual type before making any comparisons. Example of highlight
        ///     condition: <c>(int)Matrix1.ColumnValues[0] == 2000</c>.
        ///     </description>
        ///   </item>
        /// </list>
        /// </remarks>
        [Browsable(false)]
        public object[] ColumnValues
        {
            get { return columnValues; }
            set { columnValues = value; }
        }

        /// <summary>
        /// Gets or sets array of values that describes the currently printing row.
        /// </summary>
        /// <remarks>
        /// Use this property when report is running. It can be used to highlight matrix elements
        /// depending on values of the currently printing row. To do this:
        /// <list type="bullet">
        ///   <item>
        ///     <description>select the cell that you need to highlight;</description>
        ///   </item>
        ///   <item>
        ///     <description>click the "Highlight" button on the "Text" toolbar;</description>
        ///   </item>
        ///   <item>
        ///     <description>add a new highlight condition. Use the <b>Matrix.RowValues</b> to 
        ///     refer to the value you need to analyze. Note: these values are arrays of <b>System.Object</b>, 
        ///     so you need to cast it to actual type before making any comparisons. Example of highlight
        ///     condition: <c>(string)Matrix1.RowValues[0] == "Andrew Fuller"</c>.
        ///     </description>
        ///   </item>
        /// </list>
        /// </remarks>
        [Browsable(false)]
        public object[] RowValues
        {
            get { return rowValues; }
            set { rowValues = value; }
        }

        /// <summary>
        /// Gets or sets the index of currently printing column.
        /// </summary>
        /// <remarks>
        /// This property may be used to print even columns with alternate color. To do this:
        /// <list type="bullet">
        ///   <item>
        ///     <description>select the cell that you need to highlight;</description>
        ///   </item>
        ///   <item>
        ///     <description>click the "Highlight" button on the "Text" toolbar;</description>
        ///   </item>
        ///   <item>
        ///     <description>add a new highlight condition that uses the <b>Matrix.ColumnIndex</b>,
        ///     for example: <c>Matrix1.ColumnIndex % 2 == 1</c>.
        ///     </description>
        ///   </item>
        /// </list>
        /// </remarks>
        [Browsable(false)]
        public int ColumnIndex
        {
            get { return columnIndex; }
            set { columnIndex = value; }
        }

        /// <summary>
        /// Gets or sets the index of currently printing row.
        /// </summary>
        /// <remarks>
        /// This property may be used to print even rows with alternate color. To do this:
        /// <list type="bullet">
        ///   <item>
        ///     <description>select the cell that you need to highlight;</description>
        ///   </item>
        ///   <item>
        ///     <description>click the "Highlight" button on the "Text" toolbar;</description>
        ///   </item>
        ///   <item>
        ///     <description>add a new highlight condition that uses the <b>Matrix.RowIndex</b>,
        ///     for example: <c>Matrix1.RowIndex % 2 == 1</c>.
        ///     </description>
        ///   </item>
        /// </list>
        /// </remarks>
        [Browsable(false)]
        public int RowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        internal MatrixStyleSheet StyleSheet
        {
            get { return styleSheet; }
        }

        private MatrixHelper Helper
        {
            get { return helper; }
        }

        internal bool IsResultMatrix
        {
            get { return !IsDesigning && Data.Columns.Count == 0 && Data.Rows.Count == 0; }
        }

        private BandBase ParentBand
        {
            get
            {
                BandBase parentBand = this.Band;
                if (parentBand is ChildBand)
                    parentBand = (parentBand as ChildBand).GetTopParentBand;
                return parentBand;
            }
        }

        private DataBand FootersDataBand
        {
            get
            {
                DataBand dataBand = null;
                if (ParentBand is GroupFooterBand)
                    dataBand = ((ParentBand as GroupFooterBand).Parent as GroupHeaderBand).GroupDataBand;
                else if (ParentBand is DataFooterBand)
                    dataBand = ParentBand.Parent as DataBand;
                return dataBand;
            }
        }

        private bool IsOnFooter
        {
            get
            {
                DataBand dataBand = FootersDataBand;
                if (dataBand != null)
                {
                    return DataSource == dataBand.DataSource;
                }
                return false;
            }
        }
        #endregion

        #region Private Methods
        private void CreateResultTable()
        {
            SetResultTable(new TableResult());
            // assign properties from this object. Do not use Assign method: TableResult is incompatible with MatrixObject.
            ResultTable.OriginalComponent = OriginalComponent;
            ResultTable.Alias = Alias;
            ResultTable.Border = Border.Clone();
            ResultTable.Fill = Fill.Clone();
            ResultTable.Bounds = Bounds;
            ResultTable.RepeatHeaders = RepeatHeaders;
            ResultTable.RepeatRowHeaders = RepeatRowHeaders;
            ResultTable.RepeatColumnHeaders = RepeatColumnHeaders;
            ResultTable.Layout = Layout;
            ResultTable.WrappedGap = WrappedGap;
            ResultTable.AdjustSpannedCellsWidth = AdjustSpannedCellsWidth;
            ResultTable.SetReport(Report);
            ResultTable.AfterData += new EventHandler(ResultTable_AfterData);
        }

        private void DisposeResultTable()
        {
            ResultTable.Dispose();
            SetResultTable(null);
        }

        private void ResultTable_AfterData(object sender, EventArgs e)
        {
            OnModifyResult(e);
        }

        private void DataSource_Disposed(object sender, EventArgs e)
        {
            dataSource = null;
        }

        private void WireEvents(bool wire)
        {
            if (IsOnFooter)
            {
                DataBand dataBand = FootersDataBand;
                if (wire)
                    dataBand.BeforePrint += new EventHandler(dataBand_BeforePrint);
                else
                    dataBand.BeforePrint -= new EventHandler(dataBand_BeforePrint);
            }
        }

        private void dataBand_BeforePrint(object sender, EventArgs e)
        {
            bool firstRow = (sender as DataBand).IsFirstRow;
            if (firstRow)
                Helper.StartPrint();

            object match = true;
            if (!String.IsNullOrEmpty(Filter))
                match = Report.Calc(Filter);

            if (match is bool && (bool)match == true)
                Helper.AddDataRow();
        }
        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void DeserializeSubItems(FRReader reader)
        {
            if (String.Compare(reader.ItemName, "MatrixColumns", true) == 0)
                reader.Read(Data.Columns);
            else if (String.Compare(reader.ItemName, "MatrixRows", true) == 0)
                reader.Read(Data.Rows);
            else if (String.Compare(reader.ItemName, "MatrixCells", true) == 0)
                reader.Read(Data.Cells);
            else
                base.DeserializeSubItems(reader);
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            MatrixObject src = source as MatrixObject;
            AutoSize = src.AutoSize;
            CellsSideBySide = src.CellsSideBySide;
            KeepCellsSideBySide = src.KeepCellsSideBySide;
            DataSource = src.DataSource;
            Filter = src.Filter;
            ShowTitle = src.ShowTitle;
            Style = src.Style;
            MatrixEvenStylePriority = src.MatrixEvenStylePriority;
            SplitRows = src.SplitRows;
            PrintIfEmpty = src.PrintIfEmpty;
            data = src.Data;            
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            if (writer.SerializeTo != SerializeTo.SourcePages)
            {
                writer.Write(Data.Columns);
                writer.Write(Data.Rows);
                writer.Write(Data.Cells);
            }
            else
                RefreshTemplate(true);

            base.Serialize(writer);
            MatrixObject c = writer.DiffObject as MatrixObject;

            if (AutoSize != c.AutoSize)
                writer.WriteBool("AutoSize", AutoSize);
            if (CellsSideBySide != c.CellsSideBySide)
                writer.WriteBool("CellsSideBySide", CellsSideBySide);
            if (KeepCellsSideBySide != c.KeepCellsSideBySide)
                writer.WriteBool("KeepCellsSideBySide", KeepCellsSideBySide);
            if (DataSource != c.DataSource)
                writer.WriteRef("DataSource", DataSource);
            if (Filter != c.Filter)
                writer.WriteStr("Filter", Filter);
            if (ShowTitle != c.ShowTitle)
                writer.WriteBool("ShowTitle", ShowTitle);
            if (Style != c.Style)
                writer.WriteStr("Style", Style);
            if (MatrixEvenStylePriority != c.MatrixEvenStylePriority)
                writer.WriteValue("MatrixEvenStylePriority", MatrixEvenStylePriority);
            if (SplitRows != c.SplitRows)
                writer.WriteBool("SplitRows", SplitRows);
            if (PrintIfEmpty != c.PrintIfEmpty)
                writer.WriteBool("PrintIfEmpty", PrintIfEmpty);
            if (ManualBuildEvent != c.ManualBuildEvent)
                writer.WriteStr("ManualBuildEvent", ManualBuildEvent);
            if (ModifyResultEvent != c.ModifyResultEvent)
                writer.WriteStr("ModifyResultEvent", ModifyResultEvent);
            if (AfterTotalsEvent != c.AfterTotalsEvent)
                writer.WriteStr("AfterTotalsEvent", AfterTotalsEvent);
        }

        /// <summary>
        /// Creates or updates the matrix template.
        /// </summary>
        /// <remarks>
        /// Call this method after you modify the matrix descriptors using the <see cref="Data"/>
        /// object's properties. 
        /// </remarks>
        public void BuildTemplate()
        {
            Helper.BuildTemplate();
        }
        #endregion

        #region Report Engine
        /// <inheritdoc/>
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            WireEvents(true);
        }

        /// <inheritdoc/>
        public override void FinalizeComponent()
        {
            base.FinalizeComponent();
            WireEvents(false);
        }

        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            List<string> expressions = new List<string>();
            expressions.AddRange(base.GetExpressions());

            Helper.UpdateDescriptors();
            List<MatrixDescriptor> descrList = new List<MatrixDescriptor>();
            descrList.AddRange(Data.Columns.ToArray());
            descrList.AddRange(Data.Rows.ToArray());
            descrList.AddRange(Data.Cells.ToArray());

            foreach (MatrixDescriptor descr in descrList)
            {
                expressions.Add(descr.Expression);
                if (descr.TemplateCell != null)
                    descr.TemplateCell.AllowExpressions = false;
            }

            if (!String.IsNullOrEmpty(Filter))
                expressions.Add(Filter);

            return expressions.ToArray();
        }

        /// <inheritdoc/>
        public override void SaveState()
        {
            saveVisible = Visible;
            BandBase parent = Parent as BandBase;
            if (!Visible || (parent != null && !parent.Visible))
                return;

            // create the result table that will be rendered in the preview
            CreateResultTable();
            Visible = false;

            if (parent != null)
            {
                parent.Height = Top;
                parent.CanGrow = false;
                parent.CanShrink = false;
                parent.AfterPrint += new EventHandler(ResultTable.GeneratePages);
            }
        }

        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();

            if (!IsOnFooter)
            {
                Helper.StartPrint();
                Helper.AddDataRows();
            }
        }

        /// <inheritdoc/>
        public override void OnAfterData(EventArgs e)
        {
            base.OnAfterData(e);

            Helper.FinishPrint();
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            BandBase parent = Parent as BandBase;
            if (!saveVisible || (parent != null && !parent.Visible))
                return;

            if (parent != null)
                parent.AfterPrint -= new EventHandler(ResultTable.GeneratePages);

            DisposeResultTable();
            Visible = saveVisible;
        }

        /// <summary>
        /// This method fires the <b>ManualBuild</b> event and the script code connected to the <b>ManualBuildEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnManualBuild(EventArgs e)
        {
            if (ManualBuild != null)
                ManualBuild(this, e);
            InvokeEvent(ManualBuildEvent, e);
        }

        /// <summary>
        /// This method fires the <b>ModifyResult</b> event and the script code connected to the <b>ModifyResultEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnModifyResult(EventArgs e)
        {
            if (ModifyResult != null)
                ModifyResult(this, e);
            InvokeEvent(ModifyResultEvent, e);
        }

        /// <summary>
        /// This method fires the <b>AfterTotals</b> event and the script code connected to the <b>AfterTotalsEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnAfterTotals(EventArgs e)
        {
            if (AfterTotals != null)
                AfterTotals(this, e);
            InvokeEvent(AfterTotalsEvent, e);
        }

        /// <summary>
        /// Adds a value in the matrix.
        /// </summary>
        /// <param name="columnValues">Array of column values.</param>
        /// <param name="rowValues">Array of row values.</param>
        /// <param name="cellValues">Array of data values.</param>
        /// <remarks>
        /// This is a shortcut method to call the matrix <b>Data.AddValue</b>.
        /// See the <see cref="MatrixData.AddValue(object[],object[],object[])"/> method for more details.
        /// </remarks>
        public void AddValue(object[] columnValues, object[] rowValues, object[] cellValues)
        {
            Data.AddValue(columnValues, rowValues, cellValues, 0);
        }

        /// <summary>
        /// Gets the value of the data cell with the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the data cell.</param>
        /// <returns>The cell's value.</returns>
        /// <remarks>
        /// Use this method in the cell's expression if the cell has custom totals 
        /// (the total function is set to "Custom"). The example:
        /// <para/>Matrix1.Value(0) / Matrix1.Value(1)
        /// <para/>will return the result of dividing the first data cell's value by the second one.
        /// </remarks>
        public Variant Value(int index)
        {
            object value = Helper.CellValues[index];
            if (value == null)
                value = 0;
            return new Variant(value);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixObject"/> class.
        /// </summary>
        public MatrixObject()
        {
            autoSize = true;
            data = new MatrixData();
            manualBuildEvent = "";
            afterTotalsEvent = "";
            helper = new MatrixHelper(this);
            InitDesign();
            styleSheet = new MatrixStyleSheet();
            styleSheet.Load(ResourceLoader.GetStream("cross.frss"));
            style = "";
            filter = "";
            splitRows = false;
            printIfEmpty = true;
        }
    }
}