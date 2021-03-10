using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;
using System.Globalization;
using FastReport.Data;

namespace FastReport.Functions
{
  /// <summary>
  /// Contains standard functions registered in the "Data" window.
  /// </summary>
  public static class StdFunctions
  {
    #region Math functions
    /// <summary>
    /// Returns the larger of two 32-bit signed integers. 
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is larger.</returns>
    public static int Maximum(int val1, int val2)
    {
      return Math.Max(val1, val2);
    }

    /// <summary>
    /// Returns the larger of two 64-bit signed integers. 
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is larger.</returns>
    public static long Maximum(long val1, long val2)
    {
      return Math.Max(val1, val2);
    }

    /// <summary>
    /// Returns the larger of two single-precision floating-point numbers.
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is larger.</returns>
    public static float Maximum(float val1, float val2)
    {
      return Math.Max(val1, val2);
    }

    /// <summary>
    /// Returns the larger of two double-precision floating-point numbers.
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is larger.</returns>
    public static double Maximum(double val1, double val2)
    {
      return Math.Max(val1, val2);
    }

    /// <summary>
    /// Returns the larger of two decimal numbers.
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is larger.</returns>
    public static decimal Maximum(decimal val1, decimal val2)
    {
      return Math.Max(val1, val2);
    }

    /// <summary>
    /// Returns the smaller of two 32-bit signed integers. 
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is smaller.</returns>
    public static int Minimum(int val1, int val2)
    {
      return Math.Min(val1, val2);
    }

    /// <summary>
    /// Returns the smaller of two 64-bit signed integers. 
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is smaller.</returns>
    public static long Minimum(long val1, long val2)
    {
      return Math.Min(val1, val2);
    }

    /// <summary>
    /// Returns the smaller of two single-precision floating-point numbers.
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is smaller.</returns>
    public static float Minimum(float val1, float val2)
    {
      return Math.Min(val1, val2);
    }

    /// <summary>
    /// Returns the smaller of two double-precision floating-point numbers.
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is smaller.</returns>
    public static double Minimum(double val1, double val2)
    {
      return Math.Min(val1, val2);
    }

    /// <summary>
    /// Returns the smaller of two decimal numbers.
    /// </summary>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>Parameter val1 or val2, whichever is smaller.</returns>
    public static decimal Minimum(decimal val1, decimal val2)
    {
      return Math.Min(val1, val2);
    }
    #endregion

    #region Text functions
    /// <summary>
    /// Returns an integer value representing the character code corresponding to a character.
    /// </summary>
    /// <param name="c">Character to convert.</param>
    /// <returns>The character code.</returns>
    public static int Asc(char c)
    {
      return (int)c;
    }

    /// <summary>
    /// Returns the character associated with the specified character code.
    /// </summary>
    /// <param name="i">Character code to convert.</param>
    /// <returns>The character.</returns>
    public static char Chr(int i)
    {
      return (char)i;
    }

    /// <summary>
    /// Inserts a specified string at a specified index position in the original string.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="startIndex">The index position of the insertion.</param>
    /// <param name="value">The string to insert.</param>
    /// <returns>A new string.</returns>
    public static string Insert(string s, int startIndex, string value)
    {
      return s == null ? "" : s.Insert(startIndex, value);
    }
    
    /// <summary>
    /// Gets the number of characters in a string.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <returns>The number of characters.</returns>
    public static int Length(string s)
    {
      return s == null ? 0 : s.Length;
    }
    
    /// <summary>
    /// Converts a specified string to lowercase.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>A string in lowercase.</returns>
    public static string LowerCase(string s)
    {
      return s == null ? "" : s.ToLower();
    }

    /// <summary>
    /// Right-aligns the characters in a string, padding with spaces on the left for a specified total length.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="totalWidth">The number of characters in the resulting string.</param>
    /// <returns>Right-aligned string, padded on the left with spaces.</returns>
    public static string PadLeft(string s, int totalWidth)
    {
      return s == null ? "" : s.PadLeft(totalWidth);
    }

    /// <summary>
    /// Right-aligns the characters in a string, padding on the left with a specified character
    /// for a specified total length.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="totalWidth">The number of characters in the resulting string.</param>
    /// <param name="paddingChar">A padding character.</param>
    /// <returns>Right-aligned string, padded on the left with padding characters.</returns>
    public static string PadLeft(string s, int totalWidth, char paddingChar)
    {
      return s == null ? "" : s.PadLeft(totalWidth, paddingChar);
    }

    /// <summary>
    /// Left-aligns the characters in a string, padding with spaces on the right, for a specified total length.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="totalWidth">The number of characters in the resulting string.</param>
    /// <returns>Left-aligned string, padded on the right with spaces.</returns>
    public static string PadRight(string s, int totalWidth)
    {
      return s == null ? "" : s.PadRight(totalWidth);
    }

    /// <summary>
    /// Left-aligns the characters in a string, padding on the right with a specified character, 
    /// for a specified total length.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="totalWidth">The number of characters in the resulting string.</param>
    /// <param name="paddingChar">A padding character.</param>
    /// <returns>Left-aligned string, padded on the right with padding characters.</returns>
    public static string PadRight(string s, int totalWidth, char paddingChar)
    {
      return s == null ? "" : s.PadRight(totalWidth, paddingChar);
    }

    /// <summary>
    /// Converts the specified string to titlecase. 
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>A new string.</returns>
    public static string TitleCase(string s)
    {
      return s == null ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
    }

    /// <summary>
    /// Deletes all the characters from a string beginning at a specified position.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="startIndex">The position to begin deleting characters.</param>
    /// <returns>A new string.</returns>
    public static string Remove(string s, int startIndex)
    {
      return s == null ? "" : s.Remove(startIndex);
    }

