using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;

namespace FastReport
{
    /// <summary>
    /// Represents a poly line object.
    /// </summary>
    /// <remarks>
    /// Use the <b>Border.Width</b>, <b>Border.Style</b> and <b>Border.Color</b> properties to set
    /// the line width, style and color.
    /// </remarks>
    public partial class PolyLineObject : ReportComponentBase
    {
        #region Fields
        private PointF center;
        private List<PointF> points;
        private List<byte> pointTypes;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Returns origin of coordinates relative to the top left corner
        /// </summary>
        [Browsable(false)]
        public float CenterX { get { return center.X; } set { center.X = value; } }

        /// <summary>
        /// Returns origin of coordinates relative to the top left corner
        /// </summary>
        [Browsable(false)]
        public float CenterY { get { return center.Y; } set { center.Y = value; } }

        /// <summary>
        /// Return points array of line
        /// </summary>
        [Browsable(false)]
        public PointF[] PointsArray
        {
            get
            {
                if (points == null || points.Count == 0) return new PointF[0];
                return points.ToArray();
            }
        }

        /// <summary>
        /// Return point types array. 0 - Start of line, 1 - Keep on line
        /// </summary>
        [Browsable(false)]
        public byte[] PointTypesArray
        {
            get
            {
                if (pointTypes == null || pointTypes.Count == 0) return new byte[0];
                return pointTypes.ToArray();
            }
        }
        #endregion Properties

        #region Private Methods
        private float getDistance(float px, float py, float px0, float py0, float px1, float py1, out int index)
        {
            float vx = px1 - px0;
            float vy = py1 - py0;
            float wx = px - px0;
            float wy = py - py0;
            float c1 = vx * wx + vy * wy;
            if (c1 <= 0) { index = -1; return (px0 - px) * (px0 - px) + (py0 - py) * (py0 - py); }
            float c2 = vx * vx + vy * vy;
            if (c2 <= c1) { index = 1; return (px1 - px) * (px1 - px) + (py1 - py) * (py1 - py); }
            float b = c1 / c2;
            index = 0;
            float bx = px0 + vx * b;
            float by = py0 + vy * b;
            return (bx - px) * (bx - px) + (by - py) * (by - py);
        }
        #endregion Private Methods

        #region Protected Methods
        /// <summary>
        /// Add point to end of polyline, need to recalculate bounds after add
        /// First point must have zero coordinate and zero type
        /// </summary>
        /// <param name="localX">local x - relative to left-top point</param>
        /// <param name="localY">local y - relative to left-top point</param>
        /// <param name="pointType">0-start,1-line</param>
        protected void addPoint(float localX, float localY, byte pointType)
        {
            if (points == null)
            {
                points = new List<PointF>();
                points.Add(new PointF(localX, localY));
            }
            else
            {
                points.Add(new PointF(localX, localY));
            }
            if (pointTypes == null)
            {
                pointTypes = new List<byte>();
                pointTypes.Add((byte)PathPointType.Start);
            }
            else
            {
                pointTypes.Add(pointType);
            }
        }

        /// <summary>
        /// Delete point from polyline by index
        /// Recalculate bounds
        /// </summary>
        /// <param name="index">Index of point in polyline</param>
        protected void deletePoint(int index)
        {
            if (points == null || points.Count == 0)
                return;
            if (pointTypes[index] == 0 && index < pointTypes.Count - 1)
            {
                pointTypes[index + 1] = 0;
            }
            if (points.Count > 1)
            {
                points.RemoveAt(index);
                pointTypes.RemoveAt(index);
                recalculateBounds();
            }
        }

        /// <summary>
        /// Draw polyline path to graphics
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void drawPoly(FRPaintEventArgs e)
        {
            Pen pen = e.Cache.GetPen(Border.Color, Border.Width * e.ScaleX, Border.DashStyle);
            using (GraphicsPath path = GetPath(pen, AbsLeft, AbsTop, AbsRight, AbsBottom, e.ScaleX, e.ScaleY))
                e.Graphics.DrawPath(pen, path);
        }

        /// <summary>
        /// Calculate GraphicsPath for draw to page
        /// </summary>
        /// <param name="pen">Pen for lines</param>
        /// <param name="left">Left boundary</param>
        /// <param name="top">Top boundary</param>
        /// <param name="right">Right boundary</param>
        /// <param name="bottom">Bottom boundary</param>
        /// <param name="scaleX">scale by width</param>
        /// <param name="scaleY">scale by height</param>
        /// <returns>Always returns a non-empty path</returns>
        public GraphicsPath GetPath(Pen pen, float left, float top, float right, float bottom, float scaleX, float scaleY)
        {
            if (points == null)
            {
                GraphicsPath result = new GraphicsPath();
                result.AddLine(left, top, right + 1, bottom + 1);
                return result;
            }
            GraphicsPath gp = null;
            if (points.Count > 1)
            {
                PointF[] aPoints = new PointF[points.Count];
                int i = 0;
                foreach (PointF point in points)
                {
                    aPoints[i] = new PointF((point.X + left + center.X) * scaleX, (point.Y + top + center.Y) * scaleY);
                    i++;
                }
                gp = new GraphicsPath(aPoints, pointTypes.ToArray());
            }
            else gp = new GraphicsPath();
            return gp;
        }

