using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Code;

namespace FastReport
{

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
    public partial class HtmlObject : TextObjectBase
    {
        #region Fields
        private bool rightToLeft;
        private string savedText;
        #endregion

        #region Properties

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
        #endregion

        #region Private Methods

        private float InternalCalcWidth()
        {
            return this.Width;
        }

        private float InternalCalcHeight()
        {
            return this.Height;
        }

        private string BreakText()
        {
            return null;
        }

        #endregion

        #region Public Methods
        internal StringFormat GetStringFormat(GraphicCache cache, StringFormatFlags flags)
        {
            return GetStringFormat(cache, flags, 1);
        }

        internal StringFormat GetStringFormat(GraphicCache cache, StringFormatFlags flags, float scale)
        {
            if (RightToLeft)
                flags |= StringFormatFlags.DirectionRightToLeft;

            return cache.GetStringFormat(StringAlignment.Near, StringAlignment.Near, StringTrimming.None, flags, 0 * scale, 0 * scale);
        }

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            HtmlObject src = source as HtmlObject;
            RightToLeft = src.RightToLeft;
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

                StringFormat format = GetStringFormat(e.Cache, 0, e.ScaleX);

                Font font = DrawUtils.DefaultTextObjectFont;

                Brush textBrush = e.Cache.GetBrush(Color.Black);

                Report report = Report;
                if (report != null)
                    g.TextRenderingHint = report.GetTextQuality();

                if (textRect.Width > 0 && textRect.Height > 0)
                {
                        // use simple rendering
                        g.DrawString(text, font, textBrush, textRect, format);
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
            base.ApplyStyle(style);
        }

        /// <inheritdoc/>
        public override void SaveStyle()
        {
            base.SaveStyle();
        }

        /// <inheritdoc/>
        public override void RestoreStyle()
        {
            base.RestoreStyle();
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            HtmlObject c = writer.DiffObject as HtmlObject;
            base.Serialize(writer);

            if (writer.SerializeTo != SerializeTo.Preview)
            {
                if (Style != c.Style)
                    writer.WriteStr("Style", Style);
            }
        }

        internal void ApplyCondition(HighlightCondition c)
        {
            if (c.ApplyBorder)
                Border = c.Border.Clone();
            if (c.ApplyFill)
                Fill = c.Fill.Clone();
            if (!c.Visible)
                Visible = false;
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

            return expressions.ToArray();
        }

        /// <inheritdoc/>
        public override void SaveState()
        {
            base.SaveState();
            savedText = Text;
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            base.RestoreState();
            Text = savedText;
        }

        /// <summary>
        /// Calculates the object's width.
        /// </summary>
        /// <returns>The width, in pixels.</returns>
        public float CalcWidth()
        {
            return InternalCalcWidth();
        }

        /// <inheritdoc/>
        public override float CalcHeight()
        {
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
                        args.Text = args.Text.Remove(args.StartIndex, args.EndIndex - args.StartIndex);
                        args.Text = args.Text.Insert(args.StartIndex, formattedValue);
                        args.StartIndex += formattedValue.Length;
                        expressionIndex++;
                    }
                    Text = args.Text.ToString();
                }
            }

            // process highlight
            Variant varValue = new Variant(Value);
        }

        /// <inheritdoc/>
        public override bool Break(BreakableComponent breakTo)
        {
            string breakText = BreakText();
            if (breakText != null && breakTo != null)
                (breakTo as TextObject).Text = breakText;
            return breakText != null;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlObject"/> class with default settings.
        /// </summary>
        public HtmlObject()
        {
            FlagSerializeStyle = false;
            SetFlags(Flags.HasSmartTag, true);
        }
    }
}