using System;

namespace UserFunctions
{

    public static class MyFunctions
    {
        /// <summary>
        /// Converts a specified string to uppercase.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A string in uppercase.</returns>
        public static string MyUpperCase(string s)
        {
            return s == null ? "" : s.ToUpper();
        }

        /// <summary>
        /// Returns the larger of two 32-bit signed integers. 
        /// </summary>
        /// <param name="val1">The first of two values to compare.</param>
        /// <param name="val2">The second of two values to compare.</param>
        /// <returns>Parameter val1 or val2, whichever is larger.</returns>
        public static int MyMaximum(int val1, int val2)
        {
            return Math.Max(val1, val2);
        }

        /// <summary>
        /// Returns the larger of two 64-bit signed integers. 
        /// </summary>
        /// <param name="val1">The first of two values to compare.</param>
        /// <param name="val2">The second of two values to compare.</param>
        /// <returns>Parameter val1 or val2, whichever is larger.</returns>
        public static long MyMaximum(long val1, long val2)
        {
            return Math.Max(val1, val2);
        }
    }
}
