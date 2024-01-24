using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Barcode;

namespace FastReport.Import.DevExpress
{
    /// <summary>
    /// The DevExpress units converter.
    /// </summary>
    public static class UnitsConverter
    {
        public static float Ratio = 1;

        #region Public Methods

        /// <summary>
        /// Converts SizeF to pixels.
        /// </summary>
        /// <param name="str">SizeF value as string.</param>
        /// <returns>The value in pixels.</returns>
        public static float SizeFToPixels(string str)
        {
            float value = 0.0f;
            if (!String.IsNullOrEmpty(str))
            {
                int end = str.IndexOf("F");
                if (end > -1)
                {
                    value = Convert.ToSingle(str.Substring(0, end), CultureInfo.InvariantCulture);
                }
                else
                {
                    value = Convert.ToSingle(str, CultureInfo.InvariantCulture);
                }
            }
            return value / Ratio;
        }

        /// <summary>
        /// Converts SizeF to pixels.
        /// </summary>
        /// <param name="str">SizeF value as string.</param>
        /// <returns>The value in pixels.</returns>
        /// <remarks>
        /// Use this method for fonts, because font size is not stored as multiplied by dpi
        /// </remarks>
        public static float SizeFToPixelsFont(string str)
        {
            return SizeFToPixels(str) * Ratio;
        }

        /// <summary>
        /// Converts value to Boolean.
        /// </summary>
        /// <param name="str">Boolen value as string.</param>
        public static bool ConvertBool(string str)
        {
            return str.ToLower() == "true";
        }

        /// <summary>
        /// Converts DevExpress Color.
        /// </summary>
        /// <param name="str">The DevExpress Color value as string.</param>
        /// <returns>The Color value.</returns>
        public static Color ConvertColor(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                if (str.IndexOf("FromArgb") > -1)
                {
                    int start = str.IndexOf("byte") + 6;
                    int red = Convert.ToInt32(str.Substring(start, str.IndexOf(")", start) - start));
                    start = str.IndexOf("byte", start) + 6;
                    int green = Convert.ToInt32(str.Substring(start, str.IndexOf(")", start) - start));
                    start = str.IndexOf("byte", start) + 6;
                    int blue = Convert.ToInt32(str.Substring(start, str.IndexOf(")", start) - start));
                    return Color.FromArgb(red, green, blue);
                }
                else if (str.Split(',').Length == 4)
                {
                    string[] colors = str.Split(',');
                    int alpha = Convert.ToInt32(colors[0]);
                    int red = Convert.ToInt32(colors[1]);
                    int green = Convert.ToInt32(colors[2]);
                    int blue = Convert.ToInt32(colors[3]);
                    return Color.FromArgb(alpha, red, green, blue);
                }
                else
                {
                    return Color.FromName(str.Replace("System.Drawing.Color.", ""));
                }
            }
            return Color.Black;
        }

