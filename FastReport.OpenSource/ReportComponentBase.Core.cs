using FastReport.Utils;

namespace FastReport
{
    partial class ReportComponentBase
    {
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="e">Draw event arguments.</param>
        public void DrawMarkers(FRPaintEventArgs e)
        {

        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="e">Draw event arguments.</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void DrawCrossHair(FRPaintEventArgs e, float x, float y)
        {
        }

        /// <summary>
        /// Copies event handlers from another similar object.
        /// </summary>
        /// <param name="source">The object to copy handlers from.</param>
        public virtual void AssignPreviewEvents(Base source)
        {
            ReportComponentBase src = source as ReportComponentBase;
            if (src == null)
                return;
            Click = src.Click;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected bool DrawIntersectBackground(FRPaintEventArgs e)
        {
            return false;
        }
    }
}
