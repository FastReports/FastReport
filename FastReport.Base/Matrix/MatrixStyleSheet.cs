using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FastReport.Matrix
{
  internal class MatrixStyleSheet : StyleSheet
  {
    public Bitmap GetStyleBitmap(int index)
    {
      StyleCollection styleCollection = this[index];
      Style style = styleCollection[styleCollection.IndexOf("Header")];
      
      Color headerColor = Color.White;
      if (style.Fill is SolidFill)
        headerColor = (style.Fill as SolidFill).Color;
      else if (style.Fill is LinearGradientFill)
        headerColor = (style.Fill as LinearGradientFill).StartColor;

      style = styleCollection[styleCollection.IndexOf("Body")];
      Color bodyColor = Color.White;
      if (style.Fill is SolidFill)
        bodyColor = (style.Fill as SolidFill).Color;
      else if (style.Fill is LinearGradientFill)
        bodyColor = (style.Fill as LinearGradientFill).StartColor;
        
      // draw style picture
      Bitmap result = new Bitmap(16, 16);
      using (Graphics g = Graphics.FromImage(result))
      {
        g.FillRectangle(Brushes.White, 0, 0, 16, 16);
        
        using (Brush b = new SolidBrush(headerColor))
        {
          g.FillRectangle(b, 0, 0, 15, 8);
        }
        using (Brush b = new SolidBrush(bodyColor))
        {
          g.FillRectangle(b, 0, 8, 15, 8);
        }
        
        g.DrawRectangle(Pens.Silver, 0, 0, 14, 14);
      }
      
      return result;
    }
  }
}
