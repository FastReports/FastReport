using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Table
{
    /// <summary>
    /// Specifies the layout that will be used when printing a big table.
    /// </summary>
    public enum TableLayout
    {
        /// <summary>
        /// The table is printed across a pages then down.
        /// </summary>
        AcrossThenDown,

        /// <summary>
        /// The table is printed down then across a pages.
        /// </summary>
        DownThenAcross,

        /// <summary>
        /// The table is wrapped.
        /// </summary>
        Wrapped
    }

    /// <summary>
    /// The base class for table-type controls such as <see cref="TableObject"/> and 
    /// <see cref="FastReport.Matrix.MatrixObject"/>.
    /// </summary>
    public partial class TableBase : BreakableComponent, IParent
    {
        #region Fields
        private TableRowCollection rows;
        private TableColumnCollection columns;
        private TableStyleCollection styles;
        private int fixedRows;
        private int fixedColumns;
        private float wrappedGap;
        private bool repeatHeaders;
        private bool repeatRowHeaders;
        private bool repeatColumnHeaders;
        private bool adjustSpannedCellsWidth;
        private bool lockCorrectSpans;
        private bool serializingToPreview;
        private bool lockColumnRowChange;
        private TableLayout layout;
        private List<Rectangle> spanList;
        private TableResult resultTable;
        private TableCellData printingCell;
        //private static float FLeftRtl;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a collection of table rows.
        /// </summary>
        [Browsable(false)]
        public TableRowCollection Rows
        {
            get { return rows; }
        }

        /// <summary>
        /// Gets a collection of table columns.
        /// </summary>
        [Browsable(false)]
        public TableColumnCollection Columns
        {
            get { return columns; }
        }

        internal TableStyleCollection Styles
        {
            get { return styles; }
        }

        /// <summary>
        /// Gets or sets the number of fixed rows that will be repeated on each page.
        /// </summary>
        [DefaultValue(0)]
        [Category("Layout")]
        public int FixedRows
        {
            get
            {
                int value = fixedRows;
                if (value >= Rows.Count)
                    value = Rows.Count - 1;
                if (value < 0)
                    value = 0;
                return value;
            }
            set { fixedRows = value; }
        }

        /// <summary>
        /// Gets or sets the number of fixed columns that will be repeated on each page.
        /// </summary>
        [DefaultValue(0)]
        [Category("Layout")]
        public int FixedColumns
        {
            get
            {
                int value = fixedColumns;
                if (value >= Columns.Count)
                    value = Columns.Count - 1;
                if (value < 0)
                    value = 0;
                return value;
            }
            set { fixedColumns = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether is necessary to repeat table header on each page.
        /// </summary>
        /// <remarks>
        /// To define a table header, set the <see cref="FixedRows"/> and <see cref="FixedColumns"/>
        /// properties.
        /// </remarks>
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool RepeatHeaders
        {
            get { return repeatHeaders; }
            set { repeatHeaders = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether is necessary to repeat table Row header on each page.
        /// </summary>
        /// <remarks>
        /// To define a table Row header, set the <see cref="FixedRows"/>
        /// properties.
        /// </remarks>
        [Browsable(false)]
        [DefaultValue(false)]
        [Category("Behavior")]
        public virtual bool RepeatRowHeaders
        {
            get { return repeatRowHeaders; }
            set { repeatRowHeaders = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether is necessary to repeat table Column header on each page.
        /// </summary>
        /// <remarks>
        /// To define a table Column header, set the <see cref="FixedColumns"/>
        /// properties.
        /// </remarks>
        [Browsable(false)]
        [DefaultValue(false)]
        [Category("Behavior")]
        public virtual bool RepeatColumnHeaders
        {
            get { return repeatColumnHeaders; }
            set { repeatColumnHeaders = value; }
        }

        /// <summary>
        /// Gets or sets the table layout.
        /// </summary>
        /// <remarks>
        /// This property affects printing the big table that breaks across pages.
        /// </remarks>
        [DefaultValue(TableLayout.AcrossThenDown)]
        [Category("Behavior")]
        public TableLayout Layout
        {
            get { return layout; }
            set { layout = value; }
        }

        /// <summary>
        /// Gets or sets gap between parts of the table in wrapped layout mode.
        /// </summary>
        /// <remarks>
        /// This property is used if you set the <see cref="Layout"/> property to <b>Wrapped</b>.
        /// </remarks>
        [DefaultValue(0f)]
        [Category("Behavior")]
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float WrappedGap
        {
            get { return wrappedGap; }
            set { wrappedGap = value; }
        }


        /// <summary>
        /// Gets or sets a value that determines whether to adjust the spanned cell's width when breaking the table across pages.
        /// </summary>
        /// <remarks>
        /// If set to <b>true</b>, the spanned cell's width will be adjusted to accomodate all contained text.
        /// </remarks>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool AdjustSpannedCellsWidth
        {
            get { return adjustSpannedCellsWidth; }
            set { adjustSpannedCellsWidth = value; }
        }

        /// <summary>
        /// Gets or sets the table cell.
        /// </summary>
        /// <param name="col">Column index.</param>
        /// <param name="row">Row index.</param>
        /// <returns>The <b>TableCell</b> object that represents a cell.</returns>
        [Browsable(false)]
        public TableCell this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= columns.Count || row < 0 || row >= rows.Count)
                    return null;
                return rows[row][col];
            }
            set
            {
                if (col < 0 || col >= columns.Count || row < 0 || row >= rows.Count)
                    return;
                rows[row][col] = value;
            }
        }

        /// <summary>
        /// Gets or sets a number of columns in the table.
        /// </summary>
        [Category("Appearance")]
        public virtual int ColumnCount
        {
            get { return Columns.Count; }
            set
            {
                int n = value - Columns.Count;
                for (int i = 0; i < n; i++)
                {
                    TableColumn column = new TableColumn();
                    Columns.Add(column);
                }
                while (value < Columns.Count)
                    Columns.RemoveAt(Columns.Count - 1);
            }
        }

        /// <summary>
        /// Gets or sets a number of rows in the table.
        /// </summary>
        [Category("Appearance")]
        public virtual int RowCount
        {
            get { return Rows.Count; }
            set
            {
                int n = value - Rows.Count;
                for (int i = 0; i < n; i++)
                {
                    TableRow row = new TableRow();
                    Rows.Add(row);
                }
                while (value < Rows.Count)
                    Rows.RemoveAt(Rows.Count - 1);
            }
        }

        internal bool IsResultTable
        {
            get { return this is TableResult; }
        }

        /// <summary>
        /// Gets a table which contains the result of rendering dynamic table.
        /// </summary>
        /// <remarks>
        /// Use this property to access the result of rendering your table in dynamic mode.
        /// It may be useful if you want to center or right-align the result table on a page. 
        /// In this case, you need to add the following code at the end of your ManualBuild event handler:
        /// <code>
        /// // right-align the table
        /// Table1.ResultTable.Left = Engine.PageWidth - Table1.ResultTable.CalcWidth() - 1;
        /// </code>
        /// </remarks>
        [Browsable(false)]
        public TableResult ResultTable
        {
            get { return resultTable; }
        }

        internal TableCellData PrintingCell
        {
            get { return printingCell; }
            set { printingCell = value; }
        }

        internal bool LockCorrectSpans
        {
            get { return lockCorrectSpans; }
            set { lockCorrectSpans = value; }
        }
        #endregion

        #region Private Methods
        private delegate void DrawCellProc(FRPaintEventArgs e, TableCell cell);

        private void DrawCells(FRPaintEventArgs e, DrawCellProc proc)
        {
            float top = 0;

            for (int y = 0; y < Rows.Count; y++)
            {
                float left = 0;
                float height = Rows[y].Height;

                for (int x = 0; x < Columns.Count; x++)
                {
                    TableCell cell = this[x, y];
                    float width = Columns[x].Width;

                    if (!IsInsideSpan(cell) && (!IsPrinting || cell.Printable))
                    {
                        cell.Left = left;
                        cell.Top = top;
                        cell.SetPrinting(IsPrinting);
                        proc(e, cell);
                    }

                    left += width;
                }
                top += height;
            }
        }

        private void DrawCellsRtl(FRPaintEventArgs e, DrawCellProc proc)
        {
            float top = 0;

            for (int y = 0; y < Rows.Count; y++)
            {
                float left = 0;
                float height = Rows[y].Height;

                //bool thereIsColSpan = false;
                //for (int i = Columns.Count - 1; i >= 0; i--)
                //{
                //    TableCell cell = this[i, y];
                //    if (cell.ColSpan > 1)
                //    {
                //        thereIsColSpan = true;
                //    }
                //}

                for (int x = Columns.Count - 1; x >= 0; x--)
                {
                    TableCell cell = this[x, y];

                    bool thereIsColSpan = false;
                    if (cell.ColSpan > 1)
                    {
                        thereIsColSpan = true;
                    }

                    float width = Columns[x].Width;

                    //if (thereIsColSpan)
                    //{
                    //    width *= cell.ColSpan - 1;
                    //    left -= width;
                    //}

                    if (!IsInsideSpan(cell) && (!IsPrinting || cell.Printable))
                    {
                        cell.Left = left;
                        cell.Top = top;
                        cell.SetPrinting(IsPrinting);
                        proc(e, cell);

                        if (thereIsColSpan)
                            width *= cell.ColSpan;

                        left += width;
                    }

                    //if (!thereIsColSpan)
                    //    left += width;
                    //else
                    //    left -= width;
                }

                top += height;
            }
        }

        private void DrawFill(FRPaintEventArgs e, TableCell cell)
        {
            cell.DrawBackground(e);
        }

        private void DrawText(FRPaintEventArgs e, TableCell cell)
        {
            cell.DrawText(e);
        }

        private void DrawBorders(FRPaintEventArgs e, TableCell cell)
        {
            cell.Border.Draw(e, cell.AbsBounds);
        }

        private void DrawTable(FRPaintEventArgs e)
        {
            DrawCells(e, DrawFill);
            DrawCells(e, DrawText);
            DrawDesign_Borders(e);

            DrawCells(e, DrawBorders);
            DrawDesign_SelectedCells(e);

        }

        private void DrawTableRtl(FRPaintEventArgs e)
        {
            DrawCellsRtl(e, DrawFill);
            DrawCellsRtl(e, DrawText);
            DrawDesign_BordersRtl(e);

            DrawCellsRtl(e, DrawBorders);

            DrawDesign_SelectedCellsRtl(e);

        }



        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            TableBase src = source as TableBase;
            FixedRows = src.FixedRows;
            FixedColumns = src.FixedColumns;
            RepeatHeaders = src.RepeatHeaders;
            RepeatRowHeaders = src.RepeatRowHeaders;
            RepeatColumnHeaders = src.RepeatColumnHeaders;
            Layout = src.Layout;
            WrappedGap = src.WrappedGap;
            AdjustSpannedCellsWidth = src.AdjustSpannedCellsWidth;
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            if (ColumnCount == 0 || RowCount == 0)
                return;

            lockColumnRowChange = true;
            Width = Columns[Columns.Count - 1].Right;
            Height = Rows[Rows.Count - 1].Bottom;
            lockColumnRowChange = false;

            base.Draw(e);

            // draw table Right to Left if needed
            if (Config.RightToLeft)
            {
                DrawTableRtl(e);
                // !! ����������� ������ !!
                //Border.Draw(e, new RectangleF(FLeftRtl - Width + AbsLeft, AbsTop, Width, Height));
                Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
            }
            else
            {
                DrawTable(e);
                Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
            }
            DrawDesign(e);
        }

        /// <inheritdoc/>
        public override bool IsVisible(FRPaintEventArgs e)
        {
            if (RowCount == 0 || ColumnCount == 0)
                return false;
            Width = Columns[Columns.Count - 1].Right;
            Height = Rows[Rows.Count - 1].Bottom;
            RectangleF objRect = new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY,
              Width * e.ScaleX + 1, Height * e.ScaleY + 1);
            return e.Graphics.IsVisible(objRect);
        }

        internal void SetResultTable(TableResult table)
        {
            resultTable = table;
        }

        /// <summary>
        /// Gets data of the table cell with specified column and row numbers.
        /// </summary>
        /// <param name="col">The column number.</param>
        /// <param name="row">The row number.</param>
        /// <returns>TableCellData instance containing data of the table cell.</returns>
        public TableCellData GetCellData(int col, int row)
        {
            if (col < 0 || col >= columns.Count || row < 0 || row >= rows.Count)
                return null;
            return rows[row].CellData(col);
        }

        internal List<Rectangle> GetSpanList()
        {
            if (spanList == null)
            {
                spanList = new List<Rectangle>();
                for (int y = 0; y < Rows.Count; y++)
                {
                    for (int x = 0; x < Columns.Count; x++)
                    {
                        TableCellData cell = GetCellData(x, y);
                        if (cell.ColSpan > 1 || cell.RowSpan > 1)
                            spanList.Add(new Rectangle(x, y, cell.ColSpan, cell.RowSpan));
                    }
                }
            }

            return spanList;
        }

        internal void ResetSpanList()
        {
            spanList = null;
        }

        internal void CorrectSpansOnRowChange(int rowIndex, int correct)
        {
            if (lockCorrectSpans || (correct == 1 && rowIndex >= Rows.Count))
                return;

            for (int y = 0; y < rowIndex; y++)
            {
                for (int x = 0; x < Columns.Count; x++)
                {
                    TableCellData cell = GetCellData(x, y);
                    if (rowIndex < y + cell.RowSpan)
                        cell.RowSpan += correct;
                }
            }

            ResetSpanList();
        }

        internal void CorrectSpansOnColumnChange(int columnIndex, int correct)
        {
            if (lockCorrectSpans || (correct == 1 && columnIndex >= Columns.Count))
                return;

            for (int y = 0; y < Rows.Count; y++)
            {
                for (int x = 0; x < columnIndex; x++)
                {
                    TableCellData cell = GetCellData(x, y);
                    if (columnIndex < x + cell.ColSpan)
                        cell.ColSpan += correct;
                }

                // correct cells
                Rows[y].CorrectCellsOnColumnChange(columnIndex, correct);
            }

            ResetSpanList();
        }
        public bool IsInsideSpan(TableCell cell)
        {
            Point address = cell.Address;
            List<Rectangle> spans = GetSpanList();
            foreach (Rectangle span in spans)
            {
                if (span.Contains(address) && span.Location != address)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Creates unique names for all table elements such as rows, columns, cells.
        /// </summary>
        public void CreateUniqueNames()
        {
            if (Report == null)
                return;
            FastNameCreator nameCreator = new FastNameCreator(Report.AllNamedObjects);

            foreach (TableRow row in Rows)
            {
                if (String.IsNullOrEmpty(row.Name))
                    nameCreator.CreateUniqueName(row);
            }
            foreach (TableColumn column in Columns)
            {
                if (String.IsNullOrEmpty(column.Name))
                    nameCreator.CreateUniqueName(column);
            }
            for (int y = 0; y < Rows.Count; y++)
            {
                for (int x = 0; x < Columns.Count; x++)
                {
                    TableCell cell = this[x, y];
                    if (String.IsNullOrEmpty(cell.Name))
                    {
                        nameCreator.CreateUniqueName(cell);
                        cell.Font = DrawUtils.DefaultReportFont;
                    }
                    if (cell.Objects != null)
                    {
                        foreach (ReportComponentBase obj in cell.Objects)
                        {
                            if (String.IsNullOrEmpty(obj.Name))
                                nameCreator.CreateUniqueName(obj);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            TableBase c = writer.DiffObject as TableBase;
            serializingToPreview = writer.SerializeTo == SerializeTo.Preview;
            base.Serialize(writer);

            if (FixedRows != c.FixedRows)
                writer.WriteInt("FixedRows", FixedRows);
            if (FixedColumns != c.FixedColumns)
                writer.WriteInt("FixedColumns", FixedColumns);
            if (RepeatHeaders != c.RepeatHeaders)
                writer.WriteBool("RepeatHeaders", RepeatHeaders);
            if (RepeatRowHeaders != c.RepeatRowHeaders)
                writer.WriteBool("RepeatRowHeaders", RepeatRowHeaders);
            if (RepeatColumnHeaders != c.RepeatColumnHeaders)
                writer.WriteBool("RepeatColumnHeaders", RepeatColumnHeaders);
            if (Layout != c.Layout)
                writer.WriteValue("Layout", Layout);
            if (WrappedGap != c.WrappedGap)
                writer.WriteFloat("WrappedGap", WrappedGap);
            if (AdjustSpannedCellsWidth != c.AdjustSpannedCellsWidth)
                writer.WriteBool("AdjustSpannedCellsWidth", AdjustSpannedCellsWidth);
        }

        internal void EmulateOuterBorder()
        {
            for (int y = 0; y < RowCount; y++)
            {
                for (int x = 0; x < ColumnCount; x++)
                {
                    TableCell cell = this[x, y];
                    if (x == 0 && (Border.Lines & BorderLines.Left) != 0)
                    {
                        cell.Border.LeftLine.Assign(Border.LeftLine);
                        cell.Border.Lines |= BorderLines.Left;
                    }
                    if (x + cell.ColSpan == ColumnCount && (Border.Lines & BorderLines.Right) != 0)
                    {
                        cell.Border.RightLine.Assign(Border.RightLine);
                        cell.Border.Lines |= BorderLines.Right;
                    }
                    if (y == 0 && (Border.Lines & BorderLines.Top) != 0)
                    {
                        cell.Border.TopLine.Assign(Border.TopLine);
                        cell.Border.Lines |= BorderLines.Top;
                    }
                    if (y + cell.RowSpan == RowCount && (Border.Lines & BorderLines.Bottom) != 0)
                    {
                        cell.Border.BottomLine.Assign(Border.BottomLine);
                        cell.Border.Lines |= BorderLines.Bottom;
                    }
                }
            }
        }
        #endregion

        #region IParent Members
        /// <inheritdoc/>
        public bool CanContain(Base child)
        {
            return child is TableRow || child is TableColumn || child is TableCell;
        }

        /// <inheritdoc/>
        public virtual void GetChildObjects(ObjectCollection list)
        {
            foreach (TableColumn column in Columns)
            {
                if (!serializingToPreview || column.Visible)
                    list.Add(column);
            }
            foreach (TableRow row in Rows)
            {
                if (!serializingToPreview || row.Visible)
                    list.Add(row);
            }
        }

        /// <inheritdoc/>
        public void AddChild(Base child)
        {
            if (child is TableRow)
                Rows.Add(child as TableRow);
            else if (child is TableColumn)
                Columns.Add(child as TableColumn);
        }

        /// <inheritdoc/>
        public void RemoveChild(Base child)
        {
            if (child is TableRow)
                Rows.Remove(child as TableRow);
            else if (child is TableColumn)
                Columns.Remove(child as TableColumn);
        }

        /// <inheritdoc/>
        public int GetChildOrder(Base child)
        {
            if (child is TableColumn)
                return Columns.IndexOf(child as TableColumn);
            else if (child is TableRow)
                return Rows.IndexOf(child as TableRow);
            return 0;
        }

        /// <inheritdoc/>
        public void SetChildOrder(Base child, int order)
        {
            lockCorrectSpans = true;

            int oldOrder = child.ZOrder;
            if (oldOrder != -1 && order != -1 && oldOrder != order)
            {
                if (child is TableColumn)
                {
                    if (order > Columns.Count)
                        order = Columns.Count;
                    if (oldOrder <= order)
                        order--;
                    Columns.Remove(child as TableColumn);
                    Columns.Insert(order, child as TableColumn);
                }
                else if (child is TableRow)
                {
                    if (order > Rows.Count)
                        order = Rows.Count;
                    if (oldOrder <= order)
                        order--;
                    Rows.Remove(child as TableRow);
                    Rows.Insert(order, child as TableRow);
                }
            }

            lockCorrectSpans = false;
        }

        /// <inheritdoc/>
        public void UpdateLayout(float dx, float dy)
        {
        }
        #endregion

        #region Report Engine
        /// <inheritdoc/>
        public override void SaveState()
        {
            base.SaveState();

            foreach (TableRow row in Rows)
            {
                row.SaveState();
            }
            foreach (TableColumn column in Columns)
            {
                column.SaveState();
            }
            for (int y = 0; y < Rows.Count; y++)
            {
                for (int x = 0; x < Columns.Count; x++)
                {
                    this[x, y].SaveState();
                }
            }

            CanGrow = true;
            CanShrink = true;
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            base.RestoreState();

            foreach (TableRow row in Rows)
            {
                row.RestoreState();
            }
            foreach (TableColumn column in Columns)
            {
                column.RestoreState();
            }
            for (int y = 0; y < Rows.Count; y++)
            {
                for (int x = 0; x < Columns.Count; x++)
                {
                    this[x, y].RestoreState();
                }
            }
        }

        /// <summary>
        /// Calculates and returns the table width, in pixels.
        /// </summary>
        public float CalcWidth()
        {
            // first pass, calc non-spanned cells
            for (int x = 0; x < Columns.Count; x++)
            {
                TableColumn column = Columns[x];
                if (!column.AutoSize)
                    continue;
                float columnWidth = IsDesigning ? 16 : -1;

                // calc the max column width
                for (int y = 0; y < Rows.Count; y++)
                {
                    TableCellData cell = GetCellData(x, y);
                    if (cell.ColSpan == 1)
                    {
                        float cellWidth = cell.CalcWidth();
                        if (cellWidth > columnWidth)
                            columnWidth = cellWidth;
                    }
                }

                // update column width
                if (columnWidth != -1)
                    column.Width = columnWidth;
            }

            // second pass, calc spanned cells
            for (int x = 0; x < Columns.Count; x++)
            {
                Columns[x].MinimumBreakWidth = 0;
                for (int y = 0; y < Rows.Count; y++)
                {
                    TableCellData cell = GetCellData(x, y);
                    if (cell.ColSpan > 1)
                    {
                        float cellWidth = cell.CalcWidth();
                        if (AdjustSpannedCellsWidth && cellWidth > Columns[x].MinimumBreakWidth)
                            Columns[x].MinimumBreakWidth = cellWidth;

                        // check that spanned columns have enough width
                        float columnsWidth = 0;
                        for (int i = 0; i < cell.ColSpan; i++)
                        {
                            columnsWidth += Columns[x + i].Width;
                        }

                        // if cell is bigger than sum of column width, increase the last column width
                        TableColumn lastColumn = Columns[x + cell.ColSpan - 1];
                        if (columnsWidth < cellWidth && lastColumn.AutoSize)
                            lastColumn.Width += cellWidth - columnsWidth;
                    }
                }
            }

            // finally, calculate the table width
            float width = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                width += Columns[i].Width;
            }

            lockColumnRowChange = true;
            Width = width;
            lockColumnRowChange = false;
            return width;
        }

        /// <inheritdoc/>
        public override float CalcHeight()
        {

            if (ColumnCount * RowCount > 1000)
                Config.ReportSettings.OnProgress(Report, Res.Get("ComponentsMisc,Table,CalcBounds"), 0, 0);

            // calc width
            CalcWidth();

            // first pass, calc non-spanned cells
            for (int y = 0; y < Rows.Count; y++)
            {
                TableRow row = Rows[y];
                if (!row.AutoSize)
                    continue;
                float rowHeight = IsDesigning ? 16 : -1;

                // calc the max row height
                for (int x = 0; x < Columns.Count; x++)
                {
                    TableCellData cell = GetCellData(x, y);

                    if (cell.RowSpan == 1)
                    {
                        float cellHeight = cell.CalcHeight(cell.Width);
                        if (cellHeight > rowHeight)
                            rowHeight = cellHeight;
                    }
                }

                // update row height
                if (rowHeight != -1)
                    row.Height = rowHeight;
            }

            // second pass, calc spanned cells
            for (int y = 0; y < Rows.Count; y++)
            {
                for (int x = 0; x < Columns.Count; x++)
                {
                    TableCellData cell = GetCellData(x, y);
                    if (cell.RowSpan > 1)
                    {
                        float cellHeight = cell.CalcHeight(cell.Width);

                        // check that spanned rows have enough height
                        float rowsHeight = 0;
                        for (int i = 0; i < cell.RowSpan; i++)
                        {
                            if (y + i < Rows.Count)
                                rowsHeight += Rows[y + i].Height;
                            else
                            {
                                // Error, we don't have row, rowSpan has incorrect
                                cell.RowSpan--;
                            }
                        }

                        // if cell is bigger than sum of row heights, increase the last row height
                        if (y + cell.RowSpan - 1 < Rows.Count)
                        {
                            TableRow lastRow = Rows[y + cell.RowSpan - 1];
                            if (rowsHeight < cellHeight && lastRow.AutoSize)
                                lastRow.Height += cellHeight - rowsHeight;
                        }
                    }
                }
            }

            // finally, calculate the table height
            float height = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                height += Rows[i].Visible ? Rows[i].Height : 0;
            }

            return height;
        }

        private bool CanBreakRow(int rowIndex, float rowHeight)
        {
            if (!Rows[rowIndex].CanBreak)
                return false;

            // check each cell in the row
            for (int i = 0; i < ColumnCount; i++)
            {
                TableCell breakable = this[i, rowIndex];
                // use clone object because Break method will modify the Text property
                using (TableCell clone = new TableCell())
                {
                    clone.AssignAll(breakable);
                    clone.Height = rowHeight;
                    clone.SetReport(Report);
                    if (!clone.Break(null))
                        return false;
                }
            }

            return true;
        }

        private void BreakRow(TableBase breakTo, int rowIndex, float rowHeight, float newRowHeight)
        {
            // set rows height
            TableRow rowTo = breakTo.Rows[rowIndex];
            Rows[rowIndex].Height = rowHeight;
            rowTo.Height = newRowHeight;

            // break each cell in the row
            for (int i = 0; i < ColumnCount; i++)
            {
                TableCell cell = this[i, rowIndex];
                TableCell cellTo = breakTo[i, rowIndex];
                cell.Height = rowHeight;
                cell.Break(cellTo);
                
                // fix height if row is not autosized
                if (!rowTo.AutoSize)
                {
                    float h = cellTo.CalcHeight();
                    if (h > rowTo.Height)
                        rowTo.Height = h;
                }
            }
        }

        /// <inheritdoc/>
        public override bool Break(BreakableComponent breakTo)
        {
            if (Rows.Count == 0)
                return true;
            if (Height < Rows[0].Height && !Rows[0].CanBreak)
                return false;
            TableBase tableTo = breakTo as TableBase;
            if (tableTo == null)
                return true;

            // find the break row index
            int breakRowIndex = 0;
            int breakRowIndexAdd = 0;
            bool rowBroken = false;
            float rowsHeight = 0;

            while (breakRowIndex < Rows.Count)
            {
                rowsHeight += Rows[breakRowIndex].Height;
                if (rowsHeight > Height)
                {
                    float breakRowHeight = Rows[breakRowIndex].Height - (rowsHeight - Height);
                    if (CanBreakRow(breakRowIndex, breakRowHeight))
                    {
                        BreakRow(tableTo, breakRowIndex, breakRowHeight, rowsHeight - Height);
                        breakRowIndexAdd = 1;
                        rowBroken = true;
                    }
                    break;
                }

                breakRowIndex++;
            }

            // get the span list
            List<Rectangle> spans = GetSpanList();

            // break the spans
            foreach (Rectangle span in spans)
            {
                if (span.Top < breakRowIndex + breakRowIndexAdd && span.Bottom > breakRowIndex)
                {
                    TableCell cell = this[span.Left, span.Top];
                    TableCell cellTo = tableTo[span.Left, span.Top];

                    // update cell spans
                    cell.RowSpan = breakRowIndex + breakRowIndexAdd - span.Top;
                    cellTo.RowSpan = span.Bottom - breakRowIndex;

                    // break the cell
                    if (!rowBroken && !cell.Break(cellTo))
                        cell.Text = "";

                    // set the top span cell to the correct place
                    tableTo[span.Left, span.Top] = new TableCell();
                    tableTo[span.Left, breakRowIndex] = cellTo;
                }
            }

            // remove unused rows from source (this table)
            while (breakRowIndex + breakRowIndexAdd < Rows.Count)
            {
                this.Rows.RemoveAt(Rows.Count - 1);
            }

            // remove unused rows from copy (tableTo)
            for (int i = 0; i < breakRowIndex; i++)
            {
                tableTo.Rows.RemoveAt(0);
            }

            return true;
        }

        private List<TableCellData> GetAggregateCells(TableCell aggregateCell)
        {
            List<TableCellData> list = new List<TableCellData>();

            // columnIndex, rowIndex is a place where we will print a result.
            // To collect aggregate values that will be used to calculate a result, we need to go
            // to the left and top from this point and collect every cell which OriginalCell is equal to
            // the aggregateCell value. We have to stop when we meet the same row or column.

            int columnIndex = PrintingCell.Address.X;
            int rowIndex = PrintingCell.Address.Y;
            TableColumn startColumn = ResultTable.Columns[columnIndex];
            TableRow startRow = ResultTable.Rows[rowIndex];
            TableColumn aggregateColumn = Columns[aggregateCell.Address.X];
            TableRow aggregateRow = Rows[aggregateCell.Address.Y];

            // check if result is in the same row/column as aggregate cell
            bool sameRow = startRow.OriginalComponent == aggregateRow.OriginalComponent;
            bool sameColumn = startColumn.OriginalComponent == aggregateColumn.OriginalComponent;

            for (int y = rowIndex; y >= 0; y--)
            {
                if (y != rowIndex && ResultTable.Rows[y].OriginalComponent == startRow.OriginalComponent)
                    break;

                for (int x = columnIndex; x >= 0; x--)
                {
                    if (x != columnIndex && ResultTable.Columns[x].OriginalComponent == startColumn.OriginalComponent)
                        break;

                    TableCellData cell = ResultTable.GetCellData(x, y);
                    if (cell.OriginalCell == aggregateCell)
                        list.Add(cell);

                    if (sameColumn)
                        break;
                }

                if (sameRow)
                    break;
            }

            return list;
        }

        /// <summary>
        /// Calculates a sum of values in a specified cell.
        /// </summary>
        /// <param name="aggregateCell">The cell.</param>
        /// <returns>The <b>object</b> that contains calculated value.</returns>
        /// <remarks>
        /// This method can be called from the <b>ManualBuild</b> event handler only.
        /// </remarks>
        public object Sum(TableCell aggregateCell)
        {
            List<TableCellData> list = GetAggregateCells(aggregateCell);
            Variant result = 0;
            bool firstTime = true;

            foreach (TableCellData cell in list)
            {
                if (cell.Value != null)
                {
                    Variant varValue = new Variant(cell.Value);
                    if (firstTime)
                        result = varValue;
                    else
                        result += varValue;

                    firstTime = false;
                }
            }

            return result.Value;
        }

        /// <summary>
        /// Calculates a minimum of values in a specified cell.
        /// </summary>
        /// <param name="aggregateCell">The cell.</param>
        /// <returns>The <b>object</b> that contains calculated value.</returns>
        /// <remarks>
        /// This method can be called from the <b>ManualBuild</b> event handler only.
        /// </remarks>
        public object Min(TableCell aggregateCell)
        {
            List<TableCellData> list = GetAggregateCells(aggregateCell);
            Variant result = float.PositiveInfinity;
            bool firstTime = true;

            foreach (TableCellData cell in list)
            {
                if (cell.Value != null)
                {
                    Variant varValue = new Variant(cell.Value);
                    if (firstTime || varValue < result)
                        result = varValue;

                    firstTime = false;
                }
            }

            return result.Value;
        }

        /// <summary>
        /// Calculates a maximum of values in a specified cell.
        /// </summary>
        /// <param name="aggregateCell">The cell.</param>
        /// <returns>The <b>object</b> that contains calculated value.</returns>
        /// <remarks>
        /// This method can be called from the <b>ManualBuild</b> event handler only.
        /// </remarks>
        public object Max(TableCell aggregateCell)
        {
            List<TableCellData> list = GetAggregateCells(aggregateCell);
            Variant result = float.NegativeInfinity;
            bool firstTime = true;

            foreach (TableCellData cell in list)
            {
                if (cell.Value != null)
                {
                    Variant varValue = new Variant(cell.Value);
                    if (firstTime || varValue > result)
                        result = varValue;

                    firstTime = false;
                }
            }

            return result.Value;
        }

        /// <summary>
        /// Calculates an average of values in a specified cell.
        /// </summary>
        /// <param name="aggregateCell">The cell.</param>
        /// <returns>The <b>object</b> that contains calculated value.</returns>
        /// <remarks>
        /// This method can be called from the <b>ManualBuild</b> event handler only.
        /// </remarks>
        public object Avg(TableCell aggregateCell)
        {
            List<TableCellData> list = GetAggregateCells(aggregateCell);
            Variant result = 0;
            int count = 0;
            bool firstTime = true;

            foreach (TableCellData cell in list)
            {
                if (cell.Value != null)
                {
                    Variant varValue = new Variant(cell.Value);
                    if (firstTime)
                        result = varValue;
                    else
                        result += varValue;

                    count++;
                    firstTime = false;
                }
            }

            return result / (count == 0 ? 1 : count);
        }

        /// <summary>
        /// Calculates number of repeats of a specified cell.
        /// </summary>
        /// <param name="aggregateCell">The cell.</param>
        /// <returns>The <b>object</b> that contains calculated value.</returns>
        /// <remarks>
        /// This method can be called from the <b>ManualBuild</b> event handler only.
        /// </remarks>
        public object Count(TableCell aggregateCell)
        {
            List<TableCellData> list = GetAggregateCells(aggregateCell);
            int count = 0;

            foreach (TableCellData cell in list)
            {
                if (cell.Value != null)
                    count++;
            }

            return count;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TableBase"/> class.
        /// </summary>
        public TableBase()
        {
            rows = new TableRowCollection(this);
            columns = new TableColumnCollection(this);
            styles = new TableStyleCollection();
            repeatHeaders = true;
            repeatRowHeaders = false;
            repeatColumnHeaders = false;
            CanGrow = true;
            CanShrink = true;
        }
    }
}