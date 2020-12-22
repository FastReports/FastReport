using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Collections.Specialized;
using System.Text;

namespace FastReport.Utils
{
  /// <summary>
  /// Contains methods that peform string to object and vice versa conversions.
  /// </summary>
  public static class Converter
  {

        
    /// <summary>
    /// Converts an object to a string.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <returns>The string that contains the converted value.</returns>
    public static string ToString(object value)
    {
      if (value == null)
        return "";
      if (value is string)
        return (string)value;
      if (value is float)
        return ((float)value).ToString(CultureInfo.InvariantCulture.NumberFormat);
      if (value is double)
        return ((double)value).ToString(CultureInfo.InvariantCulture.NumberFormat);
      if (value is Enum)
        return Enum.Format(value.GetType(), value, "G");
      if (value is Image)
      {
        using (MemoryStream stream = new MemoryStream())
        {
          ImageHelper.Save(value as Image, stream);
          return Convert.ToBase64String(stream.ToArray());
        }
      }
      if (value is Stream)
      {
        Stream stream = value as Stream;
        byte[] bytes = new byte[stream.Length];
        stream.Position = 0;
        stream.Read(bytes, 0, bytes.Length);
        return Convert.ToBase64String(bytes);
      }
      if (value is byte[])
      {
        return Convert.ToBase64String((byte[])value);
      }
      if (value is string[])
      {
        string result = "";
        foreach (string s in (value as string[]))
        {
          result += s + "\r\n";
        }
        if (result.EndsWith("\r\n"))
          result = result.Remove(result.Length - 2);
        return result;
      }
      if (value is Type)
      {
        Type type = (Type)value;
        bool shortTypeName = type.Assembly.FullName.StartsWith("mscorlib") ||
          type.Assembly == typeof(Converter).Assembly || type.Assembly.FullName.StartsWith("System.Private.CoreLib");

        if (shortTypeName)
          return type.FullName;
        return type.AssemblyQualifiedName;
      }
#if true// | NETSTANDARD2_0 || NETSTANDARD2_1
            if (value is Font)
            {
                return FastReport.TypeConverters.FontConverter.Instance.ConvertToInvariantString(value);
            }
#endif
            if(value is System.Drawing.Imaging.ImageFormat)
            {
                var imageFormat = value as System.Drawing.Imaging.ImageFormat;
                return imageFormat.ToString();
            }
            return TypeDescriptor.GetConverter(value).ConvertToInvariantString(value);
    }

    /// <summary>
    /// Converts a value to a string using the specified converter.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <param name="converterType">The type of converter.</param>
    /// <returns>The string that contains the converted value.</returns>
    public static string ToString(object value, Type converterType)
    {
      TypeConverter converter = Activator.CreateInstance(converterType) as TypeConverter;
      return converter.ConvertToInvariantString(value);
    }

    /// <summary>
    /// Converts a string value to the specified data type.
    /// </summary>
    /// <param name="type">The data type to convert to.</param>
    /// <param name="value">The string to convert from.</param>
    /// <returns>The object of type specified in the <b>type</b> parameter that contains 
    /// a converted value.</returns>
    public static object FromString(Type type, string value)
    {
      if (type == typeof(string))
        return value;
      if (value == null || value == "")
        return null;
      if (type == typeof(float))
        return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
      if (type == typeof(double))
        return double.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
      if (type == typeof(Enum))
        return Enum.Parse(type, value);
      if (type == typeof(Image) || type == typeof(Bitmap))
        return ImageHelper.Load(Convert.FromBase64String(value));
      if (type == typeof(Stream))
        return new MemoryStream(Convert.FromBase64String(value));
      if (type == typeof(byte[]))
        return Convert.FromBase64String(value);
      if (type == typeof(Type))
        return Type.GetType(value);
      if (type == typeof(string[]))
      {
        value = value.Replace("\r\n", "\r");
        return value.Split(new char[] { '\r' });
      }
#if true //NETSTANDARD2_0 || NETSTANDARD2_1
            if (type == typeof(Font))
            {
                return FastReport.TypeConverters.FontConverter.Instance.ConvertFromInvariantString(value);
            }
#endif
      if (type == typeof(Color))
        return new ColorConverter().ConvertFromInvariantString(value);
      return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
    }

    /// <summary>
    /// Converts a string to an object using the specified converter.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <param name="converterType">The type of converter.</param>
    /// <returns>The object that contains the converted value.</returns>
    public static object FromString(string value, Type converterType)
    {
      TypeConverter converter = Activator.CreateInstance(converterType) as TypeConverter;
      return converter.ConvertFromInvariantString(value);
    }

    /// <summary>
    /// Converts a string containing special symbols to the xml-compatible string.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>The result string.</returns>
    /// <remarks>
    /// This method replaces some special symbols like &lt;, &gt; into xml-compatible 
    /// form: &amp;lt;, &amp;gt;. To convert such string back to original form, use the
    /// <see cref="FromXml"/> method.
    /// </remarks>
    public static string ToXml(string s)
    {
      return ToXml(s, true);
    }

    /// <summary>
    /// Converts a string containing special symbols to the xml-compatible string.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <param name="convertCrlf">Determines whether it is necessary to convert cr-lf symbols to xml form.</param>
    /// <returns>The result string.</returns>
    public static string ToXml(string s, bool convertCrlf)
    {
      FastString result = new FastString(s.Length);
      for (int i = 0; i < s.Length; i++)
      {
        switch (s[i])
        {
          case '\n':
          case '\r':
            if (convertCrlf)
              result.Append("&#" + (int)s[i] + ";");
            else
              result.Append(s[i]);
            break;
          case '"':
            result.Append("&quot;");
            break;
          case '&':
            result.Append("&amp;");
            break;
          case '<':
            result.Append("&lt;");
            break;
          case '>':
            result.Append("&gt;");
            break;
          default:
            result.Append(s[i]);
            break;
        }
      }
      return result.ToString();
    }

    /// <summary>
    /// Converts a value to xml-compatible string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The result string.</returns>
    public static string ToXml(object value)
    {
      return ToXml(ToString(value));
    }

    /// <summary>
    /// Convert the xml-compatible string to the regular one.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>The result string.</returns>
    /// <remarks>
    /// This is counterpart to the <see cref="ToXml(string)"/> method.
    /// </remarks>
    public static string FromXml(string s)
    {
      FastString result = new FastString(s.Length);
      int i = 0;
      while (i < s.Length)
      {
        if (s[i] == '&')
        {
          if (i + 3 < s.Length && s[i + 1] == '#')
          {
            int j = i + 3;
            j = s.IndexOf(";", j, s.Length - j);
            if (j == -1)
                throw new FormatException();
            if (s[i + 2] == 'x')
            {
                char hexChar = (char)int.Parse(s.Substring(i + 3, j - i - 3), NumberStyles.AllowHexSpecifier);
                result.Append(hexChar);
            }
            else
            {
                result.Append((char)int.Parse(s.Substring(i + 2, j - i - 2)));
            }
            
            i = j;
          }
          else if (i + 5 < s.Length && s.Substring(i + 1, 5) == "quot;")
          {
            result.Append('"');
            i += 5;
          }
          else if (i + 4 < s.Length && s.Substring(i + 1, 4) == "amp;")
          {
            result.Append('&');
            i += 4;
          }
          else if (i + 3 < s.Length && s.Substring(i + 1, 3) == "lt;")
          {
            result.Append('<');
            i += 3;
          }
          else if (i + 3 < s.Length && s.Substring(i + 1, 3) == "gt;")
          {
            result.Append('>');
            i += 3;
          }
          else
            result.Append(s[i]);
        }
        else
          result.Append(s[i]);
        i++;
      }
      return result.ToString();
    }

    /// <summary>
    /// Decreases the precision of floating-point value.
    /// </summary>
    /// <param name="value">The initial value.</param>
    /// <param name="precision">The number of decimal digits in the fraction.</param>
    /// <returns>The value with lesser precision.</returns>
    public static float DecreasePrecision(float value, int precision)
    {
      return (float)Math.Round(value, precision);
    }

    /// <summary>
    /// Converts a string value to the float.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>The float value.</returns>
    /// <remarks>
    /// Both "." or "," decimal separators are allowed.
    /// </remarks>
    public static float StringToFloat(string value)
    {
      string currentSeparator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
      return StringToFloat(value, currentSeparator);
    }

    /// <summary>
    /// Converts a string value to the float.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <param name="removeNonDigit">Indicates whether to ignore non-digit symbols.</param>
    /// <returns>The float value.</returns>
    public static float StringToFloat(string value, bool removeNonDigit)
    {
      string currentSeparator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
      return StringToFloat(value, currentSeparator, removeNonDigit);
    }

    /// <summary>
    /// Converts a string value to the float.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <param name="separator">Decimal separator.</param>
    /// <returns>The float value.</returns>
    public static float StringToFloat(string value, string separator/*, string negativeSign*/)
    {
      return (float)FromString(typeof(float), value.Replace(separator, ".")/*.Replace(negativeSign, "-")*/);
    }

    /// <summary>
    /// Converts a string value to the float.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <param name="separator">Decimal separator.</param>
    /// <param name="removeNonDigit">Indicates whether to ignore non-digit symbols.</param>
    /// <returns>The float value.</returns>
    public static float StringToFloat(string value, string separator,/* string negativeSign,*/ bool removeNonDigit)
    {
      System.Text.StringBuilder result = new System.Text.StringBuilder();
      int separatorPos = value.IndexOf(separator);
      if (separatorPos == -1)
        separatorPos = value.IndexOf('.');
      if (value.Contains(/*negativeSign*/"-"))
        result.Append('-');
      for (int i = 0; i < value.Length; i++)
      {
        if (value[i] >= '0' && value[i] <= '9')
          result.Append(value[i]);
        if (i == separatorPos)
          result.Append('.');
        if (i < separatorPos + separator.Length)
          continue;
      }
      try
      {
        if (result.Length != 0)
          return (float)FromString(typeof(float), result.ToString());
      }
      catch
      {
      }
      return 0;
    }

    /// <summary>
    /// Converts the string containing several text lines to a collection of strings.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <param name="list">The collection instance.</param>
    public static void StringToIList(string text, IList list)
    {
      list.Clear();
      string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
      foreach (string s in lines)
      {
        list.Add(s);
      }
    }

    /// <summary>
    /// Converts a collection of strings to a string.
    /// </summary>
    /// <param name="list">The collection to convert.</param>
    /// <returns>The string that contains all lines from the collection.</returns>
    public static string IListToString(IList list)
    {
      string text = "";
      foreach (object obj in list)
      {
        text += obj.ToString() + "\r\n";
      }
      if (text.EndsWith("\r\n"))
        text = text.Remove(text.Length - 2);
      return text;
    }

    /// <summary>
    /// Converts <b>null</b> value to 0, false, empty string, depending on <b>type</b>.
    /// </summary>
    /// <param name="type">The data type.</param>
    /// <returns>The value of the <b>type</b> data type.</returns>
    public static object ConvertNull(Type type)
    {
      if (type == typeof(string))
        return "";
      else if (type == typeof(char))
        return ' ';
      else if (type == typeof(DateTime))
        return DateTime.MinValue;
      else if (type == typeof(TimeSpan))
        return TimeSpan.Zero;
      else if (type == typeof(Guid))
        return Guid.Empty;
      else if (type == typeof(bool))
        return false;
      else if (type == typeof(byte[]))
        return null;
      else if (type.IsClass)
        return null;
      else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        return null;
      return Convert.ChangeType(0, type);
    }

    /// <summary>
    /// Converts <b>string</b> value to <b>byte[]</b>.
    /// </summary>
    /// <param name="Str">The string to convert</param>
    /// <returns>The value of the <b>byte[]</b> data type.</returns>
    public static byte[] StringToByteArray(string Str)
    {
      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
      return encoding.GetBytes(Str);
    }


