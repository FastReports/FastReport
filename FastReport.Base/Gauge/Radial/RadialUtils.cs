using FastReport.Utils;
using System;
using System.Drawing;

namespace FastReport.Gauge.Radial
{
     internal class RadialUtils
    {
        public static Font GetFont(FRPaintEventArgs e, GaugeObject gauge, Font font)
        {
            return e.Cache.GetFont(font.Name, gauge.IsPrinting ? font.Size : font.Size * e.ScaleX * 96f / DrawUtils.ScreenDpi, font.Style);
        }
        public static SizeF GetStringSize(FRPaintEventArgs e, GaugeObject gauge, Font font, string text)
        {
            Graphics g = e.Graphics;
            return g.MeasureString(text, GetFont(e, gauge, font));
        }

        public static PointF[] RotateVector(PointF[] vector, double angle, PointF center)
        {
            PointF[] rotatedVector = new PointF[2];
            rotatedVector[0].X = (float)(center.X + (vector[0].X - center.X) * Math.Cos(angle) + (center.Y - vector[0].Y) * Math.Sin(angle));
            rotatedVector[0].Y = (float)(center.Y + (vector[0].X - center.X) * Math.Sin(angle) + (vector[0].Y - center.Y) * Math.Cos(angle));
            rotatedVector[1].X = (float)(center.X + (vector[1].X - center.X) * Math.Cos(angle) + (center.Y - vector[1].Y) * Math.Sin(angle));
            rotatedVector[1].Y = (float)(center.Y + (vector[1].X - center.X) * Math.Sin(angle) + (vector[1].Y - center.Y) * Math.Cos(angle));
            return rotatedVector;
        }

         public static bool IsTop(GaugeObject radialGauge)
        {
            return ((radialGauge as RadialGauge).Position & RadialGaugePosition.Top) != 0;
        }
         public static bool IsBottom(GaugeObject radialGauge)
        {
            return ((radialGauge as RadialGauge).Position & RadialGaugePosition.Bottom) != 0;
        }
         public static bool IsLeft(GaugeObject radialGauge)
        {
            return ((radialGauge as RadialGauge).Position & RadialGaugePosition.Left) != 0;
        }
         public static bool IsRight(GaugeObject radialGauge)
        {
            return ((radialGauge as RadialGauge).Position & RadialGaugePosition.Right) != 0;
        }
         public static bool IsSemicircle(GaugeObject radialGauge)
        {
            return ((radialGauge as RadialGauge).Type & RadialGaugeType.Semicircle) != 0;
        }
        public static bool IsQuadrant(GaugeObject radialGauge)
        {
            return ((radialGauge as RadialGauge).Type & RadialGaugeType.Quadrant) != 0;
        }
    }
}
