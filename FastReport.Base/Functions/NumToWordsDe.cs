using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Functions
{
  internal class NumToWordsDe : NumToWordsBase
  {
    private static Dictionary<string, CurrencyInfo> currencyList;

    private static string[] fixedWords =
    {
      "", "ein", "zwei", "drei", "vier", "fünf", "sechs",
      "sieben", "acht", "neun", "zehn", "elf",
      "zwölf", "dreizehn", "vierzehn", "fünfzehn",
      "sechzehn", "siebzehn", "achtzehn", "neunzehn"
    };

    private static string[] tens =
    {
      "", "zehn", "zwanzig", "dreißig", "vierzig", "fünfzig",
      "sechzig", "siebzig", "achtzig", "neunzig"
    };

    private static string[] hunds =
    {
      "", "einhundert", "zweihundert", "dreihundert", "vierhundert",
      "fünfhundert", "sechshundert", "siebenhundert", "achthundert", "neunhundert"
    };

    private static WordInfo thousands = new WordInfo(false, "tausend", "tausend", "tausend");
    private static WordInfo millions = new WordInfo(false, "Million", "Millionen", "Millionen");
    private static WordInfo milliards = new WordInfo(false, "Milliarde", "Milliarden", "Milliarden");
    private static WordInfo trillions = new WordInfo(false, "Billion", "Billionen", "Billionen");

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

        val = val % 100;
        if (val < GetFixedWordsCount())
        {
            // val is less than fixed words count (usually 20), get fixed words
            string frac20 = GetFixedWords(info.male, val);
            if (frac20 != "")
                r.Append(frac20);
        }
        else
        {
            // val is greater than fixed words count (usually 20), get tens separately
            string ten = GetTen(info.male, val / 10);
            string frac10 = GetFixedWords(info.male, val % 10);

            // decide whether to use 10-1 separator or not
            if (ten != "" && frac10 != "")
                r.Append(frac10 + "und" + ten);
            else if (ten != "")
                r.Append(ten);
            else if (frac10 != "")
                r.Append(frac10);
        }

        string separator = counter == 2 ? "" : " "; // thousands do not use separator
        // add currency/group word
        r.Append(separator);
        r.Append(Case(value, info));

        // make the result starting with letter and ending with space
        if (r.Length != 0)
            r.Append(separator);
        return r.ToString().TrimStart(new char[] { ' ' });
    }


    protected override string GetFixedWords(bool male, long value)
    {
      string result = fixedWords[value];
      if (!male)
      {
        if (value == 1)
          return "eine";
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
      return "null";
    }

    protected override string GetMinus()
    {
      return "minus";
    }

    protected override string GetDecimalSeparator()
    {
      return "und ";
    }

    static NumToWordsDe()
    {
      currencyList = new Dictionary<string, CurrencyInfo>();
      currencyList.Add("USD", new CurrencyInfo(
        new WordInfo("Dollar", "Dollar"),
        new WordInfo("Cent", "Cent")));
      currencyList.Add("EUR", new CurrencyInfo(
        new WordInfo("Euro", "Euro"),
        new WordInfo("Cent", "Cent")));
      currencyList.Add("GBP", new CurrencyInfo(
        new WordInfo("Pfund", "Pfund"),
        new WordInfo("Penny", "Penny")));
    }
  }
}