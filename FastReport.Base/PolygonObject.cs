using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport
{
    /// <summary>
    /// Represents a polygon object.
    /// </summary>
    /// <remarks>
    /// Use the <b>Border.Width</b>, <b>Border.Style</b> and <b>Border.Color</b> properties to set 
    /// the line width, style and color.
    /// 
    /// </remarks>
    public partial class PolygonObject : PolyLineObject
    {
        #region Protected Methods

        /// <summary>
        /// Calculate GraphicsPath for draw to page
        /// </summary>
        /// <param name="pen">Pen for lines</param>
        /// <param name="scaleX">scale by width</param>
        /// <param name="scaleY">scale by height</param>
        /// <returns>Always returns a non-empty path</returns>
        protected GraphicsPath getPolygonPath(Pen pen, float scaleX, float scaleY)
        {
            GraphicsPath gp = base.GetPath(pen, AbsLeft, AbsTop, AbsRight, AbsBottom, scaleX, scaleY);
            gp.CloseAllFigures();
            return gp;
        }

        /// <summary>
        /// Draw polyline path to graphics
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void drawPoly(FRPaintEventArgs e)
        {
            float x = (AbsLeft + Border.Width / 2) * e.ScaleX;
            float y = (AbsTop + Border.Width / 2) * e.ScaleY;
            float dx = (Width - Border.Width) * e.ScaleX - 1;
            float dy = (Height - Border.Width) * e.ScaleY - 1;

            Pen pen;
            if (polygonSelectionMode == PolygonSelectionMode.MoveAndScale)
            {
                pen = e.Cache.GetPen(Border.Color, Border.Width * e.ScaleX, Border.DashStyle);
            }
            else pen = e.Cache.GetPen(Border.Color, 1, DashStyle.Solid);

            Brush brush = null;
            if (Fill is SolidFill)
                brush = e.Cache.GetBrush((Fill as SolidFill).Color);
            else
                brush = Fill.CreateBrush(new RectangleF(x, y, dx, dy), e.ScaleX, e.ScaleY);

            using (GraphicsPath path = getPolygonPath(pen, e.ScaleX, e.ScaleY))
            {
                if(polygonSelectionMode == PolygonSelectionMode.MoveAndScale)
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            Border.SimpleBorder = true;
            base.Serialize(writer);
            PolygonObject c = writer.DiffObject as PolygonObject;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LineObject"/> class with default settings.
        /// </summary>
        public PolygonObject() : base()
        {
            FlagSimpleBorder = true;
            FlagUseFill = true;
        }
    }
}