using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Functions
{

  internal class NumToWordsSp : NumToWordsBase
  {

      private static Dictionary<string, CurrencyInfo> currencyList;

      private static WordInfo thousands = new WordInfo("mil");
      private static WordInfo millions = new WordInfo("millón", "millones");
      private static WordInfo milliards = new WordInfo("millardo", "millardos");
      private static WordInfo trillions = new WordInfo("billón", "billiones");

      //if million, milliard or trillions use un instead of uno. 
      private bool _useUn;

      private static string[] hunds = {
          "","cien","doscientos","trescientos","cuatrocientos","quinientos","seiscientos","setecientos","ochocientos","novecientos"
      };

      private static string[] tens =
      {
          "", "diez", "veinte", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa"
      };

      private static string[] fixedWords =
      {
            "","uno","dos","tres","cuatro","cinco","seis","siete","ocho","nueve","diez","once","doce","trece","catorce","quince","dieciséis",
            "diecisiete","dieciocho","diecinueve"
      };

      private static string[] fixedWords21To29 =
      {
            "veintiuno","veintidós","veintitrés","veinticuatro","veinticinco","veintiséis","veintisiete","veintiocho","veintinueve"
      };

        protected override CurrencyInfo GetCurrency(string currencyName)
        {
            currencyName = currencyName.ToUpper();
            return currencyList[currencyName];
        }

        protected override string GetFixedWords(bool male, long value)
        {
            if (_useUn && value == 1) return "un";
            return fixedWords[value];
        }



        protected override string GetHund(bool male, long value)
        {
            return hunds[value / 100];
        }


        protected override WordInfo GetMilliards()
        {
            return milliards;
        }

        protected override WordInfo GetMillions()
        {
            return millions;
        }

        protected override string GetMinus()
        {
            return "menos";
        }

        protected override string GetTen(bool male, long value)
        {
            return tens[value];
        }


        protected override WordInfo GetThousands()
        {
            return thousands;
        }

        protected override WordInfo GetTrillions()
        {
            return trillions;
        }

        protected override string GetZero()
        {
            return "cero";
        }

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
            if (counter == 3 || counter==4 || counter==5) _useUn = true;
            else _useUn = false;
            if (val > 20 && val < 30)
            {
                r.Append(sep100_10 + fixedWords21To29[val - 21]);
            }

            else if (val < GetFixedWordsCount())
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

                // decide whether to use 10-1 separator or not
                if (ten != "" && frac10 != "")
                    r.Append(sep100_10 + ten + Get10_1Separator() + frac10);
                else if (ten != "")
                    r.Append(sep100_10 + ten);
                else if (frac10 != "")
                    r.Append(sep100_10 + frac10);
            }

            // add currency/group word
            r.Append(" ");
            r.Append(Case(value, info));

            // make the result starting with letter and ending with space
            if (r.Length != 0)
                r.Append(" ");
            return r.ToString().TrimStart(new char[] { ' ' });
        }

        protected override string Get10_1Separator()
        {
            return " y ";
        }

        protected override string Get100_10Separator()
        {
            return " ";
        }

        protected override string GetDecimalSeparator()
        {
            return "y ";
        }


        static NumToWordsSp()
        {
            currencyList = new Dictionary<string, CurrencyInfo>(2);
            currencyList.Add("EUR", new CurrencyInfo(
                new WordInfo ("euro", "euros"),
                new WordInfo( "céntimo", "céntimos")));

            currencyList.Add("USD",new CurrencyInfo(
                new WordInfo("dólar", "dólares"),
                new WordInfo("céntimo", "céntimos")));
           
        }
  }
}