    /// <summary>
    /// Deletes a specified number of characters from a string beginning at a specified position.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="startIndex">The position to begin deleting characters.</param>
    /// <param name="count">The number of characters to delete.</param>
    /// <returns>A new string.</returns>
    public static string Remove(string s, int startIndex, int count)
    {
      return s == null ? "" : s.Remove(startIndex, count);
    }

    /// <summary>
    /// Replaces all occurrences of a specified string in the original string, with another specified string.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="oldValue">A string to be replaced.</param>
    /// <param name="newValue">A string to replace all occurrences of oldValue.</param>
    /// <returns>A new string.</returns>
    public static string Replace(string s, string oldValue, string newValue)
    {
      return s == null ? "" : s.Replace(oldValue, newValue);
    }

    /// <summary>
    /// Retrieves a substring from the original string, starting at a specified character position.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="startIndex">The starting character position of a substring.</param>
    /// <returns>A new string.</returns>
    public static string Substring(string s, int startIndex)
    {
      return s == null ? "" : s.Substring(startIndex);
    }

    /// <summary>
    /// Retrieves a substring from the original string, starting at a specified character position, 
    /// with a specified length.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <param name="startIndex">The starting character position of a substring.</param>
    /// <param name="length">The number of characters in the substring.</param>
    /// <returns>A new string.</returns>
    public static string Substring(string s, int startIndex, int length)
    {
      return s == null ? "" : s.Substring(startIndex, length);
    }

    /// <summary>
    /// Removes all occurrences of white space characters from the beginning and end of the original string.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <returns>A new string.</returns>
    public static string Trim(string s)
    {
      return s == null ? "" : s.Trim();
    }

    /// <summary>
    /// Converts a specified string to uppercase.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>A string in uppercase.</returns>
    public static string UpperCase(string s)
    {
      return s == null ? "" : s.ToUpper();
    }
    #endregion

    #region Date & Time functions
    /// <summary>
    /// Adds the specified number of days to the original date.
    /// </summary>
    /// <param name="date">The original date.</param>
    /// <param name="value">A number of whole and fractional days.</param>
    /// <returns>A new DateTime value.</returns>
    public static DateTime AddDays(DateTime date, double value)
    {
      return date.AddDays(value);
    }

    /// <summary>
    /// Adds the specified number of hours to the original date.
    /// </summary>
    /// <param name="date">The original date.</param>
    /// <param name="value">A number of whole and fractional hours.</param>
    /// <returns>A new DateTime value.</returns>
    public static DateTime AddHours(DateTime date, double value)
    {
      return date.AddHours(value);
    }

    /// <summary>
    /// Adds the specified number of minutes to the original date.
    /// </summary>
    /// <param name="date">The original date.</param>
    /// <param name="value">A number of whole and fractional minutes.</param>
    /// <returns>A new DateTime value.</returns>
    public static DateTime AddMinutes(DateTime date, double value)
    {
      return date.AddMinutes(value);
    }

    /// <summary>
    /// Adds the specified number of months to the original date.
    /// </summary>
    /// <param name="date">The original date.</param>
    /// <param name="value">A number of months.</param>
    /// <returns>A new DateTime value.</returns>
    public static DateTime AddMonths(DateTime date, int value)
    {
      return date.AddMonths(value);
    }

    /// <summary>
    /// Adds the specified number of seconds to the original date.
    /// </summary>
    /// <param name="date">The original date.</param>
    /// <param name="value">A number of whole and fractional seconds.</param>
    /// <returns>A new DateTime value.</returns>
    public static DateTime AddSeconds(DateTime date, double value)
    {
      return date.AddSeconds(value);
    }

    /// <summary>
    /// Adds the specified number of years to the original date.
    /// </summary>
    /// <param name="date">The original date.</param>
    /// <param name="value">A number of years.</param>
    /// <returns>A new DateTime value.</returns>
    public static DateTime AddYears(DateTime date, int value)
    {
      return date.AddYears(value);
    }

    /// <summary>
    /// Subtracts the specified date and time from the original date.
    /// </summary>
    /// <param name="date1">The original date.</param>
    /// <param name="date2">The date and time to subtract.</param>
    /// <returns>A TimeSpan interval between two dates.</returns>
    public static TimeSpan DateDiff(DateTime date1, DateTime date2)
    {
      return date1.Subtract(date2);
    }

    /// <summary>
    /// Initializes a new instance of the DateTime.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <returns>A new DateTime value.</returns>
    public static DateTime DateSerial(int year, int month, int day)
    {
      return new DateTime(year, month, day);
    }

    /// <summary>
    /// Gets the day of the month.
    /// </summary>
    /// <param name="date">The date value.</param>
    /// <returns>The day component.</returns>
    public static int Day(DateTime date)
    {
      return date.Day;
    }

    /// <summary>
    /// Gets the localized name of the day of the week.
    /// </summary>
    /// <param name="date">The date value.</param>
    /// <returns>The name of the day of the week.</returns>
    public static string DayOfWeek(DateTime date)
    {
      return date.ToString("dddd");
    }

    /// <summary>
    /// Gets the day of the year.
    /// </summary>
    /// <param name="date">The date value.</param>
    /// <returns>The day of the year.</returns>
    public static int DayOfYear(DateTime date)
    {
      return date.DayOfYear;
    }

    /// <summary>
    /// Returns the number of days in the specified month and year.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <returns>The number of days in month for the specified year.</returns>
    public static int DaysInMonth(int year, int month)
    {
      return DateTime.DaysInMonth(year, month);
    }

    /// <summary>
    /// Gets the hour component of the date.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The hour component.</returns>
    public static int Hour(DateTime date)
    {
      return date.Hour;
    }

