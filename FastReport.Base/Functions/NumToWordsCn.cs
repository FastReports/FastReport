using System.Text;
using System.Collections.Generic;

namespace FastReport.Functions
{
    /// <summary>
    /// Converts numbers to Chinese financial notation (大写) for currencies CNY, USD, EUR.
    /// </summary>
    internal class NumToWordsCn : NumToWordsBase
    {
        private static Dictionary<string, CurrencyInfo> currencyList;

        private static readonly string Zero = "零";

        private static readonly string[] FixedWords =
        {
            Zero, "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖"
        };

        private static readonly string Ten = "拾";            // 10
        private static readonly string Hundred = "佰";        // 100
        private static readonly string Thousand = "仟";       // 1 000
        private static readonly string TenThousand = "万";    // 10 000
        private static readonly string HundredMillion = "亿"; // 100 000 000

        private static readonly string Yuan = "元";       // Chinese Yuan (CNY)
        private static readonly string Jiao = "角";       // 0.1 Yuan (1/10 of Yuan)
        private static readonly string Fen = "分";        // 0.01 Yuan (1/100 of Yuan)
        private static readonly string USD = "美元";      // US Dollar
        private static readonly string USCent = "美分";   // US Cents
        private static readonly string EUR = "欧元";      // Euro
        private static readonly string EuroCent = "欧分"; // Euro Cents

        private static readonly string WholeAmount = "整"; // "Exact amount" (used for whole numbers without fractional part)
        private static readonly string Minus = "负";

        private static readonly WordInfo unused = new WordInfo("");

        private bool _hasNonZeroIntegerPart = false;

        protected override void Str(long value, WordInfo senior, StringBuilder result)
        {
            if (IsFractionalUnit(senior))
            {
                HandleFractionalPart(value, senior, result);
            }
            else
            {
                HandleIntegerPart(value, senior, result);
            }
        }

        private void HandleFractionalPart(long value, WordInfo senior, StringBuilder result)
        {
            if (value == 0)
            {
                result.Append(WholeAmount);
                return;
            }

            if (senior.one == Fen)
            {
                HandleCnyFractional(value, result);
            }
            else
            {
                HandleForeignCurrencyFractional(value, senior, result);
            }
        }

        /// <summary>
        /// Handles USD/EUR fractional part as cent amounts.
        /// Adds "零" before single-digit cents when integer part exists.
        /// </summary>
        private void HandleForeignCurrencyFractional(long value, WordInfo senior, StringBuilder result)
        {
            string fractionalPart = ConvertInteger(value);
            // Add "零" before single-digit cents if integer part is non-zero
            if (_hasNonZeroIntegerPart && value < 10)
            {
                result.Append(Zero);
            }
            result.Append(fractionalPart);

            if (value != 0)
            {
                if (senior.one == USCent) result.Append(USCent);
                else if (senior.one == EuroCent) result.Append(EuroCent);

            }
        }

        /// <summary>
        /// Handles CNY fractional part with 角(jiao)/分(fen) structure.
        /// Adds "零" (zero) between integer part and 分 when 角 is missing.
        /// </summary>
        private void HandleCnyFractional(long value, StringBuilder result)
        {
            int jiao = (int)(value / 10);
            int fen = (int)(value % 10);

            if (jiao > 0)
                result.Append(FixedWords[jiao]).Append(Jiao);

            if (fen > 0)
            {
                // Add "零" only if there's no 角 but integer part exists
                if (jiao == 0 && _hasNonZeroIntegerPart)
                    result.Append(Zero);
                result.Append(FixedWords[fen]).Append(Fen);
            }
        }

        private void HandleIntegerPart(long value, WordInfo senior, StringBuilder result)
        {
            _hasNonZeroIntegerPart = (value != 0);

            string integerPart = ConvertInteger(value);
            if (value == 0 && string.IsNullOrEmpty(integerPart))
            {
                integerPart = Zero;
            }
            result.Append(integerPart);

            // Add currency symbol
            if (senior.one == Yuan) result.Append(Yuan);
            else if (senior.one == USD) result.Append(USD);
            else if (senior.one == EUR) result.Append(EUR);
        }

