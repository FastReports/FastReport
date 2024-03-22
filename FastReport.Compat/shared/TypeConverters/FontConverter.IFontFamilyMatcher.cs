using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace FastReport.TypeConverters
{
    public partial class FontConverter : TypeConverter
    {
        public static IFontFamilyMatcher FontFamilyMatcher { get; set; } = new DefaultFontFamilyMatcher();

        public interface IFontFamilyMatcher
        {
            FontFamily GetFontFamilyOrDefault(string name);
        }

        private class DefaultFontFamilyMatcher : IFontFamilyMatcher
        {
            public FontFamily GetFontFamilyOrDefault(string name)
            {
                var fontFamily = FontFamily.Families.Where(f => f.Name == name).FirstOrDefault();
                return fontFamily ?? FontFamily.GenericSansSerif;
            }
        }
    }
}
