using System.Text;
using System.Collections.Generic;

namespace FastReport.Functions
{
    internal class NumToWordsIn : NumToWordsBase
    {
        private bool shouldReturnMany = false; //if third or fourth counter, so use lakh/crore instead of thousands/millions
        private static Dictionary<string, CurrencyInfo> currencyList;

        private static string[] fixedWords =
        {
            "", "one", "two", "three", "four", "five", "six", "seven",
            "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen",
            "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
        };

        private static string[] tens =
        {
            "", "ten", "twenty", "thirty",
            "forty", "fifty", "sixty", "seventy",
            "eighty", "ninety"
        };

        private static string[] hunds =
    {
      "", "one hundred", "two hundred", "three hundred", "four hundred",
      "five hundred", "six hundred", "seven hundred", "eight hundred", "nine hundred"
    };


        private static WordInfo thousands = new WordInfo("thousand", "crore");
        private static WordInfo millions = new WordInfo("million", "lakh");
        private static WordInfo milliards = new WordInfo("billion", "crore");
        private static WordInfo trillions = new WordInfo("trillion");

        protected override CurrencyInfo GetCurrency(string currencyName)
        {
            currencyName = currencyName.ToUpper();
            return currencyList[currencyName];
        }

        protected override string GetFixedWords(bool male, long value)
        {
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
            return "minus";
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
            return "zero";
        }

        protected override string GetDecimalSeparator()
        {
            return "and ";
        }

        protected override string Get10_1Separator()
        {
            return "-";
        }

        protected override void Str(long value, WordInfo senior, StringBuilder result)
        {
            if (value == 0)
                result.Append(GetZero() + " " + senior.many + " ");
            else
            {
                if (value % 1000 != 0)
                    result.Append(Str1000(value, senior, 1));
                else
                    result.Append(" " + senior.many + " ");
                value /= 1000;
                //grouping digits not by threes as in international system, but by sets of two digits
                result.Insert(0, Str1000(value, GetThousands(), 2));
                value /= 100;
                result.Insert(0, Str1000(value, GetMillions(), 3));
                value /= 100;
                result.Insert(0, Str1000(value, GetMilliards(), 4));
                value /= 100;
                result.Insert(0, Str1000(value, new WordInfo("arab"), 5));
                value /= 100;
                result.Insert(0, Str1000(value, new WordInfo("kharab"), 6));
                value /= 100;
                result.Insert(0, Str1000(value, new WordInfo("nil"), 7));
                result.Replace("  ", " ");

            }
        }


        protected override string Case(long value, WordInfo info)
        {
            if (shouldReturnMany == true)
            {
                return info.many;
            }
            return info.one;

            /*if (value % 100 == 1)
                return info.one;
            return info.many;*/
        }

        protected override string Str1000(long value, WordInfo info, int counter)
        {
            long val;

            //if its third or 
            if (counter == 3 || counter == 4) shouldReturnMany = true;
            else shouldReturnMany = false;

            if (counter == 1) val = value % 1000;
            else val = value % 100;

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

                // decide whether to use 10-1 separator or not
                if (ten != "" && frac10 != "")
                    r.Append(sep100_10 + ten + Get10_1Separator() + frac10);
                else if (ten != "")
                    r.Append(sep100_10 + ten);
                else if (frac10 != "")
                    r.Append(sep100_10 + frac10);
            }
            //Twenty-Four crore and seventy-Five lakh and Eighty-Zero thousand  rupees and 00 paise
            // add currency/group word
            r.Append(" ");
            if (counter == 1 && value % 100 != 1)
            {
                shouldReturnMany = true;
            }
            r.Append(Case(value, info));

            // make the result starting with letter and ending with space
            if (r.Length != 0)
                r.Append(" ");
            return r.ToString().TrimStart(new char[] { ' ' });


        }


        static NumToWordsIn()
        {
            currencyList = new Dictionary<string, CurrencyInfo>();
            currencyList.Add("USD", new CurrencyInfo(
              new WordInfo("dollar", "dollars"),
              new WordInfo("cent", "cents")));
            currencyList.Add("EUR", new CurrencyInfo(
              new WordInfo("euro", "euros"),
              new WordInfo("cent", "cents")));
            currencyList.Add("INR", new CurrencyInfo(
                new WordInfo("rupee", "rupees"),
                new WordInfo("paise")
                ));

        }
    }
}
