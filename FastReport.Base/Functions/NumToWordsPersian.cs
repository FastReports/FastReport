using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Functions
{
    internal class NumToWordsPersian : NumToWordsBase
    {
        private static Dictionary<string, CurrencyInfo> currencyList;

        private static string[] fixedWords =
        {
      "", "یک", "دو", "سه", "چهار", "پنج", "شش",
      "هفت", "هشت", "نه", "ده", "یازده",
      "دوازده", "سیزده", "چهارده", "پانزده",
      "شانزده", "هفده", "هجده", "نوزده"
    };

        private static string[] tens =
        {
      "", "ده", "بیست", "سی", "چهل", "پنجاه",
      "شصت", "هفتاد", "هشتاد", "نود"
    };

        private static string[] hunds =
        {
      "", "صد", "دویست", "سیصد", "چهارصد",
      "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد"
    };

        private static WordInfo thousands = new WordInfo( "هزار");
        private static WordInfo millions = new WordInfo("میلیون");
        private static WordInfo milliards = new WordInfo("میلیارد");
        private static WordInfo trillions = new WordInfo("تریلیون");

        protected override string GetFixedWords(bool male, long value)
        {
            string result = fixedWords[value];
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
            return currencyList[currencyName];
        }

        protected override string GetZero()
        {
            return "صفر";
        }

        protected override string GetMinus()
        {
            return "منفی";
        }

        protected override string Get10_1Separator()
        {
            return " و ";
        }

        protected override string Get100_10Separator()
        {
            return " و ";
        }

        protected override string GetDecimalSeparator()
        {
            return "و ";
        }
        static NumToWordsPersian()
        {
            currencyList = new Dictionary<string, CurrencyInfo>(3);
            currencyList.Add("EUR", new CurrencyInfo(
              new WordInfo("یورو"),
              new WordInfo("یورو سنت")));
            currencyList.Add("USD", new CurrencyInfo(
              new WordInfo("دلار"),
              new WordInfo("سنت")));
            currencyList.Add("IRR", new CurrencyInfo(
                new WordInfo("ریال‎"), new WordInfo("دینار")));
        }
    }
}
