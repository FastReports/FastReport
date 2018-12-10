using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FastReport
{
    /// <summary>
    /// Represents a cache of graphics objects such as pens, brushes, fonts and text formats.
    /// </summary>
    /// <remarks>
    /// Cache holds all used graphics objects. There is no need to dispose objects returned
    /// by GetXXX calls.
    /// </remarks>
    /// <example>This example demonstrates how to use graphic cache.
    /// <code>
    /// public void Draw(FRPaintEventArgs e)
    /// {
    ///   Brush brush = e.Cache.GetBrush(BackColor);
    ///   Pen pen = e.Cache.GetPen(BorderColor, 1, BorderStyle);
    ///   e.Graphics.FillRectangle(brush, Bounds);
    ///   e.Graphics.DrawRectangle(pen, Bounds);
    /// }
    /// </code>
    /// </example>
    public class GraphicCache : IDisposable
  {
    private Hashtable pens;
    private Hashtable brushes;
    private Hashtable fonts;
    private Hashtable stringFormats;

    /// <summary>
    /// Gets a pen with specified settings.
    /// </summary>
    /// <param name="color">Color of a pen.</param>
    /// <param name="width">Width of a pen.</param>
    /// <param name="style">Dash style of a pen.</param>
    /// <returns>The <b>Pen</b> object.</returns>
    public Pen GetPen(Color color, float width, DashStyle style)
    {
      return GetPen(color, width, style, LineJoin.Miter);
    }

    /// <summary>
    /// Gets a pen with specified settings.
    /// </summary>
    /// <param name="color">Color of a pen.</param>
    /// <param name="width">Width of a pen.</param>
    /// <param name="style">Dash style of a pen.</param>
    /// <param name="lineJoin">Line join of a pen.</param>
    /// <returns>The <b>Pen</b> object.</returns>
    public Pen GetPen(Color color, float width, DashStyle style, LineJoin lineJoin)
    {
      int hash = color.GetHashCode() ^ width.GetHashCode() ^ style.GetHashCode() ^ lineJoin.GetHashCode();
      Pen result = pens[hash] as Pen;
      if (result == null)
      {
        result = new Pen(color, width);
        result.DashStyle = style;
        result.LineJoin = lineJoin;
        pens[hash] = result;
      }
      return result;
    }

    /// <summary>
    /// Gets a brush with specified color.
    /// </summary>
    /// <param name="color">Color of a brush.</param>
    /// <returns>The <b>SolidBrush</b> object.</returns>
    public SolidBrush GetBrush(Color color)
    {
      int hash = color.GetHashCode();
      SolidBrush result = brushes[hash] as SolidBrush;
      if (result == null)
      {
        result = new SolidBrush(color);
        brushes[hash] = result;
      }
      return result;
    }

    /// <summary>
    /// Gets a font with specified settings.
    /// </summary>
    /// <param name="name">Name of a font.</param>
    /// <param name="size">Size of a font.</param>
    /// <param name="style">Style of a font.</param>
    /// <returns>The <b>Font</b> object.</returns>
    public Font GetFont(string name, float size, FontStyle style)
    {
      int hash = name.GetHashCode() ^ size.GetHashCode() ^ style.GetHashCode();
      Font result = fonts[hash] as Font;
      if (result == null)
      {
        result = new Font(name, size, style);
        fonts[hash] = result;
      }
      return result;
    }

    /// <summary>
    /// Gets a string format with specified settings.
    /// </summary>
    /// <param name="align">Text alignment information on the vertical plane.</param>
    /// <param name="lineAlign">Line alignment on the horizontal plane.</param>
    /// <param name="trimming"><b>StringTrimming</b> enumeration.</param>
    /// <param name="flags"><b>StringFormatFlags</b> enumeration that contains formatting information.</param>
    /// <param name="firstTab">The number of spaces between the beginning of a line of text and the first tab stop.</param>
    /// <param name="tabWidth">Distance between tab stops.</param>
    /// <returns>The <b>StringFormat</b> object.</returns>
    public StringFormat GetStringFormat(StringAlignment align, StringAlignment lineAlign, 
      StringTrimming trimming, StringFormatFlags flags, float firstTab, float tabWidth)
    {
      int hash = align.GetHashCode() ^ (lineAlign.GetHashCode() << 2) ^ (trimming.GetHashCode() << 5) ^
        (flags.GetHashCode() << 16) ^ (100 - firstTab).GetHashCode() ^ tabWidth.GetHashCode();
      StringFormat result = stringFormats[hash] as StringFormat;
      if (result == null)
      {
        result = new StringFormat();
        result.Alignment = align;
        result.LineAlignment = lineAlign;
        result.Trimming = trimming;
        result.FormatFlags = flags;
        float[] tabStops = new float[64];
        // fixed issue 2823
        tabStops[0] = firstTab;
        for (int i = 1; i < 64; i++)
        {
          tabStops[i] = tabWidth;
        }
        result.SetTabStops(0, tabStops);
        stringFormats[hash] = result;
      }
      return result;
    }
    
    /// <summary>
    /// Disposes resources used by this object.
    /// </summary>
    public void Dispose()
    {
      foreach (Pen pen in pens.Values)
      {
        pen.Dispose();        
      }
      foreach (Brush brush in brushes.Values)
      {
        brush.Dispose();
      }
      foreach (Font font in fonts.Values)
      {
        font.Dispose();
      }
      foreach (StringFormat format in stringFormats.Values)
      {
        format.Dispose();
      }
      pens.Clear();
      brushes.Clear();
      fonts.Clear();
      stringFormats.Clear();
    }

    /// <summary>
    /// Initializes a new instance of the <b>GraphicCache</b> class with default settings.
    /// </summary>
    public GraphicCache()
    {
      pens = new Hashtable();
      brushes = new Hashtable();
      fonts = new Hashtable();
      stringFormats = new Hashtable();
    }

  }
}
