using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Net;
using System.IO;

namespace FastReport.Utils
{
    /// <summary>
    /// Advanced text renderer is used to perform the following tasks:
    /// - draw justified text, text with custom line height, text containing html tags;
    /// - calculate text height, get part of text that does not fit in the display rectangle;
    /// - get paragraphs, lines, words and char sequence to perform accurate export to such
    /// formats as PDF, TXT, RTF
    /// </summary>
    /// <example>Here is how one may operate the renderer items:
    /// <code>
    /// foreach (AdvancedTextRenderer.Paragraph paragraph in renderer.Paragraphs)
    /// {
    ///   foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
    ///   {
    ///     foreach (AdvancedTextRenderer.Word word in line.Words)
    ///     {
    ///       if (renderer.HtmlTags)
    ///       {
    ///         foreach (AdvancedTextRenderer.Run run in word.Runs)
    ///         {
    ///           using (Font f = run.GetFont())
    ///           using (Brush b = run.GetBrush())
    ///           {
    ///             g.DrawString(run.Text, f, b, run.Left, run.Top, renderer.Format);
    ///           }  
    ///         }
    ///       }
    ///       else
    ///       {
    ///         g.DrawString(word.Text, renderer.Font, renderer.Brush, word.Left, word.Top, renderer.Format);
    ///       }
    ///     }
    ///   }
    /// }
    /// </code>
    /// </example>
    public class AdvancedTextRenderer
    {
        #region Fields
        private List<Paragraph> paragraphs;
        private string text;
        private Graphics graphics;
        private Font font;
        private Brush brush;
        private Pen outlinePen;
        private RectangleF displayRect;
        private StringFormat format;
        private HorzAlign horzAlign;
        private VertAlign vertAlign;
        private float lineHeight;
        private float fontLineHeight;
        private int angle;
        private float widthRatio;
        private bool forceJustify;
        private bool wysiwyg;
        private bool htmlTags;
        private bool pDFMode;
        private float spaceWidth;
        private float scale;
        private InlineImageCache cache;
        private float fontScale;
        #endregion

        #region Properties
        public List<Paragraph> Paragraphs
        {
            get { return paragraphs; }
        }

        public Graphics Graphics
        {
            get { return graphics; }
        }

        public Font Font
        {
            get { return font; }
        }

        public Brush Brush
        {
            get { return brush; }
        }

        public Pen OutlinePen
        {
            get { return outlinePen; }
        }

        public Color BrushColor
        {
            get { return brush is SolidBrush ? (brush as SolidBrush).Color : Color.Black; }
        }

        public RectangleF DisplayRect
        {
            get { return displayRect; }
        }

        public StringFormat Format
        {
            get { return format; }
        }

        public HorzAlign HorzAlign
        {
            get { return horzAlign; }
        }

        public VertAlign VertAlign
        {
            get { return vertAlign; }
        }

        public float LineHeight
        {
            get { return lineHeight; }
        }

        public float FontLineHeight
        {
            get { return fontLineHeight; }
        }

        public int Angle
        {
            get { return angle; }
        }

        public float WidthRatio
        {
            get { return widthRatio; }
        }

        public bool ForceJustify
        {
            get { return forceJustify; }
        }

        public bool Wysiwyg
        {
            get { return wysiwyg; }
        }

        public bool HtmlTags
        {
            get { return htmlTags; }
        }

        public float TabSize
        {
            get
            {
                // re fix tab offset #2823 sorry linux users, on linux firstTab is firstTab not tabSizes[0]
                float firstTab = 0;
                float[] tabSizes = Format.GetTabStops(out firstTab);
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
                float firstTab = 0;
                float[] tabSizes = Format.GetTabStops(out firstTab);
                if (tabSizes.Length > 0)
                    return tabSizes[0];
                return 0;
            }
        }

        public bool WordWrap
        {
            get { return (Format.FormatFlags & StringFormatFlags.NoWrap) == 0; }
        }

