using System;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Table;

namespace FastReport
{
    /// <summary>
    /// Represents a text object which draws each symbol of text in its own cell.
    /// </summary>
    /// <remarks>
    /// <para/>The text may be aligned to left or right side, or centered. Use the <see cref="HorzAlign"/>
    /// property to do this. The "justify" align is not supported now, as well as vertical alignment.
    /// <para/>The cell size is defined in the <see cref="CellWidth"/> and <see cref="CellHeight"/> properties.
    /// These properties are 0 by default, in this case the size of cell is calculated automatically based
    /// on the object's <b>Font</b>.
    /// <para/>To define a spacing (gap) between cells, use the <see cref="HorzSpacing"/> and
    /// <see cref="VertSpacing"/> properties.
    /// </remarks>
    public partial class CellularTextObject : TextObject
    {
        #region Fields
        private float cellWidth;
        private float cellHeight;
        private float horzSpacing;
        private float vertSpacing;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width of cell, in pixels.
        /// </summary>
        /// <remarks>
        /// If zero width and/or height specified, the object will calculate the cell size
        /// automatically based on its font.
        /// </remarks>
        [Category("Appearance")]
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float CellWidth
        {
            get { return cellWidth; }
            set { cellWidth = value; }
        }

        /// <summary>
        /// Gets or sets the height of cell, in pixels.
        /// </summary>
        /// <remarks>
        /// If zero width and/or height specified, the object will calculate the cell size
        /// automatically based on its font.
        /// </remarks>
        [Category("Appearance")]
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float CellHeight
        {
            get { return cellHeight; }
            set { cellHeight = value; }
        }

        /// <summary>
        /// Gets or sets the horizontal spacing between cells, in pixels.
        /// </summary>
        [Category("Appearance")]
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float HorzSpacing
        {
            get { return horzSpacing; }
            set { horzSpacing = value; }
        }

        /// <summary>
        /// Gets or sets the vertical spacing between cells, in pixels.
        /// </summary>
        [Category("Appearance")]
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float VertSpacing
        {
            get { return vertSpacing; }
            set { vertSpacing = value; }
        }
        #endregion

        #region Private Methods
        // use the TableObject to represent the contents. It's easier to export it later.
        private TableObject GetTable(bool autoRows)
        {
            TableObject table = new TableObject();
            table.SetPrinting(IsPrinting);
            table.SetReport(Report);

            float cellWidth = CellWidth;
            float cellHeight = CellHeight;
            // calculate cellWidth, cellHeight automatically
            if (cellWidth == 0 || cellHeight == 0)
            {
                float fontHeight = Font.GetHeight() * 96f / DrawUtils.ScreenDpi;
                cellWidth = GetCellWidthInternal(fontHeight);
                cellHeight = cellWidth;
            }

            int colCount = (int)((Width + HorzSpacing + 1) / (cellWidth + HorzSpacing));
            if (colCount == 0)
                colCount = 1;
            int rowCount = (int)((Height + VertSpacing + 1) / (cellHeight + VertSpacing));
            if (rowCount == 0 || autoRows)
                rowCount = 1;

            table.ColumnCount = colCount;
            table.RowCount = rowCount;

            // process the text
            int row = 0;
            int lineBegin = 0;
            int lastSpace = 0;
            string text = Text.Replace("\r\n", "\n");

            for (int i = 0; i < text.Length; i++)
            {
                bool isCRLF = text[i] == '\n';
                if (text[i] == ' ' || isCRLF)
                    lastSpace = i;

                if (i - lineBegin + 1 > colCount || isCRLF)
                {
                    if (WordWrap && lastSpace > lineBegin)
                    {
                        AddText(table, row, text.Substring(lineBegin, lastSpace - lineBegin));
                        lineBegin = lastSpace + 1;
                    }
                    else if (i - lineBegin > 0)
                    {
                        AddText(table, row, text.Substring(lineBegin, i - lineBegin));
                        lineBegin = i;
                    }
                    else
                        lineBegin = i + 1;

                    lastSpace = lineBegin;
                    row++;
                    if (autoRows && row >= rowCount)
                    {
                        rowCount++;
                        table.RowCount++;
                    }
                }
            }

            // finish the last line
            if (lineBegin < text.Length)
                AddText(table, row, text.Substring(lineBegin, text.Length - lineBegin));

            // set up cells appearance
            for (int i = 0; i < colCount; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    TableCell cell = table[i, j];
                    cell.Border = Border.Clone();
                    cell.Fill = Fill.Clone();
                    cell.Font = Font;
                    cell.TextFill = TextFill.Clone();
                    cell.HorzAlign = HorzAlign.Center;
                    cell.VertAlign = VertAlign.Center;
                }
            }

