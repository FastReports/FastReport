using FastReport.Utils;
using System.Drawing;

namespace FastReport.Gauge.Radial
{
    class RadialLabel : GaugeLabel
    {
        public RadialLabel(GaugeObject parent): base(parent)
        {
            Parent = parent as RadialGauge;
        }

        public override void Assign(GaugeLabel src)
        {
            base.Assign(src);
        }

        public override void Serialize(FRWriter writer, string prefix, GaugeLabel diff)
        {
            base.Serialize(writer, prefix, diff);
        }

        public override void Draw(FRPaintEventArgs e)
        {
            if ((Parent as RadialGauge).Type == RadialGaugeType.Circle)
            {
                base.Draw(e);
                float x = (Parent.AbsLeft + Parent.Border.Width / 2) * e.ScaleX;
                float y = (Parent.AbsTop + Parent.Border.Width / 2) * e.ScaleY;
                float dx = (Parent.Width - Parent.Border.Width) * e.ScaleX - 1;
                float dy = (Parent.Height - Parent.Border.Width) * e.ScaleY - 1;

                PointF lblPt = new PointF(x + dx / 2, y + dy - ((Parent.Scale as RadialScale).AvrTick.Y - y));
                SizeF txtSize = RadialUtils.GetStringSize(e, Parent, Font, Text);
                Font font = RadialUtils.GetFont(e, Parent, Font);
                Brush brush = e.Cache.GetBrush(Color);
                e.Graphics.DrawString(Text, font, brush, lblPt.X - txtSize.Width / 2, lblPt.Y - txtSize.Height / 2);
            }
        }
    }
}
