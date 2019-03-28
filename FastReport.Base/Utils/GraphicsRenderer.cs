using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;

namespace FastReport.Utils
{
    /// <summary>
    /// The interface for unifying methods for drawing objects into different graphics
    /// </summary>
    public interface IGraphicsRenderer : IDisposable
    {
        void TranslateTransform(float left, float top);
        void RotateTransform(float angle);
        IGraphicsRendererState Save();
        void Restore(IGraphicsRendererState state);
        SizeF MeasureString(string text, Font drawFont);
        void DrawString(string text, Font drawFont, Brush brush, float left, float top);
        /// <summary>
        /// in this case if a baseline is needed, it will not be calculated
        /// </summary>
        void DrawString(string text, Font drawFont, Brush brush, float left, float top, float baseLine);
        void DrawLine(Pen pen, float x1, float y1, float x2, float y2);
        void DrawString(string text, Font drawFont, Brush brush, RectangleF rectangleF);
        void FillRectangle(Brush brush, float left, float top, float width, float height);
        void FillPolygon(Brush brush, PointF[] points);
        void DrawEllipse(Pen pen, float left, float top, float width, float height);
        /// <summary>
        /// Works with polygons only
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="path"></param>
        void FillPath(Brush brush, GraphicsPath path);
        /// <summary>
        /// Add rectangle to the graphics path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rect"></param>
        void PathAddRectangle(GraphicsPath path, RectangleF rect);
    }

    /// <summary>
    /// the interface for saving and restoring state
    /// </summary>
    public interface IGraphicsRendererState
    {

    }

    /// <summary>
    /// Drawing objects to a standard image in Bitmap
    /// </summary>
    internal class ImageGraphicsRenderer : IGraphicsRenderer
    {
        private Graphics graphics = null;
        private bool haveToDispose = false;

        public Graphics Graphics
        {
            get { return graphics; }
        }

        public ImageGraphicsRenderer(Image image)
            : this(Graphics.FromImage(image), true)
        {

        }

        public ImageGraphicsRenderer(Graphics graphics, bool haveToDispose)
        {
            this.graphics = graphics;
            //this.FGraphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            //this.FGraphics.CompositingQuality = CompositingQuality.Default;
            //this.FGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            //this.FGraphics.SmoothingMode = SmoothingMode.Default;
            this.haveToDispose = haveToDispose;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (graphics != null && haveToDispose)
                        graphics.Dispose();
                    graphics = null;
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ImageGraphicsRenderer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public void TranslateTransform(float left, float top)
        {
            this.graphics.TranslateTransform(left, top);
        }

        public void RotateTransform(float angle)
        {
            this.graphics.RotateTransform(angle);
        }

        public IGraphicsRendererState Save()
        {
            return new ImageGraphicsRendererState(this.graphics.Save());
        }

        public void Restore(IGraphicsRendererState state)
        {
            if (state is ImageGraphicsRendererState)
                this.graphics.Restore((state as ImageGraphicsRendererState).GraphicsState);
        }

        public SizeF MeasureString(string text, Font drawFont)
        {
            return this.graphics.MeasureString(text, drawFont);
        }

        public void DrawString(string text, Font drawFont, Brush brush, float left, float top)
        {
            this.graphics.DrawString(text, drawFont, brush, left, top);
        }

        /// <summary>
        /// baseLine is ignored
        /// </summary>
        public void DrawString(string text, Font drawFont, Brush brush, float left, float top, float baseLine)
        {
            DrawString(text, drawFont, brush, left, top);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            this.graphics.DrawLine(pen, x1, y1, x2, y2);
        }

        public void DrawString(string text, Font drawFont, Brush brush, RectangleF rectangleF)
        {
            this.graphics.DrawString(text, drawFont, brush, rectangleF);
        }

        public void FillRectangle(Brush brush, float left, float top, float width, float height)
        {
            this.graphics.FillRectangle(brush, left, top, width, height);
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            this.graphics.FillPolygon(brush, points);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            
            this.graphics.FillPath(brush, path);
        }

        public void DrawEllipse(Pen pen, float left, float top, float width, float height)
        {
            this.graphics.DrawEllipse(pen, left, top, width, height);
        }
        #endregion

        public void PathAddRectangle(GraphicsPath path, RectangleF rect)
        {
            path.AddRectangle(rect);
        }

        public class ImageGraphicsRendererState : IGraphicsRendererState
        {
            private GraphicsState graphicsState;

            public GraphicsState GraphicsState
            {
                get
                {
                    return graphicsState;
                }
            }
            public ImageGraphicsRendererState(GraphicsState state)
            {
                this.graphicsState = state;
            }
        }
    }

}
