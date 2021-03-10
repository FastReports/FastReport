using System.Collections.Generic;

namespace ClickHouse.Client.Types.Grammar
{
    public static class Tokenizer
    {
        private static char[] breaks = new[] { ',', '(', ')' };

        public static IEnumerable<string> GetTokens(string input)
        {
            var start = 0;
            var len = input.Length;

            while (start < len)
            {
                var nextBreak = input.IndexOfAny(breaks, start);
                if (nextBreak == start)
                {
                    start++;
                    yield return input.Substring(nextBreak, 1);
                }
                else if (nextBreak == -1)
                {
                    yield return input.Substring(start).Trim();
                    yield break;
                }
                else
                {
                    yield return input.Substring(start, nextBreak - start).Trim();
                    start = nextBreak;
                }
            }
        }
    }
}
