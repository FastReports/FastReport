using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Barcode
{
    /// <summary>
    /// Generates the Plessey barcode.
    /// </summary>
    public class BarcodePlessey : LinearBarcodeBase
    {
        private const String ALPHABET_STRING = "0123456789ABCDEF";
        private static readonly int[] startWidths = new int[] { 14, 11, 14, 11, 5, 20, 14, 11 };
        private static readonly int[] terminationWidths = new int[] { 25 };
        private static readonly int[] endWidths = new int[] { 20, 5, 20, 5, 14, 11, 14, 11 };
        private static readonly int[][] numberWidths = new int[][]
                                                        {
                                                           new int[] { 5, 20, 5, 20, 5, 20, 5, 20 },     // 0
                                                           new int[] { 14, 11, 5, 20, 5, 20, 5, 20 },    // 1
                                                           new int[] { 5, 20, 14, 11, 5, 20, 5, 20 },    // 2
                                                           new int[] { 14, 11, 14, 11, 5, 20, 5, 20 },   // 3
                                                           new int[] { 5, 20, 5, 20, 14, 11, 5, 20 },    // 4
                                                           new int[] { 14, 11, 5, 20, 14, 11, 5, 20 },   // 5
                                                           new int[] { 5, 20, 14, 11, 14, 11, 5, 20 },   // 6
                                                           new int[] { 14, 11, 14, 11, 14, 11, 5, 20 },  // 7
                                                           new int[] { 5, 20, 5, 20, 5, 20, 14, 11 },    // 8
                                                           new int[] { 14, 11, 5, 20, 5, 20, 14, 11 },   // 9
                                                           new int[] { 5, 20, 14, 11, 5, 20, 14, 11 },   // A / 10
                                                           new int[] { 14, 11, 14, 11, 5, 20, 14, 11 },  // B / 11
                                                           new int[] { 5, 20, 5, 20, 14, 11, 14, 11 },   // C / 12
                                                           new int[] { 14, 11, 5, 20, 14, 11, 14, 11 },  // D / 13
                                                           new int[] { 5, 20, 14, 11, 14, 11, 14, 11 },  // E / 14
                                                           new int[] { 14, 11, 14, 11, 14, 11, 14, 11 }  // F / 15
                                                        };
        private static readonly byte[] crcGrid = new byte[] { 1, 1, 1, 1, 0, 1, 0, 0, 1 };
        private static readonly int[] crc0Widths = new int[] { 5, 20 };
        private static readonly int[] crc1Widths = new int[] { 14, 11 };

        private static readonly string start = "606050060";
        private static readonly string end = "70050050606";
        private static readonly string[] tabelle = new string[] {
                                             "500500500500", //0
                                             "60500500500",  //1
                                             "50060500500",  //2
                                             "6060500500",   //3
                                             "50050060500",  //4
                                             "6050060500",   //5
                                             "5006060500",   //6
                                             "606060500",    //7
                                             "50050050060",  //8
                                             "6050050060",   //9
                                             "5006050060",   //A
                                             "606050060",    //B
                                             "5005006060",   //C
                                             "605006060",    //D
                                             "500606060",    //E
                                             "60606060"      //F
                                         };

        internal override string GetPattern()
        {
            #region ZXing implementation, used for CRC
            string contents = text;
            int length = contents.Length;
            for (int i = 0; i < length; i++)
            {
                int indexInString = ALPHABET_STRING.IndexOf(contents[i]);
                if (indexInString < 0)
                    throw new ArgumentException("Requested contents contains a not encodable character: '" + contents[i] + "'");
            }

            // quiet zone + start pattern + data + crc + termination bar + end pattern + quiet zone
            int codeWidth = 100 + 100 + length * 100 + 25 * 8 + 25 + 100 + 100;
            bool[] result = new bool[codeWidth];
            byte[] crcBuffer = new byte[4 * length + 8];
            int crcBufferPos = 0;
            int pos = 100;
            // start pattern
            pos += appendPattern(result, pos, startWidths, true);
            // data
            for (int i = 0; i < length; i++)
            {
                int indexInString = ALPHABET_STRING.IndexOf(contents[i]);
                int[] widths = numberWidths[indexInString];
                pos += appendPattern(result, pos, widths, true);
                // remember the position number for crc calculation
                crcBuffer[crcBufferPos++] = (byte)(indexInString & 1);
                crcBuffer[crcBufferPos++] = (byte)((indexInString >> 1) & 1);
                crcBuffer[crcBufferPos++] = (byte)((indexInString >> 2) & 1);
                crcBuffer[crcBufferPos++] = (byte)((indexInString >> 3) & 1);
            }
            // CRC calculation
            for (int i = 0; i < (4 * length); i++)
            {
                if (crcBuffer[i] != 0)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        crcBuffer[i + j] ^= crcGrid[j];
                    }
                }
            }
            // append CRC pattern
            for (int i = 0; i < 8; i++)
            {
                switch (crcBuffer[length * 4 + i])
                {
                    case 0:
                        pos += appendPattern(result, pos, crc0Widths, true);
                        break;
                    case 1:
                        pos += appendPattern(result, pos, crc1Widths, true);
                        break;
                }
            }
            // termination bar
            pos += appendPattern(result, pos, terminationWidths, true);
            // end pattern
            appendPattern(result, pos, endWidths, false);
            //return result;
            #endregion

            string pattern = "" + start;

            //data
            foreach(char c in text)
            {
                int i;
                if (int.TryParse("" + c, out i))
                    pattern += tabelle[i];
                else
                {
                    switch (c)
                    {
                        case 'A':
                            pattern += tabelle[10];
                            break;
                        case 'B':
                            pattern += tabelle[11];
                            break;
                        case 'C':
                            pattern += tabelle[12];
                            break;
                        case 'D':
                            pattern += tabelle[13];
                            break;
                        case 'E':
                            pattern += tabelle[14];
                            break;
                        case 'F':
                            pattern += tabelle[15];
                            break;
                        default:
                            throw new Exception("internal Error");
                    }
                }
            }

            //CRC from ZXing
            for (int i = 0; i < 8; i++)
            {
                switch (crcBuffer[text.Length * 4 + i])
                {
                    case 0:
                        pattern += "500";
                        break;
                    case 1:
                        pattern += "60";
                        break;
                }
            }

            pattern += end;

            return pattern;
        }

        /// <summary>
        /// Appends the given pattern to the target array starting at pos.
        /// </summary>
        /// <param name="target">encode black/white pattern into this array</param>
        /// <param name="pos">position to start encoding at in <c>target</c></param>
        /// <param name="pattern">lengths of black/white runs to encode</param>
        /// <param name="startColor">starting color - false for white, true for black</param>
        /// <returns>the number of elements added to target.</returns>
        private int appendPattern(bool[] target, int pos, int[] pattern, bool startColor)
        {
            bool color = startColor;
            int numAdded = 0;
            foreach (int len in pattern)
            {
                for (int j = 0; j < len; j++)
                {
                    target[pos++] = color;
                }
                numAdded += len;
                color = !color; // flip color after each segment
            }
            return numAdded;
        }
    }
}
