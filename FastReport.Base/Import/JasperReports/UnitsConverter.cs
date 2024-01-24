using System;
using System.Globalization;
using System.Drawing;
using FastReport.Utils;
using FastReport.Barcode;
using System.Xml;
using System.Windows.Forms;
#if MSCHART
using FastReport.DataVisualization.Charting;
#endif

namespace FastReport.Import.JasperReports
{
    /// <summary>
    /// The JasperReports units converter.
    /// </summary>
    public static class UnitsConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts value to Boolean.
        /// </summary>
        /// <param name="str">Boolen value as string.</param>
        public static bool ConvertBool(string str)
        {
            return str.ToLower() == "true";
        }

        /// <summary>
        /// Converts string to PictureBoxSizeMode.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PictureBoxSizeMode ConvertImageSizeMode(string value)
        {
            switch (value)
            {
                case "RetainShape":
                    return PictureBoxSizeMode.Zoom;
                case "Clip":
                    return PictureBoxSizeMode.Normal;
                case "FillFrame":
                    return PictureBoxSizeMode.StretchImage;
                case "RealHeight":
                case "RealSize":
                    return PictureBoxSizeMode.AutoSize;
            }
            return PictureBoxSizeMode.Normal;
        }

        /// <summary>
        /// Parse int value.
        /// </summary>
        /// <param name="strInt"></param>
        /// <returns></returns>
        public static int ConvertInt(string strInt)
        {
            int result = 0;
            int.TryParse(strInt, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result);
            return result;
        }

        /// <summary>
        /// Parse float value
        /// </summary>
        /// <param name="strFloat"></param>
        /// <returns></returns>
        public static float ConvertFloat(string strFloat)
        {
            float result = 0;
            float.TryParse(strFloat, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result);
            return result;
        }

