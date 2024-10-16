// available in FR.OS, FR.NET, FR.WPF
#if !SKIA && !FRCORE && (!MONO || WPF)
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace FastReport
{
    public static partial class FontManager
    {
        /// <summary>
        /// Adds a font from the specified file to this collection.
        /// </summary>
        /// <param name="filename">A System.String that contains the file name of the font to add.</param>
        /// <returns>true if the font is registered by application.</returns>
        public static bool AddFont(string filename)
        {
            bool success = false;
            if (File.Exists(filename))
            {

                bool isInstalled = CheckFontIsInstalled(filename);

                if (!isInstalled)
                {
                    PrivateFontCollection.AddFontFile(filename);

                    success = true;
                }
            }
            else
            {
                Debug.WriteLine($"Font file '{filename}' not found");
            }
            return success;
        }

        /// <summary>
        /// Checks whether the font from the specified file is installed in the system.
        /// </summary>
        /// <param name="filename">The path to the font file.</param>
        /// <returns>Returns true if the font is installed on the system, otherwise false.</returns>
        public static bool CheckFontIsInstalled(string filename)
        {
            PrivateFontCollection tempFontCollection = new PrivateFontCollection();
            tempFontCollection.AddFontFile(filename);
            string fontName = tempFontCollection.Families[0].Name;


            InstalledFontCollection installedFonts = new InstalledFontCollection();
            FontFamily[] fontFamilies = installedFonts.Families;

            // Checking if a font named FontName is installed in the system
            // Array.Exists checks if an element in the array exists that satisfies the condition
           bool isInstalled = Array.Exists(fontFamilies, family => family.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));

            return isInstalled;
        }

        /// <summary>
        /// Adds a font contained in system memory to this collection.
        /// </summary>
        /// <param name="memory">The memory address of the font to add.</param>
        /// <param name="length">The memory length of the font to add.</param>
        public static void AddFont(IntPtr memory, int length)
        {
            PrivateFontCollection.AddMemoryFont(memory, length);
        }
    }
}
#endif