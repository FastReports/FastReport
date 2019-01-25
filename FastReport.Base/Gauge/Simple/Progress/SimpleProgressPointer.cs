using FastReport.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FastReport.Gauge.Simple.Progress
{
    /// <summary>
    /// SimpleProgressGauge pointer types
    /// </summary>
    public enum SimpleProgressPointerType
    {
        /// <summary>
        /// Full sized pointer
        /// </summary>
        Full,

        /// <summary>
        /// Small pointer
        /// </summary>
        Small
    }

    /// <inheritdoc />
    public class SimpleProgressPointer : SimplePointer
    {
        private SimpleProgressPointerType type;
        private float smallPointerWidthRatio;

        /// <summary>
        /// Gets or sets the pointer type
        /// </summary>
        public SimpleProgressPointerType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the small pointer width ratio
        /// </summary>
        public float SmallPointerWidthRatio
        {
            get { return smallPointerWidthRatio; }
            set
            {
                if (value > 1)
                    smallPointerWidthRatio = 1;
                else if (value < 0)
                    smallPointerWidthRatio = 0;
                else
                    smallPointerWidthRatio = value;
            }
        }
        /// <inheritdoc />
        public SimpleProgressPointer(GaugeObject parent) : base(parent)
        {
            smallPointerWidthRatio = 0.1f;
        }

        internal override void DrawHorz(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(BorderColor, BorderWidth * e.ScaleX, DashStyle.Solid);

            Left = (Parent.AbsLeft + Parent.Border.Width / 2 + HorizontalOffset) * e.ScaleX;
            Top = (Parent.AbsTop + Parent.Border.Width / 2 + (Parent.Height - Parent.Border.Width) / 2 - (Parent.Height - Parent.Border.Width) * PointerRatio / 2) * e.ScaleY;
            Height = ((Parent.Height - Parent.Border.Width) * PointerRatio) * e.ScaleY;
            Width = (float)((Parent.Width - Parent.Border.Width - HorizontalOffset * 2) * (Parent.Value - Parent.Minimum) / (Parent.Maximum - Parent.Minimum) * e.ScaleX);

            if (type == SimpleProgressPointerType.Small)
            {
                float prntWidth = (Parent.Width - Parent.Border.Width + HorizontalOffset) * e.ScaleX;
                float widthSml = (Parent.Width - Parent.Border.Width - HorizontalOffset * 2) * smallPointerWidthRatio * e.ScaleX;
                float leftSml = Left + Width - prntWidth * smallPointerWidthRatio + widthSml / 2;

                if (leftSml >= Left && leftSml + widthSml < Left + prntWidth)
                    Left = leftSml;
                else if (leftSml + widthSml >= Left + prntWidth)
                    Left += prntWidth - widthSml;
                Width = widthSml;
            }
            Brush brush = Fill.CreateBrush(new RectangleF(Left, Top, Width, Height), e.ScaleX, e.ScaleY);
            g.FillRectangle(brush, Left, Top, Width, Height);
            g.DrawRectangle(pen, Left, Top, Width, Height);
        }

        internal override void DrawVert(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(BorderColor, BorderWidth * e.ScaleY, DashStyle.Solid);

            Width = ((Parent.Width - Parent.Border.Width) * PointerRatio) * e.ScaleX;
            Height = (float)((Parent.Height - Parent.Border.Width - HorizontalOffset * 2) * (Parent.Value - Parent.Minimum) / (Parent.Maximum - Parent.Minimum) * e.ScaleY);
            Left = (Parent.AbsLeft + Parent.Border.Width / 2 + (Parent.Width - Parent.Border.Width) / 2 - (Parent.Width - Parent.Border.Width) * PointerRatio / 2) * e.ScaleX;
            Top = (Parent.AbsTop + Parent.Border.Width / 2 + Parent.Height - Parent.Border.Width) * e.ScaleY - Height;

            if (type == SimpleProgressPointerType.Small)
            {
                float prntTop = (Parent.AbsTop + Parent.Border.Width / 2) * e.ScaleY;
                float prntHeight = (Parent.Height - Parent.Border.Width) * e.ScaleY;
                float heightSml = (Parent.Height - Parent.Border.Width) * smallPointerWidthRatio * e.ScaleY;
                float topSml = Top - heightSml / 2;

                if (topSml + heightSml > prntTop + prntHeight)
                    Top = prntTop + prntHeight - heightSml;
                else if (topSml < prntTop)
                    Top = prntTop;
                else
                    Top = topSml;
                Height = heightSml;
            }
            Brush brush = Fill.CreateBrush(new RectangleF(Left, Top, Width, Height), e.ScaleX, e.ScaleY);
            g.FillRectangle(brush, Left, Top, Width, Height);
            g.DrawRectangle(pen, Left, Top, Width, Height);
        }

        /// <inheritdoc />
        public override void Assign(GaugePointer src)
        {
            base.Assign(src);
            SimpleProgressPointer s = src as SimpleProgressPointer;
            Type = s.Type;
            SmallPointerWidthRatio = s.SmallPointerWidthRatio;
        }

        /// <inheritdoc />
        public override void Serialize(FRWriter writer, string prefix, GaugePointer diff)
        {
            base.Serialize(writer, prefix, diff);

            SimpleProgressPointer dc = diff as SimpleProgressPointer;
            if (Type != dc.Type)
            {
                writer.WriteValue(prefix + ".Type", Type);
            }
            if (SmallPointerWidthRatio != dc.SmallPointerWidthRatio)
            {
                writer.WriteValue(prefix + ".SmallPointerWidthRatio", SmallPointerWidthRatio);
            }
        }
    }
}
