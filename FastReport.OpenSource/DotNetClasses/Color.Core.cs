using System;
using System.Collections.Generic;
using System.Text;

namespace System.Drawing
{
    public class ColorExt
    {
        public static bool IsKnownColor(Color color)
        {
            return color.IsNamedColor;
        }

        public static KnownColor ToKnownColor(Color c)
        {
            KnownColor color;
            if (Enum.TryParse<KnownColor>(c.Name, out color))
                return color;
            return default(KnownColor);
        }

        public static Color FromKnownColor(KnownColor knownColor)
        {
            return Color.FromName(knownColor.ToString());
        }

        public static bool IsSystemColor(Color c)
        {
            if (IsKnownColor(c))
            {
                KnownColor knownColor = ToKnownColor(c);
                return ((((KnownColor)knownColor) <= KnownColor.WindowText) || (((KnownColor)knownColor) > KnownColor.YellowGreen));
            }
            return false;
        }
    }
}
