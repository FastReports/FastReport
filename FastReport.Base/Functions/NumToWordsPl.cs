using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Functions
{
    internal class NumToWordsPl : NumToWordsBase
    {
        private static Dictionary<string, CurrencyInfo> currencyList;

        private static string[] fixedWords =
        {
      "", "jeden", "dwa", "trzy", "cztery", "pięć", "sześć",
      "siedem", "osiem", "dziewięć", "dziesięć", "jedenaście",
      "dwanaście", "trzynaście", "czternaście", "piętnaście",
      "szesnaście", "siedemnaście", "osiemnaście", "dziewiętnaście"
    };

        private static string[] tens =
        {
      "", "dziesięć", "dwadzieścia", "trzydzieści", "czterdzieści", "pięćdziesiąt",
      "sześćdziesiąt", "siedemdziesiąt", "osiemdziesiąt", "dziewięćdziesiąt"
    };

        private static string[] hunds =
        {
      "", "sto", "dwieście", "trzysta", "czterysta",
      "czterysta", "sześćset", "siedemset", "osiemset", "dziewięćset"
    };

        private static WordInfo thousands = new WordInfo(false, "tysiąc", "tysiące", "tysięcy");
        private static WordInfo millions = new WordInfo(true, "milion", "miliony", "milionów");
        private static WordInfo milliards = new WordInfo(true, "miliard", "miliardy", "miliardów");
        private static WordInfo trillions = new WordInfo(true, "bilion", "biliony", "bilionów");

        protected override string GetFixedWords(bool male, long value)
        {
            string result = fixedWords[value];
            if (!male)
            {
                if (value == 1)
                    return "jedna";
                if (value == 2)
                    return "dwie";
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
            if (currencyName == "RUR")
                currencyName = "RUB";
            return currencyList[currencyName];
        }

        protected override string GetZero()
        {
            return "zero";
        }

        protected override string GetMinus()
        {
            return "minus";
        }

        protected override string Case(long value, WordInfo info)
        {
            value = value % 100;
            if (value > GetFixedWordsCount())
                value = value % 10;

            switch (value)
            {
                case 1:
                    return info.one;

                case 2:
                case 3:
                case 4:
                    return info.two;

                default:
                    return info.many;
            }
        }

        static NumToWordsPl()
        {
            currencyList = new Dictionary<string, CurrencyInfo>(1);
            currencyList.Add("PLN", new CurrencyInfo(
              new WordInfo(true, "złoty", "zlote", "złotych"),
              new WordInfo(false, "grosz", "grosze", "groszy")));
    
        }
    }
}