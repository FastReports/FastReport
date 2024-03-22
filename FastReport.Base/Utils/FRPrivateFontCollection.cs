using System;

namespace FastReport.Utils
{
    /// <summary>
    /// A wrapper around PrivateFontCollection.
    /// </summary>
    [Obsolete("Use FastReport.FontManager instead")]
    public partial class FRPrivateFontCollection
    {
        public bool AddFontFile(string filename) => FontManager.AddFont(filename);

        public void AddMemoryFont(IntPtr memory, int length) => FontManager.AddFont(memory, length);
    }
}
