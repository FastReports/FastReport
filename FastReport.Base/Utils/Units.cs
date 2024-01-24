using System;
using System.Collections;

namespace FastReport.Utils
{
    /// <summary>
    /// The report page units.
    /// </summary>
    public enum PageUnits
    {
        /// <summary>
        /// Specifies the units measured in millimeters.
        /// </summary>
        Millimeters,

        /// <summary>
        /// Specifies the units measured in centimeters.
        /// </summary>
        Centimeters,

        /// <summary>
        /// Specifies the units measured in inches.
        /// </summary>
        Inches,

        /// <summary>
        /// Specifies the units measured in hundreths of inch.
        /// </summary>
        HundrethsOfInch
    }

    /// <summary>
    /// Defines the constants used to convert between report units and screen pixels.
    /// </summary>
    /// <remarks>
    /// To convert pixels to millimeters, use the following code:
    /// <code>valueInMillimeters = valueInPixels / Units.Millimeters;</code>
    /// To convert millimeters to pixels, use the following code:
    /// <code>valueInPixels = valueInMillimeters * Units.Millimeters;</code>
    /// </remarks>
    public static class Units
    {
        /// <summary>
        /// The number of pixels in one millimeter.
        /// </summary>
        public static float Millimeters = 3.78f;

        /// <summary>
        /// The number of pixels in one centimeter.
        /// </summary>
        public static float Centimeters = 37.8f;

        /// <summary>
        /// The number of pixels in one inch.
        /// </summary>
        public static float Inches = 96;

        /// <summary>
        /// The number of pixels in 1/10 of ich.
        /// </summary>
        public static float TenthsOfInch = 9.6f;

        /// <summary>
        /// The number of pixels in 1/100 of inch.
        /// </summary>
        public static float HundrethsOfInch = 0.96f;
    }

    public static class FileSize
    {
        /// <summary>
        /// File size units.
        /// </summary>
        public enum Units
        {
            /// <summary>
            /// Bytes.
            /// </summary>
            Bytes,

            /// <summary>
            /// Kilobytes.
            /// </summary>
            KB,

            /// <summary>
            /// Megabytes.
            /// </summary>
            MB,

            /// <summary>
            /// Gigabytes.
            /// </summary>
            GB,

            /// <summary>
            /// Terobytes.
            /// </summary>
            TB
        }

        /// <summary>
        /// Convert numbers to file size (example 1 MB).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToString(long value)
        {
            int steps = 0;
            float result = value;

            while (result > 1024 - 1)
            {
                result /= 1024;
                steps++;
            }

            return result.ToString("F2") + " " + GetUnits((Units)steps);
        }

        private static string GetUnits(Units units)
        {
            switch (units)
            {
                case Units.Bytes:
                    return Res.Get("Misc,Bytes");
                case Units.KB:
                    return Res.Get("Misc,KB");
                case Units.MB:
                    return Res.Get("Misc,MB");
                case Units.GB:
                    return Res.Get("Misc,GB");
                case Units.TB:
                    return Res.Get("Misc,TB");
                default:
                    return Res.Get("Misc,Bytes");
            }
        }
    }
}