using System.Drawing;
using System.Linq;
namespace FastReport.Utils
{
    /// <summary>
    /// Provides utility methods for parsing and converting color values from various input formats.
    /// Supports named colors, HEX strings, RGB/ARGB values, and ARGB integer representations.
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// Parses a string representation of a color into a <see cref="Color"/> structure.
        /// </summary>
        /// <param name="input">The string to parse. Can be:
        ///     <list type="bullet">
        ///         <item><description>A named color (e.g., "Red", "DarkBlue")</description></item>
        ///         <item><description>A HEX color in formats "#RGB", "#RRGGBB", or "#AARRGGBB" (e.g., "#F00", "#FF0000", "#80FF0000")</description></item>
        ///         <item><description>An RGB or ARGB values as comma-separated integers (e.g., "255,0,255" or "128,255,0,255")</description></item>
        ///         <item><description>An ARGB integer as a decimal string (e.g., the result of <c>Color.ToArgb().ToString()</c>)</description></item>
        ///     </list>
        /// </param>
        /// <returns>
        /// A <see cref="Color"/> if parsing succeeded; otherwise, <see langword="null"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The method processes input in the following order:
        /// </para>
        /// <list type="number">
        ///   <item>
        ///     <strong>HEX format</strong>: If the string starts with '#', it is treated as a hexadecimal color value.
        ///      <list type="bullet">
        ///         <item><description>3-digit format "#RGB" is expanded to "#FFRRGGBB".</description></item>
        ///         <item><description>6-digit format "#RRGGBB" is expanded to "#FFRRGGBB" (non transparent).</description></item>
        ///         <item><description>8-digit format "#AARRGGBB" is used as-is.</description></item>
        ///     </list>
        ///     Invalid lengths or non-hex characters result in <see langword="null"/>.
        ///   </item>
        ///   <item>
        ///     <strong>RGB/ARGB values</strong>: If the string contains commas, it is split into 3 (RGB) or 4 (ARGB) components.
        ///     Each component must be a valid integer in the range 0–255.
        ///   </item>
        ///   <item>
        ///     <strong>ARGB integer</strong>: The entire string is parsed as a signed 64-bit integer,
        ///     which is then interpreted as an ARGB value (useful for strings produced by <c>Color.ToArgb().ToString()</c>).
        ///   </item>
        ///   <item>
        ///     <strong>Named color</strong>: The string is passed to <see cref="Color.FromName"/>.
        ///     Only colors with <see cref="Color.IsKnownColor"/> = <see langword="true"/> are accepted.
        ///   </item>
        /// </list>
        /// </remarks>
        public static Color? FromString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            // HEX: #RGB, #RRGGBB, #AARRGGBB
            if (input.StartsWith("#"))
            {
                string hex = input.Substring(1);
                if (hex.Length == 3)
                    // #RGB to #FFRRGGBB
                    hex = "FF" + string.Concat(hex.Select(c => $"{c}{c}"));
                else if (hex.Length == 6)
                    // #RRGGBB to #FFRRGGBB
                    hex = "FF" + hex;
                else if (hex.Length != 8)
                    // only 3, 6, and 8 digits are valid
                    return null;

                // parse the 8-digit hex as ARGB
                if (uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out uint argb))
                {
                    return Color.FromArgb(
                        (byte)(argb >> 24), // alpha
                        (byte)(argb >> 16), // red
                        (byte)(argb >> 8),  // green
                        (byte)(argb)        // blue
                    );
                }

                // any error during HEX processing
                return null;
            }

            // RGB / ARGB via comma-separated values: "R,G,B" or "A,R,G,B"
            if (input.Contains(','))
            {
                var parts = input.Split(',');
                if (parts.Length is 3 or 4)
                {
                    // ensure all parts are valid integers
                    if (parts.All(p => int.TryParse(p.Trim(), out int _)))
                    {
                        var nums = parts.Select(p => int.Parse(p.Trim())).ToArray();
                        if (nums.All(n => n >= 0 && n <= 255))
                        {
                            return parts.Length == 3
                              ? Color.FromArgb(nums[0], nums[1], nums[2])           // RGB
                              : Color.FromArgb(nums[0], nums[1], nums[2], nums[3]); // ARGB
                        }
                    }
                }
            }

            // ARGB integer as a decimal string (e.g., the result of Color.ToArgb().ToString())
            if (long.TryParse(input, out long argbInt))
            {
                // convert to unsigned to correctly interpret negative ARGB values
                uint argb = unchecked((uint)argbInt);
                return Color.FromArgb(
                        (byte)(argb >> 24), // alpha
                        (byte)(argb >> 16), // red
                        (byte)(argb >> 8),  // green
                        (byte)(argb)        // blue
                    );
            }

            // named color (e.g., "Red", "Blue")
            Color named = Color.FromName(input);
            if (named.IsKnownColor)
                return named;

            // no format matched - invalid color string
            return null;
        }

        /// <summary>
        /// Converts an object of various types to a <see cref="Color"/>.
        /// If the conversion fails, returns a fallback color (default: <see cref="Color.Gray"/>).
        /// </summary>
        /// <param name="value">The input value. Supported types:
        ///     <list type="bullet">
        ///         <item><description><see cref="Color"/> - returned as-is.</description></item>
        ///         <item><description><see cref="int"/> - interpreted as an ARGB value.</description></item>
        ///         <item><description><see cref="string"/> - parsed using <see cref="FromString"/>.</description></item>
        ///     </list>
        ///     Any other type treated as invalid.
        /// </param>
        /// <param name="fallback">The color to return if conversion fails.
        /// If not specified, defaults to <see cref="Color.Gray"/>.</param>
        /// <returns>
        /// A valid <see cref="Color"/> instance. Never returns <see langword="null"/>.
        /// </returns>
        public static Color FromObject(object value, Color fallback = default)
        {
            // if no fallback was provided, use Gray color
            if (fallback == default) fallback = Color.Gray;

            // use pattern matching to handle supported types
            return value switch
            {
                Color c => c, // direct Color instance
                int argb => Color.FromArgb(argb), // ARGB integer
                string s => FromString(s) ?? fallback, // parse string, fallback on failure
                _ => fallback // unsupported type - fallback
            };
        }
    }
}