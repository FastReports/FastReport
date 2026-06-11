using System;

namespace FastReport.Utils
{
    /// <summary>
    /// A wrapper around PrivateFontCollection.
    /// </summary>
    [Obsolete("Use FastReport.FontManager instead")]
    public partial class FRPrivateFontCollection
    {
        /// <summary>
        /// Adds a font.
        /// </summary>
        /// <param name="filename">The font file name.</param>
        /// <returns>true if the font added succesfully.</returns>
        public bool AddFontFile(string filename) => FontManager.AddFont(filename);

        /// <summary>
        /// Adds memory font.
        /// </summary>
        /// <param name="memory">Pointer to memory block.</param>
        /// <param name="length">Length of the block.</param>
        public void AddMemoryFont(IntPtr memory, int length) => FontManager.AddFont(memory, length);
    }
}
