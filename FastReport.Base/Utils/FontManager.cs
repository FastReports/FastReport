using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace FastReport
{
    /// <summary>
    /// Contains font management methods and properties.
    /// </summary>
    public static partial class FontManager
    {
        // NOT THREAD SAFE!
        private static PrivateFontCollection PrivateFontCollection { get; } = new PrivateFontCollection();

        // NOT THREAD SAFE!
        // Do not update PrivateFontCollection at realtime, you must update property value then dispose previous.
        private static PrivateFontCollection TemporaryFontCollection { get; set; } = null;

        private static InstalledFontCollection InstalledFontCollection { get; } = new InstalledFontCollection();

        private static List<FontSubstitute> SubstituteFonts { get; } = new List<FontSubstitute>();

        /// <summary>
        /// Gets all installed font families.
        /// </summary>
        /// <remarks>
        /// This method enumerates all font collections (PrivateFontCollection, TemporaryFontCollection, InstalledFontCollection)
        /// and sorts the result.
        /// </remarks>
        public static FontFamily[] AllFamilies
        {
            get
            {
                var families = new List<FontFamily>();

                families.AddRange(InstalledFontCollection.Families);
                families.AddRange(PrivateFontCollection.Families);
                if (TemporaryFontCollection != null)
                {
                    families.AddRange(TemporaryFontCollection.Families);
                }

                families.Sort((x, y) => x.Name.CompareTo(y.Name));
                return families.ToArray();
            }
        }

        /// <summary>
        /// Adds a new substitute font item.
        /// </summary>
        /// <param name="originalFontName">The original font name, e.g. "Arial"</param>
        /// <param name="substituteFonts">The alternatives list, e.g. "Ubuntu Sans", "Liberation Sans", "Helvetica"</param>
        /// <remarks>
        /// Substitute font replaces the original font if it is not present on a machine.
        /// For example, you may define "Helvetica Neue" substitute for "Arial".
        /// </remarks>
        public static void AddSubstituteFont(string originalFontName, params string[] substituteFonts)
        {
            SubstituteFonts.Add(new FontSubstitute(originalFontName, substituteFonts));
        }

        /// <summary>
        /// Removes substitute fonts for the given font.
        /// </summary>
        /// <param name="originalFontName">The original font name, e.g. "Arial"</param>
        public static void RemoveSubstituteFont(string originalFontName)
        {
            for (int i = 0; i < SubstituteFonts.Count; i++)
            {
                if (SubstituteFonts[i].Name == originalFontName)
                {
                    SubstituteFonts.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Clears all substitute fonts.
        /// </summary>
        public static void ClearSubstituteFonts()
        {
            SubstituteFonts.Clear();
        }

        /// <summary>
        /// Finds a FontFamily by its name in specified font collections.
        /// </summary>
        /// <param name="name">The family name, e.g. "Arial".</param>
        /// <param name="searchScope">Search scope.</param>
        /// <returns>The FontFamily instance if found; otherwise null.</returns>
        private static FontFamily FindFontFamily(string name, SearchScope searchScope = SearchScope.All)
        {
            FontFamily family = null;
            
            if ((searchScope & SearchScope.Temporary) != 0)
            {
                family = Find(TemporaryFontCollection);
            }
            if ((searchScope & SearchScope.Private) != 0)
            {
                family ??= Find(PrivateFontCollection);
            }
            if ((searchScope & SearchScope.Installed) != 0)
            {
                family ??= Find(InstalledFontCollection);
            }
            return family;

            FontFamily Find(FontCollection collection) =>
                collection?.Families.Where(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Finds a FontFamily by its name.
        /// </summary>
        /// <param name="name">The family name, e.g. "Arial".</param>
        /// <returns>The FontFamily instance if found; otherwise default FontFamily.GenericSansSerif.</returns>
        internal static FontFamily GetFontFamilyOrDefault(string name)
        {
            var fontFamily = FindFontFamily(name);

            if (fontFamily == null)
            {
                // try to substitute
                foreach (var item in SubstituteFonts)
                {
                    if (item.Name == name)
                    {
                        // may be null!
                        fontFamily = item.SubstituteFamily; 
                        break;
                    }
                }
            }

            // return default if not found
            return fontFamily ?? FontFamily.GenericSansSerif;
        }
    }
}
