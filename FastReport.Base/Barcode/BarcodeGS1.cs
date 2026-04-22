using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

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
    /// Base class for GS1 DataBar Expanded barcode encoding.
    /// Converts GS1 data into a binary stream, calculates checksum,
    /// builds barcode patterns, and renders stacked symbol rows.
    /// </summary>
    public class BarcodeGS1ExpandedBase : BarcodeGS1Omnidirectional
    {
        /// <summary>
        /// Number of barcode segments.
        /// </summary>
        protected int _segments = 4;

        protected Collection<SymbolData> Symbol = new Collection<SymbolData>();

        const int NumMode = 1;
        const int AlnuMode = 2;
        const int IsoMode = 3;

        const int IsNum = 0x1;
        const int IsFnc1 = 0x2;
        const int IsAlnu = 0x4;

        static byte[] LookUp = { 
		        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
		        8,8,8,0,0,8,8,8,8,8,0xc,8,0xc,0xc,0xc,0xc,
                0xd,0xd,0xd,0xd,0xd,0xd,0xd,0xd,0xd,0xd,
                8,8,8,8,8,8,
		        0,
                0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,
                0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,0xc,
                0xf,0,0,0xc,8,
		        0,
                8,8,8,8,8,8,8,8,8,8,8,8,8,
                8,8,8,8,8,8,8,8,8,8,8,8,8,
                0,0,0,0,0,
		        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 };

        static int InputLength;
        static int Position;
        static int CurrentMode;
        static char[] InputData;
        static BitVectorGS1 BinaryData;
        static int[] GroupSumExpanded = { 0, 348, 1388, 2948, 3988 };
        static int[] EvenExpanded = { 4, 20, 52, 104, 204 };
        static int[] OddModulesExpanded = { 12, 10, 8, 6, 4 };
        static int[] EvenModulesExpanded = { 5, 7, 9, 11, 13 };
        static int[] WidestOddExpanded = { 7, 5, 4, 3, 1 };
        static int[] WidestEvenExpanded = { 2, 4, 5, 6, 8 };
        static int[] ChecksumWeightExpanded = {
            // Table 14.
	        1, 3, 9, 27, 81, 32, 96, 77,
            20, 60, 180, 118, 143, 7, 21, 63,
            189, 145, 13, 39, 117, 140, 209, 205,
            193, 157, 49, 147, 19, 57, 171, 91,
            62, 186, 136, 197, 169, 85, 44, 132,
            185, 133, 188, 142, 4, 12, 36, 108,
            113, 128, 173, 97, 80, 29, 87, 50,
            150, 28, 84, 41, 123, 158, 52, 156,
            46, 138, 203, 187, 139, 206, 196, 166,
            76, 17, 51, 153, 37, 111, 122, 155,
            43, 129, 176, 106, 107, 110, 119, 146,
            16, 48, 144, 10, 30, 90, 59, 177,
            109, 116, 137, 200, 178, 112, 125, 164,
            70, 210, 208, 202, 184, 130, 179, 115,
            134, 191, 151, 31, 93, 68, 204, 190,
            148, 22, 66, 198, 172, 94, 71, 2,
            6, 18, 54, 162, 64, 192, 154, 40,
            120, 149, 25, 75, 14, 42, 126, 167,
            79, 26, 78, 23, 69, 207, 199, 175,
            103, 98, 83, 38, 114, 131, 182, 124,
            161, 61, 183, 127, 170, 88, 53, 159,
            55, 165, 73, 8, 24, 72, 5, 15,
            45, 135, 194, 160, 58, 174, 100, 89};

        static int[] FinderPatternExpanded = {
            // Table 15.
	        1, 8, 4, 1, 1,
            1, 1, 4, 8, 1,
            3, 6, 4, 1, 1,
            1, 1, 4, 6, 3,
            3, 4, 6, 1, 1,
            1, 1, 6, 4, 3,
            3, 2, 8, 1, 1,
            1, 1, 8, 2, 3,
            2, 6, 5, 1, 1,
            1, 1, 5, 6, 2,
            2, 2, 9, 1, 1,
            1, 1, 9, 2, 2};

        static int[] FinderSequence = {
            // Table 16.
	        1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 4, 3, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 6, 3, 8, 0, 0, 0, 0, 0, 0, 0,
            1, 10, 3, 8, 5, 0, 0, 0, 0, 0, 0,
            1, 10, 3, 8, 7, 12, 0, 0, 0, 0, 0,
            1, 10, 3, 8, 9, 12, 11, 0, 0, 0, 0,
            1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0,
            1, 2, 3, 4, 5, 6, 7, 10, 9, 0, 0,
            1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 0,
            1, 2, 3, 4, 5, 8, 7, 10, 9, 12, 11};

        static int[] RowWeights = {
            0, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 5, 6, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 9, 10, 3, 4, 13, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 17, 18, 3, 4, 13, 14, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 17, 18, 3, 4, 13, 14, 11, 12, 21, 22, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 17, 18, 3, 4, 13, 14, 15, 16, 21, 22, 19, 20, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0, 0, 0, 0, 0, 0,
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 17, 18, 15, 16, 0, 0, 0, 0,
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 17, 18, 19, 20, 21, 22, 0, 0,
            0, 1, 2, 3, 4, 5, 6, 7, 8, 13, 14, 11, 12, 17, 18, 15, 16, 21, 22, 19, 20};

        /// <summary>
        /// Encodes date (YYMMDD) into 16 bits.
        /// </summary>
        static void ConvertDate()
        {
            int value = (((InputData[Position] - '0') * 10) + (InputData[Position + 1] - '0')) * 384;       // YY
            value += (((InputData[Position + 2] - '0') * 10) + ((InputData[Position + 3] - '0') - 1)) * 32; // MM
            value += (((InputData[Position + 4] - '0') * 10) + (InputData[Position + 5] - '0'));            // DD
            BinaryData.AppendBits(value, 16);
            Position += 6;
        }

        /// <summary>
        /// Encodes 13 digits into bits.
        /// </summary>
        static void Convert13()
        {
            int value = InputData[Position] - '0';
            BinaryData.AppendBits(value, 4);
            Position++;
            Convert12();
        }

        /// <summary>
        /// Encodes 12 digits into 40 bits.
        /// </summary>
        static void Convert12()
        {
            for (int i = 0; i < 4; i++)
            {
                int value = (((InputData[Position] - '0') * 100) + ((InputData[Position + 1] - '0') * 10) + InputData[Position + 2] - '0');
                BinaryData.AppendBits(value, 10);
                Position += 3;
            }
        }

        /// <summary>
        /// Pads binary stream to required size.
        /// </summary>
        /// <param name="targetBitSize">Target bit length</param>
        static void AddPadding(int targetBitSize)
        {
            if (CurrentMode == NumMode)
                BinaryData.AppendBits(0, 4);   

            while (BinaryData.SizeInBits < targetBitSize)
                BinaryData.AppendBits(0x04, 5);

            if (BinaryData.SizeInBits > targetBitSize)
                BinaryData.RemoveBits(targetBitSize, BinaryData.SizeInBits - targetBitSize);
        }

        /// <summary>
        /// Encodes GS1 DataBar Expanded data into a binary bit stream,
        /// applying GS1 rules and aligning data based on segment count.
        /// </summary>
        /// <param name="barcodeData">GS1 input data</param>
        /// <param name="isCompositeSymbol">Composite barcode flag</param>
        /// <param name="segments">Number of stacked segments</param>
        /// <returns>Encoded bit stream</returns>
        /// <exception cref="FormatException">If data is too large</exception>
        internal static BitVectorGS1 DatabarExpBitStream(char[] barcodeData, bool isCompositeSymbol, int segments)
        {
            BinaryData = new BitVectorGS1();
            InputLength = barcodeData.Length;
            InputData = new char[InputLength];
            Array.Copy(barcodeData, InputData, InputLength);

            Position = 0;
            int placeHolder = 0;

            int weight = 0, countryCode = 0;
            string ai = new string(InputData, 0, Math.Min(2, InputLength));

            BinaryData.AppendBit(isCompositeSymbol ? (byte)1 : (byte)0);

            if (InputLength >= 26)
                int.TryParse(new string(InputData, 20, 6), out weight);

            if (InputLength >= 16 && ai == "01")
            {
                if (InputData[2] == '9')
                {
                    bool isWeightBlock = InputLength == 26 && InputData[16] == '3' && InputData[18] == '0';

                    if (isWeightBlock && InputData[17] == '1' && InputData[19] == '3' && weight <= 32767)
                    {
                        BinaryData.AppendBits(0x04, 4);
                        Position += 3; Convert12();
                        BinaryData.AppendBits(weight, 15);
                        Position += 11;
                    }
                    else if (isWeightBlock && InputData[17] == '2' && InputData[19] == '2' && weight <= 9999)
                    {
                        BinaryData.AppendBits(0x05, 4);
                        Position += 3; Convert12();
                        BinaryData.AppendBits(weight, 15);
                        Position += 11;
                    }
                    else if (isWeightBlock && InputData[17] == '2' && InputData[19] == '3' && weight <= 22767)
                    {
                        BinaryData.AppendBits(0x05, 4);
                        Position += 3; Convert12();
                        BinaryData.AppendBits(weight + 10000, 15);
                        Position += 11;
                    }
                    else if (InputLength >= 21 && InputData[16] == '3' && InputData[17] == '9' && InputData[18] == '2'
                             && InputData[19] <= '3')
                    {
                        BinaryData.AppendBits(0x0c << 2, 7);
                        Position += 3; Convert12();
                        BinaryData.AppendBits(InputData[19] - '0', 2);
                        Position += 5;
                        placeHolder = 6;
                    }
                    else if (InputLength >= 24 && InputData[16] == '3' && InputData[17] == '9' && InputData[18] == '3'
                             && InputData[19] <= '3')
                    {
                        BinaryData.AppendBits(0x0d << 2, 7);
                        Position += 3; Convert12();
                        BinaryData.AppendBits(InputData[19] - '0', 2);
                        Position += 5;

                        int.TryParse(new string(InputData, 20, 3), out countryCode);
                        BinaryData.AppendBits(countryCode, 10);
                        Position += 3;

                        placeHolder = 6;
                    }
                    else if (isWeightBlock && (InputData[17] == '1' || InputData[17] == '2') && weight <= 99999)
                    {
                        int value = 0x38 + (InputData[17] - '1');
                        BinaryData.AppendBits(value, 7);
                        Position += 3; Convert12();

                        weight += (InputData[19] - '0') * 100000;
                        BinaryData.AppendBits(weight >> 16, 4);
                        BinaryData.AppendBits(weight & 0xffff, 16);
                        Position += 11;

                        if (InputLength == 34 && InputData[26] == '1' &&
                            (InputData[27] == '1' || InputData[27] == '3' || InputData[27] == '5' || InputData[27] == '7'))
                        {
                            Position += 2;
                            ConvertDate();
                        }
                        else
                            BinaryData.AppendBits(38400, 16);
                    }
                }
                else
                {
                    BinaryData.AppendBits(1 << 2, 3);
                    Position += 2;
                    Convert13();
                    Position++;
                    placeHolder = 2;
                }
            }
            else
            {
                BinaryData.AppendBits(0, 4);
                placeHolder = 3;
            }

            CurrentMode = NumMode;

            while (Position < InputLength)
            {
                if (CurrentMode == NumMode) ProcessNUMMode();
                else if (CurrentMode == AlnuMode) ProcessALNUMode();
                else ProcessISOMode();
            }

            int remainder = 12 - (BinaryData.SizeInBits % 12);
            if (remainder == 12) remainder = 0;
            if (BinaryData.SizeInBits < 36) remainder = 36 - BinaryData.SizeInBits;
            if (BinaryData.SizeInBits > 252)
                return null; 

            int dataChars = (BinaryData.SizeInBits / 12) + 1;
            if (remainder > 0) dataChars++;
            if (dataChars % segments == 1) remainder += 12;

            if (remainder > 0)
                AddPadding(BinaryData.SizeInBits + remainder);

            if (placeHolder > 0)
            {
                BinaryData[placeHolder] = (byte)(((BinaryData.SizeInBits / 12) + 1) & 1);
                BinaryData[placeHolder + 1] = (byte)(BinaryData.SizeInBits > 156 ? 1 : 0);
            }

            return BinaryData;
        }

        //<summary>
        //Encodes the Databar Expanded symbol.
        //</summary>
        public void DatabarExpanded(char[] barcodeData)
        {
            BitVectorGS1 binaryData = DatabarExpBitStream(barcodeData, false, _segments);

            if (binaryData == null)
            {
                // Invalid GS1 data – cannot build barcode
                Symbol?.Clear();
                return;
            }

            int dataCharacters = binaryData.SizeInBits / 12;

            int[] values = new int[dataCharacters];
            int[] groups = new int[dataCharacters];
            int[,] charWidths = new int[dataCharacters, 8];

            for (int i = 0; i < dataCharacters; i++)
            {
                int value = 0, mask = 0x800;

                for (int j = 0; j < 12; j++, mask >>= 1)
                    if (binaryData[i * 12 + j] == 1)
                        value += mask;

                values[i] = value;

                int group =
                    value <= 347 ? 1 :
                    value <= 1387 ? 2 :
                    value <= 2947 ? 3 :
                    value <= 3987 ? 4 : 5;

                groups[i] = group;

                int baseValue = value - GroupSumExpanded[group - 1];
                int odd = baseValue / EvenExpanded[group - 1];
                int even = baseValue % EvenExpanded[group - 1];

                var oddWidths = GetGS1Widths(odd, OddModulesExpanded[group - 1], 4, WidestOddExpanded[group - 1], 0);
                var evenWidths = GetGS1Widths(even, EvenModulesExpanded[group - 1], 4, WidestEvenExpanded[group - 1], 1);

                for (int j = 0; j < 4; j++)
                {
                    charWidths[i, j * 2] = oddWidths[j];
                    charWidths[i, j * 2 + 1] = evenWidths[j];
                }
            }

            int checksum = 0;
            for (int i = 0; i < dataCharacters; i++)
            {
                int row = RowWeights[((dataCharacters - 2) / 2) * 21 + i];
                for (int j = 0; j < 8; j++)
                    checksum += charWidths[i, j] * ChecksumWeightExpanded[row * 8 + j];
            }

            int checkCharacter = 211 * (dataCharacters - 3) + (checksum % 211);

            int checkGroup =
                checkCharacter <= 347 ? 1 :
                checkCharacter <= 1387 ? 2 :
                checkCharacter <= 2947 ? 3 :
                checkCharacter <= 3987 ? 4 : 5;

            int checkBase = checkCharacter - GroupSumExpanded[checkGroup - 1];
            int checkOdd = checkBase / EvenExpanded[checkGroup - 1];
            int checkEven = checkBase % EvenExpanded[checkGroup - 1];

            int[] checkWidths = new int[8];
            var oddCheck = GetGS1Widths(checkOdd, OddModulesExpanded[checkGroup - 1], 4, WidestOddExpanded[checkGroup - 1], 0);
            var evenCheck = GetGS1Widths(checkEven, EvenModulesExpanded[checkGroup - 1], 4, WidestEvenExpanded[checkGroup - 1], 1);

            for (int i = 0; i < 4; i++)
            {
                checkWidths[i * 2] = oddCheck[i];
                checkWidths[i * 2 + 1] = evenCheck[i];
            }

            int pairCount = (dataCharacters + 2) / 2;
            int patternWidth = pairCount * 5 + (dataCharacters + 1) * 8 + 4;
            int[] elements = new int[patternWidth];

            for (int i = 0; i < pairCount; i++)
            {
                int index = ((((dataCharacters - 1) / 2) + ((dataCharacters + 1) & 1) - 1) * 11) + i;
                for (int j = 0; j < 5; j++)
                    elements[21 * i + 10 + j] = FinderPatternExpanded[(FinderSequence[index] - 1) * 5 + j];
            }

            for (int i = 0; i < 8; i++)
                elements[i + 2] = checkWidths[i];

            for (int i = 1; i < dataCharacters; i += 2)
                for (int j = 0; j < 8; j++)
                    elements[((i - 1) / 2) * 21 + 23 + j] = charWidths[i, j];

            for (int i = 0; i < dataCharacters; i += 2)
                for (int j = 0; j < 8; j++)
                    elements[(i / 2) * 21 + 15 + j] = charWidths[i, 7 - j];

            BuildExpandedStackedSymbol(elements, dataCharacters, patternWidth);
        }

        /// <summary>
        /// Builds GS1 DataBar Expanded Stacked barcode rows from encoded pattern data.
        /// </summary>
        /// <remarks>
        /// Splits data into rows, arranges blocks, and adds separator patterns between them.
        /// </remarks>
        /// <param name="elements">Encoded barcode pattern (module widths)</param>
        /// <param name="dataCharacters">Number of encoded data characters</param>
        /// <param name="patternWidth">Total width of the pattern</param>
        private void BuildExpandedStackedSymbol(int[] elements, int dataCharacters, int patternWidth)
        {
            Symbol.Clear();
            SymbolData symbolData;
            byte[] rowData;
            int[] subElements;
            int row;
            int currentRow;
            int position;
            int symbolWidth;
            bool isSpecialRow;
            bool isLeftToRight;
            bool latch = false;
            int column = 0;
            int columnsPerRow = _segments / 2;
            int maxWidth = 0;

            int codeBlocks = ((dataCharacters + 1) / 2) + ((dataCharacters + 1) % 2);
            if (columnsPerRow > codeBlocks) // User supplied segments is large than needed to encode symbol.
                columnsPerRow = codeBlocks;

            int stackedRows = (codeBlocks / columnsPerRow) + (codeBlocks % columnsPerRow > 0 ? 1 : 0);
            int subElementCount = 0;
            int currentBlock = 0;

            for (currentRow = 1; currentRow <= stackedRows; currentRow++)
            {
                isSpecialRow = false;
                subElements = new int[500];

                // Row start.
                subElements[0] = 1;
                subElements[1] = 1;
                subElementCount = 2;

                // Row Data.
                column = 0;
                do
                {
                    if ((((columnsPerRow & 1) != 0) || (currentRow & 1) != 0) ||
                        (currentRow == stackedRows && codeBlocks != (currentRow * columnsPerRow) &&
                        ((((currentRow * columnsPerRow) - codeBlocks) & 1) != 0)))
                    {
                        // Left to right.
                        isLeftToRight = true;
                        int i = 2 + (currentBlock * 21);
                        for (int j = 0; j < 21; j++)
                        {
                            if ((i + j) < patternWidth)
                                subElements[j + (column * 21) + 2] = elements[i + j];

                            subElementCount++;
                        }
                    }

                    else
                    {
                        // Right to left.
                        isLeftToRight = false;
                        int i = 2 + (((currentRow * columnsPerRow) - column - 1) * 21);
                        for (int j = 0; j < 21; j++)
                        {
                            if ((i + j) < patternWidth)
                                subElements[(20 - j) + (column * 21) + 2] = elements[i + j];

                            subElementCount++;
                        }
                    }

                    column++;
                    currentBlock++;
                }
                while (column < columnsPerRow && currentBlock < codeBlocks);

                // Row stop.
                subElements[subElementCount] = 1;
                subElements[subElementCount + 1] = 1;
                subElementCount += 2;

                latch = !((currentRow & 1) != 0);
                if (currentRow == stackedRows && codeBlocks != (currentRow * columnsPerRow) &&
                    ((((currentRow * columnsPerRow) - codeBlocks) & 1) != 0) && ((columnsPerRow & 1) == 0))
                {
                    // Special case bottom row.
                    isSpecialRow = true;
                    subElements[0] = 2;
                    latch = false;
                }

                symbolWidth = 0;
                position = 0;
                for (int i = 0; i < subElementCount; i++)
                    symbolWidth += subElements[i];

                rowData = new byte[symbolWidth];
                for (int i = 0; i < subElementCount; i++)
                {
                    for (int j = 0; j < subElements[i]; j++)
                    {
                        if (latch)
                            rowData[position] = 1;

                        position++;
                    }

                    latch = !latch;
                }

                symbolData = new SymbolData(rowData, 10.0f);    
                Symbol.Add(symbolData);

                if (currentRow != 1)
                {
                    // Middle separator pattern (above current row).
                    int length = 49 * columnsPerRow;
                    rowData = new byte[length + 1];
                    row = Symbol.Count - 1;

                    for (int j = 5; j < length; j += 2)
                        rowData[j] = 1;

                    symbolData = new SymbolData(rowData, 1.0f);
                    Symbol.Insert(row, symbolData);

                    // Bottom separator pattern (above current row).
                    rowData = new byte[symbolWidth];
                    row = Symbol.Count - 1;

                    for (int j = 4; j < (symbolWidth - 4); j++)
                    {
                        if (Symbol[row].GetRowData()[j] == 0)
                            rowData[j] = 1;
                    }

                    FinderAdjustment(rowData, column, row, isSpecialRow, isLeftToRight);
                    symbolData = new SymbolData(rowData, 1.0f);
                    Symbol.Insert(row, symbolData);
                }

                if (currentRow != stackedRows)
                {
                    // Top separator pattern (below previous row).
                    rowData = new byte[symbolWidth];
                    row = Symbol.Count - 1;

                    for (int j = 4; j < (symbolWidth - 4); j++)
                    {
                        if (Symbol[row].GetRowData()[j] == 0)
                            rowData[j] = 1;
                    }

                    FinderAdjustment(rowData, column, row, isSpecialRow, isLeftToRight);
                    symbolData = new SymbolData(rowData, 1.0f);
                    Symbol.Add(symbolData);
                }
            }
        }

        /// <summary>
        /// Adjusts separator pattern to avoid conflicts with finder patterns.
        /// </summary>
        /// <param name="rowData">Separator row data</param>
        /// <param name="column">Number of columns</param>
        /// <param name="row">Current row index</param>
        /// <param name="isSpecialRow">Indicates special row handling</param>
        /// <param name="isLeftToRight">Direction of processing</param>
        private void FinderAdjustment(byte[] rowData, int column, int row, bool isSpecialRow, bool isLeftToRight)
        {
            for (int j = 0; j < column; j++)
            {
                int k = (49 * j) + ((isSpecialRow) ? 19 : 18);
                if (isLeftToRight)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        if (Symbol[row].GetRowData()[i + k - 1] == 0 && Symbol[row].GetRowData()[i + k] == 0 && rowData[i + k - 1] == 1)
                            rowData[i + k] = 0;
                    }
                }

                else
                {
                    for (int i = 14; i >= 0; i--)
                    {
                        if (Symbol[row].GetRowData()[i + k + 1] == 0 && Symbol[row].GetRowData()[i + k] == 0 && rowData[i + k + 1] == 1)
                            rowData[i + k] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Encodes numeric data into the binary stream.
        /// </summary>
        /// <remarks>
        /// Processes digits in pairs and switches mode if non-numeric data is encountered.
        /// </remarks>
        static void ProcessNUMMode()
        {
            char ch1 = InputData[Position];
            int type1 = LookUp[ch1];

            if ((type1 & IsNum) == 0)
            {
                BinaryData.AppendBits(0, 4);
                CurrentMode = AlnuMode;
                return;
            }

            if (Position + 1 == InputLength && (type1 & IsFnc1) == 0)
            {
                BinaryData.AppendBits(((ch1 - '0') * 11) + 18, 7);
                Position++;
                return;
            }

            char ch2 = InputData[Position + 1];
            int type2 = LookUp[ch2];

            if ((type2 & IsNum) == 0 || (type1 & type2 & IsFnc1) != 0)
            {
                BinaryData.AppendBits(0, 4);
                CurrentMode = AlnuMode;
                return;
            }

            int d1 = (type1 & IsFnc1) != 0 ? 10 : ch1 - '0';
            int d2 = (type2 & IsFnc1) != 0 ? 10 : ch2 - '0';

            BinaryData.AppendBits((d1 * 11 + d2 + 8), 7);
            Position += 2;
        }

        /// <summary>
        /// Encodes alphanumeric data into the binary stream.
        /// </summary>
        /// <remarks>
        /// Processes letters and digits, switches to numeric or ISO mode when needed.
        /// </remarks>
        static void ProcessALNUMode()
        {
            char ch = InputData[Position];
            int type = LookUp[ch];

            if ((type & IsAlnu) == 0)
            {
                BinaryData.AppendBits(0x04, 5);
                CurrentMode = IsoMode;
                return;
            }

            if (Position + 1 < InputLength && (type & IsNum) != 0 &&
                ((type | LookUp[InputData[Position + 1]]) & IsFnc1) == 0)
            {
                int count = 1;
                while (count < 6 && Position + count < InputLength &&
                       (LookUp[InputData[Position + count]] & IsNum) != 0)
                {
                    count++;
                }

                if ((Position + count == InputLength && count >= 4) || count == 6)
                {
                    BinaryData.AppendBits(0, 3);
                    CurrentMode = NumMode;
                    return;
                }
            }

            Position++;

            if ((type & IsNum) != 0)
            {
                int val = (type & IsFnc1) != 0
                    ? 0xF
                    : ch - '0' + 5;

                if ((type & IsFnc1) != 0)
                    CurrentMode = NumMode;

                BinaryData.AppendBits(val, 5);
                return;
            }

            int result =
                ch >= 'A' ? ch - 'A' :
                ch >= ',' ? ch - ',' + 0x1B :
                0x1A;

            BinaryData.AppendBits(result + 0x20, 6);
        }

        /// <summary>
        /// Encodes ISO/extended character data into the binary stream.
        /// </summary>
        /// <remarks>
        /// Handles full character set and switches to NUM or ALNUM mode when выгодно.
        /// </remarks>
        static void ProcessISOMode()
        {
            char ch = InputData[Position];
            int type = LookUp[ch];

            int digitCount = 0;

            if ((type & IsAlnu) != 0 && (type & IsFnc1) == 0)
            {
                if ((type & IsNum) != 0)
                    digitCount = 1;

                int i = 1;
                while (i < 10 && Position + i < InputLength)
                {
                    int nextType = LookUp[InputData[Position + i]];

                    if ((nextType & IsNum) != 0)
                    {
                        if (digitCount > 0) digitCount++;
                    }
                    else if (digitCount > 0)
                        digitCount = -digitCount;

                    if ((nextType & IsAlnu) == 0)
                        break;

                    i++;
                }

                bool atEnd = Position + i == InputLength;

                if ((atEnd && (digitCount >= 4 || digitCount <= -4)) || i == 10 && (digitCount >= 4 || digitCount <= -4))
                {
                    BinaryData.AppendBits(0, 3);
                    CurrentMode = NumMode;
                    return;
                }

                if ((atEnd && i >= 5) || i == 10)
                {
                    BinaryData.AppendBits(0x04, 5);
                    CurrentMode = AlnuMode;
                    return;
                }
            }

            Position++;

            if ((type & IsNum) != 0)
            {
                int val = (type & IsFnc1) != 0 ? 0x0F : ch - '0' + 5;

                if ((type & IsFnc1) != 0)
                    CurrentMode = NumMode;

                BinaryData.AppendBits(val, 5);
                return;
            }

            if (ch >= 'A' && ch <= 'Z')
            {
                BinaryData.AppendBits(ch - 'A' + 0x40, 7);
                return;
            }

            if (ch >= 'a' && ch <= 'z')
            {
                BinaryData.AppendBits(ch - 'a' + 0x5A, 7);
                return;
            }

            int res =
                ch == ' ' ? 0xFC :
                ch == '_' ? 0xFB :
                ch >= ':' ? ch - 58 + 0xF5 :
                ch >= '%' ? ch - 37 + 0xEA :
                ch - 33 + 0xE8;

            BinaryData.AppendBits(res, 8);
        }

        internal override float GetWidth(string code)
        {
            if (Symbol == null || Symbol.Count == 0)
                return 0;

            int maxModules = 0;

            foreach (var row in Symbol)
            {
                int length = row.GetRowData().Length;

                if (length > maxModules)
                    maxModules = length;
            }

            return maxModules * WideBarRatio;
        }

        internal override string GetPattern()
        {
            string parseGS1 = GS1Helper.ParseGS1(this.text);

            if (parseGS1.StartsWith("&1;"))
                parseGS1 = parseGS1.Substring(3);

            parseGS1 = parseGS1.Replace("&1;", "[");

            char[] parseGS1Char = parseGS1.ToCharArray();

            DatabarExpanded(parseGS1Char);
            return "";
        }

        protected void DrawLineBars(byte[] rowData, IGraphics g, float zoom, RectangleF rect)
        {
            if (rowData == null || rowData.Length == 0)
                return;

            using (Pen pen = new Pen(Color))
            {
                float moduleWidth = WideBarRatio * zoom;
                pen.Width = moduleWidth;

                float currentX = rect.Left * zoom;

                float yTop = (barArea.Top + rect.Top) * zoom;
                float yBottom = (barArea.Top + rect.Bottom) * zoom;

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (rowData[i] == 1)
                    {
                        float xCoord = currentX + (moduleWidth / 2);
                        g.DrawLine(pen, xCoord, yTop, xCoord, yBottom);
                    }

                    currentX += moduleWidth;
                }
            }
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
    /// Represents a GS1 DataBar Expanded Stacked barcode.
    /// </summary>
    public class BarcodeGS1ExpandedStacked : BarcodeGS1ExpandedBase
    {
        /// <summary>
        /// Number of segments (even, 2–20).
        /// </summary>
        [DefaultValue(4)]
        [Browsable(true)]
        [Category("Data")]
        public int Segments
        {
            get => this._segments;
            set
            {
                if (value >= 2 && value <= 20 && value % 2 == 0)
                    this._segments = value;
            }
        }

        internal override string GetPattern()
        {
            base.GetPattern();

            if (Symbol == null || Symbol.Count == 0)
                throw new FormatException(Res.Get("Messages,InvalidBarcodeGS1DatabarES"));

            return "";
        }

        internal override void DoLines(string data, IGraphics g, float zoom)
        {
            if (Symbol == null || Symbol.Count == 0) return;
            float totalSymbolHeightUnits = 0;
            foreach (SymbolData row in Symbol)
            {
                totalSymbolHeightUnits += row.RowHeight;
            }

            float heightMultiplier = barArea.Height / totalSymbolHeightUnits;

            float currentY = 0;

            foreach (SymbolData row in Symbol)
            {
                float rowHeight = row.RowHeight * heightMultiplier;

                RectangleF rowRect = new RectangleF(
                    0,
                    currentY,
                    0,
                    rowHeight);

                DrawLineBars(row.GetRowData(), g, zoom, rowRect);

                currentY += rowHeight;
            }
        }

        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);
            BarcodeGS1ExpandedStacked src = source as BarcodeGS1ExpandedStacked;

            Segments = src.Segments;
        }

        internal override void Serialize(FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodeGS1ExpandedStacked c = diff as BarcodeGS1ExpandedStacked;

            if (c == null || Segments != c.Segments)
                writer.WriteInt(prefix + "Segments", Segments);
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

