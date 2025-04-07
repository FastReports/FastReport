using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace FastReport.Utils
{
    public class HtmlTextRenderer : IDisposable
    {
        #region Definitions
        /// <summary>
        /// Context for HTML rendering. <br/>
        /// Using this structure instead of the class's private fields is recommended. <br/>
        /// This allows for future optimizations and helps avoid constructors with numerous arguments.
        /// </summary>
        public struct RendererContext
        {
            internal int angle;
            internal float widthRatio;
            internal string text;
            internal IGraphics g;
            internal FontFamily font;
            internal float size;
            internal FontStyle style; // no keep
            internal Color color; // no keep
            internal Color underlineColor;
            internal RectangleF rect;
            internal bool underlines;
            internal StringFormat format; // no keep
            internal HorzAlign horzAlign;
            internal VertAlign vertAlign;
            internal ParagraphFormat paragraphFormat;
            internal bool forceJustify;
            internal float scale;
            internal float fontScale;
            internal InlineImageCache cache;
            internal bool isPrinting;
            internal bool isDifferentTabPositions;
            internal bool keepLastLineSpace; // Classic objects need false, translated objects need true
        }
        #endregion

        #region Internal Fields

        public static readonly System.Globalization.CultureInfo CultureInfo = System.Globalization.CultureInfo.InvariantCulture;

        #endregion Internal Fields

        #region Private Fields

        private int angle;
        private float widthRatio;
        private const char SOFT_ENTER = '\u2028';
        private List<RectangleFColor> backgrounds;
        private InlineImageCache cache;
        private RectangleF displayRect;
        private bool everUnderlines;
        private FontFamily font;
        private float fontLineHeight;
        private float scale;
        private bool forceJustify;
        private StringFormat format;
        private IGraphics graphics;
        private HorzAlign horzAlign;
        private ParagraphFormat paragraphFormat;
        private List<HtmlTextRenderer.Paragraph> paragraphs;
        private bool rightToLeft;
        private float size;
        private List<LineFColor> strikeouts;
        private string text;
        private Color underlineColor;
        private List<LineFColor> underlines;
        private VertAlign vertAlign;
        private StyleDescriptor initalStyle;
        private float fontScale;
        private FastString cacheString = new FastString(100);
        private bool isPrinting;
        private bool isDifferentTabPositions;
        internal bool keepLastSpace = false;

        #endregion Private Fields

        #region Public Properties

        public IEnumerable<RectangleFColor> Backgrounds { get { return backgrounds; } }
        public RectangleF DisplayRect { get { return displayRect; } }
        public float Scale { get { return scale; } }
        public float FontScale { get { return fontScale; } set { fontScale = value; } }
        public HorzAlign HorzAlign { get { return horzAlign; } }
        public ParagraphFormat ParagraphFormat { get { return paragraphFormat; } }
        public IEnumerable<Paragraph> Paragraphs { get { return paragraphs; } }

        public bool RightToLeft
        {
            get { return rightToLeft; }
        }

        public IEnumerable<LineFColor> Stikeouts { get { return strikeouts; } }

        public float[] TabPositions
        {
            get
            {
                float firstTabStop;
                return format.GetTabStops(out firstTabStop);
            }
        }

        public float TabSize
        {
            get
            {
                // re fix tab offset #2823 sorry linux users, on linux firstTab is firstTab not tabSizes[0]
                float[] tabSizes = TabPositions;
                if (tabSizes.Length > 1)
                    return tabSizes[1];
                return 0;
            }
        }

        public float TabOffset
        {
            get
            {
                // re fix tab offset #2823 sorry linux users, on linux firstTab is firstTab not tabSizes[0]
                float[] tabSizes = TabPositions;
                if (tabSizes.Length > 0)
                    return tabSizes[0];
                return 0;
            }
        }

        public IEnumerable<LineFColor> Underlines { get { return underlines; } }

        public bool WordWrap
        {
            get { return (format.FormatFlags & StringFormatFlags.NoWrap) == 0; }
        }

        /// <summary>
        /// Gets the angle of rotation.
        /// </summary>
        public int Angle
        {
            get { return angle; }
        }

        /// <summary>
        /// Gets the width ratio of the object.
        /// </summary>
        public float WidthRatio
        {
            get { return widthRatio; }
        }

        #endregion Public Properties

        ////TODO this is a problem with dotnet, because typographic width
        ////float width_dotnet = 2.7f;

        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the HTML text renderer with a specified rendering context.
        /// </summary>
        /// <param name="context">The rendering context for the HTML renderer.</param>
        public HtmlTextRenderer(RendererContext context)
        {
            this.angle = context.angle % 360;
            this.widthRatio = context.widthRatio;
            this.text = context.text;
            this.graphics = context.g;
            this.font = context.font;
            this.size = context.size;
            this.underlineColor = context.underlineColor;
            this.displayRect = context.rect;
            this.everUnderlines = context.underlines;
            this.format = context.format;
            this.horzAlign = context.horzAlign;
            this.vertAlign = context.vertAlign;
            this.paragraphFormat = context.paragraphFormat;
            this.forceJustify = context.forceJustify;
            this.scale = context.scale;
            this.fontScale = context.fontScale;
            this.cache = context.cache;
            this.isPrinting = context.isPrinting;
            this.isDifferentTabPositions = context.isDifferentTabPositions;
            this.keepLastSpace = context.keepLastLineSpace;

            paragraphs = new List<HtmlTextRenderer.Paragraph>();
            rightToLeft = (context.format.FormatFlags & StringFormatFlags.DirectionRightToLeft) == StringFormatFlags.DirectionRightToLeft;
            // Dispose it
            this.format = StringFormat.GenericTypographic.Clone() as StringFormat;
            if (RightToLeft)
                this.format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            float firstTab;
            float[] tabs = context.format.GetTabStops(out firstTab);
            this.format.SetTabStops(firstTab, tabs);
            this.format.Alignment = StringAlignment.Near;
            this.format.LineAlignment = StringAlignment.Near;
            this.format.Trimming = StringTrimming.None;
            this.format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
            //FFormat.DigitSubstitutionMethod = StringDigitSubstitute.User;
            //FFormat.DigitSubstitutionLanguage = 0;
            this.format.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit;
            //FFormat.FormatFlags |= StringFormatFlags.NoFontFallback;

            backgrounds = new List<RectangleFColor>();
            this.underlines = new List<LineFColor>();
            strikeouts = new List<LineFColor>();
            //FDisplayRect.Width -= width_dotnet * scale;

            initalStyle = new StyleDescriptor(context.style, context.color, BaseLine.Normal, this.font, this.size * this.fontScale);
            using (Font f = initalStyle.GetFont())
            {
                fontLineHeight = f.GetHeight(graphics.Graphics);
            }


            StringFormatFlags saveFlags = this.format.FormatFlags;
            StringTrimming saveTrimming = this.format.Trimming;

            // if word wrap is set, ignore trimming
            if (WordWrap)
                this.format.Trimming = StringTrimming.Word;

            SplitToParagraphs(text);
            AdjustParagraphLines();

            // restore original values
            displayRect = context.rect;
            this.format.FormatFlags = saveFlags;
            this.format.Trimming = saveTrimming;
        }

        public HtmlTextRenderer(string text, IGraphics g, FontFamily font, float size,
                    FontStyle style, Color color, Color underlineColor, RectangleF rect, bool underlines,
                    StringFormat format, HorzAlign horzAlign, VertAlign vertAlign,
                    ParagraphFormat paragraphFormat, bool forceJustify, float scale, float fontScale, InlineImageCache cache, bool isPrinting = false, bool isDifferentTabPositions = false)
        {
            this.cache = cache;
            this.scale = scale;
            this.fontScale = fontScale;
            paragraphs = new List<HtmlTextRenderer.Paragraph>();
            this.text = text;
            graphics = g;
            this.font = font;
            displayRect = rect;
            rightToLeft = (format.FormatFlags & StringFormatFlags.DirectionRightToLeft) == StringFormatFlags.DirectionRightToLeft;
            // Dispose it
            this.format = StringFormat.GenericTypographic.Clone() as StringFormat;
            if (RightToLeft)
                this.format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            float firstTab;
            float[] tabs = format.GetTabStops(out firstTab);
            this.format.SetTabStops(firstTab, tabs);
            this.format.Alignment = StringAlignment.Near;
            this.format.LineAlignment = StringAlignment.Near;
            this.format.Trimming = StringTrimming.None;
            this.format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
            this.underlineColor = underlineColor;
            //FFormat.DigitSubstitutionMethod = StringDigitSubstitute.User;
            //FFormat.DigitSubstitutionLanguage = 0;
            this.format.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit;
            //FFormat.FormatFlags |= StringFormatFlags.NoFontFallback;
            this.horzAlign = horzAlign;
            this.vertAlign = vertAlign;
            this.paragraphFormat = paragraphFormat;
            this.font = font;
            this.size = size;
            this.isPrinting = isPrinting;
            this.isDifferentTabPositions = isDifferentTabPositions;
            everUnderlines = underlines;

            backgrounds = new List<RectangleFColor>();
            this.underlines = new List<LineFColor>();
            strikeouts = new List<LineFColor>();
            //FDisplayRect.Width -= width_dotnet * scale;

            initalStyle = new StyleDescriptor(style, color, BaseLine.Normal, this.font, this.size * this.fontScale);
            using (Font f = initalStyle.GetFont())
            {
                fontLineHeight = f.GetHeight(g.Graphics);
            }

            this.forceJustify = forceJustify;

            StringFormatFlags saveFlags = this.format.FormatFlags;
            StringTrimming saveTrimming = this.format.Trimming;

            // if word wrap is set, ignore trimming
            if (WordWrap)
                this.format.Trimming = StringTrimming.Word;

            SplitToParagraphs(text);
            AdjustParagraphLines();

            // restore original values
            displayRect = rect;
            this.format.FormatFlags = saveFlags;
            this.format.Trimming = saveTrimming;
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddUnknownWord(List<CharWithIndex> w, Paragraph paragraph, StyleDescriptor style, int charIndex, ref Line line, ref Word word, ref float width, ref int tabIndex)
        {
            if (w[0].Char == ' ')
            {
                if (word == null || word.Type == WordType.Normal)
                {
                    word = new Word(this, line, WordType.WhiteSpace);
                    line.Words.Add(word);
                }
                Run r = new RunText(this, word, style, w, width, charIndex);
                word.Runs.Add(r);
                width += r.Width;
                if (width > displayRect.Width)
                    line = WrapLine(paragraph, line, charIndex, displayRect.Width, ref width, ref word, ref tabIndex);
            }
            else
            {
                if (word == null || word.Type != WordType.Normal)
                {
                    word = new Word(this, line, WordType.Normal);
                    line.Words.Add(word);
                }
                Run r = new RunText(this, word, style, w, width, charIndex);
                word.Runs.Add(r);
                width += r.Width;
                if (width > displayRect.Width)
                    line = WrapLine(paragraph, line, charIndex, displayRect.Width, ref width, ref word, ref tabIndex);
            }
        }

        public float CalcHeight()
        {
            int charsFit = 0;
            return CalcHeight(out charsFit);
        }

        public float CalcHeight(out int charsFit)
        {
            charsFit = -1;
            float height = 0;
            float displayHeight = displayRect.Height;
            float lineSpacing = 0;

            foreach (Paragraph paragraph in paragraphs)
            {
                foreach (Line line in paragraph.Lines)
                {
                    line.CalcMetrics();
                    height += line.Height;
                    if (charsFit < 0 && height > displayHeight)
                    {
                        charsFit = line.OriginalCharIndex;
                    }
                    height += lineSpacing = line.LineSpacing;
                }
            }
            if (!keepLastSpace) // It looks like TextProcessors keep this value for every line. 
                height -= lineSpacing;

            if (charsFit < 0)
                charsFit = text.Length;
            return height;
        }

        public float CalcWidth()
        {
            float width = 0;

            foreach (Paragraph paragraph in paragraphs)
            {
                foreach (Line line in paragraph.Lines)
                {
                    if (width < line.Width)
                        width = line.Width;
                }
            }
            return width;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Returns splited string
        /// </summary>
        /// <param name="text">text for splitting</param>
        /// <param name="charactersFitted">index of first character of second string</param>
        /// <param name="result">second part of string</param>
        /// <param name="endOnEnter">returns true if ends on enter</param>
        /// <returns>first part of string</returns>
        internal static string BreakHtml(string text, int charactersFitted, out string result, out bool endOnEnter)
        {
            endOnEnter = false;
            Stack<SimpleFastReportHtmlElement> elements = new Stack<SimpleFastReportHtmlElement>();
            SimpleFastReportHtmlReader reader = new SimpleFastReportHtmlReader(text);
            while (reader.IsNotEOF)
            {
                if (reader.Position >= charactersFitted)
                {
                    StringBuilder firstPart = new StringBuilder();
                    if (reader.Character.Char == SOFT_ENTER)
                        firstPart.Append(text.Substring(0, reader.LastPosition));
                    else
                        firstPart.Append(text.Substring(0, reader.Position));
                    foreach (SimpleFastReportHtmlElement el in elements)
                    {
                        SimpleFastReportHtmlElement el2 = new SimpleFastReportHtmlElement(el.name, true);
                        firstPart.Append(el2.ToString());
                    }

                    SimpleFastReportHtmlElement[] arr = elements.ToArray();

                    StringBuilder secondPart = new StringBuilder();
                    for (int i = arr.Length - 1; i >= 0; i--)
                        secondPart.Append(arr[i].ToString());
                    secondPart.Append(text.Substring(reader.Position));
                    endOnEnter = reader.Character.Char == '\n';
                    result = secondPart.ToString();
                    return firstPart.ToString();
                }
                if (!reader.Read())
                {
                    if (reader.Element.isEnd)
                    {
                        int enumIndex = 1;
                        using (Stack<SimpleFastReportHtmlElement>.Enumerator enumerator = elements.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                SimpleFastReportHtmlElement el = enumerator.Current;
                                if (el.name == reader.Element.name)
                                {
                                    for (int i = 0; i < enumIndex; i++)
                                        elements.Pop();
                                    break;
                                }
                                else
                                    enumIndex++;
                            }
                        }
                    }
                    else if (!reader.Element.IsSelfClosed) elements.Push(reader.Element);
                }
            }
            result = "";
            return text;
        }

        internal void Draw()
        {
            // set clipping
            IGraphicsState state = graphics.Save();
            RectangleF dRect = displayRect;
            // round x and y to an integer to avoid clipping the characters of the first line
            dRect.Inflate(displayRect.Left % 1, displayRect.Top % 1);
            graphics.SetClip(dRect, CombineMode.Intersect);

            if (Angle != 0)
            {
                PointF center = new PointF(displayRect.Left + displayRect.Width / 2, displayRect.Top + displayRect.Height / 2);

                // Translate the origin to the center of the rectangle
                graphics.TranslateTransform(center.X, center.Y);

                // Rotate the graphics by the specified angle
                graphics.RotateTransform(Angle);

                // Translate the origin back to the original position
                graphics.TranslateTransform(-center.X, -center.Y);
            }

            // reset alignment
            //StringAlignment saveAlign = FFormat.Alignment;
            //StringAlignment saveLineAlign = FFormat.LineAlignment;
            //FFormat.Alignment = StringAlignment.Near;
            //FFormat.LineAlignment = StringAlignment.Near;

            //if (FRightToLeft)
            //    foreach (RectangleFColor rect in FBackgrounds)
            //        using (Brush brush = new SolidBrush(rect.Color))
            //            FGraphics.FillRectangle(brush, rect.Left - rect.Width, rect.Top, rect.Width, rect.Height);
            //else
            foreach (RectangleFColor rect in backgrounds)
                using (Brush brush = new SolidBrush(rect.Color))
                    graphics.FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);

            foreach (Paragraph p in paragraphs)
                foreach (Line l in p.Lines)
                {
                    //#if DEBUG
                    //                    FGraphics.DrawRectangle(Pens.Blue, FDisplayRect.Left, l.Top, FDisplayRect.Width, l.Height);
                    //#endif
                    foreach (Word w in l.Words)
                        switch (w.Type)
                        {
                            case WordType.Normal:
                                foreach (Run r in w.Runs)
                                {
                                    r.Draw();
                                }
                                break;
                        }
                }

            //if (RightToLeft)
            //{
            //    foreach (LineFColor line in FUnderlines)
            //        using (Pen pen = new Pen(line.Color, line.Width))
            //            FGraphics.DrawLine(pen, 2 * line.Left - line.Right, line.Top, line.Left, line.Top);

            //    foreach (LineFColor line in FStrikeouts)
            //        using (Pen pen = new Pen(line.Color, line.Width))
            //            FGraphics.DrawLine(pen, 2 * line.Left - line.Right, line.Top, line.Left, line.Top);
            //}
            //else
            //{
            foreach (LineFColor line in underlines)
                using (Pen pen = new Pen(line.Color, line.Width))
                    graphics.DrawLine(pen, line.Left, line.Top, line.Right, line.Top);

            foreach (LineFColor line in strikeouts)
                using (Pen pen = new Pen(line.Color, line.Width))
                    graphics.DrawLine(pen, line.Left, line.Top, line.Right, line.Top);
            //}

            // restore alignment and clipping
            //FFormat.Alignment = saveAlign;
            //FFormat.LineAlignment = saveLineAlign;
            graphics.Restore(state);
        }

        #endregion Internal Methods

        #region Private Methods

        private void AdjustParagraphLines()
        {
            // calculate text height
            float height = 0;
            height = CalcHeight();

            // calculate Y offset
            float offsetY = displayRect.Top;
            if (vertAlign == VertAlign.Center)
                offsetY += (displayRect.Height - height) / 2;
            else if (vertAlign == VertAlign.Bottom)
                offsetY += (displayRect.Height - height) - 1;

            for (int i = 0; i < paragraphs.Count; i++)
            {
                Paragraph paragraph = paragraphs[i];
                paragraph.AlignLines(i == paragraphs.Count - 1 && forceJustify);

                // adjust line tops
                foreach (Line line in paragraph.Lines)
                {
                    line.Top = offsetY;
                    line.MakeUnderlines();
                    line.MakeStrikeouts();
                    line.MakeBackgrounds();
                    offsetY += line.Height + line.LineSpacing;
                }
            }
        }

        private void CssStyle(StyleDescriptor style, Dictionary<string, string> dict)
        {
            if (dict == null)
                return;
            string tStr;

            // If "font-style" contains "italic" or "oblique", apply the Italic style to the text.
            if (dict.TryGetValue("font-style", out tStr))
            {
                if (tStr.Contains("italic") || tStr.Contains("oblique"))
                    style.FontStyle |= FontStyle.Italic;
            }

            // If "font-weight" contains "bold", apply the Bold style to the text.
            if (dict.TryGetValue("font-weight", out tStr))
            {
                if (tStr.Contains("bold"))
                    style.FontStyle |= FontStyle.Bold;
            }

            // If "text-decoration" contains both "underline" and "line-through", apply both styles to the text.
            // Otherwise, check and apply each style individually.
            if (dict.TryGetValue("text-decoration", out tStr))
            {
                if (tStr.Contains("underline") && tStr.Contains("line-through"))
                    style.FontStyle |= FontStyle.Underline | FontStyle.Strikeout;
                else
                {
                    if (tStr.Contains("underline"))
                        style.FontStyle |= FontStyle.Underline;
                    if (tStr.Contains("line-through"))
                        style.FontStyle |= FontStyle.Strikeout;
                }
            }

            if (dict.TryGetValue("font-size", out tStr))
            {
                if (EndsWith(tStr, "px"))
                    try { style.Size = fontScale * 0.75f * Single.Parse(tStr.Substring(0, tStr.Length - 2), CultureInfo); } catch { }
                else if (EndsWith(tStr, "pt"))
                    try { style.Size = fontScale * Single.Parse(tStr.Substring(0, tStr.Length - 2), CultureInfo); } catch { }
                else if (EndsWith(tStr, "em"))
                    try { style.Size *= Single.Parse(tStr.Substring(0, tStr.Length - 2), CultureInfo); } catch { }
            }
            if (dict.TryGetValue("font-family", out tStr))
            {
                // checks if there are single quotation marks in the font name
                if (tStr.Contains("'"))
                    tStr = tStr.Replace("'", ""); // Removes all single quotes
                style.Font = new FontFamily(tStr);
            }
   
            if (dict.TryGetValue("color", out tStr))
            {
                if (StartsWith(tStr, "#"))
                    try { style.Color = Color.FromArgb((int)(0xFF000000 + uint.Parse(tStr.Substring(1), System.Globalization.NumberStyles.HexNumber))); } catch { }
                else if (StartsWith(tStr, "rgba"))
                {
                    int i1 = tStr.IndexOf('(');
                    int i2 = tStr.IndexOf(')');
                    string[] strs = tStr.Substring(i1 + 1, i2 - i1 - 1).Split(',');
                    if (strs.Length == 4)
                    {
                        float r, g, b, a;
                        try
                        {
                            r = Single.Parse(strs[0], CultureInfo);
                            g = Single.Parse(strs[1], CultureInfo);
                            b = Single.Parse(strs[2], CultureInfo);
                            a = Single.Parse(strs[3], CultureInfo);
                            style.Color = Color.FromArgb((int)(a * 0xFF), (int)r, (int)g, (int)b);
                        }
                        catch { }
                    }
                }
                else if (StartsWith(tStr, "rgb"))
                {
                    int i1 = tStr.IndexOf('(');
                    int i2 = tStr.IndexOf(')');
                    string[] strs = tStr.Substring(i1 + 1, i2 - i1 - 1).Split(',');
                    if (strs.Length == 3)
                    {
                        float r, g, b;
                        try
                        {
                            r = Single.Parse(strs[0], CultureInfo);
                            g = Single.Parse(strs[1], CultureInfo);
                            b = Single.Parse(strs[2], CultureInfo);
                            style.Color = Color.FromArgb((int)r, (int)g, (int)b);
                        }
                        catch { }
                    }
                }
                else style.Color = Color.FromName(tStr);
            }

            if (dict.TryGetValue("background-color", out tStr))
            {
                if (StartsWith(tStr, "#"))
                    try { style.BackgroundColor = Color.FromArgb((int)(0xFF000000 + uint.Parse(tStr.Substring(1), System.Globalization.NumberStyles.HexNumber))); } catch { }
                else if (StartsWith(tStr, "rgba"))
                {
                    int i1 = tStr.IndexOf('(');
                    int i2 = tStr.IndexOf(')');
                    string[] strs = tStr.Substring(i1 + 1, i2 - i1 - 1).Split(',');
                    if (strs.Length == 4)
                    {
                        float r, g, b, a;
                        try
                        {
                            r = Single.Parse(strs[0], CultureInfo);
                            g = Single.Parse(strs[1], CultureInfo);
                            b = Single.Parse(strs[2], CultureInfo);
                            a = Single.Parse(strs[3], CultureInfo);
                            style.BackgroundColor = Color.FromArgb((int)(a * 0xFF), (int)r, (int)g, (int)b);
                        }
                        catch { }
                    }
                }
                else if (StartsWith(tStr, "rgb"))
                {
                    int i1 = tStr.IndexOf('(');
                    int i2 = tStr.IndexOf(')');
                    string[] strs = tStr.Substring(i1 + 1, i2 - i1 - 1).Split(',');
                    if (strs.Length == 3)
                    {
                        float r, g, b;
                        try
                        {
                            r = Single.Parse(strs[0], CultureInfo);
                            g = Single.Parse(strs[1], CultureInfo);
                            b = Single.Parse(strs[2], CultureInfo);
                            style.BackgroundColor = Color.FromArgb((int)r, (int)g, (int)b);
                        }
                        catch { }
                    }
                }
                else style.BackgroundColor = Color.FromName(tStr);
            }
        }

        private bool EndsWith(string str1, string str2)
        {
            int len1 = str1.Length;
            int len2 = str2.Length;
            if (len1 < len2) return false;
            switch (len2)
            {
                case 0: return true;
                case 1: return str1[len1 - 1] == str2[len2 - 1];
                case 2: return str1[len1 - 1] == str2[len2 - 1] && str1[len1 - 2] == str2[len2 - 2];
                case 3: return str1[len1 - 1] == str2[len2 - 1] && str1[len1 - 2] == str2[len2 - 2] && str1[len1 - 3] == str2[len2 - 3];
                case 4: return str1[len1 - 1] == str2[len2 - 1] && str1[len1 - 2] == str2[len2 - 2] && str1[len1 - 3] == str2[len2 - 3] && str1[len1 - 4] == str2[len2 - 4];
                default: return str1.EndsWith(str2);
            }
        }

        private float GetTabPosition(float pos)
        {
            float tabOffset = TabOffset;
            float tabSize = TabSize;
            int tabPosition = (int)((pos - tabOffset) / tabSize);
            if (pos < tabOffset)
                return tabOffset;
            return (tabPosition + 1) * tabSize + tabOffset;
        }

        private float GetTabPosition(float pos, int tabIndex)
        {
            float tabOffset = 0;
            float tabSize;
            float firstTabOffset;
            float[] tabsPos = format.GetTabStops(out firstTabOffset);
            if (tabIndex <= 1)
            {
                tabOffset = TabOffset;
                tabSize = TabSize;
            }
            else
            {
                for (int i = 0; i < tabIndex; i++)
                {
                    tabOffset += tabsPos[i];
                }
                tabSize = tabsPos[tabIndex];
            }
            int tabPosition = (int)((pos - tabOffset) / tabSize);
            if (pos < tabOffset)
                return tabOffset;
            return (tabPosition + 1) * tabSize + tabOffset;
        }

        private void SplitToParagraphs(string text)
        {
            Stack<SimpleFastReportHtmlElement> elements = new Stack<SimpleFastReportHtmlElement>();
            SimpleFastReportHtmlReader reader = new SimpleFastReportHtmlReader(this.text);
            List<CharWithIndex> currentWord = new List<CharWithIndex>();
            float width = paragraphFormat.SkipFirstLineIndent ? 0 : GetStartPosition(true);
            Paragraph paragraph = new Paragraph(this);
            int charIndex = 0;
            int tabIndex = 0;
            Line line = new Line(this, paragraph, charIndex);
            paragraph.Lines.Add(line);
            paragraphs.Add(paragraph);
            Word word = null;
            StyleDescriptor style = new StyleDescriptor(initalStyle);
            //bool softReturn = false;
            //CharWithIndex softReturnChar = new CharWithIndex();

            while (reader.IsNotEOF)
            {
                if (reader.Read())
                {
                    switch (reader.Character.Char)
                    {
                        case ' ':
                            if (word == null)
                            {
                                word = new Word(this, line, WordType.WhiteSpace);
                                line.Words.Add(word);
                            }
                            if (word.Type == WordType.WhiteSpace)
                                currentWord.Add(reader.Character);
                            else
                            {
                                if (currentWord.Count > 0)
                                {
                                    Run r = new RunText(this, word, style, currentWord, width, charIndex);
                                    word.Runs.Add(r);
                                    currentWord.Clear();
                                    width += r.Width;
                                    if (width > displayRect.Width)
                                        line = WrapLine(paragraph, line, charIndex, displayRect.Width, ref width, ref word, ref tabIndex);
                                }
                                currentWord.Add(reader.Character);
                                word = new Word(this, line, WordType.WhiteSpace);
                                line.Words.Add(word);
                                charIndex = reader.LastPosition;
                            }
                            break;

                        case '\t':
                            if (word != null)
                            {
                                if (currentWord.Count > 0)
                                {
                                    Run r = new RunText(this, word, style, currentWord, width, charIndex);
                                    word.Runs.Add(r);
                                    currentWord.Clear();
                                    width += r.Width;
                                    if (width > displayRect.Width)
                                        line = WrapLine(paragraph, line, charIndex, displayRect.Width, ref width, ref word, ref tabIndex);
                                }
                            }
                            else
                            {
                                if (currentWord.Count > 0)
                                {
                                    AddUnknownWord(currentWord, paragraph, style, charIndex, ref line, ref word, ref width, ref tabIndex);
                                }
                            }
                            charIndex = reader.LastPosition;

                            word = new Word(this, line, WordType.Tab);

                            Run tabRun = new RunText(this, word, style, new List<CharWithIndex>(new CharWithIndex[] { reader.Character }), width, charIndex);
                            word.Runs.Add(tabRun);
                            float width2 = GetTabPosition(width);
                            if (isDifferentTabPositions)
                            {
                                width2 = GetTabPosition(width, tabIndex);
                            }
                            if (width2 < width) width2 = width;
                            if (line.Words.Count > 0 && width2 > displayRect.Width)
                            {
                                tabRun.Left = 0;
                                line = new Line(this, paragraph, charIndex);
                                tabIndex = 0;
                                paragraph.Lines.Add(line);
                                width = 0;
                                width2 = GetTabPosition(width);
                                if (isDifferentTabPositions)
                                {
                                    width2 = GetTabPosition(width, tabIndex);
                                }
                            }
                            // decrease by (DrawUtils.ScreenDpi / 96f) repeats the work of the Word, if the next tab position is a pixel further than the left indent,
                            // then the tab stop occurs in the tab position, otherwise the stop will be in the place of the left indentation
                            if (width < -paragraphFormat.FirstLineIndent && width2 - (DrawUtils.ScreenDpi / 96f) > -paragraphFormat.FirstLineIndent)
                            {
                                width2 = -paragraphFormat.FirstLineIndent;
                            }

                            tabIndex++;
                            line.Words.Add(word);
                            tabRun.Width = width2 - width;
                            width = width2;
                            word = null;
                            break;

                        case SOFT_ENTER://soft enter
                            if (word != null)
                            {
                                if (currentWord.Count > 0)
                                {
                                    Run r = new RunText(this, word, style, currentWord, width, charIndex);
                                    word.Runs.Add(r);
                                    currentWord.Clear();
                                    width += r.Width;
                                    if (width > displayRect.Width)
                                        line = WrapLine(paragraph, line, charIndex, displayRect.Width, ref width, ref word, ref tabIndex);
                                }
                            }
                            else
                            {
                                if (currentWord.Count > 0)
                                {
                                    AddUnknownWord(currentWord, paragraph, style, charIndex, ref line, ref word, ref width, ref tabIndex);
                                }
                            }
                            charIndex = reader.Position;
                            //currentWord.Append(' ')
                            //RunText runText = new RunText(this, word, style, new List<CharWithIndex>(new CharWithIndex[] { reader.Character }), width, charIndex);
                            //runText.Width = 0;
                            //word.Runs.Add(runText);
                            line = new Line(this, paragraph, charIndex);
                            word = null;
                            width = GetStartPosition();
                            currentWord.Clear();
                            paragraph.Lines.Add(line);
                            break;
                        case '\v':
                        case '\n':
                            if (word != null)
                            {
                                if (currentWord.Count > 0)
                                {
                                    Run r = new RunText(this, word, style, currentWord, width, charIndex);
                                    word.Runs.Add(r);
                                    currentWord.Clear();
                                    width += r.Width;
                                    if (width > displayRect.Width)
                                        line = WrapLine(paragraph, line, charIndex, displayRect.Width, ref width, ref word, ref tabIndex);
                                }
                            }
                            else
                            {
                                if (currentWord.Count > 0)
                                {
                                    AddUnknownWord(currentWord, paragraph, style, charIndex, ref line, ref word, ref width, ref tabIndex);
                                }
                            }
                            charIndex = reader.Position;

                            paragraph = new Paragraph(this);
                            paragraphs.Add(paragraph);
                            line = new Line(this, paragraph, charIndex);
                            word = null;
                            width = GetStartPosition(true);
                            paragraph.Lines.Add(line);
                            break;

                        case '\r'://ignore
                            break;

                        default:
                            if (word == null)
                            {
                                word = new Word(this, line, WordType.Normal);
                                line.Words.Add(word);
                            }
                            if (word.Type == WordType.Normal)
                                currentWord.Add(reader.Character);
                            else
                            {
                                if (currentWord.Count > 0)
                                {
                                    Run r = new RunText(this, word, style, currentWord, width, charIndex);
                                    word.Runs.Add(r);
                                    currentWord.Clear();
                                    width += r.Width;
                                    if (width > displayRect.Width)
                                        line = WrapLine(paragraph, line, charIndex, displayRect.Width, ref width, ref word, ref tabIndex);
                                }
                                currentWord.Add(reader.Character);
                                word = new Word(this, line, WordType.Normal);
                                line.Words.Add(word);
                                charIndex = reader.LastPosition;
                            }
                            break;
                    }
                }
                else
                {
                    StyleDescriptor newStyle = new StyleDescriptor(initalStyle);
                    SimpleFastReportHtmlElement element = reader.Element;

                    if (!element.IsSelfClosed)
                    {
                        if (element.isEnd)
                        {
                            int enumIndex = 1;
                            using (Stack<SimpleFastReportHtmlElement>.Enumerator enumerator = elements.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    SimpleFastReportHtmlElement el = enumerator.Current;
                                    if (el.name == element.name)
                                    {
                                        for (int i = 0; i < enumIndex; i++)
                                            elements.Pop();
                                        break;
                                    }
                                    else
                                        enumIndex++;
                                }
                            }
                        }
                        else elements.Push(element);

                        SimpleFastReportHtmlElement[] arr = elements.ToArray();
                        for (int i = arr.Length - 1; i >= 0; i--)
                        {
                            SimpleFastReportHtmlElement el = arr[i];
                            switch (el.name)
                            {
                                case "b":
                                    newStyle.FontStyle |= FontStyle.Bold;
                                    break;

                                case "i":
                                    newStyle.FontStyle |= FontStyle.Italic;
                                    break;

                                case "u":
                                    newStyle.FontStyle |= FontStyle.Underline;
                                    break;

                                case "sub":
                                    newStyle.BaseLine = BaseLine.Subscript;
                                    break;

                                case "sup":
                                    newStyle.BaseLine = BaseLine.Superscript;
                                    break;

                                case "strike":
                                    newStyle.FontStyle |= FontStyle.Strikeout;
                                    break;
                                    //case "font":
                                    //    {
                                    //        string color = null;
                                    //        string face = null;
                                    //        string size = null;
                                    //        if (el.Attributes != null)
                                    //        {
                                    //            el.Attributes.TryGetValue("color", out color);
                                    //            el.Attributes.TryGetValue("face", out face);
                                    //            el.Attributes.TryGetValue("size", out size);
                                    //        }

                                    //        if (color != null)
                                    //        {
                                    //            if (color.StartsWith("\"") && color.EndsWith("\""))
                                    //                color = color.Substring(1, color.Length - 2);
                                    //            if (color.StartsWith("#"))
                                    //            {
                                    //                newStyle.Color = Color.FromArgb((int)(0xFF000000 + uint.Parse(color.Substring(1), System.Globalization.NumberStyles.HexNumber)));
                                    //            }
                                    //            else
                                    //            {
                                    //                newStyle.Color = Color.FromName(color);
                                    //            }
                                    //        }
                                    //        if (face != null)
                                    //            newStyle.Font = face;
                                    //        if (size != null)
                                    //        {
                                    //            try
                                    //            {
                                    //                size = size.Trim(' ');
                                    //                newStyle.Size = (float)Converter.FromString(typeof(float), size) * FFontScale;
                                    //            }
                                    //            catch
                                    //            {
                                    //                newStyle.Size = FSize * FFontScale;
                                    //            }
                                    //        }
                                    //    }
                                    //    break;
                            }
                            CssStyle(newStyle, el.Style);
                        }

                        if (currentWord.Count > 0)
                        {
                            AddUnknownWord(currentWord, paragraph, style, charIndex, ref line, ref word, ref width, ref tabIndex);
                            currentWord.Clear();
                            charIndex = reader.LastPosition;
                        }

                        style = newStyle;
                    }
                    else
                    {
                        switch (element.name)
                        {
                            case "img":
                                if (element.attributes != null && element.attributes.ContainsKey("src"))
                                {
                                    float img_width = -1;
                                    float img_height = -1;
                                    string tStr;

                                    if (element.attributes.TryGetValue("width", out tStr))
                                        try { img_width = Single.Parse(tStr, System.Globalization.CultureInfo.InstalledUICulture); } catch { }
                                    if (element.attributes.TryGetValue("height", out tStr))
                                        try { img_height = Single.Parse(tStr, System.Globalization.CultureInfo.InstalledUICulture); } catch { }

                                    if (currentWord.Count > 0)
                                    {
                                        AddUnknownWord(currentWord, paragraph, style, charIndex, ref line, ref word, ref width, ref tabIndex);
                                        currentWord.Clear();
                                    }
                                    if (word == null || word.Type != WordType.Normal)
                                    {
                                        word = new Word(this, line, WordType.Normal);
                                        line.Words.Add(word);
                                        charIndex = reader.LastPosition;
                                    }

                                    Run r = new RunImage(this, word, element.attributes["src"], style, width, reader.LastPosition, img_width, img_height);
                                    word.Runs.Add(r);
                                    width += r.Width;
                                    if (width > displayRect.Width)
                                        line = WrapLine(paragraph, line, charIndex, displayRect.Width, ref width, ref word, ref tabIndex);
                                }
                                break;
                        }
                    }
                }
            }

            if (currentWord.Count > 0)
            {
                AddUnknownWord(currentWord, paragraph, style, charIndex, ref line, ref word, ref width, ref tabIndex);
            }
        }

        private bool StartsWith(string str1, string str2)
        {
            if (str1.Length < str2.Length) return false;
            switch (str2.Length)
            {
                case 0: return true;
                case 1: return str1[0] == str2[0];
                case 2: return str1[0] == str2[0] && str1[1] == str2[1];
                case 3: return str1[0] == str2[0] && str1[1] == str2[1] && str1[2] == str2[2];
                case 4: return str1[0] == str2[0] && str1[1] == str2[1] && str1[2] == str2[2] && str1[3] == str2[3];
                default: return str1.StartsWith(str2);
            }
        }

        /// <summary>
        /// Check the line, and if last word is able to move next line, move it.
        /// e.g. white space won't move to next line.
        /// If word is not moved return current line.
        /// else return new line
        /// </summary>
        /// <param name="paragraph">the paragraph for lines</param>
        /// <param name="line">the line with extra words</param>
        /// <param name="wordCharIndex">the index of start last word in this line</param>
        /// <param name="availableWidth">width to place words</param>
        /// <param name="newWidth">ref to current line width</param>
        /// <param name="currentWord">ref to current word</param>
        /// <returns></returns>
        private Line WrapLine(Paragraph paragraph, Line line, int wordCharIndex, float availableWidth, ref float newWidth, ref Word currentWord, ref int tabIndex)
        {
            if (line.Words.Count == 0)
            {
                return line;
            }
            if (line.Words.Count == 1 && line.Words[0].Type == WordType.Normal)
            {
                Word word = line.Words[0];
                float width = word.Runs.Count > 0 ? word.Runs[0].Left : 0;
                /* Foreach runs, while run in available space next run
                 * if run begger then space split run and generate new word and run
                 */
                Word newWord = new Word(word.Renderer, line, word.Type);
                line.Words.Clear();
                line.Words.Add(newWord);
                currentWord = newWord;
                foreach (Run run in word.Runs)
                {
                    width += run.Width;
                    if (width <= availableWidth || availableWidth < 0)
                    {
                        newWord.Runs.Add(run);
                        run.Word = newWord;
                    }
                    else
                    {
                        Run secondPart = run;
                        int step = 0;
                        while (secondPart != null)
                        {
                            Run firstPart = secondPart.Split(availableWidth - secondPart.Left, out secondPart);
                            if (firstPart != null)
                            {
                                if (step > 0)
                                    newWord.Runs.Clear();
                                newWord.Runs.Add(firstPart);
                                firstPart.Word = newWord;
                            }
                            else if (newWord.Runs.Count == 0)
                            {
                                newWord.Runs.Add(secondPart);
                                secondPart.Word = newWord;
                                secondPart = null;
                            }
                            if (secondPart != null)
                            {
                                line = new Line(line.Renderer, paragraph, secondPart.CharIndex);
                                paragraph.Lines.Add(line);
                                newWord = new Word(newWord.Renderer, line, newWord.Type);
                                line.Words.Add(newWord);
                                secondPart.Left = GetStartPosition();
                                width = secondPart.Width;
                                currentWord = newWord;
                                if (width < availableWidth)
                                {
                                    secondPart.Word = newWord;
                                    newWord.Runs.Add(secondPart);
                                    secondPart = null;
                                }
                            }
                            step++;
                        }
                    }
                }

                newWidth = width;
                return line;
            }
            else
            if (line.Words[line.Words.Count - 1].Type == WordType.WhiteSpace)
            {
                return line;
            }
            else if (newWidth <= availableWidth)
            {
                return line;
            }
            else
            {
                Word lastWord = line.Words[line.Words.Count - 1];
                line.Words.RemoveAt(line.Words.Count - 1);
                Line result = new Line(this, paragraph, wordCharIndex);
                paragraph.Lines.Add(result);
                newWidth = GetStartPosition();

                if (line.Words.Count > 1 && line.Words[line.Words.Count - 1].Type == WordType.Tab)
                {
                    Word tabWord = line.Words[line.Words.Count - 1];
                    line.Words.RemoveAt(line.Words.Count - 1);
                    float width2 = GetTabPosition(newWidth);
                    if (isDifferentTabPositions)
                    {
                        width2 = GetTabPosition(newWidth, 0);
                    }
                    if (width2 < newWidth) width2 = newWidth;

                    tabIndex = 1;
                    result.Words.Add(tabWord);
                    tabWord.Line = result;
                    tabWord.Runs[0].Left = GetStartPosition();
                    tabWord.Runs[0].Width = width2 - newWidth;
                    newWidth = width2;
                }
                result.Words.Add(lastWord);
                lastWord.Line = result;

                foreach (Run r in lastWord.Runs)
                {
                    r.Left = newWidth;
                    newWidth += r.Width;
                }

                //perhaps need to continue the breakdown
                return WrapLine(paragraph, result, wordCharIndex, availableWidth, ref newWidth, ref currentWord, ref tabIndex);
            }
        }

        /// <summary>
        /// Get start position of line. 
        /// </summary>
        /// <param name="isNewParagraph">
        /// if this parameter is true, the starting position of the line in the new paragraph will be returned
        /// </param>
        /// <returns></returns>
        private float GetStartPosition(bool isNewParagraph = false)
        {
            // if we have a back indent, we take it as zero and shift all the initial positions of the text in the lines by the size of this indent.
            if (isNewParagraph && paragraphFormat.FirstLineIndent > 0)
                return paragraphFormat.FirstLineIndent;

            if (paragraphFormat.FirstLineIndent < 0 && !isNewParagraph)
                return -paragraphFormat.FirstLineIndent;
            return 0;
        }

        #endregion Private Methods

        #region Public Enums

        public enum WordType
        {
            Normal,
            WhiteSpace,
            Tab,
        }

        #endregion Public Enums

        #region Internal Enums

        /// <summary>
        /// Represents character placement.
        /// </summary>
        public enum BaseLine
        {
            Normal,
            Subscript,
            Superscript
        }

        #endregion Internal Enums

        #region Public Structs

        public struct CharWithIndex
        {
            #region Public Fields

            public char Char;
            public int Index;

            #endregion Public Fields

            #region Public Constructors

            public CharWithIndex(char v, int fPosition)
            {
                this.Char = v;
                this.Index = fPosition;
            }

            #endregion Public Constructors
        }

#if READONLY_STRUCTS
        public readonly struct LineFColor
#else
        public struct LineFColor
#endif
        {
            #region Public Fields

            public readonly Color Color;
            public readonly float Left;
            public readonly float Right;
            public readonly float Top;
            public readonly float Width;

            #endregion Public Fields

            #region Public Constructors

            public LineFColor(float left, float top, float right, float width, Color color)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Width = width;
                this.Color = color;
            }

            public LineFColor(float left, float top, float right, float width, byte R, byte G, byte B)
                : this(left, top, right, width, Color.FromArgb(R, G, B))
            {
            }

            public LineFColor(float left, float top, float right, float width, byte R, byte G, byte B, byte A)
                : this(left, top, right, width, Color.FromArgb(A, R, G, B))
            {
            }

            public LineFColor(float left, float top, float right, float width, int R, int G, int B)
              : this(left, top, right, width, Color.FromArgb(R, G, B))
            {
            }

            public LineFColor(float left, float top, float right, float width, int R, int G, int B, int A)
                : this(left, top, right, width, Color.FromArgb(A, R, G, B))
            {
            }

            #endregion Public Constructors
        }

