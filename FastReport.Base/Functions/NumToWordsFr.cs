using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Functions
{
  internal class NumToWordsFr : NumToWordsBase
  {
    private static Dictionary<string, CurrencyInfo> currencyList;

    private static string[] fixedWords =
    {
      "", "un", "deux", "trois", "quatre", "cinq", "six",
      "sept", "huit", "neuf", "dix", "onze",
      "douze", "treize", "quatorze", "quinze",
      "seize", "dix-sept", "dix-huit", "dix-neuf"
    };

    private static string[] fixedWords71to79 =
    {
      "soixante et onze", "soixante-douze", "soixante-treize", "soixante-quatorze", 
      "soixante-quinze", "soixante-seize", "soixante-dix-sept", "soixante-dix-huit", "soixante-dix-neuf"
    };

    private static string[] fixedWords91to99 =
    {
      "quatre-vingt-onze", "quatre-vingt-douze", "quatre-vingt-treize", "quatre-vingt-quatorze", 
      "quatre-vingt-quinze", "quatre-vingt-seize", "quatre-vingt-dix-sept", "quatre-vingt-dix-huit", "quatre-vingt-dix-neuf"
    };

    private static string[] tens =
    {
      "", "dix", "vingt", "trente", "quarante", "cinquante",
      "soixante", "soixante-dix", "quatre-vingt", "quatre-vingt-dix"
    };

    private static string[] hunds =
    {
      "", "cent", "deux cent", "trois cent", "quatre cent",
      "cinq cent", "six cent", "sept cent", "huit cent", "neuf cent"
    };

    private static WordInfo thousands = new WordInfo(false, "mille", "mille", "mille");
    private static WordInfo millions = new WordInfo(false, "million", "millions", "millions");
    private static WordInfo milliards = new WordInfo(false, "milliard", "milliards", "milliards");
    private static WordInfo trillions = new WordInfo(false, "billion", "billions", "billions");

    protected override string Str1000(long value, WordInfo info, int counter)
    {
        long val = value % 1000;
        if (val == 0)
            return "";

        StringBuilder r = new StringBuilder();
        // add hundred
        string hund = GetHund(info.male, val);
        if (hund != "")
            r.Append(hund);

        // decide whether to use the 100-10 separator or not
        string sep100_10 = Get100_10Separator();
        if (value < 1000 && hund == "")
            sep100_10 = "";

        val = val % 100;
        if (val < 20) 
        {
            r.Append(sep100_10 + GetFixedWords(info.male, val));
        }
        else if (val > 70 && val < 80)
        {
            r.Append(sep100_10 + fixedWords71to79[val - 71]);
        }
        else if (val > 90 && val < 100)
        {
            r.Append(sep100_10 + fixedWords91to99[val - 91]);
        }
        else
        {
            // val is greater than fixed words count (usually 20), get tens separately
            string ten = GetTen(info.male, val / 10);
            string frac10 = GetFixedWords(info.male, val % 10);

            // decide whether to use 10-1 separator or not
            if (ten != "" && frac10 != "")
            {
                string sep10_1 = "-";
                if (val % 10 == 1)
                    if (val / 10 != 8)
                        sep10_1 = " et ";
                r.Append(sep100_10 + ten + sep10_1 + frac10);
            }
            else if (ten != "")
                r.Append(sep100_10 + ten);
            else if (frac10 != "")
                r.Append(sep100_10 + frac10);
        }

        // special cases
        if (counter == 1) // the final
        {
            // 80 ends with 's' in case it's the final word
            if (val == 80)
                r.Append("s");
            // 100's ends with 's' in case it's the final word
            if (val == 0 && value % 1000 > 100)
                r.Append("s");
        }

        // add currency/group word
        r.Append(" ");
        r.Append(Case(value, info));

        // make the result starting with letter and ending with space
        if (r.Length != 0)
            r.Append(" ");
        return r.ToString().TrimStart(new char[] { ' ' });
    }

    protected override string GetFixedWords(bool male, long value)
    {
        string result = fixedWords[value];
        if (!male)
        {
            if (value == 1)
                return "une";
        }
        return result;
    }

    protected override string GetTen(bool male, long value)
    {
      return tens[value];
    }

    protected override string GetHund(bool male, long value)
    {
      return hunds[value / 100];
    }

    protected override WordInfo GetThousands()
    {
      return thousands;
    }

    protected override WordInfo GetMillions()
    {
      return millions;
    }

    protected override WordInfo GetMilliards()
    {
      return milliards;
    }

    protected override WordInfo GetTrillions()
    {
      return trillions;
    }

    protected override CurrencyInfo GetCurrency(string currencyName)
    {
      currencyName = currencyName.ToUpper();
      if (currencyName == "CAD")
        currencyName = "USD";
      return currencyList[currencyName];
    }

    protected override string GetZero()
    {
        return "zéro";
    }

    protected override string GetMinus()
    {
      return "moins";
    }

    protected override string GetDecimalSeparator()
    {
      return "et ";
    }

    static NumToWordsFr()
    {
      currencyList = new Dictionary<string, CurrencyInfo>();
      currencyList.Add("USD", new CurrencyInfo(
        new WordInfo("dollar", "dollars"),
        new WordInfo("cent", "cents")));
      currencyList.Add("EUR", new CurrencyInfo(
        new WordInfo("euro", "euros"),
        new WordInfo("cent", "cents")));
      currencyList.Add("GBP", new CurrencyInfo(
        new WordInfo("GBP", "GBP"),
        new WordInfo("penny", "penny")));
    }
  }
}