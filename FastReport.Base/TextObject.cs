using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Format;
using FastReport.Code;
using System.Windows.Forms;
using System.Drawing.Design;

namespace FastReport
{
    /// <summary>
    /// Specifies the horizontal alignment of a text in the TextObject object.
    /// </summary>
    public enum HorzAlign
    {
        /// <summary>
        /// Specifies that text is aligned in the left of the layout rectangle.
        /// </summary>
        Left,

        /// <summary>
        /// Specifies that text is aligned in the center of the layout rectangle.
        /// </summary>
        Center,

        /// <summary>
        /// Specifies that text is aligned in the right of the layout rectangle.
        /// </summary>
        Right,

        /// <summary>
        /// Specifies that text is aligned in the left and right sides of the layout rectangle.
        /// </summary>
        Justify
    }

    /// <summary>
    /// Specifies the vertical alignment of a text in the TextObject object.
    /// </summary>
    public enum VertAlign
    {
        /// <summary>
        /// Specifies that text is aligned in the top of the layout rectangle.
        /// </summary>
        Top,

        /// <summary>
        /// Specifies that text is aligned in the center of the layout rectangle.
        /// </summary>
        Center,

        /// <summary>
        /// Specifies that text is aligned in the bottom of the layout rectangle.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// The type of text renderer
    /// </summary>
    public enum TextRenderType
    {
        /// <summary>
        /// The default render
        /// </summary>
        Default,
        /// <summary>
        /// Render with some html tags and stable logic
        /// </summary>
        HtmlTags,
        /// <summary>
        /// Render with img tags, span etc. Experimental and unstable logic
        /// </summary>
        HtmlParagraph
    }

    /// <summary>
    /// The format of paragraph
    /// </summary>
    [TypeConverterAttribute(typeof(TypeConverters.FRExpandableObjectConverter))]
    public class ParagraphFormat
    {
        private float firstLineIndent;
        private float lineSpacing;
        private LineSpacingType lineSpacingType;
        private bool skipFirstLineIndent;

        /// <summary>
        /// The first line on each paragraph, not effect if value less then  0
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0f)]
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float FirstLineIndent
        {
            get { return firstLineIndent; }
            set { if (value >= 0) firstLineIndent = value; }
        }