    /// <summary>
    /// Gets the minute component of the date.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The minute component.</returns>
    public static int Minute(DateTime date)
    {
      return date.Minute;
    }

    /// <summary>
    /// Gets the month component of the date.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The month component.</returns>
    public static int Month(DateTime date)
    {
      return date.Month;
    }

    /// <summary>
    /// Gets the localized month name.
    /// </summary>
    /// <param name="month">The month number.</param>
    /// <returns>The month name.</returns>
    public static string MonthName(int month)
    {
      return new DateTime(2000, month, 1).ToString("MMMM");
    }

    /// <summary>
    /// Gets the seconds component of the date.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The seconds component.</returns>
    public static int Second(DateTime date)
    {
      return date.Second;
    }

    /// <summary>
    /// Gets the week of the year.
    /// </summary>
    /// <param name="date">The date value.</param>
    /// <returns>The week of the year.</returns>
    public static int WeekOfYear(DateTime date)
    {
        CalendarWeekRule rule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
        DayOfWeek day = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, rule, day);
    }

    /// <summary>
    /// Gets the year component of the date.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The year component.</returns>
    public static int Year(DateTime date)
    {
      return date.Year;
    }
    #endregion
    
    #region Formatting
    /// <summary>
    /// Replaces the format item in a specified String with the text equivalent of the value of a 
    /// corresponding Object instance in a specified array. 
    /// </summary>
    /// <param name="format">A String containing zero or more format items.</param>
    /// <param name="args">An Object array containing zero or more objects to format.</param>
    /// <returns>A copy of format in which the format items have been replaced by the String equivalent of the corresponding instances of Object in args.</returns>
    public static string Format(string format, params object[] args)
    {
      if (args != null)
      {
        for (int i = 0; i < args.Length; i++)
        {
          if (args[i] is Variant)
            args[i] = ((Variant)args[i]).Value;
        }
      }
      return String.Format(format, args);
    }

    /// <summary>
    /// Returns a string formatted as a currency value.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatCurrency(object value)
    {
      if (value is Variant)
        value = ((Variant)value).Value;
      return String.Format("{0:c}", value);
    }

    /// <summary>
    /// Returns a string formatted as a currency value with specified number of decimal digits.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="decimalDigits">Number of decimal digits.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatCurrency(object value, int decimalDigits)
    {
      if (value is Variant)
        value = ((Variant)value).Value;
      NumberFormatInfo info = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
      info.CurrencyDecimalDigits = decimalDigits;
      return String.Format(info, "{0:c}", value);
    }

    /// <summary>
    /// Returns a string formatted as a date/time value.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatDateTime(DateTime value)
    {
      string format = "G";
      if (value.TimeOfDay.Ticks == value.Ticks)
        format = "T";
      else if (value.TimeOfDay.Ticks == 0)
        format = "d";
      return value.ToString(format, null);
    }

    /// <summary>
    /// Returns a string formatted as a date/time value.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="format">The format specifier, one of the 
    /// "Long Date", "Short Date", "Long Time", "Short Time" values.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatDateTime(DateTime value, string format)
    {
      string _format = format.ToLower().Replace(" ", "");
      switch (_format)
      {
        case "longdate":
          return value.ToString("D", null);
        case "shortdate":
          return value.ToString("d", null);
        case "longtime":
          return value.ToString("T", null);
        case "shorttime":
          return value.ToString("HH:mm", null);
      }
      return value.ToString(format, null);
    }

    /// <summary>
    /// Returns a string formatted as a numeric value.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatNumber(object value)
    {
      if (value is Variant)
        value = ((Variant)value).Value;
      return String.Format("{0:n}", value);
    }

    /// <summary>
    /// Returns a string formatted as a numeric value with specified number of decimal digits.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="decimalDigits">Number of decimal digits.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatNumber(object value, int decimalDigits)
    {
      if (value is Variant)
        value = ((Variant)value).Value;
      NumberFormatInfo info = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
      info.NumberDecimalDigits = decimalDigits;
      return String.Format(info, "{0:n}", value);
    }

    /// <summary>
    /// Returns a string formatted as a percent value.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatPercent(object value)
    {
      if (value is Variant)
        value = ((Variant)value).Value;
      return String.Format("{0:p}", value);
    }

    /// <summary>
    /// Returns a string formatted as a percent value with specified number of decimal digits.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <param name="decimalDigits">Number of decimal digits.</param>
    /// <returns>The formatted string.</returns>
    public static string FormatPercent(object value, int decimalDigits)
    {
      if (value is Variant)
        value = ((Variant)value).Value;
      NumberFormatInfo info = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
      info.PercentDecimalDigits = decimalDigits;
      return String.Format(info, "{0:p}", value);
    }
    #endregion

    #region Conversion
    /// <summary>
    /// Converts a numeric value to Roman string representation.
    /// </summary>
    /// <param name="value">Integer value in range 0-3998.</param>
    /// <returns>The string in Roman form.</returns>
    public static string ToRoman(object value)
    {
      return Roman.Convert(Convert.ToInt32(value));
    }
    
    /// <summary>
    /// Converts a currency value to an english (US) string representation of that value.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWords(object value)
    {
      return ToWords(value, "USD");
    }

    /// <summary>
    /// Converts a currency value to an english (US) string representation of that value, 
    /// using the specified currency.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWords(object value, string currencyName)
    {
      return new NumToWordsEn().ConvertCurrency(Convert.ToDecimal(value), currencyName);
    }

    /// <summary>
    /// Converts a numeric value to an english (US) string representation of that value.
    /// </summary>
    /// <param name="value">The numeric value to convert.</param>
    /// <param name="one">The name in singular form, for example "page".</param>
    /// <param name="many">The name in plural form, for example "pages".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWords(object value, string one, string many)
    {
      return new NumToWordsEn().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
    }

