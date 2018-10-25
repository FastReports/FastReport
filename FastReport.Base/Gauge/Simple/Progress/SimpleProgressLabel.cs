using FastReport.Gauge.Radial;
using FastReport.Utils;
using System;
using System.ComponentModel;
using System.Drawing;

namespace FastReport.Gauge.Simple.Progress
{
    /// <inheritdoc />
    public class SimpleProgressLabel : GaugeLabel
    { 
        private int decimals;

        /// <summary>
        /// Gets or sets the number of fractional digits
        /// </summary>
        public int Decimals
        {
            get { return decimals; }
            set
            {
                if (value < 0)
                    decimals = 0;
                else if (value > 15)
                    decimals = 15;
                else
                    decimals = value;
            }
        }

        /// <inheritdoc />
        [Browsable(false)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <inheritdoc />
        public SimpleProgressLabel(GaugeObject parent) : base(parent)
        {
            Parent = parent as SimpleProgressGauge;
            decimals = 0;
        }

        /// <inheritdoc />
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            float x = (Parent.AbsLeft + Parent.Border.Width / 2) * e.ScaleX;
            float y = (Parent.AbsTop + Parent.Border.Width / 2) * e.ScaleY;
            float dx = (Parent.Width - Parent.Border.Width) * e.ScaleX;
            float dy = (Parent.Height - Parent.Border.Width) * e.ScaleY;

            PointF lblPt = new PointF(x + dx / 2, y + dy/2);
            SizeF txtSize = RadialUtils.GetStringSize(e, Parent, Font, Text);
            Font font = RadialUtils.GetFont(e, Parent, Font);
            Brush brush = e.Cache.GetBrush(Color);
            Text = Math.Round((Parent.Value - Parent.Minimum) / (Parent.Maximum - Parent.Minimum) * 100, decimals) + "%";
            e.Graphics.DrawString(Text, font, brush, lblPt.X - txtSize.Width / 2, lblPt.Y - txtSize.Height / 2);
        }
    }
}
