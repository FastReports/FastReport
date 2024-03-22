using System;
using System.Drawing;
using System.Linq;

namespace FastReport.Utils
{
    internal enum MonoRendering
    {
        Undefined,
        Pango,
        Cairo
    }

    public static partial class DrawUtils
    {
        private static Font FDefaultFont;
        private static Font FDefaultReportFont;
        private static Font FDefaultTextObjectFont;
        private static Font FFixedFont;
        private static int FScreenDpi;
        private static float FDpiFX;
        private static MonoRendering FMonoRendering = MonoRendering.Undefined;

        public static int ScreenDpi
        {
            get
            {
                if (FScreenDpi == 0)
                    FScreenDpi = GetDpi();
                return FScreenDpi;
            }
        }

        public static float ScreenDpiFX
        {
            get
            {
                if (FDpiFX == 0f)
                    FDpiFX = 96f / DrawUtils.ScreenDpi;
                return FDpiFX;
            }
        }

        private static int GetDpi()
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                return (int)g.DpiX;
            }
        }

        public static Font DefaultFont
        {
            get
            {
                if (FDefaultFont == null)
                {
#if AVALONIA
                    if (OperatingSystem.IsWindows())
                    {
                        FDefaultFont = new Font("Segoe UI", 8.5f);
                    }
                    else if (OperatingSystem.IsMacOS())
                    {
                        FDefaultFont = new Font("Helvetica Neue", 8.5f);
                    }
                    else if (OperatingSystem.IsLinux())
                    {
                        FDefaultFont = new Font("Liberation Sans", 8.5f);
                    }
#else
                    switch (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                    {
                        case "ja":
                            FDefaultFont = CreateFont("MS UI Gothic", 9);
                            break;

                        case "zh":
                            FDefaultFont = CreateFont("SimSun", 9);
                            break;

                        default:
#if WPF
                            FDefaultFont = CreateFont("Segoe UI", 8.5f);
#else
                            FDefaultFont = CreateFont("Tahoma", 8);
#endif
                            break;
                    }
#endif
                }
                return FDefaultFont;
            }
            set
            {
                FDefaultFont = value;
            }
        }

        public static Font DefaultReportFont
        {
            get
            {
                if (FDefaultReportFont == null)
                {
                    switch (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                    {
                        case "ja":
                            FDefaultReportFont = CreateFont("MS UI Gothic", 9);
                            break;

                        case "zh":
                            FDefaultReportFont = CreateFont("SimSun", 9);
                            break;

                        default:
                            FDefaultReportFont = CreateFont("Arial", 10);
                            break;
                    }
                }
                return FDefaultReportFont;
            }
        }

        public static Font DefaultTextObjectFont
        {
            get
            {
                if (FDefaultTextObjectFont == null)
                    FDefaultTextObjectFont = CreateFont("Arial", 10);
                return FDefaultTextObjectFont;
            }
        }

        public static Font FixedFont
        {
            get
            {
                if (FFixedFont == null)
#if WPF
                    FFixedFont = CreateFont("Consolas", 9);
#elif AVALONIA
                    if (OperatingSystem.IsWindows())
                    {
                        FFixedFont = CreateFont("Lucida Console", 9);
                    }
                    else if (OperatingSystem.IsMacOS())
                    {
                        FFixedFont = CreateFont("PT Mono", 9);
                    }
                    else if (OperatingSystem.IsLinux())
                    {
                        FFixedFont = CreateFont("Liberation Mono", 9);
                    }
#else
                    FFixedFont = CreateFont("Courier New", 10);
#endif
                return FFixedFont;
            }
        }

        internal static Font CreateFont(string familyName, float emSize,
            FontStyle style = FontStyle.Regular,
            GraphicsUnit unit = GraphicsUnit.Point,
            byte gdiCharSet = 1,
            bool gdiVerticalFont = false)
        {
            Font font = new Font(familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont);

// skia now handles Font instantiation correctly
/*#if SKIA
            if (font.Name != familyName)
            {
                // font family not found in installed fonts, search in the user fonts
                font = new Font(familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont, Config.PrivateFontCollection.Collection);
            }
#endif*/
                    return font;
        }

        public static SizeF MeasureString(string text)
        {
            return MeasureString(text, DefaultFont);
        }

        public static SizeF MeasureString(string text, Font font)
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            using (StringFormat sf = new StringFormat())
            {
                Graphics g = Graphics.FromImage(bmp);
                return MeasureString(g, text, font, sf);
            }
        }

        public static SizeF MeasureString(Graphics g, string text, Font font, StringFormat format)
        {
            return MeasureString(g, text, font, new RectangleF(0, 0, 10000, 10000), format);
        }

        public static SizeF MeasureString(Graphics g, string text, Font font, RectangleF layoutRect, StringFormat format)
        {
            if (String.IsNullOrEmpty(text))
                return new SizeF(0, 0);
            CharacterRange[] characterRanges = { new CharacterRange(0, text.Length) };
            StringFormatFlags saveFlags = format.FormatFlags;
            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            format.SetMeasurableCharacterRanges(characterRanges);
            Region[] regions = g.MeasureCharacterRanges(text, font, layoutRect, format);
            format.FormatFlags = saveFlags;
            RectangleF rect = regions[0].GetBounds(g);
            regions[0].Dispose();
            return rect.Size;
        }

        internal static MonoRendering GetMonoRendering(IGraphics printerGraphics)
        {
            if (FMonoRendering == MonoRendering.Undefined)
            {
                GraphicsUnit savedUnit = printerGraphics.PageUnit;
                printerGraphics.PageUnit = GraphicsUnit.Point;

                const string s = "test string test string test string test string";
                float f1 = printerGraphics.MeasureString(s, DefaultReportFont).Width;
                FMonoRendering = f1 > 200 ? MonoRendering.Pango : MonoRendering.Cairo;

                printerGraphics.PageUnit = savedUnit;
            }
            return FMonoRendering;
        }


        /// <summary>
        /// The method adjusts the dotted line style for the <see cref="Pen"/> in a graphical context.
        /// </summary>
        /// <param name="dashPattern">Collection of values for custom dash pattern.</param>
        /// <param name="pen">Pen for lines.</param>
        /// <param name="border">Border around the report object.</param>
        /// <remarks>
        /// If a <c>DashPattern</c> pattern is specified and contains elements, the method checks each element.
        /// If the element is less than or equal to 0, it is replaced by 1.<br/>
        /// Then the resulting array of patterns is converted to the <see cref="float"/> type and set as a dotted line pattern for the <see cref="Pen"/>.<br/>
        /// If the pattern is empty or not specified,
        /// the method sets the style of the dotted line of the <see cref="Pen"/> equal to the style of the dotted line of the <see cref="Border"/> object.
        ///</remarks>
        internal static void SetPenDashPatternOrStyle(FloatCollection dashPattern, Pen pen, Border border)
        {
            if (dashPattern?.Count > 0)
            {
                for (int i = 0; i < dashPattern.Count; i++)
                {
                    if (dashPattern[i] <= 0)
                        dashPattern[i] = 1;
                }
                pen.DashPattern = dashPattern.Cast<float>().ToArray();
            }
            else
            {
                pen.DashStyle = border.DashStyle;
            }
        }
    }
}