using System.Collections.Generic;

namespace FastReport.Barcode
{
    internal static class GS1Helper
    {
        struct AI
        {
            public string numAI;
            public int numAILength;
            public int minDataLength;
            public int maxDataLength;
            public bool useFNC1;

            public AI(string numAI, int numAILength, int minDataLength, int maxDataLength, bool useFNC1)
            {
                this.numAI = numAI;
                this.numAILength = numAILength;
                this.minDataLength = minDataLength;
                this.maxDataLength = maxDataLength;
                this.useFNC1 = useFNC1;
            }
        }

        static List<AI> AICodes = new List<AI>()
        {
            new AI("00", 2, 18, 18, false),
            new AI("01", 2, 14, 14, false),
            new AI("02", 2, 14, 14, false),
            new AI("10", 2, 1, 20, true),
            new AI("11", 2, 6, 6, false),
            new AI("12", 2, 6, 6, false),
            new AI("13", 2, 6, 6, false),
            new AI("15", 2, 6, 6, false),
            new AI("16", 2, 6, 6, false),
            new AI("17", 2, 6, 6, false),
            new AI("20", 2, 2, 2, false),
            new AI("21", 2, 1, 20, true),
            new AI("22", 2, 1, 20, true),
            new AI("240", 3, 1, 30, true),
            new AI("241", 3, 1, 30, true),
            new AI("242", 3, 1, 6, true),
            new AI("243", 3, 1, 20, true),
            new AI("250", 3, 1, 30, true),
            new AI("251", 3, 1, 30, true),
            new AI("253", 3, 13, 30, true),
            new AI("254", 3, 1, 20, true),
            new AI("255", 3, 13, 25, true),
            new AI("30", 2, 1, 8, true),
            new AI("31XX", 4, 6, 6, false),  // all 31xx
            new AI("32XX", 4, 6, 6, false),  // all 32xx
            new AI("33XX", 4, 6, 6, false),  // all 33xx
            new AI("34XX", 4, 6, 6, false),  // all 34xx
            new AI("35XX", 4, 6, 6, false),  // all 35xx
            new AI("36XX", 4, 6, 6, false),  // all 36xx
            new AI("37", 2, 1, 8, true),
            new AI("390X", 4, 1, 15, true),
            new AI("391X", 4, 3, 18, true),
            new AI("392X", 4, 1, 15, true),
            new AI("393X", 4, 3, 18, true),
            new AI("394X", 4, 4, 4, true),
            new AI("400", 3, 1, 30, true),
            new AI("401", 3, 1, 30, true),
            new AI("402", 3, 17, 17, true),
            new AI("403", 3, 1, 30, true),
            new AI("41X", 3, 13, 13, false),
            new AI("420", 3, 1, 20, true),
            new AI("421", 3, 3, 12, true),
            new AI("422", 3, 3, 3, true),
            new AI("423", 3, 3, 15, true),
            new AI("424", 3, 3, 3, true),
            new AI("425", 3, 3, 15, true),
            new AI("426", 3, 3, 3, true),
            new AI("7001", 4, 13, 13, true),
            new AI("7002", 4, 1, 30, true),
            new AI("7003", 4, 10, 10, true),
            new AI("7004", 4, 1, 4, true),
            new AI("7005", 4, 1, 12, true),
            new AI("7006", 4, 6, 6, true),
            new AI("7007", 4, 6, 12, true),
            new AI("7008", 4, 1, 3, true),
            new AI("7009", 4, 1, 10, true),
            new AI("7010", 4, 1, 2, true),
            new AI("7020", 4, 1, 20, true),
            new AI("7021", 4, 1, 20, true),
            new AI("7022", 4, 1, 20, true),
            new AI("7023", 4, 1, 30, true),
            new AI("703X", 4, 3, 30, true),
            new AI("710", 3, 1, 20, true),
            new AI("711", 3, 1, 20, true),
            new AI("712", 3, 1, 20, true),
            new AI("713", 3, 1, 20, true),
            new AI("71X", 3, 1, 20, true),
            new AI("8001", 4, 14, 14, true),
            new AI("8002", 4, 1, 20, true),
            new AI("8003", 4, 14, 30, true),
            new AI("8004", 4, 1, 30, true),
            new AI("8005", 4, 6, 6, true),
            new AI("8006", 4, 18, 18, true),
            new AI("8007", 4, 1, 34, true),
            new AI("8008", 4, 8, 12, true),
            new AI("8010", 4, 1, 30, true),
            new AI("8011", 4, 1, 12, true),
            new AI("8012", 4, 1, 20, true),
            new AI("8013", 4, 1, 30, true),
            new AI("8017", 4, 18, 18, true),
            new AI("8018", 4, 18, 18, true),
            new AI("8019", 4, 1, 10, true),
            new AI("8020", 4, 1, 25, true),
            new AI("8110", 4, 1, 70, true),
            new AI("8111", 4, 4, 4, true),
            new AI("8112", 4, 1, 70, true),
            new AI("8200", 4, 1, 70, true),
            new AI("90", 2, 1, 30, true),
            new AI("9X", 2, 1, 90, true)
        };

        static int FindAIIndex(string code, int index)
        {
            int codeLen, maxLen, result;
            result = -1;

            if (index == -1)
                return -1;

            if (code[index] != '(')
                return result;

            codeLen = code.Length - index;
            if (codeLen < 3)
                return result;

            for (int i = 0; i < AICodes.Count; i++)
            {
                result = -1;
                maxLen = AICodes[i].numAI.Length;
                if (maxLen > codeLen)
                    continue;

                for (int j = 0; j < maxLen; j++)
                {
                    result = i;
                    if ((AICodes[i].numAI[j] != code[index + j + 1]) && (AICodes[i].numAI[j] != 'X'))
                    {
                        result = -1;
                        break;
                    }
                }

                if (result != -1)
                    break;
            }

            return result;
        }

        static string GetCode(string code, ref int index)
        {
            int foundAI, maxLen, codeLen, tempIndex;
            string result = "";
            foundAI = FindAIIndex(code, index);

            if (foundAI == -1)
                return "";

            index += AICodes[foundAI].numAI.Length + 1;

            if (code[index] != ')')
                return "";

            codeLen = code.Length - index - 1;
            index++;

            if (!AICodes[foundAI].useFNC1 && (codeLen >= AICodes[foundAI].maxDataLength))
            {
                result = code.Substring(index - AICodes[foundAI].numAILength - 1, AICodes[foundAI].numAILength) +
                    code.Substring(index, AICodes[foundAI].maxDataLength);
                index += AICodes[foundAI].maxDataLength;
            }
            else if (AICodes[foundAI].useFNC1)
            {
                maxLen = codeLen;

                tempIndex = code.IndexOf('(', index);
                if (tempIndex != -1)
                    maxLen = tempIndex - index;

                if (maxLen < 0)
                    maxLen = codeLen;

                if ((maxLen >= AICodes[foundAI].minDataLength) && (maxLen <= AICodes[foundAI].maxDataLength))
                {
                    result = code.Substring(index - AICodes[foundAI].numAILength - 1, AICodes[foundAI].numAILength) +
                        code.Substring(index, maxLen);
                    if (maxLen < codeLen)
                        result += "&1;";
                    index += maxLen;
                }
            }

            return result;
        }

        public static string ParseGS1(string code)
        {
            var result = "";
            if (code.Length < 3)
                return result;

            var str = "";
            var i = 0;
            result = "&1;";
            while (i < code.Length)
            {
                str = GetCode(code, ref i);
                if (str != "")
                    result += str;
                else
                {
                    result = "";
                    return result;
                }
            }

            return result;
        }
    }
}