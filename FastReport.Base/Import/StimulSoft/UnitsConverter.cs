using System;
using System.Globalization;
using System.Drawing;
using FastReport.Utils;
using FastReport.Barcode;
using System.Drawing.Drawing2D;
using FastReport.Format;
using System.Xml;
#if MSCHART
using FastReport.DataVisualization.Charting;
#endif
using FastReport.Matrix;

namespace FastReport.Import.StimulSoft
{
    /// <summary>
    /// The StimulSoft units converter.
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
        /// Converts value to PageUnits.
        /// </summary>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public static float GetPixelsInUnit(PageUnits unitType)
        {
            switch (unitType)
            {
                case PageUnits.Centimeters:
                    return Units.Centimeters;
                case PageUnits.Inches:
                    return Units.Inches;
                case PageUnits.HundrethsOfInch:
                    return Units.HundrethsOfInch;
                default:
                    return Units.Millimeters;
            }
        }

        /// <summary>
        /// Converts value to PageUnits.
        /// </summary>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public static PageUnits ConverPageUnits(string unitType)
        {
            switch (unitType)
            {
                case "Centimeters":
                    return PageUnits.Centimeters;
                case "Inches":
                    return PageUnits.Inches;
                case "HundredthsOfInch":
                    return PageUnits.HundrethsOfInch;
                default:
                    return PageUnits.Millimeters;
            }
        }

        /// <summary>
        /// Converts the PaperSize to width and height values of paper size in millimeters
        /// </summary>
        /// <param name="paperSize">The PaperSize value.</param>
        /// <param name="page">The ReportPage instance.</param>
        public static void ConvertPaperSize(string paperSize, ReportPage page)
        {
            if (page == null)
                return;

            float width = 210;
            float height = 297;
            switch (paperSize)
            {
                case "A4":
                    width = 210;
                    height = 297;
                    break;
                case "A3":
                    width = 297;
                    height = 420;
                    break;
                case "A5":
                    width = 148;
                    height = 210;
                    break;
                case "B4":
                    width = 257;
                    height = 364;
                    break;
                case "B5":
                    width = 182;
                    height = 257;
                    break;
                case "Legal":
                    width = 216;
                    height = 356;
                    break;
                case "Letter":
                    width = 216;
                    height = 279;
                    break;
                case "Tabloid":
                    width = 432;
                    height = 279;
                    break;
                case "Statement":
                    width = 140;
                    height = 216;
                    break;
                case "Executive":
                    width = 184;
                    height = 267;
                    break;
                default:
                    width = 210;
                    height = 297;
                    break;
            }

            page.PaperWidth = width;
            page.PaperHeight = height;
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
        /// Converts StimulSoft Color.
        /// </summary>
        /// <param name="str">The DevExpress Color value as string.</param>
        /// <returns>The Color value.</returns>
        public static Color ConvertColor(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                if (str.Contains("["))
                {
                    string[] rgb = str.Replace("[", "").Replace("]", "").Split(':');

                    if (rgb.Length > 3)
                        return Color.FromArgb(ConvertInt(rgb[0]), ConvertInt(rgb[1]), ConvertInt(rgb[2]), ConvertInt(rgb[3]));

                    return Color.FromArgb(ConvertInt(rgb[0]), ConvertInt(rgb[1]), ConvertInt(rgb[2]));
                }
                else if (str.Contains(","))
                {
                    string[] rgb = str.Replace(" ", "").Split(',');

                    if (rgb.Length > 3)
                        return Color.FromArgb(ConvertInt(rgb[0]), ConvertInt(rgb[1]), ConvertInt(rgb[2]), ConvertInt(rgb[3]));

                    return Color.FromArgb(ConvertInt(rgb[0]), ConvertInt(rgb[1]), ConvertInt(rgb[2]));
                }
                else
                    return Color.FromName(str);
            }
            return Color.Black;
        }