        /// <summary>
        /// Insert point to desired place of polyline
        /// </summary>
        /// <param name="index">Index of place from zero to count</param>
        /// <param name="localX">local x - relative to left-top point</param>
        /// <param name="localY">local y - relative to left-top point</param>
        /// <param name="pointType">0-start,1-line</param>
        protected void insertPoint(int index, float localX, float localY, byte pointType)
        {
            if (points == null || points.Count == 0)
            {
                addPoint(localX, localY, pointType);
                return;
            }

            if (pointTypes.Count > index && pointTypes[index] == 0) { pointTypes[index] = 1; pointType = 0; }
            points.Insert(index, new PointF(localX, localY));
            pointTypes.Insert(index, pointType);
            //recalculateBounds();
        }

        /// <summary>
        /// Recalculate position and size of element
        /// </summary>
        protected void recalculateBounds()
        {
            float left = points[0].X;
            float top = points[0].Y;
            float right = points[0].X;
            float bottom = points[0].Y;

            foreach (PointF point in points)
            {
                if (point.X < left)
                    left = point.X;
                else if (point.X > right)
                    right = point.X;
                if (point.Y < top)
                    top = point.Y;
                else if (point.Y > bottom)
                    bottom = point.Y;
            }
            if (Math.Abs(right - left) == 0)
            {
                right += 1;
            }
            if (Math.Abs(bottom - top) == 0)
            {
                bottom += 1;
            }
            Left += left + center.X;
            center.X = -left;
            base.Width = Math.Abs(right - left);
            Top += top + center.Y;
            center.Y = -top;
            base.Height = Math.Abs(bottom - top);
        }
        #endregion Protected Methods

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            PolyLineObject src = source as PolyLineObject;
            if (src.points == null) return;
            if (src.pointTypes == null) return;
            points = new List<PointF>(src.points);
            pointTypes = new List<byte>(src.pointTypes);
            center = src.center;
            //recalculateBounds();
        }

        /// <inheritdoc/>
        public override void Deserialize(FRReader reader)
        {
            base.Deserialize(reader);
            points = new List<PointF>();
            pointTypes = new List<byte>();
            string polyPoints = reader.ReadStr("PolyPoints");
            foreach (string str in polyPoints.Split('|'))
            {
                string[] point = str.Split('\\');
                if (point.Length == 3)
                {
                    float f1 = float.Parse(point[0].Replace(',', '.'), CultureInfo.InvariantCulture);
                    float f2 = float.Parse(point[1].Replace(',', '.'), CultureInfo.InvariantCulture);
                    addPoint(f1, f2, byte.Parse(point[2]));
                }
            }
            if (reader.HasProperty("CenterX"))
                center.X = reader.ReadFloat("CenterX");
            if (reader.HasProperty("CenterY"))
                center.Y = reader.ReadFloat("CenterY");
            //recalculateBounds();
        }

      internal void DoDrawPoly(FRPaintEventArgs e)
      {
        Graphics g = e.Graphics;
        Report report = Report;
        if (report != null && report.SmoothGraphics)
        {
          g.InterpolationMode = InterpolationMode.HighQualityBicubic;
          g.SmoothingMode = SmoothingMode.AntiAlias;
        }

        drawPoly(e);

        if (report != null && report.SmoothGraphics)
        {
          g.InterpolationMode = InterpolationMode.Default;
          g.SmoothingMode = SmoothingMode.Default;
        }
      }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            if ((points == null || points.Count == 1))
            {
                Graphics g = e.Graphics;
                g.DrawLine(Pens.Black, AbsLeft * e.ScaleX - 6, AbsTop * e.ScaleY, AbsLeft * e.ScaleX + 6, AbsTop * e.ScaleY);
                g.DrawLine(Pens.Black, AbsLeft * e.ScaleX, AbsTop * e.ScaleY - 6, AbsLeft * e.ScaleX, AbsTop * e.ScaleY + 6);
            }
            else
            {
                DoDrawPoly(e);
                DrawDesign0(e);
            }
            DrawDesign1(e);
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            Border.SimpleBorder = true;
            base.Serialize(writer);
            PolyLineObject c = writer.DiffObject as PolyLineObject;

            StringBuilder sb = new StringBuilder(points.Count * 10);
            for (int i = 0; i < points.Count; i++)
            {
                sb.AppendFormat("{0}\\{1}\\{2}", points[i].X.ToString(CultureInfo.InvariantCulture),
                    points[i].Y.ToString(CultureInfo.InvariantCulture),
                    pointTypes[i]);
                if (i < points.Count - 1)
                    sb.Append("|");
            }

            writer.WriteStr("PolyPoints", sb.ToString());

            writer.WriteFloat("CenterX", center.X);
            writer.WriteFloat("CenterY", center.Y);
        }

        #endregion Public Methods

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineObject"/> class with default settings.
        /// </summary>
        public PolyLineObject()
        {
            FlagSimpleBorder = true;
            FlagUseFill = false;
            points = null;
            pointTypes = null;
            center = PointF.Empty;
            InitDesign();
        }

        #endregion Public Constructors
    }
}