            // set cell's width and height
            for (int i = 0; i < colCount; i++)
            {
                table.Columns[i].Width = cellWidth;
            }

            for (int i = 0; i < rowCount; i++)
            {
                table.Rows[i].Height = cellHeight;
            }

            // insert spacing between cells
            if (HorzSpacing > 0)
            {
                for (int i = 0; i < colCount - 1; i++)
                {
                    TableColumn newColumn = new TableColumn();
                    newColumn.Width = HorzSpacing;
                    table.Columns.Insert(i * 2 + 1, newColumn);
                }
            }

            if (VertSpacing > 0)
            {
                for (int i = 0; i < rowCount - 1; i++)
                {
                    TableRow newRow = new TableRow();
                    newRow.Height = VertSpacing;
                    table.Rows.Insert(i * 2 + 1, newRow);
                }
            }

            table.Left = AbsLeft;
            table.Top = AbsTop;
            table.Width = table.Columns[table.ColumnCount - 1].Right;
            table.Height = table.Rows[table.RowCount - 1].Bottom;
            return table;
        }

        private void AddText(TableObject table, int row, string text)
        {
            if (row >= table.RowCount)
                return;

            text = text.TrimEnd(new char[] { ' ' });
            if (text.Length > table.ColumnCount)
                text = text.Substring(0, table.ColumnCount);

            int offset = 0;
            if (HorzAlign == HorzAlign.Right)
                offset = table.ColumnCount - text.Length;
            else if (HorzAlign == HorzAlign.Center)
                offset = (table.ColumnCount - text.Length) / 2;

            for (int i = 0; i < text.Length; i++)
            {
                table[i + offset, row].Text = text[i].ToString();
            }
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            CellularTextObject src = source as CellularTextObject;
            CellWidth = src.CellWidth;
            CellHeight = src.CellHeight;
            HorzSpacing = src.HorzSpacing;
            VertSpacing = src.VertSpacing;
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            CellularTextObject c = writer.DiffObject as CellularTextObject;
            base.Serialize(writer);

            if (FloatDiff(CellWidth, c.CellWidth))
                writer.WriteFloat("CellWidth", CellWidth);
            if (FloatDiff(CellHeight, c.CellHeight))
                writer.WriteFloat("CellHeight", CellHeight);
            if (FloatDiff(HorzSpacing, c.HorzSpacing))
                writer.WriteFloat("HorzSpacing", HorzSpacing);
            if (FloatDiff(VertSpacing, c.VertSpacing))
                writer.WriteFloat("VertSpacing", VertSpacing);
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            using (TableObject table = GetTable())
            {
                table.Draw(e);
            }
        }

        public TableObject GetTable()
        {
            return GetTable(false);
        }
        #endregion

        #region Report Engine
        /// <inheritdoc/>
        public override float CalcHeight()
        {
            using (TableObject table = GetTable(true))
            {
                return table.Height;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CellularTextObject"/> class with the default settings.
        /// </summary>
        public CellularTextObject()
        {
            CanBreak = false;
            Border.Lines = BorderLines.All;
        }
    }
}