        /// <summary>
        /// Converts JasperReports Color.
        /// </summary>
        /// <param name="str">The DevExpress Color value as string.</param>
        /// <returns>The Color value.</returns>
        public static Color ConvertColor(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                return ColorTranslator.FromHtml(str);
            }
            return Color.Black;
        }

        /// <summary>
        /// Converts string to QRCodeErrorCorrection.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QRCodeErrorCorrection ConvertErrorCorrection(string value)
        {
            switch (value)
            {
                case "M":
                    return QRCodeErrorCorrection.M;
                case "Q":
                    return QRCodeErrorCorrection.Q;
                case "H":
                    return QRCodeErrorCorrection.H;
                case "L":
                    return QRCodeErrorCorrection.L;
                default:
                    return QRCodeErrorCorrection.L;
            }
        }

        /// <summary>
        /// Parse BorderLine from XmlNode.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static BorderLine ConvertBorderLine(XmlNode node)
        {
            BorderLine line = new BorderLine();

            if (node.Attributes["lineWidth"] != null)
                line.Width = ConvertInt(node.Attributes["lineWidth"].Value) * DrawUtils.ScreenDpi / 96f;

            if (node.Attributes["lineColor"] != null)
                line.Color = ConvertColor(node.Attributes["lineColor"].Value);

            if (node.Attributes["lineStyle"] != null)
                line.Style = ConvertLineStyle(node.Attributes["lineStyle"].Value);

            return line;
        }

        /// <summary>
        /// Converts JasperReports Border.
        /// </summary>
        /// <param name="border"></param>
        /// <returns></returns>
        public static Border ConvertBorder(XmlNode border)
        {
            Border result = new Border();

            foreach (XmlNode node in border.ChildNodes)
            {
                if (node.Name == "pen")
                {
                    result.TopLine = ConvertBorderLine(node);
                    result.LeftLine = result.TopLine;
                    result.BottomLine = result.TopLine;
                    result.RightLine = result.TopLine;
                    result.Lines = BorderLines.All;
                }
                if (node.Name == "topPen")
                {
                    result.TopLine = ConvertBorderLine(node);
                    result.Lines |= BorderLines.Top;
                }
                if (node.Name == "leftPen")
                {
                    result.LeftLine = ConvertBorderLine(node);
                    result.Lines |= BorderLines.Left;
                }
                if (node.Name == "bottomPen")
                {
                    result.BottomLine = ConvertBorderLine(node);
                    result.Lines |= BorderLines.Bottom;
                }
                if (node.Name == "rightPen")
                {
                    result.RightLine = ConvertBorderLine(node);
                    result.Lines |= BorderLines.Right;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the JasperReports BorderDashStyle to LineStyle.
        /// </summary>
        /// <param name="borderDashStyle">The DevExpress BorderDashStyle value.</param>
        /// <returns>The LineStyle value.</returns>
        public static LineStyle ConvertBorderDashStyle(string borderDashStyle)
        {
            if (borderDashStyle.Equals("Dot"))
            {
                return LineStyle.Dot;
            }
            else if (borderDashStyle.Equals("Dash"))
            {
                return LineStyle.Dash;
            }
            else if (borderDashStyle.Equals("DashDot"))
            {
                return LineStyle.DashDot;
            }
            else if (borderDashStyle.Equals("DashDotDot"))
            {
                return LineStyle.DashDotDot;
            }
            else if (borderDashStyle.Equals("Double"))
            {
                return LineStyle.Double;
            }
            return LineStyle.Solid;
        }

        /// <summary>
        /// Converts the JasperReports LineStyle to LineStyle.
        /// </summary>
        /// <param name="lineStyle">The JasperReports LineStyle value.</param>
        /// <returns>The LineStyle value.</returns>
        public static LineStyle ConvertLineStyle(string lineStyle)
        {
            if (lineStyle == "Dotted")
            {
                return LineStyle.Dot;
            }
            else if (lineStyle == "Dashed")
            {
                return LineStyle.Dash;
            }
            else if (lineStyle == "Double")
            {
                return LineStyle.Double;
            }
            return LineStyle.Solid;
        }

        /// <summary>
        /// Converts the JasperReports TextAlignment to HorzAlignment.
        /// </summary>
        /// <param name="textAlignment">The JasperReports TextAlignment value.</param>
        /// <returns>The HorzAlign value.</returns>
        public static HorzAlign ConvertTextAlignmentToHorzAlign(string textAlignment)
        {
            if (textAlignment.Contains("Center"))
            {
                return HorzAlign.Center;
            }
            if (textAlignment.Contains("Justified"))
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
        /// Converts the JasperReports rotation to int.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static int ConvertRotation(string rotation)
        {
            switch (rotation)
            {
                case "Right":
                    return 90;
                case "Left":
                    return 270;
                case "UpsideDown":
                    return 180;
            }
            return 0;
        }

        /// <summary>
        /// Converts the JasperReports TextAlignment to VertAlignment.
        /// </summary>
        /// <param name="textAlignment">The JasperReports TextAlignment value.</param>
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
        /// Converts the JasperReports Barcode.Symbology to Barcode.Barcode.
        /// </summary>
        /// <param name="symbology">The JasperReports Barcode.Symbology value as string.</param>
        /// <param name="barcode">The BarcodeObject instance.</param>
        public static void ConvertBarcodeSymbology(string symbology, BarcodeObject barcode)
        {
            switch (symbology)
            {
                case "ean128":
                    barcode.Barcode = new BarcodeEAN128();
                    break;
                case "pdf417":
                    barcode.Barcode = new BarcodePDF417();
                    break;
                case "ean13":
                    barcode.Barcode = new BarcodeEAN13();
                    break;
                case "uspsintelligentmail":
                case "usps":
                    barcode.Barcode = new BarcodeIntelligentMail();
                    break;
                case "code128a":
                case "code128b":
                case "code128c":
                case "code128":
                    barcode.Barcode = new Barcode128();
                    break;
                case "code39":
                    barcode.Barcode = new Barcode39();
                    break;
                case "code39 (extended)":
                    barcode.Barcode = new Barcode39Extended();
                    break;
                case "codabar":
                    barcode.Barcode = new BarcodeCodabar();
                    break;
                case "upca":
                    barcode.Barcode = new BarcodeUPC_A();
                    break;
                case "interleaved2of5":
                case "int2of5":
                    barcode.Barcode = new Barcode2of5Interleaved();
                    break;
                case "std2of5":
                    barcode.Barcode = new Barcode2of5Industrial();
                    break;
                case "postnet":
                    barcode.Barcode = new BarcodePostNet();
                    break;
                case "upce":
                    barcode.Barcode = new BarcodeUPC_E0();
                    break;
                case "datamatrix":
                    barcode.Barcode = new BarcodeDatamatrix();
                    break;
                case "ean8":
                    barcode.Barcode = new BarcodeEAN8();
                    break;
                default:
                    barcode.Barcode = new Barcode39();
                    break;
            }
        }

        /// <summary>
        /// Convert JasperReports HyperlinkType to HyperlinkKind.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HyperlinkKind ConvertHyperlinkType(XmlNode node)
        {
            if (node.Attributes["hyperlinkType"] != null)
            {
                switch (node.Attributes["hyperlinkType"].Value)
                {
                    case "LocalAnchor":
                        return HyperlinkKind.Bookmark;
                    case "LocalPage":
                        return HyperlinkKind.PageNumber;
                    case "Reference":
                        return HyperlinkKind.URL;
                }
            }
            return HyperlinkKind.Custom;
        }

#if MSCHART
        /// <summary>
        /// Converts the JasperReports SeriesChartType to SeriesChartType.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SeriesChartType ConvertChartType(string value)
        {
            switch (value)
            {
                case "pie3DChart":
                case "pieChart":
                    return SeriesChartType.Pie;
                case "stackedBar3DChart":
                case "stackedBarChart":
                    return SeriesChartType.StackedColumn;
                case "xyBarChart":
                case "bar3DChart":
                case "barChart":
                    return SeriesChartType.Column;
                case "scatterChart":
                case "xyLineChart":
                case "lineChart":
                case "timeSeriesChart":
                    return SeriesChartType.Line;
                case "c:spiderChart":
                    return SeriesChartType.Radar;
                case "xyAreaChart":
                case "areaChart":
                    return SeriesChartType.Area;
                case "highLowChart":
                    return SeriesChartType.RangeColumn;
                case "candlestickChart":
                    return SeriesChartType.Candlestick;
                case "stackedAreaChart":
                    return SeriesChartType.StackedArea;
                case "bubbleChart":
                    return SeriesChartType.Bubble;
                case "thermometerChart":
                case "meterChart":
                case "ganttChart":
                case "multiAxisPlot":
                default:
                    return SeriesChartType.Column;
            }
        }
#endif

        #endregion // Public Methods
    }
}
