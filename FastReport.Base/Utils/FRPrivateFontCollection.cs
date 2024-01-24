using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Drawing;
using System.Linq;
#if SKIA
using static SkiaSharp.HarfBuzz.SKShaper;
using System.Diagnostics;
using FastReport.Fonts;
#endif

namespace FastReport.Utils
{
    /// <summary>
    /// A wrapper around PrivateFontCollection.
    /// </summary>
    public partial class FRPrivateFontCollection
    {
        private readonly PrivateFontCollection collection = TypeConverters.FontConverter.PrivateFontCollection;
        private readonly Dictionary<string, DictionaryFont> _fonts = new Dictionary<string, DictionaryFont>();


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
            return _fonts.ContainsKey(fontName);
        }

        /// <summary>
        /// Returns the font's stream.
        /// </summary>
        /// <param name="fontName">The name of the font.</param>
        /// <returns>Either FileStream or MemoryStream containing font data.</returns>
        public Stream GetFontStream(string fontName)
        {
            if (_fonts.TryGetValue(fontName, out var font))
            {
                return font.GetFontStream();
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
            if (File.Exists(filename))
            {
                // if (!FontFiles.ContainsValue(filename))
                if (!_fonts.Values.OfType<FontFromFile>().Any(fontFile => fontFile._filepath == filename))
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
            using (FontStream fs = new FontStream(stream))
            {
                FontType font_type = TrueTypeCollection.CheckFontType(fs);
                IList<TrueTypeFont> list = TrueTypeCollection.AddFontData(font_type, fs);
                foreach (var ttf in list)
                {
                    if (!_fonts.ContainsKey(ttf.FastName))
                    {
                        stream.Position = 0;
                        var ms = new MemoryStream();
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        _fonts.Add(ttf.FastName, new FontFromStream(ms));
                    }
                    else
                        Console.WriteLine("Font {0} already registered\n", ttf.FastName);
                }
                fs.LeaveOpen = true;
            }

            stream.Position = 0;
            collection.AddFont(stream);
            stream.Position = 0;
        }
    
        public void AddTempFontStream(Stream stream)
        {

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
            if (!_fonts.ContainsKey(fontName))
                _fonts.Add(fontName, new MemoryFont(memory, length));
        }

        private abstract class DictionaryFont
        {
            public abstract Stream GetFontStream();
        }

        private sealed class FontFromFile : DictionaryFont
        {
            internal readonly string _filepath;

            public FontFromFile(string filepath)
            {
                _filepath = filepath;
            }

            public override Stream GetFontStream()
            {
                return new FileStream(_filepath, FileMode.Open, FileAccess.Read);
            }

            public override string ToString()
            {
                return _filepath;
            }
        }

        private sealed class FontFromStream : DictionaryFont
        {
            private readonly Stream _stream;

            public FontFromStream(Stream stream)
            {
                _stream = stream;
            }

            public override Stream GetFontStream()
            {
                var newStream = new MemoryStream();
                _stream.CopyTo(newStream);
                _stream.Position = 0;
                newStream.Position = 0;
                return newStream;
            }
        }

        private sealed class MemoryFont : DictionaryFont
        {
            private readonly IntPtr Memory;
            private readonly int Length;

            public MemoryFont(IntPtr memory, int length)
            {
                Memory = memory;
                Length = length;
            }

            public override Stream GetFontStream()
            {
                byte[] buffer = new byte[Length];
                Marshal.Copy(Memory, buffer, 0, Length);
                return new MemoryStream(buffer);
            }
        }
    }
}
