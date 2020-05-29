using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Functions
{
  internal class NumToWordsNl : NumToWordsBase
  {
    private static Dictionary<string, CurrencyInfo> currencyList;

    private static string[] fixedWords =
    {
      "", "een", "twee", "drie", "vier", "vijf", "zes",
      "zeven", "acht", "negen", "tien", "elf",
      "twaalf", "dertien", "veertien", "vijftien",
      "zestien", "zeventien", "achttien", "negentien"
    };

    private static string[] tens =
    {
      "", "tien", "twintig", "dertig", "veertig", "vijftig",
      "zestig", "zeventig", "tachtig", "negentig"
    };

    private static string[] hunds =
    {
      "", "honderd", "tweehonderd", "driehonderd", "vierhonderd",
      "vijfhonderd", "zeshonderd", "zevenhonderd", "achthonderd", "negenhonderd"
    };

    private static WordInfo thousands = new WordInfo("duizend");
    private static WordInfo millions = new WordInfo("miljoen");
    private static WordInfo milliards = new WordInfo("miljard");
    private static WordInfo trillions = new WordInfo("trillion");

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
        if (val < GetFixedWordsCount())
        {
            // val is less than fixed words count (usually 20), get fixed words
            string frac20 = GetFixedWords(info.male, val);
            if (frac20 != "")
                r.Append(sep100_10 + frac20);
        }
        else
        {
            // val is greater than fixed words count (usually 20), get tens separately
            string ten = GetTen(info.male, val / 10);
            string frac10 = GetFixedWords(info.male, val % 10);

            string sep10_1 = "en";
            if (val % 10 == 2 || val % 10 == 3)
                sep10_1 = "ën";
            // decide whether to use 10-1 separator or not
            if (ten != "" && frac10 != "")
                r.Append(sep100_10 + frac10 + sep10_1 + ten);
            else if (ten != "")
                r.Append(sep100_10 + ten);
            else if (frac10 != "")
                r.Append(sep100_10 + frac10);
        }

        // add currency/group word
        if (counter != 2)
            r.Append(" ");
        r.Append(Case(value, info));

        // make the result starting with letter and ending with space
        if (r.Length != 0)
            r.Append(" ");
        return r.ToString().TrimStart(new char[] { ' ' });
    }

    protected override string GetFixedWords(bool male, long value)
    {
      return fixedWords[value];
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
      return "nul";
    }

    protected override string GetMinus()
    {
      return "min";
    }

    protected override string GetDecimalSeparator()
    {
      return "en ";
    }

    static NumToWordsNl()
    {
      currencyList = new Dictionary<string, CurrencyInfo>();
      currencyList.Add("USD", new CurrencyInfo(
        new WordInfo("dollar", "dollar"),
        new WordInfo("cent", "cent")));
      currencyList.Add("EUR", new CurrencyInfo(
        new WordInfo("euro", "euro"),
        new WordInfo("cent", "cent")));
      currencyList.Add("GBP", new CurrencyInfo(
        new WordInfo("pound", "pound"),
        new WordInfo("penny", "penny")));
    }
  }
}