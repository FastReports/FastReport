using System;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// The layout of the data band columns.
  /// </summary>
  public enum ColumnLayout 
  { 
    /// <summary>
    /// Print columns across then down.
    /// </summary>
    AcrossThenDown, 
    
    /// <summary>
    /// Print columns down then across.
    /// </summary>
    DownThenAcross 
  }
  
  /// <summary>
  /// This class holds the band columns settings. It is used in the <see cref="DataBand.Columns"/> property.
  /// </summary>
  [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
  public class BandColumns
  {
    private int count;
    private float width;
    private ColumnLayout layout;
    private int minRowCount;
    private DataBand band;

    /// <summary>
    /// Gets or sets the number of columns.
    /// </summary>
    /// <remarks>
    /// Set this property to 0 or 1 if you don't want to use columns.
    /// </remarks>
    [DefaultValue(0)]
    public int Count
    {
      get { return count; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("Count", "Value must be >= 0");
        count = value;
      }
    }

    /// <summary>
    /// The column width, in pixels.
    /// </summary>
    [DefaultValue(0f)]
    [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
    public float Width
    {
      get { return width; }
      set { width = value; }
    }
    
    /// <summary>
    /// Gets or sets the layout of the columns.
    /// </summary>
    [DefaultValue(ColumnLayout.AcrossThenDown)]
    public ColumnLayout Layout
    {
      get { return layout; }
      set { layout = value; }
    }
    
    /// <summary>
    /// Gets or sets the minimum row count that must be printed.
    /// </summary>
    /// <remarks>
    /// This property is used if the <b>Layout</b> property is set to <b>DownThenAcross</b>. 0 means that
    /// FastReport should calculate the optimal number of rows.
    /// </remarks>
    [DefaultValue(0)]
    public int MinRowCount
    {
      get { return minRowCount; }
      set { minRowCount = value; }
    }
    
    internal float ActualWidth
    {
      get
      {
        ReportPage page = band.Page as ReportPage;
        if (Width == 0 && page != null)
          return (page.PaperWidth - page.LeftMargin - page.RightMargin) * Units.Millimeters / (Count == 0 ? 1 : Count);
        return Width;
      }
    }
    
    internal FloatCollection Positions
    {
      get
      {
        FloatCollection positions = new FloatCollection();
        float columnWidth = ActualWidth;
        for (int i = 0; i < Count; i++)
        {
          positions.Add(i * columnWidth);
        }
        return positions;
      }
    }

    /// <summary>
    /// Assigns values from another source.
    /// </summary>
    /// <param name="source">Source to assign from.</param>
    public void Assign(BandColumns source)
    {
      Count = source.Count;
      Width = source.Width;
      Layout = source.Layout;
      MinRowCount = source.MinRowCount;
    }

    internal void Serialize(FRWriter writer, BandColumns c)
    {
      if (Count != c.Count)
        writer.WriteInt("Columns.Count", Count);
      if (Width != c.Width)
        writer.WriteFloat("Columns.Width", Width);
      if (Layout != c.Layout)
        writer.WriteValue("Columns.Layout", Layout);
      if (MinRowCount != c.MinRowCount)
        writer.WriteInt("Columns.MinRowCount", MinRowCount);
    }
    
    /// <summary>
    /// Initializes a new instance of the <b>BandColumns</b> class with default settings. 
    /// </summary>
    public BandColumns(DataBand band)
    {
            this.band = band;
    }
  }
}
