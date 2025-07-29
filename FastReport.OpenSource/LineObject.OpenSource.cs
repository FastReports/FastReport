using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FastReport
{
    public partial class LineObject
    {
        /// <summary>
        /// Converts it to an picture object if it has cap.
        /// </summary>
        /// <returns>PictureObject</returns>

        public override IEnumerable<Base> GetConvertedObjects()
        {
            PictureObject pictObj = new PictureObject();
            pictObj.SetReport(Report);
            pictObj.Assign(this);
            pictObj.SetParentCore(this.Parent);

            RectangleF rect = CreatePath().GetBounds();
            rect.X -= Border.Width / 2;
            rect.Width += Border.Width;
            rect.Y -= Border.Width / 2;
            rect.Height += Border.Width;
            Bitmap b = new Bitmap((int)Math.Ceiling(rect.Width), (int)Math.Ceiling(rect.Height));
            using (Graphics g = Graphics.FromImage(b))
            {
                g.TranslateTransform(-(int)Math.Ceiling(rect.X), -(int)Math.Ceiling(rect.Y));
                Draw(new FRPaintEventArgs(g, 1, 1, Report.GraphicCache));
            }
            pictObj.Left += rect.Left - pictObj.AbsLeft;
            pictObj.Top += rect.Top - pictObj.AbsTop;
            pictObj.Width = rect.Width;
            pictObj.Height = rect.Height;
            pictObj.Image = b;

            yield return pictObj;
        }
    }
}