        /// <summary>
        /// The distance between lines, not effect if value less then 0
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0f)]
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float LineSpacing
        {
            get { return lineSpacing; }
            set { if (value >= 0) lineSpacing = value; }
        }

        /// <summary>
        /// The spacing type for distance between line calculation
        /// </summary>
        [Browsable(true)]
        [DefaultValue(LineSpacingType.Single)]
        public LineSpacingType LineSpacingType
        {
            get { return lineSpacingType; }
            set { lineSpacingType = value; }
        }

        /// <summary>
        /// The value for a multiplication line height for adding spacing
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0f)]
        public float LineSpacingMultiple
        {
            get { return lineSpacing / 100f; }
            set { if (value >= 0) lineSpacing = value * 100f; }
        }

        /// <summary>
        /// Skip the line indent in the first paragraph, for broken paragraphs
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool SkipFirstLineIndent
        {
            get { return skipFirstLineIndent; }
            set { skipFirstLineIndent = value; }
        }

        /// <summary>
        /// clone with new scale;
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        internal ParagraphFormat MultipleScale(float scale)
        {
            ParagraphFormat clone = new ParagraphFormat();
            clone.lineSpacingType = lineSpacingType;
            if (LineSpacingType == LineSpacingType.Multiple)
                clone.lineSpacing = lineSpacing;
            else
                clone.lineSpacing = lineSpacing * scale;
            clone.firstLineIndent = firstLineIndent * scale;
            clone.skipFirstLineIndent = skipFirstLineIndent;
            return clone;
        }

        internal void Assign(ParagraphFormat p)
        {
            lineSpacingType = p.lineSpacingType;
            lineSpacing = p.lineSpacing;
            firstLineIndent = p.firstLineIndent;
            skipFirstLineIndent = p.skipFirstLineIndent;
        }

        public override bool Equals(object obj)
        {
            ParagraphFormat format = obj as ParagraphFormat;
            return format != null &&
                   firstLineIndent == format.firstLineIndent &&
                   lineSpacing == format.lineSpacing &&
                   lineSpacingType == format.lineSpacingType &&
                   skipFirstLineIndent == format.skipFirstLineIndent;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = -1051315095;
                hashCode = hashCode * -1521134295 + firstLineIndent.GetHashCode();
                hashCode = hashCode * -1521134295 + lineSpacing.GetHashCode();
                hashCode = hashCode * -1521134295 + lineSpacingType.GetHashCode();
                hashCode = hashCode * -1521134295 + skipFirstLineIndent.GetHashCode();
                return hashCode;
            }
        }
    }

    /// <summary>
    /// The spacing type between lines
    /// </summary>
    public enum LineSpacingType
    {
        /// <summary>
        /// Single spacing, not effect from LineSpacing
        /// </summary>
        Single,
        /// <summary>
        /// Minimal spacing in exactly size
        /// </summary>
        AtLeast,
        /// <summary>
        /// The specific distance between the lines, for some exports, does not work if the distance value is too small.
        /// </summary>
        Exactly,
        /// <summary>
        /// The calculated distance between lines, for some exports, does not work if the distance value is too small.
        /// </summary>
        Multiple
    }

    /// <summary>
    /// Specifies the behavior of the <b>AutoShrink</b> feature of <b>TextObject</b>.
    /// </summary>
    public enum AutoShrinkMode
    {
        /// <summary>
        /// AutoShrink is disabled.
        /// </summary>
        None,

        /// <summary>
        /// AutoShrink decreases the <b>Font.Size</b> property of the <b>TextObject</b>.
        /// </summary>
        FontSize,

        /// <summary>
        /// AutoShrink decreases the <b>FontWidthRatio</b> property of the <b>TextObject</b>.
        /// </summary>
        FontWidth
    }

    /// <summary>
    /// Represents the Text object that may display one or several text lines.
    /// </summary>
    /// <remarks>
    /// Specify the object's text in the <see cref="TextObjectBase.Text">Text</see> property. 
    /// Text may contain expressions and data items, for example: "Today is [Date]". When report 
    /// is running, all expressions are calculated and replaced with actual values, so the text 
    /// would be "Today is 01.01.2008".
    /// <para/>The symbols used to find expressions in a text are set in the 
    /// <see cref="TextObjectBase.Brackets">Brackets</see> property. You also may disable expressions 
    /// using the <see cref="TextObjectBase.AllowExpressions">AllowExpressions</see> property.
    /// <para/>To format an expression value, use the <see cref="Format"/> property.
    /// </remarks>
    public partial class TextObject : TextObjectBase
    {
        #region Fields
        private bool autoWidth;
        private HorzAlign horzAlign;
        private VertAlign vertAlign;
        private int angle;
        private bool rightToLeft;
        private bool wordWrap;
        private bool underlines;
        private Font font;
        private FillBase textFill;
        private TextOutline textOutline;
        private StringTrimming trimming;
        private float fontWidthRatio;
        private float firstTabOffset;
        private float tabWidth;
        private bool clip;
        private ConditionCollection highlight;
        private bool wysiwyg;
        private float lineHeight;
        private bool forceJustify;
        private TextRenderType textRenderType;
        private AutoShrinkMode autoShrink;
        private float autoShrinkMinSize;
        private float paragraphOffset;
        private FillBase savedTextFill;
        private Font savedFont;
        private string savedText;
        private FormatBase savedFormat;
        private InlineImageCache inlineImageCache;
        private ParagraphFormat paragraphFormat;
        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets a paragraph format for a new html rendering type, not for others rendering
        /// </summary>
        [Category("Appearance")]
        public ParagraphFormat ParagraphFormat
        {
            get { return paragraphFormat; }
            set { paragraphFormat = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines if the text object should handle its width automatically.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool AutoWidth
        {
            get { return autoWidth; }
            set { autoWidth = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the font size should shrink to
        /// display the longest text line without word wrap.
        /// </summary>
        /// <remarks>
        /// To limit the minimum size, use the <see cref="AutoShrinkMinSize"/> property.
        /// </remarks>
        [DefaultValue(AutoShrinkMode.None)]
        [Category("Behavior")]
        public AutoShrinkMode AutoShrink
        {
            get { return autoShrink; }
            set { autoShrink = value; }
        }

        /// <summary>
        /// Gets or sets the minimum size of font (or minimum width ratio) if the <see cref="AutoShrink"/>
        /// mode is on.
        /// </summary>
        /// <remarks>
        /// This property determines the minimum font size (in case the <see cref="AutoShrink"/> property is set to
        /// <b>FontSize</b>), or the minimum font width ratio (if <b>AutoShrink</b> is set to <b>FontWidth</b>).
        /// <para/>The default value is 0, that means no limits.
        /// </remarks>
        [DefaultValue(0f)]
        [Category("Behavior")]
        public float AutoShrinkMinSize
        {
            get { return autoShrinkMinSize; }
            set { autoShrinkMinSize = value; }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of a text in the TextObject object.
        /// </summary>
        [DefaultValue(HorzAlign.Left)]
        [Category("Appearance")]
        public HorzAlign HorzAlign
        {
            get { return horzAlign; }
            set { horzAlign = value; }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of a text in the TextObject object.
        /// </summary>
        [DefaultValue(VertAlign.Top)]
        [Category("Appearance")]
        public VertAlign VertAlign
        {
            get { return vertAlign; }
            set { vertAlign = value; }
        }

        /// <summary>
        /// Gets or sets the text angle, in degrees.
        /// </summary>
        [DefaultValue(0)]
        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.AngleEditor, FastReport", typeof(UITypeEditor))]
        public int Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the component should draw right-to-left for RTL languages.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool RightToLeft
        {
            get { return rightToLeft; }
            set { rightToLeft = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates if lines are automatically word-wrapped.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool WordWrap
        {
            get { return wordWrap; }
            set { wordWrap = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines if the text object will underline each text line.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        public bool Underlines
        {
            get { return underlines; }
            set { underlines = value; }
        }

        /// <summary>
        /// Gets or sets the font settings for this object.
        /// </summary>
        [Category("Appearance")]
        public Font Font
        {
            get { return font; }
            set
            {
                font = value;
                if (!String.IsNullOrEmpty(Style))
                    Style = "";
            }
        }

        /// <summary>
        /// Gets or sets the fill color used to draw a text.
        /// </summary>
        /// <remarks>
        /// Default fill is <see cref="SolidFill"/>. You may specify other fill types, for example:
        /// <code>
        /// text1.TextFill = new HatchFill(Color.Black, Color.White, HatchStyle.Cross);
        /// </code>
        /// Use the <see cref="TextColor"/> property to set the solid text color.
        /// </remarks>
        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.FillEditor, FastReport", typeof(UITypeEditor))]
        public FillBase TextFill
        {
            get { return textFill; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("TextFill");
                textFill = value;
                if (!String.IsNullOrEmpty(Style))
                    Style = "";
            }
        }

        /// <summary>
        /// Gets or sets the text outline.
        /// </summary>
        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.OutlineEditor, FastReport", typeof(UITypeEditor))]
        public TextOutline TextOutline
        {
            get { return textOutline; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("TextOutline");
                textOutline = value;
                if (!String.IsNullOrEmpty(Style))
                    Style = "";
            }
        }

        /// <summary>
        /// Gets or sets the text color in a simple manner.
        /// </summary>
        /// <remarks>
        /// This property can be used in a report script to change the text color of the object. It is 
        /// equivalent to: <code>textObject1.TextFill = new SolidFill(color);</code>
        /// </remarks>
        [Browsable(false)]
        public Color TextColor
        {
            get { return TextFill is SolidFill ? (TextFill as SolidFill).Color : Color.Black; }
            set { TextFill = new SolidFill(value); }
        }

        /// <summary>
        /// Gets or sets the string trimming options.
        /// </summary>
        [DefaultValue(StringTrimming.None)]
        [Category("Behavior")]
        public StringTrimming Trimming
        {
            get { return trimming; }
            set { trimming = value; }
        }

        /// <summary>
        /// Gets or sets the width ratio of the font. 
        /// </summary>
        /// <remarks>
        /// Default value is 1. To make a font wider, set a value grether than 1; to make a font narrower,
        /// set a value less than 1.
        /// </remarks>
        [DefaultValue(1f)]
        [Category("Appearance")]
        public float FontWidthRatio
        {
            get { return fontWidthRatio; }
            set { fontWidthRatio = value; }
        }

        /// <summary>
        /// Gets or sets the height of single text line, in pixels.
        /// </summary>
        [DefaultValue(0f)]
        [Category("Appearance")]
        public float LineHeight
        {
            get { return lineHeight; }
            set { lineHeight = value; }
        }

        /// <summary>
        /// Gets or sets the offset of the first TAB symbol.
        /// </summary>
        [DefaultValue(0f)]
        [Category("Appearance")]
        //[TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float FirstTabOffset
        {
            get { return firstTabOffset; }
            set { firstTabOffset = value; }
        }

        /// <summary>
        /// Gets or sets the width of TAB symbol, in pixels.
        /// </summary>
        [DefaultValue(58f)]
        [Category("Appearance")]
        public float TabWidth
        {
            get { return tabWidth; }
            set { tabWidth = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates if text should be clipped inside the object's bounds.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool Clip
        {
            get { return clip; }
            set { clip = value; }
        }

        /// <summary>
        /// Gets the collection of conditional highlight attributes.
        /// </summary>
        /// <remarks>
        /// Conditional highlight is used to change the visual appearance of the Text object
        /// depending on some condition(s). For example, you may highlight negative values displayed by
        /// the Text object with red color. To do this, add the highlight condition:
        /// <code>
        /// TextObject text1;
        /// HighlightCondition highlight = new HighlightCondition();
        /// highlight.Expression = "Value &lt; 0";
        /// highlight.Fill = new SolidFill(Color.Red);
        /// highlight.ApplyFill = true;
        /// text1.Highlight.Add(highlight);
        /// </code>
        /// </remarks>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.HighlightEditor, FastReport", typeof(UITypeEditor))]
        public ConditionCollection Highlight
        {
            get { return highlight; }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the text object should display its contents similar to the printout.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool Wysiwyg
        {
            get { return wysiwyg; }
            set { wysiwyg = value; }
        }

        /// <summary>
        /// Forces justify for the last text line.
        /// </summary>
        [Browsable(false)]
        public bool ForceJustify
        {
            get { return forceJustify; }
            set { forceJustify = value; }
        }

        /// <summary>
        /// Allows handling html tags in the text.
        /// </summary>
        /// <remarks>
        /// The following html tags can be used in the object's text: &lt;b&gt;, &lt;i&gt;, &lt;u&gt;,
        /// &lt;strike&gt;, &lt;sub&gt;, &lt;sup&gt;, &lt;/b&gt;, &lt;/i&gt;, &lt;/u&gt;,
        /// &lt;/strike&gt;, &lt;/sub&gt;, &lt;/sup&gt;,
        /// &lt;font color=&amp;...&amp;&gt;, &lt;/font&gt;. Font size cannot
        /// be changed due to limitations in the rendering engine.
        /// </remarks>
        [DefaultValue(false)]
        [Category("Behavior")]
        [Browsable(false)]
        [Obsolete("This method is deprecated please use TextRenderer")]
        public bool HtmlTags
        {
            get
            {
                return HasHtmlTags;
            }
            set { textRenderType = value ? TextRenderType.HtmlTags : TextRenderType.Default; }
        }

        /// <summary>
        /// Indicates handling html tags in the text.
        /// </summary>
        /// <remarks>To set the value use the TextRenderer property.</remarks>
        [DefaultValue(false)]
        [Category("Behavior")]
        [Browsable(false)]
        public bool HasHtmlTags
        {
            get
            {
                switch (textRenderType)
                {
                    case TextRenderType.HtmlTags:
                    case TextRenderType.HtmlParagraph:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// The type of text render
        /// </summary>
        ///     /// <remarks>
        /// The following html tags can be used in the object's text: &lt;b&gt;, &lt;i&gt;, &lt;u&gt;,
        /// &lt;strike&gt;, &lt;sub&gt;, &lt;sup&gt;, &lt;/b&gt;, &lt;/i&gt;, &lt;/u&gt;,
        /// &lt;/strike&gt;, &lt;/sub&gt;, &lt;/sup&gt;,
        /// &lt;font color=&amp;...&amp;&gt;, &lt;/font&gt;. Font size cannot
        /// be changed due to limitations in the rendering engine.
        /// </remarks>
        [DefaultValue(TextRenderType.Default)]
        [Category("Behavior")]
        public TextRenderType TextRenderType
        {
            get { return textRenderType; }
            set { textRenderType = value; }
        }



        /// <summary>
        /// Gets or sets the paragraph offset, in pixels. For HtmlParagraph use ParagraphFormat.FirstLineIndent.
        /// </summary>
        [DefaultValue(0f)]
        [Category("Appearance")]
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float ParagraphOffset
        {
            get { return paragraphOffset; }
            set { paragraphOffset = value; }
        }

        internal bool IsAdvancedRendererNeeded
        {
            get { return HorzAlign == HorzAlign.Justify || Wysiwyg || LineHeight != 0 || HasHtmlTags; }
        }

        /// <summary>
        /// Cache for inline images
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InlineImageCache InlineImageCache
        {
            get
            {
                if (inlineImageCache == null)
                    inlineImageCache = new InlineImageCache();
                return inlineImageCache;
            }
        }
        #endregion

        #region Private Methods
        private void DrawUnderlines(FRPaintEventArgs e)
        {
            if (!Underlines || Angle != 0)
                return;

            Graphics g = e.Graphics;
            float lineHeight = LineHeight == 0 ? Font.GetHeight() : LineHeight;
            lineHeight *= e.ScaleY;
            float curY = AbsTop * e.ScaleY + lineHeight + 1;
            Pen pen = e.Cache.GetPen(Border.Color, Border.Width * e.ScaleY, DashStyle.Solid);
            while (curY < AbsBottom * e.ScaleY)
            {
                g.DrawLine(pen, AbsLeft * e.ScaleX, curY, AbsRight * e.ScaleY, curY);
                curY += lineHeight;
            }
        }

        private SizeF CalcSize()
        {
            Report report = Report;
            if (String.IsNullOrEmpty(Text) || report == null)
                return new SizeF(0, 0);

            Font font = report.GraphicCache.GetFont(Font.Name, Font.Size * 96f / DrawUtils.ScreenDpi, Font.Style);
            float width = 0;
            if (WordWrap)
            {
                if (Angle == 90 || Angle == 270)
                    width = Height - Padding.Vertical;
                else
                    width = Width - Padding.Horizontal;
            }

            Graphics g = report.MeasureGraphics;
            GraphicsState state = g.Save();
            try
            {
                if (report.TextQuality != TextQuality.Default)
                    g.TextRenderingHint = report.GetTextQuality();
                if (TextRenderType == TextRenderType.HtmlParagraph)
                {
                    if (width == 0)
                        width = 100000;

                    using (HtmlTextRenderer htmlRenderer = GetHtmlTextRenderer(g, new RectangleF(0, 0, width, 100000), 1, 1))
                    {
                        float height = htmlRenderer.CalcHeight();
                        width = htmlRenderer.CalcWidth();

                        width += Padding.Horizontal + 1;
                        if (LineHeight == 0)
                            height += Padding.Vertical + 1;
                        return new SizeF(width, height);
                    }
                }
#if !(NETSTANDARD2_0 || NETSTANDARD2_1)
                if (IsAdvancedRendererNeeded)
#endif
                {
                    if (width == 0)
                        width = 100000;
                    AdvancedTextRenderer renderer = new AdvancedTextRenderer(Text, g, font, Brushes.Black, Pens.Black,
                      new RectangleF(0, 0, width, 100000), GetStringFormat(report.GraphicCache, 0),
                      HorzAlign, VertAlign, LineHeight, Angle, FontWidthRatio, false, Wysiwyg, HasHtmlTags, false, 96f / DrawUtils.ScreenDpi,
                      96f / DrawUtils.ScreenDpi, InlineImageCache);
                    float height = renderer.CalcHeight();
                    width = renderer.CalcWidth();

                    width += Padding.Horizontal + 1;
                    if (LineHeight == 0)
                        height += Padding.Vertical + 1;
                    return new SizeF(width, height);
                }
#if !(NETSTANDARD2_0 || NETSTANDARD2_1)
                else
                {
                    if (FontWidthRatio != 1)
                        width /= FontWidthRatio;
                    SizeF size = g.MeasureString(Text, font, new SizeF(width, 100000));
                    size.Width += Padding.Horizontal + 1;
                    size.Height += Padding.Vertical + 1;
                    return size;
                }
#endif
            }
            finally
            {
                g.Restore(state);
            }
        }

        private float InternalCalcWidth()
        {
            bool saveWordWrap = WordWrap;
            WordWrap = false;
            float result = 0;

            try
            {
                SizeF size = CalcSize();
                result = size.Width;
            }
            finally
            {
                WordWrap = saveWordWrap;
            }

            return result;
        }

        private float InternalCalcHeight()
        {
            return CalcSize().Height;
        }

        private string BreakTextHtml(out bool endOnEnter)
        {
            endOnEnter = false;
            ForceJustify = false;
            if (String.IsNullOrEmpty(Text))
                return "";
            string result = null;
            Report report = Report;
            if (report == null)
                return "";


            StringFormat format = GetStringFormat(report.GraphicCache, StringFormatFlags.LineLimit);
            RectangleF textRect = new RectangleF(0, 0, Width - Padding.Horizontal, Height - Padding.Vertical);

            int charactersFitted;
            Graphics g = report.MeasureGraphics;
            GraphicsState state = g.Save();

            if (report.TextQuality != TextQuality.Default)
                g.TextRenderingHint = report.GetTextQuality();

            try
            {
                using (HtmlTextRenderer htmlRenderer = GetHtmlTextRenderer(g, 1, 1, textRect, format))
                {
                    htmlRenderer.CalcHeight(out charactersFitted);
                    if (charactersFitted == 0)
                        return null;

                    Text = HtmlTextRenderer.BreakHtml(Text, charactersFitted, out result, out endOnEnter);

                    if (HorzAlign == HorzAlign.Justify && !endOnEnter && result != "")
                    {
                        if (Text.EndsWith(" "))
                            Text = Text.TrimEnd(' ');
                        ForceJustify = true;
                    }
                }
            }
            finally
            {
                g.Restore(state);
            }

            return result;
        }

        private string BreakText()
        {
            ForceJustify = false;
            if (String.IsNullOrEmpty(Text))
                return "";

            string result = null;
            Report report = Report;
            if (report == null)
                return "";

            Font font = report.GraphicCache.GetFont(Font.Name, Font.Size * 96f / DrawUtils.ScreenDpi, Font.Style);
            StringFormat format = GetStringFormat(report.GraphicCache, StringFormatFlags.LineLimit);
            RectangleF textRect = new RectangleF(0, 0, Width - Padding.Horizontal, Height - Padding.Vertical);
            if (textRect.Height < 0)
                return null;

            int charactersFitted;
            int linesFilled;

            Graphics g = report.MeasureGraphics;
            GraphicsState state = g.Save();
            try
            {
                if (report.TextQuality != TextQuality.Default)
                    g.TextRenderingHint = report.GetTextQuality();

                AdvancedTextRenderer.StyleDescriptor htmlStyle = null;

                if (IsAdvancedRendererNeeded)
                {
                    AdvancedTextRenderer renderer = new AdvancedTextRenderer(Text, g, font, Brushes.Black, Pens.Black,
                      textRect, format, HorzAlign, VertAlign, LineHeight, Angle, FontWidthRatio, false, Wysiwyg, HasHtmlTags, false, 96f / DrawUtils.ScreenDpi,
                       96f / DrawUtils.ScreenDpi, InlineImageCache);
                    renderer.CalcHeight(out charactersFitted, out htmlStyle);

                    if (charactersFitted == 0)
                        linesFilled = 0;
                    else
                        linesFilled = 2;
                }
                else
                {
                    g.MeasureString(Text, font, textRect.Size, format, out charactersFitted, out linesFilled);
                }


                if (linesFilled == 0)
                    return null;
                if (linesFilled == 1)
                {
                    // check if there is enough space for one line
                    float lineHeight = g.MeasureString("Wg", font).Height;
                    if (textRect.Height < lineHeight)
                        return null;
                }

                if (charactersFitted < Text.Length)
                    result = Text.Substring(charactersFitted);
                else
                    result = "";

                Text = Text.Substring(0, charactersFitted);


                if (HorzAlign == HorzAlign.Justify && !Text.EndsWith("\n") && result != "")
                {
                    if (Text.EndsWith(" "))
                        Text = Text.Substring(0, Text.Length - 1);
                    ForceJustify = true;
                }
                if (HasHtmlTags && htmlStyle != null && result != "")
                    result = htmlStyle.ToString() + result;
            }
            finally
            {
                g.Restore(state);
            }

            return result;
        }

        private void ProcessAutoShrink()
        {
            if (TextRenderType == TextRenderType.HtmlParagraph)
                return;
            if (String.IsNullOrEmpty(Text))
                return;

            if (AutoShrink == AutoShrinkMode.FontSize)
            {
                while (CalcWidth() > Width - 1 && Font.Size > AutoShrinkMinSize && Font.Size>1)
                {
                    Font = new Font(Font.Name, Font.Size - 1, Font.Style);
                }
            }
            else if (AutoShrink == AutoShrinkMode.FontWidth)
            {
                FontWidthRatio = 1;
                float ratio = Converter.DecreasePrecision((Width - 1) / CalcWidth(), 2) - 0.01f;
                if (ratio < 1)
                    FontWidthRatio = Math.Max(ratio, AutoShrinkMinSize);
            }
        }

        private string MakeParagraphOffset(string text)
        {
            // fixed issue 2823
            FirstTabOffset = ParagraphOffset;
            string[] lines = text.Split(new char[] { '\n' });
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].StartsWith("\t"))
                    lines[i] = "\t" + lines[i];
            }
            return String.Join("\n", lines);
        }


        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void DeserializeSubItems(FRReader reader)
        {
            if (String.Compare(reader.ItemName, "Highlight", true) == 0)
                reader.Read(Highlight);
            else
                base.DeserializeSubItems(reader);
        }
        #endregion

        #region Public Methods
        public StringFormat GetStringFormat(GraphicCache cache, StringFormatFlags flags)
        {
            return GetStringFormat(cache, flags, 1);
        }

        internal StringFormat GetStringFormat(GraphicCache cache, StringFormatFlags flags, float scale)
        {
            StringAlignment align = StringAlignment.Near;
            if (HorzAlign == HorzAlign.Center)
                align = StringAlignment.Center;
            else if (HorzAlign == HorzAlign.Right)
                align = StringAlignment.Far;

            StringAlignment lineAlign = StringAlignment.Near;
            if (VertAlign == VertAlign.Center)
                lineAlign = StringAlignment.Center;
            else if (VertAlign == VertAlign.Bottom)
                lineAlign = StringAlignment.Far;

            if (RightToLeft)
                flags |= StringFormatFlags.DirectionRightToLeft;
            if (!WordWrap)
                flags |= StringFormatFlags.NoWrap;
            if (!Clip)
                flags |= StringFormatFlags.NoClip;

            return cache.GetStringFormat(align, lineAlign, Trimming, flags, firstTabOffset * scale, tabWidth * scale);
        }

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            TextObject src = source as TextObject;
            AutoWidth = src.AutoWidth;
            HorzAlign = src.HorzAlign;
            VertAlign = src.VertAlign;
            Angle = src.Angle;
            RightToLeft = src.RightToLeft;
            WordWrap = src.WordWrap;
            Underlines = src.Underlines;
            Font = src.Font;
            TextFill = src.TextFill.Clone();
            TextOutline.Assign(src.TextOutline);
            Trimming = src.Trimming;
            FontWidthRatio = src.FontWidthRatio;
            FirstTabOffset = src.FirstTabOffset;
            TabWidth = src.TabWidth;
            Clip = src.Clip;
            Highlight.Assign(src.Highlight);
            Wysiwyg = src.Wysiwyg;
            LineHeight = src.LineHeight;
            TextRenderType = src.TextRenderType;
            AutoShrink = src.AutoShrink;
            AutoShrinkMinSize = src.AutoShrinkMinSize;
            ParagraphOffset = src.ParagraphOffset;
            inlineImageCache = src.inlineImageCache;
            paragraphFormat.Assign(src.paragraphFormat);
        }

        internal HtmlTextRenderer GetHtmlTextRenderer(Graphics g, float scale, float fontScale)
        {
            StringFormat format = GetStringFormat(Report.GraphicCache, 0, scale);
            RectangleF textRect = new RectangleF(
                (AbsLeft + Padding.Left) * scale,
                (AbsTop + Padding.Top) * scale,
                (Width - Padding.Horizontal) * scale,
                (Height - Padding.Vertical) * scale);
            return GetHtmlTextRenderer(g, scale, fontScale, textRect, format);

        }

        internal HtmlTextRenderer GetHtmlTextRenderer(float scale, float fontScale)
        {
            return GetHtmlTextRenderer(Report.MeasureGraphics, scale, fontScale);
        }

        internal HtmlTextRenderer GetHtmlTextRenderer(Graphics g, RectangleF textRect, float scale, float fontScale)
        {
            StringFormat format = GetStringFormat(Report.GraphicCache, 0, fontScale);
            return GetHtmlTextRenderer(g, scale, fontScale, textRect, format);
        }

        internal HtmlTextRenderer GetHtmlTextRenderer(Graphics g, float scale, float fontScale, RectangleF textRect, StringFormat format)
        {
            return GetHtmlTextRenderer(g, fontScale, scale, fontScale, textRect, format, false);
        }


        internal HtmlTextRenderer GetHtmlTextRenderer(Graphics g, float formatScale, float scale, float fontScale, RectangleF textRect, StringFormat format, bool isPrinting)
        {
            return new HtmlTextRenderer(Text, g, font.Name, font.Size, font.Style, TextColor,
                      textOutline.Color, textRect, Underlines,
                      format, horzAlign, vertAlign, ParagraphFormat.MultipleScale(formatScale), ForceJustify,
                      scale * 96f / DrawUtils.ScreenDpi, fontScale * 96f / DrawUtils.ScreenDpi, InlineImageCache,
                      isPrinting);
        }

        /// <summary>
        /// Draws a text.
        /// </summary>
        /// <param name="e">Paint event data.</param>
        public void DrawText(FRPaintEventArgs e)
        {
            string text = Text;
            if (!String.IsNullOrEmpty(text))
            {
                Graphics g = e.Graphics;
                RectangleF textRect = new RectangleF(
                  (AbsLeft + Padding.Left) * e.ScaleX,
                  (AbsTop + Padding.Top) * e.ScaleY,
                  (Width - Padding.Horizontal) * e.ScaleX,
                  (Height - Padding.Vertical) * e.ScaleY);

                if (ParagraphOffset != 0 && IsDesigning)
                    text = MakeParagraphOffset(text);
                StringFormat format = GetStringFormat(e.Cache, 0, e.ScaleX);

                Font font = e.Cache.GetFont(Font.Name,
                  IsPrinting ? Font.Size : Font.Size * e.ScaleX * 96f / DrawUtils.ScreenDpi,
                  Font.Style);

                Brush textBrush = null;
                if (TextFill is SolidFill)
                    textBrush = e.Cache.GetBrush((TextFill as SolidFill).Color);
                else
                    textBrush = TextFill.CreateBrush(textRect, e.ScaleX, e.ScaleY);

                Pen outlinePen = null;
                if (textOutline.Enabled)
                    outlinePen = e.Cache.GetPen(textOutline.Color, textOutline.Width * e.ScaleX, textOutline.Style);

                Report report = Report;
                if (report != null && report.TextQuality != TextQuality.Default)
                    g.TextRenderingHint = report.GetTextQuality();

                if (textRect.Width > 0 && textRect.Height > 0)
                {
                    switch (TextRenderType)
                    {
                        case TextRenderType.HtmlParagraph:
                            try
                            {
                                using (HtmlTextRenderer htmlRenderer = GetHtmlTextRenderer(e.Graphics, e.ScaleX,
                                    IsPrinting ? 1 : e.ScaleX, IsPrinting ? 1 : e.ScaleX, textRect, format, IsPrinting))
                                {
                                    htmlRenderer.Draw();
                                }
                            }
                            catch
                            {
                                textBrush.Dispose();
                                textBrush = null;
                            }
                            break;
                        default:
                            if (IsAdvancedRendererNeeded)
                            {
                                // use advanced rendering
                                AdvancedTextRenderer advancedRenderer = new AdvancedTextRenderer(text, g, font, textBrush,
                                    outlinePen, textRect, format, HorzAlign, VertAlign, LineHeight * e.ScaleY, Angle,
                                    FontWidthRatio, ForceJustify, Wysiwyg, HasHtmlTags, false,
                                    e.ScaleX * 96f / DrawUtils.ScreenDpi,
                                    IsPrinting ? 1 : e.ScaleX * 96f / DrawUtils.ScreenDpi, InlineImageCache, IsPrinting);
                                advancedRenderer.Draw();
                            }
                            else
                            {
                                // use simple rendering
                                if (Angle == 0 && FontWidthRatio == 1)
                                {
                                    if (outlinePen == null)
                                    {
                                        g.DrawString(text, font, textBrush, textRect, format);
                                    }
                                    else
                                    {
                                        GraphicsPath path = new GraphicsPath();
                                        path.AddString(text, font.FontFamily, Convert.ToInt32(font.Style),
                                            g.DpiY * font.Size / 72, textRect, format);

                                        GraphicsState state = g.Save();
                                        g.SetClip(textRect);
                                        //g.FillPath(textBrush, path);

                                        //regime for text output with drawbehind 
                                        if (TextOutline.DrawBehind)
                                        {
                                            g.DrawPath(outlinePen, path);
                                            g.FillPath(textBrush, path);
                                        }
                                        else //without. default
                                        {
                                            g.FillPath(textBrush, path);
                                            g.DrawPath(outlinePen, path);
                                        }
                                        g.Restore(state);


                                    }
                                }
                                else
                                    StandardTextRenderer.Draw(text, g, font, textBrush, outlinePen, textRect, format, Angle,
                                        FontWidthRatio);

                            }
                            DrawUnderlines(e);
                            break;
                    }

                }
                if (!(TextFill is SolidFill))
                {
                    textBrush.Dispose();
                }
            }
        }


        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            DrawText(e);
            DrawMarkers(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
            DrawDesign(e);
        }

        /// <inheritdoc/>
        public override void ApplyStyle(Style style)
        {
            if (style.ApplyTextFill)
                TextFill = style.TextFill.Clone();
            if (style.ApplyFont)
                Font = style.Font;
            base.ApplyStyle(style);
        }

        /// <inheritdoc/>
        public override void SaveStyle()
        {
            base.SaveStyle();
            savedTextFill = TextFill;
            savedFont = Font;
        }

        /// <inheritdoc/>
        public override void RestoreStyle()
        {
            base.RestoreStyle();
            TextFill = savedTextFill;
            Font = savedFont;
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            if (writer.SerializeTo == SerializeTo.Preview && AutoWidth)
            {
                WordWrap = false;
                float width = CalcSize().Width;
                if ((Anchor & AnchorStyles.Right) != 0)
                    Left = Right - width;
                Width = width;
            }
            TextObject c = writer.DiffObject as TextObject;
            base.Serialize(writer);

            if (c == null)
                return; // RichObject here

            if (AutoWidth != c.AutoWidth)
                writer.WriteBool("AutoWidth", AutoWidth);
            if (AutoShrink != c.AutoShrink)
                writer.WriteValue("AutoShrink", AutoShrink);
            if (FloatDiff(AutoShrinkMinSize, c.AutoShrinkMinSize))
                writer.WriteFloat("AutoShrinkMinSize", AutoShrinkMinSize);
            if (HorzAlign != c.HorzAlign)
                writer.WriteValue("HorzAlign", HorzAlign);
            if (VertAlign != c.VertAlign)
                writer.WriteValue("VertAlign", VertAlign);
            if (Angle != c.Angle)
                writer.WriteInt("Angle", Angle);
            if (RightToLeft != c.RightToLeft)
                writer.WriteBool("RightToLeft", RightToLeft);
            if (WordWrap != c.WordWrap)
                writer.WriteBool("WordWrap", WordWrap);
            if (Underlines != c.Underlines)
                writer.WriteBool("Underlines", Underlines);
            if ((writer.SerializeTo != SerializeTo.Preview || !Font.Equals(c.Font)) && writer.ItemName != "inherited")
                writer.WriteValue("Font", Font);
            TextFill.Serialize(writer, "TextFill", c.TextFill);
            if (TextOutline != null)
                TextOutline.Serialize(writer, "TextOutline", c.TextOutline);
            if (Trimming != c.Trimming)
                writer.WriteValue("Trimming", Trimming);
            if (FontWidthRatio != c.FontWidthRatio)
                writer.WriteFloat("FontWidthRatio", FontWidthRatio);
            if (FirstTabOffset != c.FirstTabOffset)
                writer.WriteFloat("FirstTabOffset", FirstTabOffset);
            if (TabWidth != c.TabWidth)
                writer.WriteFloat("TabWidth", TabWidth);
            if (Clip != c.Clip)
                writer.WriteBool("Clip", Clip);
            if (Wysiwyg != c.Wysiwyg)
                writer.WriteBool("Wysiwyg", Wysiwyg);
            if (LineHeight != c.LineHeight)
                writer.WriteFloat("LineHeight", LineHeight);
            if (TextRenderType != c.TextRenderType)
                writer.WriteValue("TextRenderType", TextRenderType);
            if (ParagraphOffset != c.ParagraphOffset)
                writer.WriteFloat("ParagraphOffset", ParagraphOffset);
            if (ForceJustify != c.ForceJustify)
                writer.WriteBool("ForceJustify", ForceJustify);
            if (writer.SerializeTo != SerializeTo.Preview)
            {
                if (Style != c.Style)
                    writer.WriteStr("Style", Style);
                if (Highlight.Count > 0)
                    writer.Write(Highlight);
            }

            if (ParagraphFormat.FirstLineIndent > 0)
                writer.WriteFloat("ParagraphFormat.FirstLineIndent", ParagraphFormat.FirstLineIndent);
            if (ParagraphFormat.LineSpacing > 0)
                writer.WriteFloat("ParagraphFormat.LineSpacing", ParagraphFormat.LineSpacing);
            if (ParagraphFormat.LineSpacingType != LineSpacingType.Single)
                writer.WriteValue("ParagraphFormat.LineSpacingType", ParagraphFormat.LineSpacingType);
            if (ParagraphFormat.SkipFirstLineIndent)
                writer.WriteBool("ParagraphFormat.SkipFirstLineIndent", ParagraphFormat.SkipFirstLineIndent);

            StringBuilder sb = null;

            if (InlineImageCache != null && writer.BlobStore != null && HasHtmlTags == true)
                foreach (InlineImageCache.CacheItem item in InlineImageCache.AllItems())
                {

                    if (item.Src.StartsWith("data:")) continue;
                    if (sb == null)
                        sb = new StringBuilder();
                    sb.Append(writer.BlobStore.AddOrUpdate(item.Stream, item.Src))
                      .Append(',');
                }
            if (sb != null)
            {
                sb.Length--;
                writer.WriteStr("InlineImageCacheIndexes", sb.ToString());
            }
        }


        /// <inheritdoc/>
        public override void Deserialize(FRReader reader)
        {
            base.Deserialize(reader);
            TextFill.Deserialize(reader, "TextFill");
            if (reader.BlobStore != null)
            {
                string indexes = reader.ReadStr("InlineImageCacheIndexes");
                if (indexes != "null" && !String.IsNullOrEmpty(indexes))
                {
                    string[] arr = indexes.Split(',');
                    foreach (string index in arr)
                    {
                        int val = 0;
                        if (Int32.TryParse(index, out val))
                        {
                            if (val >= 0 && val < reader.BlobStore.Count)
                            {
                                InlineImageCache.CacheItem it = new InlineImageCache.CacheItem();
                                it.Set(reader.BlobStore.Get(val));
                                InlineImageCache.Set(reader.BlobStore.GetSource(val), it);
                            }
                        }
                    }
                }
            }
            switch (reader.DeserializeFrom)
            {
                case SerializeTo.Undo:
                case SerializeTo.Preview:
                case SerializeTo.Clipboard:
                    // skip
                    break;
                default:
                    if (!reader.HasProperty("Font") && reader.ItemName != "inherited")
                    {
                        string creatorVersion = reader.Root.GetProp("ReportInfo.CreatorVersion");
                        if (!String.IsNullOrEmpty(creatorVersion))
                        {
                            try
                            {
                                string[] versions = creatorVersion.Split('.');
                                int major = 0;
                                if (Int32.TryParse(versions[0], out major))
                                {
                                    if (major < 2016)
                                    {
                                        Font = new Font("Arial", 10);
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    break;
            }
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();
            TextFill.InitializeComponent();
        }

        public override void FinalizeComponent()
        {
            base.FinalizeComponent();
            TextFill.FinalizeComponent();
        }

        internal void ApplyCondition(HighlightCondition c)
        {
            if (c.ApplyBorder)
                Border = c.Border.Clone();
            if (c.ApplyFill)
                Fill = c.Fill.Clone();
            if (c.ApplyTextFill)
                TextFill = c.TextFill.Clone();
            if (c.ApplyFont)
                Font = c.Font;

            Visible = c.Visible;
        }

        #endregion

        #region Report Engine
        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            List<string> expressions = new List<string>();
            expressions.AddRange(base.GetExpressions());

            if (AllowExpressions && !String.IsNullOrEmpty(Brackets))
            {
                string[] brackets = Brackets.Split(new char[] { ',' });
                // collect expressions found in the text
                expressions.AddRange(CodeUtils.GetExpressions(Text, brackets[0], brackets[1]));
            }

            // add highlight conditions
            foreach (HighlightCondition condition in Highlight)
            {
                expressions.Add(condition.Expression);
            }

            return expressions.ToArray();
        }

        /// <inheritdoc/>
        public override void SaveState()
        {
            base.SaveState();
            savedText = Text;
            savedTextFill = TextFill;
            savedFont = Font;
            savedFormat = Format;

        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            base.RestoreState();
            Text = savedText;
            TextFill = savedTextFill;
            Font = savedFont;
            Format = savedFormat;
        }

        /// <summary>
        /// Calculates the object's width.
        /// </summary>
        /// <returns>The width, in pixels.</returns>
        public float CalcWidth()
        {
            if (Angle == 90 || Angle == 270)
                return InternalCalcHeight();
            return InternalCalcWidth();
        }

        /// <inheritdoc/>
        public override float CalcHeight()
        {
            if (Angle == 90 || Angle == 270)
                return InternalCalcWidth();
            return InternalCalcHeight();
        }

        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();

            // process expressions
            if (AllowExpressions)
            {
                if (!String.IsNullOrEmpty(Brackets))
                {
                    string[] brackets = Brackets.Split(new char[] { ',' });
                    FindTextArgs args = new FindTextArgs();
                    args.Text = new FastString(Text);
                    args.OpenBracket = brackets[0];
                    args.CloseBracket = brackets[1];
                    int expressionIndex = 0;
                    while (args.StartIndex < args.Text.Length)
                    {
                        string expression = CodeUtils.GetExpression(args, false);
                        if (expression == "")
                            break;

                        string formattedValue = CalcAndFormatExpression(expression, expressionIndex);

                        args.Text.Remove(args.StartIndex, args.EndIndex - args.StartIndex);
                        args.Text.Insert(args.StartIndex, formattedValue);

                        args.StartIndex += formattedValue.Length;
                        expressionIndex++;
                    }
                    Text = args.Text.ToString();
                }
            }

            // process highlight
            Variant varValue = new Variant(Value);
            foreach (HighlightCondition condition in Highlight)
            {
                try
                {
                    object val = Report.Calc(condition.Expression, varValue);
                    if (val != null && (bool)val == true)
                    {
                        ApplyCondition(condition);
                        break;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(Name + ": " + Res.Get("Messages,ErrorInHighlightCondition") + ": " + condition.Expression, e.InnerException);
                }
            }

            // make paragraph offset
            if (ParagraphOffset != 0)
                Text = MakeParagraphOffset(Text);
            // process AutoShrink
            ProcessAutoShrink();
        }

        /// <inheritdoc/>
        public override bool Break(BreakableComponent breakTo)
        {
            switch (TextRenderType)
            {
                case TextRenderType.HtmlParagraph:
                    bool endOnEnter;
                    string breakTextHtml = BreakTextHtml(out endOnEnter);
                    if (breakTextHtml != null && breakTo != null)
                    {
                        (breakTo as TextObject).Text = breakTextHtml;
                        if (!endOnEnter)
                            (breakTo as TextObject).ParagraphFormat.SkipFirstLineIndent = true;
                    }
                    return breakTextHtml != null;
                default:
                    string breakText = BreakText();
                    if (breakText != null && breakTo != null)
                        (breakTo as TextObject).Text = breakText;
                    return breakText != null;
            }
        }

        internal IEnumerable<PictureObject> GetPictureFromHtmlText(AdvancedTextRenderer renderer)
        {
            if (renderer == null)
            {
                using (Bitmap b = new Bitmap(1, 1))
                using (Graphics g = Graphics.FromImage(b))
                {
                    RectangleF textRect = new RectangleF(
                      (AbsLeft + Padding.Left),
                      (AbsTop + Padding.Top),
                      (Width - Padding.Horizontal),
                      (Height - Padding.Vertical));
                    StringFormat format = GetStringFormat(Report.GraphicCache, StringFormatFlags.LineLimit);
                    renderer = new AdvancedTextRenderer(Text, g, Font, Brushes.Black, Pens.Black,
                        textRect, format, HorzAlign, VertAlign, LineHeight, Angle, FontWidthRatio,
                        ForceJustify, Wysiwyg, HasHtmlTags, false, 1, 1,
                        InlineImageCache);
                    foreach (PictureObject obj in GetPictureFromHtmlText(renderer))
                        yield return obj;
                }
            }
            else
            {
                RectangleF textRect = renderer.DisplayRect;
                foreach (AdvancedTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                    foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                        foreach (AdvancedTextRenderer.Word word in line.Words)
                            foreach (AdvancedTextRenderer.Run run in word.Runs)
                                if (run is AdvancedTextRenderer.RunImage)
                                {
                                    AdvancedTextRenderer.RunImage runImage = run as AdvancedTextRenderer.RunImage;
                                    PictureObject obj = new PictureObject();

                                    float left = runImage.Left - textRect.Left;
                                    float top = runImage.Top - textRect.Top;
                                    float width =
                                      runImage.Left + runImage.Width > textRect.Right ?
                                      textRect.Right - (left < 0 ? textRect.Left : runImage.Left) :
                                      (
                                        runImage.Left < textRect.Left ?
                                        runImage.Left + runImage.Width - textRect.Left :
                                        runImage.Width
                                      );
                                    float height =
                                      runImage.Top + runImage.Height > textRect.Bottom ?
                                      textRect.Bottom - (top < 0 ? textRect.Top : runImage.Top) :
                                      (
                                        runImage.Top < textRect.Top ?
                                        runImage.Top + runImage.Height - textRect.Top :
                                        runImage.Height
                                      );


                                    if (left < 0 || top < 0 || width < runImage.Width || height < runImage.Height)
                                    {
                                        Bitmap bmp = new Bitmap((int)width, (int)height);
                                        using (Graphics g = Graphics.FromImage(bmp))
                                        {
                                            g.DrawImage(runImage.Image, new PointF(
                                              left < 0 ? left : 0,
                                              top < 0 ? top : 0
                                              ));
                                        }
                                        obj.Image = bmp;
                                        obj.Left = (left < 0 ? textRect.Left : runImage.Left) / renderer.Scale;
                                        obj.Top = (top < 0 ? textRect.Top : runImage.Top) / renderer.Scale;
                                        obj.Width = width / renderer.Scale;
                                        obj.Height = height / renderer.Scale;
                                        obj.SizeMode = PictureBoxSizeMode.StretchImage;
                                    }
                                    else
                                    {
                                        obj.Image = runImage.Image;
                                        obj.Left = runImage.Left / renderer.Scale;
                                        obj.Top = runImage.Top / renderer.Scale;
                                        obj.Width = runImage.Width / renderer.Scale;
                                        obj.Height = runImage.Height / renderer.Scale;
                                        obj.SizeMode = PictureBoxSizeMode.StretchImage;
                                    }
                                    yield return obj;
                                }
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TextObject"/> class with default settings.
        /// </summary>
        public TextObject()
        {
            paragraphFormat = new ParagraphFormat();
            wordWrap = true;
            font = DrawUtils.DefaultReportFont;
            textFill = new SolidFill(Color.Black);
            textOutline = new TextOutline();
            trimming = StringTrimming.None;
            fontWidthRatio = 1;
            tabWidth = 58;
            clip = true;
            highlight = new ConditionCollection();
            FlagSerializeStyle = false;
            SetFlags(Flags.HasSmartTag, true);
        }
    }
}