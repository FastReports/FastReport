using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace FastReport
{
    /// <summary>
    /// The interface for unifying methods for drawing objects into different graphics
    /// </summary>
    public interface IGraphics : IDisposable
    {
        #region Properties
        Graphics Graphics { get; }
        float DpiY { get; }
        TextRenderingHint TextRenderingHint { get; set; }
        InterpolationMode InterpolationMode { get; set; }
        SmoothingMode SmoothingMode { get; set; }
        System.Drawing.Drawing2D.Matrix Transform { get; set; }
        GraphicsUnit PageUnit { get; set; }
        bool IsClipEmpty { get; }
        Region Clip { get; set; }
        float DpiX { get; }
        CompositingQuality CompositingQuality { get; set; }
        #endregion

        #region Draw and measure text
        void DrawString(string text, Font font, Brush brush, float left, float top);
        void DrawString(string text, Font font, Brush brush, float left, float top, StringFormat format);
        // in this case if a baseline is needed, it will not be calculated
        void DrawString(string text, Font font, Brush brush, RectangleF rectangleF);
        void DrawString(string text, Font font, Brush textBrush, RectangleF textRect, StringFormat format);
        void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format);
        Region[] MeasureCharacterRanges(string text, Font font, RectangleF textRect, StringFormat format);
        SizeF MeasureString(string text, Font font);
        SizeF MeasureString(string text, Font font, SizeF size);
        SizeF MeasureString(string text, Font font, int v, StringFormat format);
        void MeasureString(string text, Font font, SizeF size, StringFormat format, out int charsFit, out int linesFit);
        SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat);
        #endregion

        #region Draw images
        void DrawImage(Image image, float x, float y);
        void DrawImage(Image image, RectangleF rect1, RectangleF rect2, GraphicsUnit unit);
        void DrawImage(Image image, RectangleF rect);
        void DrawImage(Image image, float x, float y, float width, float height);
        void DrawImage(Image image, PointF[] points);
        void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr);
        void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs);
        void DrawImageUnscaled(Image image, Rectangle rect);
        #endregion

        #region Draw geometry
        void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);
        void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension);
        void DrawEllipse(Pen pen, float left, float top, float width, float height);
        void DrawEllipse(Pen pen, RectangleF rect);
        void DrawLine(Pen pen, float x1, float y1, float x2, float y2);
        void DrawLine(Pen pen, PointF p1, PointF p2);
        void DrawLines(Pen pen, PointF[] points);
        void DrawPath(Pen outlinePen, GraphicsPath path);
        void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);
        void DrawPolygon(Pen pen, PointF[] points);
        void DrawPolygon(Pen pen, Point[] points);
        void DrawRectangle(Pen pen, float left, float top, float width, float height);
        void DrawRectangle(Pen pen, Rectangle rectangle);
        #endregion

        #region Fill geometry
        void FillEllipse(Brush brush, float left, float top, float width, float height);
        void FillEllipse(Brush brush, RectangleF rect);
        // Works with polygons only
        void FillPath(Brush brush, GraphicsPath path);
        void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle);
        void FillPolygon(Brush brush, PointF[] points);
        void FillPolygon(Brush brush, Point[] points);
        // Add rectangle to the graphics path
        void FillRectangle(Brush brush, RectangleF rect);
        void FillRectangle(Brush brush, float left, float top, float width, float height);
        void FillRegion(Brush brush, Region region);
        #endregion

        #region Fill and Draw

        void FillAndDrawPath(Pen pen, Brush brush, GraphicsPath path);
        void FillAndDrawEllipse(Pen pen, Brush brush, RectangleF rect);
        void FillAndDrawEllipse(Pen pen, Brush brush, float left, float top, float width, float height);
        void FillAndDrawPolygon(Pen pen, Brush brush, Point[] points);
        void FillAndDrawPolygon(Pen pen, Brush brush, PointF[] points);
        void FillAndDrawRectangle(Pen pen, Brush brush, float left, float top, float width, float height);

        #endregion

        #region Transform
        void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix, MatrixOrder prepend);
        void RotateTransform(float angle);
        void ScaleTransform(float scaleX, float scaleY);
        void TranslateTransform(float left, float top);
        #endregion

        #region State
        void Restore(IGraphicsState state);
        IGraphicsState Save();
        #endregion

        #region Clip
        bool IsVisible(RectangleF rect);
        void ResetClip();
        void SetClip(RectangleF rect);
        void SetClip(RectangleF rect, CombineMode combineMode);
        void SetClip(GraphicsPath path, CombineMode combineMode);
        #endregion
    }

    /// <summary>
    /// the interface for saving and restoring state
    /// </summary>
    public interface IGraphicsState
    {

    }

}
