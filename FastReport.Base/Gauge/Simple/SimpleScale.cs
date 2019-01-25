using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport.Gauge.Simple
{
    /// <summary>
    /// Represents a simple scale.
    /// </summary>
    public class SimpleScale : GaugeScale
    {
        #region Fields

        private float left;
        private float top;
        private float height;
        private float width;
        private int majorTicksNum;
        private SimpleSubScale firstSubScale;
        private SimpleSubScale secondSubScale;
        private float pointerWidthOffset;
        private float pointerHeightOffset;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the first subscale (top or left).
        /// </summary>
        [Browsable(true)]
        public virtual SimpleSubScale FirstSubScale
        {
            get { return firstSubScale; }
            set { firstSubScale = value; }
        }

        /// <summary>
        /// Gets or sets the second subscale (right or bottom).
        /// </summary>
        [Browsable(true)]
        public virtual SimpleSubScale SecondSubScale
        {
            get { return secondSubScale; }
            set { secondSubScale = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleScale"/> class.
        /// </summary>
        /// <param name="parent">The parent gauge object.</param>
        public SimpleScale(GaugeObject parent) : base(parent)
        {
            MajorTicks = new ScaleTicks(10, 2, Color.Black);
            MinorTicks = new ScaleTicks(6, 1, Color.Black);
            majorTicksNum = 6;
            firstSubScale = new SimpleSubScale();
            secondSubScale = new SimpleSubScale();
        }

        #endregion // Constructors

        #region Private Methods

        private void DrawMajorTicksHorz(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(MajorTicks.Color, MajorTicks.Width * e.ScaleX, DashStyle.Solid);
            Brush brush = TextFill.CreateBrush(new RectangleF(Parent.AbsLeft, Parent.AbsTop, Parent.Width, Parent.Height), e.ScaleX, e.ScaleY);
            pointerHeightOffset = (Parent.Pointer as SimplePointer).Height / 2 + Parent.Pointer.BorderWidth * 2 * e.ScaleY;
            float x = left;
            float y1 = top;
            float y2 = top + height / 2 - pointerHeightOffset;
            float y3 = top + height / 2 + pointerHeightOffset;
            float y4 = top + height;
            float step = width / (majorTicksNum - 1);
            int textStep = (int)((Parent.Maximum - Parent.Minimum) / (majorTicksNum - 1));
            Font font = e.Cache.GetFont(Font.Name, Parent.IsPrinting ? Font.Size : Font.Size * e.ScaleX * 96f / DrawUtils.ScreenDpi, Font.Style);
            string text = Parent.Minimum.ToString();
            if (firstSubScale.Enabled)
            {
                for (int i = 0; i < majorTicksNum; i++)
                {
                    g.DrawLine(pen, x, y1, x, y2);
                    if (firstSubScale.ShowCaption)
                    {
                        SizeF strSize = g.MeasureString(text, Font);
                        g.DrawString(text, font, brush, x - strSize.Width / 2 * e.ScaleX, y1 - 0.4f * Units.Centimeters * e.ScaleY);
                        text = Convert.ToString(textStep * (i + 1) + Parent.Minimum);
                    }
                    x += step;
                }
            }
            x = left;
            text = Parent.Minimum.ToString();
            if (secondSubScale.Enabled)
            {
                for (int i = 0; i < majorTicksNum; i++)
                {
                    g.DrawLine(pen, x, y3, x, y4);
                    if (secondSubScale.ShowCaption)
                    {
                        SizeF strSize = g.MeasureString(text, Font);

                        g.DrawString(text, font, brush, x - strSize.Width / 2 * e.ScaleX, y4 + 0.08f * Units.Centimeters * e.ScaleY);
                        text = Convert.ToString(textStep * (i + 1) + Parent.Minimum);
                    }
                    x += step;
                }
            }
            brush.Dispose();
        }

        private void DrawMinorTicksHorz(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(MinorTicks.Color, MinorTicks.Width * e.ScaleX, DashStyle.Solid);
            pointerHeightOffset = (Parent.Pointer as SimplePointer).Height / 2 + Parent.Pointer.BorderWidth * 2 * e.ScaleY;
            float x = left;
            float y1 = top + height * 0.15f;
            float y2 = top + height / 2 - pointerHeightOffset;
            float y3 = top + height / 2 + pointerHeightOffset;
            float y4 = top + height - height * 0.15f;
            float step = width / (majorTicksNum - 1) / 4;
            if (firstSubScale.Enabled)
            {
                for (int i = 0; i < majorTicksNum - 1; i++)
                {
                    x += step;
                    for (int j = 0; j < 3; j++)
                    {
                        g.DrawLine(pen, x, y1, x, y2);
                        x += step;
                    }
                }
            }
            x = left;
            if (secondSubScale.Enabled)
            {
                for (int i = 0; i < majorTicksNum - 1; i++)
                {
                    x += step;
                    for (int j = 0; j < 3; j++)
                    {
                        g.DrawLine(pen, x, y3, x, y4);
                        x += step;
                    }
                }
            }
        }

        private void DrawMajorTicksVert(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(MajorTicks.Color, MajorTicks.Width * e.ScaleY, DashStyle.Solid);
            Brush brush = TextFill.CreateBrush(new RectangleF(Parent.AbsLeft * e.ScaleX, Parent.AbsTop * e.ScaleY,
    Parent.Width * e.ScaleX, Parent.Height * e.ScaleY), e.ScaleX, e.ScaleY);
            pointerWidthOffset = (Parent.Pointer as SimplePointer).Width / 2 + Parent.Pointer.BorderWidth * 2 * e.ScaleX;
            float y = top + height;
            float x1 = left;
            float x2 = left + width / 2 - pointerWidthOffset;
            float x3 = left + width / 2 + pointerWidthOffset;
            float x4 = left + width;
            float step = height / (majorTicksNum - 1);
            int textStep = (int)((Parent.Maximum - Parent.Minimum) / (majorTicksNum - 1));
            Font font = e.Cache.GetFont(Font.Name, Parent.IsPrinting ? Font.Size : Font.Size * e.ScaleX * 96f / DrawUtils.ScreenDpi, Font.Style);
            string text = Parent.Minimum.ToString();
            if (firstSubScale.Enabled)
            {
                for (int i = 0; i < majorTicksNum; i++)
                {
                    g.DrawLine(pen, x1, y, x2, y);
                    if (firstSubScale.ShowCaption)
                    {
                        SizeF strSize = g.MeasureString(text, Font);
                        g.DrawString(text, font, brush, x1 - strSize.Width * e.ScaleX - 0.04f * Units.Centimeters * e.ScaleX, y - strSize.Height / 2 * e.ScaleY);
                        text = Convert.ToString(textStep * (i + 1) + Parent.Minimum);
                    }
                    y -= step;
                }
            }
            y = top + height;
            text = Parent.Minimum.ToString();
            if (secondSubScale.Enabled)
            {
                for (int i = 0; i < majorTicksNum; i++)
                {
                    g.DrawLine(pen, x3, y, x4, y);
                    if (secondSubScale.ShowCaption)
                    {
                        SizeF strSize = g.MeasureString(text, Font);

                        g.DrawString(text, font, brush, x4 + 0.04f * Units.Centimeters * e.ScaleX, y - strSize.Height / 2 * e.ScaleY);
                        text = Convert.ToString(textStep * (i + 1) + Parent.Minimum);
                    }
                    y -= step;
                }
            }
            brush.Dispose();
        }

        private void DrawMinorTicksVert(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(MinorTicks.Color, MinorTicks.Width * e.ScaleY, DashStyle.Solid);
            pointerWidthOffset = (Parent.Pointer as SimplePointer).Width / 2 + Parent.Pointer.BorderWidth * 2 * e.ScaleX;
            float y = top + height;
            float x1 = left + width * 0.15f;
            float x2 = left + width / 2 - pointerWidthOffset;
            float x3 = left + width / 2 + pointerWidthOffset;
            float x4 = left + width - width * 0.15f;
            float step = height / (majorTicksNum - 1) / 4;
            if (firstSubScale.Enabled)
            {
                for (int i = 0; i < majorTicksNum - 1; i++)
                {
                    y -= step;
                    for (int j = 0; j < 3; j++)
                    {
                        g.DrawLine(pen, x1, y, x2, y);
                        y -= step;
                    }
                }
            }
            y = top + height;
            if (secondSubScale.Enabled)
            {
                for (int i = 0; i < majorTicksNum - 1; i++)
                {
                    y -= step;
                    for (int j = 0; j < 3; j++)
                    {
                        g.DrawLine(pen, x3, y, x4, y);
                        y -= step;
                    }
                }
            }
        }

        #endregion // Private Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void Assign(GaugeScale src)
        {
            base.Assign(src);

            SimpleScale s = src as SimpleScale;
            MajorTicks.Assign(s.MajorTicks);
            MinorTicks.Assign(s.MinorTicks);
            FirstSubScale.Assign(s.FirstSubScale);
            SecondSubScale.Assign(s.SecondSubScale);
        }

        /// <inheritdoc/>
        public override void Draw(FastReport.Utils.FRPaintEventArgs e)
        {
            base.Draw(e);

            if (Parent.Vertical)
            {
                left = (Parent.AbsLeft + 0.7f * Units.Centimeters) * e.ScaleX;
                top = (Parent.AbsTop + 0.5f * Units.Centimeters) * e.ScaleY;
                height = (Parent.Height - 1.0f * Units.Centimeters) * e.ScaleY;
                width = (Parent.Width - 1.4f * Units.Centimeters) * e.ScaleX;

                DrawMajorTicksVert(e);
                DrawMinorTicksVert(e);
            }
            else
            {
                left = (Parent.AbsLeft + 0.5f * Units.Centimeters) * e.ScaleX;
                top = (Parent.AbsTop + 0.6f * Units.Centimeters) * e.ScaleY;
                height = (Parent.Height - 1.2f * Units.Centimeters) * e.ScaleY;
                width = (Parent.Width - 1.0f * Units.Centimeters) * e.ScaleX;

                DrawMajorTicksHorz(e);
                DrawMinorTicksHorz(e);
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer, string prefix, GaugeScale diff)
        {
            base.Serialize(writer, prefix, diff);

            SimpleScale dc = diff as SimpleScale;
            MajorTicks.Serialize(writer, prefix + ".MajorTicks", dc.MajorTicks);
            MinorTicks.Serialize(writer, prefix + ".MinorTicks", dc.MinorTicks);
            FirstSubScale.Serialize(writer, prefix + ".FirstSubScale", dc.FirstSubScale);
            SecondSubScale.Serialize(writer, prefix + ".SecondSubScale", dc.SecondSubScale);
        }

        #endregion // Public Methods
    }

    /// <summary>
    /// Represent the subscale of simple scale.
    /// </summary>
    [ToolboxItem(false)]
    public class SimpleSubScale : Component
    {
        #region Fields

        private bool enabled;
        private bool showCaption;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value that specifies enabled subscale or not.
        /// </summary>
        [Browsable(true)]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies show caption or not.
        /// </summary>
        [Browsable(true)]
        public bool ShowCaption
        {
            get { return showCaption; }
            set { showCaption = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSubScale"/> class.
        /// </summary>
        public SimpleSubScale()
        {
            enabled = true;
            showCaption = true;
        }

        #endregion // Constructors

        #region Public Methods

        /// <summary>
        /// Copies the contents of another SimpleSubScale.
        /// </summary>
        /// <param name="src">The SimpleSubScale instance to copy the contents from.</param>
        public virtual void Assign(SimpleSubScale src)
        {
            Enabled = src.Enabled;
            ShowCaption = src.ShowCaption;
        }

        /// <summary>
        /// Serializes the SimpleSubScale.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        /// <param name="prefix">SimpleSubScale property name.</param>
        /// <param name="diff">Another SimpleSubScale to compare with.</param>
        /// <remarks>
        /// This method is for internal use only.
        /// </remarks>
        public virtual void Serialize(FRWriter writer, string prefix, SimpleSubScale diff)
        {
            if (Enabled != diff.Enabled)
            {
                writer.WriteBool(prefix + ".Enabled", Enabled);
            }
            if (ShowCaption != diff.ShowCaption)
            {
                writer.WriteBool(prefix + ".ShowCaption", ShowCaption);
            }
        }

        #endregion // Public Methods
    }
}
