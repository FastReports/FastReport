using FastReport.Utils;
using System.Drawing;

namespace FastReport
{
    partial class BandBase
    {
        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            DrawBackground(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
        }
    }
}
