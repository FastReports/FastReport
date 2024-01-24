using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

    /// <summary>
    /// Generates the Japan Post 4 State Code barcode.
    /// </summary>
    public class BarcodeJapanPost4StateCode : LinearBarcodeBase
    {
        private string CeckDigitSet = "0123456789-abcdefgh";
        private string EncodeTable = "1234567890-abcdefgh";
        private static string[] JapanTable =
        {
            "6161E", //1
            "61G1F", //2
            "G161F", //3
            "61F1G", //4
            "61E16", //5
            "G1F16", //6
            "F161G", //7
            "F1G16", //8
            "E1616", //9
            "61E1E", //0
            "E161E", //-
            "G1F1E", //a
            "G1E1F", //b
            "F1G1E", //c
            "E1G1F", //d
            "F1E1G", //e
            "E1F1G", //f
            "E1E16", //g
            "61616"  //h
        };

        internal override string GetPattern()
        {
            string encoded = "";
            int sum = 0;
            int weight = 0;
            string result = "61G1"; // start bar

            if (text.Length < 7)
            {
                throw new FormatException(Res.Get("Messages,BarcodeFewError"));
            }

            foreach (var i in text)
            {
                if (((i >= '0') && (i <= '9')) || (i == '-'))
                {
                    encoded += i;
                    weight++;
                }
                else
                {
                    if ((i >= 'A') && (i <= 'J'))
                    {
                        encoded += 'a';
                        encoded += (char)(i - 'A' + '0');
                    }
                    if ((i >= 'K') && (i <= 'T'))
                    {
                        encoded += 'b';
                        encoded += (char)(i - 'K' + '0');
                    }
                    if ((i >= 'U') && (i <= 'Z'))
                    {
                        encoded += 'c';
                        encoded += (char)(i - 'U' + '0');
                    }
                    weight += 2;
                }
            }

            // remove the hyphens that will not be encoded in the barcode
            if (encoded.IndexOf('-') == 3)
            {
                encoded = encoded.Remove(3, 1);
                weight--;
            }
            if (encoded.IndexOf('-', 5) == 7)
            {
                encoded = encoded.Remove(7, 1);
                weight--;
            }

            if (weight > 20 || Regex.IsMatch(text.Substring(0, 7), "[^0-9\\-]") ||
                Regex.IsMatch(text.Substring(7, text.Length - 7), "[^A-Z0-9\\-]") ||
                (encoded.IndexOf('-') < 8 && encoded.IndexOf('-') != -1))
            {
                throw new FormatException(Res.Get("Messages,BarcodeLengthMismatch"));
            }

            // fill pad character CC4, if need
            for (int i = encoded.Length; i < 20; i++)
            {
                encoded += 'd';
            }

            for (int i = 0; i < 20; i++)
            {
                result += JapanTable[EncodeTable.IndexOf(encoded[i])];
                sum += CeckDigitSet.IndexOf(encoded[i]);
                result += '1';
            }

            //Calculate check digit
            char check_char = char.MinValue;
            int check = 19 - (sum % 19);
            if (check == 19) { check = 0; }
            if (check <= 9) { check_char = (char)(check + '0'); }
            if (check == 10) { check_char = '-'; }
            if (check >= 11) { check_char = (char)((check - 11) + 'a'); }
            result += JapanTable[EncodeTable.IndexOf(check_char)];

            // data + stop bar
            return result + "1G16";
        }
    }
}