        /// <summary>
        /// Converts StimulSoft Border.
        /// </summary>
        /// <param name="border"></param>
        /// <returns></returns>
        public static Border ConvertBorder(string border)
        {
            Border result = new Border();
            string[] parametrs = border.Split(';');
            int indexParam = 0;
            if (border.StartsWith("Adv"))
            {
                parametrs = border.Remove(0, 3).Split(';');
                result.SimpleBorder = false;

                for (int i = 0; i < 4; i++)
                {
                    BorderLine line = new BorderLine();

                    line.Color = ConvertColor(parametrs[indexParam]);
                    indexParam++;
                    line.Width = ConvertInt(parametrs[indexParam]);
                    indexParam++;

                    if (parametrs[indexParam] == "None")
                    {
                        indexParam++;
                        continue;
                    }
                    else
                    {
                        line.Style = ConvertBorderDashStyle(parametrs[indexParam]);
                        indexParam++;
                    }

                    switch (i)
                    {
                        case 0:
                            result.TopLine = line;
                            result.Lines |= BorderLines.Top;
                            break;
                        case 1:
                            result.BottomLine = line;
                            result.Lines |= BorderLines.Bottom;
                            break;
                        case 2:
                            result.LeftLine = line;
                            result.Lines |= BorderLines.Left;
                            break;
                        case 3:
                            result.RightLine = line;
                            result.Lines |= BorderLines.Right;
                            break;
                    }
                }
            }
            else
            {
                result.Lines = ConvertBorderSides(parametrs[indexParam]);
                indexParam++;
                result.Color = ConvertColor(parametrs[indexParam]);
                indexParam++;
                result.Width = ConvertInt(parametrs[indexParam]);
                indexParam++;
                result.Style = ConvertBorderDashStyle(parametrs[indexParam]);
                indexParam++;
            }

            result.Shadow = ConvertBool(parametrs[indexParam]);
            indexParam++;
            result.ShadowWidth = ConvertInt(parametrs[indexParam]);
            indexParam++;
            result.ShadowColor = ConvertColor(parametrs[indexParam]);


            return result;
        }

        /// <summary>
        /// Converts the StimulSoft BorderDashStyle to LineStyle.
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

#if MSCHART
        /// <summary>
        /// Converts the StimulSoft BorderDashStyle to LineStyle.
        /// </summary>
        /// <param name="chartDashStyle">The DevExpress BorderDashStyle value.</param>
        /// <returns>The LineStyle value.</returns>
        public static ChartDashStyle ConvertBorderChartDashStyle(string chartDashStyle)
        {
            if (chartDashStyle.Equals("Dot"))
            {
                return ChartDashStyle.Dot;
            }
            else if (chartDashStyle.Equals("Dash"))
            {
                return ChartDashStyle.Dash;
            }
            else if (chartDashStyle.Equals("DashDot"))
            {
                return ChartDashStyle.DashDot;
            }
            else if (chartDashStyle.Equals("DashDotDot"))
            {
                return ChartDashStyle.DashDotDot;
            }
            else if (chartDashStyle.Equals("Double"))
            {
                return ChartDashStyle.NotSet;
            }
            return ChartDashStyle.Solid;
        }
#endif