    /// <summary>
    /// Converts a currency value to an english (GB) string representation of that value.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsEnGb(object value)
    {
      return ToWordsEnGb(value, "GBP");
    }

    /// <summary>
    /// Converts a currency value to an english (GB) string representation of that value, 
    /// using the specified currency.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsEnGb(object value, string currencyName)
    {
      return new NumToWordsEnGb().ConvertCurrency(Convert.ToDecimal(value), currencyName);
    }

    /// <summary>
    /// Converts a numeric value to an english (GB) string representation of that value.
    /// </summary>
    /// <param name="value">The numeric value to convert.</param>
    /// <param name="one">The name in singular form, for example "page".</param>
    /// <param name="many">The name in plural form, for example "pages".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsEnGb(object value, string one, string many)
    {
      return new NumToWordsEnGb().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
    }

    /// <summary>
    /// Converts a currency value to a spanish string representation of that value.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsEs(object value)
    {
      return ToWordsEs(value, "EUR");
    }

    /// <summary>
    /// Converts a currency value to a spanish string representation of that value, 
    /// using the specified currency.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsEs(object value, string currencyName)
    {
      return new NumToWordsEs().ConvertCurrency(Convert.ToDecimal(value), currencyName);
    }

    /// <summary>
    /// Converts a numeric value to a spanish string representation of that value.
    /// </summary>
    /// <param name="value">The numeric value to convert.</param>
    /// <param name="one">The name in singular form, for example "page".</param>
    /// <param name="many">The name in plural form, for example "pages".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsEs(object value, string one, string many)
    {
      return new NumToWordsEs().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
    }

    /// <summary>
    /// Converts a currency value to a russian string representation of that value.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsRu(object value)
    {
      return ToWordsRu(value, "RUR");
    }

    /// <summary>
    /// Converts a currency value to a russian string representation of that value, 
    /// using the specified currency.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsRu(object value, string currencyName)
    {
      return new NumToWordsRu().ConvertCurrency(Convert.ToDecimal(value), currencyName);
    }

    /// <summary>
    /// Converts a numeric value to a russian string representation of that value.
    /// </summary>
    /// <param name="value">The numeric value to convert.</param>
    /// <param name="male">True if the name is of male gender.</param>
    /// <param name="one">The name in singular form, for example "страница".</param>
    /// <param name="two">The name in plural form, for example "страницы".</param>
    /// <param name="many">The name in plural form, for example "страниц".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsRu(object value, bool male, string one, string two, string many)
    {
      return new NumToWordsRu().ConvertNumber(Convert.ToDecimal(value), male, one, two, many);
    }

    /// <summary>
    /// Converts a currency value to a german string representation of that value.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsDe(object value)
    {
        return ToWordsDe(value, "EUR");
    }

    /// <summary>
    /// Converts a currency value to a german string representation of that value, 
    /// using the specified currency.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsDe(object value, string currencyName)
    {
        return new NumToWordsDe().ConvertCurrency(Convert.ToDecimal(value), currencyName);
    }

    /// <summary>
    /// Converts a numeric value to a german string representation of that value.
    /// </summary>
    /// <param name="value">The numeric value to convert.</param>
    /// <param name="one">The name in singular form, for example "page".</param>
    /// <param name="many">The name in plural form, for example "pages".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsDe(object value, string one, string many)
    {
        return new NumToWordsDe().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
    }

    /// <summary>
    /// Converts a currency value to a french string representation of that value.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsFr(object value)
    {
        return ToWordsFr(value, "EUR");
    }

    /// <summary>
    /// Converts a currency value to a french string representation of that value, 
    /// using the specified currency.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsFr(object value, string currencyName)
    {
        return new NumToWordsFr().ConvertCurrency(Convert.ToDecimal(value), currencyName);
    }

    /// <summary>
    /// Converts a numeric value to a french string representation of that value.
    /// </summary>
    /// <param name="value">The numeric value to convert.</param>
    /// <param name="one">The name in singular form, for example "page".</param>
    /// <param name="many">The name in plural form, for example "pages".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsFr(object value, string one, string many)
    {
        return new NumToWordsFr().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
    }

    /// <summary>
    /// Converts a currency value to a dutch string representation of that value.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsNl(object value)
    {
        return ToWordsNl(value, "EUR");
    }

    /// <summary>
    /// Converts a currency value to a dutch string representation of that value, 
    /// using the specified currency.
    /// </summary>
    /// <param name="value">The currency value to convert.</param>
    /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsNl(object value, string currencyName)
    {
        return new NumToWordsNl().ConvertCurrency(Convert.ToDecimal(value), currencyName);
    }

    /// <summary>
    /// Converts a numeric value to a dutch string representation of that value.
    /// </summary>
    /// <param name="value">The numeric value to convert.</param>
    /// <param name="one">The name in singular form, for example "page".</param>
    /// <param name="many">The name in plural form, for example "pages".</param>
    /// <returns>The string representation of the specified value.</returns>
    public static string ToWordsNl(object value, string one, string many)
    {
        return new NumToWordsNl().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
    }

        /// <summary>
        /// Converts a numeric value to a indian numbering system string representation of that value.
        /// </summary>
        /// <param name="value">the currency value to convert</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsIn(object value)
        {
            return ToWordsIn(value, "INR");
        }
        /// <summary>
        /// Converts a numeric value to a indian numbering system string representation of that value.
        /// </summary>
        /// <param name="value">he numeric value to convert.</param>
        /// <param name="currencyName">The 3-digit ISO name of the currency, for example "INR".</param>
        /// <returns></returns>
        public static string ToWordsIn(object value,string currencyName)
        {
            return new NumToWordsIn().ConvertCurrency(Convert.ToDecimal(value), currencyName);
        }

