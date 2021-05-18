using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Import.ListAndLabel
{
    /// <summary>
    /// The List and Label units converter.
    /// </summary>
    public static class UnitsConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts List and Label units to millimeters.
        /// </summary>
        /// <param name="str">The List and Label unit as string.</param>
        /// <returns>The value in millimeters.</returns>
        public static float LLUnitsToMillimeters(string str)
        {
            return (float)Math.Round(Convert.ToDouble(str) / 1000);
        }

        /// <summary>
        /// Converts List and Label units to pixels.
        /// </summary>
        /// <param name="str">The List and Label unit as string.</param>
        /// <returns>The value in pixels.</returns>
        public static float LLUnitsToPixels(string str)
        {
            return LLUnitsToMillimeters(str) * Units.Millimeters;
        }

        /// <summary>
        /// Converts List and Label paper orientation.
        /// </summary>
        /// <param name="str">The List and Label paper orientation value as string.</param>
        /// <returns>Returns <b>true</b> if orientation is landscape.</returns>
        public static bool ConvertPaperOrientation(string str)
        {
            return str == "2" ? true : false;
        }

        /// <summary>
        /// Converts List and Label bool.
        /// </summary>
        /// <param name="str">The List and Label bool value as string.</param>
        /// <returns>A bool value.</returns>
        public static bool ConvertBool(string str)
        {
            return str.ToLower() == "true" ? true : false;
        }

        /// <summary>
        /// Converts List and Label text Align.
        /// </summary>
        /// <param name="str">The List and Label text Align value as string.</param>
        /// <returns>A HorzAlign value.</returns>
        public static HorzAlign ConvertTextAlign(string str)
        {
            HorzAlign align = HorzAlign.Left;
            switch (str)
            {
                case "1":
                    align = HorzAlign.Center;
                    break;
                case "2":
                    align = HorzAlign.Right;
                    break;
                default:
                    align = HorzAlign.Left;
                    break;
            }
            return align;
        }

        /// <summary>
        /// Convert List and Label LineType to LineStyle.
        /// </summary>
        /// <param name="str">The List and Label LineType value as string.</param>
        /// <returns>A LineStyle value.</returns>
        public static LineStyle ConvertLineType(string str)
        {
            LineStyle style = LineStyle.Solid;
            switch (str)
            {
                case "0":
                    style = LineStyle.Solid;
                    break;
                case "1":
                    style = LineStyle.Dot;
                    break;
                case "2":
                    style = LineStyle.Dash;
                    break;
                case "3":
                    style = LineStyle.DashDot;
                    break;
                case "4":
                    style = LineStyle.DashDotDot;
                    break;
                case "5":
                    style = LineStyle.Double;
                    break;
            }
            return style;
        }

        /// <summary>
        /// Converts List and Label rounding to float.
        /// </summary>
        /// <param name="str">The List and Label rounding value as string.</param>
        /// <returns>A float value.</returns>
        public static float ConvertRounding(string str)
        {
            return Convert.ToSingle(str) / 10;
        }

        #endregion // Public Methods
    }
}
