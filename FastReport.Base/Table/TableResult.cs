using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Engine;
using FastReport.Preview;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Table
{
    /// <summary>
    /// Represents a result table.
    /// </summary>
    /// <remarks>
    /// Do not use this class directly. It is used by the <see cref="TableObject"/> and 
    /// <see cref="FastReport.Matrix.MatrixObject"/> objects to render a result.
    /// </remarks>
    public class TableResult : TableBase
    {
        private bool skip;
        private List<TableRow> rowsToSerialize;
        private List<TableColumn> columnsToSerialize;
        private bool isFirstRow;

        /// <summary>
        /// Occurs after calculation of table bounds.
        /// </summary>
        /// <remarks>
        /// You may use this event to change automatically calculated rows/column sizes. It may be useful
        /// if you need to fit dynamically printed table on a page.
        /// </remarks>
        public event EventHandler AfterCalcBounds;

        internal bool Skip
        {
            get { return skip; }
            set { skip = value; }
        }

        internal List<TableRow> RowsToSerialize
        {
            get { return rowsToSerialize; }
        }

        internal List<TableColumn> ColumnsToSerialize
        {
            get { return columnsToSerialize; }
        }

        private float GetRowsHeight(int startRow, int count)
        {
            float height = 0;

            // include row header
            if (startRow != 0 && (RepeatHeaders || RepeatColumnHeaders))
            {
                for (int i = 0; i < FixedRows; i++)
                {
                    height += Rows[i].Height;
                }
            }

            for (int i = 0; i < count; i++)
            {
                height += Rows[startRow + i].Height;
            }

            return height;
        }

        private float GetColumnsWidth(int startColumn, int count)
        {
            float width = 0;

            // include column header
            if (startColumn != 0 && (RepeatHeaders || RepeatRowHeaders))
            {
                for (int i = 0; i < FixedColumns; i++)
                {
                    width += Columns[i].Width;
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (i == count - 1)
                    width += Math.Max(Columns[startColumn + i].Width, Columns[startColumn + i].MinimumBreakWidth);
                else
                    width += Columns[startColumn + i].Width;
            }

            return width;
        }

        private int GetRowsFit(int startRow, float freeSpace)
        {
            int rowsFit = 0;
            int rowsToKeep = 0;
            int rowsKept = 0;
            int saveRowsFit = 0;
            bool keeping = false;

            while (startRow + rowsFit < Rows.Count &&
              (rowsFit == 0 || !Rows[startRow + rowsFit].PageBreak) &&
              (!this.CanBreak | GetRowsHeight(startRow, rowsFit + 1) <= freeSpace + 0.1f))
            {
                if (keeping)
                {
                    rowsKept++;
                    if (rowsKept >= rowsToKeep)
                        keeping = false;
                }
                else if (Rows[startRow + rowsFit].KeepRows > 1)
                {
                    rowsToKeep = Rows[startRow + rowsFit].KeepRows;
                    rowsKept = 1;
                    saveRowsFit = rowsFit;
                    keeping = true;
                }
                rowsFit++;
            }

            if (keeping)
                rowsFit = saveRowsFit;
            // case if the row header does not fit on a page (at the start of table)
            if (startRow == 0 && rowsFit < FixedRows)
                rowsFit = 0;

            return rowsFit;
        }

        private int GetColumnsFit(int startColumn, float freeSpace)
        {
            int columnsFit = 0;
            int columnsToKeep = 0;
            int columnsKept = 0;
            int saveColumnsFit = 0;
            bool keeping = false;

            while (startColumn + columnsFit < Columns.Count &&
              (columnsFit == 0 || !Columns[startColumn + columnsFit].PageBreak) &&
              GetColumnsWidth(startColumn, columnsFit + 1) <= freeSpace + 0.1f)
            {
                if (keeping)
                {
                    columnsKept++;
                    if (columnsKept >= columnsToKeep)
                        keeping = false;
                }
                else if (Columns[startColumn + columnsFit].KeepColumns > 1)
                {
                    columnsToKeep = Columns[startColumn + columnsFit].KeepColumns;
                    columnsKept = 1;
                    saveColumnsFit = columnsFit;
                    keeping = true;
                }
                columnsFit++;
            }

            if (keeping)
                columnsFit = saveColumnsFit;
            return columnsFit;
        }

        internal void GeneratePages(object sender, EventArgs e)
        {
            isFirstRow = false;
            if (Skip)
            {
                Skip = false;
                return;
            }


            // check if band contains several tables
            if (sender is BandBase)
            {
                BandBase senderBand = sender as BandBase;
                isFirstRow = senderBand.IsFirstRow;
                SortedList<float, TableBase> tables = new SortedList<float, TableBase>();
                foreach (Base obj in senderBand.Objects)
                {
                    TableBase table = obj as TableBase;
                    if (table != null && table.ResultTable != null)
                        try
                        {
                            tables.Add(table.Left, table);
                        }
                        catch (ArgumentException)
                        {
                            throw new ArgumentException(Res.Get("Messages,MatrixLayoutError"));
                        }
                }

                // render tables side-by-side
                if (tables.Count > 1)
                {
                    ReportEngine engine = Report.Engine;
                    TableLayoutInfo info = new TableLayoutInfo();
                    info.startPage = engine.CurPage;
                    info.tableSize = new Size(1, 1);
                    info.startX = tables.Values[0].Left;

                    int startPage = info.startPage;
                    float saveCurY = engine.CurY;
                    int maxPage = 0;
                    float maxCurY = 0;

                    for (int i = 0; i < tables.Count; i++)
                    {
                        TableBase table = tables.Values[i];

                        // do not allow table to render itself in the band.AfterPrint event
                        table.ResultTable.Skip = true;
                        // render using the down-then-across mode
                        table.Layout = TableLayout.DownThenAcross;

                        engine.CurPage = info.startPage + (info.tableSize.Width - 1) * info.tableSize.Height;
                        engine.CurY = saveCurY;
                        float addLeft = 0;
                        if (i > 0)
                            addLeft = table.Left - tables.Values[i - 1].Right;
                        table.ResultTable.Left = info.startX + addLeft;

                        // calculate cells' bounds
                        table.ResultTable.CalcBounds();
                        // generate pages
                        Report.PreparedPages.AddPageAction = AddPageAction.WriteOver;
                        info = table.ResultTable.GeneratePagesDownThenAcross();

                        if (engine.CurPage > maxPage)
                        {
                            maxPage = engine.CurPage;
                            maxCurY = engine.CurY;
                        }
                        else if (engine.CurPage == maxPage && engine.CurY > maxCurY)
                        {
                            maxCurY = engine.CurY;
                        }
                    }

                    engine.CurPage = maxPage;
                    engine.CurY = maxCurY;

                    Skip = false;
                    return;
                }
            }

            // calculate cells' bounds
            CalcBounds();

            if (Report.Engine.UnlimitedHeight || Report.Engine.UnlimitedWidth)
            {
                if (!Report.Engine.UnlimitedWidth)
                    GeneratePagesWrapped();
                else if (!Report.Engine.UnlimitedHeight)
                    GeneratePagesDownThenAcross();
                else
                    GeneratePagesAcrossThenDown();
            }
            else if (Layout == TableLayout.AcrossThenDown)
                GeneratePagesAcrossThenDown();
            else if (Layout == TableLayout.DownThenAcross)
                GeneratePagesDownThenAcross();
            else
                GeneratePagesWrapped();
        }

        internal void CalcBounds()
        {
            // allow row/column manipulation from a script
            LockCorrectSpans = false;

            // fire AfterData event
            OnAfterData();

            // calculate cells' bounds
            Height = CalcHeight();

            // fire AfterCalcBounds event
            OnAfterCalcBounds();
        }

        private void OnAfterCalcBounds()
        {
            if (AfterCalcBounds != null)
                AfterCalcBounds(this, EventArgs.Empty);
        }

        private void GeneratePagesAcrossThenDown()
        {
            ReportEngine engine = Report.Engine;
            PreparedPages preparedPages = Report.PreparedPages;
            preparedPages.CanUploadToCache = false;
            preparedPages.AddPageAction = AddPageAction.WriteOver;

            List<Rectangle> spans = GetSpanList();
            int startRow = 0;
            bool addNewPage = false;
            float freeSpace = engine.FreeSpace;
            Top = 0;

            while (startRow < Rows.Count)
            {
                if (addNewPage)
                {
                    engine.StartNewPage();
                    freeSpace = engine.FreeSpace;
                }

                int startColumn = 0;
                int rowsFit = GetRowsFit(startRow, freeSpace);
                if (startRow == 0 && engine.IsKeeping && rowsFit < RowCount && isFirstRow && engine.KeepCurY > 0)
                {
                    engine.EndColumn();
                    freeSpace = engine.FreeSpace;
                    rowsFit = GetRowsFit(startRow, freeSpace);
                }
                // avoid the infinite loop if there is not enough space for one row
                if (startRow > 0 && rowsFit == 0)
                    rowsFit = 1;

                int saveCurPage = engine.CurPage;
                float saveLeft = Left;
                float saveCurY = engine.CurY;
                float curY = engine.CurY;

                if (rowsFit != 0)
                {
                    while (startColumn < Columns.Count)
                    {
                        int columnsFit = GetColumnsFit(startColumn, engine.PageWidth - Left);
                        // avoid the infinite loop if there is not enough space for one column
                        if (startColumn > 0 && columnsFit == 0)
                            columnsFit = 1;

                        engine.CurY = saveCurY;
                        curY = GeneratePage(startColumn, startRow, columnsFit, rowsFit,
                            new RectangleF(0, 0, engine.PageWidth, CanBreak ? freeSpace : Height), spans) + saveCurY;

                        Left = 0;
                        startColumn += columnsFit;
                        if (startColumn < Columns.Count)
                        {
                            // if we have something to print, start a new page
                            engine.StartNewPage();
                        }
                        else if (engine.CurPage > saveCurPage)
                        {
                            // finish the last printed page in case it is not the start page
                            engine.EndPage(false);
                        }

                        if (Report.Aborted)
                            break;
                    }
                }

                startRow += rowsFit;
                Left = saveLeft;
                engine.CurPage = saveCurPage;
                engine.CurY = curY;
                preparedPages.AddPageAction = AddPageAction.Add;
                addNewPage = true;

                if (Report.Aborted)
                    break;
            }
        }

        private TableLayoutInfo GeneratePagesDownThenAcross()
        {
            ReportEngine engine = Report.Engine;
            PreparedPages preparedPages = Report.PreparedPages;
            preparedPages.CanUploadToCache = false;

            TableLayoutInfo info = new TableLayoutInfo();
            info.startPage = engine.CurPage;
            List<Rectangle> spans = GetSpanList();
            int startColumn = 0;
            bool addNewPage = false;
            float saveCurY = engine.CurY;
            float lastCurY = 0;
            int lastPage = 0;
            Top = 0;

            while (startColumn < Columns.Count)
            {
                if (addNewPage)
                    engine.StartNewPage();

                int startRow = 0;
                int columnsFit = GetColumnsFit(startColumn, engine.PageWidth - Left);
                // avoid the infinite loop if there is not enough space for one column
                if (startColumn > 0 && columnsFit == 0)
                    columnsFit = 1;

                engine.CurY = saveCurY;
                info.tableSize.Width++;
                info.tableSize.Height = 0;

                if (columnsFit > 0)
                {
                    while (startRow < Rows.Count)
                    {
                        int rowsFit = GetRowsFit(startRow, engine.FreeSpace);
                        if (startRow == 0 && engine.IsKeeping && rowsFit < RowCount && isFirstRow && engine.KeepCurY > 0)
                        {
                            engine.EndColumn();

                            rowsFit = GetRowsFit(startRow, engine.FreeSpace);
                        }
                        // avoid the infinite loop if there is not enough space for one row
                        if (startRow > 0 && rowsFit == 0)
                            rowsFit = 1;

                        engine.CurY += GeneratePage(startColumn, startRow, columnsFit, rowsFit,
                          new RectangleF(0, 0, engine.PageWidth, engine.FreeSpace), spans);
                        info.tableSize.Height++;

                        startRow += rowsFit;
                        if (startRow < Rows.Count)
                        {
                            // if we have something to print, start a new page
                            engine.StartNewPage();
                        }
                        else if (startColumn > 0)
                        {
                            // finish the last printed page in case it is not a start page
                            engine.EndPage(false);
                        }

                        if (Report.Aborted)
                            break;
                    }
                }

                info.startX = Left + GetColumnsWidth(startColumn, columnsFit);
                startColumn += columnsFit;
                Left = 0;
                preparedPages.AddPageAction = AddPageAction.Add;
                addNewPage = true;

                if (lastPage == 0)
                {
                    lastPage = engine.CurPage;
                    lastCurY = engine.CurY;
                }

                if (Report.Aborted)
                    break;
            }

            engine.CurPage = lastPage;
            engine.CurY = lastCurY;
            return info;
        }

        private void GeneratePagesWrapped()
        {
            ReportEngine engine = Report.Engine;
            PreparedPages preparedPages = Report.PreparedPages;
            preparedPages.CanUploadToCache = false;

            List<Rectangle> spans = GetSpanList();
            int startColumn = 0;
            Top = 0;

            while (startColumn < Columns.Count)
            {
                int startRow = 0;
                int columnsFit = GetColumnsFit(startColumn, engine.PageWidth - Left);
                // avoid the infinite loop if there is not enough space for one column
                if (startColumn > 0 && columnsFit == 0)
                    columnsFit = 1;

                while (startRow < Rows.Count)
                {
                    int rowsFit = GetRowsFit(startRow, engine.FreeSpace);
                    if (startRow == 0 && engine.IsKeeping && rowsFit < RowCount && isFirstRow && engine.KeepCurY > 0)
                    {
                        engine.EndColumn();

                        rowsFit = GetRowsFit(startRow, engine.FreeSpace);
                    }
                    if (rowsFit == 0)
                    {
                        engine.StartNewPage();
                        rowsFit = GetRowsFit(startRow, engine.FreeSpace);
                    }

                    engine.CurY += GeneratePage(startColumn, startRow, columnsFit, rowsFit,
                      new RectangleF(0, 0, engine.PageWidth, engine.FreeSpace), spans);

                    startRow += rowsFit;

                    if (Report.Aborted)
                        break;
                }

                startColumn += columnsFit;
                if (startColumn < Columns.Count)
                    engine.CurY += WrappedGap;

                if (Report.Aborted)
                    break;
            }
        }

        private void AdjustSpannedCellWidth(TableCellData cell)
        {
            if (!AdjustSpannedCellsWidth)
                return;

            // check that spanned cell has enough width
            float columnsWidth = 0;
            for (int i = 0; i < cell.ColSpan; i++)
            {
                columnsWidth += Columns[cell.Address.X + i].Width;
            }
            // if cell is bigger than sum of column width, increase the last column width
            float cellWidth = cell.CalcWidth();
            if (columnsWidth < cellWidth)
                Columns[cell.Address.X + cell.ColSpan - 1].Width += cellWidth - columnsWidth;
        }

        private float GeneratePage(int startColumn, int startRow, int columnsFit, int rowsFit,
          RectangleF bounds, List<Rectangle> spans)
        {
            // break spans
            foreach (Rectangle span in spans)
            {
                TableCellData spannedCell = GetCellData(span.Left, span.Top);
                TableCellData newSpannedCell = null;
                if (span.Left < startColumn && span.Right > startColumn)
                {
                    if ((RepeatHeaders || RepeatRowHeaders) && span.Left < FixedColumns)
                    {
                        spannedCell.ColSpan = Math.Min(span.Right, startColumn + columnsFit) - startColumn + FixedColumns;
                    }
                    else
                    {
                        newSpannedCell = GetCellData(startColumn, span.Top);
                        newSpannedCell.RunTimeAssign(spannedCell.Cell, true);
                        newSpannedCell.ColSpan = Math.Min(span.Right, startColumn + columnsFit) - startColumn;
                        newSpannedCell.RowSpan = spannedCell.RowSpan;
                        AdjustSpannedCellWidth(newSpannedCell);
                    }
                }
                if (span.Left < startColumn + columnsFit && span.Right > startColumn + columnsFit)
                {
                    spannedCell.ColSpan = startColumn + columnsFit - span.Left;
                    AdjustSpannedCellWidth(spannedCell);
                }
                if (span.Top < startRow && span.Bottom > startRow)
                {
                    if ((RepeatHeaders || RepeatColumnHeaders) && span.Top < FixedRows)
                        spannedCell.RowSpan = Math.Min(span.Bottom, startRow + rowsFit) - startRow + FixedRows;
                }
                if (span.Top < startRow + rowsFit && span.Bottom > startRow + rowsFit)
                {
                    spannedCell.RowSpan = startRow + rowsFit - span.Top;

                    newSpannedCell = GetCellData(span.Left, startRow + rowsFit);
                    newSpannedCell.RunTimeAssign(spannedCell.Cell, true);
                    newSpannedCell.ColSpan = spannedCell.ColSpan;
                    newSpannedCell.RowSpan = span.Bottom - (startRow + rowsFit);

                    // break the cell text
                    TableCell cell = spannedCell.Cell;
                    using (TextObject tempObject = new TextObject())
                    {
                        if (!cell.Break(tempObject))
                            cell.Text = "";
                        if (cell.CanBreak)
                            newSpannedCell.Text = tempObject.Text;
                    }

                    // fix the row height
                    float textHeight = newSpannedCell.Cell.CalcHeight();
                    float rowsHeight = 0;
                    for (int i = 0; i < newSpannedCell.RowSpan; i++)
                    {
                        rowsHeight += Rows[i + startRow + rowsFit].Height;
                    }

                    if (rowsHeight < textHeight)
                    {
                        // fix the last row's height
                        Rows[startRow + rowsFit + newSpannedCell.RowSpan - 1].Height += textHeight - rowsHeight;
                    }
                }
            }

            // set visible columns
            ColumnsToSerialize.Clear();
            if (RepeatHeaders || RepeatRowHeaders)
            {
                for (int i = 0; i < FixedColumns; i++)
                {
                    if (Columns[i].Visible)
                        ColumnsToSerialize.Add(Columns[i]);
                }
                if (startColumn < FixedColumns)
                {
                    columnsFit -= FixedColumns - startColumn;
                    startColumn = FixedColumns;
                }
            }

            // calc visible columns and last X coordinate of table for unlimited page width
            float tableEndX = Columns[0].Width;
            for (int i = startColumn; i < startColumn + columnsFit; i++)
            {
                if (Columns[i].Visible)
                {
                    ColumnsToSerialize.Add(Columns[i]);
                    tableEndX += Columns[i].Width;
                }
            }

            // set visible rows
            RowsToSerialize.Clear();
            if (RepeatHeaders || RepeatColumnHeaders)
            {
                for (int i = 0; i < FixedRows; i++)
                {
                    if (Rows[i].Visible)
                        RowsToSerialize.Add(Rows[i]);
                }
                if (startRow < FixedRows)
                {
                    rowsFit -= FixedRows - startRow;
                    startRow = FixedRows;
                }
            }

            // calc visible rows and last Y coordinate of table for unlimited page height
            float tableEndY = Rows[0].Top;
            for (int i = startRow; i < startRow + rowsFit; i++)
            {
                if (Rows[i].Visible)
                {
                    RowsToSerialize.Add(Rows[i]);
                    tableEndY += Rows[i].Height;
                }
            }
            // include row header
            if (startRow != 0 && (RepeatHeaders || RepeatColumnHeaders))
            {
                for (int i = 0; i < FixedRows; i++)
                {
                    tableEndY += Rows[i].Height;
                }
            }


            // generate unlimited page
            if (Report.Engine.UnlimitedHeight || Report.Engine.UnlimitedWidth)
            {
                ReportPage page = Report.PreparedPages.GetPage(Report.Engine.CurPage);
                if (Report.Engine.UnlimitedHeight)
                {
                    bounds.Height = tableEndY;
                }
                if (Report.Engine.UnlimitedWidth)
                {
                    bounds.Width = tableEndX;
                }
                Report.PreparedPages.ModifyPage(Report.Engine.CurPage, page);
            }

            DataBand band = new DataBand();
            band.Bounds = bounds;
            band.Objects.Add(this);
            Report.Engine.AddToPreparedPages(band);

            return GetRowsHeight(startRow, rowsFit);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            LockCorrectSpans = true;
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        public override void GetChildObjects(ObjectCollection list)
        {
            foreach (TableColumn column in ColumnsToSerialize)
            {
                list.Add(column);
            }
            foreach (TableRow row in RowsToSerialize)
            {
                list.Add(row);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TableResult"/> class.
        /// </summary>
        public TableResult()
        {
            LockCorrectSpans = true;
            rowsToSerialize = new List<TableRow>();
            columnsToSerialize = new List<TableColumn>();
        }


        private class TableLayoutInfo
        {
            public int startPage;
            public Size tableSize;
            public float startX;
        }
    }
}