        /// <summary>
        /// Converts a numeric value to a indian numbering system string representation of that value.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <param name="one">The name in singular form, for example "page".</param>
        /// <param name="many">The name in plural form, for example "pages".</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsIn(object value, string one, string many)
        {
            return new NumToWordsIn().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
        }

        /// <summary>
        /// Converts a numeric value to a ukrainian string representation of that value.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsUkr(object value)
        {
            return ToWordsUkr(value,"UAH");
        }


        /// <summary>
        /// Converts a currency value to a ukrainian string representation of that value, 
        /// using the specified currency.
        /// </summary>
        /// <param name="value">The currency value to convert.</param>
        /// <param name="currencyName">The 3-digit ISO name of the currency, for example "UAH".</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsUkr(object value, string currencyName)
        {
            return new NumToWordsUkr().ConvertCurrency(Convert.ToDecimal(value), currencyName);
        }

        /// <summary>
        /// Converts a numeric value to a ukrainian string representation of that value.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <param name="male">True if the name is of male gender.</param>
        /// <param name="one">The name in singular form, for example "сторінка".</param>
        /// <param name="two">The name in plural form, for example "сторінки".</param>
        /// <param name="many">The name in plural form, for example "сторінок".</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsUkr(object value, bool male, string one, string two, string many)
        {
            return new NumToWordsUkr().ConvertNumber(Convert.ToDecimal(value), male, one, two, many);
        }


        /// <summary>
        /// Converts a numeric value to a spanish string representation of that value.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsSp(object value)
        {
            return ToWordsSp(value, "EUR");
        }

        /// <summary>
        /// Converts a numeric value to a spanish representation of that value.
        /// </summary>
        /// <param name="value">he numeric value to convert.</param>
        /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
        /// <returns></returns>
        public static string ToWordsSp(object value, string currencyName)
        {
            return new NumToWordsSp().ConvertCurrency(Convert.ToDecimal(value), currencyName);
        }

        /// <summary>
        /// Converts a numeric value to a spanish string representation of that value.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <param name="male">True if the name is of male gender.</param>
        /// <param name="one">The name in singular form, for example "silla".</param>
        /// <param name="two">The name in plural form, for example "Sillas".</param>
        /// <param name="many">The name in plural form, for example "Sillas".</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsSp(object value, string one, string many)
        {
            return new NumToWordsSp().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
        }

		/// <summary>
        /// Converts a numeric value to a persian string representation of that value.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsPersian(object value)
        {
            return ToWordsPersian(value, "EUR");
        }

        /// <summary>
        /// Converts a numeric value to a persian representation of that value.
        /// </summary>
        /// <param name="value">he numeric value to convert.</param>
        /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
        /// <returns></returns>
        public static string ToWordsPersian(object value, string currencyName)
        {
            return new NumToWordsPersian().ConvertCurrency(Convert.ToDecimal(value), currencyName);
        }

        /// <summary>
        /// Converts a numeric value to a persian string representation of that value.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <param name="male">True if the name is of male gender.</param>
        /// <param name="one">The name in singular form, for example "silla".</param>
        /// <param name="two">The name in plural form, for example "Sillas".</param>
        /// <param name="many">The name in plural form, for example "Sillas".</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsPersian(object value, string one, string many)
        {
            return new NumToWordsPersian().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
        }

        /// <summary>
        /// Converts a numeric value to a polish string representation of that value.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsPl(object value)
        {
            return ToWordsPl(value, "PLN");
        }

        /// <summary>
        /// Converts a numeric value to a polish representation of that value.
        /// </summary>
        /// <param name="value">he numeric value to convert.</param>
        /// <param name="currencyName">The 3-digit ISO name of the currency, for example "EUR".</param>
        /// <returns></returns>
        public static string ToWordsPl(object value, string currencyName)
        {
            return new NumToWordsPl().ConvertCurrency(Convert.ToDecimal(value), currencyName);
        }

        /// <summary>
        /// Converts a numeric value to a polish string representation of that value.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <param name="male">True if the name is of male gender.</param>
        /// <param name="one">The name in singular form, for example "silla".</param>
        /// <param name="two">The name in plural form, for example "Sillas".</param>
        /// <param name="many">The name in plural form, for example "Sillas".</param>
        /// <returns>The string representation of the specified value.</returns>
        public static string ToWordsPl(object value, string one, string many)
        {
            return new NumToWordsPl().ConvertNumber(Convert.ToDecimal(value), true, one, many, many);
        }

        /// <summary>
        /// Converts a value to an english (US) alphabet string representation of that value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The alphabet string representation of the specified value.</returns>
        public static string ToLetters(object value)
        {
            return ToLetters(value, false);
        }

        /// <summary>
        /// Converts a value to an english (US) alphabet string representation of that value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="isUpper">Bool indicating that letters should be in upper registry.</param>
        /// <returns>The alphabet string representation of the specified value.</returns>
        public static string ToLetters(object value, bool isUpper)
        {
            return new NumToLettersEn().ConvertNumber(Convert.ToInt32(value), isUpper);
        }

        /// <summary>
        /// Converts a value to a russian alphabet string representation of that value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The alphabet string representation of the specified value.</returns>
        public static string ToLettersRu(object value)
        {
            return ToLettersRu(value, false);
        }

        /// <summary>
        /// Converts a value to a russian alphabet string representation of that value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="isUpper">Bool indicating that letters should be in upper registry.</param>
        /// <returns>The alphabet string representation of the specified value.</returns>
        public static string ToLettersRu(object value, bool isUpper)
        {
            return new NumToLettersRu().ConvertNumber(Convert.ToInt32(value), isUpper);
        }

        #endregion

