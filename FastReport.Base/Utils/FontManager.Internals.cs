using System;
using System.Drawing;

namespace FastReport
{
    public static partial class FontManager
    {
        // do not remove
        private static readonly FontFamilyMatcher fontFamilyMatcher = new FontFamilyMatcher();

        [Flags]
        private enum SearchScope
        {
            Temporary = 0x1,
            Private = 0x2,
            Installed = 0x4,
            
            NonInstalled = Temporary | Private,
            All = Temporary | Private | Installed
        }

        // Defines a font substitute item.
        private class FontSubstitute
        {
            private string[] _substituteList;
            private FontFamily _substituteFamily;

            public string Name { get; }

            // null value indicates that no substitute found
            public FontFamily SubstituteFamily => _substituteFamily ?? FindSubstituteFamily(SearchScope.NonInstalled);

            private FontFamily FindSubstituteFamily(SearchScope searchScope)
            {
                foreach (var item in _substituteList)
                {
                    var family = FindFontFamily(item, searchScope);
                    if (family != null)
                    {
                        return family;
                    }
                }
                return null;
            }

            public FontSubstitute(string name, params string[] substituteList)
            {
                Name = name;
                _substituteList = substituteList;
                // do initial search in installed fonts. Other collections should be checked later.
                _substituteFamily = FindSubstituteFamily(SearchScope.Installed);
            }
        }

        // used in the FR FontConverter to look up family name in all font collections
        private class FontFamilyMatcher : FastReport.TypeConverters.FontConverter.IFontFamilyMatcher
        {
            public FontFamilyMatcher()
            {
                FastReport.TypeConverters.FontConverter.FontFamilyMatcher = this;
            }

            public FontFamily GetFontFamilyOrDefault(string name) => FontManager.GetFontFamilyOrDefault(name);
        }
    }
}
