using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FastReport.Table
{
  internal class TableHelper
  {
    private TableObject sourceTable;
    private TableResult resultTable;

    private enum NowPrinting { None, Row, Column }
    private NowPrinting nowPrinting;
    private bool rowsPriority;
    private int originalRowIndex;
    private int originalColumnIndex;
    private int printingRowIndex;
    private int printingColumnIndex;
    private List<SpanData> columnSpans;
    private List<SpanData> rowSpans;
    private bool pageBreak;

    private bool AutoSpans
    {
      get { return sourceTable.ManualBuildAutoSpans; }
    }

    #region Build the Table
    public void PrintRow(int rowIndex)
    {
      originalRowIndex = rowIndex;

      if (nowPrinting == NowPrinting.None)
      {
        // we are at the start. Rows will now have priority over columns.
        rowsPriority = true;
      }

      if (rowsPriority)
      {
        switch (nowPrinting)
        {
          case NowPrinting.None:
            printingRowIndex = 0;
            break;

          case NowPrinting.Column:
            printingRowIndex++;
            break;

          case NowPrinting.Row:
            // we have two sequential calls of the PrintRow. But we must print
            // some columns...
            break;
        }

        // add new row, do not copy cells: it will be done in the PrintColumn.
        TableRow row = new TableRow();
        row.Assign(sourceTable.Rows[rowIndex]);
        row.PageBreak = pageBreak;
        resultTable.Rows.Add(row);

        columnSpans.Clear();
      }
      else
      {
        if (nowPrinting == NowPrinting.Column)
        {
          // this is the first row inside a column, reset the index
          printingRowIndex = 0;
        }
        else
        {
          // not the first row, increment the index
          printingRowIndex++;
        }

        TableRow row = null;
        if (resultTable.Rows.Count <= printingRowIndex)
        {
          // index is outside existing rows. Probably not all rows created yet, 
          // we're at the start. Add new row.
          row = new TableRow();
          row.Assign(sourceTable.Rows[rowIndex]);
          resultTable.Rows.Add(row);
        }
        else
        {
          // do not create row, use existing one
          row = resultTable.Rows[printingRowIndex];
        }
        // apply page break
        row.PageBreak = pageBreak;

        // copy cells from the template to the result
        CopyCells(originalColumnIndex, originalRowIndex,
          printingColumnIndex, printingRowIndex);
      }

      nowPrinting = NowPrinting.Row;
      pageBreak = false;
    }

    public void PrintColumn(int columnIndex)
    {
      originalColumnIndex = columnIndex;

      if (nowPrinting == NowPrinting.None)
      {
        // we are at the start. Columns will now have priority over rows.
        rowsPriority = false;
      }

      if (!rowsPriority)
      {
        switch (nowPrinting)
        {
          case NowPrinting.None:
            printingColumnIndex = 0;
            break;

          case NowPrinting.Column:
            // we have two sequential calls of the PrintColumn. But we must print
            // some rows...
            break;

          case NowPrinting.Row:
            printingColumnIndex++;
            break;
        }

        // add new column, do not copy cells: it will be done in the PrintRow.
        TableColumn column = new TableColumn();
        column.Assign(sourceTable.Columns[columnIndex]);
        column.PageBreak = pageBreak;
        resultTable.Columns.Add(column);

        rowSpans.Clear();
      }
      else
      {
        if (nowPrinting == NowPrinting.Row)
        {
          // this is the first column inside a row, reset the index
          printingColumnIndex = 0;
        }
        else
        {
          // not the first column, increment the index
          printingColumnIndex++;
        }

        TableColumn column = null;
        if (resultTable.Columns.Count <= printingColumnIndex)
        {
          // index is outside existing columns. Probably not all columns 
          // created yet, we're at the start. Add new column.
          column = new TableColumn();
          column.Assign(sourceTable.Columns[columnIndex]);
          resultTable.Columns.Add(column);
        }
        else
        {
          // do not create column, use existing one
          column = resultTable.Columns[printingColumnIndex];
        }
        // apply page break
        column.PageBreak = pageBreak;

        // copy cells from the template to the result
        CopyCells(originalColumnIndex, originalRowIndex,
          printingColumnIndex, printingRowIndex);
      }

      nowPrinting = NowPrinting.Column;
      pageBreak = false;
    }

    public void PageBreak()
    {
      pageBreak = true;
    }

    private void CopyCells(int originalColumnIndex, int originalRowIndex,
      int resultColumnIndex, int resultRowIndex)
    {
      TableCell cell = sourceTable[originalColumnIndex, originalRowIndex];
      TableCellData cellTo = resultTable.GetCellData(resultColumnIndex, resultRowIndex);
      sourceTable.PrintingCell = cellTo;
      bool needData = true;

      if (AutoSpans)
      {
        if (rowsPriority)
        {
          // We are printing columns inside a row. Check if we need to finish the column cell.
          if (columnSpans.Count > 0)
          {
            SpanData spanData = columnSpans[0];

            // check if we are printing the last column of the cell's span. From now, we will not accept 
            // the first column.
            if (originalColumnIndex == spanData.originalCellOrigin.X + spanData.originalCell.ColSpan - 1)
              spanData.finishFlag = true;

            if ((spanData.finishFlag && originalColumnIndex == spanData.originalCellOrigin.X) ||
              (originalColumnIndex < spanData.originalCellOrigin.X ||
               originalColumnIndex > spanData.originalCellOrigin.X + spanData.originalCell.ColSpan - 1))
              columnSpans.Clear();
            else
            {
              spanData.resultCell.ColSpan++;
              needData = false;
            }
          }

          // add the column cell if it has ColSpan > 1
          if (cell.ColSpan > 1 && columnSpans.Count == 0)
          {
            SpanData spanData = new SpanData();
            columnSpans.Add(spanData);

            spanData.originalCell = cell;
            spanData.resultCell = cellTo;
            spanData.originalCellOrigin = new Point(originalColumnIndex, originalRowIndex);
            spanData.resultCellOrigin = new Point(resultColumnIndex, resultRowIndex);
          }

          // now check the row cells. Do this once for each row.
          if (printingColumnIndex == 0)
          {
            for (int i = 0; i < rowSpans.Count; i++)
            {
              SpanData spanData = rowSpans[i];

              // check if we are printing the last row of the cell's span. From now, we will not accept 
              // the first row.
              if (originalRowIndex == spanData.originalCellOrigin.Y + spanData.originalCell.RowSpan - 1)
                spanData.finishFlag = true;

              if ((spanData.finishFlag && originalRowIndex == spanData.originalCellOrigin.Y) ||
                (originalRowIndex < spanData.originalCellOrigin.Y ||
                 originalRowIndex > spanData.originalCellOrigin.Y + spanData.originalCell.RowSpan - 1))
              {
                rowSpans.RemoveAt(i);
                i--;
              }
              else
                spanData.resultCell.RowSpan++;
            }
          }

          // check if we should skip current cell because it is inside a span
          for (int i = 0; i < rowSpans.Count; i++)
          {
            SpanData spanData = rowSpans[i];

            if (resultColumnIndex >= spanData.resultCellOrigin.X &&
              resultColumnIndex <= spanData.resultCellOrigin.X + spanData.resultCell.ColSpan - 1 &&
              resultRowIndex >= spanData.resultCellOrigin.Y &&
              resultRowIndex <= spanData.resultCellOrigin.Y + spanData.resultCell.RowSpan)
            {
              needData = false;
              break;
            }
          }

          // add the row cell if it has RowSpan > 1 and not added yet
          if (cell.RowSpan > 1 && needData)
          {
            SpanData spanData = new SpanData();
            rowSpans.Add(spanData);

            spanData.originalCell = cell;
            spanData.resultCell = cellTo;
            spanData.originalCellOrigin = new Point(originalColumnIndex, originalRowIndex);
            spanData.resultCellOrigin = new Point(resultColumnIndex, resultRowIndex);
          }
        }
        else
        {
          // We are printing rows inside a column. Check if we need to finish the row cell.
          if (rowSpans.Count > 0)
          {
            SpanData spanData = rowSpans[0];

            // check if we are printing the last row of the cell's span. From now, we will not accept 
            // the first row.
            if (originalRowIndex == spanData.originalCellOrigin.Y + spanData.originalCell.RowSpan - 1)
              spanData.finishFlag = true;

            if ((spanData.finishFlag && originalRowIndex == spanData.originalCellOrigin.Y) ||
              (originalRowIndex < spanData.originalCellOrigin.Y ||
               originalRowIndex > spanData.originalCellOrigin.Y + spanData.originalCell.RowSpan - 1))
              rowSpans.Clear();
            else
            {
              spanData.resultCell.RowSpan++;
              needData = false;
            }
          }

          // add the row cell if it has RowSpan > 1
          if (cell.RowSpan > 1 && rowSpans.Count == 0)
          {
            SpanData spanData = new SpanData();
            rowSpans.Add(spanData);

            spanData.originalCell = cell;
            spanData.resultCell = cellTo;
            spanData.originalCellOrigin = new Point(originalColumnIndex, originalRowIndex);
            spanData.resultCellOrigin = new Point(resultColumnIndex, resultRowIndex);
          }

          // now check the column cells. Do this once for each column.
          if (printingRowIndex == 0)
          {
            for (int i = 0; i < columnSpans.Count; i++)
            {
              SpanData spanData = columnSpans[i];

              // check if we are printing the last column of the cell's span. From now, we will not accept 
              // the first column.
              if (originalColumnIndex == spanData.originalCellOrigin.X + spanData.originalCell.ColSpan - 1)
                spanData.finishFlag = true;

              if ((spanData.finishFlag && originalColumnIndex == spanData.originalCellOrigin.X) ||
                (originalColumnIndex < spanData.originalCellOrigin.X ||
                 originalColumnIndex > spanData.originalCellOrigin.X + spanData.originalCell.ColSpan - 1))
              {
                columnSpans.RemoveAt(i);
                i--;
              }
              else
                spanData.resultCell.ColSpan++;
            }
          }

          // check if we should skip current cell because it is inside a span
          for (int i = 0; i < columnSpans.Count; i++)
          {
            SpanData spanData = columnSpans[i];

            if (resultColumnIndex >= spanData.resultCellOrigin.X &&
              resultColumnIndex <= spanData.resultCellOrigin.X + spanData.resultCell.ColSpan - 1 &&
              resultRowIndex >= spanData.resultCellOrigin.Y &&
              resultRowIndex <= spanData.resultCellOrigin.Y + spanData.resultCell.RowSpan)
            {
              needData = false;
              break;
            }
          }

          // add the column cell if it has ColSpan > 1 and not added yet
          if (cell.ColSpan > 1 && needData)
          {
            SpanData spanData = new SpanData();
            columnSpans.Add(spanData);

            spanData.originalCell = cell;
            spanData.resultCell = cellTo;
            spanData.originalCellOrigin = new Point(originalColumnIndex, originalRowIndex);
            spanData.resultCellOrigin = new Point(resultColumnIndex, resultRowIndex);
          }
        }
      }
      else
      {
        cellTo.ColSpan = cell.ColSpan;
        cellTo.RowSpan = cell.RowSpan;
      }

      if (needData)
      {
        cell.SaveState();
        cell.GetData();
        cellTo.RunTimeAssign(cell, true);
        cell.RestoreState();
      }
    }
    #endregion

    #region Aggregate Functions
    #endregion

    public TableHelper(TableObject source, TableResult result)
    {
      sourceTable = source;
      resultTable = result;
      columnSpans = new List<SpanData>();
      rowSpans = new List<SpanData>();
    }


    private class SpanData
    {
      public TableCell originalCell;
      public TableCellData resultCell;
      public Point originalCellOrigin;
      public Point resultCellOrigin;
      public bool finishFlag;
    }
  }
}
