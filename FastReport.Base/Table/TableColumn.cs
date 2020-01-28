using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using FastReport.Data;
using FastReport.Utils;

namespace FastReport.Table
{
  /// <summary>
  /// Represents a table column.
  /// </summary>
  /// <remarks>
  /// Use the <see cref="Width"/> property to set the width of a column. If <see cref="AutoSize"/>
  /// property is <b>true</b>, the column will calculate its width automatically.
  /// <para/>You can also set the <see cref="MinWidth"/> and <see cref="MaxWidth"/> properties
  /// to restrict the column's width.
  /// </remarks>
  public partial class TableColumn : ComponentBase
  {
    #region Fields
    private float minWidth;
    private float maxWidth;
    private bool autoSize;
    private bool pageBreak;
    private int keepColumns;
    private int index;
    private float saveWidth;
    private bool saveVisible;
    private float minimumBreakWidth;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a width of the column, in pixels.
    /// </summary>
    /// <remarks>
    /// The column width cannot exceed the range defined by the <see cref="MinWidth"/> 
    /// and <see cref="MaxWidth"/> properties.
    /// <note>To convert between pixels and report units, use the constants defined 
    /// in the <see cref="Units"/> class.</note>
    /// </remarks>
    [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
    public override float Width
    {
      get { return base.Width; }
      set
      {
        value = Converter.DecreasePrecision(value, 2);
        if (value > MaxWidth)
          value = MaxWidth;
        if (value < MinWidth)
          value = MinWidth;
        if (FloatDiff(base.Width, value))
        {
          UpdateLayout(value - base.Width, 0);
          base.Width = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the minimal width for this column, in pixels.
    /// </summary>
    [DefaultValue(0f)]
    [Category("Layout")]
    [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
    public float MinWidth
    {
      get { return minWidth; }
      set { minWidth = value; }
    }

    /// <summary>
    /// Gets or sets the maximal width for this column, in pixels.
    /// </summary>
    [DefaultValue(5000f)]
    [Category("Layout")]
    [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
    public float MaxWidth
    {
      get { return maxWidth; }
      set { maxWidth = value; }
    }

    /// <summary>
    /// Gets or sets a value determines if the column should calculate its width automatically.
    /// </summary>
    /// <remarks>
    /// The column width cannot exceed the range defined by the <see cref="MinWidth"/> 
    /// and <see cref="MaxWidth"/> properties.
    /// </remarks>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool AutoSize
    {
      get { return autoSize; }
      set { autoSize = value; }
    }

    /// <summary>
    /// Gets the index of this column.
    /// </summary>
    [Browsable(false)]
    public int Index
    {
      get { return index; }
    }

    /// <inheritdoc/>
    [Browsable(false)]
    public override float Left
    {
      get
      {
        TableBase table = Parent as TableBase;
        if (table == null)
          return 0;

        float result = 0;
        for (int i = 0; i < Index; i++)
        {
          result += table.Columns[i].Width;
        }
        return result;
      }
      set { base.Left = value; }
    }

    /// <summary>
    /// Gets or sets the page break flag for this column.
    /// </summary>
    [Browsable(false)]
    public bool PageBreak
    {
      get { return pageBreak; }
      set { pageBreak = value; }
    }

    /// <summary>
    /// Gets or sets the number of columns to keep on the same page.
    /// </summary>
    [Browsable(false)]
    public int KeepColumns
    {
      get { return keepColumns; }
      set { keepColumns = value; }
    }

    internal float MinimumBreakWidth
    {
      get { return minimumBreakWidth; }
      set { minimumBreakWidth = value; }
    }
    
    internal static float DefaultWidth
    {
      get { return (int)Math.Round(64 / (0.25f * Units.Centimeters)) * (0.25f * Units.Centimeters); }
    }
    #endregion

    #region Private Methods
    private void UpdateLayout(float dx, float dy)
    {
      TableBase table = Parent as TableBase;
      if (table == null)
        return;

      // update this column cells
      foreach (TableRow row in table.Rows)
      {
        row.CellData(Index).UpdateLayout(Width, row.Height, dx, dy);
      }

      // update spanned cells that contains this column
      List<Rectangle> spanList = table.GetSpanList();
      foreach (Rectangle span in spanList)
      {
        if (Index > span.Left && Index < span.Right)
        {
          TableRow row = table.Rows[span.Top];
          row.CellData(span.Left).UpdateLayout(table.Columns[span.Left].Width, row.Height, dx, dy);
        }
      }
    }
    #endregion
    
    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      TableColumn src = source as TableColumn;
      MinWidth = src.MinWidth;
      MaxWidth = src.MaxWidth;
      AutoSize = src.AutoSize;
      KeepColumns = src.KeepColumns;

      base.Assign(source);
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      TableColumn c = writer.DiffObject as TableColumn;
      base.Serialize(writer);

      if (FloatDiff(MinWidth, c.MinWidth))
        writer.WriteFloat("MinWidth", MinWidth);
      if (FloatDiff(MaxWidth, c.MaxWidth))
        writer.WriteFloat("MaxWidth", MaxWidth);
      if (FloatDiff(Width, c.Width))
        writer.WriteFloat("Width", Width);
      if (AutoSize != c.AutoSize)
        writer.WriteBool("AutoSize", AutoSize);
    }

    public void SetIndex(int value)
    {
      index = value;
    }
    
    internal void SaveState()
    {
      saveWidth = Width;
      saveVisible = Visible;
    }

    internal void RestoreState()
    {
      Width = saveWidth;
      Visible = saveVisible;
    }

    /// <inheritdoc/>
    public override void Clear()
    {
      TableBase grid = Parent as TableBase;
      if (grid == null)
        return;

      int colIndex = grid.Columns.IndexOf(this);
      foreach (TableRow row in grid.Rows)
      {
        row[colIndex].Dispose();
      }

      base.Clear();
    }
    #endregion
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TableColumn"/> class.
    /// </summary>
    public TableColumn()
    {
      maxWidth = 5000;
      Width = DefaultWidth;
      SetFlags(Flags.CanCopy | Flags.CanDelete | Flags.CanWriteBounds, false);
      BaseName = "Column";
    }
  }
}