        #region Program Flow
        /// <summary>
        /// Selects and returns a value from a list of arguments.
        /// </summary>
        /// <param name="index">A value between 1 and the number of elements passed in the "choice" argument.</param>
        /// <param name="choice">Object parameter array.</param>
        /// <returns>One of the values in the "choice" argument.</returns>
        public static object Choose(double index, params object[] choice)
    {
      int ind = (int)index - 1;
      if (ind < 0 || ind >= choice.Length)
        return null;
      return choice[ind];
    }

    /// <summary>
    /// Returns one of two objects, depending on the evaluation of an expression.
    /// </summary>
    /// <param name="expression">The expression you want to evaluate.</param>
    /// <param name="truePart">Returned if Expression evaluates to True.</param>
    /// <param name="falsePart">Returned if Expression evaluates to False.</param>
    /// <returns>Either truePart os falsePart.</returns>
    public static object IIf(bool expression, object truePart, object falsePart)
    {
      return expression ? truePart : falsePart;
    }

    /// <summary>
    /// Evaluates a list of expressions and returns a value corresponding to the first 
    /// expression in the list that is True.
    /// </summary>
    /// <param name="expressions">Parameter array consists of paired expressions and values.</param>
    /// <returns>The value corresponding to an expression which returns true.</returns>
    public static object Switch(params object[] expressions)
    {
      for (int i = 0; i + 1 < expressions.Length; i += 2)
      {
        if (Convert.ToBoolean(expressions[i]) == true)
          return expressions[i + 1];
      }
      return null;
    }

    /// <summary>
    /// Checks if the specified object is null.
    /// </summary>
    /// <param name="thisReport">The report instance.</param>
    /// <param name="name">Either a name of DB column, or a parameter name, or a total name to check.</param>
    /// <returns><b>true</b> if the object's value is null.</returns>
    public static bool IsNull(Report thisReport, string name)
    {
      object value = null;
      if (DataHelper.IsValidColumn(thisReport.Dictionary, name))
      {
        value = thisReport.GetColumnValueNullable(name);
      }
      else if (DataHelper.IsValidParameter(thisReport.Dictionary, name))
      {
        value = thisReport.GetParameterValue(name);
      }
      else if (DataHelper.IsValidTotal(thisReport.Dictionary, name))
      {
        value = thisReport.GetTotalValueNullable(name).Value;
      }
      return value == null || value == DBNull.Value;
    }
    #endregion

