using FastReport.Utils;
using System;
using System.Collections;
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
        #region Protected Internal Fields

        /// <summary>
        /// do not set this value, internal use only
        /// </summary>
        protected internal PolygonSelectionMode polygonSelectionMode;

        #endregion Protected Internal Fields

        #region Private Fields

        private PointF center;
        private PolyPointCollection pointsCollection;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Return points collection.
        /// You can modify the collection for change this object.
        /// </summary>
        public PolyPointCollection Points
        {
            get
            {
                return pointsCollection;
            }
        }

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
        /// deprecated
        /// </summary>
        [Browsable(false)]
        [Obsolete]
        public PointF[] PointsArray
        {
            get
            {
                List<PointF> result = new List<PointF>();
                foreach (PolyPoint point in pointsCollection)
                {
                    result.Add(new PointF(point.X, point.Y));
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// Return point types array. 0 - Start of line, 1 - Keep on line
        /// deprecated
        /// </summary>
        [Browsable(false)]
        [Obsolete]
        public byte[] PointTypesArray
        {
            get
            {
                List<byte> result = new List<byte>();
                result.Add(0);
                for (int i = 1; i < pointsCollection.Count; i++)
                    result.Add(1);
                return result.ToArray();
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineObject"/> class with default settings.
        /// </summary>
        public PolyLineObject()
        {
            FlagSimpleBorder = true;
            FlagUseFill = false;
            pointsCollection = new PolyPointCollection();
            center = PointF.Empty;
            InitDesign();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            PolyLineObject src = source as PolyLineObject;

            pointsCollection = src.pointsCollection.Clone();
            center = src.center;
            //recalculateBounds();
        }

        /// <inheritdoc/>
        public override void Deserialize(FRReader reader)
        {
            base.Deserialize(reader);
            pointsCollection.Clear();
            if (reader.HasProperty("PolyPoints"))
            {
                string polyPoints = reader.ReadStr("PolyPoints");
                foreach (string str in polyPoints.Split('|'))
                {
                    string[] point = str.Split('\\');
                    if (point.Length == 3)
                    {
                        float f1 = float.Parse(point[0].Replace(',', '.'), CultureInfo.InvariantCulture);
                        float f2 = float.Parse(point[1].Replace(',', '.'), CultureInfo.InvariantCulture);
                        pointsCollection.Add(new PolyPoint(f1, f2));
                    }
                }
            }
            else if (reader.HasProperty("PolyPoints_v2"))
            {
                string polyPoints = reader.ReadStr("PolyPoints_v2");
                foreach (string str in polyPoints.Split('|'))
                {
                    PolyPoint point = new PolyPoint();
                    point.Deserialize(str);
                    pointsCollection.Add(point);
                }
            }
            if (reader.HasProperty("CenterX"))
                center.X = reader.ReadFloat("CenterX");
            if (reader.HasProperty("CenterY"))
                center.Y = reader.ReadFloat("CenterY");
            //recalculateBounds();
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            switch (pointsCollection.Count)
            {
                case 0:
                case 1:
                    Graphics g = e.Graphics;
                    float x = AbsLeft + CenterX;
                    float y = AbsTop + CenterY;
                    if(pointsCollection.Count == 1)
                    {
                        x += pointsCollection[0].X;
                        y += pointsCollection[0].Y;
                    }
                    g.DrawLine(Pens.Black, x * e.ScaleX - 6, y * e.ScaleY, x * e.ScaleX + 6, y * e.ScaleY);
                    g.DrawLine(Pens.Black, x * e.ScaleX, y * e.ScaleY - 6, x * e.ScaleX, y * e.ScaleY + 6);
                    break;

                default:
                    DoDrawPoly(e);
                    DrawDesign0(e);
                    break;
            }
            DrawDesign1(e);
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
            if (pointsCollection.Count == 0)
            {
                GraphicsPath result = new GraphicsPath();
                result.AddLine(left * scaleX, top * scaleX, (right + 1) * scaleX, (bottom + 1) * scaleX);
                return result;
            }
            else if (pointsCollection.Count == 1)
            {
                GraphicsPath result = new GraphicsPath();
                left = left + CenterX + pointsCollection[0].X;
                top = top + CenterY + pointsCollection[0].Y;
                result.AddLine(left * scaleX, top * scaleX, (left + 1) * scaleX, (top + 1) * scaleX);
                return result;
            }

            List<PointF> aPoints = new List<PointF>();
            List<byte> pointTypes = new List<byte>();

            PolyPoint prev = null;
            PolyPoint point = pointsCollection[0];

            aPoints.Add(new PointF((point.X + left + center.X) * scaleX, (point.Y + top + center.Y) * scaleY));
            pointTypes.Add(0);

            int count = pointsCollection.Count;
            if (this is PolygonObject)
            {
                count++;
            }

            for (int i = 1; i < count; i++)
            {
                prev = point;
                point = pointsCollection[i];

                //is bezier?
                if (prev.RightCurve != null || point.LeftCurve != null)
                {
                    if (prev.RightCurve != null)
                    {
                        aPoints.Add(new PointF((prev.X + left + center.X + prev.RightCurve.X) * scaleX, (prev.Y + top + center.Y + prev.RightCurve.Y) * scaleY));
                        pointTypes.Add(3);
                    }
                    else
                    {
                        PolyPoint pseudo = GetPseudoPoint(prev, point);
                        aPoints.Add(new PointF((pseudo.X + left + center.X) * scaleX, (pseudo.Y + top + center.Y) * scaleY));
                        pointTypes.Add(3);
                    }

                    if (point.LeftCurve != null)
                    {
                        aPoints.Add(new PointF((point.X + left + center.X + point.LeftCurve.X) * scaleX, (point.Y + top + center.Y + point.LeftCurve.Y) * scaleY));
                        pointTypes.Add(3);
                    }
                    else
                    {
                        PolyPoint pseudo = GetPseudoPoint(point, prev);
                        aPoints.Add(new PointF((pseudo.X + left + center.X) * scaleX, (pseudo.Y + top + center.Y) * scaleY));
                        pointTypes.Add(3);
                    }

                    aPoints.Add(new PointF((point.X + left + center.X) * scaleX, (point.Y + top + center.Y) * scaleY));
                    pointTypes.Add(3);
                }
                else
                {
                    aPoints.Add(new PointF((point.X + left + center.X) * scaleX, (point.Y + top + center.Y) * scaleY));
                    pointTypes.Add(1);
                }
            }
            return new GraphicsPath(aPoints.ToArray(), pointTypes.ToArray());
        }

        /// <summary>
        /// Recalculate position and size of element
        /// </summary>
        public void RecalculateBounds()
        {
            if (pointsCollection.Count > 0)
            {
                // init
                PolyPoint prev = null;
                PolyPoint point = pointsCollection[0];
                float left = point.X;
                float top = point.Y;
                float right = point.X;
                float bottom = point.Y;
                int count = pointsCollection.Count;
                if (this is PolygonObject)
                    count++;

                // stage 1 calculate min bounds

                foreach (PolyPoint pnt in pointsCollection)
                {
                    if (pnt.X < left)
                        left = pnt.X;
                    else if (pnt.X > right)
                        right = pnt.X;

                    if (pnt.Y < top)
                        top = pnt.Y;
                    else if (pnt.Y > bottom)
                        bottom = pnt.Y;
                }

                // stage 2 check if one of bezier way point is outside

                for (int i = 1; i < count; i++)
                {
                    prev = point;
                    point = pointsCollection[i];

                    bool haveToCalculate = false;

                    
                    PolyPoint p_1 = null;
                    PolyPoint p_2 = null;

                    if (prev.RightCurve != null)
                    {
                        p_1 = new PolyPoint(prev.X + prev.RightCurve.X, prev.Y + prev.RightCurve.Y);

                        if (p_1.X < left)
                            haveToCalculate = true;
                        else if (p_1.X > right)
                            haveToCalculate = true;

                        if (p_1.Y < top)
                            haveToCalculate = true;
                        else if (p_1.Y > bottom)
                            haveToCalculate = true;
                    }

                    if (point.LeftCurve != null)
                    {
                        p_2 = new PolyPoint(point.X + point.LeftCurve.X, point.Y + point.LeftCurve.Y);
                        if (p_2.X < left)
                            haveToCalculate = true;
                        else if (p_2.X > right)
                            haveToCalculate = true;

                        if (p_2.Y < top)
                            haveToCalculate = true;
                        else if (p_2.Y > bottom)
                            haveToCalculate = true;
                    }

                    if (haveToCalculate)
                    {
                        if (p_1 == null)
                            p_1 = GetPseudoPoint(prev, point);

                        if (p_2 == null)
                            p_2 = GetPseudoPoint(point, prev);

                       
                        // now calculate extrema

                        // x
                        float delta = RecalculateBounds_Delta(prev.X, p_1.X, p_2.X, point.X);
                        if (delta > 0)
                        {
                            delta = (float)Math.Sqrt(delta);
                            float t_1 = RecalculateBounds_Solve(prev.X, p_1.X, p_2.X, point.X, -delta);
                            if (0 < t_1 && t_1 < 1)
                            {
                                float x = RecalculateBounds_Value(prev.X, p_1.X, p_2.X, point.X, t_1);
                                if (x < left)
                                    left = x;
                                else if (x > right)
                                    right = x;
                            }
                            float t_2 = RecalculateBounds_Solve(prev.X, p_1.X, p_2.X, point.X, delta);
                            if (0 < t_2 && t_2 < 1)
                            {
                                float x = RecalculateBounds_Value(prev.X, p_1.X, p_2.X, point.X, t_2);
                                if (x < left)
                                    left = x;
                                else if (x > right)
                                    right = x;
                            }
                        }

                        // y
                        delta = RecalculateBounds_Delta(prev.Y, p_1.Y, p_2.Y, point.Y);
                        if (delta > 0)
                        {
                            delta = (float)Math.Sqrt(delta);
                            float t_1 = RecalculateBounds_Solve(prev.Y, p_1.Y, p_2.Y, point.Y, -delta);
                            if (0 < t_1 && t_1 < 1)
                            {
                                float y = RecalculateBounds_Value(prev.Y, p_1.Y, p_2.Y, point.Y, t_1);
                                if (y < top)
                                    top = y;
                                else if (y > bottom)
                                    bottom = y;
                            }
                            float t_2 = RecalculateBounds_Solve(prev.Y, p_1.Y, p_2.Y, point.Y, delta);
                            if (0 < t_2 && t_2 < 1)
                            {
                                float y = RecalculateBounds_Value(prev.Y, p_1.Y, p_2.Y, point.Y, t_2);
                                if (y < top)
                                    top = y;
                                else if (y > bottom)
                                    bottom = y;
                            }
                        }
                        

                    }
                }


                // update
                float centerX = center.X;
                float centerY = center.Y;
                center.X = -left;
                center.Y = -top;
                base.Left += left + centerX;
                base.Top += top + centerY;
                base.Height = bottom - top;
                base.Width = right - left;
            }
            else
            {
                CenterX = 0;
                CenterY = 0;
                base.Width = 5;
                base.Height = 5;
            }
        }

        private float RecalculateBounds_Delta(float p_0,float p_1, float p_2, float p_3)
        {
            return p_1 * p_1 - p_0 * p_2 - p_1 * p_2 + p_2 * p_2 + p_0 * p_3 - p_1 * p_3;
        }

        private float RecalculateBounds_Solve(float p_0, float p_1, float p_2, float p_3, float deltaSqrt)
        {
            return (p_0 - 2 * p_1 + p_2 + deltaSqrt) / (p_0 - 3 * p_1 + 3 * p_2 - p_3);
        }

        private float RecalculateBounds_Value(float p_0, float p_1, float p_2, float p_3, float t)
        {
            float t1 = 1 - t;
            return p_0 * t1 * t1 * t1 + 3 * p_1 * t1 * t1 * t + 3 * p_2 * t1 * t * t + p_3 * t * t * t;
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            Border.SimpleBorder = true;
            base.Serialize(writer);
            PolyLineObject c = writer.DiffObject as PolyLineObject;

            StringBuilder sb = new StringBuilder(pointsCollection.Count * 10);
            foreach (PolyPoint point in pointsCollection)
            {
                point.Serialize(sb);
                sb.Append("|");
            }

            if (sb.Length > 0)
                sb.Length--;

            writer.WriteStr("PolyPoints_v2", sb.ToString());

            writer.WriteFloat("CenterX", center.X);
            writer.WriteFloat("CenterY", center.Y);
        }

        public void SetPolyLine(PointF[] newPoints)
        {
            pointsCollection.Clear();
            if (newPoints != null)
            {
                CenterX = 0;
                CenterY = 0;
                foreach (PointF point in newPoints)
                {
                    pointsCollection.Add(new PolyPoint(point.X, point.Y));
                }
            }
            float l = Left;
            float t = Top;
            RecalculateBounds();
            Left = l;
            Top = t;
        }

        #endregion Public Methods

        #region Internal Methods

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

        #endregion Internal Methods

        #region Protected Methods

        /// <summary>
        /// Add point to end of polyline, need to recalculate bounds after add
        /// First point must have zero coordinate and zero type.
        /// Recalculate bounds.
        /// Method is slow do not use this.
        /// </summary>
        /// <param name="localX">local x - relative to left-top point</param>
        /// <param name="localY">local y - relative to left-top point</param>
        /// <param name="pointType">depreceted</param>
        protected PolyPoint addPoint(float localX, float localY, byte pointType)
        {
            PolyPoint result;
            pointsCollection.Add(result = new PolyPoint(localX, localY));
            RecalculateBounds();
            return result;
        }

        /// <summary>
        /// Delete point from polyline by index.
        /// Recalculate bounds.
        /// Method is slow do not use this.
        /// </summary>
        /// <param name="index">Index of point in polyline</param>
        protected void deletePoint(int index)
        {
            pointsCollection.Remove(index);
            RecalculateBounds();
        }

        /// <summary>
        /// Draw polyline path to graphics
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void drawPoly(FRPaintEventArgs e)
        {
            Pen pen;
            if (polygonSelectionMode == PolygonSelectionMode.MoveAndScale)
                pen = e.Cache.GetPen(Border.Color, Border.Width * e.ScaleX, Border.DashStyle);
            else pen = e.Cache.GetPen(Border.Color, 1, DashStyle.Solid);

            using (GraphicsPath path = GetPath(pen, AbsLeft, AbsTop, AbsRight, AbsBottom, e.ScaleX, e.ScaleY))
                e.Graphics.DrawPath(pen, path);
        }

        /// <summary>
        /// Insert point to desired place of polyline
        /// recalculateBounds();
        /// Method is slow do not use this
        /// </summary>
        /// <param name="index">Index of place from zero to count</param>
        /// <param name="localX">local x - relative to left-top point</param>
        /// <param name="localY">local y - relative to left-top point</param>
        /// <param name="pointType">deprecated</param>
        protected PolyPoint insertPoint(int index, float localX, float localY, byte pointType)
        {
            PolyPoint result;
            pointsCollection.Insert(index, result = new PolyPoint(localX, localY));
            RecalculateBounds();
            return result;
        }

        #endregion Protected Methods

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

        private PolyPoint GetPseudoPoint(PolyPoint start, PolyPoint end)
        {
            float vecX = end.X - start.X;
            float vecY = end.Y - start.Y;

            float distance = (float)Math.Sqrt(vecX * vecX + vecY * vecY);

            vecX = vecX / 3;
            vecY = vecY / 3;

            return new PolyPoint(start.X + vecX, start.Y + vecY);
        }

        #endregion Private Methods

        #region Protected Internal Enums

        protected internal enum PolygonSelectionMode : int
        {
            MoveAndScale,
            Normal,
            AddToLine,
            AddBezier,
            Delete
        }

        #endregion Protected Internal Enums

        #region Public Classes

        /// <summary>
        /// Represent a point for polygon object
        /// </summary>
        public class PolyPoint
        {
            #region Private Fields

            private static readonly NumberFormatInfo invariant;
            private PolyPoint left;
            private PolyPoint right;
            private float x;
            private float y;

            #endregion Private Fields

            #region Public Properties

            public PolyPoint LeftCurve
            {
                get { return left; }
                set { left = value; }
            }

            public PolyPoint RightCurve
            {
                get { return right; }
                set { right = value; }
            }

            public float X
            {
                get { return x; }
                set { x = value; }
            }

            public float Y
            {
                get { return y; }
                set { y = value; }
            }

            #endregion Public Properties

            #region Public Constructors

            static PolyPoint()
            {
                invariant = new NumberFormatInfo();
                invariant.NumberGroupSeparator = String.Empty;
                invariant.NumberDecimalSeparator = ".";
            }

            public PolyPoint()
            {
            }

            public PolyPoint(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            #endregion Public Constructors

            #region Public Methods

            public void Deserialize(string s)
            {
                string[] strs = s.Split('/');
                int index = 0;
                Deserialize(strs, ref index);
                if (index < strs.Length)
                {
                    if (strs[index] == "L")
                    {
                        index++;
                        LeftCurve = new PolyPoint();
                        LeftCurve.Deserialize(strs, ref index);
                    }

                    if (index < strs.Length)
                    {
                        if (strs[index] == "R")
                        {
                            index++;
                            RightCurve = new PolyPoint();
                            RightCurve.Deserialize(strs, ref index);
                        }
                    }
                }
            }

            public bool Near(PolyPoint p)
            {
                return (p != null) && (Math.Abs(x - p.x) < 0.0001) && (Math.Abs(y - p.y) < 0.0001);
            }

            public void ScaleX(float scale)
            {
                x *= scale;
                if (LeftCurve != null)
                    LeftCurve.X *= scale;
                if (RightCurve != null)
                    RightCurve.X *= scale;
            }

            public void ScaleY(float scale)
            {
                y *= scale;
                if (LeftCurve != null)
                    LeftCurve.Y *= scale;
                if (RightCurve != null)
                    RightCurve.Y *= scale;
            }

            public void Serialize(StringBuilder sb)
            {
                sb.Append(Round(x)).Append("/").Append(Round(y));
                if (LeftCurve != null)
                {
                    sb.Append("/L/").Append(Round(LeftCurve.X)).Append("/").Append(Round(LeftCurve.Y));
                }
                if (RightCurve != null)
                {
                    sb.Append("/R/").Append(Round(RightCurve.X)).Append("/").Append(Round(RightCurve.Y));
                }
            }

            public override string ToString()
            {
                return "(" + Round(x) + ";" + Round(y) + ")";
            }

            #endregion Public Methods

            #region Private Methods

            private void Deserialize(string[] strs, ref int index)
            {
                for (int i = 0; i < 2 && index < strs.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            Single.TryParse(strs[index], NumberStyles.Float, invariant, out x);
                            break;

                        case 1:
                            Single.TryParse(strs[index], NumberStyles.Float, invariant, out y);
                            break;
                    }
                    index++;
                }
            }

            private string Round(float value)
            {
                return Convert.ToString(Math.Round(value, 4), invariant);
            }

            internal PolyPoint Clone()
            {
                PolyPoint result = new PolyPoint(x, y);
                if (LeftCurve != null)
                    result.LeftCurve = LeftCurve.Clone();
                if (RightCurve != null)
                    result.RightCurve = RightCurve.Clone();
                return result;
            }

            #endregion Private Methods
        }

        public class PolyPointCollection : IEnumerable<PolyPoint>
        {
            #region Private Fields

            private List<PolyPoint> points;

            #endregion Private Fields

            #region Public Indexers

            public PolyPoint this[int index]
            {
                get
                {
                    index = NormalizeIndex(index);
                    return points[index];
                }
                set
                {
                    index = NormalizeIndex(index);
                    points[index] = value;
                }
            }

            #endregion Public Indexers

            #region Public Properties

            public int Count
            {
                get
                {
                    return points.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            #endregion Public Properties

            #region Public Constructors

            public PolyPointCollection()
            {
                points = new List<PolyPoint>();
            }

            #endregion Public Constructors

            #region Public Methods

            public void Add(PolyPoint item)
            {
                points.Add(item);
            }

            public void Clear()
            {
                points.Clear();
            }

            public PolyPointCollection Clone()
            {
                PolyPointCollection result = new PolyPointCollection();

                result.points = new List<PolyPoint>();
                foreach(PolyPoint point in points)
                {
                    result.points.Add(point.Clone());
                }
                return result;
            }

            public IEnumerator<PolyPoint> GetEnumerator()
            {
                return points.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return points.GetEnumerator();
            }

            public int IndexOf(PolyPoint currentPoint)
            {
                return points.IndexOf(currentPoint);
            }

            public void Insert(int index, PolyPoint item)
            {
                int count = points.Count;

                if (count > 0)
                {
                    while (index < 0)
                        index += count;
                    while (index > count)
                        index -= count;
                }
                points.Insert(index, item);
            }

            public void Remove(int index)
            {
                index = NormalizeIndex(index);
                points.RemoveAt(index);
            }

            #endregion Public Methods

            #region Private Methods

            private int NormalizeIndex(int index)
            {
                int count = points.Count;
                if (count == 0)
                    return 0;
                if (index >= 0 && index < count)
                    return index;
                while (index < 0)
                    index += count;
                while (index >= count)
                    index -= count;
                return index;
            }

            #endregion Private Methods
        }

        #endregion Public Classes
    }
}