        /// <summary>
        /// Converts the StimulSoft LineStyle to LineStyle.
        /// </summary>
        /// <param name="lineStyle">The StimulSoft LineStyle value.</param>
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
        /// Converts the StimulSoft TextAlignment to HorzAlignment.
        /// </summary>
        /// <param name="textAlignment">The StimulSoft TextAlignment value.</param>
        /// <returns>The HorzAlign value.</returns>
        public static HorzAlign ConvertTextAlignmentToHorzAlign(string textAlignment)
        {
            if (textAlignment.Contains("Center"))
            {
                return HorzAlign.Center;
            }
            if (textAlignment.Contains("Width"))
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
        /// Converts the StimulSoft Brush to FillBase object.
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static FillBase ConvertBrush(string brush)
        {
            string[] parametrs = brush.Split(',');

            if (brush.Contains("HatchBrush"))
            {
                HatchFill fill = new HatchFill();
                fill.ForeColor = ConvertColor(parametrs[2]);
                fill.BackColor = ConvertColor(parametrs[3]);
                foreach (HatchStyle style in Enum.GetValues(typeof(HatchStyle)))
                {
                    if (style.ToString() == parametrs[1])
                    {
                        fill.Style = style;
                        break;
                    }
                }
                return fill;
            }

            if (brush.Contains("GradientBrush"))
            {
                return new LinearGradientFill(ConvertColor(parametrs[2]), ConvertColor(parametrs[1]), ConvertInt(parametrs[3]));
            }

            if (brush.Contains("GlareBrush"))
            {
                return new LinearGradientFill(ConvertColor(parametrs[1]), ConvertColor(parametrs[2]), ConvertInt(parametrs[3]), ConvertFloat(parametrs[4]), 100);
            }

            if (brush.Contains("GlassBrush"))
            {
                return new GlassFill(ConvertColor(parametrs[1]), ConvertFloat(parametrs[3]), ConvertBool(parametrs[2]));
            }

            return new SolidFill(ConvertColor(brush));
        }

        /// <summary>
        ///  Converts the StimulSoft Format to FormatBase object.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static FormatBase ConvertFormat(XmlNode node)
        {
            switch (node.Attributes["type"].Value)
            {
                case "NumberFormat":
                    NumberFormat numberFormat = new NumberFormat();
                    if (node["GroupSeparator"] != null)
                        numberFormat.GroupSeparator = node["GroupSeparator"].InnerText;
                    if (node["NegativePattern"] != null)
                        numberFormat.NegativePattern = ConvertInt(node["NegativePattern"].InnerText);
                    if (node["DecimalDigits"] != null)
                        numberFormat.DecimalDigits = ConvertInt(node["DecimalDigits"].InnerText);
                    if (node["DecimalSeparator"] != null)
                        numberFormat.DecimalSeparator = node["DecimalSeparator"].InnerText;
                    return numberFormat;

                case "CurrencyFormat":
                    CurrencyFormat currencyFormat = new CurrencyFormat();
                    if (node["GroupSeparator"] != null)
                        currencyFormat.GroupSeparator = node["GroupSeparator"].InnerText;
                    if (node["NegativePattern"] != null)
                        currencyFormat.NegativePattern = ConvertInt(node["NegativePattern"].InnerText);
                    if (node["DecimalDigits"] != null)
                        currencyFormat.DecimalDigits = ConvertInt(node["DecimalDigits"].InnerText);
                    if (node["DecimalSeparator"] != null)
                        currencyFormat.DecimalSeparator = node["DecimalSeparator"].InnerText;
                    if (node["PositivePattern"] != null)
                        currencyFormat.PositivePattern = ConvertInt(node["PositivePattern"].InnerText);
                    if (node["Symbol"] != null)
                        currencyFormat.CurrencySymbol = node["Symbol"].InnerText;
                    return currencyFormat;

                case "PercentageFormat":
                    PercentFormat percentFormat = new PercentFormat();
                    if (node["GroupSeparator"] != null)
                        percentFormat.GroupSeparator = node["GroupSeparator"].InnerText;
                    if (node["NegativePattern"] != null)
                        percentFormat.NegativePattern = ConvertInt(node["NegativePattern"].InnerText);
                    if (node["DecimalDigits"] != null)
                        percentFormat.DecimalDigits = ConvertInt(node["DecimalDigits"].InnerText);
                    if (node["DecimalSeparator"] != null)
                        percentFormat.DecimalSeparator = node["DecimalSeparator"].InnerText;
                    if (node["PositivePattern"] != null)
                        percentFormat.PositivePattern = ConvertInt(node["PositivePattern"].InnerText);
                    return percentFormat;

                case "DateFormat":
                    DateFormat dataFormats = new DateFormat();
                    if (node["StringFormat"] != null)
                        dataFormats.Format = node["StringFormat"].InnerText;
                    return dataFormats;

                case "TimeFormat":
                    TimeFormat timeFormats = new TimeFormat();
                    if (node["StringFormat"] != null)
                        timeFormats.Format = node["StringFormat"].InnerText;
                    return timeFormats;

                case "BooleanFormat":
                    BooleanFormat booleanFormat = new BooleanFormat();
                    if (node["FalseDisplay"] != null)
                        booleanFormat.FalseText = node["FalseDisplay"].InnerText;
                    if (node["TrueDisplay"] != null)
                        booleanFormat.TrueText = node["TrueDisplay"].InnerText;
                    return booleanFormat;

                case "CustomFormat":
                    CustomFormat customFormat = new CustomFormat();
                    customFormat.Format = node["StringFormat"].InnerText;
                    return customFormat;
            }

            return new GeneralFormat();
        }


        /// <summary>
        /// Converts the StimulSoft RTF string to raw RTF.
        /// </summary>
        /// <param name="rtfStimulsoft"></param>
        /// <returns></returns>
        public static string ConvertRTF(string rtfStimulsoft)
        {
            string result = rtfStimulsoft.Replace("__LP__", "{").Replace("__RP__", "}");
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] == '_')
                    try
                    {
                        result = result.Replace(result.Substring(i, 7), ((char)Int16.Parse(result.Substring(i + 2, 4), NumberStyles.AllowHexSpecifier)).ToString());
                    }
                    catch { }
            }

            return result;
        }


        /// <summary>
        /// Converts the StimulSoft CapStyle to CapStyle.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static CapStyle ConvertCapStyle(string style)
        {
            switch (style)
            {
                case "Diamond":
                    return CapStyle.Diamond;
                case "Open":
                    return CapStyle.Arrow;
                case "Square":
                    return CapStyle.Square;
                case "Oval":
                    return CapStyle.Circle;
                default:
                    return CapStyle.Arrow;
            }
        }

        /// <summary>
        /// Converts the StimulSoft TextAlignment to VertAlignment.
        /// </summary>
        /// <param name="textAlignment">The StimulSoft TextAlignment value.</param>
        /// <returns>The VertAlign value.</returns>
        public static VertAlign ConvertTextAlignmentToVertAlign(string textAlignment)
        {
            if (textAlignment.Contains("Center"))
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
        /// Converts the StimulSoft CheckedSymbol to CheckedSymbol.
        /// </summary>
        /// <param name="checksymbol"></param>
        /// <returns></returns>
        public static CheckedSymbol ConvertCheckSymbol(string checksymbol)
        {
            switch (checksymbol)
            {
                case "Cross":
                    return CheckedSymbol.Cross;
                case "DotRectangle":
                    return CheckedSymbol.Fill;
                default:
                    return CheckedSymbol.Check;
            }
        }

        /// <summary>
        /// Converts the StimulSoft Barcode.Symbology to Barcode.Barcode.
        /// </summary>
        /// <param name="symbology">The StimulSoft Barcode.Symbology value as string.</param>
        /// <param name="barcode">The BarcodeObject instance.</param>
        public static void ConvertBarcodeSymbology(string symbology, BarcodeObject barcode)
        {
            switch (symbology)
            {
                case "Stimulsoft.Report.BarCodes.StiEAN128bBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiEAN128cBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiEAN128AutoBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiEAN128aBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiGS1_128BarCodeType":
                    barcode.Barcode = new BarcodeEAN128();
                    break;
                case "Stimulsoft.Report.BarCodes.StiDataMatrixBarCodeType":
                    barcode.Barcode = new BarcodeDatamatrix();
                    break;
                case "Stimulsoft.Report.BarCodes.StiQRCodeBarCodeType":
                    barcode.Barcode = new BarcodeQR();
                    break;
                case "Stimulsoft.Report.BarCodes.StiMaxicodeBarCodeType":
                    barcode.Barcode = new BarcodeMaxiCode();
                    break;
                case "Stimulsoft.Report.BarCodes.StiPdf417BarCodeType":
                    barcode.Barcode = new BarcodePDF417();
                    break;
                case "Stimulsoft.Report.BarCodes.StiAztecBarCodeType":
                    barcode.Barcode = new BarcodeAztec();
                    break;
                case "Stimulsoft.Report.BarCodes.StiEAN8BarCodeType":
                    barcode.Barcode = new BarcodeEAN8();
                    break;
                case "Stimulsoft.Report.BarCodes.StiUpcABarCodeType":
                    barcode.Barcode = new BarcodeUPC_A();
                    break;
                case "Stimulsoft.Report.BarCodes.StiITF14BarCodeType":
                    barcode.Barcode = new BarcodeITF14();
                    break;
                case "Stimulsoft.Report.BarCodes.StiAustraliaPost4StateBarCodeType":
                    break;
                case "Stimulsoft.Report.BarCodes.StiIntelligentMail4StateBarCodeType":
                    barcode.Barcode = new BarcodeIntelligentMail();
                    break;
                case "Stimulsoft.Report.BarCodes.StiCode128aBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiCode128bBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiCode128cBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiCode128AutoBarCodeType":
                    barcode.Barcode = new Barcode128();
                    break;
                case "Stimulsoft.Report.BarCodes.StiCode39BarCodeType":
                    barcode.Barcode = new Barcode39();
                    break;
                case "Stimulsoft.Report.BarCodes.StiCode39ExtBarCodeType":
                    barcode.Barcode = new Barcode39Extended();
                    break;
                case "Stimulsoft.Report.BarCodes.StiCode93BarCodeType":
                    barcode.Barcode = new Barcode93();
                    break;
                case "Stimulsoft.Report.BarCodes.StiCode93ExtBarCodeType":
                    barcode.Barcode = new Barcode93Extended();
                    break;
                case "Stimulsoft.Report.BarCodes.StiCodabarBarCodeType":
                    barcode.Barcode = new BarcodeCodabar();
                    break;

                case "Stimulsoft.Report.BarCodes.StiMsiBarCodeType":
                    barcode.Barcode = new BarcodeMSI();
                    break;
                case "Stimulsoft.Report.BarCodes.StiPlesseyBarCodeType":
                    barcode.Barcode = new BarcodePlessey();
                    break;
                case "Stimulsoft.Report.BarCodes.StiInterleaved2of5BarCodeType":
                    barcode.Barcode = new Barcode2of5Interleaved();
                    break;
                case "Stimulsoft.Report.BarCodes.StiPharmacodeBarCodeType":
                    barcode.Barcode = new BarcodePharmacode();
                    break;

                case "Stimulsoft.Report.BarCodes.StiStandard2of5BarCodeType":
                case "Stimulsoft.Report.BarCodes.StiDutchKIXBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiRoyalMail4StateBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiFIMBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiCode11BarCodeType":
                case "Stimulsoft.Report.BarCodes.StiUpcEBarCodeType":
                case "Stimulsoft.Report.BarCodes.StiUpcSup2BarCodeType":
                case "Stimulsoft.Report.BarCodes.StiJan13BarCodeType":
                case "Stimulsoft.Report.BarCodes.StiJan8BarCodeType":
                case "Stimulsoft.Report.BarCodes.StiSSCC18BarCodeType":
                case "Stimulsoft.Report.BarCodes.StiIsbn10BarCodeType":
                case "Stimulsoft.Report.BarCodes.StiIsbn13BarCodeType":
                default:
                    barcode.Barcode = new Barcode39();
                    break;
            }
        }

        /// <summary>
        /// Converts the StimulSoft border sides to FastReport border sides
        /// </summary>
        /// <param name="sides"></param>
        public static BorderLines ConvertBorderSides(string sides)
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

        /// <summary>
        /// Converts the StimulSoft AggregateFunction sides to FastReport MatrixAggregateFunction
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static MatrixAggregateFunction GetMatrixAggregateFunction(XmlNode node)
        {
            if (node["Summary"] != null)
            {
                switch (node["Summary"].InnerText)
                {
                    case "Image":
                        return MatrixAggregateFunction.None;
                    case "Average":
                        return MatrixAggregateFunction.Avg;
                    case "Min":
                        return MatrixAggregateFunction.Min;
                    case "Max":
                        return MatrixAggregateFunction.Max;
                    case "Count":
                        return MatrixAggregateFunction.Count;
                    case "CountDistinct":
                        return MatrixAggregateFunction.CountDistinct;
                    case "None":
                        return MatrixAggregateFunction.None;
                }
            }

            return MatrixAggregateFunction.Sum;
        }

        /// <summary>
        /// Convert fill to color
        /// </summary>
        /// <param name="fill"></param>
        /// <returns></returns>
        public static Color ConvertBrushToColor(FillBase fill)
        {
            if (fill is HatchFill)
                return (fill as HatchFill).ForeColor;
            if (fill is LinearGradientFill)
                return (fill as LinearGradientFill).StartColor;
            if (fill is GlassFill)
                return (fill as GlassFill).Color;
            if (fill is SolidFill)
                return (fill as SolidFill).Color;
            return Color.Transparent;
        }

#if MSCHART
        /// <summary>
        /// Converts the StimulSoft SeriesChartType to SeriesChartType.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SeriesChartType ConvertChartType(string value)
        {
            switch (value)
            {
                case "Stimulsoft.Report.Chart.StiPieSeries":
                    return SeriesChartType.Pie;
                case "Stimulsoft.Report.Chart.StiStackedColumnSeries":
                    return SeriesChartType.StackedColumn;
                case "Stimulsoft.Report.Chart.StiStackedLineSeries":
                    return SeriesChartType.StackedColumn;
                case "Stimulsoft.Report.Chart.StiClusteredBarSeries":
                    return SeriesChartType.Bar;
                case "Stimulsoft.Report.Chart.StiLineSeries":
                    return SeriesChartType.Line;
                case "Stimulsoft.Report.Chart.StiGanttSeries":
                case "Stimulsoft.Report.Chart.StiClusteredColumnSeries":
                    return SeriesChartType.Column;
                case "Stimulsoft.Report.Chart.StiRadarAreaSeries":
                case "Stimulsoft.Report.Chart.StiSteppedAreaSeries":
                    return SeriesChartType.Area;
                case "Stimulsoft.Report.Chart.StiDoughnutSeries":
                    return SeriesChartType.Doughnut;
                case "Stimulsoft.Report.Chart.StiStackedSplineAreaSeries":
                    return SeriesChartType.SplineArea;
                case "Stimulsoft.Report.Chart.StiFullStackedColumnSeries":
                    return SeriesChartType.StackedColumn100;
                case "Stimulsoft.Report.Chart.StiFullStackedAreaSeries":
                    return SeriesChartType.StackedArea100;
                case "Stimulsoft.Report.Chart.StiStackedBarSeries":
                    return SeriesChartType.StackedBar;
                default:
                    return SeriesChartType.Column;
            }
        }
#endif

        /// <summary>
        /// Parse string to struct f Point.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Point ConvertPoint(string value)
        {
            string[] points = value.Split(',');
            return new Point(ConvertInt(points[0]), ConvertInt(points[1]));
        }

        /// <summary>
        /// Parse string to struct of Size.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Size ConvertSize(string value)
        {
            string[] points = value.Split(',');
            return new Size(ConvertInt(points[0]), ConvertInt(points[1]));
        }


        /// <summary>
        /// Converts the StimulSoft SeriesChartType to SeriesChartType.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ContentAlignment ConvertContentAlignment(string value)
        {
            switch (value)
            {
                case "TopLeft":
                    return ContentAlignment.TopLeft;
                case "TopCenter":
                    return ContentAlignment.TopCenter;
                case "TopRight":
                    return ContentAlignment.TopRight;
                case "MiddleLeft":
                    return ContentAlignment.MiddleLeft;
                case "MiddleCenter":
                    return ContentAlignment.MiddleCenter;
                case "MiddleRight":
                    return ContentAlignment.MiddleRight;
                case "BottomLeft":
                    return ContentAlignment.BottomLeft;
                case "BottomCenter":
                    return ContentAlignment.BottomCenter;
                case "BottomRight":
                    return ContentAlignment.BottomRight;
            }
            return ContentAlignment.TopLeft;
        }
        #endregion // Public Methods
    }
}
