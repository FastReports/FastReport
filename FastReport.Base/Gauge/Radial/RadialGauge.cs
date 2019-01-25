using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport.Gauge.Radial
{
    #region Enums

    /// <summary>
    /// Radial Gauge types
    /// </summary>
    [Flags]
    public enum RadialGaugeType
    {
        /// <summary>
        /// Full sized gauge
        /// </summary>
        Circle = 1,

        /// <summary>
        /// Half of the radial gauge
        /// </summary>
        Semicircle = 2,

        /// <summary>
        /// Quarter of the radial gauge
        /// </summary>
        Quadrant = 4
    }

    /// <summary>
    /// Radial Gauge position types
    /// </summary>
    [Flags]
    public enum RadialGaugePosition
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Top
        /// </summary>
        Top = 1,

        /// <summary>
        /// Bottom
        /// </summary>
        Bottom = 2,

        /// <summary>
        /// Left
        /// </summary>
        Left = 4,

        /// <summary>
        /// Right
        /// </summary>
        Right = 8
    }

    #endregion // Enums
    
    /// <summary>
    /// Represents a linear gauge.
    /// </summary>
    public partial class RadialGauge : GaugeObject
    {
        private const double RAD = Math.PI / 180.0;
        private PointF center;
        private RadialGaugeType type;
        private RadialGaugePosition position;
        private float semicircleOffsetRatio;

        #region Properties
        /// <inheritdoc/>
        public override float Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                if (base.Height != base.Width)
                {
                    base.Height = Width;
                }
            }
        }

        /// <inheritdoc/>
        public override float Height
        {
            get { return base.Height; }
            set
            {
                base.Height = value;
                if (base.Width != base.Height)
                {
                    base.Width = Height;
                }
            }
        }

        /// <summary>
        /// Returns centr of the gauge
        /// </summary>
        [Browsable(false)]
        public PointF Center
        {
            get { return center; }
            set { center = value; }
        }

        /// <summary>
        /// The number of radians in one degree
        /// </summary>
        public static double Radians
        {
            get { return RAD; }
        }

        /// <summary>
        /// Gets or sets the Radial Gauge type
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public RadialGaugeType Type
        {
            get { return type; }
            set
            {
                if (value == RadialGaugeType.Circle)
                {
                    position = RadialGaugePosition.None;
                    type = value;
                }               
                if (value == RadialGaugeType.Semicircle &&
                    !(Position == RadialGaugePosition.Bottom ||
                    Position == RadialGaugePosition.Left ||
                    Position == RadialGaugePosition.Right ||
                    Position == RadialGaugePosition.Top))
                {
                    position = RadialGaugePosition.Top;
                    type = value;
                }
                else if (value == RadialGaugeType.Quadrant &&
                    !(
                    ((Position & RadialGaugePosition.Left) != 0 && (Position & RadialGaugePosition.Top) != 0 &&
                    (Position & RadialGaugePosition.Right) == 0 && (Position & RadialGaugePosition.Bottom) == 0) ||

                    ((Position & RadialGaugePosition.Right) != 0 && (Position & RadialGaugePosition.Top) != 0 &&
                    (Position & RadialGaugePosition.Left) == 0 && (Position & RadialGaugePosition.Bottom) == 0) ||
                 
                    ((Position & RadialGaugePosition.Left) != 0 && (Position & RadialGaugePosition.Bottom) != 0 &&
                    (Position & RadialGaugePosition.Right) == 0 && (Position & RadialGaugePosition.Top) == 0) ||
                 
                    ((Position & RadialGaugePosition.Right) != 0 && (Position & RadialGaugePosition.Bottom) != 0 &&
                    (Position & RadialGaugePosition.Left) == 0 && (Position & RadialGaugePosition.Top) == 0)
                    ))
                {
                    position = RadialGaugePosition.Top | RadialGaugePosition.Left;
                    type = value;
                }
                    
            }
        }

        /// <summary>
        /// Gats or sets the Radial Gauge position. Doesn't work for Full Radial Gauge.
        /// </summary>
        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.FlagsEditor, FastReport", typeof(UITypeEditor))]
        public RadialGaugePosition Position
        {
            get { return position; }
            set
            {
                if(Type == RadialGaugeType.Semicircle &&
                    (value == RadialGaugePosition.Bottom ||
                    value == RadialGaugePosition.Left ||
                    value == RadialGaugePosition.Right ||
                    value == RadialGaugePosition.Top))
                position = value;
                else if (Type == RadialGaugeType.Quadrant &&
                    (
                    ((value & RadialGaugePosition.Left) != 0 && (value & RadialGaugePosition.Top) != 0 &&
                    (value & RadialGaugePosition.Right) == 0 && (value & RadialGaugePosition.Bottom) == 0) ||

                    ((value & RadialGaugePosition.Right) != 0 && (value & RadialGaugePosition.Top) != 0 &&
                    (value & RadialGaugePosition.Left) == 0 && (value & RadialGaugePosition.Bottom) == 0) ||

                    ((value & RadialGaugePosition.Left) != 0 && (value & RadialGaugePosition.Bottom) != 0 &&
                    (value & RadialGaugePosition.Right) == 0 && (value & RadialGaugePosition.Top) == 0) ||

                    ((value & RadialGaugePosition.Right) != 0 && (value & RadialGaugePosition.Bottom) != 0 &&
                    (value & RadialGaugePosition.Left) == 0 && (value & RadialGaugePosition.Top) == 0)
                    ))
                    position = value;
                else if (Type == RadialGaugeType.Circle)
                    position  = 0;

            }
        }

        /// <summary>
        /// Gets or sets the semicircles offset
        /// </summary>
        [Category("Appearance")]
        public float SemicircleOffsetRatio
        {
            get { return semicircleOffsetRatio; }
            set { semicircleOffsetRatio = value; }
        }
        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGauge"/> class.
        /// </summary>
        public RadialGauge() : base()
        {
            InitializeComponent();
            Scale = new RadialScale(this);
            Pointer = new RadialPointer(this, Scale as RadialScale);
            Label = new RadialLabel(this);
            Height = 4.0f * Units.Centimeters;
            Width = 4.0f * Units.Centimeters;
            semicircleOffsetRatio = type == RadialGaugeType.Semicircle && 
                (position == RadialGaugePosition.Left || position == RadialGaugePosition.Right) ? 1.5f :  1;
            Type = RadialGaugeType.Circle;
            Border.Lines = BorderLines.None;
        }

        #endregion // Constructor

        #region Public Methods

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);
            RadialGauge src = source as RadialGauge;
            Type = src.Type;
            Position = src.Position;
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;

            float x = (AbsLeft + Border.Width / 2) * e.ScaleX;
            float y = (AbsTop + Border.Width / 2) * e.ScaleY;
            float dx = (Width - Border.Width) * e.ScaleX - 1;
            float dy = (Height - Border.Width) * e.ScaleY - 1;
            float x1 = x + dx;
            float y1 = y + dy;

            DashStyle[] styles = new DashStyle[] { DashStyle.Solid, DashStyle.Dash, DashStyle.Dot, DashStyle.DashDot, DashStyle.DashDotDot, DashStyle.Solid };
            Pen pen = e.Cache.GetPen(Border.Color, Border.Width * e.ScaleX, styles[(int)Border.Style]);
            Brush brush = null;
            if (Fill is SolidFill)
                brush = e.Cache.GetBrush((Fill as SolidFill).Color);
            else
                brush = Fill.CreateBrush(new RectangleF(x, y, dx, dy), e.ScaleX, e.ScaleY);

            center = new PointF(x + dx / 2, y + dy / 2);

            if (type == RadialGaugeType.Circle)
            {
                g.FillEllipse(brush, x, y, dx, dy);
                g.DrawEllipse(pen, x, y, dx, dy);
            }
            else if (type == RadialGaugeType.Semicircle)
            {
                float semiOffset = (Width / 16f /2f + 2f) * semicircleOffsetRatio * e.ScaleY;
                if (position == RadialGaugePosition.Top)
                {
                    g.FillPie(brush, x, y, dx, dy, -180, 180);
                    g.DrawArc(pen, x, y, dx, dy, -180, 180);

                    PointF startPoint = RadialUtils.RotateVector(new PointF[] { new PointF(x + dx / 2, y), center }, -90 * RAD, center)[0];
                    
                    PointF[] points = new PointF[4];
                    points[0] = new PointF(startPoint.X, startPoint.Y - 1 * e.ScaleY);
                    points[1] = new PointF(startPoint.X, startPoint.Y + semiOffset);
                    points[2] = new PointF(startPoint.X + dx, startPoint.Y + semiOffset);
                    points[3] = new PointF(startPoint.X + dx, startPoint.Y - 1 * e.ScaleY);
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(points);
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
          
                }
                else if(position == RadialGaugePosition.Bottom)
                {
                    g.FillPie(brush, x, y, dx, dy, 0, 180);
                    g.DrawArc(pen, x, y, dx, dy, 0, 180);

                    PointF startPoint = RadialUtils.RotateVector(new PointF[] { new PointF(x + dx / 2, y), center }, 90 * RAD, center)[0];

                    PointF[] points = new PointF[4];
                    points[0] = new PointF(startPoint.X, startPoint.Y + 1 * e.ScaleY);
                    points[1] = new PointF(startPoint.X, startPoint.Y - semiOffset);
                    points[2] = new PointF(startPoint.X - dx, startPoint.Y - semiOffset);
                    points[3] = new PointF(startPoint.X - dx, startPoint.Y + 1 * e.ScaleY);
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(points);
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }
                else if (position == RadialGaugePosition.Left)
                {
                    g.FillPie(brush, x, y, dx, dy, 90, 180);
                    g.DrawArc(pen, x, y, dx, dy, 90, 180);

                    PointF startPoint = RadialUtils.RotateVector(new PointF[] { new PointF(x + dx / 2, y), center }, 180 * RAD, center)[0];

                    PointF[] points = new PointF[4];
                    points[0] = new PointF(startPoint.X - 1 * e.ScaleX, startPoint.Y);
                    points[1] = new PointF(startPoint.X + semiOffset, startPoint.Y);
                    points[2] = new PointF(startPoint.X + semiOffset, startPoint.Y - dy);
                    points[3] = new PointF(startPoint.X - 1 * e.ScaleX, startPoint.Y - dy);
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(points);
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }
                else if (position == RadialGaugePosition.Right)
                {
                    g.FillPie(brush, x, y, dx, dy, -90, 180);
                    g.DrawArc(pen, x, y, dx, dy, -90, 180);

                    PointF startPoint = RadialUtils.RotateVector(new PointF[] { new PointF(x + dx / 2, y), center }, -180 * RAD, center)[0];
                    
                    PointF[] points = new PointF[4];
                    points[0] = new PointF(startPoint.X + 1 * e.ScaleX, startPoint.Y);
                    points[1] = new PointF(startPoint.X - semiOffset, startPoint.Y);
                    points[2] = new PointF(startPoint.X - semiOffset, startPoint.Y - dy);
                    points[3] = new PointF(startPoint.X + 1 * e.ScaleX, startPoint.Y - dy);
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(points);
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }
            }
            else if (type == RadialGaugeType.Quadrant)
            {
                float semiOffset = (Width / 16f / 2f + 2f) * semicircleOffsetRatio * e.ScaleY;
                if (RadialUtils.IsTop(this) && RadialUtils.IsLeft(this))
                {
                    g.FillPie(brush, x, y, dx, dy, -180, 90);
                    g.DrawArc(pen, x, y, dx, dy, -180, 90);

                    PointF startPoint = RadialUtils.RotateVector(new PointF[] { new PointF(x + dx / 2, y), center }, -90 * RAD, center)[0];

                    PointF[] points = new PointF[5];
                    points[0] = new PointF(startPoint.X, startPoint.Y - 1 * e.ScaleY);
                    points[1] = new PointF(startPoint.X, startPoint.Y + semiOffset);
                    points[2] = new PointF(startPoint.X + dx / 2 + semiOffset, startPoint.Y + semiOffset);
                    points[3] = new PointF(startPoint.X + dx / 2 + semiOffset, y);
                    points[4] = new PointF(startPoint.X + dx / 2 - 1 * e.ScaleX, y);
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(points);
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);

                }
                else if (RadialUtils.IsBottom(this) && RadialUtils.IsLeft(this))
                {

                    g.FillPie(brush, x, y, dx, dy, -270, 90);
                    g.DrawArc(pen, x, y, dx, dy, -270, 90);

                    PointF startPoint = RadialUtils.RotateVector(new PointF[] { new PointF(x + dx / 2, y), center }, -90 * RAD, center)[0];
                    PointF[] points = new PointF[5];
                    points[0] = new PointF(startPoint.X, startPoint.Y + 1 * e.ScaleY);
                    points[1] = new PointF(startPoint.X, startPoint.Y - semiOffset);
                    points[2] = new PointF(startPoint.X + dx / 2 + semiOffset, startPoint.Y - semiOffset);
                    points[3] = new PointF(startPoint.X + dx / 2 + semiOffset, y + dy);
                    points[4] = new PointF(x + dx / 2 - 1 * e.ScaleX, y + dy);
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(points);
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }
               else if (RadialUtils.IsTop(this) && RadialUtils.IsRight(this))
                {
                    g.FillPie(brush, x, y, dx, dy, -90, 90);
                    g.DrawArc(pen, x, y, dx, dy, -90, 90);

                    PointF startPoint = RadialUtils.RotateVector(new PointF[] { new PointF(x + dx / 2, y), center }, 90 * RAD, center)[0];

                    PointF[] points = new PointF[5];
                    points[0] = new PointF(startPoint.X, startPoint.Y - 1 * e.ScaleY);
                    points[1] = new PointF(startPoint.X, startPoint.Y + semiOffset);
                    points[2] = new PointF(startPoint.X - dx / 2 - semiOffset, startPoint.Y + semiOffset);
                    points[3] = new PointF(x + dx / 2 - semiOffset , y);
                    points[4] = new PointF(x + dx / 2 + 1 * e.ScaleX, y);
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(points);
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }
                else if (RadialUtils.IsBottom(this) && RadialUtils.IsRight(this))
                {
                    g.FillPie(brush, x, y, dx, dy, 0, 90);
                    g.DrawArc(pen, x, y, dx, dy, 0, 90);

                    PointF startPoint = RadialUtils.RotateVector(new PointF[] { new PointF(x + dx / 2, y), center }, 90 * RAD, center)[0];

                    PointF[] points = new PointF[5];
                    points[0] = new PointF(startPoint.X, startPoint.Y + 1 * e.ScaleY);
                    points[1] = new PointF(startPoint.X, startPoint.Y - semiOffset);
                    points[2] = new PointF(x + dx / 2 - semiOffset, startPoint.Y - semiOffset);
                    points[3] = new PointF(x + dx / 2 - semiOffset, y + dy);
                    points[4] = new PointF(x + dx / 2 + 1 * e.ScaleX, y + dy);
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(points);
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }
            }

            Scale.Draw(e);
            Pointer.Draw(e);
            Label.Draw(e);
            DrawMarkers(e);
            if (!(Fill is SolidFill))
                brush.Dispose();
            if (Report != null && Report.SmoothGraphics)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            RadialGauge c = writer.DiffObject as RadialGauge;
            base.Serialize(writer);
            if (Type != c.Type)
            {
                writer.WriteValue("Type", Type);
            }
            if (Position != c.Position)
            {
                writer.WriteValue("Position", Position);
            }
        }

        #endregion // Public Methods
    }
}
