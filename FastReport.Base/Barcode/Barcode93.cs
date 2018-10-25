using System;
using System.Text;
using FastReport.Utils;

namespace FastReport.Barcode
{
  /// <summary>
  /// Generates the Code93 barcode.
  /// </summary>
  public class Barcode93 : LinearBarcodeBase
  {
    private struct Code93
    {
      public string data;
#pragma warning disable FR0006 // Field name of struct must be longer than 2 characters.
      public string c;
#pragma warning restore FR0006 // Field name of struct must be longer than 2 characters.
      public Code93(string _c, string _data)
      {
        data = _data;
        c = _c;
      }
    }

    private static Code93[] tabelle_93 = {
      new Code93("0", "131112"),
      new Code93("1", "111213"),
      new Code93("2", "111312"),
      new Code93("3", "111411"),
      new Code93("4", "121113"),
      new Code93("5", "121212"),
      new Code93("6", "121311"),
      new Code93("7", "111114"),
      new Code93("8", "131211"),
      new Code93("9", "141111"),
      new Code93("A", "211113"),
      new Code93("B", "211212"),
      new Code93("C", "211311"),
      new Code93("D", "221112"),
      new Code93("E", "221211"),
      new Code93("F", "231111"),
      new Code93("G", "112113"),
      new Code93("H", "112212"),
      new Code93("I", "112311"),
      new Code93("J", "122112"),
      new Code93("K", "132111"),
      new Code93("L", "111123"),
      new Code93("M", "111222"),
      new Code93("N", "111321"),
      new Code93("O", "121122"),
      new Code93("P", "131121"),
      new Code93("Q", "212112"),
      new Code93("R", "212211"),
      new Code93("S", "211122"),
      new Code93("T", "211221"),
      new Code93("U", "221121"),
      new Code93("V", "222111"),
      new Code93("W", "112122"),
      new Code93("X", "112221"),
      new Code93("Y", "122121"),
      new Code93("Z", "123111"),
      new Code93("-", "121131"),
      new Code93(".", "311112"),
      new Code93(" ", "311211"),
      new Code93("$", "321111"),
      new Code93("/", "112131"),
      new Code93("+", "113121"),
      new Code93("%", "211131"),
      new Code93("[", "121221"),     // only used for Extended Code 93
      new Code93("]", "312111"),     // only used for Extended Code 93}
      new Code93("{", "311121"),     // only used for Extended Code 93}
      new Code93("}", "122211")      // only used for Extended Code 93}
    };

    /// <inheritdoc/>
    public override bool IsNumeric
    {
      get { return false; }
    }

    private int FindBarItem(string c)
    {
      for (int i = 0; i < tabelle_93.Length; i++)
      {
        if (c == tabelle_93[i].c)
          return i;
      }    

      return -1;
    }

    internal override string GetPattern()
    {
      string result = "111141";   // Startcode

      foreach (char c in text)
      {
        int idx = FindBarItem(c.ToString());
        if (idx < 0)
          throw new Exception(Res.Get("Messages,InvalidBarcode2"));
        result += tabelle_93[idx].data;
      }

      // Checksums
      if (CalcCheckSum)
      {
        int checkC = 0;
        int checkK = 0;

        int weightC = 1;
        int weightK = 2;

        for (int i = text.Length - 1; i >= 0; i--)
        {
          int idx = FindBarItem(text[i].ToString());

          checkC += idx * weightC;
          checkK += idx * weightK;

          weightC++;

          if (weightC > 20)
            weightC = 1;

          weightK++;

          if (weightK > 15)
            weightC = 1;
        };

        checkK += checkC;

        checkC = checkC % 47;
        checkK = checkK % 47;

        result += tabelle_93[checkC].data + tabelle_93[checkK].data;
      }
      
      // Stopcode
      result += "1111411";

      return DoConvert(result);
    }
  }

  /// <summary>
  /// Generates the Code93 extended barcode.
  /// </summary>
  public class Barcode93Extended : Barcode93
  {
    private static string[] code93x = {
      "]U", "[A", "[B", "[C", "[D", "[E", "[F", "[G",
      "[H", "[I", "[J", "[K", "[L", "[M", "[N", "[O",
      "[P", "[Q", "[R", "[S", "[T", "[U", "[V", "[W",
      "[X", "[Y", "[Z", "]A", "]B", "]C", "]D", "]E",
      " ", "{A", "{B", "{C", "{D", "{E", "{F", "{G",
      "{H", "{I", "{J", "{K", "{L", "{M", "{N", "{O",
      "0",  "1",  "2",  "3",  "4",  "5",  "6",  "7",
      "8",  "9", "{Z", "]F", "]G", "]H", "]I", "]J",
      "]V",  "A",  "B",  "C",  "D",  "E",  "F",  "G",
      "H",  "I",  "J",  "K",  "L",  "M",  "N",  "O",
      "P",  "Q",  "R",  "S",  "T",  "U",  "V",  "W",
      "X",  "Y",  "Z", "]K", "]L", "]M", "]N", "]O",
      "]W", "}A", "}B", "}C", "}D", "}E", "}F", "}G",
      "}H", "}I", "}J", "}K", "}L", "}M", "}N", "}O",
      "}P", "}Q", "}R", "}S", "}T", "}U", "}V", "}W",
      "}X", "}Y", "}Z", "]P", "]Q", "]R", "]S", "]T"
    };

    internal override string GetPattern()
    {
      string saveText = text;
      text = "";

      for (int i = 0; i < saveText.Length; i++)
      {
        if (saveText[i] <= (char)127)
          text += code93x[saveText[i]];
      }

      string pattern = base.GetPattern();
      text = saveText;
      return pattern;
    }
  }
}
