using FastReport.Utils;
using System;
using System.ComponentModel;

namespace FastReport
{
  /// <summary>
  /// This class contains the page columns settings. 
  /// It is used in the <see cref="ReportPage.Columns"/> property.
  /// </summary>
  [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
  public class PageColumns
  {
    private int count;
    private float width;
    private FloatCollection positions;
    private ReportPage page;

    /// <summary>
    /// Gets or sets the number of columns.
    /// </summary>
    /// <remarks>
    /// Set this property to 0 or 1 if you don't want to use columns.
    /// </remarks>
    [DefaultValue(1)]
    public int Count
    {
      get { return count; }
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException("Count", "Value must be greather than 0");

        count = value;
        width = (page.PaperWidth - page.LeftMargin - page.RightMargin) / count;
        positions.Clear();
        for (int i = 0; i < count; i++)
        {
          positions.Add(i * Width);
        }
      }
    }

    /// <summary>
    /// Gets or sets the column width.
    /// </summary>
    [TypeConverter("FastReport.TypeConverters.PaperConverter, FastReport")]
    public float Width
    {
      get { return width; }
      set { width = value; }
    }

    /// <summary>
    /// Gets or sets a list of column starting positions.
    /// </summary>
    /// <remarks>
    /// Each value represents a column starting position measured in the millimeters.
    /// </remarks>
    public FloatCollection Positions
    {
        get { return positions; }
        set
        {
            if (value.Count == count)
            {
                positions = value;
            }
            else
            {
                positions.Clear();
                for (int i = 0; i < count; i++)
                {
                    positions.Add(i * Width);
                }
            }
        }
    }

    private bool ShouldSerializeWidth()
    {
      return Count > 1;
    }

    private bool ShouldSerializePositions()
    {
      return Count > 1;
    }

    /// <summary>
    /// Assigns values from another source.
    /// </summary>
    /// <param name="source">Source to assign from.</param>
    public void Assign(PageColumns source)
    {
      Count = source.Count;
      Width = source.Width;
      Positions.Assign(source.Positions);
    }
    
    internal void Serialize(FRWriter writer, PageColumns c)
    {
      if (Count != c.Count)
        writer.WriteInt("Columns.Count", Count);
      if (Count > 1)
      {
        writer.WriteFloat("Columns.Width", Width);
        Positions = Positions; // avoid bug when number of positions is not equal number of columns
        writer.WriteValue("Columns.Positions", Positions);
      }  
    }
    
    internal PageColumns(ReportPage page)
    {
            this.page = page;
      positions = new FloatCollection();
      Count = 1;
    }
  }
}
