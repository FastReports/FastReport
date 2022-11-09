using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FastReport.Import.RDL
{
    /// <summary>
    /// The RDL units converter.
    /// </summary>
    public static partial class UnitsConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts the RDL Boolean to bool value.
        /// </summary>
        /// <param name="boolean">The RDL Boolean value.</param>
        /// <returns>The bool value.</returns>
        public static bool BooleanToBool(string boolean)
        {
            if (boolean.ToLower() == "true")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts the RDL Color to Color.
        /// </summary>
        /// <param name="colorName">The RDL Color value.</param>
        /// <returns>The Color value.</returns>
        public static Color ConvertColor(string colorName)
        {
            return Color.FromName(colorName);
        }

        /// <summary>
        /// Converts the RDL Size to float value.
        /// </summary>
        /// <param name="size">The RDL Size value.</param>
        /// <param name="unit">The RDL Size units measure.</param>
        /// <returns>The float value of RDL Size.</returns>
        public static float SizeToFloat(string size, string unit)
        {
            return float.Parse(size.Replace(unit, ""), new CultureInfo("en-US", false).NumberFormat);
        }

        /// <summary>
        /// Converts the RDL Size to int value.
        /// </summary>
        /// <param name="size">The RDL Size value.</param>
        /// <param name="unit">The RDL Size units measure.</param>
        /// <returns>The int value of RDL Size.</returns>
        public static int SizeToInt(string size, string unit)
        {
            int resSize = 0;
            int.TryParse(size.Replace(unit, ""), NumberStyles.None, new CultureInfo("en-US", false).NumberFormat, out resSize);
            return resSize;
        }

        /// <summary>
        /// Converts the RDL Size to millimeters.
        /// </summary>
        /// <param name="size">The RDL Size value.</param>
        /// <returns>The float value of RDL Size in millimeters.</returns>
        public static float SizeToMillimeters(string size)
        {
            if (size.Contains(SizeUnits.Millimeter))
            {
                return SizeToFloat(size, SizeUnits.Millimeter);
            }
            else if (size.Contains(SizeUnits.Centimeter))
            {
                return SizeToFloat(size, SizeUnits.Centimeter) * SizeUnitsM.Centimeter;
            }
            else if (size.Contains(SizeUnits.Inch))
            {
                return SizeToFloat(size, SizeUnits.Inch) * SizeUnitsM.Inch;
            }
            else if (size.Contains(SizeUnits.Point))
            {
                return SizeToFloat(size, SizeUnits.Point) * SizeUnitsM.Point;
            }
            else if (size.Contains(SizeUnits.Pica))
            {
                return SizeToFloat(size, SizeUnits.Pica) * SizeUnitsM.Pica;
            }
            return 0.0f;
        }

        /// <summary>
        /// Converts the RDL Size to pixels.
        /// </summary>
        /// <param name="size">The RDL Size value.</param>
        /// <returns>The float value of RDL Size in pixels.</returns>
        public static float SizeToPixels(string size)
        {
            if (size.Contains(SizeUnits.Millimeter))
            {
                return SizeToFloat(size, SizeUnits.Millimeter) * SizeUnitsP.Millimeter;
            }
            else if (size.Contains(SizeUnits.Centimeter))
            {
                return SizeToFloat(size, SizeUnits.Centimeter) * SizeUnitsP.Centimeter;
            }
            else if (size.Contains(SizeUnits.Inch))
            {
                return SizeToFloat(size, SizeUnits.Inch) * SizeUnitsP.Inch;
            }
            else if (size.Contains(SizeUnits.Point))
            {
                return SizeToFloat(size, SizeUnits.Point) * SizeUnitsP.Point;
            }
            else if (size.Contains(SizeUnits.Pica))
            {
                return SizeToFloat(size, SizeUnits.Pica) * SizeUnitsP.Pica;
            }
            return 0.0f;
        }

        /// <summary>
        /// Converts the RDL FontStyle to FontStyle.
        /// </summary>
        /// <param name="fontStyle">The RDL FontStyle value.</param>
        /// <returns>The FontStyle value.</returns>
        public static FontStyle ConvertFontStyle(string fontStyle)
        {
            if (fontStyle == "Italic")
            {
                return FontStyle.Italic;
            }
            return FontStyle.Regular;
        }

        /// <summary>
        /// Converts the RDL FontSize to float.
        /// </summary>
        /// <param name="fontSize">The RDL FontSize value.</param>
        /// <returns>The float value of RDL FontSize in points.</returns>
        public static float ConvertFontSize(string fontSize)
        {
            return float.Parse(fontSize.Replace(SizeUnits.Point, ""), new CultureInfo("en-US", false).NumberFormat);
        }

        /// <summary>
        /// Converts the RDL TextAlign to HorzAlign.
        /// </summary>
        /// <param name="textAlign">The RDL TextAlign value.</param>
        /// <returns>The HorzAlign value.</returns>
        public static HorzAlign ConvertTextAlign(string textAlign)
        {
            if (textAlign == "Center")
            {
                return HorzAlign.Center;
            }
            else if (textAlign == "Right")
            {
                return HorzAlign.Right;
            }
            return HorzAlign.Left;
        }

        /// <summary>
        /// Converts the RDL TextAlign to VerticalAlign.
        /// </summary>
        /// <param name="verticalAlign">The RDL VerticalAlign value.</param>
        /// <returns>The VertAlign value.</returns>
        public static VertAlign ConvertVerticalAlign(string verticalAlign)
        {
            if (verticalAlign == "Middle")
            {
                return VertAlign.Center;
            }
            else if (verticalAlign == "Bottom")
            {
                return VertAlign.Bottom;
            }
            return VertAlign.Top;
        }

        /// <summary>
        /// Converts the RDL WritingMode to Angle.
        /// </summary>
        /// <param name="writingMode">The RDL WritingMode value.</param>
        /// <returns>The int value of RDL WritingMode in degree.</returns>
        public static int ConvertWritingMode(string writingMode)
        {
            if (writingMode == "tb-rl")
            {
                return 90;
            }
            return 0;
        }

        /// <summary>
        /// Converts the RDL TextAlign to StringAlignment.
        /// </summary>
        /// <param name="textAlign">The RDL TextAling value.</param>
        /// <returns>The StringAlignment value.</returns>
        public static StringAlignment ConvertTextAlignToStringAlignment(string textAlign)
        {
            if (textAlign == "Left")
            {
                return StringAlignment.Near;
            }
            else if (textAlign == "Right")
            {
                return StringAlignment.Far;
            }
            return StringAlignment.Center;
        }

        /// <summary>
        /// Converts the RDL TextAlign and VerticalAlign to ContentAlignment.
        /// </summary>
        /// <param name="textAlign">The RDL TextAlign value.</param>
        /// <param name="vertAlign">The RDL VerticalAlign value.</param>
        /// <returns>The ContentAlignment value.</returns>
        public static ContentAlignment ConvertTextAndVerticalAlign(string textAlign, string vertAlign)
        {
            if (textAlign == "General" || textAlign == "Center")
            {
                if (vertAlign == "Top")
                {
                    return ContentAlignment.TopCenter;
                }
                else if (vertAlign == "Middle")
                {
                    return ContentAlignment.MiddleCenter;
                }
                else if (vertAlign == "Bottom")
                {
                    return ContentAlignment.BottomCenter;
                }
                return ContentAlignment.TopCenter;
            }
            else if (textAlign == "Left")
            {
                if (vertAlign == "Top")
                {
                    return ContentAlignment.TopLeft;
                }
                else if (vertAlign == "Middle")
                {
                    return ContentAlignment.MiddleLeft;
                }
                else if (vertAlign == "Bottom")
                {
                    return ContentAlignment.BottomLeft;
                }
                return ContentAlignment.TopLeft;
            }
            else if (textAlign == "Right")
            {
                if (vertAlign == "Top")
                {
                    return ContentAlignment.TopRight;
                }
                else if (vertAlign == "Middle")
                {
                    return ContentAlignment.MiddleRight;
                }
                else if (vertAlign == "Bottom")
                {
                    return ContentAlignment.BottomRight;
                }
                return ContentAlignment.TopRight;
            }
            return ContentAlignment.TopLeft;
        }

        /// <summary>
        /// Converts the RDL BorderStyle to LineStyle.
        /// </summary>
        /// <param name="borderStyle">The RDL BorderStyle value.</param>
        /// <returns>The LineStyle value.</returns>
        public static LineStyle ConvertBorderStyle(string borderStyle)
        {
            if (borderStyle == "Dotted")
            {
                return LineStyle.Dot;
            }
            else if (borderStyle == "Dashed")
            {
                return LineStyle.Dash;
            }
            else if (borderStyle == "Double")
            {
                return LineStyle.Double;
            }
            return LineStyle.Solid;
        }

        /// <summary>
        /// Converts the RDL Sizing to PictureBoxSizeMode.
        /// </summary>
        /// <param name="sizing">The RDL Sizing value.</param>
        /// <returns>The PictureBoxSizeMode value.</returns>
        public static PictureBoxSizeMode ConvertSizing(string sizing)
        {
            if (sizing == "AutoSize")
            {
                return PictureBoxSizeMode.AutoSize;
            }
            else if (sizing == "Fit")
            {
                return PictureBoxSizeMode.StretchImage;
            }
            else if (sizing == "Clip")
            {
                return PictureBoxSizeMode.Normal;
            }
            return PictureBoxSizeMode.Zoom;
        }

        /*/// <summary>
        /// Converts the RDL GradientType to GradientStyle.
        /// </summary>
        /// <param name="gradientType">The RDL GradientType value.</param>
        /// <returns>The GradientStyle value.</returns>
        public static GradientStyle ConvertGradientType(string gradientType)
        {
            if (gradientType == "LeftRight")
            {
                return GradientStyle.LeftRight;
            }
            else if (gradientType == "TopBottom")
            {
                return GradientStyle.TopBottom;
            }
            else if (gradientType == "Center")
            {
                return GradientStyle.Center;
            }
            else if (gradientType == "DiagonalLeft")
            {
                return GradientStyle.DiagonalLeft;
            }
            else if (gradientType == "DiagonalRight")
            {
                return GradientStyle.DiagonalRight;
            }
            else if (gradientType == "HorizontalCenter")
            {
                return GradientStyle.HorizontalCenter;
            }
            else if (gradientType == "VerticalCenter")
            {
                return GradientStyle.VerticalCenter;
            }
            return GradientStyle.None;
        }*/

        /*/// <summary>
        /// Converts the RDL Chart.Type to SeriesChartType.
        /// </summary>
        /// <param name="chartType">The RDL Chart.Type value.</param>
        /// <returns>The SeriesChartType value.</returns>
        public static SeriesChartType ConvertChartType(string chartType)
        {
            if (chartType == "Area")
            {
                return SeriesChartType.Area;
            }
            else if (chartType == "Bar")
            {
                return SeriesChartType.Bar;
            }
            else if (chartType == "Doughnut")
            {
                return SeriesChartType.Doughnut;
            }
            else if (chartType == "Line")
            {
                return SeriesChartType.Line;
            }
            else if (chartType == "Pie")
            {
                return SeriesChartType.Pie;
            }
            else if (chartType == "Bubble")
            {
                return SeriesChartType.Bubble;
            }
            return SeriesChartType.Column;
        }*/

        /*/// <summary>
        /// Converts the RDL Chart.Palette to ChartColorPalette.
        /// </summary>
        /// <param name="chartPalette">The RDL Chart.Palette value.</param>
        /// <returns>The RDL ChartColorPalette value.</returns>
        public static ChartColorPalette ConvertChartPalette(string chartPalette)
        {
            if (chartPalette == "EarthTones")
            {
                return ChartColorPalette.EarthTones;
            }
            else if (chartPalette == "Excel")
            {
                return ChartColorPalette.Excel;
            }
            else if (chartPalette == "GrayScale")
            {
                return ChartColorPalette.Grayscale;
            }
            else if (chartPalette == "Light")
            {
                return ChartColorPalette.Light;
            }
            else if (chartPalette == "Pastel")
            {
                return ChartColorPalette.Pastel;
            }
            else if (chartPalette == "SemiTransparent")
            {
                return ChartColorPalette.SemiTransparent;
            }
            return ChartColorPalette.None;
        }*/

        /*/// <summary>
        /// Converts the RDL Chart.Legend.Position to Legend.Docking and Legend.Alignment.
        /// </summary>
        /// <param name="chartLegendPosition">The RDL Chart.Legend.Position value.</param>
        /// <param name="legend">The Legend instance to convert to.</param>
        public static void ConvertChartLegendPosition(string chartLegendPosition, Legend legend)
        {
            if (chartLegendPosition == "TopLeft")
            {
                legend.Docking = Docking.Top;
                legend.Alignment = StringAlignment.Near;
            }
            else if (chartLegendPosition == "TopCenter")
            {
                legend.Docking = Docking.Top;
                legend.Alignment = StringAlignment.Center;
            }
            else if (chartLegendPosition == "TopRight")
            {
                legend.Docking = Docking.Top;
                legend.Alignment = StringAlignment.Far;
            }
            else if (chartLegendPosition == "LeftTop")
            {
                legend.Docking = Docking.Left;
                legend.Alignment = StringAlignment.Near;
            }
            else if (chartLegendPosition == "LeftCenter")
            {
                legend.Docking = Docking.Left;
                legend.Alignment = StringAlignment.Center;
            }
            else if (chartLegendPosition == "LeftBottom")
            {
                legend.Docking = Docking.Left;
                legend.Alignment = StringAlignment.Far;
            }
            else if (chartLegendPosition == "RightTop")
            {
                legend.Docking = Docking.Right;
                legend.Alignment = StringAlignment.Near;
            }
            else if (chartLegendPosition == "RightCenter")
            {
                legend.Docking = Docking.Right;
                legend.Alignment = StringAlignment.Center;
            }
            else if (chartLegendPosition == "RightBottom")
            {
                legend.Docking = Docking.Right;
                legend.Alignment = StringAlignment.Far;
            }
            else if (chartLegendPosition == "BottomLeft")
            {
                legend.Docking = Docking.Bottom;
                legend.Alignment = StringAlignment.Near;
            }
            else if (chartLegendPosition == "BottomCenter")
            {
                legend.Docking = Docking.Bottom;
                legend.Alignment = StringAlignment.Center;
            }
            else if (chartLegendPosition == "BottomRight")
            {
                legend.Docking = Docking.Bottom;
                legend.Alignment = StringAlignment.Far;
            }
        }*/

        /*/// <summary>
        /// Converts the RDL Chart.Legend.Layout to LegendStyle.
        /// </summary>
        /// <param name="chartLegendLayout">The RDL Chart.Legend.Layout value.</param>
        /// <returns>The LegendStyle value.</returns>
        public static LegendStyle ConvertChartLegendLayout(string chartLegendLayout)
        {
            if (chartLegendLayout == "Table")
            {
                return LegendStyle.Table;
            }
            else if (chartLegendLayout == "Row")
            {
                return LegendStyle.Row;
            }
            return LegendStyle.Column;
        }*/

        /*/// <summary>
        /// Converts the RDL BorderStyle to ChartDashStyle.
        /// </summary>
        /// <param name="borderStyle">The RDL BorderStyle value.</param>
        /// <returns>The ChartDashStyle value.</returns>
        public static ChartDashStyle ConvertBorderStyleToChartDashStyle(string borderStyle)
        {
            if (borderStyle == "Dotted")
            {
                return ChartDashStyle.Dot;
            }
            else if (borderStyle == "Dashed")
            {
                return ChartDashStyle.Dash;
            }
            return ChartDashStyle.Solid;
        }*/

        /*/// <summary>
        /// Converts the RDL Axis.Visible to AxisEnabled.
        /// </summary>
        /// <param name="axisVisible">The RDL Axis.Visible value.</param>
        /// <returns>The AxisEnabled value.</returns>
        public static AxisEnabled ConvertAxisVisibleToAxisEnabled(string axisVisible)
        {
            if (axisVisible.ToLower() == "true")
            {
                return AxisEnabled.True;
            }
            else if (axisVisible.ToLower() == "false")
            {
                return AxisEnabled.False;
            }
            return AxisEnabled.Auto;
        }*/

        /*/// <summary>
        /// Converts the RDL TickMarkStyle to TickMarkStyle.
        /// </summary>
        /// <param name="tickMarkStyle">The RDL TickMarkStyle value.</param>
        /// <returns>The TickMarkStyle value.</returns>
        public static TickMarkStyle ConvertTickMarkStyle(string tickMarkStyle)
        {
            if (tickMarkStyle == "Inside")
            {
                return TickMarkStyle.InsideArea;
            }
            else if (tickMarkStyle == "Outside")
            {
                return TickMarkStyle.OutsideArea;
            }
            else if (tickMarkStyle == "Cross")
            {
                return TickMarkStyle.AcrossAxis;
            }
            return TickMarkStyle.None;
        }*/

        /*/// <summary>
        /// Converts the RDL Shading to LightStyle.
        /// </summary>
        /// <param name="shading">The RDL Shading value.</param>
        /// <returns>The LightStyle value.</returns>
        public static LightStyle ConvertShading(string shading)
        {
            if (shading == "Simple")
            {
                return LightStyle.Simplistic;
            }
            else if (shading == "Real")
            {
                return LightStyle.Realistic;
            }
            return LightStyle.None;
        }*/

        #endregion // Public Methods
    }
}