    internal static void Register()
    {
      #region Math
      RegisteredObjects.AddFunctionCategory("Math", "Functions,Math");
      Type math = typeof(Math);
      RegisteredObjects.AddFunction(math.GetMethod("Abs", new Type[] { typeof(sbyte) }), "Math,Abs");
      RegisteredObjects.AddFunction(math.GetMethod("Abs", new Type[] { typeof(short) }), "Math,Abs");
      RegisteredObjects.AddFunction(math.GetMethod("Abs", new Type[] { typeof(int) }), "Math,Abs");
      RegisteredObjects.AddFunction(math.GetMethod("Abs", new Type[] { typeof(long) }), "Math,Abs");
      RegisteredObjects.AddFunction(math.GetMethod("Abs", new Type[] { typeof(float) }), "Math,Abs");
      RegisteredObjects.AddFunction(math.GetMethod("Abs", new Type[] { typeof(double) }), "Math,Abs");
      RegisteredObjects.AddFunction(math.GetMethod("Abs", new Type[] { typeof(decimal) }), "Math,Abs");
      RegisteredObjects.AddFunction(math.GetMethod("Acos"), "Math");
      RegisteredObjects.AddFunction(math.GetMethod("Asin"), "Math");
      RegisteredObjects.AddFunction(math.GetMethod("Atan"), "Math");
      RegisteredObjects.AddFunction(math.GetMethod("Ceiling", new Type[] { typeof(double) }), "Math,Ceiling");
      RegisteredObjects.AddFunction(math.GetMethod("Ceiling", new Type[] { typeof(decimal) }), "Math,Ceiling");
      RegisteredObjects.AddFunction(math.GetMethod("Cos"), "Math");
      RegisteredObjects.AddFunction(math.GetMethod("Exp"), "Math");
      RegisteredObjects.AddFunction(math.GetMethod("Floor", new Type[] { typeof(double) }), "Math,Floor");
      RegisteredObjects.AddFunction(math.GetMethod("Floor", new Type[] { typeof(decimal) }), "Math,Floor");
      RegisteredObjects.AddFunction(math.GetMethod("Log", new Type[] { typeof(double) }), "Math");
      Type myMath = typeof(StdFunctions);
      RegisteredObjects.AddFunction(myMath.GetMethod("Maximum", new Type[] { typeof(int), typeof(int) }), "Math,Maximum");
      RegisteredObjects.AddFunction(myMath.GetMethod("Maximum", new Type[] { typeof(long), typeof(long) }), "Math,Maximum");
      RegisteredObjects.AddFunction(myMath.GetMethod("Maximum", new Type[] { typeof(float), typeof(float) }), "Math,Maximum");
      RegisteredObjects.AddFunction(myMath.GetMethod("Maximum", new Type[] { typeof(double), typeof(double) }), "Math,Maximum");
      RegisteredObjects.AddFunction(myMath.GetMethod("Maximum", new Type[] { typeof(decimal), typeof(decimal) }), "Math,Maximum");
      RegisteredObjects.AddFunction(myMath.GetMethod("Minimum", new Type[] { typeof(int), typeof(int) }), "Math,Minimum");
      RegisteredObjects.AddFunction(myMath.GetMethod("Minimum", new Type[] { typeof(long), typeof(long) }), "Math,Minimum");
      RegisteredObjects.AddFunction(myMath.GetMethod("Minimum", new Type[] { typeof(float), typeof(float) }), "Math,Minimum");
      RegisteredObjects.AddFunction(myMath.GetMethod("Minimum", new Type[] { typeof(double), typeof(double) }), "Math,Minimum");
      RegisteredObjects.AddFunction(myMath.GetMethod("Minimum", new Type[] { typeof(decimal), typeof(decimal) }), "Math,Minimum");
      RegisteredObjects.AddFunction(math.GetMethod("Round", new Type[] { typeof(double) }), "Math,Round");
      RegisteredObjects.AddFunction(math.GetMethod("Round", new Type[] { typeof(decimal) }), "Math,Round");
      RegisteredObjects.AddFunction(math.GetMethod("Round", new Type[] { typeof(double), typeof(int) }), "Math,Round");
      RegisteredObjects.AddFunction(math.GetMethod("Round", new Type[] { typeof(decimal), typeof(int) }), "Math,Round");
      RegisteredObjects.AddFunction(math.GetMethod("Sin"), "Math");
      RegisteredObjects.AddFunction(math.GetMethod("Sqrt"), "Math");
      RegisteredObjects.AddFunction(math.GetMethod("Tan"), "Math");
      RegisteredObjects.AddFunction(math.GetMethod("Truncate", new Type[] { typeof(double) }), "Math,Truncate");
      RegisteredObjects.AddFunction(math.GetMethod("Truncate", new Type[] { typeof(decimal) }), "Math,Truncate");
      #endregion

      #region Text
      RegisteredObjects.AddFunctionCategory("Text", "Functions,Text");
      Type str = typeof(StdFunctions);
      RegisteredObjects.AddFunction(str.GetMethod("Asc"), "Text");
      RegisteredObjects.AddFunction(str.GetMethod("Chr"), "Text");
      RegisteredObjects.AddFunction(str.GetMethod("Insert"), "Text");
      RegisteredObjects.AddFunction(str.GetMethod("Length"), "Text");
      RegisteredObjects.AddFunction(str.GetMethod("LowerCase"), "Text");
      RegisteredObjects.AddFunction(str.GetMethod("PadLeft", new Type[] { typeof(string), typeof(int) }), "Text,PadLeft");
      RegisteredObjects.AddFunction(str.GetMethod("PadLeft", new Type[] { typeof(string), typeof(int), typeof(char) }), "Text,PadLeft");
      RegisteredObjects.AddFunction(str.GetMethod("PadRight", new Type[] { typeof(string), typeof(int) }), "Text,PadRight");
      RegisteredObjects.AddFunction(str.GetMethod("PadRight", new Type[] { typeof(string), typeof(int), typeof(char) }), "Text,PadRight");
      RegisteredObjects.AddFunction(str.GetMethod("Remove", new Type[] { typeof(string), typeof(int) }), "Text,Remove");
      RegisteredObjects.AddFunction(str.GetMethod("Remove", new Type[] { typeof(string), typeof(int), typeof(int) }), "Text,Remove");
      RegisteredObjects.AddFunction(str.GetMethod("Replace"), "Text");
      RegisteredObjects.AddFunction(str.GetMethod("Substring", new Type[] { typeof(string), typeof(int) }), "Text,Substring");
      RegisteredObjects.AddFunction(str.GetMethod("Substring", new Type[] { typeof(string), typeof(int), typeof(int) }), "Text,Substring");
      RegisteredObjects.AddFunction(str.GetMethod("TitleCase"), "Text");
      RegisteredObjects.AddFunction(str.GetMethod("Trim"), "Text");
      RegisteredObjects.AddFunction(str.GetMethod("UpperCase"), "Text");
      #endregion

      #region Date & Time
      RegisteredObjects.AddFunctionCategory("DateTime", "Functions,DateTime");
      Type dt = typeof(StdFunctions);
      RegisteredObjects.AddFunction(dt.GetMethod("AddDays"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("AddHours"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("AddMinutes"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("AddMonths"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("AddSeconds"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("AddYears"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("DateDiff"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("DateSerial"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("Day"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("DayOfWeek"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("DayOfYear"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("DaysInMonth"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("Hour"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("Minute"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("Month"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("MonthName"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("Second"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("WeekOfYear"), "DateTime");
      RegisteredObjects.AddFunction(dt.GetMethod("Year"), "DateTime");
      #endregion
      
      #region Formatting
      RegisteredObjects.AddFunctionCategory("Formatting", "Functions,Formatting");
      Type fmt = typeof(StdFunctions);
      RegisteredObjects.AddFunction(fmt.GetMethod("Format", new Type[] { typeof(string), typeof(object[]) }), "Formatting");
      RegisteredObjects.AddFunction(fmt.GetMethod("FormatCurrency", new Type[] { typeof(object) }), "Formatting,FormatCurrency");
      RegisteredObjects.AddFunction(fmt.GetMethod("FormatCurrency", new Type[] { typeof(object), typeof(int) }), "Formatting,FormatCurrency");
      RegisteredObjects.AddFunction(fmt.GetMethod("FormatDateTime", new Type[] { typeof(DateTime) }), "Formatting,FormatDateTime");
      RegisteredObjects.AddFunction(fmt.GetMethod("FormatDateTime", new Type[] { typeof(DateTime), typeof(string) }), "Formatting,FormatDateTime");
      RegisteredObjects.AddFunction(fmt.GetMethod("FormatNumber", new Type[] { typeof(object) }), "Formatting,FormatNumber");
      RegisteredObjects.AddFunction(fmt.GetMethod("FormatNumber", new Type[] { typeof(object), typeof(int) }), "Formatting,FormatNumber");
      RegisteredObjects.AddFunction(fmt.GetMethod("FormatPercent", new Type[] { typeof(object) }), "Formatting,FormatPercent");
      RegisteredObjects.AddFunction(fmt.GetMethod("FormatPercent", new Type[] { typeof(object), typeof(int) }), "Formatting,FormatPercent");
      #endregion

      #region Conversion
      RegisteredObjects.AddFunctionCategory("Conversion", "Functions,Conversion");
      Type stdConv = typeof(Convert);
      Type myConv = typeof(StdFunctions);
      RegisteredObjects.AddFunction(stdConv.GetMethod("ToBoolean", new Type[] { typeof(object) }), "Conversion");
      RegisteredObjects.AddFunction(stdConv.GetMethod("ToByte", new Type[] { typeof(object) }), "Conversion");
      RegisteredObjects.AddFunction(stdConv.GetMethod("ToChar", new Type[] { typeof(object) }), "Conversion");
      RegisteredObjects.AddFunction(stdConv.GetMethod("ToDateTime", new Type[] { typeof(object) }), "Conversion");
      RegisteredObjects.AddFunction(stdConv.GetMethod("ToDecimal", new Type[] { typeof(object) }), "Conversion");
      RegisteredObjects.AddFunction(stdConv.GetMethod("ToDouble", new Type[] { typeof(object) }), "Conversion");
      RegisteredObjects.AddFunction(stdConv.GetMethod("ToInt32", new Type[] { typeof(object) }), "Conversion");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToRoman"), "Conversion");
      RegisteredObjects.AddFunction(stdConv.GetMethod("ToSingle", new Type[] { typeof(object) }), "Conversion");
      RegisteredObjects.AddFunction(stdConv.GetMethod("ToString", new Type[] { typeof(object) }), "Conversion");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWords", new Type[] { typeof(object) }), "Conversion,ToWords");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWords", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWords");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWords", new Type[] { typeof(object), typeof(string), typeof(string) }), "Conversion,ToWords");
            RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsIn", new Type[] { typeof(object) }), "Conversion,ToWordsIn");
            RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsIn", new Type[] { typeof(object),typeof(string) }), "Conversion,ToWordsIn");
            RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsIn", new Type[] { typeof(object), typeof(string),typeof(string) }), "Conversion,ToWordsIn");
            RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsDe", new Type[] { typeof(object) }), "Conversion,ToWordsDe");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsDe", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsDe");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsDe", new Type[] { typeof(object), typeof(string), typeof(string) }), "Conversion,ToWordsDe");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsEnGb", new Type[] { typeof(object) }), "Conversion,ToWordsEnGb");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsEnGb", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsEnGb");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsEnGb", new Type[] { typeof(object), typeof(string), typeof(string) }), "Conversion,ToWordsEnGb");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsEs", new Type[] { typeof(object) }), "Conversion,ToWordsEs");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsEs", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsEs");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsEs", new Type[] { typeof(object), typeof(string), typeof(string) }), "Conversion,ToWordsEs");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsFr", new Type[] { typeof(object) }), "Conversion,ToWordsFr");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsFr", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsFr");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsFr", new Type[] { typeof(object), typeof(string), typeof(string) }), "Conversion,ToWordsFr");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsNl", new Type[] { typeof(object) }), "Conversion,ToWordsNl");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsNl", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsNl");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsNl", new Type[] { typeof(object), typeof(string), typeof(string) }), "Conversion,ToWordsNl");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsRu", new Type[] { typeof(object) }), "Conversion,ToWordsRu");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsRu", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsRu");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsRu", new Type[] { typeof(object), typeof(bool), typeof(string), typeof(string), typeof(string) }), "Conversion,ToWordsRu");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsUkr",new Type[]{typeof(object)}),"Conversion,ToWordsUkr");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsUkr", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsUkr");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsUkr", new Type[] { typeof(object), typeof(bool), typeof(string), typeof(string), typeof(string) }), "Conversion,ToWordsUkr");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsSp", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsSp");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsSp", new Type[] { typeof(object) }), "Conversion,ToWordsSp");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsSp", new Type[] { typeof(object), typeof(string), typeof(string) }), "Conversion,ToWordsSp");
	  RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsPersian", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsPersian");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsPersian", new Type[] { typeof(object) }), "Conversion,ToWordsPersian");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsPersian", new Type[] { typeof(object), typeof(string), typeof(string) }), "Conversion,ToWordsPersian");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToLetters", new Type[] { typeof(object) }), "Conversion,ToLetters");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToLetters", new Type[] { typeof(object), typeof(bool) }), "Conversion,ToLetters");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToLettersRu", new Type[] { typeof(object) }), "Conversion,ToLettersRu");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToLettersRu", new Type[] { typeof(object), typeof(bool) }), "Conversion,ToLettersRu");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsPl", new Type[] { typeof(object), typeof(string) }), "Conversion,ToWordsPl");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsPl", new Type[] { typeof(object) }), "Conversion,ToWordsPl");
      RegisteredObjects.AddFunction(myConv.GetMethod("ToWordsPl", new Type[] { typeof(object), typeof(string), typeof(string) }), "Conversion,ToWordsPl");
            #endregion

            #region Program Flow
            RegisteredObjects.AddFunctionCategory("ProgramFlow", "Functions,ProgramFlow");
      Type misc = typeof(StdFunctions);
      RegisteredObjects.AddFunction(misc.GetMethod("Choose"), "ProgramFlow");
      RegisteredObjects.AddFunction(misc.GetMethod("IIf"), "ProgramFlow");
      RegisteredObjects.AddFunction(misc.GetMethod("Switch"), "ProgramFlow");
      RegisteredObjects.AddFunction(misc.GetMethod("IsNull"), "ProgramFlow");
      #endregion

    }
  }
}
