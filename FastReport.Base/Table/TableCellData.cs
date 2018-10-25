using System;
using System.Drawing;

namespace FastReport.Table
{
  /// <summary>
  /// Represents data of the table cell.
  /// </summary>
  public class TableCellData : IDisposable
  {
    #region Fields
    
    private string text;
    private object value;
    private string hyperlinkValue;
    private int colSpan;
    private int rowSpan;
    private ReportComponentCollection objects;
    private TableCell style;
    private TableCell cell;
    private TableBase table;
    private Point address;

    #endregion // Fields
    
    #region Properties

    /// <summary>
    /// Gets or sets parent table of the cell.
    /// </summary>
    public TableBase Table
    {
      get { return table; }
      set { table = value; }
    }

    /// <summary>
    /// Gets or sets objects collection of the cell.
    /// </summary>
    public ReportComponentCollection Objects
    {
      get { return objects; }
      set { objects = value; }
    }

    /// <summary>
    /// Gets or sets text of the table cell.
    /// </summary>
    public string Text
    {
      get { return text; }
      set { text = value; }
    }
    
    /// <summary>
    /// Gets or sets value of the table cell.
    /// </summary>
    public object Value
    {
      get { return value; }
      set { this.value = value; }
    }

    /// <summary>
    /// Gets or sets hyperlink value of the table cell.
    /// </summary>
    public string HyperlinkValue
    {
      get { return hyperlinkValue; }
      set { hyperlinkValue = value; }
    }

    /// <summary>
    /// Gets or sets column span of the table cell.
    /// </summary>
    public int ColSpan
    {
      get { return colSpan; }
      set 
      {
        if (colSpan != value)
        {
          float oldWidth = Width;
          colSpan = value;
          float newWidth = Width;
          UpdateLayout(newWidth, Height, newWidth - oldWidth, 0);
          
          if (Table != null)
            Table.ResetSpanList();
        }  
      }
    }

    /// <summary>
    /// Gets or sets row span of the table cell.
    /// </summary>
    public int RowSpan
    {
      get { return rowSpan; }
      set 
      {
        if (rowSpan != value)
        {
          float oldHeight = Height;
          rowSpan = value;
          float newHeight = Height;
          UpdateLayout(Width, newHeight, 0, newHeight - oldHeight);

          if (Table != null)
            Table.ResetSpanList();
        }
      }
    }
    
    /// <summary>
    /// Gets or sets the address of the table cell.
    /// </summary>
    public Point Address
    {
      get { return address; }
      set { address = value; }
    }
    
    /// <summary>
    /// Gets the table cell.
    /// </summary>
    public TableCell Cell
    {
      get 
      {
        if (Table.IsResultTable)
        {
          TableCell cell0 = style;
          if (cell0 == null)
            cell0 = Table.Styles.DefaultStyle;
          
          if (this.cell != null)
          {
            cell0.Alias = this.cell.Alias;
            cell0.OriginalComponent = this.cell.OriginalComponent;
          }
          cell0.CellData = this;
          cell0.Hyperlink.Value = HyperlinkValue;
          return cell0;
        }
        
        if (cell == null)
        {
          cell = new TableCell();
          cell.CellData = this;
        }
        return cell; 
      }
    }

    /// <summary>
    /// Gets style of table cell.
    /// </summary>
    public TableCell Style
    {
      get { return style; }
    }
    
    /// <summary>
    /// Gets original the table cell.
    /// </summary>
    public TableCell OriginalCell
    {
      get { return cell; }
    }
    
    /// <summary>
    /// Gets width of the table cell.
    /// </summary>
    public float Width
    {
      get 
      {
        if (Table == null)
          return 0;

        float result = 0;
        for (int i = 0; i < ColSpan; i++)
        {
          if (Address.X + i < Table.Columns.Count)
            result += Table.Columns[Address.X + i].Width;
        }
        return result;
      }
    }
    
    /// <summary>
    /// Gets height of the table cell.
    /// </summary>
    public float Height
    {
      get
      {
        if (Table == null)
          return 0;

        float result = 0;
        for (int i = 0; i < RowSpan; i++)
        {
          if (Address.Y + i < Table.Rows.Count)
            result += Table.Rows[Address.Y + i].Height;
        }
        return result;
      }
    }

    #endregion // Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TableCellData"/> class.
    /// </summary>
    public TableCellData()
    {
      colSpan = 1;
      rowSpan = 1;
      text = "";
      hyperlinkValue = "";
    }

    #endregion // Constructors

    #region Public Methods

    /// <summary>
    /// Attaches the specified table cell.
    /// </summary>
    /// <param name="cell">The table cell instance.</param>
    /// <remarks>This method is called when we load the table.</remarks>
    public void AttachCell(TableCell cell)
    {
      if (this.cell != null)
      {
                this.cell.CellData = null;
                this.cell.Dispose();
      }  

      Text = cell.Text;
      ColSpan = cell.ColSpan;
      RowSpan = cell.RowSpan;
      objects = cell.Objects;
      style = null;
            this.cell = cell;
      cell.CellData = this;
    }

    /// <summary>
    /// Assigns another <see cref="TableCellData"/> instance.
    /// </summary>
    /// <param name="source">The table cell data that used as a source.</param>
    /// <remarks>This method is called when we copy cells or clone columns/rows in a designer.</remarks>
    public void Assign(TableCellData source)
    {
      AttachCell(source.Cell);
    }
    