        public bool RightToLeft
        {
            get { return (Format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0; }
        }

        public bool PDFMode
        {
            get { return pDFMode; }
        }

        internal float SpaceWidth
        {
            get { return spaceWidth; }
        }

        /// <summary>
        /// The scale for font tag
        /// </summary>
        public float FontScale { get { return fontScale; } set { fontScale = value; } }
        public float Scale { get { return scale; } set { scale = value; } }

        public InlineImageCache Cache
        {
            get
            {
                if (cache == null)
                    cache = new InlineImageCache();
                return cache;
            }
        }
        #endregion

        #region Private Methods

        const string ab = "abcdefabcdef";
        const string a40b = "abcdef                                        abcdef";

        internal static float CalculateSpaceSize(Graphics g, Font f)
        {
            float w_ab = g.MeasureString(ab, f).Width;
            float w_a40b = g.MeasureString(a40b, f).Width;
            return (w_a40b - w_ab) / 40;
        }

        private void SplitToParagraphs(string text)
        {
            StyleDescriptor style = new StyleDescriptor(Font.Style, BrushColor, BaseLine.Normal);
            if (HtmlTags)
                text = text.Replace("<br>", "\r\n").Replace("<br/>", "\r\n").Replace("<br />", "\r\n");
            string[] lines = text.Split(new char[] { '\n' });
            int originalCharIndex = 0;

            foreach (string line in lines)
            {
                string s = line;
                if (s.Length > 0 && s[s.Length - 1] == '\r')
                    s = s.Remove(s.Length - 1);

                Paragraph paragraph = new Paragraph(s, this, originalCharIndex);
                paragraphs.Add(paragraph);
                if (HtmlTags)
                    style = paragraph.WrapHtmlLines(style);
                else
                    paragraph.WrapLines();

                originalCharIndex += line.Length + 1;
            }

            // skip empty paragraphs at the end
            for (int i = paragraphs.Count - 1; i >= 0; i--)
            {
                if (paragraphs[i].IsEmpty && paragraphs.Count != 1)
                    paragraphs.RemoveAt(i);
                else
                    break;
            }
        }

        private void AdjustParagraphLines()
        {
            // calculate text height
            float height = 0;
            height = CalcHeight();
            /*foreach (Paragraph paragraph in Paragraphs)
            {
              height += paragraph.Lines.Count * LineHeight;
            }*/

            // calculate Y offset
            float offsetY = DisplayRect.Top;
            if (VertAlign == VertAlign.Center)
                offsetY += (DisplayRect.Height - height) / 2;
            else if (VertAlign == VertAlign.Bottom)
                offsetY += (DisplayRect.Height - height) - 1;

            for (int i = 0; i < Paragraphs.Count; i++)
            {
                Paragraph paragraph = Paragraphs[i];
                paragraph.AlignLines(i == Paragraphs.Count - 1 && ForceJustify);

                // adjust line tops
                foreach (Line line in paragraph.Lines)
                {
                    line.Top = offsetY;
                    line.MakeUnderlines();
                    offsetY += line.CalcHeight();
                }
            }
        }
        #endregion

        #region Public Methods
        public void Draw()
        {
            // set clipping
            GraphicsState state = Graphics.Save();
            Graphics.SetClip(DisplayRect, CombineMode.Intersect);

            // reset alignment
            StringAlignment saveAlign = Format.Alignment;
            StringAlignment saveLineAlign = Format.LineAlignment;
            Format.Alignment = StringAlignment.Near;
            Format.LineAlignment = StringAlignment.Near;

            if (Angle != 0)
            {
                Graphics.TranslateTransform(DisplayRect.Left + DisplayRect.Width / 2,
                  DisplayRect.Top + DisplayRect.Height / 2);
                Graphics.RotateTransform(Angle);
            }

            Graphics.ScaleTransform(WidthRatio, 1);

            foreach (Paragraph paragraph in Paragraphs)
            {
                paragraph.Draw();
            }

            // restore alignment and clipping
            Format.Alignment = saveAlign;
            Format.LineAlignment = saveLineAlign;
            Graphics.Restore(state);
        }

        public float CalcHeight()
        {
            int charsFit = 0;
            StyleDescriptor style = null;
            return CalcHeight(out charsFit, out style);
        }

        public float CalcHeight(out int charsFit, out StyleDescriptor style)
        {
            charsFit = 0;
            style = null;
            float height = 0;
            float displayHeight = DisplayRect.Height;
            if (LineHeight > displayHeight)
                return 0;

            foreach (Paragraph paragraph in Paragraphs)
            {
                foreach (Line line in paragraph.Lines)
                {
                    height += line.CalcHeight();
                    if (charsFit == 0 && height > displayHeight)
                    {
                        charsFit = line.OriginalCharIndex;
                        if (HtmlTags)
                            style = line.Style;
                    }
                }
            }

            if (charsFit == 0)
                charsFit = text.Length;
            return height;
        }

        public float CalcWidth()
        {
            float width = 0;

            foreach (Paragraph paragraph in Paragraphs)
            {
                foreach (Line line in paragraph.Lines)
                {
                    if (width < line.Width)
                        width = line.Width;
                }
            }
            return width + spaceWidth;
        }

        internal float GetTabPosition(float pos)
        {
            float tabOffset = TabOffset;
            float tabSize = TabSize;
            int tabPosition = (int)((pos - tabOffset) / tabSize);
            if (pos < tabOffset)
                return tabOffset;
            return (tabPosition + 1) * tabSize + tabOffset;
        }
        #endregion

        public AdvancedTextRenderer(string text, Graphics g, Font font, Brush brush, Pen outlinePen,
          RectangleF rect, StringFormat format, HorzAlign horzAlign, VertAlign vertAlign,
          float lineHeight, int angle, float widthRatio,
          bool forceJustify, bool wysiwyg, bool htmlTags, bool pdfMode,
          float scale, float fontScale, InlineImageCache cache, bool isPrinting = false)
        {
            this.cache = cache;
            this.scale = scale;
            this.fontScale = fontScale;
            paragraphs = new List<Paragraph>();
            this.text = text;
            graphics = g;
            this.font = font;
            this.brush = brush;
            this.outlinePen = outlinePen;
            displayRect = rect;
            this.format = format;
            this.horzAlign = horzAlign;
            this.vertAlign = vertAlign;
            this.lineHeight = lineHeight;
            fontLineHeight = font.GetHeight(g);
            if (this.lineHeight == 0)
            {
                this.lineHeight = fontLineHeight;
                if (isPrinting && Config.IsRunningOnMono && DrawUtils.GetMonoRendering(g) == MonoRendering.Pango)
                {
                    // we need this in order to fix inconsistent line spacing when print using Pango rendering
                    this.lineHeight = fontLineHeight * 1.33f;
                }
            }
            this.angle = angle % 360;
            this.widthRatio = widthRatio;
            this.forceJustify = forceJustify;
            this.wysiwyg = wysiwyg;
            this.htmlTags = htmlTags;
            pDFMode = pdfMode;
            spaceWidth = CalculateSpaceSize(g, font);// g.MeasureString(" ", font).Width;

            StringFormatFlags saveFlags = Format.FormatFlags;
            StringTrimming saveTrimming = Format.Trimming;

            // match DrawString behavior: 
            // if height is less than 1.25 of font height, turn off word wrap
            // commented out due to bug with band.break
            //if (rect.Height < FFontLineHeight * 1.25f)
            //FFormat.FormatFlags |= StringFormatFlags.NoWrap;

            // if word wrap is set, ignore trimming
            if (WordWrap)
                Format.Trimming = StringTrimming.Word;

            Format.FormatFlags = Format.FormatFlags | StringFormatFlags.MeasureTrailingSpaces;

            if (Angle != 0)
            {
                // shift displayrect 
                displayRect.X = -DisplayRect.Width / 2;
                displayRect.Y = -DisplayRect.Height / 2;

                // rotate displayrect if angle is 90 or 270
                if ((Angle >= 90 && Angle < 180) || (Angle >= 270 && Angle < 360))
                    displayRect = new RectangleF(DisplayRect.Y, DisplayRect.X, DisplayRect.Height, DisplayRect.Width);
            }

            displayRect.X /= WidthRatio;
            displayRect.Width /= WidthRatio;

            SplitToParagraphs(text);
            AdjustParagraphLines();

            // restore original values
            displayRect = rect;
            Format.FormatFlags = saveFlags;
            Format.Trimming = saveTrimming;
        }


        /// <summary>
        /// Paragraph represents single paragraph. It consists of one or several <see cref="Lines"/>.
        /// </summary>
        public class Paragraph
        {
            #region Fields
            private List<Line> lines;
            private AdvancedTextRenderer renderer;
            private string text;
            private int originalCharIndex;
            #endregion

            #region Properties
            public List<Line> Lines
            {
                get { return lines; }
            }

            public AdvancedTextRenderer Renderer
            {
                get { return renderer; }
            }

            public bool Last
            {
                get { return renderer.Paragraphs[renderer.Paragraphs.Count - 1] == this; }
            }

            public bool IsEmpty
            {
                get { return String.IsNullOrEmpty(text); }
            }

            public string Text
            {
                get { return text; }
            }
            #endregion

            #region Private Methods
            private int MeasureString(string text)
            {
                if (text.Length > 0)
                {
                    // BEGIN: The fix for linux and core app a264aae5-193b-4e5c-955c-0818de3ca01b
                    float left = 0;
                    int tabFit = 0;
                    while (text.Length > 0 && text[0] == '\t')
                    {
                        left = Renderer.GetTabPosition(left);
                        text = text.Substring(1);
                        if (Renderer.DisplayRect.Width < left)
                            return tabFit;
                        tabFit++;
                    }
                    if (tabFit > 0 && Renderer.DisplayRect.Width < left)
                        return tabFit;
                    int charsFit = 0;
                    int linesFit = 0;
                    // END: The fix for linux and core app a264aae5-193b-4e5c-955c-0818de3ca01b
                    Renderer.Graphics.MeasureString(text, Renderer.Font,
                      new SizeF(Renderer.DisplayRect.Width - left, Renderer.FontLineHeight + 1),
                      Renderer.Format, out charsFit, out linesFit);
                    return charsFit + tabFit;
                }
                return 0;
            }
            #endregion

            #region Public Methods
            public void WrapLines()
            {
                string text = this.text;
                int charsFit = 0;

                if (String.IsNullOrEmpty(text))
                {
                    lines.Add(new Line("", this, originalCharIndex));
                    return;
                }

                if (Renderer.WordWrap)
                {
                    int originalCharIndex = this.originalCharIndex;
                    while (text.Length > 0)
                    {
                        charsFit = MeasureString(text);

                        // avoid infinite loop when width of object less than width of one character
                        if (charsFit == 0)
                        {
                            break;
                        }

                        string textFit = text.Substring(0, charsFit).TrimEnd(new char[] { ' ' });
                        lines.Add(new Line(textFit, this, originalCharIndex));
                        text = text.Substring(charsFit)
                                        // Fix for linux system
                                        .TrimStart(' ');
                        originalCharIndex += charsFit;
                    }
                }
                else
                {
                    string ellipsis = "\u2026";
                    StringTrimming trimming = Renderer.Format.Trimming;
                    if (trimming == StringTrimming.EllipsisPath)
                        Renderer.Format.Trimming = StringTrimming.Character;
                    charsFit = MeasureString(text);

                    switch (trimming)
                    {
                        case StringTrimming.Character:
                        case StringTrimming.Word:
                            text = text.Substring(0, charsFit);
                            break;

                        case StringTrimming.EllipsisCharacter:
                        case StringTrimming.EllipsisWord:
                            if (charsFit < text.Length)
                            {
                                text = text.Substring(0, charsFit);
                                if (text.EndsWith(" "))
                                    text = text.Substring(0, text.Length - 1);
                                text += ellipsis;
                            }
                            break;

                        case StringTrimming.EllipsisPath:
                            if (charsFit < text.Length)
                            {
                                while (text.Length > 3)
                                {
                                    int mid = text.Length / 2;
                                    string newText = text.Substring(0, mid) + ellipsis + text.Substring(mid + 1);
                                    if (MeasureString(newText) == newText.Length)
                                    {
                                        text = newText;
                                        break;
                                    }
                                    else
                                    {
                                        text = text.Remove(mid, 1);
                                    }
                                }
                            }
                            break;
                    }

                    lines.Add(new Line(text, this, originalCharIndex));
                }
            }

            public StyleDescriptor WrapHtmlLines(StyleDescriptor style)
            {
                Line line = new Line("", this, this.originalCharIndex);
                lines.Add(line);
                Word word = new Word("", line);
                line.Words.Add(word);
                //    for img
                //RunImage img = null;
                //end     img
                string text = this.text;
                StringBuilder currentWord = new StringBuilder(100);
                float width = 0;
                bool skipSpace = true;
                int originalCharIndex = this.originalCharIndex;

                for (int i = 0; i < text.Length; i++)
                {
                    char lastChar = text[i];
                    if (lastChar == '&')
                    {
                        if (Converter.FromHtmlEntities(text, ref i, currentWord))
                        {
                            if (i >= text.Length - 1)
                            {
                                word.Runs.Add(new Run(currentWord.ToString(), style, word));
                                // check width
                                width += word.Width + Renderer.SpaceWidth;
                                if (width > Renderer.DisplayRect.Width)
                                {
                                    // line is too long, make a new line
                                    if (line.Words.Count > 1)
                                    {
                                        // if line has several words, delete the last word from the current line
                                        line.Words.RemoveAt(line.Words.Count - 1);
                                        // make new line
                                        line = new Line("", this, originalCharIndex);
                                        // and add word to it
                                        line.Words.Add(word);
                                        word.SetLine(line);
                                        lines.Add(line);
                                    }
                                }
#if DOTNET_4
                                currentWord.Clear();    // .NET 2.0 doesn't have Clear()
#else
                currentWord.Length = 0;
#endif
                                lastChar = ' ';
                            }
                            else
                            {
                                if (currentWord[currentWord.Length - 1] == '\t')
                                {
                                    currentWord.Length--;
                                    lastChar = '\t';

                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    if (lastChar == '<')
                    {
                        // probably html tag
                        StyleDescriptor newStyle = new StyleDescriptor(style.FontStyle, style.Color, style.BaseLine);
                        newStyle.Font = style.Font;
                        newStyle.Size = style.Size;
                        string tag = "";
                        bool match = false;

                        // <b>, <i>, <u>
                        if (i + 3 <= text.Length)
                        {
                            match = true;
                            tag = text.Substring(i, 3).ToLower();
                            if (tag == "<b>")
                                newStyle.FontStyle |= FontStyle.Bold;
                            else if (tag == "<i>")
                                newStyle.FontStyle |= FontStyle.Italic;
                            else if (tag == "<u>")
                                newStyle.FontStyle |= FontStyle.Underline;
                            else
                                match = false;

                            if (match)
                                i += 3;
                        }

                        // </b>, </i>, </u>
                        if (!match && i + 4 <= text.Length && text[i + 1] == '/')
                        {
                            match = true;
                            tag = text.Substring(i, 4).ToLower();
                            if (tag == "</b>")
                                newStyle.FontStyle &= ~FontStyle.Bold;
                            else if (tag == "</i>")
                                newStyle.FontStyle &= ~FontStyle.Italic;
                            else if (tag == "</u>")
                                newStyle.FontStyle &= ~FontStyle.Underline;
                            else
                                match = false;

                            if (match)
                                i += 4;
                        }

                        // <sub>, <sup> // <img· // <font
                        if (!match && i + 5 <= text.Length)
                        {
                            match = true;
                            tag = text.Substring(i, 5).ToLower();
                            if (tag == "<sub>")
                                newStyle.BaseLine = BaseLine.Subscript;
                            else if (tag == "<sup>")
                                newStyle.BaseLine = BaseLine.Superscript;
                            else if (tag == "<img ")
                            {
                                //try to found end tag
                                int right = text.IndexOf('>', i + 5);
                                if (right <= 0) match = false;
                                else
                                {
                                    //found img and parse them
                                    string src = null;
                                    string alt = " ";
                                    //currentWord = "";
                                    int src_ind = text.IndexOf("src=\"", i + 5);
                                    if (src_ind < right && src_ind >= 0)
                                    {
                                        src_ind += 5;
                                        int src_end = text.IndexOf("\"", src_ind);
                                        if (src_end < right && src_end >= 0)
                                        {
                                            src = text.Substring(src_ind, src_end - src_ind);
                                        }
                                    }
                                    int alt_ind = text.IndexOf("alt=\"", i + 5);
                                    if (alt_ind < right && alt_ind >= 0)
                                    {
                                        alt_ind += 5;
                                        int alt_end = text.IndexOf("\"", alt_ind);
                                        if (alt_end < right && alt_end >= 0)
                                        {
                                            alt = text.Substring(alt_ind, alt_end - alt_ind);
                                        }
                                    }
                                    //begin
                                    if (currentWord.Length != 0)
                                    {
                                        // finish the word
                                        word.Runs.Add(new Run(currentWord.ToString(), style, word));
                                    }
#if DOTNET_4
                                    currentWord.Clear();    // .NET 2.0 doesn't have Clear()
#else
                  currentWord.Length = 0;
#endif
                                    //end
                                    word.Runs.Add(new RunImage(src, alt, style, word));
                                    skipSpace = false;
                                    i = right - 4;
                                }
                            }
                            else if (tag == "<font")
                            {
                                //try to found end of open tag
                                int right = text.IndexOf('>', i + 5);
                                if (right <= 0) match = false;
                                else
                                {
                                    //found font and parse them
                                    string color = null;
                                    string face = null;
                                    string size = null;
                                    int color_ind = text.IndexOf("color=\"", i + 5);
                                    if (color_ind < right && color_ind >= 0)
                                    {
                                        color_ind += 7;
                                        int color_end = text.IndexOf("\"", color_ind);
                                        if (color_end < right && color_end >= 0)
                                        {
                                            color = text.Substring(color_ind, color_end - color_ind);
                                        }
                                    }

                                    int face_ind = text.IndexOf("face=\"", i + 5);
                                    if (face_ind < right && face_ind >= 0)
                                    {
                                        face_ind += 6;
                                        int face_end = text.IndexOf("\"", face_ind);
                                        if (face_end < right && face_end >= 0)
                                        {
                                            face = text.Substring(face_ind, face_end - face_ind);
                                        }
                                    }

                                    int size_ind = text.IndexOf("size=\"", i + 5);
                                    if (size_ind < right && size_ind >= 0)
                                    {
                                        size_ind += 6;
                                        int size_end = text.IndexOf("\"", size_ind);
                                        if (size_end < right && size_end >= 0)
                                        {
                                            size = text.Substring(size_ind, size_end - size_ind);
                                        }
                                    }

                                    if (color != null)
                                    {
                                        if (color.StartsWith("\"") && color.EndsWith("\""))
                                            color = color.Substring(1, color.Length - 2);
                                        if (color.StartsWith("#"))
                                        {
                                            newStyle.Color = Color.FromArgb((int)(0xFF000000 + uint.Parse(color.Substring(1), NumberStyles.HexNumber)));
                                        }
                                        else
                                        {
                                            newStyle.Color = Color.FromName(color);
                                        }
                                    }
                                    newStyle.Font = face;
                                    if (size != null)
                                    {

                                        try
                                        {
                                            size = size.Trim(' ');

                                            switch (size[0])
                                            {
                                                case '-':
                                                    size = size.Substring(1);
                                                    if (style.Size == 0)
                                                        newStyle.Size = Renderer.Font.Size - (float)Converter.FromString(typeof(float), size) * Renderer.FontScale;
                                                    else
                                                        newStyle.Size = style.Size - (float)Converter.FromString(typeof(float), size) * Renderer.FontScale;
                                                    break;
                                                case '+':
                                                    size = size.Substring(1);
                                                    if (style.Size == 0)
                                                        newStyle.Size = Renderer.Font.Size + (float)Converter.FromString(typeof(float), size) * Renderer.FontScale;
                                                    else
                                                        newStyle.Size = style.Size + (float)Converter.FromString(typeof(float), size) * Renderer.FontScale;
                                                    break;
                                                default: newStyle.Size = (float)Converter.FromString(typeof(float), size) * Renderer.FontScale; break;
                                            }
                                            if (newStyle.Size < 0) newStyle.Size = 0;
                                        }
                                        catch { }
                                    }
                                    i = right - 4;
                                }
                            }
                            else
                                match = false;

                            if (match)
                                i += 5;
                        }

                        // </sub>, </sup>
                        if (!match && i + 6 <= text.Length && text[i + 1] == '/')
                        {
                            match = true;
                            tag = text.Substring(i, 6).ToLower();
                            if (tag == "</sub>")
                                newStyle.BaseLine = BaseLine.Normal;
                            else if (tag == "</sup>")
                                newStyle.BaseLine = BaseLine.Normal;
                            else
                                match = false;

                            if (match)
                                i += 6;
                        }

                        // <strike>
                        if (!match && i + 8 <= text.Length && text.Substring(i, 8).ToLower() == "<strike>")
                        {
                            newStyle.FontStyle |= FontStyle.Strikeout;
                            match = true;
                            i += 8;
                        }

                        // </strike>
                        if (!match && i + 9 <= text.Length && text.Substring(i, 9).ToLower() == "</strike>")
                        {
                            newStyle.FontStyle &= ~FontStyle.Strikeout;
                            match = true;
                            i += 9;
                        }
                        /*
                        // <font color
                        if (!match && i + 12 < text.Length && text.Substring(i, 12).ToLower() == "<font color=")
                        {
                          int start = i + 12;
                          int end = start;
                          for (; end < text.Length && text[end] != '>'; end++)
                          {
                          }

                          if (end < text.Length)
                          {
                            string colorName = text.Substring(start, end - start);
                            if (colorName.StartsWith("\"") && colorName.EndsWith("\""))
                              colorName = colorName.Substring(1, colorName.Length - 2);
                            if (colorName.StartsWith("#"))
                            {
                              newStyle.Color = Color.FromArgb((int)(0xFF000000 + uint.Parse(colorName.Substring(1), NumberStyles.HexNumber)));
                            }
                            else
                            {
                              newStyle.Color = Color.FromName(colorName);
                            }
                            i = end + 1;
                            match = true;
                          }
                        }
                        */
                        // </font>
                        if (!match && i + 7 <= text.Length && text.Substring(i, 7).ToLower() == "</font>")
                        {
                            newStyle.Color = Renderer.BrushColor;
                            newStyle.Size = 0;
                            newStyle.Font = null;
                            match = true;
                            i += 7;
                        }

                        if (match)
                        {
                            if (currentWord.Length != 0)
                            {
                                // finish the word
                                word.Runs.Add(new Run(currentWord.ToString(), style, word));
                            }

#if DOTNET_4
                            currentWord.Clear();    // .NET 2.0 doesn't have Clear()
#else
              currentWord.Length = 0;
#endif
                            style = newStyle;
                            i--;

                            if (i >= text.Length - 1)
                            {
                                // check width
                                width += word.Width + Renderer.SpaceWidth;
                                if (width > Renderer.DisplayRect.Width)
                                {
                                    // line is too long, make a new line
                                    if (line.Words.Count > 1)
                                    {
                                        // if line has several words, delete the last word from the current line
                                        line.Words.RemoveAt(line.Words.Count - 1);
                                        // make new line
                                        line = new Line("", this, originalCharIndex);
                                        // and add word to it
                                        line.Words.Add(word);
                                        word.SetLine(line);
                                        lines.Add(line);
                                    }
                                }
                            }
                            continue;
                        }
                    }
                    if (lastChar == ' ' || lastChar == '\t' || i == text.Length - 1)
                    {
                        // finish the last word
                        bool isLastWord = i == text.Length - 1;
                        if (isLastWord)
                        {
                            currentWord.Append(lastChar);
                            skipSpace = false;
                        }

                        if (lastChar == '\t')
                            skipSpace = false;

                        // space
                        if (skipSpace)
                        {
                            currentWord.Append(lastChar);
                        }
                        else
                        {
                            // finish the word
                            if (currentWord.Length != 0)
                                word.Runs.Add(new Run(currentWord.ToString(), style, word));

                            // check width
                            width += word.Width + word.SpaceWidth;
                            if (width > Renderer.DisplayRect.Width)
                            {
                                // line is too long, make a new line
                                width = 0;
                                if (line.Words.Count > 1)
                                {
                                    // if line has several words, delete the last word from the current line
                                    line.Words.RemoveAt(line.Words.Count - 1);
                                    // make new line
                                    line = new Line("", this, originalCharIndex);
                                    // and add word to it
                                    line.Words.Add(word);
                                    word.SetLine(line);
                                    width += word.Width + word.SpaceWidth;
                                }
                                else
                                {
                                    line = new Line("", this, i + 1);
                                }
                                lines.Add(line);
                            }

                            // TAB symbol
                            if (lastChar == '\t')
                            {
                                if (currentWord.Length == 0 && line.Words.Count > 0 && line.Words[line.Words.Count - 1].Width == 0)
                                    line.Words.RemoveAt(line.Words.Count - 1);
                                word = new Word("\t", line);
                                line.Words.Add(word);
                                // adjust width
                                width = Renderer.GetTabPosition(width);
                            }

                            if (!isLastWord)
                            {
                                word = new Word("", line);
                                line.Words.Add(word);
#if DOTNET_4
                                currentWord.Clear();    // .NET 2.0 doesn't have Clear()
#else
                currentWord.Length = 0;
#endif
                                originalCharIndex = this.originalCharIndex + i + 1;
                                skipSpace = true;
                            }
                        }
                    }
                    else
                    {
                        // symbol
                        currentWord.Append(lastChar);
                        skipSpace = false;
                    }
                }

                return style;
            }

            public void AlignLines(bool forceJustify)
            {
                for (int i = 0; i < Lines.Count; i++)
                {
                    HorzAlign align = Renderer.HorzAlign;
                    if (align == HorzAlign.Justify && i == Lines.Count - 1 && !forceJustify)
                        align = HorzAlign.Left;
                    Lines[i].AlignWords(align);
                }
            }

            public void Draw()
            {
                foreach (Line line in Lines)
                {
                    line.Draw();
                }
            }
            #endregion

            public Paragraph(string text, AdvancedTextRenderer renderer, int originalCharIndex)
            {
                lines = new List<Line>();
                this.text = text;
                this.renderer = renderer;
                this.originalCharIndex = originalCharIndex;
            }
        }


        /// <summary>
        /// Line represents single text line. It consists of one or several <see cref="Words"/>.
        /// Simple line (that does not contain tabs, html tags, and is not justified) has
        /// single <see cref="Word"/> which contains all the text.
        /// </summary>
        public class Line
        {
            #region Fields
            private List<Word> words;
            private string text;
            private bool hasTabs;
            private Paragraph paragraph;
            private float top;
            private float width;
            private int originalCharIndex;
            private List<RectangleF> underlines;
            private List<RectangleF> strikeouts;
            #endregion

            #region Properties
            public List<Word> Words
            {
                get { return words; }
            }

            public string Text
            {
                get { return text; }
            }

            public bool HasTabs
            {
                get { return hasTabs; }
            }

            public float Left
            {
                get { return Words.Count > 0 ? Words[0].Left : 0; }
            }

            public float Top
            {
                get { return top; }
                set { top = value; }
            }

            public float Width
            {
                get { return width; }
            }

            public int OriginalCharIndex
            {
                get { return originalCharIndex; }
            }

            public AdvancedTextRenderer Renderer
            {
                get { return paragraph.Renderer; }
            }

            public StyleDescriptor Style
            {
                get
                {
                    if (Words.Count > 0)
                        if (Words[0].Runs.Count > 0)
                            return Words[0].Runs[0].Style;
                    return null;
                }
            }

            public bool Last
            {
                get { return paragraph.Lines[paragraph.Lines.Count - 1] == this; }
            }

            public List<RectangleF> Underlines
            {
                get { return underlines; }
            }

            public List<RectangleF> Strikeouts
            {
                get { return strikeouts; }
            }
            #endregion

            #region Private Methods
            private void PrepareUnderlines(List<RectangleF> list, FontStyle style)
            {
                list.Clear();
                if (Words.Count == 0)
                    return;

                if (Renderer.HtmlTags)
                {
                    float left = 0;
                    float right = 0;
                    bool styleOn = false;

                    foreach (Word word in Words)
                    {
                        foreach (Run run in word.Runs)
                        {
                            using (Font fnt = run.GetFont())
                            {
                                if ((fnt.Style & style) > 0)
                                {
                                    if (!styleOn)
                                    {
                                        styleOn = true;
                                        left = run.Left;
                                    }
                                    right = run.Left + run.Width;
                                }
                                if ((fnt.Style & style) == 0 && styleOn)
                                {
                                    styleOn = false;
                                    list.Add(new RectangleF(left, Top, right - left, 1));
                                }
                            }
                        }
                    }
                    // close the style
                    if (styleOn)
                        list.Add(new RectangleF(left, Top, right - left, 1));
                }
                else if ((Renderer.Font.Style & style) > 0)
                {
                    float lineWidth = Width;
                    if (Renderer.HorzAlign == HorzAlign.Justify && (!Last || (paragraph.Last && Renderer.ForceJustify)))
                        lineWidth = Renderer.DisplayRect.Width - Renderer.SpaceWidth;

                    list.Add(new RectangleF(Left, Top, lineWidth, 1));
                }
            }
            #endregion

            #region Public Methods
            public void AlignWords(HorzAlign align)
            {
                width = 0;

                // handle each word
                if (align == HorzAlign.Justify || HasTabs || Renderer.Wysiwyg || Renderer.HtmlTags)
                {
                    float left = 0;
                    Word word = null;
                    for (int i = 0; i < Words.Count; i++)
                    {
                        word = Words[i];
                        word.Left = left;

                        if (word.Text == "\t")
                        {
                            left = Renderer.GetTabPosition(left);
                            // remove tab
                            Words.RemoveAt(i);
                            i--;
                        }
                        else
                            left += word.Width + word.SpaceWidth;
                    }
                    if (word != null)
                        width = left - word.SpaceWidth;
                    else
                        width = left - Renderer.SpaceWidth;
                }
                else
                {
                    // join all words into one
                    Words.Clear();
                    Words.Add(new Word(text, this));
                    width = Words[0].Width;
                }

                float rectWidth = Renderer.DisplayRect.Width;
                if (align == HorzAlign.Justify)
                {
                    float delta = (rectWidth - width - Renderer.SpaceWidth) / (Words.Count - 1);
                    float curDelta = delta;
                    for (int i = 1; i < Words.Count; i++)
                    {
                        words[i].Left += curDelta;
                        curDelta += delta;
                    }
                }
                else
                {
                    float delta = 0;
                    if (align == HorzAlign.Center)
                        delta = (rectWidth - width) / 2;
                    else if (align == HorzAlign.Right)
                        delta = rectWidth - width - Renderer.SpaceWidth;
                    for (int i = 0; i < Words.Count; i++)
                    {
                        words[i].Left += delta;
                    }
                }

                // adjust X offset
                foreach (Word word in Words)
                {
                    if (Renderer.RightToLeft)
                        word.Left = Renderer.DisplayRect.Right - word.Left;
                    else
                        word.Left += Renderer.DisplayRect.Left;
                    word.AdjustRuns();
                    if (Renderer.RightToLeft && Renderer.PDFMode)
                        word.Left -= word.Width;
                }
            }

            public void MakeUnderlines()
            {
                PrepareUnderlines(underlines, FontStyle.Underline);
                PrepareUnderlines(strikeouts, FontStyle.Strikeout);
            }

            public void Draw()
            {
                foreach (Word word in Words)
                {
                    word.Draw();
                }

                if (Underlines.Count > 0 || Strikeouts.Count > 0)
                {
                    using (Pen pen = new Pen(Renderer.Brush, Renderer.Font.Size * 0.1f))
                    {
                        float h = Renderer.FontLineHeight;
                        float w = h * 0.1f; // to match .net char X offset
                                            // invert offset in case of rtl
                        if (Renderer.RightToLeft)
                            w = -w;

                        // emulate underline & strikeout
                        foreach (RectangleF rect in Underlines)
                        {
                            Renderer.Graphics.DrawLine(pen, rect.Left + w, rect.Top + h - w, rect.Right + w, rect.Top + h - w);
                        }

                        h /= 2;
                        foreach (RectangleF rect in Strikeouts)
                        {
                            Renderer.Graphics.DrawLine(pen, rect.Left + w, rect.Top + h, rect.Right + w, rect.Top + h);
                        }
                    }
                }
            }

            public float CalcHeight()
            {
                float height = -1;
                foreach (Word word in Words)
                {
                    height = Math.Max(height, word.CalcHeight());
                }
                if (height < 0)
                    height = Renderer.LineHeight;
                return height;
            }
            #endregion

            public Line(string text, Paragraph paragraph, int originalCharIndex)
            {
                this.words = new List<Word>();
                this.text = text;
                this.paragraph = paragraph;
                this.originalCharIndex = originalCharIndex;
                underlines = new List<RectangleF>();
                strikeouts = new List<RectangleF>();
                hasTabs = text.Contains("\t");

                // split text by spaces
                string[] words = text.Split(new char[] { ' ' });
                string textWithSpaces = "";

                foreach (string word in words)
                {
                    if (word == "")
                        textWithSpaces += " ";
                    else
                    {
                        // split text by tabs
                        textWithSpaces += word;
                        string[] tabWords = textWithSpaces.Split(new char[] { '\t' });

                        foreach (string word1 in tabWords)
                        {
                            if (word1 == "")
                                this.words.Add(new Word("\t", this));
                            else
                            {
                                this.words.Add(new Word(word1, this));
                                this.words.Add(new Word("\t", this));
                            }
                        }

                        // remove last tab
                        this.words.RemoveAt(this.words.Count - 1);

                        textWithSpaces = "";
                    }
                }
            }

            internal float CalcBaseLine()
            {

                float baseline = 0;
                foreach (Word word in Words)
                {
                    baseline = Math.Max(baseline, word.CalcBaseLine());
                }
                return baseline;
            }

            internal float CalcUnderBaseLine()
            {

                float underbaseline = 0;
                foreach (Word word in Words)
                {
                    underbaseline = Math.Max(underbaseline, word.CalcUnderBaseLine());
                }
                return underbaseline;
            }
        }


        /// <summary>
        /// Word represents single word. It may consist of one or several <see cref="Runs"/>, in case
        /// when HtmlTags are enabled in the main <see cref="AdvancedTextRenderer"/> class.
        /// </summary>
        public class Word
        {
            #region Fields
            private List<Run> runs;
            protected string text;
            private float left;
            private float width;
            internal Line line;
            #endregion

            #region Properties
            public string Text
            {
                get { return text; }
            }

            public float Left
            {
                get { return left; }
                set { left = value; }
            }

            public float Width
            {
                get
                {
                    if (width == -1)
                    {
                        if (Renderer.HtmlTags)
                        {
                            width = 0;
                            foreach (Run run in Runs)
                            {
                                width += run.Width;
                            }
                        }
                        else
                        {
                            width = Renderer.Graphics.MeasureString(text, Renderer.Font, 10000, StringFormat.GenericTypographic).Width;
                        }
                    }
                    return width;
                }
            }

            public float Top
            {
                get { return line.Top; }
            }

            public AdvancedTextRenderer Renderer
            {
                get { return line.Renderer; }
            }

            public List<Run> Runs
            {
                get { return runs; }
            }

            public float SpaceWidth
            {
                get
                {
                    if (Runs == null || Runs.Count == 0)
                        return Renderer.SpaceWidth;
                    return Runs[Runs.Count - 1].SpaceWidth;
                }
            }
            #endregion

            #region Public Methods
            public void AdjustRuns()
            {
                float left = Left;
                foreach (Run run in Runs)
                {
                    run.Left = left;

                    if (Renderer.RightToLeft)
                    {
                        left -= run.Width;
                        if (Renderer.PDFMode)
                            run.Left -= run.Width;
                    }
                    else
                        left += run.Width;
                }
            }

            public void SetLine(Line line)
            {
                this.line = line;
            }

            public void Draw()
            {
                if (Renderer.HtmlTags)
                {
                    foreach (Run run in Runs)
                    {
                        run.Draw();
                    }
                }
                else
                {
                    // don't draw underlines & strikeouts because they are drawn in the Line.Draw method
                    Font font = Renderer.Font;
                    bool disposeFont = false;
                    if ((Renderer.Font.Style & FontStyle.Underline) > 0 || (Renderer.Font.Style & FontStyle.Strikeout) > 0)
                    {
                        font = new Font(Renderer.Font, Renderer.Font.Style & ~FontStyle.Underline & ~FontStyle.Strikeout);
                        disposeFont = true;
                    }

                    if (Renderer.OutlinePen == null)
                    {
                        Renderer.Graphics.DrawString(Text, font, Renderer.Brush, Left, Top, Renderer.Format);
                    }
                    else
                    {
                        GraphicsPath path = new GraphicsPath();
                        path.AddString(Text, font.FontFamily, Convert.ToInt32(font.Style), Renderer.Graphics.DpiY * font.Size / 72, new PointF(Left - 1, Top - 1), Renderer.Format);
                        Renderer.Graphics.FillPath(Renderer.Brush, path);
                        Renderer.Graphics.DrawPath(Renderer.OutlinePen, path);
                    }

                    if (disposeFont)
                    {
                        font.Dispose();
                        font = null;
                    }
                }
            }

            internal float CalcHeight()
            {
                if (Renderer.HtmlTags)
                {
                    float height = -1;
                    foreach (Run run in Runs)
                    {
                        height = Math.Max(height, run.Height);
                    }
                    if (height < 0)
                        height = Renderer.LineHeight;
                    return height;
                }
                else
                {
                    return Renderer.LineHeight;
                }
            }

            internal float CalcBaseLine()
            {
                float baseLine = 0;
                if (Renderer.HtmlTags)
                {
                    foreach (Run run in Runs)
                    {
                        baseLine = Math.Max(baseLine, run.CurrentBaseLine);
                    }
                    return baseLine;
                }
                else
                {
                    return 0;
                }
            }

            internal float CalcUnderBaseLine()
            {
                float underbaseLine = 0;
                if (Renderer.HtmlTags)
                {
                    foreach (Run run in Runs)
                    {
                        underbaseLine = Math.Max(underbaseLine, run.CurrentUnderBaseLine);
                    }
                    return underbaseLine;
                }
                else
                {
                    return 0;
                }
            }
            #endregion

            public Word(string text, Line line)
            {
                this.text = text;
                runs = new List<Run>();
                this.line = line;
                width = -1;
            }
        }




        /// <summary>
        /// Represents character placement.
        /// </summary>
        public enum BaseLine
        {
            Normal,
            Subscript,
            Superscript
        }


        /// <summary>
        /// Represents a style used in HtmlTags mode.
        /// </summary>
        public class StyleDescriptor
        {
            #region Fields
            private FontStyle fontStyle;
            private Color color;
            private BaseLine baseLine;
            private string font;
            private float size;
            #endregion

            #region Properties
            public FontStyle FontStyle
            {
                get { return fontStyle; }
                set { fontStyle = value; }
            }

            public string Font
            {
                get { return font; }
                set { font = value; }
            }

            public float Size
            {
                get { return size; }
                set { size = value; }
            }
            public Color Color
            {
                get { return color; }
                set { color = value; }
            }

            public BaseLine BaseLine
            {
                get { return baseLine; }
                set { baseLine = value; }
            }
            #endregion

            #region Public Methods
            public override string ToString()
            {
                string result = "";

                if ((FontStyle & FontStyle.Bold) != 0)
                    result += "<b>";
                if ((FontStyle & FontStyle.Italic) != 0)
                    result += "<i>";
                if ((FontStyle & FontStyle.Underline) != 0)
                    result += "<u>";
                if ((FontStyle & FontStyle.Strikeout) != 0)
                    result += "<strike>";
                if (BaseLine == BaseLine.Subscript)
                    result += "<sub>";
                if (BaseLine == BaseLine.Superscript)
                    result += "<sup>";

                result += "<font color=\"";
                if (ColorExt.IsKnownColor(Color))
                    result += Color.Name;
                else
                    result += "#" + Color.ToArgb().ToString("x");
                result += "\"";
                if (Font != null)
                    result += " face=\"" + Font + "\"";
                if (Size != 0)
                    result += " size=\"" + Math.Round(Size).ToString() + "\"";
                result += ">";

                return result;
            }
            #endregion

            public StyleDescriptor(FontStyle fontStyle, Color color, BaseLine baseLine)
            {
                this.fontStyle = fontStyle;
                this.color = color;
                this.baseLine = baseLine;
            }
        }


        /// <summary>
        /// Represents sequence of characters that have the same <see cref="Style"/>.
        /// </summary>
        public class Run
        {
            #region Fields
            protected string text;
            private StyleDescriptor style;
            protected Word word;
            private float left;
            protected float width;
            protected float lineHeight;
            protected float fontLineHeight;
            private float baseLine;
            protected float underBaseLine;
            protected float spaceWidth;
            #endregion

            #region Properties
            public string Text
            {
                get { return text; }
            }

            public StyleDescriptor Style
            {
                get { return style; }
            }

            public AdvancedTextRenderer Renderer
            {
                get { return word.Renderer; }
            }

            public float Left
            {
                get { return left; }
                set { left = value; }
            }

            public float LineHeight
            {
                get
                {
                    if (lineHeight == 0)
                    {
                        if (style.Font == null && style.Size <= 0)
                            lineHeight = Renderer.LineHeight;
                        else
                            lineHeight = GetFont().GetHeight(Renderer.Graphics);
                    }
                    return lineHeight;
                }
            }

            virtual public float CurrentBaseLine
            {
                get
                {
                    if (baseLine < 0)
                    {
                        Font ff = GetFont();
                        float lineSpace = ff.FontFamily.GetLineSpacing(Style.FontStyle);
                        float ascent = ff.FontFamily.GetCellAscent(Style.FontStyle);
                        baseLine = FontLineHeight * ascent / lineSpace;
                        underBaseLine = FontLineHeight - baseLine;
                    }
                    return baseLine;
                }
            }

            virtual public float CurrentUnderBaseLine
            {
                get
                {
                    if (underBaseLine < 0)
                    {
                        Font ff = GetFont();
                        float lineSpace = ff.FontFamily.GetLineSpacing(Style.FontStyle);
                        float ascent = ff.FontFamily.GetCellAscent(Style.FontStyle);
                        baseLine = FontLineHeight * ascent / lineSpace;
                        underBaseLine = FontLineHeight - baseLine;
                    }
                    return baseLine;
                }
            }

            public float FontLineHeight
            {
                get
                {
                    if (fontLineHeight == 0)
                    {
                        if (style.Font == null && style.Size <= 0)
                            fontLineHeight = Renderer.FontLineHeight;
                        else
                            fontLineHeight = GetFont().GetHeight(Renderer.Graphics);
                    }
                    return fontLineHeight;
                }
            }

            public virtual float Top
            {
                get
                {
                    float baseLine = 0;
                    if (Style.BaseLine == BaseLine.Subscript)
                        baseLine += FontLineHeight * 0.45f;
                    else if (Style.BaseLine == BaseLine.Superscript)
                        baseLine -= FontLineHeight * 0.15f;
                    return word.Top + word.line.CalcBaseLine() - CurrentBaseLine + baseLine;
                }
            }

            virtual public float Width
            {
                get { return width; }
            }

            virtual public float Height
            {
                get
                {
                    return LineHeight;
                }
            }



            public float SpaceWidth
            {
                get
                {
                    if (spaceWidth < 0)
                    {
                        spaceWidth = CalculateSpaceSize(Renderer.Graphics, GetFont());// Renderer.Graphics.MeasureString(" ", GetFont()).Width;
                    }
                    return spaceWidth;
                }
            }
            #endregion

            #region Private Methods
            private Font GetFont(bool disableUnderlinesStrikeouts)
            {
                float fontSize = Renderer.Font.Size;
                if (Style.Size != 0)
                    fontSize = Style.Size;
                if (Style.BaseLine != BaseLine.Normal)
                    fontSize *= 0.6f;

                FontStyle fontStyle = Style.FontStyle;
                if (disableUnderlinesStrikeouts)
                    fontStyle = fontStyle & ~FontStyle.Underline & ~FontStyle.Strikeout;
                if (Style.Font != null)
                    return new Font(Style.Font, fontSize, fontStyle);
                return new Font(Renderer.Font.Name, fontSize, fontStyle);
            }
            #endregion

            #region Public Methods
            public Font GetFont()
            {
                return GetFont(false);
            }

            public Brush GetBrush()
            {
                return new SolidBrush(Style.Color);
            }

            public virtual void Draw()
            {
                using (Font font = GetFont(true))
                using (Brush brush = GetBrush())
                {
                    Renderer.Graphics.DrawString(text, font, brush, Left, Top, Renderer.Format);
                }
            }
            #endregion

            public Run(string text, StyleDescriptor style, Word word)
            {
                baseLine = float.MinValue;
                underBaseLine = float.MinValue;
                this.text = text;
                this.style = new StyleDescriptor(style.FontStyle, style.Color, style.BaseLine);
                this.style.Font = style.Font;
                this.style.Size = style.Size;
                this.word = word;
                spaceWidth = -1;

                using (Font font = GetFont())
                {
                    width = Renderer.Graphics.MeasureString(text, font, 10000, StringFormat.GenericTypographic).Width;
                }
            }
        }

        /// <summary>
        /// Represents inline Image.
        /// </summary>
        internal class RunImage : Run
        {
            public Image Image { get { return image; } }
            override public float Width
            {
                get
                {
                    if (Image == null) return base.Width;
                    return Image.Width;
                }
            }

            override public float Top
            {
                get
                {
                    float baseLine = 0;
                    if (Style.BaseLine == BaseLine.Subscript)
                        baseLine += FontLineHeight * 0.45f;
                    else if (Style.BaseLine == BaseLine.Superscript)
                        baseLine -= FontLineHeight * 0.15f;
                    return word.Top + word.line.CalcBaseLine() - CurrentBaseLine + baseLine;
                }
            }

            override public float CurrentBaseLine
            {
                get
                {
                    if (Image == null) return base.CurrentBaseLine;
                    return Image.Height;
                }
            }

            override public float Height
            {
                get
                {
                    if (Image == null) return base.Height;
                    return Image.Height + word.line.CalcUnderBaseLine();
                }
            }

            private Image image;

            override public void Draw()
            {
                if (Image == null)
                {
                    base.Draw();
                    return;
                }
                Renderer.Graphics.DrawImage(Image, Left, Top);// (FText, font, brush, Left, Top, Renderer.Format);
            }

            public static Bitmap ResizeImage(Image image, float scale)
            {
                int width = (int)(image.Width * scale);
                int height = (int)(image.Height * scale);
                if (width == 0) width = 1;
                if (height == 0) height = 1;
                Rectangle destRect = new Rectangle(0, 0, width, height);
                Bitmap destImage = new Bitmap(width, height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (Graphics graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (System.Drawing.Imaging.ImageAttributes wrapMode = new System.Drawing.Imaging.ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            }

            public RunImage(string src, string text, StyleDescriptor style, Word word) : base(text, style, word)
            {
                underBaseLine = 0;
                image = ResizeImage(InlineImageCache.Load(Renderer.Cache, src), Renderer.Scale);
            }
        }
    }


    /// <summary>
    /// Standard text renderer uses standard DrawString method to draw text. It also supports:
    /// - text rotation;
    /// - fonts with non-standard width ratio.
    /// In case your text is justified, or contains html tags, use the <see cref="AdvancedTextRenderer"/>
    /// class instead.
    /// </summary>
    internal class StandardTextRenderer
    {
        public static void Draw(string text, Graphics g, Font font, Brush brush, Pen outlinePen,
          RectangleF rect, StringFormat format, int angle, float widthRatio)
        {
            GraphicsState state = g.Save();
            g.SetClip(rect, CombineMode.Intersect);
            g.TranslateTransform(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            g.RotateTransform(angle);
            rect.X = -rect.Width / 2;
            rect.Y = -rect.Height / 2;

            if ((angle >= 90 && angle < 180) || (angle >= 270 && angle < 360))
                rect = new RectangleF(rect.Y, rect.X, rect.Height, rect.Width);

            g.ScaleTransform(widthRatio, 1);
            rect.X /= widthRatio;
            rect.Width /= widthRatio;

            if (outlinePen == null)
            {
                g.DrawString(text, font, brush, rect, format);
            }
            else
            {
                GraphicsPath path = new GraphicsPath();
                path.AddString(text, font.FontFamily, Convert.ToInt32(font.Style), g.DpiY * font.Size / 72, rect, format);
                g.FillPath(brush, path);
                g.DrawPath(outlinePen, path);
            }

            g.Restore(state);
        }
    }

    /// <summary>
    /// Cache for rendering img tags in textobject.
    /// You can use only HTTP[s] protocol with absolute urls.
    /// </summary>
    public class InlineImageCache : IDisposable
    {
        #region Private Fields

        private WebClient client;

        private Dictionary<string, CacheItem> items;

        private bool serialized;

        private object locker;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Is serialized
        /// </summary>
        public bool Serialized { get { return serialized; } set { serialized = value; } }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Get or set WebClient for downloading imgs by url
        /// </summary>
        private WebClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new WebClient();
                }
                return client;
            }
            set
            {
                client = value;
            }
        }

        #endregion Private Properties

        #region Public Events

        /// <summary>
        /// Occurs before image load
        /// </summary>
        public static event EventHandler<LoadEventArgs> AfterLoad;
        /// <summary>
        /// Occurs after image load
        /// </summary>
        public static event EventHandler<LoadEventArgs> BeforeLoad;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Enumerates all values
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CacheItem> AllItems()
        {
            List<CacheItem> list = new List<CacheItem>();
            lock (locker)
            {
                if (items != null)
                {
                    foreach (KeyValuePair<string, CacheItem> item in items)
                    {
                        item.Value.Src = item.Key;
                        list.Add(item.Value);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Return CacheItem by src
        /// </summary>
        /// <param name="src">Src attribute from img tag</param>
        /// <returns></returns>
        public CacheItem Get(string src)
        {
            CacheItem item = null;
            if (!Validate(src))
                item = new CacheItem();
            if (String.IsNullOrEmpty(src))
                return item;
            lock (locker)
            {
                if (items == null)
                {
                    items = new Dictionary<string, CacheItem>();
                    if (item == null)
                        item = new CacheItem();
                    items[src] = item;
                    Serialized = false;
                }
                if (items.ContainsKey(src))
                    return items[src];
            }
            return item;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Image Load(string src)
        {
            CacheItem item = null;
            if (String.IsNullOrEmpty(src))
                item = new CacheItem();
            else
                lock (locker)
                {
                    if (items == null)
                        items = new Dictionary<string, CacheItem>();
                    else
                      if (items.ContainsKey(src))
                        return items[src].Image;
                    item = new CacheItem();
                    if (Validate(src))
                    {
                        try
                        {
                            if (src.StartsWith("data:"))
                            {
                                item.Set(src.Substring(src.IndexOf("base64,") + "base64,".Length));
                            }
                            else
                                item.Set(Client.DownloadData(src));
                        }
                        catch
                        {
                            item.Set("");
                        }
                    }
                    items[src] = item;
                    Serialized = false;
                }
            item.Src = src;
            return item.Image;
        }

        /// <summary>
        /// Set CacheItem by src
        /// </summary>
        /// <param name="src">Src attribute from img tag</param>
        /// <param name="item">CacheItem</param>
        /// <returns></returns>
        public CacheItem Set(string src, CacheItem item)
        {
            if (String.IsNullOrEmpty(src))
                return new CacheItem();
            lock (locker)
            {
                if (items == null)
                    items = new Dictionary<string, CacheItem>();
                if (!Validate(src))
                    item = new CacheItem();
                items[src] = item;
                Serialized = false;
            }
            item.Src = src;
            return item;
        }

        /// <summary>
        /// Validate src attribute from image
        /// </summary>
        /// <param name="src">Src attribute from img tag</param>
        /// <returns>return true if src is valid</returns>
        public bool Validate(string src)
        {
            if (String.IsNullOrEmpty(src))
                return false;
            src = src.ToLower();
            if (src.StartsWith("http://"))
                return true;
            if (src.StartsWith("https://"))
                return true;
            if (src.StartsWith("data:") && src.IndexOf("base64,") > 0)
                return true;
            return false;
        }

        #endregion Public Methods

        #region Internal Methods

        static internal Image Load(InlineImageCache cache, string src)
        {
            LoadEventArgs args = new LoadEventArgs(cache, src);
            if (BeforeLoad != null) BeforeLoad(null, args);
            Image result = null;
            if (!args.Handled) result = cache.Load(src);
            args.Handled = false;
            if (AfterLoad != null) AfterLoad(null, args);
            if (args.Handled)
                return cache.Get(src).Image;
            return result;
        }

        #endregion Internal Methods

        #region Public Constructors

        /// <inheritdoc/>
        public InlineImageCache()
        {
            locker = new object();
            client = null;
        }

        /// <summary>
        /// 
        /// </summary>
        ~InlineImageCache()
        {
            Dispose(false);
        }

        #endregion Public Constructors

        #region Public Classes

        /// <summary>
        /// Item of image cache Dictionary
        /// </summary>
        public class CacheItem : IDisposable
        {
            #region Private Fields

            private string base64;

            private bool error;

            private Image image;

            //private int FId;
            private string src;

            private byte[] stream;

            #endregion Private Fields

            #region Public Properties

            /// <summary>
            /// Get Base64 string
            /// </summary>
            public string Base64
            {
                get
                {
                    try
                    {//For strange img tag
                        if (base64 != null)
                            return base64;
                        if (stream != null)
                        {
                            base64 = Convert.ToBase64String(stream);
                            return base64;
                        }
                        if (image != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Flush();
                                stream = ms.ToArray();
                            }
                            base64 = Convert.ToBase64String(stream);
                            return base64;
                        }
                    }
                    catch { }
                    GetErrorImage();
                    return "";
                }
            }

            /// <summary>
            /// Return true if has some error with Image
            /// </summary>
            public bool Error
            {
                get { return error; }
            }

            /// <summary>
            /// Get Image
            /// </summary>
            public Image Image
            {
                get
                {
                    try
                    {//for strange img tag
                        if (image != null)
                            return image;
                        if (stream != null)
                        {
                            MemoryStream ms = new MemoryStream(stream);
                            image = Bitmap.FromStream(ms);
                            return image;
                        }
                        if (base64 != null)
                        {
                            this.stream = Convert.FromBase64String(base64);
                            MemoryStream ms = new MemoryStream(stream);
                            image = Bitmap.FromStream(ms);
                            return image;
                        }
                    }
                    catch { }
                    return GetErrorImage();
                }
            }

            /// <summary>
            /// Get byte array
            /// </summary>
            public byte[] Stream
            {
                get
                {
                    if (stream != null) return stream;
                    if (base64 != null)
                    {
                        stream = Convert.FromBase64String(base64);
                        return stream;
                    }
                    if (image != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Flush();
                            stream = ms.ToArray();
                        }
                        return stream;
                    }
                    return new byte[0];
                }
            }

            #endregion Public Properties

            #region Internal Properties

            internal string Src
            {
                get { return src; }
                set { src = value; }
            }

            #endregion Internal Properties

            #region Public Methods

            /// <summary>
            /// Return error image and set true to error property
            /// </summary>
            /// <returns></returns>
            public Image GetErrorImage()
            {
                error = true;
                base64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAFdJREFUOE9jbGlq+c9AIqipq2GEawEZQAo4dvgYqoXD0QAGhv9ATyKCBY1PXBjANKEbBjSWOANA9mPRDBImzgCKXECVMMCTsojzwtAzAOQvUjCJmRe/cgDt6ZAkZx23LwAAAABJRU5ErkJggg==";
                src = "data:image/png;base64," + base64;
                stream = Convert.FromBase64String(base64);
                using (MemoryStream ms = new MemoryStream(stream))
                    image = Bitmap.FromStream(ms);
                return image;
            }

            /// <summary>
            /// Set value for cache item
            /// </summary>
            /// <param name="base64">Image encoded base64 string</param>
            public void Set(string base64)
            {
                this.base64 = base64;
                image = null;
                stream = null;
            }

            /// <summary>
            /// Set value for cache item
            /// </summary>
            /// <param name="img">Image</param>
            public void Set(Image img)
            {
                base64 = null;
                image = img;
                stream = null;
            }

            /// <summary>
            /// Set value for cache item
            /// </summary>
            /// <param name="arr">Image</param>
            public void Set(byte[] arr)
            {
                base64 = null;
                image = null;
                stream = arr;
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            /// <summary>
            /// 
            /// </summary>
            /// <param name="disposing"></param>
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        if (image != null)
                        {
                            image.Dispose();
                            image = null;
                        }
                        // TODO: dispose managed state (managed objects).
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~CacheItem() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion

            #endregion Public Methods
        }

        /// <summary>
        /// WebClientEventArgs
        /// </summary>
        public class LoadEventArgs : EventArgs
        {
            #region Private Fields

            private InlineImageCache cache;
            private bool handled;
            private string source;

            #endregion Private Fields

            #region Public Properties

            /// <summary>
            /// Gets a cache
            /// </summary>
            public InlineImageCache Cache { get { return cache; } }

            /// <summary>
            /// Gets or sets a value indicating whether the event was handled.
            /// </summary>
            public bool Handled { get { return handled; } set { handled = value; } }

            /// <summary>
            /// Gets or sets a url from src attribue of img tag
            /// </summary>
            public string Source { get { return source; } set { source = value; } }

            #endregion Public Properties

            #region Internal Constructors

            internal LoadEventArgs(InlineImageCache c, string src)
            {
                cache = c;
                source = src;
                handled = false;
            }

            #endregion Internal Constructors
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if (this.items != null)
                {
                    Dictionary<string, CacheItem> items = this.items;
                    this.items = null;
                    foreach (CacheItem item in items.Values)
                        item.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~InlineImageCache() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion Public Classes
    }
}