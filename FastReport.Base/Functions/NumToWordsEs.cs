using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Functions
{
  internal class NumToWordsEs : NumToWordsBase
  {
    private static Dictionary<string, CurrencyInfo> currencyList;

    private static string[] fixedWords =
    {
      "", "un", "dos", "tres", "cuatro", "cinco", "seis",
      "siete", "ocho", "nueve", "diez", "once",
      "doce", "trece", "catorce", "quince",
      "dieciséis", "diecisiete", "dieciocho", "diecinueve",
      "veinte", "veintiún", "veintidós", "veintitrés", "veinticuatro", 
      "veinticinco", "veintiséis", "veintisiete", "veintiocho", "veintinueve"
    };

    private static string[] tens =
    {
      "", "diez", "veinte", "treinta", "cuarenta", "cincuenta",
      "sesenta", "setenta", "ochenta", "noventa"
    };

    private static string[] hunds =
    {
      "", "cien", "doscientos", "trescientos", "cuatrocientos",
      "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos"
    };

    private static WordInfo thousands = new WordInfo("mil");
    private static WordInfo millions = new WordInfo("millón", "millones");
    private static WordInfo milliards = new WordInfo("mil");
    private static WordInfo trillions = new WordInfo("billón", "billones");

    protected override void Str(long value, WordInfo senior, StringBuilder result)
    {
      if (value == 0)
      {
        result.Append(GetZero() + " " + senior.many + " ");
      }
      else if (value == 1)
      {
        if (senior.male)
          result.Append("un ");
        else
          result.Append("una ");
        result.Append(senior.one).Append(" ");
      }
      else
      {
        if (value % 1000 != 0)
          result.Append(Str1000(value, senior, 1).Replace("veintiún", "veintiun"));
        else
          result.Append(senior.many + " ");

        value /= 1000;
        result.Insert(0, Str1000(value, GetThousands(), 2));

        value /= 1000;
        result.Insert(0, Str1000(value, GetMillions(), 3));

        // in spanish, the "milliard" is not used. They use "thousand of million" instead
        bool hasMillions = value % 1000 > 0;
        value /= 1000;
        string thousandsOfMillions = Str1000(value, GetThousands(), 4);
        if (value > 0 && !hasMillions)
          thousandsOfMillions += "millones ";
        result.Insert(0, thousandsOfMillions);

        value /= 1000;
        result.Insert(0, Str1000(value, GetTrillions(), 5));
      }
    }

    protected override string Case(long value, WordInfo info)
    {
      // return things (dollars, euros, pages, etc) in the plural form unless it is 1. 
      // the "1" case is handled in the Str method. 
      if (info == thousands || info == millions || info == milliards || info == trillions)
      {
        if (value == 1)
          return info.one;
      }
      return info.many;
    }

    protected override int GetFixedWordsCount()
    {
      return 30;
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
      if (value / 100 == 1 && value % 100 != 0)
        return "ciento";
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
      return "cero";
    }

    protected override string GetMinus()
    {
      return "minus";
    }

    protected override string GetDecimalSeparator()
    {
      return "con ";
    }

    protected override string Get10_1Separator()
    {
      return " y ";
    }

    static NumToWordsEs()
    {
      currencyList = new Dictionary<string, CurrencyInfo>();
      currencyList.Add("USD", new CurrencyInfo(
        new WordInfo("dolar", "dolares"),
        new WordInfo("centavo", "centavos")));
      currencyList.Add("EUR", new CurrencyInfo(
        new WordInfo("euro", "euros"),
        new WordInfo("centavo", "centavos")));
      currencyList.Add("MXN", new CurrencyInfo(
        new WordInfo("peso", "pesos"),
        new WordInfo("centavo", "centavos")));
    }
  }
}