    /// <summary>
    /// Assigns another <see cref="TableCellData"/> instance at run time.
    /// </summary>
    /// <param name="cell">The table cell data that used as a source.</param>
    /// <param name="copyChildren">This flag shows should children be copied or not.</param>
    /// <remarks>This method is called when we print a table. We should create a copy of the cell and set the style.</remarks>
    public void RunTimeAssign(TableCell cell, bool copyChildren)
    {
      Text = cell.Text;
      Value = cell.Value;
      HyperlinkValue = cell.Hyperlink.Value;
      // don't copy ColSpan, RowSpan - they will be handled in the TableHelper.
      //ColSpan = cell.ColSpan;
      //RowSpan = cell.RowSpan;
      
      // clone objects
      objects = null;
      if (cell.Objects != null && copyChildren)
      {
        objects = new ReportComponentCollection();
        foreach (ReportComponentBase obj in cell.Objects)
        {
          if (obj.Visible)
          {
            ReportComponentBase cloneObj = Activator.CreateInstance(obj.GetType()) as ReportComponentBase;
            cloneObj.AssignAll(obj);
            objects.Add(cloneObj);
          }
        }
      }

      // add the cell to the style list. If the list contains such style,
      // return the existing style; in other case, create new style based
      // on the given cell.
      SetStyle(cell);
            // FCell is used to reference the original cell. It is necessary to use Alias, OriginalComponent
            this.cell = cell;

      // reset object's location as if we set ColSpan and RowSpan to 1.
      // It is nesessary when printing spanned cells because the span of such cells will be corrected
      // when print new rows/columns and thus will move cell objects.
      if (objects != null)
        UpdateLayout(Width, Height, Width - cell.CellData.Width, Height - cell.CellData.Height);
    }

    /// <summary>
    /// Sets style of the table cell.
    /// </summary>
    /// <param name="style">The new style of the table cell.</param>
    public void SetStyle(TableCell style)
    {
            this.style = Table.Styles.Add(style);
    }
    
    /// <summary>
    /// Disposes the <see cref="TableCellData"/> instance.
    /// </summary>
    public void Dispose()
    {
      if (style == null && cell != null)
        cell.Dispose();
      cell = null;
      style = null;
    }
    
    /// <summary>
    /// Calculates width of the table cell.
    /// </summary>
    /// <returns>The value of the table cell width.</returns>
    public float CalcWidth()
    {
      TableCell cell = Cell;
      cell.SetReport(Table.Report);
      return cell.CalcWidth();  
    }
    
    /// <summary>
    /// Calculates height of the table cell.
    /// </summary>
    /// <param name="width">The width of the table cell.</param>
    /// <returns>The value of the table cell height.</returns>
    public float CalcHeight(float width)
    {
      TableCell cell = Cell;
      cell.SetReport(Table.Report);
      cell.Width = width;
      float cellHeight = cell.CalcHeight();
      
      if (objects != null)
      {
// pasted from BandBase.cs

        // sort objects by Top
        ReportComponentCollection sortedObjects = objects.SortByTop();

        // calc height of each object
        float[] heights = new float[sortedObjects.Count];
        for (int i = 0; i < sortedObjects.Count; i++)
        {
          ReportComponentBase obj = sortedObjects[i];
          float height = obj.Height;
          if (obj.CanGrow || obj.CanShrink)
          {
            float height1 = obj.CalcHeight();
            if ((obj.CanGrow && height1 > height) || (obj.CanShrink && height1 < height))
              height = height1;
          }
          heights[i] = height;
        }

        // calc shift amounts
        float[] shifts = new float[sortedObjects.Count];
        for (int i = 0; i < sortedObjects.Count; i++)
        {
          ReportComponentBase parent = sortedObjects[i];
          float shift = heights[i] - parent.Height;
          if (shift == 0)
            continue;

          for (int j = i + 1; j < sortedObjects.Count; j++)
          {
            ReportComponentBase child = sortedObjects[j];
            if (child.ShiftMode == ShiftMode.Never)
              continue;

            if (child.Top >= parent.Bottom - 1e-4)
            {
              if (child.ShiftMode == ShiftMode.WhenOverlapped &&
                (child.Left > parent.Right - 1e-4 || parent.Left > child.Right - 1e-4))
                continue;

              float parentShift = shifts[i];
              float childShift = shifts[j];
              if (shift > 0)
                childShift = Math.Max(shift + parentShift, childShift);
              else
                childShift = Math.Min(shift + parentShift, childShift);
              shifts[j] = childShift;
            }
          }
        }

        // update location and size of each component, calc max height
        float maxHeight = 0;
        for (int i = 0; i < sortedObjects.Count; i++)
        {
          ReportComponentBase obj = sortedObjects[i];
          obj.Height = heights[i];
          obj.Top += shifts[i];
          if (obj.Bottom > maxHeight)
            maxHeight = obj.Bottom;
        }
        
        if (cellHeight < maxHeight)
          cellHeight = maxHeight;

        // perform grow to bottom
        foreach (ReportComponentBase obj in objects)
        {
          if (obj.GrowToBottom)
            obj.Height = cellHeight - obj.Top;
        }

// -----------------------

      }
      
      return cellHeight;
    }
    
    /// <summary>
    /// Updates layout of the table cell.
    /// </summary>
    /// <param name="width">The width of the table cell.</param>
    /// <param name="height">The height of the table cell.</param>
    /// <param name="dx">The new value of x coordinate.</param>
    /// <param name="dy">The new value of y coordinate.</param>
    public void UpdateLayout(float width, float height, float dx, float dy)
    {
      if (Objects == null)
        return;
      
      TableCell cell = Cell;
      cell.Width = width;
      cell.Height = height;
      cell.UpdateLayout(dx, dy);
    }

    #endregion // Public Methods
  }
}
