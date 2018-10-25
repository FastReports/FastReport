using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Barcode
{
  /// <summary>
  /// Generates the PostNet barcode.
  /// </summary>
  public class BarcodePostNet : LinearBarcodeBase
  {
    private static string[] tabelle_PostNet = {
      "5151919191",    //0
      "9191915151",    //1
      "9191519151",    //2
      "9191515191",    //3
      "9151919151",    //4
      "9151915191",    //5
      "9151519191",    //6
      "5191919151",    //7
      "5191915191",    //8
      "5191519191"     //9
    };

    internal override string GetPattern()
    {
      string result = "51";

      for (int i = 0; i < text.Length; i++)
        result += tabelle_PostNet[CharToInt(text[i])];

      result += "5";

      return result;
    }
  }
}
