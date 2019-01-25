using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport.Gauge.Simple
{
    /// <summary>
    /// Represents a simple pointer.
    /// </summary>
    public class SimplePointer : GaugePointer
    {
        #region Fields

        private float left;
        private float top;
        private float horizontalOffset;
        private float height;
        private float width;
        private float ptrRatio = 0.08f;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets o sets the Left offset of gauge pointer.
        /// </summary>
        [Browsable(false)]
        internal float Left
        {
            get { return left; }
            set { left = value; }
        }

        /// <summary>
        /// Gets o sets the Top offset of gauge pointer.
        /// </summary>
        [Browsable(false)]
        internal float Top
        {
            get { return top; }
            set { top = value; }
        }

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

        /// <summary>
        /// Gets or sets the pointer ratio.
        /// </summary>
        [Browsable(false)]
        public float PointerRatio
        {
            get { return ptrRatio; }
            set { ptrRatio = value; }
        }

        /// <summary>
        /// Gets or sets the pointer horizontal offset (cm).
        /// </summary>
        [Browsable(false)]
        public float HorizontalOffset
        {
            get { return horizontalOffset; }
            set { horizontalOffset = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePointer"/> class.
        /// </summary>
        /// <param name="parent">The parent gauge object.</param>
        public SimplePointer(GaugeObject parent) : base(parent)
        {
            height = Parent.Height * ptrRatio;
            width = Parent.Width * ptrRatio;
            horizontalOffset = 0.5f * Units.Centimeters;
        }

        #endregion // Constructors

        #region Internal Methods

        internal virtual void DrawHorz(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(BorderColor, BorderWidth * e.ScaleX, DashStyle.Solid);

            left = (Parent.AbsLeft + Parent.Border.Width / 2 + horizontalOffset) * e.ScaleX;
            top = (Parent.AbsTop + Parent.Border.Width / 2 + (Parent.Height - Parent.Border.Width) / 2 - (Parent.Height - Parent.Border.Width) * ptrRatio / 2) * e.ScaleY;
            height = ((Parent.Height - Parent.Border.Width) * ptrRatio) * e.ScaleY;
            width = (float)((Parent.Width - Parent.Border.Width - horizontalOffset * 2) * (Parent.Value - Parent.Minimum) / (Parent.Maximum - Parent.Minimum) * e.ScaleX);

            Brush brush = Fill.CreateBrush(new RectangleF(left, top, width, height), e.ScaleX, e.ScaleY);
            g.FillRectangle(brush, left, top, width, height);
            g.DrawRectangle(pen, left, top, width, height);
        }

        internal virtual void DrawVert(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = e.Cache.GetPen(BorderColor, BorderWidth * e.ScaleY, DashStyle.Solid);

            width = ((Parent.Width - Parent.Border.Width) * ptrRatio) * e.ScaleX;
            height = (float)((Parent.Height - Parent.Border.Width -  horizontalOffset * 2) * (Parent.Value - Parent.Minimum) / (Parent.Maximum - Parent.Minimum) * e.ScaleY);
            left = (Parent.AbsLeft + Parent.Border.Width / 2 + (Parent.Width - Parent.Border.Width) / 2 - (Parent.Width - Parent.Border.Width) * ptrRatio / 2) * e.ScaleX;
            top = (Parent.AbsTop + Parent.Border.Width / 2 + Parent.Height - Parent.Border.Width - horizontalOffset) * e.ScaleY - height;

            Brush brush = Fill.CreateBrush(new RectangleF(left, top, width, height), e.ScaleX, e.ScaleY);
            g.FillRectangle(brush, left, top, width, height);
            g.DrawRectangle(pen, left, top, width, height);
        }

        #endregion // Internal Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void Assign(GaugePointer src)
        {
            base.Assign(src);

            SimplePointer s = src as SimplePointer;
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

            SimplePointer dc = diff as SimplePointer;
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