    /// <summary>
    /// Converts a string to NameValueCollection.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>The NameValueCollection that contains the name/value pairs.</returns>
    public static NameValueCollection StringToNameValueCollection(string text)
    {
      NameValueCollection collection = new NameValueCollection();
      int start = 0;
      while (start < text.Length)
      {
        // skip spaces
        for (; start < text.Length && text[start] == ' '; start++)
        {
        }
        // find '='
        int end = start;
        for (; end < text.Length && text[end] != '='; end++)
        {
        }
        // '=' not found, break
        if (end >= text.Length)
          break;

        // get name/value
        string name = text.Substring(start, end - start);
        string value = "";
        start = end + 1;
        // no value, break
        if (start >= text.Length)
          break;

        if (text[start] == '"')
        {
          // value is inside quotes, find trailing '"'
          start++;
          for (end = start; end < text.Length && text[end] != '"'; end++)
          {
          }
          // '"' not found, break
          if (end == text.Length)
            break;

          value = text.Substring(start, end - start);
          start = end + 1;
        }
        else
        {
          // get the value till ' ' or eol
          for (end = start; end < text.Length && text[end] != ' '; end++)
          {
          }

          value = text.Substring(start, end - start);
          start = end;
        }

        collection.Add(name, value);
      }

      return collection;
    }

    /// <summary>
    /// Convert &amp;amp;&amp;Tab;&amp;quot; etc to symbol and return result as string
    /// </summary>
    /// <param name="text">String for processing</param>
    /// <param name="position">Position for processing</param>
    /// <param name="result">Result of processing</param>
    /// <returns>True if successful</returns>
    public static bool FromHtmlEntities(string text, ref int position, out string result)
    {
        StringBuilder fastString = new StringBuilder(2);
      if (FromHtmlEntities(text, ref position, fastString))
      {
        result = fastString.ToString();
        return true;
      }
      else
      {
        result = "";
        return false;
      }
    }

    /// <summary>
    /// Convert &amp;amp;&amp;Tab;&amp;quot; etc to symbol and return result as string
    /// </summary>
    /// <param name="text">String for processing</param>
    /// <param name="position">Position for processing</param>
    /// <param name="result">Append result of processing to FastString</param>
    /// <returns>True if successful</returns>
    public static bool FromHtmlEntities(string text, ref int position, StringBuilder result)
    {
      if (text[position] != '&') return false;
      int end = text.IndexOf(';', position);
      if (end <= position + 1) return false;
      if (text[position + 1] == '#')
      {
        if (text[position + 2] == 'x')
        {
            //hex
            StringBuilder fastString = new StringBuilder(end - position);
          for (int i = position + 3; i < end; i++)
          {
            if (!Uri.IsHexDigit(text[i]))
              return false;
            fastString.Append(text[i]);
          }
          try
          {
            int value = Convert.ToInt32(fastString.ToString(), 16);
            if (value > char.MaxValue)
              result.Append(Char.ConvertFromUtf32(value));
            else
              result.Append((char)value);
            position = end;
            return true;
          }
          catch
          {
            return false;
          }
        }
        else
        {
          //dec
          FastString fastString = new FastString(end - position);
          for (int i = position + 2; i < end; i++)
          {
            if (!Char.IsDigit(text[i]))
              return false;
            fastString.Append(text[i]);
          }
          try
          {
            int value = Convert.ToInt32(fastString.ToString(), 10);
            if (value > char.MaxValue)
              result.Append(Char.ConvertFromUtf32(value));
            else
              result.Append((char)value);
            position = end;
            return true;
          }
          catch
          {
            return false;
          }
        }
      }
      int code = GetCode(text.Substring(position, end - position + 1 ));
      if (code == 0)
        return false;
      if (code > char.MaxValue)
        result.Append(Char.ConvertFromUtf32(code));
      else
        result.Append((char)code);
      position = end;
      return true;
    }

