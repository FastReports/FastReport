using System;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Barcode
{
  /// <summary>
  /// Generates the Code128 barcode.
  /// </summary>
  /// <remarks>
  /// This barcode supports three code pages: A, B and C. You need to set appropriate code page
  /// in the barcode text, or use the auto encode feature. See the <see cref="AutoEncode"/> property
  /// for more details.
  /// </remarks>
  /// <example>This example shows how to configure the BarcodeObject to display Code128 barcode.
  /// <code>
  /// BarcodeObject barcode;
  /// ...
  /// barcode.Barcode = new Barcode128();
  /// (barcode.Barcode as Barcode128).AutoEncode = false;
  /// </code>
  /// </example>
  public class Barcode128 : LinearBarcodeBase
  {
    #region Fields
    private struct Code128
    {
#pragma warning disable FR0006 // Field name of struct must be longer than 2 characters.
      public string a;
      public string b;
      public string c;
#pragma warning restore FR0006 // Field name of struct must be longer than 2 characters.
      public string data;
      
      public Code128(string a, string b, string c, string data)
      {
        this.a = a;
        this.b = b;
        this.c = c;
        this.data = data;
      }
    }

    private static Code128[] tabelle_128 = {
      new Code128(" ", " ", "00", "212222"),
      new Code128("!", "!", "01", "222122"),
      new Code128("\"", "\"", "02", "222221"),
      new Code128("#", "#", "03", "121223"),
      new Code128("$", "$", "04", "121322"),
      new Code128("%", "%", "05", "131222"),
      new Code128("&", "&", "06", "122213"),
      new Code128("'", "'", "07", "122312"),
      new Code128("(", "(", "08", "132212"),
      new Code128(")", ")", "09", "221213"),
      new Code128("*", "*", "10", "221312"),
      new Code128("+", "+", "11", "231212"),
      new Code128(",", ",", "12", "112232"),
      new Code128("-", "-", "13", "122132"),
      new Code128(".", ".", "14", "122231"),
      new Code128("/", "/", "15", "113222"),
      new Code128("0", "0", "16", "123122"),
      new Code128("1", "1", "17", "123221"),
      new Code128("2", "2", "18", "223211"),
      new Code128("3", "3", "19", "221132"),
      new Code128("4", "4", "20", "221231"),
      new Code128("5", "5", "21", "213212"),
      new Code128("6", "6", "22", "223112"),
      new Code128("7", "7", "23", "312131"),
      new Code128("8", "8", "24", "311222"),
      new Code128("9", "9", "25", "321122"),
      new Code128(":", ":", "26", "321221"),
      new Code128(";", ";", "27", "312212"),
      new Code128("<", "<", "28", "322112"),
      new Code128("=", "=", "29", "322211"),
      new Code128(">", ">", "30", "212123"),
      new Code128("?", "?", "31", "212321"),
      new Code128("@", "@", "32", "232121"),
      new Code128("A", "A", "33", "111323"),
      new Code128("B", "B", "34", "131123"),
      new Code128("C", "C", "35", "131321"),
      new Code128("D", "D", "36", "112313"),
      new Code128("E", "E", "37", "132113"),
      new Code128("F", "F", "38", "132311"),
      new Code128("G", "G", "39", "211313"),
      new Code128("H", "H", "40", "231113"),
      new Code128("I", "I", "41", "231311"),
      new Code128("J", "J", "42", "112133"),
      new Code128("K", "K", "43", "112331"),
      new Code128("L", "L", "44", "132131"),
      new Code128("M", "M", "45", "113123"),
      new Code128("N", "N", "46", "113321"),
      new Code128("O", "O", "47", "133121"),
      new Code128("P", "P", "48", "313121"),
      new Code128("Q", "Q", "49", "211331"),
      new Code128("R", "R", "50", "231131"),
      new Code128("S", "S", "51", "213113"),
      new Code128("T", "T", "52", "213311"),
      new Code128("U", "U", "53", "213131"),
      new Code128("V", "V", "54", "311123"),
      new Code128("W", "W", "55", "311321"),
      new Code128("X", "X", "56", "331121"),
      new Code128("Y", "Y", "57", "312113"),
      new Code128("Z", "Z", "58", "312311"),
      new Code128("[", "[", "59", "332111"),
      new Code128("\\", "\\", "60", "314111"),
      new Code128("]", "]", "61", "221411"),
      new Code128("^", "^", "62", "431111"),
      new Code128("_", "_", "63", "111224"),
      new Code128("\x00", "`", "64", "111422"),
      new Code128("\x01", "a", "65", "121124"),
      new Code128("\x02", "b", "66", "121421"),
      new Code128("\x03", "c", "67", "141122"),
      new Code128("\x04", "d", "68", "141221"),
      new Code128("\x05", "e", "69", "112214"),
      new Code128("\x06", "f", "70", "112412"),
      new Code128("\x07", "g", "71", "122114"),
      new Code128("\x08", "h", "72", "122411"),
      new Code128("\x09", "i", "73", "142112"),
      new Code128("\x0A", "j", "74", "142211"),
      new Code128("\x0B", "k", "75", "241211"),
      new Code128("\x0C", "l", "76", "221114"),
      new Code128("\x0D", "m", "77", "413111"),
      new Code128("\x0E", "n", "78", "241112"),
      new Code128("\x0F", "o", "79", "134111"),
      new Code128("\x10", "p", "80", "111242"),
      new Code128("\x11", "q", "81", "121142"),
      new Code128("\x12", "r", "82", "121241"),
      new Code128("\x13", "s", "83", "114212"),
      new Code128("\x14", "t", "84", "124112"),
      new Code128("\x15", "u", "85", "124211"),
      new Code128("\x16", "v", "86", "411212"),
      new Code128("\x17", "w", "87", "421112"),
      new Code128("\x18", "x", "88", "421211"),
      new Code128("\x19", "y", "89", "212141"),
      new Code128("\x1A", "z", "90", "214121"),
      new Code128("\x1B", "{", "91", "412121"),
      new Code128("\x1C", "|", "92", "111143"),
      new Code128("\x1D", "}", "93", "111341"),
      new Code128("\x1E", "~", "94", "131141"),
      new Code128("\x1F", "\x7F", "95", "114113"),
      new Code128(" ", " ", "96", "114311"),     // FNC3
      new Code128(" ", " ", "97", "411113"),     // FNC2
      new Code128(" ", " ", "98", "411311"),     // SHIFT
      new Code128(" ", " ", "99", "113141"),     // CODE C
      new Code128(" ", " ", "  ", "114131"),     // FNC4, CODE B
      new Code128(" ", " ", "  ", "311141"),     // FNC4, CODE A
      new Code128(" ", " ", "  ", "411131"),     // FNC1 
      new Code128(" ", " ", "  ", "211412"),     // START A
      new Code128(" ", " ", "  ", "211214"),     // START B
      new Code128(" ", " ", "  ", "211232")      // START C
    };

    private enum Encoding { A, B, C, AorB, None }
    private bool autoEncode;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value that determines whether the barcode should automatically 
    /// use appropriate encoding.
    /// </summary>
    /// <remarks>
    /// You may use this property to encode data automatically. If you set it to <b>false</b>, 
    /// you must specify the code page inside the data string. The following control codes are available:
    /// <list type="table">
    ///   <listheader>
    ///     <term>Sequence</term>
    ///     <description>Code128 control code</description>
    ///   </listheader>
    ///   <item>
    ///     <term>&amp;A;</term>
    ///     <description>START A / CODE A</description>
    ///   </item>
    ///   <item>
    ///     <term>&amp;B;</term>
    ///     <description>START B / CODE B</description>
    ///   </item>
    ///   <item>
    ///     <term>&amp;C;</term>
    ///     <description>START C / CODE C</description>
    ///   </item>
    /// </list>
    ///   <item>
    ///     <term>&amp;S;</term>
    ///     <description>SHIFT</description>
    ///   </item>
    ///   <item>
    ///     <term>&amp;1;</term>
    ///     <description>FNC1</description>
    ///   </item>
    ///   <item>
    ///     <term>&amp;2;</term>
    ///     <description>FNC2</description>
    ///   </item>
    ///   <item>
    ///     <term>&amp;3;</term>
    ///     <description>FNC3</description>
    ///   </item>
    ///   <item>
    ///     <term>&amp;4;</term>
    ///     <description>FNC4</description>
    ///   </item>
    /// </remarks>
    /// <example>The following example shows how to specify control codes:
    /// <code>
    /// BarcodeObject barcode;
    /// barcode.Barcode = new Barcode128();
    /// (barcode.Barcode as Barcode128).AutoEncode = false;
    /// barcode.Text = "&amp;C;1234&amp;A;ABC";
    /// </code>
    /// </example>
    [DefaultValue(true)]
    public bool AutoEncode
    {
      get { return autoEncode; }
      set { autoEncode = value; }
    }

    /// <inheritdoc/>
    public override bool IsNumeric
    {
      get { return false; }
    }
    #endregion

    #region Private Methods
    private bool IsDigit(char c)
    {
      return c >= '0' && c <= '9';
    }
    
    private bool IsFourOrMoreDigits(string code, int index, out int numDigits)
    {
      numDigits = 0;
      if (IsDigit(code[index]) && index + 4 < code.Length)
      {
        while (index + numDigits < code.Length && IsDigit(code[index + numDigits]))
        {
          numDigits++;
        }
      }
      
      return numDigits >= 4;
    }

    private int FindCodeA(char c)
    {
      for (int i = 0; i < tabelle_128.Length; i++)
      {
        if (c == tabelle_128[i].a[0])
          return i;
      }

      return -1;
    }

    private int FindCodeB(char c)
    {
      for (int i = 0; i < tabelle_128.Length; i++)
      {
        if (c == tabelle_128[i].b[0])
          return i;
      }

      return -1;
    }

    private int FindCodeC(string c)
    {
      for (int i = 0; i < tabelle_128.Length; i++)
      {
        if (c == tabelle_128[i].c)
          return i;
      }

      return -1;
    }

    // Returns a group of characters with the same encoding. Updates encoding and index parameters.
    private string GetNextPortion(string code, ref int index, ref Encoding encoding)
    {
      if (index >= code.Length)
        return "";

      string result = "";
        
      // determine the first character encoding
      int aIndex = FindCodeA(code[index]);
      int bIndex = FindCodeB(code[index]);
      Encoding firstCharEncoding = Encoding.A;
      if (aIndex == -1 && bIndex != -1)
        firstCharEncoding = Encoding.B;
      else if (aIndex != -1 && bIndex != -1)
        firstCharEncoding = Encoding.AorB;
      // if we have four or more digits in the current position, use C encoding.
      int numDigits = 0;
      if (IsFourOrMoreDigits(code, index, out numDigits))
        firstCharEncoding = Encoding.C;

      // if encoding = C, we have found the group, just return it.
      if (firstCharEncoding == Encoding.C)
      {
        // we need digit pairs, so round it to even value
        numDigits = (numDigits / 2) * 2;
        result = code.Substring(index, numDigits);
        index += numDigits;
        encoding = Encoding.C;
        return "&C;" + result;
      }

      // search for next characters with the same encoding. Calculate numChars with the same encoding.
      int numChars = 1;
      while (index + numChars < code.Length)
      {
        // same as above...
        aIndex = FindCodeA(code[index + numChars]);
        bIndex = FindCodeB(code[index + numChars]);
        Encoding nextCharEncoding = Encoding.A;
        if (aIndex == -1 && bIndex != -1)
          nextCharEncoding = Encoding.B;
        else if (aIndex != -1 && bIndex != -1)
          nextCharEncoding = Encoding.AorB;
        if (IsFourOrMoreDigits(code, index + numChars, out numDigits))
          nextCharEncoding = Encoding.C;

        // switch to particular encoding from AorB
        if (nextCharEncoding != Encoding.C && nextCharEncoding != firstCharEncoding)
        {
          if (firstCharEncoding == Encoding.AorB)
            firstCharEncoding = nextCharEncoding;
          else if (nextCharEncoding == Encoding.AorB)
            nextCharEncoding = firstCharEncoding;  
        }

        if (firstCharEncoding != nextCharEncoding)
          break;
        numChars++;  
      }

      // give precedence to B encoding
      if (firstCharEncoding == Encoding.AorB)
        firstCharEncoding = Encoding.B;
      
      string prefix = firstCharEncoding == Encoding.A ? "&A;" : "&B;";
      // if we have only one character, use SHIFT code to switch encoding. Do not change current encoding.
      if (encoding != firstCharEncoding && 
        numChars == 1 &&
        (encoding == Encoding.A || encoding == Encoding.B) && 
        (firstCharEncoding == Encoding.A || firstCharEncoding == Encoding.B))
        prefix = "&S;";
      else
        encoding = firstCharEncoding;  

      result = prefix + code.Substring(index, numChars);
      index += numChars;
      return result;
    }
    
    private string StripControlCodes(string code, bool stripFNCodes)
    {
      string result = "";
      int index = 0;

      while (index < code.Length)
      {
        string nextChar = GetNextChar(code, ref index, Encoding.None);
        if (nextChar != "&A;" && nextChar != "&B;" && nextChar != "&C;" && nextChar != "&S;")
        {
          if (!stripFNCodes || (nextChar != "&1;" && nextChar != "&2;" && nextChar != "&3;" && nextChar != "&4;"))
            result += nextChar;
        }  
      }

      return result;
    }

    private string Encode(string code)
    {
      code = StripControlCodes(code, false);
      string result = "";
      int index = 0;
      Encoding encoding = Encoding.None;
      
      while (index < code.Length)
      {
        result += GetNextPortion(code, ref index, ref encoding);
      }
      
      return result;
    }

    private string GetNextChar(string code, ref int index, Encoding encoding)
    {
      if (index >= code.Length)
        return "";

      string result = "";

      // check special codes:
      // "&A;" means START A / CODE A
      // "&B;" means START B / CODE B
      // "&C;" means START C / CODE C
      // "&S;" means SHIFT
      // "&1;" means FNC1
      // "&2;" means FNC2
      // "&3;" means FNC3
      // "&4;" means FNC4
      
      if (code[index] == '&' && index + 2 < code.Length && code[index + 2] == ';')
      {
        char c = code[index + 1].ToString().ToUpper()[0];
        if (c == 'A' || c == 'B' || c == 'C' || c == 'S' || c == '1' || c == '2' || c == '3' || c == '4')
        {
          index += 3;
          return "&" + c + ";";
        }
      }
      
      // if encoding is C, get next two chars
      if (encoding == Encoding.C && index + 1 < code.Length)
      {
        result = code.Substring(index, 2);
        index += 2;
        return result;
      }
      
      result = code.Substring(index, 1);
      index++;
      return result;
    }
    #endregion

    #region Protected Methods
    internal override string StripControlCodes(string data)
    {
      return StripControlCodes(data, true);
    }

    internal override string GetPattern()
    {
      string code = text;
      if (AutoEncode)
        code = Encode(code);
      
      // get first char to determine encoding
      Encoding encoding = Encoding.None;
      int index = 0;
      string nextChar = GetNextChar(code, ref index, encoding);
      int checksum = 0;
      string startCode = "";
      
      // setup encoding
      switch (nextChar)
      {
        case "&A;":
          encoding = Encoding.A;
          checksum = 103;
          startCode = tabelle_128[103].data;
          break;

        case "&B;":
          encoding = Encoding.B;
          checksum = 104;
          startCode = tabelle_128[104].data;
          break;

        case "&C;":
          encoding = Encoding.C;
          checksum = 105;
          startCode = tabelle_128[105].data;
          break;
          
        default:
          throw new Exception(Res.Get("Messages,InvalidBarcode1"));
      }
      
      string result = startCode;    // Startcode
      int codeword_pos = 1;

      while (index < code.Length)
      {
        nextChar = GetNextChar(code, ref index, encoding);
        int idx = 0;

        switch (nextChar)
        {
          case "&A;":
            encoding = Encoding.A;
            idx = 101;
            break;

          case "&B;":
            encoding = Encoding.B;
            idx = 100;
            break;

          case "&C;":
            encoding = Encoding.C;
            idx = 99;
            break;

          case "&S;":
            if (encoding == Encoding.A)
              encoding = Encoding.B;
            else
              encoding = Encoding.A;
            idx = 98;
            break;
            
          case "&1;":
            idx = 102;
            break;
            
          case "&2;":
            idx = 97;
            break;
            
          case "&3;":
            idx = 96;
            break;
            
          case "&4;":
            idx = encoding == Encoding.A ? 101 : 100;
            break;
            
          default:
            if (encoding == Encoding.A)
              idx = FindCodeA(nextChar[0]);
            else if (encoding == Encoding.B)
              idx = FindCodeB(nextChar[0]);
            else
              idx = FindCodeC(nextChar);
            break;  
        }

        if (idx < 0)
          throw new Exception(Res.Get("Messages,InvalidBarcode2"));

        result += tabelle_128[idx].data;
        checksum += idx * codeword_pos;
        codeword_pos++;

        // switch encoding back after the SHIFT
        if (nextChar == "&S;")
        {
          if (encoding == Encoding.A)
            encoding = Encoding.B;
          else
            encoding = Encoding.A;
        }    
      }
      
      checksum = checksum % 103;
      result += tabelle_128[checksum].data;

      // stop code
      result += "2331112";
      return DoConvert(result);
    }
    #endregion
    
    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(BarcodeBase source)
    {
      base.Assign(source);
      AutoEncode = (source as Barcode128).AutoEncode;
    }

    internal override void Serialize(FRWriter writer, string prefix, BarcodeBase diff)
    {
      base.Serialize(writer, prefix, diff);
      Barcode128 c = diff as Barcode128;

      if (c == null || AutoEncode != c.AutoEncode)
        writer.WriteBool(prefix + "AutoEncode", AutoEncode);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Barcode128"/> class with default settings.
    /// </summary>
    public Barcode128()
    {
      AutoEncode = true;
    }
  }
}
