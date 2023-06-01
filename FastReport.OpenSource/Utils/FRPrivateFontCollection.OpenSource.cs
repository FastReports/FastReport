using System;
using System.Collections.Generic;
using System.Drawing;

namespace FastReport.Utils
{
    /// <summary>
    /// A wrapper around PrivateFontCollection.
    /// </summary>
    partial class FRPrivateFontCollection
    {
        private void RegisterFontInternal(string filename)
        {
            string fontName = Families[Families.Length - 1].Name;
            if (!FontFiles.ContainsKey(fontName))
                FontFiles.Add(fontName, filename);
#if DEBUG
            else
                Console.WriteLine("Font \"{0}\" already present in collection.\n Files:\n  {1}\n  {2}\n", fontName, FontFiles[fontName], filename);
#endif
        }

        internal Font CheckFamily(Font font)
        {
#if DEBUG
            if (font.OriginalFontName != null && font.Name != font.OriginalFontName)
            {
                Console.WriteLine("Font '{0}' was substituted with {1}", font.OriginalFontName, font.Name);
            }
#endif
            return font;
        }
    }
}
