using FastReport.Utils;
using System.Drawing;

namespace FastReport.Barcode
{
    /// <summary>
    /// The base class for 2D-barcodes such as PDF417 and Datamatrix.
    /// </summary>
    public abstract class Barcode2DBase : BarcodeBase
    {
        internal float FontHeight => Font.Height * DrawUtils.ScreenDpiFX * 18 / 13; // 18/13 to be more or less compatible with old behavior (Arial,8 with hardcoded 18px height)

        private void DrawBarcode(IGraphics g, float width, float height)
        {
            SizeF originalSize = CalcBounds();
            float kx = width / originalSize.Width;
            float ky = height / originalSize.Height;

            Draw2DBarcode(g, kx, ky);

            //If swiss qr, draw the swiss cross
            if (text.StartsWith("SPC"))
            {
                float top = showText ? height - 21 : height;
                g.FillRectangle(Brushes.White, width / 2 - width / 100f * 7, top / 2 - top / 100 * 7, width / 100f * 14, top / 100 * 14);
                g.FillRectangle(Brushes.Black, width / 2 - width / 100f * 6, top / 2 - top / 100 * 6, width / 100f * 12, top / 100 * 12);
                g.FillRectangle(Brushes.White, width / 2 - width / 100f * 4, top / 2 - top / 100 * 1.5f, width / 100f * 8, top / 100 * 3);
                g.FillRectangle(Brushes.White, width / 2 - width / 100f * 1.5f, top / 2 - top / 100 * 4, width / 100f * 3, top / 100 * 8);
            }
            if (showMarker && text.StartsWith("ST"))
            {
                using (Pen skyBluePen = new Pen(Brushes.Black))
                {
                    skyBluePen.Width = (kx * 4 * zoom) / 2;
                    g.DrawLine(skyBluePen, width - 2, height / 2, width - 2, height - 2);
                    g.DrawLine(skyBluePen, width / 2, height - 2, width - 2, height - 2);
                }
            }
            // draw the text.
            if (showText)
            {
                string data = StripControlCodes(text);
                if (data.Length > 0)
                {
                    // When we print, .Net automatically scales the font. However, we need to handle this process.
                    // Downscale the font to the screen resolution, then scale by required value (ky).
                    float fontHeight = FontHeight;
                    float fontZoom = fontHeight / (int)g.MeasureString(data, Font).Height * ky;
                    using (Font drawFont = new Font(Font.FontFamily, Font.Size * fontZoom, Font.Style))
                    {
                        using (StringFormat format = (StringFormat)StringFormat.GenericDefault.Clone())
                        {
#if !SKIA
                            format.FormatFlags |= StringFormatFlags.NoWrap;
#endif
                            g.DrawString(data, drawFont, Brushes.Black, new RectangleF(0, height - fontHeight * ky, width, fontHeight * ky), format);
                        }
                    }
                }
            }
        }

        internal virtual void Draw2DBarcode(IGraphics g, float kx, float ky)
        {
        }

        /// <inheritdoc/>
        public override void DrawBarcode(IGraphics g, RectangleF displayRect)
        {
            float width = angle == 90 || angle == 270 ? displayRect.Height : displayRect.Width;
            float height = angle == 90 || angle == 270 ? displayRect.Width : displayRect.Height;
            IGraphicsState state = g.Save();
            try
            {
                // rotate
                g.TranslateTransform(displayRect.Left, displayRect.Top);
                g.RotateTransform(angle);

                switch (angle)
                {
                    case 90:
                        g.TranslateTransform(0, -displayRect.Width);
                        break;

                    case 180:
                        g.TranslateTransform(-displayRect.Width, -displayRect.Height);
                        break;

                    case 270:
                        g.TranslateTransform(-displayRect.Height, 0);
                        break;
                }

                DrawBarcode(g, width, height);
            }
            finally
            {
                g.Restore(state);
            }
        }
    }
}
