using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport.Gauge.Linear
{
    /// <summary>
    /// Represents a linear pointer.
    /// </summary>
    public class LinearPointer : GaugePointer
    {
        #region Fields

        private float left;
        private float top;
        private float height;
        private float width;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets o sets the height of gauge pointer.
        /// </summary>
        [Browsable(false)]
        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Gets or sets the width of a pointer.
        /// </summary>
        [Browsable(false)]
        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearPointer"/>
        /// </summary>
        /// <param name="parent">The parent gauge object.</param>
        public LinearPointer(GaugeObject parent) : base(parent)
        {
            height = 4.0f;
            width = 8.0f;
        }

        #endregion // Constructors

        #region Private Methods

        private void DrawHorz(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(BorderColor, BorderWidth * e.ScaleX, DashStyle.Solid);

            left = (float)(Parent.AbsLeft + 0.5f * Units.Centimeters + (Parent.Width - 1.0f * Units.Centimeters) * (Parent.Value - Parent.Minimum) / (Parent.Maximum - Parent.Minimum)) * e.ScaleX;
            top = (Parent.AbsTop + Parent.Height / 2) * e.ScaleY;
            height = Parent.Height * 0.4f * e.ScaleY;
            width = Parent.Width * 0.036f * e.ScaleX;

            float dx = width / 2;
            float dy = height * 0.3f;
            Brush brush = Fill.CreateBrush(new RectangleF(left - dx, top, width, height), e.ScaleX, e.ScaleY);
            PointF[] p = new PointF[]
            {
                new PointF(left, top),
                new PointF(left + dx, top + dy),
                new PointF(left + dx, top + height),
                new PointF(left - dx, top + height),
                new PointF(left - dx, top + dy)
            };

            if ((Parent as LinearGauge).Inverted)
            {
                p[1].Y = top - dy;
                p[2].Y = top - height;
                p[3].Y = top - height;
                p[4].Y = top - dy;
            }

            GraphicsPath path = new GraphicsPath();
            path.AddLines(p);
            path.AddLine(p[4], p[0]);

            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        private void DrawVert(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(BorderColor, BorderWidth * e.ScaleX, DashStyle.Solid);

            left = (Parent.AbsLeft + Parent.Width / 2) * e.ScaleX;
            top = (float)(Parent.AbsTop + Parent.Height - 0.5f * Units.Centimeters - (Parent.Height - 1.0f * Units.Centimeters) * (Parent.Value - Parent.Minimum) / (Parent.Maximum - Parent.Minimum)) * e.ScaleY;
            height = Parent.Height * 0.036f * e.ScaleY;
            width = Parent.Width * 0.4f * e.ScaleX;

            float dx = width * 0.3f;
            float dy = height / 2;
            Brush brush = Fill.CreateBrush(new RectangleF(left, top - dy, width, height), e.ScaleX, e.ScaleY);
            PointF[] p = new PointF[]
            {
                new PointF(left, top),
                new PointF(left + dx, top - dy),
                new PointF(left + width, top - dy),
                new PointF(left + width, top + dy),
                new PointF(left + dx, top + dy)
            };

            if ((Parent as LinearGauge).Inverted)
            {
                p[1].X = left - dx;
                p[2].X = left - width;
                p[3].X = left - width;
                p[4].X = left - dx;
            }

            GraphicsPath path = new GraphicsPath();
            path.AddLines(p);
            path.AddLine(p[4], p[0]);

            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        #endregion // Private Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void Assign(GaugePointer src)
        {
            base.Assign(src);

            LinearPointer s = src as LinearPointer;
            Height = s.Height;
            Width = s.Width;
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);

            if (Parent.Vertical)
            {
                DrawVert(e);
            }
            else
            {
                DrawHorz(e);
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer, string prefix, GaugePointer diff)
        {
            base.Serialize(writer, prefix, diff);

            LinearPointer dc = diff as LinearPointer;
            if (Height != dc.Height)
            {
                writer.WriteFloat(prefix + ".Height", Height);
            }
            if (Width != dc.Width)
            {
                writer.WriteFloat(prefix + ".Width", Width);
            }
        }

        #endregion // Public Methods
    }
}