        /// <summary>
        /// Converts DevExpress BackColor.
        /// </summary>
        /// <param name="str">The DevExpress BackColor value as string.</param>
        /// <returns>The Color value.</returns>
        public static Color ConvertBackColor(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                if (str.IndexOf("FromArgb") > -1)
                {
                    int start = str.IndexOf("byte") + 6;
                    int red = Convert.ToInt32(str.Substring(start, str.IndexOf(")", start) - start));
                    start = str.IndexOf("byte", start) + 6;
                    int green = Convert.ToInt32(str.Substring(start, str.IndexOf(")", start) - start));
                    start = str.IndexOf("byte", start) + 6;
                    int blue = Convert.ToInt32(str.Substring(start, str.IndexOf(")", start) - start));
                    return Color.FromArgb(red, green, blue);
                }
                else if (str.Split(',').Length == 4)
                {
                    string[] colors = str.Split(',');
                    int alpha = Convert.ToInt32(colors[0]);
                    int red = Convert.ToInt32(colors[1]);
                    int green = Convert.ToInt32(colors[2]);
                    int blue = Convert.ToInt32(colors[3]);
                    return Color.FromArgb(alpha, red, green, blue);
                }
                else
                {
                    return Color.FromName(str.Replace("System.Drawing.Color.", ""));
                }
            }
            return Color.Transparent;
        }

        /// <summary>
        /// Converts the DevExpress BorderDashStyle to LineStyle.
        /// </summary>
        /// <param name="borderDashStyle">The DevExpress BorderDashStyle value.</param>
        /// <returns>The LineStyle value.</returns>
        public static LineStyle ConvertBorderDashStyle(string borderDashStyle)
        {
            if (borderDashStyle == "DevExpress.XtraPrinting.BorderDashStyle.Dot" || borderDashStyle.Equals("Dot"))
            {
                return LineStyle.Dot;
            }
            else if (borderDashStyle == "DevExpress.XtraPrinting.BorderDashStyle.Dash" || borderDashStyle.Equals("Dash"))
            {
                return LineStyle.Dash;
            }
            else if (borderDashStyle == "DevExpress.XtraPrinting.BorderDashStyle.DashDot" || borderDashStyle.Equals("DashDot"))
            {
                return LineStyle.DashDot;
            }
            else if (borderDashStyle == "DevExpress.XtraPrinting.BorderDashStyle.DashDotDot" || borderDashStyle.Equals("DashDotDot"))
            {
                return LineStyle.DashDotDot;
            }
            else if (borderDashStyle == "DevExpress.XtraPrinting.BorderDashStyle.Double" || borderDashStyle.Equals("Double"))
            {
                return LineStyle.Double;
            }
            return LineStyle.Solid;
        }

        /// <summary>
        /// Converts the DevExpress LineStyle to LineStyle.
        /// </summary>
        /// <param name="lineStyle">The DevExpress LineStyle value.</param>
        /// <returns>The LineStyle value.</returns>
        public static LineStyle ConvertLineStyle(string lineStyle)
        {
            if (lineStyle == "System.Drawing.Drawing2D.DashStyle.Dot")
            {
                return LineStyle.Dot;
            }
            else if (lineStyle == "System.Drawing.Drawing2D.DashStyle.Dash")
            {
                return LineStyle.Dash;
            }
            else if (lineStyle == "System.Drawing.Drawing2D.DashStyle.DashDot")
            {
                return LineStyle.DashDot;
            }
            else if (lineStyle == "System.Drawing.Drawing2D.DashStyle.DashDotDot")
            {
                return LineStyle.DashDotDot;
            }
            else if (lineStyle == "System.Drawing.Drawing2D.DashStyle.Double")
            {
                return LineStyle.Double;
            }
            return LineStyle.Solid;
        }

        /// <summary>
        /// Converts the DevExpress TextAlignment to HorzAlignment.
        /// </summary>
        /// <param name="textAlignment">The DevExpress TextAlignment value.</param>
        /// <returns>The HorzAlign value.</returns>
        public static HorzAlign ConvertTextAlignmentToHorzAlign(string textAlignment)
        {
            if (textAlignment.Contains("Center"))
            {
                return HorzAlign.Center;
            }
            if (textAlignment.Contains("Justify"))
            {
                return HorzAlign.Justify;
            }
            if (textAlignment.Contains("Right"))
            {
                return HorzAlign.Right;
            }
            return HorzAlign.Left;
        }

        /// <summary>
        /// Converts the DevExpress TextAlignment to VertAlignment.
        /// </summary>
        /// <param name="textAlignment">The DevExpress TextAlignment value.</param>
        /// <returns>The VertAlign value.</returns>
        public static VertAlign ConvertTextAlignmentToVertAlign(string textAlignment)
        {
            if (textAlignment.Contains("Middle"))
            {
                return VertAlign.Center;
            }
            if (textAlignment.Contains("Bottom"))
            {
                return VertAlign.Bottom;
            }
            return VertAlign.Top;
        }

        /// <summary>
        /// Converts the DevExpress ImageSizeMode to PictureBoxSizeMode.
        /// </summary>
        /// <param name="sizeMode">The ImageSizeMode value as string.</param>
        /// <returns>The PictureBoxSizeMode value.</returns>
        public static PictureBoxSizeMode ConvertImageSizeMode(string sizeMode)
        {
            if (sizeMode == "DevExpress.XtraPrinting.ImageSizeMode.StretchImage" || sizeMode == "StretchImage")
            {
                return PictureBoxSizeMode.StretchImage;
            }
            else if (sizeMode == "DevExpress.XtraPrinting.ImageSizeMode.AutoSize" || sizeMode == "AutoSize")
            {
                return PictureBoxSizeMode.AutoSize;
            }
            else if (sizeMode == "DevExpress.XtraPrinting.ImageSizeMode.CenterImage" || sizeMode == "CenterImage")
            {
                return PictureBoxSizeMode.CenterImage;
            }
            else if (sizeMode == "DevExpress.XtraPrinting.ImageSizeMode.ZoomImage" || sizeMode == "ZoomImage")
            {
                return PictureBoxSizeMode.Zoom;
            }
            else if (sizeMode == "DevExpress.XtraPrinting.ImageSizeMode.Squeeze" || sizeMode == "Squeeze")
            {
                return PictureBoxSizeMode.Zoom;
            }
            return PictureBoxSizeMode.Normal;
        }

        internal static ImageAlign ConvertImageAlignment(string alignment)
        {
            ImageAlign align = ImageAlign.None;
            switch (alignment)
            {
                case "TopCenter":
                    return ImageAlign.Top_Center;
                case "TopLeft":
                    return ImageAlign.Top_Left;
                case "TopRight":
                    return ImageAlign.Top_Right;
                case "CenterLeft":
                    return ImageAlign.Center_Left;
                case "CenterCenter":
                    return ImageAlign.Center_Center;
                case "CenterRight":
                    return ImageAlign.Center_Right;
                case "BottomLeft":
                    return ImageAlign.Bottom_Left;
                case "BottomCenter":
                    return ImageAlign.Bottom_Center;
                case "BottomRight":
                    return ImageAlign.Bottom_Right;
            }
            return align;
        }

        /// <summary>
        /// Converts the DevExpress Shape to ShapeKind.
        /// </summary>
        /// <param name="shape">The DevExpress Shape value as string.</param>
        /// <returns>The ShapeKind value.</returns>
        public static ShapeKind ConvertShape(string shape)
        {
            if (shape.Contains("Rectangle"))
            {
                return ShapeKind.Rectangle;
            }
            else if (shape.Contains("Polygon"))
            {
                return ShapeKind.Triangle;
            }
            return ShapeKind.Ellipse;
        }

        /// <summary>
        /// Converts the DevExpress Barcode.Symbology to Barcode.Barcode.
        /// </summary>
        /// <param name="symbology">The DevExpress Barcode.Symbology value as string.</param>
        /// <param name="barcode">The BarcodeObject instance.</param>
        public static void ConvertBarcodeSymbology(string symbology, BarcodeObject barcode)
        {
            symbology = symbology.ToLower();
            if (symbology.Contains("codabar"))
            {
                barcode.Barcode = new BarcodeCodabar();
            }
            else if (symbology.Contains("code128"))
            {
                barcode.Barcode = new Barcode128();
            }
            else if (symbology.Contains("code39"))
            {
                barcode.Barcode = new Barcode39();
            }
            else if (symbology.Contains("code39extended"))
            {
                barcode.Barcode = new Barcode39Extended();
            }
            else if (symbology.Contains("code93"))
            {
                barcode.Barcode = new Barcode93();
            }
            else if (symbology.Contains("code9eextended"))
            {
                barcode.Barcode = new Barcode93Extended();
            }
            else if (symbology.Contains("codemsi"))
            {
                barcode.Barcode = new BarcodeMSI();
            }
            else if (symbology.Contains("datamatrix"))
            {
                barcode.Barcode = new BarcodeDatamatrix();
            }
            else if (symbology.Contains("ean128"))
            {
                barcode.Barcode = new BarcodeEAN128();
            }
            else if (symbology.Contains("ean13"))
            {
                barcode.Barcode = new BarcodeEAN13();
            }
            else if (symbology.Contains("ean8"))
            {
                barcode.Barcode = new BarcodeEAN8();
            }
            else if (symbology.Contains("industrial2of5"))
            {
                barcode.Barcode = new Barcode2of5Industrial();
            }
            else if (symbology.Contains("interleaved2of5"))
            {
                barcode.Barcode = new Barcode2of5Interleaved();
            }
            else if (symbology.Contains("matrix2of5"))
            {
                barcode.Barcode = new Barcode2of5Matrix();
            }
            else if (symbology.Contains("pdf417"))
            {
                barcode.Barcode = new BarcodePDF417();
            }
            else if (symbology.Contains("postnet"))
            {
                barcode.Barcode = new BarcodePostNet();
            }
            else if (symbology.Contains("qrcode"))
            {
                barcode.Barcode = new BarcodeQR();
            }
            else if (symbology.Contains("upca"))
            {
                barcode.Barcode = new BarcodeUPC_A();
            }
            else if (symbology.Contains("upce0"))
            {
                barcode.Barcode = new BarcodeUPC_E0();
            }
            else if (symbology.Contains("upce1"))
            {
                barcode.Barcode = new BarcodeUPC_E1();
            }
            else if (symbology.Contains("upcsupplemental2"))
            {
                barcode.Barcode = new BarcodeSupplement2();
            }
            else if (symbology.Contains("upcsupplemental5"))
            {
                barcode.Barcode = new BarcodeSupplement5();
            }
        }

        /// <summary>
        /// Converts the DevExpress border sides to FastReport border sides
        /// </summary>
        /// <param name="sides">The DevExpress Barcode.Symbology value as string.</param>
        /// <param name="border">The BarcodeObject instance.</param>
        public static BorderLines ConvertBorderSides(string sides, Border border)
        {
            BorderLines borderLines = BorderLines.None;
            if (!String.IsNullOrEmpty(sides))
            {
                if (sides.IndexOf("Left") > -1)
                {
                    borderLines |= BorderLines.Left;
                }
                if (sides.IndexOf("Top") > -1)
                {
                    borderLines |= BorderLines.Top;
                }
                if (sides.IndexOf("Right") > -1)
                {
                    borderLines |= BorderLines.Right;
                }
                if (sides.IndexOf("Bottom") > -1)
                {
                    borderLines |= BorderLines.Bottom;
                }
                if (sides.IndexOf("All") > -1)
                {
                    borderLines = BorderLines.All;
                }
            }
            return borderLines;
        }

        #endregion // Public Methods
    }
}
