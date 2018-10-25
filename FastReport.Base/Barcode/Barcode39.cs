using System;
using System.Text;

namespace FastReport.Barcode
{
  /// <summary>
  /// Generates the Code39 barcode.
  /// </summary>
  public class Barcode39 : LinearBarcodeBase
  {
    private struct Code39
    {
#pragma warning disable FR0006 // Field name of struct must be longer than 2 characters.
      public string c;
#pragma warning restore FR0006 // Field name of struct must be longer than 2 characters.
      public string data;
      public short chk;

      public Code39(string c, string data, short chk)
      {
        this.c = c;
        this.data = data;
        this.chk = chk;
      }
    }

    private static Code39[] tabelle_39 = {
      new Code39("0", "505160605", 0),
      new Code39("1", "605150506", 1),
      new Code39("2", "506150506", 2),
      new Code39("3", "606150505", 3),
      new Code39("4", "505160506", 4),
      new Code39("5", "605160505", 5),
      new Code39("6", "506160505", 6),
      new Code39("7", "505150606", 7),
      new Code39("8", "605150605", 8),
      new Code39("9", "506150605", 9),
      new Code39("A", "605051506", 10),
      new Code39("B", "506051506", 11),
      new Code39("C", "606051505", 12),
      new Code39("D", "505061506", 13),
      new Code39("E", "605061505", 14),
      new Code39("F", "506061505", 15),
      new Code39("G", "505051606", 16),
      new Code39("H", "605051605", 17),
      new Code39("I", "506051605", 18),
      new Code39("J", "505061605", 19),
      new Code39("K", "605050516", 20),
      new Code39("L", "506050516", 21),
      new Code39("M", "606050515", 22),
      new Code39("N", "505060516", 23),
      new Code39("O", "605060515", 24),
      new Code39("P", "506060515", 25),
      new Code39("Q", "505050616", 26),
      new Code39("R", "605050615", 27),
      new Code39("S", "506050615", 28),
      new Code39("T", "505060615", 29),
      new Code39("U", "615050506", 30),
      new Code39("V", "516050506", 31),
      new Code39("W", "616050505", 32),
      new Code39("X", "515060506", 33),
      new Code39("Y", "615060505", 34),
      new Code39("Z", "516060505", 35),
      new Code39("-", "515050606", 36),
      new Code39(".", "615050605", 37),
      new Code39(" ", "516050605", 38),
      new Code39("*", "515060605", 0),
      new Code39("$", "515151505", 39),
      new Code39("/", "515150515", 40),
      new Code39("+", "515051515", 41),
      new Code39("%", "505151515", 42)
    };

    /// <inheritdoc/>
    public override bool IsNumeric
    {
      get { return false; }
    }

    private int FindBarItem(string c)
    {
      for (int i = 0; i < tabelle_39.Length; i++)
      {
        if (c == tabelle_39[i].c)
          return i;
      }
      
      return -1;
    }

    internal override string GetPattern()
    {
      int checksum = 0;

      //  Startcode
      string result = tabelle_39[FindBarItem("*")].data + '0';

      foreach (char c in text)
      {
        int idx = FindBarItem(c.ToString());
        if (idx < 0)
          continue;

        result += tabelle_39[idx].data + '0';
        checksum += tabelle_39[idx].chk;
      }

      // Calculate Checksum Data
      if (CalcCheckSum)
      {
        checksum = checksum % 43;
        foreach (Code39 i in tabelle_39)
        {
          if (checksum == i.chk)
          {
            result += i.data + '0';
            break;
          }
        }  
      }

      // Stopcode
      result += tabelle_39[FindBarItem("*")].data;

      return result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Barcode39"/> class with default settings.
    /// </summary>
    public Barcode39()
    {
      ratioMin = 2;
      ratioMax = 3;
    }
  }

  /// <summary>
  /// Generates the Code39 extended barcode.
  /// </summary>
  public class Barcode39Extended : Barcode39
  {
    private static string[] code39x = {
      "%U", "$A", "$B", "$C", "$D", "$E", "$F", "$G",
      "$H", "$I", "$J", "$K", "$L", "$M", "$N", "$O",
      "$P", "$Q", "$R", "$S", "$T", "$U", "$V", "$W",
      "$X", "$Y", "$Z", "%A", "%B", "%C", "%D", "%E",
      " ", "/A", "/B", "/C", "/D", "/E", "/F", "/G",
      "/H", "/I", "/J", "/K", "/L", "/M", "/N", "/O",
      "0",  "1",  "2",  "3",  "4",  "5",  "6",  "7",
      "8",  "9", "/Z", "%F", "%G", "%H", "%I", "%J",
      "%V",  "A",  "B",  "C",  "D",  "E",  "F",  "G",
      "H",  "I",  "J",  "K",  "L",  "M",  "N",  "O",
      "P",  "Q",  "R",  "S",  "T",  "U",  "V",  "W",
      "X",  "Y",  "Z", "%K", "%L", "%M", "%N", "%O",
      "%W", "+A", "+B", "+C", "+D", "+E", "+F", "+G",
      "+H", "+I", "+J", "+K", "+L", "+M", "+N", "+O",
      "+P", "+Q", "+R", "+S", "+T", "+U", "+V", "+W",
      "+X", "+Y", "+Z", "%P", "%Q", "%R", "%S", "%T"
    };

    internal override string GetPattern()
    {
      string saveText = text;
      text = "";

      for (int i = 0; i < saveText.Length; i++)
      {
        if (saveText[i] <= (char)127)
          text += code39x[saveText[i]];
      }
      
      string pattern = base.GetPattern();
      text = saveText;
      return pattern;
    }
  }
}