#if READONLY_STRUCTS
        public readonly struct RectangleFColor
#else
        public struct RectangleFColor
#endif
        {
            #region Public Fields

            public readonly Color Color;
            public readonly float Height;
            public readonly float Left;
            public readonly float Top;
            public readonly float Width;

            #endregion Public Fields

            #region Public Constructors

            public RectangleFColor(float left, float top, float width, float height, Color color)
            {
                this.Left = left;
                this.Top = top;
                this.Width = width;
                this.Height = height;
                this.Color = color;
            }

            public RectangleFColor(float left, float top, float width, float height, byte R, byte G, byte B)
                : this(left, top, width, height, Color.FromArgb(R, G, B))
            {
            }

            public RectangleFColor(float left, float top, float width, float height, byte R, byte G, byte B, byte A)
                : this(left, top, width, height, Color.FromArgb(A, R, G, B))
            {
            }

            public RectangleFColor(float left, float top, float width, float height, int R, int G, int B)
              : this(left, top, width, height, Color.FromArgb(R, G, B))
            {
            }

            public RectangleFColor(float left, float top, float width, float height, int R, int G, int B, int A)
                : this(left, top, width, height, Color.FromArgb(A, R, G, B))
            {
            }

            #endregion Public Constructors
        }

        #endregion Public Structs

        #region Public Classes

        public class Line
        {
            #region Private Fields

            private float baseLine;
            private float height;
            private HorzAlign horzAlign;
            private float lineSpacing;
            private int originalCharIndex;
            private Paragraph paragraph;
            private HtmlTextRenderer renderer;
            private float top;
            private float width;
            private List<Word> words;

            #endregion Private Fields

            #region Public Properties

            public float BaseLine
            {
                get { return baseLine; }
                set { baseLine = value; }
            }

            public float Height
            {
                get { return height; }
                set { height = value; }
            }

            public HorzAlign HorzAlign
            {
                get { return horzAlign; }
            }

            public float LineSpacing
            {
                get { return lineSpacing; }
                set { lineSpacing = value; }
            }

            public int OriginalCharIndex
            {
                get { return originalCharIndex; }
                set { originalCharIndex = value; }
            }

            public Paragraph Paragraph
            {
                get { return paragraph; }
                set { paragraph = value; }
            }

            public HtmlTextRenderer Renderer
            {
                get { return renderer; }
            }


            public float Top
            {
                get { return top; }
                set
                {
                    top = value;
                    foreach (Word w in Words)
                    {
                        foreach (Run r in w.Runs)
                        {
                            float shift = 0;
                            if (r.Style.BaseLine == HtmlTextRenderer.BaseLine.Subscript)
                                shift += r.Height * 0.45f;
                            else if (r.Style.BaseLine == HtmlTextRenderer.BaseLine.Superscript)
                                shift -= r.BaseLine - r.Height * 0.15f;
                            r.Top = top + BaseLine - r.BaseLine + shift;
                        }
                    }
                }
            }

            public float Width
            {
                get
                {
                    return width;
                }
            }

            public List<Word> Words
            {
                get { return words; }
            }

            #endregion Public Properties

            #region Public Constructors

            public Line(HtmlTextRenderer renderer, Paragraph paragraph, int charIndex)
            {
                words = new List<Word>();
                this.renderer = renderer;
                this.paragraph = paragraph;
                originalCharIndex = charIndex;
            }

            #endregion Public Constructors

            #region Public Methods

            public override string ToString()
            {
                return String.Format("Words[{0}]", Words.Count);
            }


            #endregion Public Methods

            #region Internal Methods

            internal void AlignWords(HorzAlign align)
            {
                horzAlign = align;
                float width = CalcWidth();
                float left = Words.Count > 0 && Words[0].Runs.Count > 0 ? Words[0].Runs[0].Left : 0;
                width += left;
                this.width = width;
                switch (align)
                {
                    case HorzAlign.Left:
                        break;

                    case HorzAlign.Right:
                        {
                            float delta = Renderer.displayRect.Width - width;
                            foreach (Word w in Words)
                                foreach (Run r in w.Runs)
                                    r.Left += delta;
                        }
                        break;

                    case HorzAlign.Center:
                        {
                            float delta = (Renderer.displayRect.Width - width) / 2f;
                            foreach (Word w in Words)
                                foreach (Run r in w.Runs)
                                    r.Left += delta;
                        }
                        break;

                    case HorzAlign.Justify:
                        {
                            int spaces = 0;
                            int tab_index = -1;
                            bool isWordExistAfterTab = true;
                            for (int i = 0; i < Words.Count - 1; i++)
                            {
                                if (isWordExistAfterTab)
                                {
                                    if (Words[i].Type == WordType.WhiteSpace)
                                        foreach (Run r in Words[i].Runs)
                                            if (r is RunText)
                                                spaces += (r as RunText).Text.Length;
                                }
                                else if (Words[i].Type == WordType.Normal)
                                    isWordExistAfterTab = true;
                                if (Words[i].Type == WordType.Tab)
                                {
                                    spaces = 0;
                                    tab_index = i;
                                    isWordExistAfterTab = false;
                                }
                            }
                            if (spaces > 0)
                            {
                                float space_width = (Renderer.displayRect.Width - width) / spaces;

                                for (int i = 0; i < Words.Count; i++)
                                {
                                    Word w = Words[i];
                                    if (w.Type == WordType.WhiteSpace)
                                        foreach (Run r in w.Runs)
                                        {
                                            if (i > tab_index && r is RunText)
                                                r.Width += space_width * (r as RunText).Text.Length;
                                            r.Left = left;
                                            left += r.Width;
                                        }
                                    else foreach (Run r in w.Runs)
                                        {
                                            r.Left = left;
                                            left += r.Width;
                                        }
                                }
                            }
                        }

                        break;
                }
                if (renderer.RightToLeft)
                {
                    float rectRight = Renderer.displayRect.Right;
                    foreach (Word w in Words)
                        foreach (Run r in w.Runs)
                            r.Left = rectRight - r.Left;
                }
                else
                {
                    float rectLeft = Renderer.displayRect.Left;
                    foreach (Word w in Words)
                        foreach (Run r in w.Runs)
                            r.Left += rectLeft;
                }
            }

            internal void CalcMetrics()
            {
                baseLine = 0;
                foreach (Word word in Words)
                {
                    word.CalcMetrics();
                    baseLine = Math.Max(baseLine, word.BaseLine);
                }
                height = renderer.fontLineHeight;
                float decent = 0;
                foreach (Word word in Words)
                {
                    decent = Math.Max(decent, word.Descent);
                }
                if (baseLine + decent > 0.01)
                    height = baseLine + decent;
                switch (renderer.paragraphFormat.LineSpacingType)
                {
                    case LineSpacingType.AtLeast:
                        if (height < renderer.paragraphFormat.LineSpacing)
                            lineSpacing = renderer.paragraphFormat.LineSpacing - height;
                        break;

                    case LineSpacingType.Single:
                        break;

                    case LineSpacingType.Multiple:
                        lineSpacing = height * (renderer.paragraphFormat.LineSpacingMultiple - 1);
                        break;

                    case LineSpacingType.Exactly:
                        lineSpacing = renderer.paragraphFormat.LineSpacing - height;
                        break;
                }


                if (lineSpacing < 0)
                {
                    // There is a rune in the line with a larger font size than the start font. Line spacing is not needed
                    lineSpacing = 0;
                }
            }

            internal void MakeBackgrounds()
            {
                List<RectangleFColor> list = renderer.backgrounds;
                if (renderer.rightToLeft)
                {
                    foreach (Word word in Words)
                        foreach (Run run in word.Runs)
                            if (run.Style.BackgroundColor.A > 0)
                                list.Add(new RectangleFColor(
                                    run.Left - run.Width, top, run.Width, height, run.Style.BackgroundColor
                                    ));
                }
                else
                {
                    foreach (Word word in Words)
                        foreach (Run run in word.Runs)
                            if (run.Style.BackgroundColor.A > 0)
                                list.Add(new RectangleFColor(
                                    run.Left, top, run.Width, height, run.Style.BackgroundColor
                                    ));
                }
            }

            internal void MakeEverUnderlines()
            {
                OwnHashSet<StyleDescriptor> styles = new OwnHashSet<StyleDescriptor>();
                float size = 0;
                float underline = 0;
                foreach (Word word in Words)
                    foreach (Run run in word.Runs)
                        if (!styles.Contains(run.Style))
                        {
                            styles.Add(run.Style);
                            size += run.Style.Size;
                            underline += run.Descent / 2;
                        }
                if (styles.Count == 0 || BaseLine <= 0.01)
                {
                    using (Font ff = renderer.initalStyle.GetFont())
                    {
                        float lineSpace = ff.FontFamily.GetLineSpacing(renderer.initalStyle.FontStyle);
                        float ascent = ff.FontFamily.GetCellAscent(renderer.initalStyle.FontStyle);
                        baseLine = height * ascent / lineSpace;
                        float FDescent = height - baseLine;
                        underline = FDescent / 2;
                        size = ff.Size;
                    }
                }
                else
                {
                    size /= styles.Count;
                    underline /= styles.Count;
                }

                float fixScale = Renderer.Scale / Renderer.FontScale;

                renderer.underlines.Add(new LineFColor(
                    renderer.displayRect.Left, Top + BaseLine + underline, renderer.displayRect.Right, size * 0.1f * fixScale, renderer.underlineColor
                    ));
            }

            internal void MakeStrikeouts()
            {
                List<LineFColor> lines = renderer.strikeouts;
                float fixScale = Renderer.Scale / Renderer.FontScale;
                if (renderer.rightToLeft)
                {
                    foreach (Word word in Words)
                        foreach (Run r in word.Runs)
                            if ((r.Style.FontStyle & FontStyle.Strikeout) == FontStyle.Strikeout)
                                lines.Add(new LineFColor(
                                r.Left - r.Width, r.Top + r.BaseLine / 3f * 2f, r.Left, r.Style.Size * 0.1f * fixScale,
                                r.Style.Color));
                }
                else
                {
                    foreach (Word word in Words)
                        foreach (Run r in word.Runs)
                            if ((r.Style.FontStyle & FontStyle.Strikeout) == FontStyle.Strikeout)
                                lines.Add(new LineFColor(
                                r.Left, r.Top + r.BaseLine / 3f * 2f, r.Left + r.Width, r.Style.Size * 0.1f * fixScale,
                                r.Style.Color));
                }
            }

            internal void MakeUnderlines()
            {
                if (renderer.everUnderlines)
                {
                    MakeEverUnderlines();
                    return;
                }
                List<List<Run>> runs = new List<List<Run>>();
                List<Run> currentRuns = null;

                foreach (Word word in Words)
                    foreach (Run run in word.Runs)
                    {
                        if ((run.Style.FontStyle & FontStyle.Underline) == FontStyle.Underline)
                        {
                            if (currentRuns == null)
                            {
                                currentRuns = new List<Run>();
                                runs.Add(currentRuns);
                            }
                            currentRuns.Add(run);
                        }
                        else
                        {
                            currentRuns = null;
                        }
                    }
                List<LineFColor> unerlines = renderer.underlines;
                float fixScale = Renderer.Scale / Renderer.FontScale;

                foreach (List<Run> cRuns in runs)
                {
                    OwnHashSet<StyleDescriptor> styles = new OwnHashSet<StyleDescriptor>();
                    float size = 0;
                    float underline = 0;
                    foreach (Run r in cRuns)
                        if (!styles.Contains(r.Style))
                        {
                            styles.Add(r.Style);
                            size += r.Style.Size;
                            underline += r.Descent / 2;
                        }

                    size /= styles.Count;
                    underline /= styles.Count;

                    if (renderer.rightToLeft)
                        foreach (Run r in cRuns)
                            unerlines.Add(new LineFColor(
                                r.Left - r.Width, r.Top + r.BaseLine + underline, r.Left, size * 0.1f * fixScale,
                                r.Style.Color));
                    else
                        foreach (Run r in cRuns)
                            unerlines.Add(new LineFColor(
                                r.Left, r.Top + r.BaseLine + underline, r.Left + r.Width, size * 0.1f * fixScale,
                                r.Style.Color));
                }
            }

            #endregion Internal Methods

            #region Private Methods

            private float CalcWidth()
            {
                float width = 0;
                foreach (Word w in Words)
                    foreach (Run r in w.Runs)
                        width += r.Width;
                Word lastWord = Words.Count > 0 ? Words[Words.Count - 1] : null;
                if (lastWord != null && lastWord.Type == WordType.WhiteSpace)
                {
                    foreach (Run r in lastWord.Runs)
                        width -= r.Width;
                }
                return width;
            }

            #endregion Private Methods
        }

        public class Paragraph
        {
            #region Private Fields

            private List<Line> lines;
            private HtmlTextRenderer renderer;

            #endregion Private Fields

            #region Public Properties

            public List<Line> Lines
            {
                get { return lines; }
            }

            public HtmlTextRenderer Renderer
            {
                get { return renderer; }
            }

            #endregion Public Properties

            #region Public Constructors

            public Paragraph(HtmlTextRenderer renderer)
            {
                lines = new List<Line>();
                this.renderer = renderer;
            }

            #endregion Public Constructors

            #region Public Methods

            public override string ToString()
            {
                if (Lines.Count == 0) return "Lines[0]";
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Lines[{0}]", Lines.Count);
                sb.Append("{");
                foreach (Line line in Lines)
                {
                    sb.Append(line);
                    sb.Append(",");
                }
                sb.Append("}");
                return sb.ToString();
            }

            #endregion Public Methods

            #region Internal Methods

            internal void AlignLines(bool forceJustify)
            {
                for (int i = 0; i < Lines.Count; i++)
                {
                    HorzAlign align = Renderer.horzAlign;
                    if (align == HorzAlign.Justify && i == Lines.Count - 1 && !forceJustify)
                        align = HorzAlign.Left;
                    Lines[i].AlignWords(align);
                }
            }

            #endregion Internal Methods
        }

        public abstract class Run
        {
            #region Protected Fields

            protected float baseLine;
            protected int charIndex;
            protected float descent;
            protected float height;
            protected float left;
            protected HtmlTextRenderer renderer;
            protected StyleDescriptor style;
            protected float top;
            //protected float FUnderline;
            //protected float FUnderlineSize;
            protected float width;
            protected Word word;

            #endregion Protected Fields

            #region Public Properties

            public float BaseLine
            {
                get { return baseLine; }
                set { baseLine = value; }
            }

            public int CharIndex
            {
                get { return charIndex; }
            }

            public float Descent
            {
                get { return descent; }
                set { descent = value; }
            }

            public float Height
            {
                get { return height; }
                set { height = value; }
            }

            public float Left
            {
                get { return left; }
                set { left = value; }
            }

            public HtmlTextRenderer Renderer
            {
                get { return renderer; }
            }

            public StyleDescriptor Style
            {
                get { return style; }
            }

            public float Top
            {
                get { return top; }
                set { top = value; }
            }

            //public float Underline
            //{
            //    get { return FUnderline; }
            //    set { FUnderline = value; }
            //}

            //public float UnderlineSize
            //{
            //    get { return FUnderlineSize; }
            //    set { FUnderlineSize = value; }
            //}

            public float Width
            {
                get { return width; }
                set { width = value; }
            }

            public Word Word
            {
                get { return word; }
                set { word = value; }
            }

            #endregion Public Properties

            #region Public Constructors

            public Run(HtmlTextRenderer renderer, Word word, StyleDescriptor style, float left, int charIndex)
            {
                this.renderer = renderer;
                this.word = word;
                this.style = style;
                this.left = left;
                this.charIndex = charIndex;
            }

            #endregion Public Constructors

            //public virtual void DrawBack(float top, float height)
            //{
            //    if (FStyle.BackgroundColor.A > 0)
            //    {
            //        using (Brush brush = GetBackgroundBrush())
            //            FRenderer.FGraphics.FillRectangle(brush, Left, top, Width, height);
            //    }
            //}

            #region Public Methods

            public abstract void Draw();

            public abstract Run Split(float availableWidth, out Run secondPart);

            #endregion Public Methods

            #region Protected Methods

            protected Brush GetBackgroundBrush()
            {
                return new SolidBrush(style.BackgroundColor);
            }

            #endregion Protected Methods

            //public virtual void Draw(bool drawContents)
            //{
            //    if ((FStyle.FontStyle & FontStyle.Underline) == FontStyle.Underline)
            //    {
            //        if (!FRenderer.FUnderLines)
            //        {
            //            float top = Top + FUnderline;
            //            using (Pen pen = new Pen(FStyle.Color, FUnderlineSize * 0.1f))
            //                if (FRenderer.FRightToLeft)
            //                    FRenderer.FGraphics.DrawLine(pen, Left - Width, top, Left, top);
            //                else
            //                    FRenderer.FGraphics.DrawLine(pen, Left, top, Left + Width, top);
            //        }
            //    }
            //    if ((FStyle.FontStyle & FontStyle.Strikeout) == FontStyle.Strikeout)
            //    {
            //        float top = Top + FBaseLine / 3 * 2;
            //        using (Pen pen = new Pen(FStyle.Color, FStyle.Size * 0.1f))
            //            if (FRenderer.FRightToLeft)
            //                FRenderer.FGraphics.DrawLine(pen, Left - Width, top, Left, top);
            //            else
            //                FRenderer.FGraphics.DrawLine(pen, Left, top, Left + Width, top);
            //    }
            //}
        }

        public class RunImage : Run
        {
            #region Private Fields

            private Image image;
            private string src;

            #endregion Private Fields

            #region Public Properties

            public Image Image { get { return image; } }

            #endregion Public Properties

            #region Public Constructors

            public RunImage(HtmlTextRenderer renderer, Word word, string src, StyleDescriptor style, float left, int charIndex, float img_width, float img_height) : base(renderer, word, style, left, charIndex)
            {
                base.style = new StyleDescriptor(style);
                this.src = src;
                //disable for exports because img tag not support strikeouts and underlines
                base.style.FontStyle &= ~(FontStyle.Strikeout | FontStyle.Underline);
                image = InlineImageCache.Load(Renderer.cache, src);
                Width = image.Width * Renderer.Scale;
                Height = image.Height * Renderer.Scale;
                if (img_height > 0)
                {
                    if (img_width > 0)
                    {
                        Width = img_width * Renderer.Scale;
                        Height = img_height * Renderer.Scale;
                    }
                    else
                    {
                        Width *= img_height / image.Height;
                        Height = img_height * Renderer.Scale;
                    }
                }
                else if (img_width > 0)
                {
                    Width = img_width * Renderer.Scale;
                    Height *= img_width / image.Width;
                }
                baseLine = Height;
                using (Font ff = style.GetFont())
                {
                    float height = ff.GetHeight(Renderer.graphics.Graphics);
                    float lineSpace = ff.FontFamily.GetLineSpacing(style.FontStyle);
                    float descent = ff.FontFamily.GetCellDescent(style.FontStyle);
                    base.descent = height * descent / lineSpace;
                }
            }

            #endregion Public Constructors

            #region Public Methods

            public override void Draw()
            //public override void Draw(bool drawContents)
            {
                //if (drawContents)
                if (image != null)
                {
                    if (renderer.rightToLeft)
                        renderer.graphics.DrawImage(image, new RectangleF(Left - Width, Top, Width, Height));
                    else
                        renderer.graphics.DrawImage(image, new RectangleF(Left, Top, Width, Height));
                }

                //base.Draw(drawContents);
            }

            public override Run Split(float availableWidth, out Run secondPart)
            {
                secondPart = this;
                return null;
            }

            #endregion Public Methods

            #region Internal Methods

            public Bitmap GetBitmap(out float width, out float height)
            {
                width = 1;
                height = 1;
                if (image == null)
                    return new Bitmap(1, 1);

                width = image.Width;
                height = image.Height;
                float x = 0;
                float y = 0;

                float scaleX = width / this.Width;
                float scaleY = height / this.Height;

                if (left < renderer.displayRect.Left)
                {
                    x = -((renderer.displayRect.Left - left) * scaleX);
                    width += x;
                }

                if (top < renderer.displayRect.Top)
                {
                    y = -((renderer.displayRect.Top - top) * scaleY);
                    height += y;
                }

                if (left + base.width > renderer.displayRect.Right)
                {
                    width -= ((left + base.width - renderer.displayRect.Right) * scaleX);
                }

                if (top + base.height > renderer.displayRect.Bottom)
                {
                    height -= ((top + base.height - renderer.displayRect.Bottom) * scaleY);
                }

                if (width < 1) width = 1;
                if (height < 1) height = 1;

                Bitmap bmp = new Bitmap((int)width, (int)height);
                using (Graphics g = Graphics.FromImage(bmp))
                    g.DrawImage(image, new PointF(x, y));
                width /= scaleX;
                height /= scaleY;
                return bmp;
            }

            #endregion Internal Methods

            //public override void ToHtml(FastString sb, bool download)
            //{
            //    if(download)
            //    {
            //        if(FImage!=null)
            //        {
            //            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            //            {
            //                try
            //                {
            //                    using (Bitmap bmp = new Bitmap(FImage.Width, FImage.Height))
            //                    {
            //                        using (Graphics g = Graphics.FromImage(bmp))
            //                        {
            //                            g.DrawImage(FImage, Point.Empty);
            //                        }
            //                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //                    }
            //                    ms.Flush();
            //                    sb.Append("<img src=\"data:image/png;base64,").Append(Convert.ToBase64String(ms.ToArray()))
            //                        .Append("\" width=\"").Append(FWidth.ToString(CultureInfo)).Append("\" height=\"").Append(FHeight.ToString(CultureInfo)).Append("\"/>");
            //                }
            //                catch(Exception e)
            //                {
            //                }
            //            }
            //        }
            //    }else  if(!String.IsNullOrEmpty(FSrc))
            //    {
            //        if (FImage != null)
            //        {
            //            sb.Append("<img src=\"").Append(FSrc).Append("\" width=\"").Append(FWidth.ToString(CultureInfo)).Append("\" height=\"").Append(FHeight.ToString(CultureInfo)).Append("\"/>");
            //        }
            //        else
            //        {
            //            sb.Append("<img src=\"").Append(FSrc).Append("\"/>");
            //        }
            //    }

            //}
        }

        public class RunText : Run
        {
            #region Private Fields

            private List<CharWithIndex> chars;
            private string text;

            #endregion Private Fields

            #region Public Properties

            public string Text { get { return text; } }

            #endregion Public Properties

            #region Public Constructors

            public RunText(HtmlTextRenderer renderer, Word word, StyleDescriptor style, List<CharWithIndex> text, float left, int charIndex) : base(renderer, word, style, left, charIndex)
            {
                using (Font ff = style.GetFont())
                {
                    chars = new List<CharWithIndex>(text);

                    this.text = GetString(text);

                    if (ff.FontFamily.Name == "Wingdings" || ff.FontFamily.Name == "Webdings")
                    {
                        this.text = WingdingsToUnicodeConverter.Convert(this.text);
                    }

                    if (this.text.Length > 0)
                    {
                        base.charIndex = text[0].Index;
                        if (word.Type == WordType.WhiteSpace)
                        {
                            //using (Font f = new Font("Consolas", 10))
                            width = CalcSpaceWidth(this.text, ff);
                        }
                        else
                        {
                            width = Renderer.graphics.MeasureString(this.text, ff, int.MaxValue, base.renderer.format).Width;
                        }
                    }
                    height = ff.GetHeight(Renderer.graphics.Graphics);
                    float lineSpace = ff.FontFamily.GetLineSpacing(style.FontStyle);
                    float ascent = ff.FontFamily.GetCellAscent(style.FontStyle);
                    baseLine = height * ascent / lineSpace;
                    descent = height - baseLine;
                    if (style.BaseLine == HtmlTextRenderer.BaseLine.Subscript)
                        descent += height * 0.45f;
                }
            }

            #endregion Public Constructors

            #region Public Methods

            public float CalcSpaceWidth(string text, Font ff)
            {
                return Renderer.graphics.MeasureString("1" + text + "2", ff, int.MaxValue, renderer.format).Width
                    - Renderer.graphics.MeasureString("12", ff, int.MaxValue, renderer.format).Width;
            }

            public override void Draw()
            //public override void Draw(bool drawContents)
            {
                using (Font font = style.GetFont())
                using (Brush brush = GetBrush())
                {
                    //if (drawContents)
                    //{
                    //#if DEBUG
                    //SizeF size = renderer.graphics.MeasureString(text, font, int.MaxValue, renderer.format);
                    //if (renderer.RightToLeft)
                    //   renderer.graphics.DrawRectangle(Pens.Red, Left - size.Width, Top, size.Width, size.Height);
                    //else
                    //    renderer.graphics.DrawRectangle(Pens.Red, Left, Top, Width, Height);
                    //#endif
                    renderer.graphics.DrawString(text, font, brush, Left, Top, renderer.format);
                }
                //}
                //base.Draw(drawContents);
            }

            public Brush GetBrush()
            {
                return new SolidBrush(Style.Color);
            }

            public override Run Split(float availableWidth, out Run secondPart)
            {
                int size = chars.Count;
                if (size == 0)
                {
                    secondPart = this;
                    return null;
                }

                int from = 0;
                int point = size / 2;
                int to = size;
                Run r = null;
                while (to - from > 1)
                {
                    List<CharWithIndex> list = new List<CharWithIndex>();
                    for (int i = 0; i < point; i++)
                        list.Add(chars[i]);
                    r = new RunText(renderer, word, style, list, left, charIndex);
                    if (r.Width > availableWidth)
                    {
                        if (point == 1 && left == 0)
                        {
                            // Single char width is less than availableWidth
                            // Give up splitting
                            secondPart = null;
                            return this;
                        }
                        to = point;
                        point = (to + from) / 2;
                    }
                    else
                    {
                        from = point;
                        point = (to + from) / 2;
                    }
                }
                if (to < 2)
                {
                    secondPart = this;
                    return null;
                }
                else
                {
                    List<CharWithIndex> list = new List<CharWithIndex>();
                    for (int i = point; i < size; i++)
                        list.Add(chars[i]);
                    secondPart = new RunText(renderer, word, style, list, left + r.Width, charIndex);
                    list.Clear();
                    for (int i = 0; i < point; i++)
                        list.Add(chars[i]);
                    r = new RunText(renderer, word, style, list, left, charIndex);
                    return r;
                }
            }

            #endregion Public Methods

            #region Private Methods

            private string GetString(List<CharWithIndex> str)
            {
                renderer.cacheString.Clear();
                foreach (CharWithIndex ch in str)
                {
                    renderer.cacheString.Append(ch.Char);
                }
                return renderer.cacheString.ToString();
            }

            #endregion Private Methods

            //public override void ToHtml(FastString sb, bool download)
            //{
            //    //if (FWord.Type == WordType.Tab)
            //    //    sb.Append("<span style=\"display:inline-block;min-width:").Append((FWidth * 0.99f).ToString(CultureInfo)).Append("px;\">");
            //    foreach(char ch in Text)
            //    {
            //        switch (ch)
            //        {
            //            case '"':
            //                sb.Append("&quot;");
            //                break;
            //            case '&':
            //                sb.Append("&amp;");
            //                break;
            //            case '<':
            //                sb.Append("&lt;");
            //                break;
            //            case '>':
            //                sb.Append("&gt;");
            //                break;
            //            case '\t':
            //                sb.Append("&Tab;");
            //                break;
            //            default:
            //                sb.Append(ch);
            //                break;
            //        }
            //    }
            //    //if (FWord.Type == WordType.Tab)
            //    //    sb.Append("</span>");
            //}
        }

        public class Word
        {
            #region Private Fields

            private float baseLine;
            private float descent;
            private float height;
            private Line line;
            private HtmlTextRenderer renderer;
            private List<Run> runs;
            private WordType type;

            #endregion Private Fields

            #region Public Properties

            public float BaseLine { get { return baseLine; } }

            public float Descent { get { return descent; } }

            public float Height { get { return height; } }

            public Line Line
            {
                get { return line; }
                set { line = value; }
            }

            public HtmlTextRenderer Renderer
            {
                get { return renderer; }
            }

            public List<Run> Runs
            {
                get { return runs; }
            }

            public WordType Type
            {
                get { return type; }
                set { type = value; }
            }

            #endregion Public Properties

            #region Public Constructors

            public Word(HtmlTextRenderer renderer, Line line)
            {
                this.renderer = renderer;
                runs = new List<Run>();
                this.line = line;
            }

            public Word(HtmlTextRenderer renderer, Line line, WordType type)
            {
                this.renderer = renderer;
                runs = new List<Run>();
                this.line = line;
                this.type = type;
            }

            #endregion Public Constructors

            #region Internal Methods

            internal void CalcMetrics()
            {
                baseLine = 0;
                descent = 0;
                foreach (Run run in Runs)
                {
                    baseLine = Math.Max(baseLine, run.BaseLine);
                    descent = Math.Max(descent, run.Descent);
                }
                height = baseLine + descent;
            }

            #endregion Internal Methods
        }

        #endregion Public Classes

        #region Internal Classes

        internal class SimpleFastReportHtmlElement
        {
            #region Public Fields

            public Dictionary<string, string> attributes;
            public bool isSelfClosed;
            public bool isEnd;
            public string name;

            #endregion Public Fields

            #region Private Fields

            private Dictionary<string, string> style;

            #endregion Private Fields

            #region Public Properties

            public bool IsSelfClosed
            {
                get
                {
                    switch (name)
                    {
                        case "img":
                        case "br":
                            return true;

                        default:
                            return isSelfClosed;
                    }
                }
                set { isSelfClosed = value; }
            }

            /// <summary>
            /// Be care generates dictionary only one time
            /// </summary>
            public Dictionary<string, string> Style
            {
                get
                {
                    if (style == null && attributes != null && attributes.ContainsKey("style"))
                    {
                        string styleString = attributes["style"];
                        style = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (string kv in styleString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] strs = kv.Split(':');
                            if (strs.Length == 2)
                            {
                                style[strs[0]] = strs[1];
                            }
                        }
                    }
                    return style;
                }
            }

            #endregion Public Properties

            #region Public Constructors

            public SimpleFastReportHtmlElement(string name)
            {
                this.name = name;
            }

            public SimpleFastReportHtmlElement(string name, Dictionary<string, string> attributes)
            {
                this.name = name;
                this.attributes = attributes;
            }

            public SimpleFastReportHtmlElement(string name, bool isEnd)
            {
                this.name = name;
                this.isEnd = isEnd;
            }

            public SimpleFastReportHtmlElement(string name, bool isBegin, Dictionary<string, string> attributes)
            {
                this.name = name;
                this.isEnd = isBegin;
                this.attributes = attributes;
            }

            public SimpleFastReportHtmlElement(string name, bool isBegin, bool isSelfClosed)
            {
                this.name = name;
                this.isEnd = isBegin;
                this.IsSelfClosed = isSelfClosed;
            }

            public SimpleFastReportHtmlElement(string name, bool isBegin, bool isSelfClosed, Dictionary<string, string> attributes)
            {
                this.name = name;
                this.isEnd = isBegin;
                this.IsSelfClosed = isSelfClosed;
                this.attributes = attributes;
            }

            #endregion Public Constructors

            #region Public Methods

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<");
                if (isEnd)
                    sb.Append("/");
                sb.Append(name);
                if (attributes != null)
                {
                    foreach (KeyValuePair<string, string> kv in attributes)
                    {
                        sb.Append(" ");
                        sb.Append(kv.Key);
                        sb.Append("=\"");
                        sb.Append(kv.Value);
                        sb.Append("\"");
                    }
                }
                if (IsSelfClosed)
                    sb.Append('/');
                sb.Append(">");
                return sb.ToString();
            }

            #endregion Public Methods
        }

        internal class SimpleFastReportHtmlReader
        {
            #region Private Fields

            private CharWithIndex @char;
            private SimpleFastReportHtmlElement element;
            private int lastPosition;
            private int position;
            private string substring;
            private string text;

            #endregion Private Fields

            #region Public Properties

            public CharWithIndex Character
            {
                get
                {
                    return @char;
                }
            }

            public SimpleFastReportHtmlElement Element
            {
                get
                {
                    return element;
                }
            }

            public bool IsEOF
            {
                get
                {
                    return position >= text.Length;
                }
            }

            public bool IsNotEOF
            {
                get
                {
                    return position < text.Length;
                }
            }

            public int LastPosition
            {
                get { return lastPosition; }
            }

            public int Position
            {
                get
                {
                    return position;
                }
                set
                {
                    position = value;
                }
            }

            #endregion Public Properties

            #region Public Constructors

            public SimpleFastReportHtmlReader(string text)
            {
                this.text = text;
            }

            #endregion Public Constructors

            #region Public Methods

            public static bool IsCanBeCharacterInTagName(char c)
            {
                if (c == ':') return true;
                if ('A' <= c && c <= 'Z') return true;
                if (c == '_') return true;
                if ('a' <= c && c <= 'z') return true;
                if (c == '-') return true;//
                if (c == '.') return true;//
                if ('0' <= c && c <= '9') return true;//
                if (c == '\u00B7') return true;//
                if ('\u00C0' <= c && c <= '\u00D6') return true;
                if ('\u00D8' <= c && c <= '\u00F6') return true;
                if ('\u00F8' <= c && c <= '\u02FF') return true;
                if ('\u0300' <= c && c <= '\u036F') return true;//
                if ('\u0370' <= c && c <= '\u037D') return true;
                if ('\u037F' <= c && c <= '\u1FFF') return true;
                if ('\u200C' <= c && c <= '\u200D') return true;
                if ('\u203F' <= c && c <= '\u2040') return true;//
                if ('\u2070' <= c && c <= '\u218F') return true;
                if ('\u2C00' <= c && c <= '\u2FEF') return true;
                if ('\u3001' <= c && c <= '\uD7FF') return true;
                if ('\uF900' <= c && c <= '\uFDCF') return true;
                if ('\uFDF0' <= c && c <= '\uFFFD') return true;
                return false;
            }

            public static bool IsCanBeFirstCharacterInTagName(char c)
            {
                if (c == ':') return true;
                if ('A' <= c && c <= 'Z') return true;
                if (c == '_') return true;
                if ('a' <= c && c <= 'z') return true;
                if ('\u00C0' <= c && c <= '\u00D6') return true;
                if ('\u00D8' <= c && c <= '\u00F6') return true;
                if ('\u00F8' <= c && c <= '\u02FF') return true;
                if ('\u0370' <= c && c <= '\u037D') return true;
                if ('\u037F' <= c && c <= '\u1FFF') return true;
                if ('\u200C' <= c && c <= '\u200D') return true;
                if ('\u2070' <= c && c <= '\u218F') return true;
                if ('\u2C00' <= c && c <= '\u2FEF') return true;
                if ('\u3001' <= c && c <= '\uD7FF') return true;
                if ('\uF900' <= c && c <= '\uFDCF') return true;
                if ('\uFDF0' <= c && c <= '\uFFFD') return true;
                return false;
            }

            /// <summary>
            /// Return true if read char
            /// </summary>
            /// <returns></returns>
            public bool Read()
            {
                lastPosition = position;
                switch ((@char = new CharWithIndex(text[position], position)).Char)
                {
                    case '&':
                        if (Converter.FromHtmlEntities(text, ref position, out substring))
                            @char.Char = substring[0];
                        position++;
                        return true;

                    case '<':
                        element = GetElement(text, ref position);
                        position++;
                        if (element != null)
                            switch (element.name)
                            {
                                case "br":
                                    @char = new CharWithIndex('\n', lastPosition);
                                    return true;

                                default:
                                    return false;
                            }
                        return true;
                }
                position++;
                return true;
            }

            #endregion Public Methods

            #region Private Methods

            private SimpleFastReportHtmlElement GetElement(string line, ref int index)
            {
                int to = line.Length - 1;
                int i = index + 1;
                bool closed = false;
                if (i <= to)
                    if (closed = line[i] == '/')
                        i++;
                if (i <= to)
                    if (!IsCanBeFirstCharacterInTagName(line[i]))
                        return null;
                for (i++; i <= to && line[i] != ' ' && line[i] != '>' && line[i] != '/'; i++)
                {
                    if (!IsCanBeCharacterInTagName(line[i]))
                        return null;
                }
                if (i <= to)
                {
                    string tagName = line.Substring(index + (closed ? 2 : 1), i - index - (closed ? 2 : 1));
                    Dictionary<string, string> attrs = null;
                    if (!IsAvailableTagName(tagName))
                        return null;
                    if (line[i] == ' ')
                    {
                        //read attributes
                        for (; i <= to && line[i] != '>' && line[i] != '/'; i++)
                        {
                            for (; i <= to && line[i] == ' '; i++) ;
                            if (line[i] == '>' || line[i] == '/') i--;
                            else
                            {
                                if (!IsCanBeFirstCharacterInTagName(line[i]))
                                    return null;
                                int attrNameStartIndex = i;
                                for (i++; i <= to && line[i] != '='; i++)
                                    if (!IsCanBeFirstCharacterInTagName(line[i]))
                                        return null;
                                int attrNameEndIndex = i; //index of =
                                i++;
                                if (i <= to && line[i] == '"')
                                {//begin attr
                                    int attrValueStartIndex = i + 1;
                                    for (i++; i <= to && line[i] != '"'; i++)
                                    {
                                        switch (line[i])
                                        {
                                            case '<': return null;
                                            case '>': return null;
                                        }
                                    }
                                    if (i <= to)
                                    {
                                        string attrName = line.Substring(attrNameStartIndex, attrNameEndIndex - attrNameStartIndex);
                                        string attrValue = line.Substring(attrValueStartIndex, i - attrValueStartIndex);
                                        if (attrs == null) attrs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                                        attrs[attrName] = attrValue;
                                    }
                                }
                            }
                        }
                    }
                    if (i <= to)
                    {
                        if (line[i] == '>')
                        {
                            index = i;
                            return new SimpleFastReportHtmlElement(tagName, closed, false, attrs);
                        }
                        if (line[i] == '/' && i < to && line[i + 1] == '>')
                        {
                            index = i + 1;
                            return new SimpleFastReportHtmlElement(tagName, closed, true, attrs);
                        }
                    }
                }
                return null;
            }

            private bool IsAvailableTagName(string tagName)
            {
                switch (tagName)
                {
                    case "b":
                    case "br":
                    case "i":
                    case "u":
                    case "sub":
                    case "sup":
                    case "img":
                    //case "font":
                    case "strike":
                    case "span":
                        return true;
                }
                return false;
            }

            #endregion Private Methods
        }

        /// <summary>
        /// Represents a style used in HtmlTags mode. Color does not affect the equals function.
        /// </summary>
        public class StyleDescriptor
        {
            #region Private Fields

            private static readonly Color DefaultColor = Color.Transparent;
            private Color backgroundColor;
            private BaseLine baseLine;
            private Color color;
            private FontFamily font;
            private FontStyle fontStyle;
            private float size;

            #endregion Private Fields

            #region Public Properties

            public Color BackgroundColor
            {
                get { return backgroundColor; }
                set { backgroundColor = value; }
            }

            public BaseLine BaseLine
            {
                get { return baseLine; }
                set { baseLine = value; }
            }

            public Color Color
            {
                get { return color; }
                set { color = value; }
            }

            public FontFamily Font
            {
                get { return font; }
                set { font = value; }
            }

            public FontStyle FontStyle
            {
                get { return fontStyle; }
                set { fontStyle = value; }
            }

            public float Size
            {
                get { return size; }
                set { size = value; }
            }

            #endregion Public Properties

            #region Public Constructors

            public StyleDescriptor(FontStyle fontStyle, Color color, BaseLine baseLine, FontFamily font, float size)
            {
                this.fontStyle = fontStyle;
                this.color = color;
                this.baseLine = baseLine;
                this.font = font;
                this.size = size;
                backgroundColor = DefaultColor;
            }

            public StyleDescriptor(StyleDescriptor styleDescriptor)
            {
                fontStyle = styleDescriptor.fontStyle;
                color = styleDescriptor.color;
                baseLine = styleDescriptor.baseLine;
                font = styleDescriptor.font;
                size = styleDescriptor.size;
                backgroundColor = styleDescriptor.backgroundColor;
            }

            #endregion Public Constructors

            #region Public Methods

            public override bool Equals(object obj)
            {
                StyleDescriptor descriptor = obj as StyleDescriptor;
                return descriptor != null &&
                       baseLine == descriptor.baseLine &&
                       font == descriptor.font &&
                       fontStyle == descriptor.fontStyle &&
                       size == descriptor.size;
            }

            /// <summary>
            /// returns true if objects realy equals
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool FullEquals(StyleDescriptor obj)
            {
                return obj != null && GetHashCode() == obj.GetHashCode() &&
                    this.Equals(obj) &&
                    color.Equals(obj.color) &&
                    backgroundColor.Equals(obj.backgroundColor);
            }

            public Font GetFont()
            {
                float fontSize = size;
                if (baseLine != BaseLine.Normal)
                    fontSize *= 0.6f;

                FontStyle fontStyle = FontStyle;

                fontStyle = fontStyle & ~FontStyle.Underline & ~FontStyle.Strikeout;
                return new Font(font, fontSize, fontStyle);
            }

            public override int GetHashCode()
            {
                int hashCode = -1631016721;
                unchecked
                {
                    hashCode = hashCode * -1521134295 + baseLine.GetHashCode();
                    hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(font.Name);
                    hashCode = hashCode * -1521134295 + fontStyle.GetHashCode();
                    hashCode = hashCode * -1521134295 + size.GetHashCode();
                }
                return hashCode;
            }

            public void ToHtml(FastString sb, bool close)
            {
                float fontsize = size / DrawUtils.ScreenDpiFX;
                if (close)
                {
                    switch (baseLine)
                    {
                        case BaseLine.Subscript: sb.Append("</sub>"); break;
                        case BaseLine.Superscript: sb.Append("</sup>"); break;
                    }
                    sb.Append("</span>");

                    if ((fontStyle & FontStyle.Strikeout) == FontStyle.Strikeout) sb.Append("</strike>");
                    if ((fontStyle & FontStyle.Underline) == FontStyle.Underline) sb.Append("</u>");
                    if ((fontStyle & FontStyle.Italic) == FontStyle.Italic) sb.Append("</i>");
                    if ((fontStyle & FontStyle.Bold) == FontStyle.Bold) sb.Append("</b>");

                }
                else
                {
                    if ((fontStyle & FontStyle.Bold) == FontStyle.Bold) sb.Append("<b>");
                    if ((fontStyle & FontStyle.Italic) == FontStyle.Italic) sb.Append("<i>");
                    if ((fontStyle & FontStyle.Underline) == FontStyle.Underline) sb.Append("<u>");
                    if ((fontStyle & FontStyle.Strikeout) == FontStyle.Strikeout) sb.Append("<strike>");

                    sb.Append("<span style=\"");
                    if (backgroundColor.A > 0) sb.Append(String.Format(CultureInfo, "background-color:rgba({0},{1},{2},{3});", backgroundColor.R, backgroundColor.G, backgroundColor.B, ((float)backgroundColor.A) / 255f));
                    if (color.A > 0) sb.Append(String.Format(CultureInfo, "color:rgba({0},{1},{2},{3});", color.R, color.G, color.B, ((float)color.A) / 255f));
                    if (font != null) { sb.Append("font-family:"); sb.Append("\'"+font.Name+"\'"); sb.Append(";"); }
                    if (fontsize > 0) { sb.Append("font-size:"); sb.Append(fontsize.ToString(CultureInfo)); sb.Append("pt;"); }

                    //if ((fontStyle & FontStyle.Italic) == FontStyle.Italic) { sb.Append("font-style:italic;"); }
                    //if ((fontStyle & FontStyle.Bold) == FontStyle.Bold) { sb.Append("font-weight:bold;"); }

                    //bool underline = (fontStyle & FontStyle.Underline) == FontStyle.Underline;
                    //bool strikeout = (fontStyle & FontStyle.Strikeout) == FontStyle.Strikeout;

                    //if (underline && strikeout)
                    //{
                    //    sb.Append("text-decoration:underline line-through;");
                    //}
                    //else if (underline)
                    //{
                    //    sb.Append("text-decoration: underline;");
                    //}
                    //else if (strikeout)
                    //{
                    //    sb.Append("text-decoration:line-through;");
                    //}

                    sb.Append("\">");
                    switch (baseLine)
                    {
                        case BaseLine.Subscript: sb.Append("<sub>"); break;
                        case BaseLine.Superscript: sb.Append("<sup>"); break;
                    }
                }
            }

            #endregion Public Methods
        }

        private class OwnHashSet<T>
        {
#if DOTNET_4
            private HashSet<T> internalHashSet;
            public int Count { get { return internalHashSet.Count; } }
#else
            private Dictionary<T, object> internalDictionary;
            private object FHashSetObject;
            public int Count { get { return internalDictionary.Count; } }
#endif

            public OwnHashSet()
            {
#if DOTNET_4
                internalHashSet = new HashSet<T>();
#else
                internalDictionary = new Dictionary<T, object>();
                FHashSetObject = new object();
#endif
            }

            public void Clear()
            {
#if DOTNET_4
                internalHashSet.Clear();
#else
                internalDictionary.Clear();
#endif
            }

            public bool Contains(T value)
            {
#if DOTNET_4
                return internalHashSet.Contains(value);
#else
                return internalDictionary.ContainsKey(value);
#endif
            }

            public void Add(T value)
            {
#if DOTNET_4
                internalHashSet.Add(value);
#else
                internalDictionary.Add(value, FHashSetObject);
#endif
            }
        }
        #endregion Internal Classes

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    format.Dispose();
                    format = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }

    /// <summary>
    /// Class that converts strings with Wingdings characters to Unicode strings.
    /// </summary>
    public static class WingdingsToUnicodeConverter
    {
        /// <summary>
        /// Converts string with Wingdings characters to its Unicode analog.
        /// </summary>
        /// <param name="str">The string that should be converted.</param>
        /// <returns></returns>
        public static string Convert(string str)
        {
            char[] chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] >= 0x20 && chars[i] <= 0xFF)
                {
                    chars[i] = (char)(0xF000 + chars[i]);
                }
            }
            return new string(chars);
        }
    }
}