        private bool IsFractionalUnit(WordInfo unit)
        {
            return (unit != null) &&
                (unit.one == Fen || unit.one == USCent || unit.one == EuroCent);
        }

        /// <summary>
        /// Converts integer number to Chinese financial notation using 亿/万 blocks.
        /// </summary>
        private string ConvertInteger(long num)
        {
            if (num == 0) return "";

            long units = num % 10000;
            long tenThousand = (num / 10000) % 10000;
            long hundredMillion = num / 100000000;

            StringBuilder sb = new StringBuilder();

            // 亿
            if (hundredMillion > 0)
            {
                sb.Append(ConvertFourDigits((int)hundredMillion));
                sb.Append(HundredMillion);
            }

            // 万
            if (tenThousand > 0)
            {
                // Missing digits between 亿 and 万 blocks
                if (hundredMillion > 0 && units > 0 && tenThousand < 1000)
                {
                    if (!sb.ToString().EndsWith(Zero))
                        sb.Append(Zero);
                }
                sb.Append(ConvertFourDigits((int)tenThousand));
                sb.Append(TenThousand);
            }
            else if (hundredMillion > 0 && units > 0)
            {
                sb.Append(Zero);
            }

            if (units > 0)
            {
                string unitStr = ConvertFourDigits((int)units);
                // Missing thousands after 万 block
                if (tenThousand > 0 && units < 1000 && !unitStr.StartsWith(Zero))
                {
                    unitStr = Zero + unitStr;
                }
                sb.Append(unitStr);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts 4-digit number to Chinese financial notation.
        /// </summary>
        private string ConvertFourDigits(int num)
        {
            if (num == 0) return "";
            StringBuilder sb = new StringBuilder();

            bool hasThousands = false;
            bool hasHundreds = false;

            if (num >= 1000)
            {
                sb.Append(FixedWords[num / 1000]).Append(Thousand);
                num %= 1000;
                hasThousands = true;
            }

            if (num >= 100)
            {
                sb.Append(FixedWords[num / 100]).Append(Hundred);
                num %= 100;
                hasHundreds = true;
            }
            else if ((hasThousands) && num > 0)
            {
                // Missing hundreds between thousands and lower digits
                sb.Append(Zero);
            }

            if (num >= 10)
            {
                sb.Append(FixedWords[num / 10]).Append(Ten);
                num %= 10;
            }
            else if ((hasThousands || hasHundreds) && num > 0)
            {
                // Missing tens between higher digits and units
                if (!sb.ToString().EndsWith(Zero))
                    sb.Append(Zero);
            }

            if (num > 0)
            {
                sb.Append(FixedWords[num]);
            }

            return sb.ToString();
        }

        protected override int GetFixedWordsCount() => 10;
        protected override string GetFixedWords(bool male, long value) =>
             (value >= 0 && value < FixedWords.Length) ? FixedWords[value] : string.Empty;
        protected override string GetTen(bool male, long value) => string.Empty;
        protected override string GetHund(bool male, long value) => string.Empty;

        protected override WordInfo GetThousands() => unused;
        protected override WordInfo GetMillions() => unused;
        protected override WordInfo GetMilliards() => unused;
        protected override WordInfo GetTrillions() => unused;

        protected override CurrencyInfo GetCurrency(string currencyName)
        {
            currencyName = currencyName.ToUpper();
            if (currencyList.TryGetValue(currencyName, out var info))
                return info;
            return currencyList["CNY"];
        }

        protected override string GetZero() => Zero;
        protected override string GetMinus() => Minus;
        protected override string GetDecimalSeparator() => string.Empty;
        protected override string Get10_1Separator() => string.Empty;
        protected override string Get100_10Separator() => string.Empty;
        protected override string Case(long value, WordInfo info) => info.one;

        static NumToWordsCn()
        {
            currencyList = new Dictionary<string, CurrencyInfo>
            {
                ["CNY"] = new CurrencyInfo(new WordInfo(Yuan), new WordInfo(Fen)),
                ["USD"] = new CurrencyInfo(new WordInfo(USD), new WordInfo(USCent)),
                ["EUR"] = new CurrencyInfo(new WordInfo(EUR), new WordInfo(EuroCent))
            };
        }
    }
}