using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Barcode
{
  /// <summary>
  /// Generates the "2/5 Interleaved" barcode.
  /// </summary>
  public class Barcode2of5Interleaved : LinearBarcodeBase
  {
    internal static int[,] tabelle_2_5 = {
      {0, 0, 1, 1, 0},    // 0
      {1, 0, 0, 0, 1},    // 1
      {0, 1, 0, 0, 1},    // 2
      {1, 1, 0, 0, 0},    // 3
      {0, 0, 1, 0, 1},    // 4
      {1, 0, 1, 0, 0},    // 5
      {0, 1, 1, 0, 0},    // 6
      {0, 0, 0, 1, 1},    // 7
      {1, 0, 0, 1, 0},    // 8
      {0, 1, 0, 1, 0}     // 9
    };

    internal override string GetPattern()
    {
      string text = base.text;
      string result = "5050";   //Startcode
      string c;

      if (CalcCheckSum)
      {
        if (text.Length % 2 == 0)
          text = text.Substring(1, text.Length - 1);
        text = DoCheckSumming(text);
      }
      else
      {
        if (text.Length % 2 != 0)
          text = "0" + text;
      }

      for (int i = 0; i < (text.Length / 2); i++)
      {
        for (int j = 0; j <= 4; j++)
        {
          if (tabelle_2_5[CharToInt(text[i * 2]), j] == 1)
            c = "6";
          else
            c = "5";
          result += c;

          if (tabelle_2_5[CharToInt(text[i * 2 + 1]), j] == 1)
            c = "1";
          else
            c = "0";
          result += c;
        }
      }

      result += "605";    // Stopcode
      return result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Barcode2of5Interleaved"/> class with default settings.
    /// </summary>
    public Barcode2of5Interleaved()
    {
      ratioMin = 2;
      ratioMax = 3;
    }
  }

  /// <summary>
  /// Generates the "2/5 Industrial" barcode.
  /// </summary>
  public class Barcode2of5Industrial : Barcode2of5Interleaved
  {
    internal override string GetPattern()
    {
      string text = base.text;
      string result = "606050";   // Startcode

      if (CalcCheckSum)
      {
        text = DoCheckSumming(text);
      }

      for (int i = 0; i < text.Length; i++)
      {
        for (int j = 0; j <= 4; j++)
        {
          if (tabelle_2_5[CharToInt(text[i]), j] == 1)
            result += "60";
          else
            result += "50";
        }
      }

      result += "605060";   //Stopcode 
      return result;
    }
  }

  /// <summary>
  /// Generates the "2/5 Matrix" barcode.
  /// </summary>
  public class Barcode2of5Matrix : Barcode2of5Interleaved
  {
    internal override string GetPattern()
    {
      string text = base.text;
      string result = "705050";   // Startcode
      char c;

      if (CalcCheckSum)
      {
        text = DoCheckSumming(text);
      }

      for (int i = 0; i < text.Length; i++)
      {
        for (int j = 0; j <= 4; j++)
        {
          if (tabelle_2_5[CharToInt(text[i]), j] == 1)
            c = '1';
          else
            c = '0';

          if ((j % 2) == 0)
            c = (char)((int)c + 5);
          result += c;
        }
        result += '0';
      }

      result = result + "70505";   // Stopcode

      return result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Barcode2of5Matrix"/> class with default settings.
    /// </summary>
    public Barcode2of5Matrix()
    {
      ratioMin = 2.25f;
      ratioMax = 3;
      WideBarRatio = 2.25f;
    }
  }
}
