using FastReport.Utils;
using System.Drawing;

namespace FastReport
{
    partial class PictureObjectBase
    {
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="g"></param>
        /// <param name="e"></param>
        protected void DrawErrorImage(Graphics g, FRPaintEventArgs e)
        {

        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="e"></param>
        partial void DrawDesign(FRPaintEventArgs e);
    }
}
