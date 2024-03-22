using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace FastReport.Barcode
{
    /// <summary>
    /// Base methods for GS1 DataBar barcodes.
    /// </summary>
    public class BarcodeGS1Base : LinearBarcodeBase
    {
        protected List<string> EncodedData { get; set; }

        /// <summary>
        /// Routine to generate widths for GS1 elements for a given value.
        /// </summary>
        /// <param name="val">Required value.</param>
        /// <param name="n">Number of modules.</param>
        /// <param name="elements">Elements in a set (GS1 omni based and Expanded = 4; GS1 Limited = 7).</param>
        /// <param name="maxWidth">Maximum module width of an element.</param>
        /// <param name="noNarrow">False will skip patterns without a one module wide element.</param>
        /// <returns>Element widths</returns>
        protected List<int> GetGS1Widths(int val, int n, int elements, int maxWidth, int noNarrow)
        {
            int elmWidth;
            int subVal, lessVal;
            int narrowMask = 0;
            List<int> widths = new List<int>();

            for (int bar = 0; bar < elements - 1; bar++)
            {
                for (elmWidth = 1, narrowMask |= (1 << bar); ; elmWidth++, narrowMask &= ~(1 << bar))
                {
                    // Get all combinations
                    subVal = Combins(n - elmWidth - 1, elements - bar - 2);

                    //Less combinations with no single-module element
                    if ((noNarrow == 0) && (narrowMask == 0) &&
                    (n - elmWidth - (elements - bar - 1) >= elements - bar - 1))
                    {
                        subVal -= Combins(n - elmWidth - (elements - bar), elements - bar - 2);
                    }

                    // Less combinations with elements > maxVal
                    if (elements - bar - 1 > 1)
                    {
                        lessVal = 0;
                        for (int mxwElement = n - elmWidth - (elements - bar - 2); mxwElement > maxWidth; mxwElement--)
                        {
                            lessVal += Combins(n - elmWidth - mxwElement - 1, elements - bar - 3);
                        }
                        subVal -= lessVal * (elements - 1 - bar);
                    }
                    else if (n - elmWidth > maxWidth)
                    {
                        subVal--;
                    }
                    val -= subVal;
                    if (val < 0)
                        break;
                }
                val += subVal;
                n -= elmWidth;
                widths.Add(elmWidth);
            }
            widths.Add(n);

            return widths;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="r"></param>
        /// <returns>Returns the number of Combinations of r selected from n.</returns>
        private int Combins(int n, int r)
        {
            int i, j;
            int maxDenom, minDenom;
            int val;

            if (n - r > r)
            {
                minDenom =
                r; maxDenom
                = n - r;
            }
            else
            {
                minDenom = n - r;
                maxDenom = r;
            }
            val = 1;
            j = 1;
            for (i = n; i > maxDenom; i--)
            {
                val *= i;
                if (j <= minDenom)
                {
                    val /= j;
                    j++;
                }
            }

            for (; j <= minDenom; j++)
            {
                val /= j;
            }

            return (val);
        }

        /// <summary>
        /// Drawing lines of strokes
        /// </summary>
        /// <param name="data">Encoded data in width strokes; For separate line, these are colored strokes, any value that is not equal to zero is black.</param>
        /// <param name="g"></param>
        /// <param name="zoom">Scale size.</param>
        /// <param name="rect">Use left of rectangle for  to set start position x, top for top pos y, bottom for bottom pos y of strokes.</param>
        /// <param name="reversColor">Flag for reversing color by default first strokes white, disabled for separate line. </param>
        /// <param name="separatorLine">Flag separete line </param>
        protected void DrawLineBars(string data, IGraphics g, float zoom, RectangleF rect, bool reversColor, bool separatorLine = false)
        {
            using (Pen pen = new Pen(Color))
            {
                float currentWidth = rect.Left;
                for (int x = 0; x < data.Length; x++)
                {
                    float heightStart = rect.Top;
                    float heightEnd = rect.Bottom;
                    float width = WideBarRatio;
                    if (!separatorLine)
                        width = (data[x] - '0') * WideBarRatio;

                    width *= zoom;
                    heightStart *= zoom;
                    heightEnd *= zoom;
                    pen.Width = width;

                    if (reversColor)
                        pen.Color = Color;
                    else
                        pen.Color = Color.Transparent;

                    if (separatorLine)
                    {
                        if (data[x] != '0')
                            pen.Color = Color;
                    }
                    else
                    {
                        if ((x % 2 != 0 && !reversColor))
                            pen.Color = Color;
                        if ((x % 2 != 0 && reversColor))
                            pen.Color = Color.Transparent;
                    }

                    g.DrawLine(pen,
                        currentWidth + width / 2,
                        barArea.Top * zoom + heightStart,
                        currentWidth + width / 2,
                        barArea.Top * zoom + heightEnd);

                    currentWidth += width;
                }
            }
        }

        /// <inheritdoc />
        public override string GetDefaultValue()
        {
            return "(01)0000123456789";
        }
    }

    /// <summary>
    /// Generates the GS1 DataBar Omnidirectional barcode.
    /// </summary>
    public class BarcodeGS1Omnidirectional : BarcodeGS1Base
    {
        /// <summary>
        /// Get value for encoding.
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns></returns>
        protected long GetValue(string data)
        {
            long result;
            string prefix = "";
            int startPrefix = data.IndexOf('(');
            int endPrefix = data.IndexOf(')');

            if (startPrefix >= 0 && endPrefix > 0)
            {
                prefix = data.Substring(startPrefix, endPrefix + 1);
                data = data.Replace(prefix, "");
            }

            if (data.Length > 13)
                data = data.Remove(13, 1);

            if (!long.TryParse(data, out result) || data.Length != 13 || result < 0)
                throw new FormatException(Res.Get("Messages,InvalidBarcode2"));

            if (prefix == "")
                prefix = "(01)";

            this.text = prefix + CheckSumModulo10(data);
            return result + 10000000000000;
        }

        private int[] ChecksumWeight =
        {
            1, 3, 9, 27, 2, 6, 18, 54,
            4, 12, 36, 29, 8, 24, 72, 58,
            16, 48, 65, 37, 32, 17, 51, 74,
            64, 34, 23, 69, 49, 68, 46, 59
        };

        private int[] FinderPattern =
        {
            3, 8, 2, 1, 1,
            3, 5, 5, 1, 1,
            3, 3, 7, 1, 1,
            3, 1, 9, 1, 1,
            2, 7, 4, 1, 1,
            2, 5, 6, 1, 1,
            2, 3, 8, 1, 1,
            1, 5, 7, 1, 1,
            1, 3, 9, 1, 1
        };

        private int[] ModulesOdd = { 12, 10, 8, 6, 4, 5, 7, 9, 11 };
        private int[] ModulesEven = { 4, 6, 8, 10, 12, 10, 8, 6, 4 };
        private int[] WidthsOdd = { 8, 6, 4, 3, 1, 2, 4, 6, 8 };
        private int[] WidthsEven = { 1, 3, 5, 6, 8, 7, 5, 3, 1 };
        private int[] GSums = { 0, 161, 961, 2015, 2715, 0, 336, 1036, 1516 };
        private int[] TList = { 1, 10, 34, 70, 126, 4, 20, 48, 81 };

        internal override string GetPattern()
        {
            EncodedData = new List<string>();
            int[] dataGroup = new int[4];
            int[] v_odd = new int[4];
            int[] v_even = new int[4];
            long value = GetValue(text);
            long left = value / 4537077;
            long right = value % 4537077;
            int data1 = (int)(left / 1597);
            int data2 = (int)(left % 1597);
            int data3 = (int)(right / 1597);
            int data4 = (int)(right % 1597);


            if ((data1 >= 0) && (data1 <= 160)) { dataGroup[0] = 0; }
            if ((data1 >= 161) && (data1 <= 960)) { dataGroup[0] = 1; }
            if ((data1 >= 961) && (data1 <= 2014)) { dataGroup[0] = 2; }
            if ((data1 >= 2015) && (data1 <= 2714)) { dataGroup[0] = 3; }
            if ((data1 >= 2715) && (data1 <= 2840)) { dataGroup[0] = 4; }
            if ((data2 >= 0) && (data2 <= 335)) { dataGroup[1] = 5; }
            if ((data2 >= 336) && (data2 <= 1035)) { dataGroup[1] = 6; }
            if ((data2 >= 1036) && (data2 <= 1515)) { dataGroup[1] = 7; }
            if ((data2 >= 1516) && (data2 <= 1596)) { dataGroup[1] = 8; }
            if ((data4 >= 0) && (data4 <= 335)) { dataGroup[3] = 5; }
            if ((data4 >= 336) && (data4 <= 1035)) { dataGroup[3] = 6; }
            if ((data4 >= 1036) && (data4 <= 1515)) { dataGroup[3] = 7; }
            if ((data4 >= 1516) && (data4 <= 1596)) { dataGroup[3] = 8; }
            if ((data3 >= 0) && (data3 <= 160)) { dataGroup[2] = 0; }
            if ((data3 >= 161) && (data3 <= 960)) { dataGroup[2] = 1; }
            if ((data3 >= 961) && (data3 <= 2014)) { dataGroup[2] = 2; }
            if ((data3 >= 2015) && (data3 <= 2714)) { dataGroup[2] = 3; }
            if ((data3 >= 2715) && (data3 <= 2840)) { dataGroup[2] = 4; }

            v_odd[0] = (data1 - GSums[dataGroup[0]]) / TList[dataGroup[0]];
            v_even[0] = (data1 - GSums[dataGroup[0]]) % TList[dataGroup[0]];
            v_odd[1] = (data2 - GSums[dataGroup[1]]) % TList[dataGroup[1]];
            v_even[1] = (data2 - GSums[dataGroup[1]]) / TList[dataGroup[1]];
            v_odd[3] = (data4 - GSums[dataGroup[3]]) % TList[dataGroup[3]];
            v_even[3] = (data4 - GSums[dataGroup[3]]) / TList[dataGroup[3]];
            v_odd[2] = (data3 - GSums[dataGroup[2]]) / TList[dataGroup[2]];
            v_even[2] = (data3 - GSums[dataGroup[2]]) % TList[dataGroup[2]];

            int[,] data_widths = new int[8, 4];
            /* Use GS1 subset width algorithm */
            for (int i = 0; i < 4; i++)
            {
                if ((i == 0) || (i == 2))
                {
                    List<int> widths;
                    widths = GetGS1Widths(v_odd[i], ModulesOdd[dataGroup[i]], 4, WidthsOdd[dataGroup[i]], 1);
                    data_widths[0, i] = widths[0];
                    data_widths[2, i] = widths[1];
                    data_widths[4, i] = widths[2];
                    data_widths[6, i] = widths[3];
                    widths = GetGS1Widths(v_even[i], ModulesEven[dataGroup[i]], 4, WidthsEven[dataGroup[i]], 0);
                    data_widths[1, i] = widths[0];
                    data_widths[3, i] = widths[1];
                    data_widths[5, i] = widths[2];
                    data_widths[7, i] = widths[3];
                }
                else
                {
                    List<int> widths;
                    widths = GetGS1Widths(v_odd[i], ModulesOdd[dataGroup[i]], 4, WidthsOdd[dataGroup[i]], 0);
                    data_widths[0, i] = widths[0];
                    data_widths[2, i] = widths[1];
                    data_widths[4, i] = widths[2];
                    data_widths[6, i] = widths[3];
                    widths = GetGS1Widths(v_even[i], ModulesEven[dataGroup[i]], 4, WidthsEven[dataGroup[i]], 1);
                    data_widths[1, i] = widths[0];
                    data_widths[3, i] = widths[1];
                    data_widths[5, i] = widths[2];
                    data_widths[7, i] = widths[3];
                }
            }


            // Calculate the checksum
            int checksum = 0;
            for (int i = 0; i < 8; i++)
            {
                checksum += ChecksumWeight[i] * data_widths[i, 0];
                checksum += ChecksumWeight[i + 8] * data_widths[i, 1];
                checksum += ChecksumWeight[i + 16] * data_widths[i, 2];
                checksum += ChecksumWeight[i + 24] * data_widths[i, 3];
            }
            checksum %= 79;

            // Calculate the two check characters
            if (checksum >= 8) { checksum++; }
            if (checksum >= 72) { checksum++; }
            int c_left = checksum / 9;
            int c_right = checksum % 9;
            int[] barWeights = new int[46];

            // Put element widths together
            barWeights[0] = 1;
            barWeights[1] = 1;
            barWeights[44] = 1;
            barWeights[45] = 1;
            for (int i = 0; i < 8; i++)
            {
                barWeights[i + 2] = data_widths[i, 0];
                barWeights[i + 15] = data_widths[7 - i, 1];
                barWeights[i + 23] = data_widths[i, 3];
                barWeights[i + 36] = data_widths[7 - i, 2];
            }
            for (int i = 0; i < 5; i++)
            {
                barWeights[i + 10] = FinderPattern[i + (5 * c_left)];
                barWeights[i + 31] = FinderPattern[(4 - i) + (5 * c_right)];
            }

            EncodedData.Add("");
            foreach (int val in barWeights)
                EncodedData[0] += val;

            return EncodedData[0];
        }

        internal override float GetWidth(string code)
        {
            float width = 0;
            for (int x = 0; x < EncodedData[0].Length; x++)
                width += EncodedData[0][x] - '0';
            return width * WideBarRatio;
        }

        /// <inheritdoc />
        internal override void DoLines(string data, IGraphics g, float zoom)
        {
            DrawLineBars(EncodedData[0], g, zoom, new RectangleF(0, 0, 0, barArea.Height), false);
        }
    }

    /// <summary>
    /// Generates the GS1 DataBar Stacked barcode.
    /// </summary>
    public class BarcodeGS1Stacked : BarcodeGS1Omnidirectional
    {
        internal override string GetPattern()
        {
            string data = base.GetPattern();
            EncodedData = new List<string>();
            EncodedData.Add(data.Substring(0, 23) + "11");

            EncodedData.Add("0000"); // left padding of separate line
            EncodedData.Add("11" + data.Substring(23, 23));

            // convert line of strokes to black and white modules
            string[] bars = new string[2];
            for (int i = 0; i < EncodedData[0].Length; i++)
            {
                if (i % 2 == 0)
                {
                    for (int x = 0; x < EncodedData[0][i] - '0'; x++)
                        bars[0] += "0";
                    for (int x = 0; x < EncodedData[2][i] - '0'; x++)
                        bars[1] += "1";
                }
                else
                {
                    for (int x = 0; x < EncodedData[0][i] - '0'; x++)
                        bars[0] += "1";
                    for (int x = 0; x < EncodedData[2][i] - '0'; x++)
                        bars[1] += "0";
                }
            }

            // Encode separate line (applying encoding rules from sections 5.3.2.1)
            for (int i = 4; i < bars[0].Length - 4; i++)
            {
                if (bars[0][i] == '1' && bars[1][i] == '1')
                    EncodedData[1] += "0";
                else if (bars[0][i] == '0' && bars[1][i] == '0')
                    EncodedData[1] += "1";
                else if (bars[0][i] != bars[1][i])
                {
                    EncodedData[1] += EncodedData[1][EncodedData[1].Length - 1] == '0' ? "1" : "0";
                }
            }

            return "";
        }

        /// <inheritdoc />
        internal override void DoLines(string data, IGraphics g, float zoom)
        {
            DrawLineBars(EncodedData[0], g, zoom, new RectangleF(0, 0, 0, barArea.Height * 5 / 13), false);
            DrawLineBars(EncodedData[1], g, zoom, new RectangleF(0, barArea.Height * 5 / 13, 0, barArea.Height * 1 / 13), false, true);
            DrawLineBars(EncodedData[2], g, zoom, new RectangleF(0, barArea.Height * 6 / 13, 0, barArea.Height * 7 / 13), true);
        }

        internal override float GetWidth(string code)
        {
            float width = 0;
            for (int x = 0; x < EncodedData[0].Length; x++)
                width += EncodedData[0][x] - '0';
            return width * WideBarRatio;
        }
    }

    /// <summary>
    /// Generates the GS1 DataBar Stacked Omnidirectional barcode.
    /// </summary>
    public class BarcodeGS1StackedOmnidirectional : BarcodeGS1Omnidirectional
    {

        internal override string GetPattern()
        {
            string[] bars = new string[2];
            bool nextBarBlack = true;
            bool nextBarWhite = true;
            string data = base.GetPattern();
            EncodedData = new List<string>();
            EncodedData.Add(data.Substring(0, 23) + "11");
            EncodedData.Add("0000"); // left padding of top separate line
            EncodedData.Add("0000010101010101010101010101010101010101010101"); // left padding of middle separate line
            EncodedData.Add("0000"); // left padding of bottom separate line
            EncodedData.Add("11" + data.Substring(23, 23));

            // Encode separate lines (applying encoding rules from sections 5.3.2.2)
            for (int i = 0; i < EncodedData[0].Length; i++)
            {
                if (i % 2 == 0)
                {
                    for (int x = 0; x < EncodedData[0][i] - '0'; x++)
                    {
                        if (i > 5 && i < 9)
                        {
                            if (nextBarBlack)
                            {
                                bars[0] += "1";
                                nextBarBlack = false;
                            }
                            else
                            {
                                bars[0] += "0";
                                nextBarBlack = true;
                            }
                        }
                        else
                            bars[0] += "1";
                    }
                    for (int x = 0; x < EncodedData[4][i] - '0'; x++)
                    {
                        if (i > 15 && i < 19)
                        {
                            if (nextBarWhite)
                            {
                                bars[1] += "0";
                                nextBarWhite = false;
                            }
                            else
                            {
                                bars[1] += "1";
                                nextBarWhite = true;
                            }
                        }
                        else
                            bars[1] += "0";
                    }
                }
                else
                {
                    for (int x = 0; x < EncodedData[0][i] - '0'; x++)
                    {
                        bars[0] += "0";
                    }
                    for (int x = 0; x < EncodedData[4][i] - '0'; x++)
                    {
                        bars[1] += "1";
                    }
                }
            }

            EncodedData[1] += bars[0].Remove(0, 4).Remove(42, 4);
            EncodedData[3] += bars[1].Remove(0, 4).Remove(42, 4);

            return "";
        }

        internal override float GetWidth(string code)
        {
            float width = 0;
            for (int x = 0; x < EncodedData[0].Length; x++)
                width += EncodedData[0][x] - '0';
            return width * WideBarRatio;
        }

        /// <inheritdoc />
        internal override void DoLines(string data, IGraphics g, float zoom)
        {
            DrawLineBars(EncodedData[0], g, zoom, new RectangleF(0, 0, 0, barArea.Height * 33 / 69), false);
            DrawLineBars(EncodedData[1], g, zoom, new RectangleF(0, barArea.Height * 33 / 69, 0, barArea.Height * 1 / 69), false, true);
            DrawLineBars(EncodedData[2], g, zoom, new RectangleF(0, barArea.Height * 34 / 69, 0, barArea.Height * 1 / 69), false, true);
            DrawLineBars(EncodedData[3], g, zoom, new RectangleF(0, barArea.Height * 35 / 69, 0, barArea.Height * 1 / 69), false, true);
            DrawLineBars(EncodedData[4], g, zoom, new RectangleF(0, barArea.Height * 36 / 69, 0, barArea.Height * 33 / 69), true);
        }
    }

    /// <summary>
    /// Generates the GS1 DataBar Limited barcode.
    /// </summary>
    public class BarcodeGS1Limited : BarcodeGS1Base
    {
        /// <summary>
        /// Get value for encoding.
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns></returns>
        protected long GetValue(string data)
        {
            long result;
            string prefix = "";
            int startPrefix = data.IndexOf('(');
            int endPrefix = data.IndexOf(')');
            if (startPrefix >= 0 && endPrefix > 0)
            {
                prefix = data.Substring(startPrefix, endPrefix + 1);
                data = data.Replace(prefix, "");
            }

            if (data.Length > 13)
                data = data.Remove(13, 1);

            if (!long.TryParse(data, out result) || data.Length != 13 || result > 1999999999999 || result < 0)
                throw new FormatException(Res.Get("Messages,InvalidBarcode2"));

            if (prefix == "")
                prefix = "(01)";

            this.text = prefix + CheckSumModulo10(data);
            return result;
        }

        int[] ChecksumWeight =
            {
                1, 3, 9, 27, 81, 65, 17, 51, 64, 14, 42, 37, 22, 66,
                20, 60, 2, 6, 18, 54, 73, 41, 34, 13, 39, 28, 84, 74
            };

        int[] FinderPattern =
            {
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 2, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 3, 2, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 3, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 3, 1, 1, 1,
                1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 3, 2, 1, 1,
                1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 3, 1, 1, 1,
                1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 3, 1, 1, 1,
                1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 3, 1, 1, 1,
                1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 3, 2, 1, 1,
                1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 3, 1, 1, 1,
                1, 1, 1, 2, 1, 1, 1, 2, 1, 1, 3, 1, 1, 1,
                1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 3, 1, 1, 1,
                1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1,
                1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 3, 2, 1, 1,
                1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 3, 1, 1, 1,
                1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 3, 1, 1, 1,
                1, 2, 1, 1, 1, 2, 1, 1, 1, 1, 3, 1, 1, 1,
                1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1,
                1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 3, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 2, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 3, 2, 1, 2, 1, 1, 1,
                1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 2, 2, 1, 1,
                1, 1, 1, 1, 1, 2, 1, 1, 2, 2, 2, 1, 1, 1,
                1, 1, 1, 1, 1, 2, 1, 2, 2, 1, 2, 1, 1, 1,
                1, 1, 1, 1, 1, 3, 1, 1, 2, 1, 2, 1, 1, 1,
                1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 2, 2, 1, 1,
                1, 1, 1, 2, 1, 1, 1, 1, 2, 2, 2, 1, 1, 1,
                1, 1, 1, 2, 1, 1, 1, 2, 2, 1, 2, 1, 1, 1,
                1, 1, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 1, 1,
                1, 1, 1, 3, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1,
                1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 2, 2, 1, 1,
                1, 2, 1, 1, 1, 1, 1, 1, 2, 2, 2, 1, 1, 1,
                1, 2, 1, 1, 1, 1, 1, 2, 2, 1, 2, 1, 1, 1,
                1, 2, 1, 1, 1, 2, 1, 1, 2, 1, 2, 1, 1, 1,
                1, 2, 1, 2, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1,
                1, 3, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 3, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 3, 2, 1, 2, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 2, 3, 1, 1, 2, 1, 1,
                1, 1, 1, 2, 1, 1, 1, 1, 3, 1, 1, 2, 1, 1,
                1, 2, 1, 1, 1, 1, 1, 1, 3, 1, 1, 2, 1, 1,
                1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 2, 3, 1, 1,
                1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 2, 2, 1, 1,
                1, 1, 1, 1, 1, 1, 2, 1, 1, 3, 2, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 2, 2, 1, 1,
                1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 2, 2, 1, 1,
                1, 1, 1, 2, 1, 1, 2, 1, 1, 2, 2, 1, 1, 1,
                1, 1, 1, 2, 1, 1, 2, 2, 1, 1, 2, 1, 1, 1,
                1, 1, 1, 2, 1, 2, 2, 1, 1, 1, 2, 1, 1, 1,
                1, 1, 1, 3, 1, 1, 2, 1, 1, 1, 2, 1, 1, 1,
                1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 2, 2, 1, 1,
                1, 2, 1, 1, 1, 1, 2, 1, 1, 2, 2, 1, 1, 1,
                1, 2, 1, 2, 1, 1, 2, 1, 1, 1, 2, 1, 1, 1,
                1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 3, 1, 1,
                1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 2, 2, 1, 1,
                1, 1, 1, 1, 2, 1, 1, 1, 1, 3, 2, 1, 1, 1,
                1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 2, 2, 1, 1,
                1, 1, 1, 1, 2, 1, 1, 2, 1, 2, 2, 1, 1, 1,
                1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1,
                1, 2, 1, 1, 2, 1, 1, 1, 1, 1, 2, 2, 1, 1,
                1, 2, 1, 1, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1,
                1, 2, 1, 1, 2, 1, 1, 2, 1, 1, 2, 1, 1, 1,
                1, 2, 1, 1, 2, 2, 1, 1, 1, 1, 2, 1, 1, 1,
                1, 2, 1, 2, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1,
                1, 3, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1,
                1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 3, 1, 1,
                1, 1, 2, 1, 1, 1, 1, 1, 1, 2, 2, 2, 1, 1,
                1, 1, 2, 1, 1, 1, 1, 1, 1, 3, 2, 1, 1, 1,
                1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 2, 2, 1, 1,
                1, 1, 2, 1, 1, 1, 1, 2, 1, 2, 2, 1, 1, 1,
                1, 1, 2, 1, 1, 1, 1, 3, 1, 1, 2, 1, 1, 1,
                1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 2, 2, 1, 1,
                1, 1, 2, 1, 1, 2, 1, 1, 1, 2, 2, 1, 1, 1,
                1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1,
                2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 1, 1,
                2, 1, 1, 1, 1, 1, 1, 1, 1, 3, 2, 1, 1, 1,
                2, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 2, 1, 1,
                2, 1, 1, 1, 1, 1, 1, 2, 1, 2, 2, 1, 1, 1,
                2, 1, 1, 1, 1, 1, 1, 3, 1, 1, 2, 1, 1, 1,
                2, 1, 1, 1, 1, 2, 1, 1, 1, 2, 2, 1, 1, 1,
                2, 1, 1, 1, 1, 2, 1, 2, 1, 1, 2, 1, 1, 1,
                2, 1, 1, 2, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1
            };

        int[] ModulesOdd = { 17, 13, 9, 15, 11, 19, 7 };
        int[] ModulesEven = { 9, 13, 17, 11, 15, 7, 19 };
        int[] WidthsOdd = { 6, 5, 3, 5, 4, 8, 1 };
        int[] WidthsEven = { 3, 4, 6, 4, 5, 1, 8 };
        int[] TEven = { 28, 728, 6454, 203, 2408, 1, 16632 };

        internal override string GetPattern()
        {
            EncodedData = new List<string>();
            long value = GetValue(text);
            long left = value / 2013571;
            long right = value % 2013571;

            int leftGroup = 0;
            if (left > 183063) { leftGroup = 1; }
            if (left > 820063) { leftGroup = 2; }
            if (left > 1000775) { leftGroup = 3; }
            if (left > 1491020) { leftGroup = 4; }
            if (left > 1979844) { leftGroup = 5; }
            if (left > 1996938) { leftGroup = 6; }

            int rightGroup = 0;
            if (right > 183063) { rightGroup = 1; }
            if (right > 820063) { rightGroup = 2; }
            if (right > 1000775) { rightGroup = 3; }
            if (right > 1491020) { rightGroup = 4; }
            if (right > 1979844) { rightGroup = 5; }
            if (right > 1996938) { rightGroup = 6; }
            switch (leftGroup)
            {
                case 1:
                    left -= 183064;
                    break;
                case 2:
                    left -= 820064;
                    break;
                case 3:
                    left -= 1000776;
                    break;
                case 4:
                    left -= 1491021;
                    break;
                case 5:
                    left -= 1979845;
                    break;
                case 6:
                    left -= 1996939;
                    break;
            }

            switch (rightGroup)
            {
                case 1:
                    right -= 183064;
                    break;
                case 2:
                    right -= 820064;
                    break;
                case 3:
                    right -= 1000776;
                    break;
                case 4:
                    right -= 1491021;
                    break;
                case 5:
                    right -= 1979845;
                    break;
                case 6:
                    right -= 1996939;
                    break;
            }

            int leftOdd = (int)(left / TEven[leftGroup]);
            int leftEven = (int)(left % TEven[leftGroup]);
            int rightOdd = (int)(right / TEven[rightGroup]);
            int rightEven = (int)(right % TEven[rightGroup]);

            List<int> widths;
            int[] leftWidths = new int[14];
            int[] rightWidths = new int[14];
            widths = GetGS1Widths(leftOdd, ModulesOdd[leftGroup], 7, WidthsOdd[leftGroup], 1);
            leftWidths[0] = widths[0];
            leftWidths[2] = widths[1];
            leftWidths[4] = widths[2];
            leftWidths[6] = widths[3];
            leftWidths[8] = widths[4];
            leftWidths[10] = widths[5];
            leftWidths[12] = widths[6];
            widths = GetGS1Widths(leftEven, ModulesEven[leftGroup], 7, WidthsEven[leftGroup], 0);
            leftWidths[1] = widths[0];
            leftWidths[3] = widths[1];
            leftWidths[5] = widths[2];
            leftWidths[7] = widths[3];
            leftWidths[9] = widths[4];
            leftWidths[11] = widths[5];
            leftWidths[13] = widths[6];
            widths = GetGS1Widths(rightOdd, ModulesOdd[rightGroup], 7, WidthsOdd[rightGroup], 1);
            rightWidths[0] = widths[0];
            rightWidths[2] = widths[1];
            rightWidths[4] = widths[2];
            rightWidths[6] = widths[3];
            rightWidths[8] = widths[4];
            rightWidths[10] = widths[5];
            rightWidths[12] = widths[6];
            widths = GetGS1Widths(rightEven, ModulesEven[rightGroup], 7, WidthsEven[rightGroup], 0);
            rightWidths[1] = widths[0];
            rightWidths[3] = widths[1];
            rightWidths[5] = widths[2];
            rightWidths[7] = widths[3];
            rightWidths[9] = widths[4];
            rightWidths[11] = widths[5];
            rightWidths[13] = widths[6];

            int checksum = 0;
            /* Calculate the checksum */
            for (int i = 0; i < 14; i++)
            {
                checksum += ChecksumWeight[i] * leftWidths[i];
                checksum += ChecksumWeight[i + 14] * rightWidths[i];
            }
            checksum %= 89;

            int[] checkElements = new int[14];
            for (int i = 0; i < 14; i++)
            {
                checkElements[i] = FinderPattern[i + (checksum * 14)];
            }

            int[] totalWidths = new int[46];
            totalWidths[0] = 1;
            totalWidths[1] = 1;
            totalWidths[44] = 1;
            totalWidths[45] = 1;
            for (int i = 0; i < 14; i++)
            {
                totalWidths[i + 2] = leftWidths[i];
                totalWidths[i + 16] = checkElements[i];
                totalWidths[i + 30] = rightWidths[i];
            }

            EncodedData.Add("");
            foreach (int val in totalWidths)
                EncodedData[0] += val;

            return EncodedData[0];
        }

        /// <inheritdoc />
        internal override void DoLines(string data, IGraphics g, float zoom)
        {
            DrawLineBars(EncodedData[0], g, zoom, new RectangleF(0, 0, 0, barArea.Height), false);
        }

        internal override float GetWidth(string code)
        {
            float width = 0;
            for (int x = 0; x < EncodedData[0].Length; x++)
                width += EncodedData[0][x] - '0';
            return width * WideBarRatio;
        }
    }
}

