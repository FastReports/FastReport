using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Functions
{
    internal class NumToWordsUkr : NumToWordsBase
    {
        private static Dictionary<string, CurrencyInfo> currencyList;

        private static string[] fixedWords =
        {
      "", "один", "два", "три", "чотири", "п’ять", "шість",
      "сім", "вісім", "дев’ять", "десять", "одинадцять",
      "дванадцять", "тринадцять", "чотирнадцять", "п'ятнадцять",
      "шістнадцять", "сімнадцять", "вісімнадцять", "дев'ятнадцять"
    };

        private static string[] tens =
        {
      "", "десять", "двадцять", "тридцять", "сорок", "п'ятдесят",
      "шістдесят", "сімдесят", "вісімдесят", "дев'яносто"
    };

        private static string[] hunds =
        {
      "", "сто", "двісті", "триста", "чотириста",
      "п'ятсот", "шістсот", "сімсот", "вісімсот", "дев'ятсот"
    };

        private static WordInfo thousands = new WordInfo(false, "тисяч", "тисячі", "тисяч");
        private static WordInfo millions = new WordInfo(true, "мільйон", "мільйона", "мільйонів");
        private static WordInfo milliards = new WordInfo(true, "мільярд", "мільярда", "мільярдів");
        private static WordInfo trillions = new WordInfo(true, "трильйон", "трильйона", "трильйонів");

        protected override string GetFixedWords(bool male, long value)
        {
            string result = fixedWords[value];
            if (!male)
            {
                if (value == 1)
                    return "одна";
                if (value == 2)
                    return "дві";
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
            return "нуль";
        }

        protected override string GetMinus()
        {
            return "мінус";
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

        static NumToWordsUkr()
        {
            currencyList = new Dictionary<string, CurrencyInfo>(4);
            currencyList.Add("RUB", new CurrencyInfo(
              new WordInfo(true, "рубль", "рубля", "рублів"),
              new WordInfo(false, "копійка", "копійки", "копійок")));
            currencyList.Add("UAH", new CurrencyInfo(
              new WordInfo(false, "гривня", "гривні", "гривень"),
              new WordInfo(false, "копійка", "копійки", "копійок")));
            currencyList.Add("EUR", new CurrencyInfo(
              new WordInfo(true, "євро", "євро", "євро"),
              new WordInfo(true, "євроцент", "євроцента", "євроценту")));
            currencyList.Add("USD", new CurrencyInfo(
              new WordInfo(true, "долар", "долара", "доларів"),
              new WordInfo(true, "цент", "цента", "центів")));
        }
    }
}