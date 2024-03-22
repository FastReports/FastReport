using System;
using System.Collections.Generic;
using System.Text;
#if !NETSTANDARD
namespace System.Drawing
{
    public class ColorExt
    {
        public static bool IsKnownColor(Color color)
        {
            return color.IsKnownColor;
        }

        public static KnownColor ToKnownColor(Color c)
        {
            return c.ToKnownColor();
        }

        public static Color FromKnownColor(KnownColor knownColor)
        {
            return Color.FromKnownColor(knownColor);
        }

        public static bool IsSystemColor(Color c)
        {
            return c.IsSystemColor;
        }
    }
}
#endif