    private static int GetCode(string text)
    {
      switch(text)
      {
        case "&Tab;": return 9;
        case "&NewLine;": return 10;
        case "&excl;": return 33;
        case "&quot;": return 34;
        case "&QUOT;": return 34;
        case "&num;": return 35;
        case "&dollar;": return 36;
        case "&percnt;": return 37;
        case "&amp;": return 38;
        case "&AMP;": return 38;
        case "&apos;": return 39;
        case "&lpar;": return 40;
        case "&rpar;": return 41;
        case "&ast;": return 42;
        case "&midast;": return 42;
        case "&plus;": return 43;
        case "&comma;": return 44;
        case "&period;": return 46;
        case "&sol;": return 47;
        case "&colon;": return 58;
        case "&semi;": return 59;
        case "&lt;": return 60;
        case "&LT;": return 60;
        case "&equals;": return 61;
        case "&gt;": return 62;
        case "&GT;": return 62;
        case "&quest;": return 63;
        case "&commat;": return 64;
        case "&lsqb;": return 91;
        case "&lbrack;": return 91;
        case "&bsol;": return 92;
        case "&rsqb;": return 93;
        case "&rbrack;": return 93;
        case "&Hat;": return 94;
        case "&lowbar;": return 95;
        case "&grave;": return 96;
        case "&DiacriticalGrave;": return 96;
        case "&lcub;": return 123;
        case "&lbrace;": return 123;
        case "&verbar;": return 124;
        case "&vert;": return 124;
        case "&VerticalLine;": return 124;
        case "&rcub;": return 125;
        case "&rbrace;": return 125;
        case "&nbsp;": return 160;
        case "&NonBreakingSpace;": return 160;
        case "&iexcl;": return 161;
        case "&cent;": return 162;
        case "&pound;": return 163;
        case "&curren;": return 164;
        case "&yen;": return 165;
        case "&brvbar;": return 166;
        case "&sect;": return 167;
        case "&Dot;": return 168;
        case "&die;": return 168;
        case "&DoubleDot;": return 168;
        case "&uml;": return 168;
        case "&copy;": return 169;
        case "&COPY;": return 169;
        case "&ordf;": return 170;
        case "&laquo;": return 171;
        case "&not;": return 172;
        case "&shy;": return 173;
        case "&reg;": return 174;
        case "&circledR;": return 174;
        case "&REG;": return 174;
        case "&macr;": return 175;
        case "&OverBar;": return 175;
        case "&strns;": return 175;
        case "&deg;": return 176;
        case "&plusmn;": return 177;
        case "&pm;": return 177;
        case "&PlusMinus;": return 177;
        case "&sup2;": return 178;
        case "&sup3;": return 179;
        case "&acute;": return 180;
        case "&DiacriticalAcute;": return 180;
        case "&micro;": return 181;
        case "&para;": return 182;
        case "&middot;": return 183;
        case "&centerdot;": return 183;
        case "&CenterDot;": return 183;
        case "&cedil;": return 184;
        case "&Cedilla;": return 184;
        case "&sup1;": return 185;
        case "&ordm;": return 186;
        case "&raquo;": return 187;
        case "&frac14;": return 188;
        case "&frac12;": return 189;
        case "&half;": return 189;
        case "&frac34;": return 190;
        case "&iquest;": return 191;
        case "&Agrave;": return 192;
        case "&Aacute;": return 193;
        case "&Acirc;": return 194;
        case "&Atilde;": return 195;
        case "&Auml;": return 196;
        case "&Aring;": return 197;
        case "&AElig;": return 198;
        case "&Ccedil;": return 199;
        case "&Egrave;": return 200;
        case "&Eacute;": return 201;
        case "&Ecirc;": return 202;
        case "&Euml;": return 203;
        case "&Igrave;": return 204;
        case "&Iacute;": return 205;
        case "&Icirc;": return 206;
        case "&Iuml;": return 207;
        case "&ETH;": return 208;
        case "&Ntilde;": return 209;
        case "&Ograve;": return 210;
        case "&Oacute;": return 211;
        case "&Ocirc;": return 212;
        case "&Otilde;": return 213;
        case "&Ouml;": return 214;
        case "&times;": return 215;
        case "&Oslash;": return 216;
        case "&Ugrave;": return 217;
        case "&Uacute;": return 218;
        case "&Ucirc;": return 219;
        case "&Uuml;": return 220;
        case "&Yacute;": return 221;
        case "&THORN;": return 222;
        case "&szlig;": return 223;
        case "&agrave;": return 224;
        case "&aacute;": return 225;
        case "&acirc;": return 226;
        case "&atilde;": return 227;
        case "&auml;": return 228;
        case "&aring;": return 229;
        case "&aelig;": return 230;
        case "&ccedil;": return 231;
        case "&egrave;": return 232;
        case "&eacute;": return 233;
        case "&ecirc;": return 234;
        case "&euml;": return 235;
        case "&igrave;": return 236;
        case "&iacute;": return 237;
        case "&icirc;": return 238;
        case "&iuml;": return 239;
        case "&eth;": return 240;
        case "&ntilde;": return 241;
        case "&ograve;": return 242;
        case "&oacute;": return 243;
        case "&ocirc;": return 244;
        case "&otilde;": return 245;
        case "&ouml;": return 246;
        case "&divide;": return 247;
        case "&div;": return 247;
        case "&oslash;": return 248;
        case "&ugrave;": return 249;
        case "&uacute;": return 250;
        case "&ucirc;": return 251;
        case "&uuml;": return 252;
        case "&yacute;": return 253;
        case "&thorn;": return 254;
        case "&yuml;": return 255;
        case "&Amacr;": return 256;
        case "&amacr;": return 257;
        case "&Abreve;": return 258;
        case "&abreve;": return 259;
        case "&Aogon;": return 260;
        case "&aogon;": return 261;
        case "&Cacute;": return 262;
        case "&cacute;": return 263;
        case "&Ccirc;": return 264;
        case "&ccirc;": return 265;
        case "&Cdot;": return 266;
        case "&cdot;": return 267;
        case "&Ccaron;": return 268;
        case "&ccaron;": return 269;
        case "&Dcaron;": return 270;
        case "&dcaron;": return 271;
        case "&Dstrok;": return 272;
        case "&dstrok;": return 273;
        case "&Emacr;": return 274;
        case "&emacr;": return 275;
        case "&Edot;": return 278;
        case "&edot;": return 279;
        case "&Eogon;": return 280;
        case "&eogon;": return 281;
        case "&Ecaron;": return 282;
        case "&ecaron;": return 283;
        case "&Gcirc;": return 284;
        case "&gcirc;": return 285;
        case "&Gbreve;": return 286;
        case "&gbreve;": return 287;
        case "&Gdot;": return 288;
        case "&gdot;": return 289;
        case "&Gcedil;": return 290;
        case "&Hcirc;": return 292;
        case "&hcirc;": return 293;
        case "&Hstrok;": return 294;
        case "&hstrok;": return 295;
        case "&Itilde;": return 296;
        case "&itilde;": return 297;
        case "&Imacr;": return 298;
        case "&imacr;": return 299;
        case "&Iogon;": return 302;
        case "&iogon;": return 303;
        case "&Idot;": return 304;
        case "&imath;": return 305;
        case "&inodot;": return 305;
        case "&IJlig;": return 306;
        case "&ijlig;": return 307;
        case "&Jcirc;": return 308;
        case "&jcirc;": return 309;
        case "&Kcedil;": return 310;
        case "&kcedil;": return 311;
        case "&kgreen;": return 312;
        case "&Lacute;": return 313;
        case "&lacute;": return 314;
        case "&Lcedil;": return 315;
        case "&lcedil;": return 316;
        case "&Lcaron;": return 317;
        case "&lcaron;": return 318;
        case "&Lmidot;": return 319;
        case "&lmidot;": return 320;
        case "&Lstrok;": return 321;
        case "&lstrok;": return 322;
        case "&Nacute;": return 323;
        case "&nacute;": return 324;
        case "&Ncedil;": return 325;
        case "&ncedil;": return 326;
        case "&Ncaron;": return 327;
        case "&ncaron;": return 328;
        case "&napos;": return 329;
        case "&ENG;": return 330;
        case "&eng;": return 331;
        case "&Omacr;": return 332;
        case "&omacr;": return 333;
        case "&Odblac;": return 336;
        case "&odblac;": return 337;
        case "&OElig;": return 338;
        case "&oelig;": return 339;
        case "&Racute;": return 340;
        case "&racute;": return 341;
        case "&Rcedil;": return 342;
        case "&rcedil;": return 343;
        case "&Rcaron;": return 344;
        case "&rcaron;": return 345;
        case "&Sacute;": return 346;
        case "&sacute;": return 347;
        case "&Scirc;": return 348;
        case "&scirc;": return 349;
        case "&Scedil;": return 350;
        case "&scedil;": return 351;
        case "&Scaron;": return 352;
        case "&scaron;": return 353;
        case "&Tcedil;": return 354;
        case "&tcedil;": return 355;
        case "&Tcaron;": return 356;
        case "&tcaron;": return 357;
        case "&Tstrok;": return 358;
        case "&tstrok;": return 359;
        case "&Utilde;": return 360;
        case "&utilde;": return 361;
        case "&Umacr;": return 362;
        case "&umacr;": return 363;
        case "&Ubreve;": return 364;
        case "&ubreve;": return 365;
        case "&Uring;": return 366;
        case "&uring;": return 367;
        case "&Udblac;": return 368;
        case "&udblac;": return 369;
        case "&Uogon;": return 370;
        case "&uogon;": return 371;
        case "&Wcirc;": return 372;
        case "&wcirc;": return 373;
        case "&Ycirc;": return 374;
        case "&ycirc;": return 375;
        case "&Yuml;": return 376;
        case "&Zacute;": return 377;
        case "&zacute;": return 378;
        case "&Zdot;": return 379;
        case "&zdot;": return 380;
        case "&Zcaron;": return 381;
        case "&zcaron;": return 382;
        case "&fnof;": return 402;
        case "&imped;": return 437;
        case "&gacute;": return 501;
        case "&jmath;": return 567;
        case "&circ;": return 710;
        case "&caron;": return 711;
        case "&Hacek;": return 711;
        case "&breve;": return 728;
        case "&Breve;": return 728;
        case "&dot;": return 729;
        case "&DiacriticalDot;": return 729;
        case "&ring;": return 730;
        case "&ogon;": return 731;
        case "&tilde;": return 732;
        case "&DiacriticalTilde;": return 732;
        case "&dblac;": return 733;
        case "&DiacriticalDoubleAcute;": return 733;
        case "&DownBreve;": return 785;
        case "&UnderBar;": return 818;
        case "&Alpha;": return 913;
        case "&Beta;": return 914;
        case "&Gamma;": return 915;
        case "&Delta;": return 916;
        case "&Epsilon;": return 917;
        case "&Zeta;": return 918;
        case "&Eta;": return 919;
        case "&Theta;": return 920;
        case "&Iota;": return 921;
        case "&Kappa;": return 922;
        case "&Lambda;": return 923;
        case "&Mu;": return 924;
        case "&Nu;": return 925;
        case "&Xi;": return 926;
        case "&Omicron;": return 927;
        case "&Pi;": return 928;
        case "&Rho;": return 929;
        case "&Sigma;": return 931;
        case "&Tau;": return 932;
        case "&Upsilon;": return 933;
        case "&Phi;": return 934;
        case "&Chi;": return 935;
        case "&Psi;": return 936;
        case "&Omega;": return 937;
        case "&alpha;": return 945;
        case "&beta;": return 946;
        case "&gamma;": return 947;
        case "&delta;": return 948;
        case "&epsiv;": return 949;
        case "&varepsilon;": return 949;
        case "&epsilon;": return 949;
        case "&zeta;": return 950;
        case "&eta;": return 951;
        case "&theta;": return 952;
        case "&iota;": return 953;
        case "&kappa;": return 954;
        case "&lambda;": return 955;
        case "&mu;": return 956;
        case "&nu;": return 957;
        case "&xi;": return 958;
        case "&omicron;": return 959;
        case "&pi;": return 960;
        case "&rho;": return 961;
        case "&sigmav;": return 962;
        case "&varsigma;": return 962;
        case "&sigmaf;": return 962;
        case "&sigma;": return 963;
        case "&tau;": return 964;
        case "&upsi;": return 965;
        case "&upsilon;": return 965;
        case "&phi;": return 966;
        case "&phiv;": return 966;
        case "&varphi;": return 966;
        case "&chi;": return 967;
        case "&psi;": return 968;
        case "&omega;": return 969;
        case "&thetav;": return 977;
        case "&vartheta;": return 977;
        case "&thetasym;": return 977;
        case "&Upsi;": return 978;
        case "&upsih;": return 978;
        case "&straightphi;": return 981;
        case "&piv;": return 982;
        case "&varpi;": return 982;
        case "&Gammad;": return 988;
        case "&gammad;": return 989;
        case "&digamma;": return 989;
        case "&kappav;": return 1008;
        case "&varkappa;": return 1008;
        case "&rhov;": return 1009;
        case "&varrho;": return 1009;
        case "&epsi;": return 1013;
        case "&straightepsilon;": return 1013;
        case "&bepsi;": return 1014;
        case "&backepsilon;": return 1014;
        case "&IOcy;": return 1025;
        case "&DJcy;": return 1026;
        case "&GJcy;": return 1027;
        case "&Jukcy;": return 1028;
        case "&DScy;": return 1029;
        case "&Iukcy;": return 1030;
        case "&YIcy;": return 1031;
        case "&Jsercy;": return 1032;
        case "&LJcy;": return 1033;
        case "&NJcy;": return 1034;
        case "&TSHcy;": return 1035;
        case "&KJcy;": return 1036;
        case "&Ubrcy;": return 1038;
        case "&DZcy;": return 1039;
        case "&Acy;": return 1040;
        case "&Bcy;": return 1041;
        case "&Vcy;": return 1042;
        case "&Gcy;": return 1043;
        case "&Dcy;": return 1044;
        case "&IEcy;": return 1045;
        case "&ZHcy;": return 1046;
        case "&Zcy;": return 1047;
        case "&Icy;": return 1048;
        case "&Jcy;": return 1049;
        case "&Kcy;": return 1050;
        case "&Lcy;": return 1051;
        case "&Mcy;": return 1052;
        case "&Ncy;": return 1053;
        case "&Ocy;": return 1054;
        case "&Pcy;": return 1055;
        case "&Rcy;": return 1056;
        case "&Scy;": return 1057;
        case "&Tcy;": return 1058;
        case "&Ucy;": return 1059;
        case "&Fcy;": return 1060;
        case "&KHcy;": return 1061;
        case "&TScy;": return 1062;
        case "&CHcy;": return 1063;
        case "&SHcy;": return 1064;
        case "&SHCHcy;": return 1065;
        case "&HARDcy;": return 1066;
        case "&Ycy;": return 1067;
        case "&SOFTcy;": return 1068;
        case "&Ecy;": return 1069;
        case "&YUcy;": return 1070;
        case "&YAcy;": return 1071;
        case "&acy;": return 1072;
        case "&bcy;": return 1073;
        case "&vcy;": return 1074;
        case "&gcy;": return 1075;
        case "&dcy;": return 1076;
        case "&iecy;": return 1077;
        case "&zhcy;": return 1078;
        case "&zcy;": return 1079;
        case "&icy;": return 1080;
        case "&jcy;": return 1081;
        case "&kcy;": return 1082;
        case "&lcy;": return 1083;
        case "&mcy;": return 1084;
        case "&ncy;": return 1085;
        case "&ocy;": return 1086;
        case "&pcy;": return 1087;
        case "&rcy;": return 1088;
        case "&scy;": return 1089;
        case "&tcy;": return 1090;
        case "&ucy;": return 1091;
        case "&fcy;": return 1092;
        case "&khcy;": return 1093;
        case "&tscy;": return 1094;
        case "&chcy;": return 1095;
        case "&shcy;": return 1096;
        case "&shchcy;": return 1097;
        case "&hardcy;": return 1098;
        case "&ycy;": return 1099;
        case "&softcy;": return 1100;
        case "&ecy;": return 1101;
        case "&yucy;": return 1102;
        case "&yacy;": return 1103;
        case "&iocy;": return 1105;
        case "&djcy;": return 1106;
        case "&gjcy;": return 1107;
        case "&jukcy;": return 1108;
        case "&dscy;": return 1109;
        case "&iukcy;": return 1110;
        case "&yicy;": return 1111;
        case "&jsercy;": return 1112;
        case "&ljcy;": return 1113;
        case "&njcy;": return 1114;
        case "&tshcy;": return 1115;
        case "&kjcy;": return 1116;
        case "&ubrcy;": return 1118;
        case "&dzcy;": return 1119;
        case "&ensp;": return 8194;
        case "&emsp;": return 8195;
        case "&emsp13;": return 8196;
        case "&emsp14;": return 8197;
        case "&numsp;": return 8199;
        case "&puncsp;": return 8200;
        case "&thinsp;": return 8201;
        case "&ThinSpace;": return 8201;
        case "&hairsp;": return 8202;
        case "&VeryThinSpace;": return 8202;
        case "&ZeroWidthSpace;": return 8203;
        case "&NegativeVeryThinSpace;": return 8203;
        case "&NegativeThinSpace;": return 8203;
        case "&NegativeMediumSpace;": return 8203;
        case "&NegativeThickSpace;": return 8203;
        case "&zwnj;": return 8204;
        case "&zwj;": return 8205;
        case "&lrm;": return 8206;
        case "&rlm;": return 8207;
        case "&hyphen;": return 8208;
        case "&dash;": return 8208;
        case "&ndash;": return 8211;
        case "&mdash;": return 8212;
        case "&horbar;": return 8213;
        case "&Verbar;": return 8214;
        case "&Vert;": return 8214;
        case "&lsquo;": return 8216;
        case "&OpenCurlyQuote;": return 8216;
        case "&rsquo;": return 8217;
        case "&rsquor;": return 8217;
        case "&CloseCurlyQuote;": return 8217;
        case "&lsquor;": return 8218;
        case "&sbquo;": return 8218;
        case "&ldquo;": return 8220;
        case "&OpenCurlyDoubleQuote;": return 8220;
        case "&rdquo;": return 8221;
        case "&rdquor;": return 8221;
        case "&CloseCurlyDoubleQuote;": return 8221;
        case "&ldquor;": return 8222;
        case "&bdquo;": return 8222;
        case "&dagger;": return 8224;
        case "&Dagger;": return 8225;
        case "&ddagger;": return 8225;
        case "&bull;": return 8226;
        case "&bullet;": return 8226;
        case "&nldr;": return 8229;
        case "&hellip;": return 8230;
        case "&mldr;": return 8230;
        case "&permil;": return 8240;
        case "&pertenk;": return 8241;
        case "&prime;": return 8242;
        case "&Prime;": return 8243;
        case "&tprime;": return 8244;
        case "&bprime;": return 8245;
        case "&backprime;": return 8245;
        case "&lsaquo;": return 8249;
        case "&rsaquo;": return 8250;
        case "&oline;": return 8254;
        case "&caret;": return 8257;
        case "&hybull;": return 8259;
        case "&frasl;": return 8260;
        case "&bsemi;": return 8271;
        case "&qprime;": return 8279;
        case "&MediumSpace;": return 8287;
        case "&NoBreak;": return 8288;
        case "&ApplyFunction;": return 8289;
        case "&af;": return 8289;
        case "&InvisibleTimes;": return 8290;
        case "&it;": return 8290;
        case "&InvisibleComma;": return 8291;
        case "&ic;": return 8291;
        case "&euro;": return 8364;
        case "&tdot;": return 8411;
        case "&TripleDot;": return 8411;
        case "&DotDot;": return 8412;
        case "&Copf;": return 8450;
        case "&complexes;": return 8450;
        case "&incare;": return 8453;
        case "&gscr;": return 8458;
        case "&hamilt;": return 8459;
        case "&HilbertSpace;": return 8459;
        case "&Hscr;": return 8459;
        case "&Hfr;": return 8460;
        case "&Poincareplane;": return 8460;
        case "&quaternions;": return 8461;
        case "&Hopf;": return 8461;
        case "&planckh;": return 8462;
        case "&planck;": return 8463;
        case "&hbar;": return 8463;
        case "&plankv;": return 8463;
        case "&hslash;": return 8463;
        case "&Iscr;": return 8464;
        case "&imagline;": return 8464;
        case "&image;": return 8465;
        case "&Im;": return 8465;
        case "&imagpart;": return 8465;
        case "&Ifr;": return 8465;
        case "&Lscr;": return 8466;
        case "&lagran;": return 8466;
        case "&Laplacetrf;": return 8466;
        case "&ell;": return 8467;
        case "&Nopf;": return 8469;
        case "&naturals;": return 8469;
        case "&numero;": return 8470;
        case "&copysr;": return 8471;
        case "&weierp;": return 8472;
        case "&wp;": return 8472;
        case "&Popf;": return 8473;
        case "&primes;": return 8473;
        case "&rationals;": return 8474;
        case "&Qopf;": return 8474;
        case "&Rscr;": return 8475;
        case "&realine;": return 8475;
        case "&real;": return 8476;
        case "&Re;": return 8476;
        case "&realpart;": return 8476;
        case "&Rfr;": return 8476;
        case "&reals;": return 8477;
        case "&Ropf;": return 8477;
        case "&rx;": return 8478;
        case "&trade;": return 8482;
        case "&TRADE;": return 8482;
        case "&integers;": return 8484;
        case "&Zopf;": return 8484;
        case "&ohm;": return 8486;
        case "&mho;": return 8487;
        case "&Zfr;": return 8488;
        case "&zeetrf;": return 8488;
        case "&iiota;": return 8489;
        case "&angst;": return 8491;
        case "&bernou;": return 8492;
        case "&Bernoullis;": return 8492;
        case "&Bscr;": return 8492;
        case "&Cfr;": return 8493;
        case "&Cayleys;": return 8493;
        case "&escr;": return 8495;
        case "&Escr;": return 8496;
        case "&expectation;": return 8496;
        case "&Fscr;": return 8497;
        case "&Fouriertrf;": return 8497;
        case "&phmmat;": return 8499;
        case "&Mellintrf;": return 8499;
        case "&Mscr;": return 8499;
        case "&order;": return 8500;
        case "&orderof;": return 8500;
        case "&oscr;": return 8500;
        case "&alefsym;": return 8501;
        case "&aleph;": return 8501;
        case "&beth;": return 8502;
        case "&gimel;": return 8503;
        case "&daleth;": return 8504;
        case "&CapitalDifferentialD;": return 8517;
        case "&DD;": return 8517;
        case "&DifferentialD;": return 8518;
        case "&dd;": return 8518;
        case "&ExponentialE;": return 8519;
        case "&exponentiale;": return 8519;
        case "&ee;": return 8519;
        case "&ImaginaryI;": return 8520;
        case "&ii;": return 8520;
        case "&frac13;": return 8531;
        case "&frac23;": return 8532;
        case "&frac15;": return 8533;
        case "&frac25;": return 8534;
        case "&frac35;": return 8535;
        case "&frac45;": return 8536;
        case "&frac16;": return 8537;
        case "&frac56;": return 8538;
        case "&frac18;": return 8539;
        case "&frac38;": return 8540;
        case "&frac58;": return 8541;
        case "&frac78;": return 8542;
        case "&larr;": return 8592;
        case "&leftarrow;": return 8592;
        case "&LeftArrow;": return 8592;
        case "&slarr;": return 8592;
        case "&ShortLeftArrow;": return 8592;
        case "&uarr;": return 8593;
        case "&uparrow;": return 8593;
        case "&UpArrow;": return 8593;
        case "&ShortUpArrow;": return 8593;
        case "&rarr;": return 8594;
        case "&rightarrow;": return 8594;
        case "&RightArrow;": return 8594;
        case "&srarr;": return 8594;
        case "&ShortRightArrow;": return 8594;
        case "&darr;": return 8595;
        case "&downarrow;": return 8595;
        case "&DownArrow;": return 8595;
        case "&ShortDownArrow;": return 8595;
        case "&harr;": return 8596;
        case "&leftrightarrow;": return 8596;
        case "&LeftRightArrow;": return 8596;
        case "&varr;": return 8597;
        case "&updownarrow;": return 8597;
        case "&UpDownArrow;": return 8597;
        case "&nwarr;": return 8598;
        case "&UpperLeftArrow;": return 8598;
        case "&nwarrow;": return 8598;
        case "&nearr;": return 8599;
        case "&UpperRightArrow;": return 8599;
        case "&nearrow;": return 8599;
        case "&searr;": return 8600;
        case "&searrow;": return 8600;
        case "&LowerRightArrow;": return 8600;
        case "&swarr;": return 8601;
        case "&swarrow;": return 8601;
        case "&LowerLeftArrow;": return 8601;
        case "&nlarr;": return 8602;
        case "&nleftarrow;": return 8602;
        case "&nrarr;": return 8603;
        case "&nrightarrow;": return 8603;
        case "&rarrw;": return 8605;
        case "&rightsquigarrow;": return 8605;
        case "&Larr;": return 8606;
        case "&twoheadleftarrow;": return 8606;
        case "&Uarr;": return 8607;
        case "&Rarr;": return 8608;
        case "&twoheadrightarrow;": return 8608;
        case "&Darr;": return 8609;
        case "&larrtl;": return 8610;
        case "&leftarrowtail;": return 8610;
        case "&rarrtl;": return 8611;
        case "&rightarrowtail;": return 8611;
        case "&LeftTeeArrow;": return 8612;
        case "&mapstoleft;": return 8612;
        case "&UpTeeArrow;": return 8613;
        case "&mapstoup;": return 8613;
        case "&map;": return 8614;
        case "&RightTeeArrow;": return 8614;
        case "&mapsto;": return 8614;
        case "&DownTeeArrow;": return 8615;
        case "&mapstodown;": return 8615;
        case "&larrhk;": return 8617;
        case "&hookleftarrow;": return 8617;
        case "&rarrhk;": return 8618;
        case "&hookrightarrow;": return 8618;
        case "&larrlp;": return 8619;
        case "&looparrowleft;": return 8619;
        case "&rarrlp;": return 8620;
        case "&looparrowright;": return 8620;
        case "&harrw;": return 8621;
        case "&leftrightsquigarrow;": return 8621;
        case "&nharr;": return 8622;
        case "&nleftrightarrow;": return 8622;
        case "&lsh;": return 8624;
        case "&Lsh;": return 8624;
        case "&rsh;": return 8625;
        case "&Rsh;": return 8625;
        case "&ldsh;": return 8626;
        case "&rdsh;": return 8627;
        case "&crarr;": return 8629;
        case "&cularr;": return 8630;
        case "&curvearrowleft;": return 8630;
        case "&curarr;": return 8631;
        case "&curvearrowright;": return 8631;
        case "&olarr;": return 8634;
        case "&circlearrowleft;": return 8634;
        case "&orarr;": return 8635;
        case "&circlearrowright;": return 8635;
        case "&lharu;": return 8636;
        case "&LeftVector;": return 8636;
        case "&leftharpoonup;": return 8636;
        case "&lhard;": return 8637;
        case "&leftharpoondown;": return 8637;
        case "&DownLeftVector;": return 8637;
        case "&uharr;": return 8638;
        case "&upharpoonright;": return 8638;
        case "&RightUpVector;": return 8638;
        case "&uharl;": return 8639;
        case "&upharpoonleft;": return 8639;
        case "&LeftUpVector;": return 8639;
        case "&rharu;": return 8640;
        case "&RightVector;": return 8640;
        case "&rightharpoonup;": return 8640;
        case "&rhard;": return 8641;
        case "&rightharpoondown;": return 8641;
        case "&DownRightVector;": return 8641;
        case "&dharr;": return 8642;
        case "&RightDownVector;": return 8642;
        case "&downharpoonright;": return 8642;
        case "&dharl;": return 8643;
        case "&LeftDownVector;": return 8643;
        case "&downharpoonleft;": return 8643;
        case "&rlarr;": return 8644;
        case "&rightleftarrows;": return 8644;
        case "&RightArrowLeftArrow;": return 8644;
        case "&udarr;": return 8645;
        case "&UpArrowDownArrow;": return 8645;
        case "&lrarr;": return 8646;
        case "&leftrightarrows;": return 8646;
        case "&LeftArrowRightArrow;": return 8646;
        case "&llarr;": return 8647;
        case "&leftleftarrows;": return 8647;
        case "&uuarr;": return 8648;
        case "&upuparrows;": return 8648;
        case "&rrarr;": return 8649;
        case "&rightrightarrows;": return 8649;
        case "&ddarr;": return 8650;
        case "&downdownarrows;": return 8650;
        case "&lrhar;": return 8651;
        case "&ReverseEquilibrium;": return 8651;
        case "&leftrightharpoons;": return 8651;
        case "&rlhar;": return 8652;
        case "&rightleftharpoons;": return 8652;
        case "&Equilibrium;": return 8652;
        case "&nlArr;": return 8653;
        case "&nLeftarrow;": return 8653;
        case "&nhArr;": return 8654;
        case "&nLeftrightarrow;": return 8654;
        case "&nrArr;": return 8655;
        case "&nRightarrow;": return 8655;
        case "&lArr;": return 8656;
        case "&Leftarrow;": return 8656;
        case "&DoubleLeftArrow;": return 8656;
        case "&uArr;": return 8657;
        case "&Uparrow;": return 8657;
        case "&DoubleUpArrow;": return 8657;
        case "&rArr;": return 8658;
        case "&Rightarrow;": return 8658;
        case "&Implies;": return 8658;
        case "&DoubleRightArrow;": return 8658;
        case "&dArr;": return 8659;
        case "&Downarrow;": return 8659;
        case "&DoubleDownArrow;": return 8659;
        case "&hArr;": return 8660;
        case "&Leftrightarrow;": return 8660;
        case "&DoubleLeftRightArrow;": return 8660;
        case "&iff;": return 8660;
        case "&vArr;": return 8661;
        case "&Updownarrow;": return 8661;
        case "&DoubleUpDownArrow;": return 8661;
        case "&nwArr;": return 8662;
        case "&neArr;": return 8663;
        case "&seArr;": return 8664;
        case "&swArr;": return 8665;
        case "&lAarr;": return 8666;
        case "&Lleftarrow;": return 8666;
        case "&rAarr;": return 8667;
        case "&Rrightarrow;": return 8667;
        case "&zigrarr;": return 8669;
        case "&larrb;": return 8676;
        case "&LeftArrowBar;": return 8676;
        case "&rarrb;": return 8677;
        case "&RightArrowBar;": return 8677;
        case "&duarr;": return 8693;
        case "&DownArrowUpArrow;": return 8693;
        case "&loarr;": return 8701;
        case "&roarr;": return 8702;
        case "&hoarr;": return 8703;
        case "&forall;": return 8704;
        case "&ForAll;": return 8704;
        case "&comp;": return 8705;
        case "&complement;": return 8705;
        case "&part;": return 8706;
        case "&PartialD;": return 8706;
        case "&exist;": return 8707;
        case "&Exists;": return 8707;
        case "&nexist;": return 8708;
        case "&NotExists;": return 8708;
        case "&nexists;": return 8708;
        case "&empty;": return 8709;
        case "&emptyset;": return 8709;
        case "&emptyv;": return 8709;
        case "&varnothing;": return 8709;
        case "&nabla;": return 8711;
        case "&Del;": return 8711;
        case "&isin;": return 8712;
        case "&isinv;": return 8712;
        case "&Element;": return 8712;
        case "&in;": return 8712;
        case "&notin;": return 8713;
        case "&NotElement;": return 8713;
        case "&notinva;": return 8713;
        case "&niv;": return 8715;
        case "&ReverseElement;": return 8715;
        case "&ni;": return 8715;
        case "&SuchThat;": return 8715;
        case "&notni;": return 8716;
        case "&notniva;": return 8716;
        case "&NotReverseElement;": return 8716;
        case "&prod;": return 8719;
        case "&Product;": return 8719;
        case "&coprod;": return 8720;
        case "&Coproduct;": return 8720;
        case "&sum;": return 8721;
        case "&Sum;": return 8721;
        case "&minus;": return 8722;
        case "&mnplus;": return 8723;
        case "&mp;": return 8723;
        case "&MinusPlus;": return 8723;
        case "&plusdo;": return 8724;
        case "&dotplus;": return 8724;
        case "&setmn;": return 8726;
        case "&setminus;": return 8726;
        case "&Backslash;": return 8726;
        case "&ssetmn;": return 8726;
        case "&smallsetminus;": return 8726;
        case "&lowast;": return 8727;
        case "&compfn;": return 8728;
        case "&SmallCircle;": return 8728;
        case "&radic;": return 8730;
        case "&Sqrt;": return 8730;
        case "&prop;": return 8733;
        case "&propto;": return 8733;
        case "&Proportional;": return 8733;
        case "&vprop;": return 8733;
        case "&varpropto;": return 8733;
        case "&infin;": return 8734;
        case "&angrt;": return 8735;
        case "&ang;": return 8736;
        case "&angle;": return 8736;
        case "&angmsd;": return 8737;
        case "&measuredangle;": return 8737;
        case "&angsph;": return 8738;
        case "&mid;": return 8739;
        case "&VerticalBar;": return 8739;
        case "&smid;": return 8739;
        case "&shortmid;": return 8739;
        case "&nmid;": return 8740;
        case "&NotVerticalBar;": return 8740;
        case "&nsmid;": return 8740;
        case "&nshortmid;": return 8740;
        case "&par;": return 8741;
        case "&parallel;": return 8741;
        case "&DoubleVerticalBar;": return 8741;
        case "&spar;": return 8741;
        case "&shortparallel;": return 8741;
        case "&npar;": return 8742;
        case "&nparallel;": return 8742;
        case "&NotDoubleVerticalBar;": return 8742;
        case "&nspar;": return 8742;
        case "&nshortparallel;": return 8742;
        case "&and;": return 8743;
        case "&wedge;": return 8743;
        case "&or;": return 8744;
        case "&vee;": return 8744;
        case "&cap;": return 8745;
        case "&cup;": return 8746;
        case "&int;": return 8747;
        case "&Integral;": return 8747;
        case "&Int;": return 8748;
        case "&tint;": return 8749;
        case "&iiint;": return 8749;
        case "&conint;": return 8750;
        case "&oint;": return 8750;
        case "&ContourIntegral;": return 8750;
        case "&Conint;": return 8751;
        case "&DoubleContourIntegral;": return 8751;
        case "&Cconint;": return 8752;
        case "&cwint;": return 8753;
        case "&cwconint;": return 8754;
        case "&ClockwiseContourIntegral;": return 8754;
        case "&awconint;": return 8755;
        case "&CounterClockwiseContourIntegral;": return 8755;
        case "&there4;": return 8756;
        case "&therefore;": return 8756;
        case "&Therefore;": return 8756;
        case "&becaus;": return 8757;
        case "&because;": return 8757;
        case "&Because;": return 8757;
        case "&ratio;": return 8758;
        case "&Colon;": return 8759;
        case "&Proportion;": return 8759;
        case "&minusd;": return 8760;
        case "&dotminus;": return 8760;
        case "&mDDot;": return 8762;
        case "&homtht;": return 8763;
        case "&sim;": return 8764;
        case "&Tilde;": return 8764;
        case "&thksim;": return 8764;
        case "&thicksim;": return 8764;
        case "&bsim;": return 8765;
        case "&backsim;": return 8765;
        case "&ac;": return 8766;
        case "&mstpos;": return 8766;
        case "&acd;": return 8767;
        case "&wreath;": return 8768;
        case "&VerticalTilde;": return 8768;
        case "&wr;": return 8768;
        case "&nsim;": return 8769;
        case "&NotTilde;": return 8769;
        case "&esim;": return 8770;
        case "&EqualTilde;": return 8770;
        case "&eqsim;": return 8770;
        case "&sime;": return 8771;
        case "&TildeEqual;": return 8771;
        case "&simeq;": return 8771;
        case "&nsime;": return 8772;
        case "&nsimeq;": return 8772;
        case "&NotTildeEqual;": return 8772;
        case "&cong;": return 8773;
        case "&TildeFullEqual;": return 8773;
        case "&simne;": return 8774;
        case "&ncong;": return 8775;
        case "&NotTildeFullEqual;": return 8775;
        case "&asymp;": return 8776;
        case "&ap;": return 8776;
        case "&TildeTilde;": return 8776;
        case "&approx;": return 8776;
        case "&thkap;": return 8776;
        case "&thickapprox;": return 8776;
        case "&nap;": return 8777;
        case "&NotTildeTilde;": return 8777;
        case "&napprox;": return 8777;
        case "&ape;": return 8778;
        case "&approxeq;": return 8778;
        case "&apid;": return 8779;
        case "&bcong;": return 8780;
        case "&backcong;": return 8780;
        case "&asympeq;": return 8781;
        case "&CupCap;": return 8781;
        case "&bump;": return 8782;
        case "&HumpDownHump;": return 8782;
        case "&Bumpeq;": return 8782;
        case "&bumpe;": return 8783;
        case "&HumpEqual;": return 8783;
        case "&bumpeq;": return 8783;
        case "&esdot;": return 8784;
        case "&DotEqual;": return 8784;
        case "&doteq;": return 8784;
        case "&eDot;": return 8785;
        case "&doteqdot;": return 8785;
        case "&efDot;": return 8786;
        case "&fallingdotseq;": return 8786;
        case "&erDot;": return 8787;
        case "&risingdotseq;": return 8787;
        case "&colone;": return 8788;
        case "&coloneq;": return 8788;
        case "&Assign;": return 8788;
        case "&ecolon;": return 8789;
        case "&eqcolon;": return 8789;
        case "&ecir;": return 8790;
        case "&eqcirc;": return 8790;
        case "&cire;": return 8791;
        case "&circeq;": return 8791;
        case "&wedgeq;": return 8793;
        case "&veeeq;": return 8794;
        case "&trie;": return 8796;
        case "&triangleq;": return 8796;
        case "&equest;": return 8799;
        case "&questeq;": return 8799;
        case "&ne;": return 8800;
        case "&NotEqual;": return 8800;
        case "&equiv;": return 8801;
        case "&Congruent;": return 8801;
        case "&nequiv;": return 8802;
        case "&NotCongruent;": return 8802;
        case "&le;": return 8804;
        case "&leq;": return 8804;
        case "&ge;": return 8805;
        case "&GreaterEqual;": return 8805;
        case "&geq;": return 8805;
        case "&lE;": return 8806;
        case "&LessFullEqual;": return 8806;
        case "&leqq;": return 8806;
        case "&gE;": return 8807;
        case "&GreaterFullEqual;": return 8807;
        case "&geqq;": return 8807;
        case "&lnE;": return 8808;
        case "&lneqq;": return 8808;
        case "&gnE;": return 8809;
        case "&gneqq;": return 8809;
        case "&Lt;": return 8810;
        case "&NestedLessLess;": return 8810;
        case "&ll;": return 8810;
        case "&Gt;": return 8811;
        case "&NestedGreaterGreater;": return 8811;
        case "&gg;": return 8811;
        case "&twixt;": return 8812;
        case "&between;": return 8812;
        case "&NotCupCap;": return 8813;
        case "&nlt;": return 8814;
        case "&NotLess;": return 8814;
        case "&nless;": return 8814;
        case "&ngt;": return 8815;
        case "&NotGreater;": return 8815;
        case "&ngtr;": return 8815;
        case "&nle;": return 8816;
        case "&NotLessEqual;": return 8816;
        case "&nleq;": return 8816;
        case "&nge;": return 8817;
        case "&NotGreaterEqual;": return 8817;
        case "&ngeq;": return 8817;
        case "&lsim;": return 8818;
        case "&LessTilde;": return 8818;
        case "&lesssim;": return 8818;
        case "&gsim;": return 8819;
        case "&gtrsim;": return 8819;
        case "&GreaterTilde;": return 8819;
        case "&nlsim;": return 8820;
        case "&NotLessTilde;": return 8820;
        case "&ngsim;": return 8821;
        case "&NotGreaterTilde;": return 8821;
        case "&lg;": return 8822;
        case "&lessgtr;": return 8822;
        case "&LessGreater;": return 8822;
        case "&gl;": return 8823;
        case "&gtrless;": return 8823;
        case "&GreaterLess;": return 8823;
        case "&ntlg;": return 8824;
        case "&NotLessGreater;": return 8824;
        case "&ntgl;": return 8825;
        case "&NotGreaterLess;": return 8825;
        case "&pr;": return 8826;
        case "&Precedes;": return 8826;
        case "&prec;": return 8826;
        case "&sc;": return 8827;
        case "&Succeeds;": return 8827;
        case "&succ;": return 8827;
        case "&prcue;": return 8828;
        case "&PrecedesSlantEqual;": return 8828;
        case "&preccurlyeq;": return 8828;
        case "&sccue;": return 8829;
        case "&SucceedsSlantEqual;": return 8829;
        case "&succcurlyeq;": return 8829;
        case "&prsim;": return 8830;
        case "&precsim;": return 8830;
        case "&PrecedesTilde;": return 8830;
        case "&scsim;": return 8831;
        case "&succsim;": return 8831;
        case "&SucceedsTilde;": return 8831;
        case "&npr;": return 8832;
        case "&nprec;": return 8832;
        case "&NotPrecedes;": return 8832;
        case "&nsc;": return 8833;
        case "&nsucc;": return 8833;
        case "&NotSucceeds;": return 8833;
        case "&sub;": return 8834;
        case "&subset;": return 8834;
        case "&sup;": return 8835;
        case "&supset;": return 8835;
        case "&Superset;": return 8835;
        case "&nsub;": return 8836;
        case "&nsup;": return 8837;
        case "&sube;": return 8838;
        case "&SubsetEqual;": return 8838;
        case "&subseteq;": return 8838;
        case "&supe;": return 8839;
        case "&supseteq;": return 8839;
        case "&SupersetEqual;": return 8839;
        case "&nsube;": return 8840;
        case "&nsubseteq;": return 8840;
        case "&NotSubsetEqual;": return 8840;
        case "&nsupe;": return 8841;
        case "&nsupseteq;": return 8841;
        case "&NotSupersetEqual;": return 8841;
        case "&subne;": return 8842;
        case "&subsetneq;": return 8842;
        case "&supne;": return 8843;
        case "&supsetneq;": return 8843;
        case "&cupdot;": return 8845;
        case "&uplus;": return 8846;
        case "&UnionPlus;": return 8846;
        case "&sqsub;": return 8847;
        case "&SquareSubset;": return 8847;
        case "&sqsubset;": return 8847;
        case "&sqsup;": return 8848;
        case "&SquareSuperset;": return 8848;
        case "&sqsupset;": return 8848;
        case "&sqsube;": return 8849;
        case "&SquareSubsetEqual;": return 8849;
        case "&sqsubseteq;": return 8849;
        case "&sqsupe;": return 8850;
        case "&SquareSupersetEqual;": return 8850;
        case "&sqsupseteq;": return 8850;
        case "&sqcap;": return 8851;
        case "&SquareIntersection;": return 8851;
        case "&sqcup;": return 8852;
        case "&SquareUnion;": return 8852;
        case "&oplus;": return 8853;
        case "&CirclePlus;": return 8853;
        case "&ominus;": return 8854;
        case "&CircleMinus;": return 8854;
        case "&otimes;": return 8855;
        case "&CircleTimes;": return 8855;
        case "&osol;": return 8856;
        case "&odot;": return 8857;
        case "&CircleDot;": return 8857;
        case "&ocir;": return 8858;
        case "&circledcirc;": return 8858;
        case "&oast;": return 8859;
        case "&circledast;": return 8859;
        case "&odash;": return 8861;
        case "&circleddash;": return 8861;
        case "&plusb;": return 8862;
        case "&boxplus;": return 8862;
        case "&minusb;": return 8863;
        case "&boxminus;": return 8863;
        case "&timesb;": return 8864;
        case "&boxtimes;": return 8864;
        case "&sdotb;": return 8865;
        case "&dotsquare;": return 8865;
        case "&vdash;": return 8866;
        case "&RightTee;": return 8866;
        case "&dashv;": return 8867;
        case "&LeftTee;": return 8867;
        case "&top;": return 8868;
        case "&DownTee;": return 8868;
        case "&bottom;": return 8869;
        case "&bot;": return 8869;
        case "&perp;": return 8869;
        case "&UpTee;": return 8869;
        case "&models;": return 8871;
        case "&vDash;": return 8872;
        case "&DoubleRightTee;": return 8872;
        case "&Vdash;": return 8873;
        case "&Vvdash;": return 8874;
        case "&VDash;": return 8875;
        case "&nvdash;": return 8876;
        case "&nvDash;": return 8877;
        case "&nVdash;": return 8878;
        case "&nVDash;": return 8879;
        case "&prurel;": return 8880;
        case "&vltri;": return 8882;
        case "&vartriangleleft;": return 8882;
        case "&LeftTriangle;": return 8882;
        case "&vrtri;": return 8883;
        case "&vartriangleright;": return 8883;
        case "&RightTriangle;": return 8883;
        case "&ltrie;": return 8884;
        case "&trianglelefteq;": return 8884;
        case "&LeftTriangleEqual;": return 8884;
        case "&rtrie;": return 8885;
        case "&trianglerighteq;": return 8885;
        case "&RightTriangleEqual;": return 8885;
        case "&origof;": return 8886;
        case "&imof;": return 8887;
        case "&mumap;": return 8888;
        case "&multimap;": return 8888;
        case "&hercon;": return 8889;
        case "&intcal;": return 8890;
        case "&intercal;": return 8890;
        case "&veebar;": return 8891;
        case "&barvee;": return 8893;
        case "&angrtvb;": return 8894;
        case "&lrtri;": return 8895;
        case "&xwedge;": return 8896;
        case "&Wedge;": return 8896;
        case "&bigwedge;": return 8896;
        case "&xvee;": return 8897;
        case "&Vee;": return 8897;
        case "&bigvee;": return 8897;
        case "&xcap;": return 8898;
        case "&Intersection;": return 8898;
        case "&bigcap;": return 8898;
        case "&xcup;": return 8899;
        case "&Union;": return 8899;
        case "&bigcup;": return 8899;
        case "&diam;": return 8900;
        case "&diamond;": return 8900;
        case "&Diamond;": return 8900;
        case "&sdot;": return 8901;
        case "&sstarf;": return 8902;
        case "&Star;": return 8902;
        case "&divonx;": return 8903;
        case "&divideontimes;": return 8903;
        case "&bowtie;": return 8904;
        case "&ltimes;": return 8905;
        case "&rtimes;": return 8906;
        case "&lthree;": return 8907;
        case "&leftthreetimes;": return 8907;
        case "&rthree;": return 8908;
        case "&rightthreetimes;": return 8908;
        case "&bsime;": return 8909;
        case "&backsimeq;": return 8909;
        case "&cuvee;": return 8910;
        case "&curlyvee;": return 8910;
        case "&cuwed;": return 8911;
        case "&curlywedge;": return 8911;
        case "&Sub;": return 8912;
        case "&Subset;": return 8912;
        case "&Sup;": return 8913;
        case "&Supset;": return 8913;
        case "&Cap;": return 8914;
        case "&Cup;": return 8915;
        case "&fork;": return 8916;
        case "&pitchfork;": return 8916;
        case "&epar;": return 8917;
        case "&ltdot;": return 8918;
        case "&lessdot;": return 8918;
        case "&gtdot;": return 8919;
        case "&gtrdot;": return 8919;
        case "&Ll;": return 8920;
        case "&Gg;": return 8921;
        case "&ggg;": return 8921;
        case "&leg;": return 8922;
        case "&LessEqualGreater;": return 8922;
        case "&lesseqgtr;": return 8922;
        case "&gel;": return 8923;
        case "&gtreqless;": return 8923;
        case "&GreaterEqualLess;": return 8923;
        case "&cuepr;": return 8926;
        case "&curlyeqprec;": return 8926;
        case "&cuesc;": return 8927;
        case "&curlyeqsucc;": return 8927;
        case "&nprcue;": return 8928;
        case "&NotPrecedesSlantEqual;": return 8928;
        case "&nsccue;": return 8929;
        case "&NotSucceedsSlantEqual;": return 8929;
        case "&nsqsube;": return 8930;
        case "&NotSquareSubsetEqual;": return 8930;
        case "&nsqsupe;": return 8931;
        case "&NotSquareSupersetEqual;": return 8931;
        case "&lnsim;": return 8934;
        case "&gnsim;": return 8935;
        case "&prnsim;": return 8936;
        case "&precnsim;": return 8936;
        case "&scnsim;": return 8937;
        case "&succnsim;": return 8937;
        case "&nltri;": return 8938;
        case "&ntriangleleft;": return 8938;
        case "&NotLeftTriangle;": return 8938;
        case "&nrtri;": return 8939;
        case "&ntriangleright;": return 8939;
        case "&NotRightTriangle;": return 8939;
        case "&nltrie;": return 8940;
        case "&ntrianglelefteq;": return 8940;
        case "&NotLeftTriangleEqual;": return 8940;
        case "&nrtrie;": return 8941;
        case "&ntrianglerighteq;": return 8941;
        case "&NotRightTriangleEqual;": return 8941;
        case "&vellip;": return 8942;
        case "&ctdot;": return 8943;
        case "&utdot;": return 8944;
        case "&dtdot;": return 8945;
        case "&disin;": return 8946;
        case "&isinsv;": return 8947;
        case "&isins;": return 8948;
        case "&isindot;": return 8949;
        case "&notinvc;": return 8950;
        case "&notinvb;": return 8951;
        case "&isinE;": return 8953;
        case "&nisd;": return 8954;
        case "&xnis;": return 8955;
        case "&nis;": return 8956;
        case "&notnivc;": return 8957;
        case "&notnivb;": return 8958;
        case "&barwed;": return 8965;
        case "&barwedge;": return 8965;
        case "&Barwed;": return 8966;
        case "&doublebarwedge;": return 8966;
        case "&lceil;": return 8968;
        case "&LeftCeiling;": return 8968;
        case "&rceil;": return 8969;
        case "&RightCeiling;": return 8969;
        case "&lfloor;": return 8970;
        case "&LeftFloor;": return 8970;
        case "&rfloor;": return 8971;
        case "&RightFloor;": return 8971;
        case "&drcrop;": return 8972;
        case "&dlcrop;": return 8973;
        case "&urcrop;": return 8974;
        case "&ulcrop;": return 8975;
        case "&bnot;": return 8976;
        case "&profline;": return 8978;
        case "&profsurf;": return 8979;
        case "&telrec;": return 8981;
        case "&target;": return 8982;
        case "&ulcorn;": return 8988;
        case "&ulcorner;": return 8988;
        case "&urcorn;": return 8989;
        case "&urcorner;": return 8989;
        case "&dlcorn;": return 8990;
        case "&llcorner;": return 8990;
        case "&drcorn;": return 8991;
        case "&lrcorner;": return 8991;
        case "&frown;": return 8994;
        case "&sfrown;": return 8994;
        case "&smile;": return 8995;
        case "&ssmile;": return 8995;
        case "&cylcty;": return 9005;
        case "&profalar;": return 9006;
        case "&topbot;": return 9014;
        case "&ovbar;": return 9021;
        case "&solbar;": return 9023;
        case "&angzarr;": return 9084;
        case "&lmoust;": return 9136;
        case "&lmoustache;": return 9136;
        case "&rmoust;": return 9137;
        case "&rmoustache;": return 9137;
        case "&tbrk;": return 9140;
        case "&OverBracket;": return 9140;
        case "&bbrk;": return 9141;
        case "&UnderBracket;": return 9141;
        case "&bbrktbrk;": return 9142;
        case "&OverParenthesis;": return 9180;
        case "&UnderParenthesis;": return 9181;
        case "&OverBrace;": return 9182;
        case "&UnderBrace;": return 9183;
        case "&trpezium;": return 9186;
        case "&elinters;": return 9191;
        case "&blank;": return 9251;
        case "&oS;": return 9416;
        case "&circledS;": return 9416;
        case "&boxh;": return 9472;
        case "&HorizontalLine;": return 9472;
        case "&boxv;": return 9474;
        case "&boxdr;": return 9484;
        case "&boxdl;": return 9488;
        case "&boxur;": return 9492;
        case "&boxul;": return 9496;
        case "&boxvr;": return 9500;
        case "&boxvl;": return 9508;
        case "&boxhd;": return 9516;
        case "&boxhu;": return 9524;
        case "&boxvh;": return 9532;
        case "&boxH;": return 9552;
        case "&boxV;": return 9553;
        case "&boxdR;": return 9554;
        case "&boxDr;": return 9555;
        case "&boxDR;": return 9556;
        case "&boxdL;": return 9557;
        case "&boxDl;": return 9558;
        case "&boxDL;": return 9559;
        case "&boxuR;": return 9560;
        case "&boxUr;": return 9561;
        case "&boxUR;": return 9562;
        case "&boxuL;": return 9563;
        case "&boxUl;": return 9564;
        case "&boxUL;": return 9565;
        case "&boxvR;": return 9566;
        case "&boxVr;": return 9567;
        case "&boxVR;": return 9568;
        case "&boxvL;": return 9569;
        case "&boxVl;": return 9570;
        case "&boxVL;": return 9571;
        case "&boxHd;": return 9572;
        case "&boxhD;": return 9573;
        case "&boxHD;": return 9574;
        case "&boxHu;": return 9575;
        case "&boxhU;": return 9576;
        case "&boxHU;": return 9577;
        case "&boxvH;": return 9578;
        case "&boxVh;": return 9579;
        case "&boxVH;": return 9580;
        case "&uhblk;": return 9600;
        case "&lhblk;": return 9604;
        case "&block;": return 9608;
        case "&blk14;": return 9617;
        case "&blk12;": return 9618;
        case "&blk34;": return 9619;
        case "&squ;": return 9633;
        case "&square;": return 9633;
        case "&Square;": return 9633;
        case "&squf;": return 9642;
        case "&squarf;": return 9642;
        case "&blacksquare;": return 9642;
        case "&FilledVerySmallSquare;": return 9642;
        case "&EmptyVerySmallSquare;": return 9643;
        case "&rect;": return 9645;
        case "&marker;": return 9646;
        case "&fltns;": return 9649;
        case "&xutri;": return 9651;
        case "&bigtriangleup;": return 9651;
        case "&utrif;": return 9652;
        case "&blacktriangle;": return 9652;
        case "&utri;": return 9653;
        case "&triangle;": return 9653;
        case "&rtrif;": return 9656;
        case "&blacktriangleright;": return 9656;
        case "&rtri;": return 9657;
        case "&triangleright;": return 9657;
        case "&xdtri;": return 9661;
        case "&bigtriangledown;": return 9661;
        case "&dtrif;": return 9662;
        case "&blacktriangledown;": return 9662;
        case "&dtri;": return 9663;
        case "&triangledown;": return 9663;
        case "&ltrif;": return 9666;
        case "&blacktriangleleft;": return 9666;
        case "&ltri;": return 9667;
        case "&triangleleft;": return 9667;
        case "&loz;": return 9674;
        case "&lozenge;": return 9674;
        case "&cir;": return 9675;
        case "&tridot;": return 9708;
        case "&xcirc;": return 9711;
        case "&bigcirc;": return 9711;
        case "&ultri;": return 9720;
        case "&urtri;": return 9721;
        case "&lltri;": return 9722;
        case "&EmptySmallSquare;": return 9723;
        case "&FilledSmallSquare;": return 9724;
        case "&starf;": return 9733;
        case "&bigstar;": return 9733;
        case "&star;": return 9734;
        case "&phone;": return 9742;
        case "&female;": return 9792;
        case "&male;": return 9794;
        case "&spades;": return 9824;
        case "&spadesuit;": return 9824;
        case "&clubs;": return 9827;
        case "&clubsuit;": return 9827;
        case "&hearts;": return 9829;
        case "&heartsuit;": return 9829;
        case "&diams;": return 9830;
        case "&diamondsuit;": return 9830;
        case "&sung;": return 9834;
        case "&flat;": return 9837;
        case "&natur;": return 9838;
        case "&natural;": return 9838;
        case "&sharp;": return 9839;
        case "&check;": return 10003;
        case "&checkmark;": return 10003;
        case "&cross;": return 10007;
        case "&malt;": return 10016;
        case "&maltese;": return 10016;
        case "&sext;": return 10038;
        case "&VerticalSeparator;": return 10072;
        case "&lbbrk;": return 10098;
        case "&rbbrk;": return 10099;
        case "&lobrk;": return 10214;
        case "&LeftDoubleBracket;": return 10214;
        case "&robrk;": return 10215;
        case "&RightDoubleBracket;": return 10215;
        case "&lang;": return 10216;
        case "&LeftAngleBracket;": return 10216;
        case "&langle;": return 10216;
        case "&rang;": return 10217;
        case "&RightAngleBracket;": return 10217;
        case "&rangle;": return 10217;
        case "&Lang;": return 10218;
        case "&Rang;": return 10219;
        case "&loang;": return 10220;
        case "&roang;": return 10221;
        case "&xlarr;": return 10229;
        case "&longleftarrow;": return 10229;
        case "&LongLeftArrow;": return 10229;
        case "&xrarr;": return 10230;
        case "&longrightarrow;": return 10230;
        case "&LongRightArrow;": return 10230;
        case "&xharr;": return 10231;
        case "&longleftrightarrow;": return 10231;
        case "&LongLeftRightArrow;": return 10231;
        case "&xlArr;": return 10232;
        case "&Longleftarrow;": return 10232;
        case "&DoubleLongLeftArrow;": return 10232;
        case "&xrArr;": return 10233;
        case "&Longrightarrow;": return 10233;
        case "&DoubleLongRightArrow;": return 10233;
        case "&xhArr;": return 10234;
        case "&Longleftrightarrow;": return 10234;
        case "&DoubleLongLeftRightArrow;": return 10234;
        case "&xmap;": return 10236;
        case "&longmapsto;": return 10236;
        case "&dzigrarr;": return 10239;
        case "&nvlArr;": return 10498;
        case "&nvrArr;": return 10499;
        case "&nvHarr;": return 10500;
        case "&Map;": return 10501;
        case "&lbarr;": return 10508;
        case "&rbarr;": return 10509;
        case "&bkarow;": return 10509;
        case "&lBarr;": return 10510;
        case "&rBarr;": return 10511;
        case "&dbkarow;": return 10511;
        case "&RBarr;": return 10512;
        case "&drbkarow;": return 10512;
        case "&DDotrahd;": return 10513;
        case "&UpArrowBar;": return 10514;
        case "&DownArrowBar;": return 10515;
        case "&Rarrtl;": return 10518;
        case "&latail;": return 10521;
        case "&ratail;": return 10522;
        case "&lAtail;": return 10523;
        case "&rAtail;": return 10524;
        case "&larrfs;": return 10525;
        case "&rarrfs;": return 10526;
        case "&larrbfs;": return 10527;
        case "&rarrbfs;": return 10528;
        case "&nwarhk;": return 10531;
        case "&nearhk;": return 10532;
        case "&searhk;": return 10533;
        case "&hksearow;": return 10533;
        case "&swarhk;": return 10534;
        case "&hkswarow;": return 10534;
        case "&nwnear;": return 10535;
        case "&nesear;": return 10536;
        case "&toea;": return 10536;
        case "&seswar;": return 10537;
        case "&tosa;": return 10537;
        case "&swnwar;": return 10538;
        case "&rarrc;": return 10547;
        case "&cudarrr;": return 10549;
        case "&ldca;": return 10550;
        case "&rdca;": return 10551;
        case "&cudarrl;": return 10552;
        case "&larrpl;": return 10553;
        case "&curarrm;": return 10556;
        case "&cularrp;": return 10557;
        case "&rarrpl;": return 10565;
        case "&harrcir;": return 10568;
        case "&Uarrocir;": return 10569;
        case "&lurdshar;": return 10570;
        case "&ldrushar;": return 10571;
        case "&LeftRightVector;": return 10574;
        case "&RightUpDownVector;": return 10575;
        case "&DownLeftRightVector;": return 10576;
        case "&LeftUpDownVector;": return 10577;
        case "&LeftVectorBar;": return 10578;
        case "&RightVectorBar;": return 10579;
        case "&RightUpVectorBar;": return 10580;
        case "&RightDownVectorBar;": return 10581;
        case "&DownLeftVectorBar;": return 10582;
        case "&DownRightVectorBar;": return 10583;
        case "&LeftUpVectorBar;": return 10584;
        case "&LeftDownVectorBar;": return 10585;
        case "&LeftTeeVector;": return 10586;
        case "&RightTeeVector;": return 10587;
        case "&RightUpTeeVector;": return 10588;
        case "&RightDownTeeVector;": return 10589;
        case "&DownLeftTeeVector;": return 10590;
        case "&DownRightTeeVector;": return 10591;
        case "&LeftUpTeeVector;": return 10592;
        case "&LeftDownTeeVector;": return 10593;
        case "&lHar;": return 10594;
        case "&uHar;": return 10595;
        case "&rHar;": return 10596;
        case "&dHar;": return 10597;
        case "&luruhar;": return 10598;
        case "&ldrdhar;": return 10599;
        case "&ruluhar;": return 10600;
        case "&rdldhar;": return 10601;
        case "&lharul;": return 10602;
        case "&llhard;": return 10603;
        case "&rharul;": return 10604;
        case "&lrhard;": return 10605;
        case "&udhar;": return 10606;
        case "&UpEquilibrium;": return 10606;
        case "&duhar;": return 10607;
        case "&ReverseUpEquilibrium;": return 10607;
        case "&RoundImplies;": return 10608;
        case "&erarr;": return 10609;
        case "&simrarr;": return 10610;
        case "&larrsim;": return 10611;
        case "&rarrsim;": return 10612;
        case "&rarrap;": return 10613;
        case "&ltlarr;": return 10614;
        case "&gtrarr;": return 10616;
        case "&subrarr;": return 10617;
        case "&suplarr;": return 10619;
        case "&lfisht;": return 10620;
        case "&rfisht;": return 10621;
        case "&ufisht;": return 10622;
        case "&dfisht;": return 10623;
        case "&lopar;": return 10629;
        case "&ropar;": return 10630;
        case "&lbrke;": return 10635;
        case "&rbrke;": return 10636;
        case "&lbrkslu;": return 10637;
        case "&rbrksld;": return 10638;
        case "&lbrksld;": return 10639;
        case "&rbrkslu;": return 10640;
        case "&langd;": return 10641;
        case "&rangd;": return 10642;
        case "&lparlt;": return 10643;
        case "&rpargt;": return 10644;
        case "&gtlPar;": return 10645;
        case "&ltrPar;": return 10646;
        case "&vzigzag;": return 10650;
        case "&vangrt;": return 10652;
        case "&angrtvbd;": return 10653;
        case "&ange;": return 10660;
        case "&range;": return 10661;
        case "&dwangle;": return 10662;
        case "&uwangle;": return 10663;
        case "&angmsdaa;": return 10664;
        case "&angmsdab;": return 10665;
        case "&angmsdac;": return 10666;
        case "&angmsdad;": return 10667;
        case "&angmsdae;": return 10668;
        case "&angmsdaf;": return 10669;
        case "&angmsdag;": return 10670;
        case "&angmsdah;": return 10671;
        case "&bemptyv;": return 10672;
        case "&demptyv;": return 10673;
        case "&cemptyv;": return 10674;
        case "&raemptyv;": return 10675;
        case "&laemptyv;": return 10676;
        case "&ohbar;": return 10677;
        case "&omid;": return 10678;
        case "&opar;": return 10679;
        case "&operp;": return 10681;
        case "&olcross;": return 10683;
        case "&odsold;": return 10684;
        case "&olcir;": return 10686;
        case "&ofcir;": return 10687;
        case "&olt;": return 10688;
        case "&ogt;": return 10689;
        case "&cirscir;": return 10690;
        case "&cirE;": return 10691;
        case "&solb;": return 10692;
        case "&bsolb;": return 10693;
        case "&boxbox;": return 10697;
        case "&trisb;": return 10701;
        case "&rtriltri;": return 10702;
        case "&LeftTriangleBar;": return 10703;
        case "&RightTriangleBar;": return 10704;
        case "&race;": return 10714;
        case "&iinfin;": return 10716;
        case "&infintie;": return 10717;
        case "&nvinfin;": return 10718;
        case "&eparsl;": return 10723;
        case "&smeparsl;": return 10724;
        case "&eqvparsl;": return 10725;
        case "&lozf;": return 10731;
        case "&blacklozenge;": return 10731;
        case "&RuleDelayed;": return 10740;
        case "&dsol;": return 10742;
        case "&xodot;": return 10752;
        case "&bigodot;": return 10752;
        case "&xoplus;": return 10753;
        case "&bigoplus;": return 10753;
        case "&xotime;": return 10754;
        case "&bigotimes;": return 10754;
        case "&xuplus;": return 10756;
        case "&biguplus;": return 10756;
        case "&xsqcup;": return 10758;
        case "&bigsqcup;": return 10758;
        case "&qint;": return 10764;
        case "&iiiint;": return 10764;
        case "&fpartint;": return 10765;
        case "&cirfnint;": return 10768;
        case "&awint;": return 10769;
        case "&rppolint;": return 10770;
        case "&scpolint;": return 10771;
        case "&npolint;": return 10772;
        case "&pointint;": return 10773;
        case "&quatint;": return 10774;
        case "&intlarhk;": return 10775;
        case "&pluscir;": return 10786;
        case "&plusacir;": return 10787;
        case "&simplus;": return 10788;
        case "&plusdu;": return 10789;
        case "&plussim;": return 10790;
        case "&plustwo;": return 10791;
        case "&mcomma;": return 10793;
        case "&minusdu;": return 10794;
        case "&loplus;": return 10797;
        case "&roplus;": return 10798;
        case "&Cross;": return 10799;
        case "&timesd;": return 10800;
        case "&timesbar;": return 10801;
        case "&smashp;": return 10803;
        case "&lotimes;": return 10804;
        case "&rotimes;": return 10805;
        case "&otimesas;": return 10806;
        case "&Otimes;": return 10807;
        case "&odiv;": return 10808;
        case "&triplus;": return 10809;
        case "&triminus;": return 10810;
        case "&tritime;": return 10811;
        case "&iprod;": return 10812;
        case "&intprod;": return 10812;
        case "&amalg;": return 10815;
        case "&capdot;": return 10816;
        case "&ncup;": return 10818;
        case "&ncap;": return 10819;
        case "&capand;": return 10820;
        case "&cupor;": return 10821;
        case "&cupcap;": return 10822;
        case "&capcup;": return 10823;
        case "&cupbrcap;": return 10824;
        case "&capbrcup;": return 10825;
        case "&cupcup;": return 10826;
        case "&capcap;": return 10827;
        case "&ccups;": return 10828;
        case "&ccaps;": return 10829;
        case "&ccupssm;": return 10832;
        case "&And;": return 10835;
        case "&Or;": return 10836;
        case "&andand;": return 10837;
        case "&oror;": return 10838;
        case "&orslope;": return 10839;
        case "&andslope;": return 10840;
        case "&andv;": return 10842;
        case "&orv;": return 10843;
        case "&andd;": return 10844;
        case "&ord;": return 10845;
        case "&wedbar;": return 10847;
        case "&sdote;": return 10854;
        case "&simdot;": return 10858;
        case "&congdot;": return 10861;
        case "&easter;": return 10862;
        case "&apacir;": return 10863;
        case "&apE;": return 10864;
        case "&eplus;": return 10865;
        case "&pluse;": return 10866;
        case "&Esim;": return 10867;
        case "&Colone;": return 10868;
        case "&Equal;": return 10869;
        case "&eDDot;": return 10871;
        case "&ddotseq;": return 10871;
        case "&equivDD;": return 10872;
        case "&ltcir;": return 10873;
        case "&gtcir;": return 10874;
        case "&ltquest;": return 10875;
        case "&gtquest;": return 10876;
        case "&les;": return 10877;
        case "&LessSlantEqual;": return 10877;
        case "&leqslant;": return 10877;
        case "&ges;": return 10878;
        case "&GreaterSlantEqual;": return 10878;
        case "&geqslant;": return 10878;
        case "&lesdot;": return 10879;
        case "&gesdot;": return 10880;
        case "&lesdoto;": return 10881;
        case "&gesdoto;": return 10882;
        case "&lesdotor;": return 10883;
        case "&gesdotol;": return 10884;
        case "&lap;": return 10885;
        case "&lessapprox;": return 10885;
        case "&gap;": return 10886;
        case "&gtrapprox;": return 10886;
        case "&lne;": return 10887;
        case "&lneq;": return 10887;
        case "&gne;": return 10888;
        case "&gneq;": return 10888;
        case "&lnap;": return 10889;
        case "&lnapprox;": return 10889;
        case "&gnap;": return 10890;
        case "&gnapprox;": return 10890;
        case "&lEg;": return 10891;
        case "&lesseqqgtr;": return 10891;
        case "&gEl;": return 10892;
        case "&gtreqqless;": return 10892;
        case "&lsime;": return 10893;
        case "&gsime;": return 10894;
        case "&lsimg;": return 10895;
        case "&gsiml;": return 10896;
        case "&lgE;": return 10897;
        case "&glE;": return 10898;
        case "&lesges;": return 10899;
        case "&gesles;": return 10900;
        case "&els;": return 10901;
        case "&eqslantless;": return 10901;
        case "&egs;": return 10902;
        case "&eqslantgtr;": return 10902;
        case "&elsdot;": return 10903;
        case "&egsdot;": return 10904;
        case "&el;": return 10905;
        case "&eg;": return 10906;
        case "&siml;": return 10909;
        case "&simg;": return 10910;
        case "&simlE;": return 10911;
        case "&simgE;": return 10912;
        case "&LessLess;": return 10913;
        case "&GreaterGreater;": return 10914;
        case "&glj;": return 10916;
        case "&gla;": return 10917;
        case "&ltcc;": return 10918;
        case "&gtcc;": return 10919;
        case "&lescc;": return 10920;
        case "&gescc;": return 10921;
        case "&smt;": return 10922;
        case "&lat;": return 10923;
        case "&smte;": return 10924;
        case "&late;": return 10925;
        case "&bumpE;": return 10926;
        case "&pre;": return 10927;
        case "&preceq;": return 10927;
        case "&PrecedesEqual;": return 10927;
        case "&sce;": return 10928;
        case "&succeq;": return 10928;
        case "&SucceedsEqual;": return 10928;
        case "&prE;": return 10931;
        case "&scE;": return 10932;
        case "&prnE;": return 10933;
        case "&precneqq;": return 10933;
        case "&scnE;": return 10934;
        case "&succneqq;": return 10934;
        case "&prap;": return 10935;
        case "&precapprox;": return 10935;
        case "&scap;": return 10936;
        case "&succapprox;": return 10936;
        case "&prnap;": return 10937;
        case "&precnapprox;": return 10937;
        case "&scnap;": return 10938;
        case "&succnapprox;": return 10938;
        case "&Pr;": return 10939;
        case "&Sc;": return 10940;
        case "&subdot;": return 10941;
        case "&supdot;": return 10942;
        case "&subplus;": return 10943;
        case "&supplus;": return 10944;
        case "&submult;": return 10945;
        case "&supmult;": return 10946;
        case "&subedot;": return 10947;
        case "&supedot;": return 10948;
        case "&subE;": return 10949;
        case "&subseteqq;": return 10949;
        case "&supE;": return 10950;
        case "&supseteqq;": return 10950;
        case "&subsim;": return 10951;
        case "&supsim;": return 10952;
        case "&subnE;": return 10955;
        case "&subsetneqq;": return 10955;
        case "&supnE;": return 10956;
        case "&supsetneqq;": return 10956;
        case "&csub;": return 10959;
        case "&csup;": return 10960;
        case "&csube;": return 10961;
        case "&csupe;": return 10962;
        case "&subsup;": return 10963;
        case "&supsub;": return 10964;
        case "&subsub;": return 10965;
        case "&supsup;": return 10966;
        case "&suphsub;": return 10967;
        case "&supdsub;": return 10968;
        case "&forkv;": return 10969;
        case "&topfork;": return 10970;
        case "&mlcp;": return 10971;
        case "&Dashv;": return 10980;
        case "&DoubleLeftTee;": return 10980;
        case "&Vdashl;": return 10982;
        case "&Barv;": return 10983;
        case "&vBar;": return 10984;
        case "&vBarv;": return 10985;
        case "&Vbar;": return 10987;
        case "&Not;": return 10988;
        case "&bNot;": return 10989;
        case "&rnmid;": return 10990;
        case "&cirmid;": return 10991;
        case "&midcir;": return 10992;
        case "&topcir;": return 10993;
        case "&nhpar;": return 10994;
        case "&parsim;": return 10995;
        case "&parsl;": return 11005;
        case "&fflig;": return 64256;
        case "&filig;": return 64257;
        case "&fllig;": return 64258;
        case "&ffilig;": return 64259;
        case "&ffllig;": return 64260;
        case "&Ascr;": return 119964;
        case "&Cscr;": return 119966;
        case "&Dscr;": return 119967;
        case "&Gscr;": return 119970;
        case "&Jscr;": return 119973;
        case "&Kscr;": return 119974;
        case "&Nscr;": return 119977;
        case "&Oscr;": return 119978;
        case "&Pscr;": return 119979;
        case "&Qscr;": return 119980;
        case "&Sscr;": return 119982;
        case "&Tscr;": return 119983;
        case "&Uscr;": return 119984;
        case "&Vscr;": return 119985;
        case "&Wscr;": return 119986;
        case "&Xscr;": return 119987;
        case "&Yscr;": return 119988;
        case "&Zscr;": return 119989;
        case "&ascr;": return 119990;
        case "&bscr;": return 119991;
        case "&cscr;": return 119992;
        case "&dscr;": return 119993;
        case "&fscr;": return 119995;
        case "&hscr;": return 119997;
        case "&iscr;": return 119998;
        case "&jscr;": return 119999;
        case "&kscr;": return 120000;
        case "&lscr;": return 120001;
        case "&mscr;": return 120002;
        case "&nscr;": return 120003;
        case "&pscr;": return 120005;
        case "&qscr;": return 120006;
        case "&rscr;": return 120007;
        case "&sscr;": return 120008;
        case "&tscr;": return 120009;
        case "&uscr;": return 120010;
        case "&vscr;": return 120011;
        case "&wscr;": return 120012;
        case "&xscr;": return 120013;
        case "&yscr;": return 120014;
        case "&zscr;": return 120015;
        case "&Afr;": return 120068;
        case "&Bfr;": return 120069;
        case "&Dfr;": return 120071;
        case "&Efr;": return 120072;
        case "&Ffr;": return 120073;
        case "&Gfr;": return 120074;
        case "&Jfr;": return 120077;
        case "&Kfr;": return 120078;
        case "&Lfr;": return 120079;
        case "&Mfr;": return 120080;
        case "&Nfr;": return 120081;
        case "&Ofr;": return 120082;
        case "&Pfr;": return 120083;
        case "&Qfr;": return 120084;
        case "&Sfr;": return 120086;
        case "&Tfr;": return 120087;
        case "&Ufr;": return 120088;
        case "&Vfr;": return 120089;
        case "&Wfr;": return 120090;
        case "&Xfr;": return 120091;
        case "&Yfr;": return 120092;
        case "&afr;": return 120094;
        case "&bfr;": return 120095;
        case "&cfr;": return 120096;
        case "&dfr;": return 120097;
        case "&efr;": return 120098;
        case "&ffr;": return 120099;
        case "&gfr;": return 120100;
        case "&hfr;": return 120101;
        case "&ifr;": return 120102;
        case "&jfr;": return 120103;
        case "&kfr;": return 120104;
        case "&lfr;": return 120105;
        case "&mfr;": return 120106;
        case "&nfr;": return 120107;
        case "&ofr;": return 120108;
        case "&pfr;": return 120109;
        case "&qfr;": return 120110;
        case "&rfr;": return 120111;
        case "&sfr;": return 120112;
        case "&tfr;": return 120113;
        case "&ufr;": return 120114;
        case "&vfr;": return 120115;
        case "&wfr;": return 120116;
        case "&xfr;": return 120117;
        case "&yfr;": return 120118;
        case "&zfr;": return 120119;
        case "&Aopf;": return 120120;
        case "&Bopf;": return 120121;
        case "&Dopf;": return 120123;
        case "&Eopf;": return 120124;
        case "&Fopf;": return 120125;
        case "&Gopf;": return 120126;
        case "&Iopf;": return 120128;
        case "&Jopf;": return 120129;
        case "&Kopf;": return 120130;
        case "&Lopf;": return 120131;
        case "&Mopf;": return 120132;
        case "&Oopf;": return 120134;
        case "&Sopf;": return 120138;
        case "&Topf;": return 120139;
        case "&Uopf;": return 120140;
        case "&Vopf;": return 120141;
        case "&Wopf;": return 120142;
        case "&Xopf;": return 120143;
        case "&Yopf;": return 120144;
        case "&aopf;": return 120146;
        case "&bopf;": return 120147;
        case "&copf;": return 120148;
        case "&dopf;": return 120149;
        case "&eopf;": return 120150;
        case "&fopf;": return 120151;
        case "&gopf;": return 120152;
        case "&hopf;": return 120153;
        case "&iopf;": return 120154;
        case "&jopf;": return 120155;
        case "&kopf;": return 120156;
        case "&lopf;": return 120157;
        case "&mopf;": return 120158;
        case "&nopf;": return 120159;
        case "&oopf;": return 120160;
        case "&popf;": return 120161;
        case "&qopf;": return 120162;
        case "&ropf;": return 120163;
        case "&sopf;": return 120164;
        case "&topf;": return 120165;
        case "&uopf;": return 120166;
        case "&vopf;": return 120167;
        case "&wopf;": return 120168;
        case "&xopf;": return 120169;
        case "&yopf;": return 120170;
        case "&zopf;": return 120171;
      }
      return 0;
    }
  }
}
