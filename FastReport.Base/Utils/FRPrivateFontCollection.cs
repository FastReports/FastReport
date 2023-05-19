using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Drawing;

namespace FastReport.Utils
{
    /// <summary>
    /// A wrapper around PrivateFontCollection.
    /// </summary>
    public partial class FRPrivateFontCollection
    {
        private readonly PrivateFontCollection collection = TypeConverters.FontConverter.PrivateFontCollection;
        private Dictionary<string, string> FontFiles = new Dictionary<string, string>();
        private Dictionary<string, MemoryFont> MemoryFonts = new Dictionary<string, MemoryFont>();

        internal PrivateFontCollection Collection { get { return collection; } }

        /// <summary>
        /// Gets the array of FontFamily objects associated with this collection.
        /// </summary>
        public FontFamily[] Families { get { return collection.Families; } }

        /// <summary>
        /// Checks if the font name is contained in this collection.
        /// </summary>
        /// <param name="fontName">The name of the font.</param>
        /// <returns>true if the font is contained in this collection.</returns>
        public bool HasFont(string fontName)
        {
            return FontFiles.ContainsKey(fontName) || MemoryFonts.ContainsKey(fontName);
        }

        /// <summary>
        /// Returns the font's stream.
        /// </summary>
        /// <param name="fontName">The name of the font.</param>
        /// <returns>Either FileStream or MemoryStream containing font data.</returns>
        public Stream GetFontStream(string fontName)
        {
            if (FontFiles.ContainsKey(fontName))
            {
                return new FileStream(FontFiles[fontName], FileMode.Open, FileAccess.Read);
            }
            else if (MemoryFonts.ContainsKey(fontName))
            {
                MemoryFont font = MemoryFonts[fontName];
                byte[] buffer = new byte[font.Length];
                Marshal.Copy(font.Memory, buffer, 0, font.Length);
                return new MemoryStream(buffer);
            }

            return null;
        }

        /// <summary>
        /// Adds a font from the specified file to this collection.
        /// </summary>
        /// <param name="filename">A System.String that contains the file name of the font to add.</param>
        /// <returns>true if the font is registered by application.</returns>
        public bool AddFontFile(string filename)
        {
            bool success = false;
            if(File.Exists(filename))
            {
                if (!FontFiles.ContainsValue(filename))
                {
                    collection.AddFontFile(filename);
                    RegisterFontInternal(filename);
                    success = true;
                }
#if DEBUG // Not so important information           
                else
                {
                    Console.WriteLine("Font file '{0}' already present in collection", filename);
                }
#endif
            }
#if DEBUG
            else
            {
                Console.WriteLine("Font file '{0}' not found", filename);
            }
#endif
            return success;
        }

#if SKIA
        public void AddFontFromStream(Stream stream)
        {
            collection.AddFont(stream);
        }
#endif

        /// <summary>
        /// Adds a font contained in system memory to this collection.
        /// </summary>
        /// <param name="memory">The memory address of the font to add.</param>
        /// <param name="length">The memory length of the font to add.</param>
        public void AddMemoryFont(IntPtr memory, int length)
        {
            collection.AddMemoryFont(memory, length);
            string fontName = Families[Families.Length - 1].Name;
            if (!FontFiles.ContainsKey(fontName))
                MemoryFonts.Add(fontName, new MemoryFont(memory, length));
        }

        private struct MemoryFont
        {
            public readonly IntPtr Memory;
            public readonly int Length;

            public MemoryFont(IntPtr memory, int length)
            {
                Memory = memory;
                Length = length;
            }
        }

    }
}
