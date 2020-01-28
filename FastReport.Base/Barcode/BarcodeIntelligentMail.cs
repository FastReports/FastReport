using FastReport.Utils;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FastReport.Barcode
{
    /// <summary>
    /// Generates the Intelligent Mail (USPS) barcode.
    /// </summary>
    public class BarcodeIntelligentMail : LinearBarcodeBase
    {
        #region LinearBarcodeBase
        
        private bool quietZone;
        const string space = "2";

        /// <summary>
        /// Gets or sets the value indicating that quiet zone must be shown.
        /// </summary>
        [DefaultValue(false)]
        public bool QuietZone
        {
            get { return quietZone; }
            set { quietZone = value; }
        }

        /// <inheritdoc/>
        public override bool IsNumeric
        {
            get { return true; }
        }

        internal override string GetPattern()
        {
            string bars = Bars(text);
            if (QuietZone)
                bars = space + bars + space;
            return bars;
        }

        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);
            BarcodeIntelligentMail src = source as BarcodeIntelligentMail;
            QuietZone = src.QuietZone;
        }

        internal override void Serialize(FastReport.Utils.FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodeIntelligentMail c = diff as BarcodeIntelligentMail;
            if (c == null || QuietZone != c.QuietZone)
                writer.WriteBool(prefix + "QuietZone", QuietZone);
        }

        #endregion

        // for more information and specs check
        // http://ribbs.usps.gov/onecodesolution/USPS-B-3200D001.pdf

        int table2Of13Size = 78;
        int table5Of13Size = 1287;
        long entries2Of13;
        long entries5Of13;
        int[] table2Of13 = null;
        int[] table5Of13 = null;
        decimal[][] codewordArray = null;
        int[] barTopCharIndexArray = new int[] { 4, 0, 2, 6, 3, 5, 1, 9, 8, 7, 1, 2, 0, 6, 4, 8, 2, 9, 5, 3, 0, 1, 3, 7, 4, 6, 8, 9, 2, 0, 5, 1, 9, 4, 3, 8, 6, 7, 1, 2, 4, 3, 9, 5, 7, 8, 3, 0, 2, 1, 4, 0, 9, 1, 7, 0, 2, 4, 6, 3, 7, 1, 9, 5, 8 };
        int[] barBottomCharIndexArray = new int[] { 7, 1, 9, 5, 8, 0, 2, 4, 6, 3, 5, 8, 9, 7, 3, 0, 6, 1, 7, 4, 6, 8, 9, 2, 5, 1, 7, 5, 4, 3, 8, 7, 6, 0, 2, 5, 4, 9, 3, 0, 1, 6, 8, 2, 0, 4, 5, 9, 6, 7, 5, 2, 6, 3, 8, 5, 1, 9, 8, 7, 4, 0, 2, 6, 3 };
        int[] barTopCharShiftArray = new int[] { 3, 0, 8, 11, 1, 12, 8, 11, 10, 6, 4, 12, 2, 7, 9, 6, 7, 9, 2, 8, 4, 0, 12, 7, 10, 9, 0, 7, 10, 5, 7, 9, 6, 8, 2, 12, 1, 4, 2, 0, 1, 5, 4, 6, 12, 1, 0, 9, 4, 7, 5, 10, 2, 6, 9, 11, 2, 12, 6, 7, 5, 11, 0, 3, 2 };
        int[] barBottomCharShiftArray = new int[] { 2, 10, 12, 5, 9, 1, 5, 4, 3, 9, 11, 5, 10, 1, 6, 3, 4, 1, 10, 0, 2, 11, 8, 6, 1, 12, 3, 8, 6, 4, 4, 11, 0, 6, 1, 9, 11, 5, 3, 7, 3, 10, 7, 11, 8, 2, 10, 3, 5, 8, 0, 3, 12, 11, 8, 4, 5, 1, 3, 0, 7, 12, 9, 8, 10 };

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeIntelligentMail"/> class with default settings.
        /// </summary>
        public BarcodeIntelligentMail()
        {
            table2Of13 = OneCodeInfo(1);
            table5Of13 = OneCodeInfo(2);
            codewordArray = OneCodeInfo();
            QuietZone = false;
        }

        string Bars(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            source = TrimOff(source, " -.");
            if (!Regex.IsMatch(source, "^[0-9][0-4]([0-9]{18})|([0-9]{23})|([0-9]{27})|([0-9]{29})$"))
            //return string.Empty;
            {
                MyRes res = new MyRes("Messages");
                throw new FormatException(res.Get("BarcodeFewError"));
            }
            int fcs = 0;
            long l = 0;
            decimal v = 0;
            string encoded = string.Empty, ds = string.Empty, zip = source.Substring(20);
            int[] byteArray = new int[14], ai = new int[66], ai1 = new int[66];
            decimal[][] ad = new decimal[11][];
            if (!string.IsNullOrEmpty(zip) && zip.Length > 0)
                l = long.Parse(zip, CultureInfo.InvariantCulture) + ((zip.Length == 5) ? 1 : ((zip.Length == 9) ? 100001 : (zip.Length == 11 ? 1000100001 : 0)));
            v = l * 10 + int.Parse(source.Substring(0, 1), CultureInfo.InvariantCulture);
            v = v * 5 + int.Parse(source.Substring(1, 1), CultureInfo.InvariantCulture);
            ds = v.ToString(CultureInfo.InvariantCulture) + source.Substring(2, 18);
            byteArray[12] = (int)(l & 255);
            byteArray[11] = (int)(l >> 8 & 255);
            byteArray[10] = (int)(l >> 16 & 255);
            byteArray[9] = (int)(l >> 24 & 255);
            byteArray[8] = (int)(l >> 32 & 255);
            OneCodeMathMultiply(ref byteArray, 13, 10);
            OneCodeMathAdd(ref byteArray, 13, int.Parse(source.Substring(0, 1), CultureInfo.InvariantCulture));
            OneCodeMathMultiply(ref byteArray, 13, 5);
            OneCodeMathAdd(ref byteArray, 13, int.Parse(source.Substring(1, 1), CultureInfo.InvariantCulture));
            for (short i = 2; i <= 19; i++)
            {
                OneCodeMathMultiply(ref byteArray, 13, 10);
                OneCodeMathAdd(ref byteArray, 13, int.Parse(source.Substring(i, 1), CultureInfo.InvariantCulture));
            }
            fcs = OneCodeMathFcs(byteArray);
            for (short i = 0; i <= 9; i++)
            {
                codewordArray[i][0] = entries2Of13 + entries5Of13;
                codewordArray[i][1] = 0;
            }
            codewordArray[0][0] = 659;
            codewordArray[9][0] = 636;
            OneCodeMathDivide(ds);
            codewordArray[9][1] *= 2;
            if (fcs >> 10 != 0) codewordArray[0][1] += 659;
            for (short i = 0; i <= 9; i++) ad[i] = new decimal[3];
            for (short i = 0; i <= 9; i++)
            {
                if (codewordArray[i][1] >= (decimal)(entries2Of13 + entries5Of13)) return null;
                ad[i][0] = 8192;
                ad[i][1] = (codewordArray[i][1] >= (decimal)entries2Of13) ? ad[i][1] = table2Of13[(int)(codewordArray[i][1] - entries2Of13)] : ad[i][1] = table5Of13[(int)codewordArray[i][1]];
            }
            for (short i = 0; i <= 9; i++) if ((fcs & 1 << i) != 0) ad[i][1] = ~(int)ad[i][1] & 8191;
            for (short i = 0; i <= 64; i++)
            {
                ai[i] = (int)ad[barTopCharIndexArray[i]][1] >> barTopCharShiftArray[i] & 1;
                ai1[i] = (int)ad[barBottomCharIndexArray[i]][1] >> barBottomCharShiftArray[i] & 1;
            }
            encoded = "";
            for (int i = 0; i <= 64; i++)
            {
                //if (ai[i] == 0) encoded += (ai1[i] == 0) ? "T" : "D";
                //else encoded += (ai1[i] == 0) ? "A" : "F";
                if (ai[i] == 0)
                    encoded += (ai1[i] == 0) ? "E" : "G";
                else
                    encoded += (ai1[i] == 0) ? "F" : "6";
                encoded += space;
            }
            return encoded.Substring(0, encoded.Length - 1);
        }

        int[] OneCodeInfo(byte topic)
        {
            int[] a;
            switch (topic)
            {
                case 1:
                    a = new int[table2Of13Size + 1];
                    OneCodeInitializeNof13Table(ref a, 2, table2Of13Size);
                    entries5Of13 = table2Of13Size;
                    break;
                default:
                    a = new int[table5Of13Size + 1];
                    OneCodeInitializeNof13Table(ref a, 5, table5Of13Size);
                    entries2Of13 = table5Of13Size;
                    break;
            }
            return a;
        }

        decimal[][] OneCodeInfo()
        {
            decimal[][] da = new decimal[11][];
            try
            {
                for (short i = 0; i <= 9; i++) da[i] = new decimal[3];
                return da;
            }
            finally
            {
                da = null;
            }
        }

        bool OneCodeInitializeNof13Table(ref int[] ai, int i, int j)
        {
            int i1 = 0;
            int j1 = j - 1;
            for (short k = 0; k <= 8191; k++)
            {
                int k1 = 0;
                for (int l1 = 0; l1 <= 12; l1++) if ((k & 1 << l1) != 0) k1 += 1;
                if (k1 == i)
                {
                    int l = OneCodeMathReverse(k) >> 3;
                    bool flag = k == l;
                    if (l >= k)
                    {
                        if (flag)
                        {
                            ai[j1] = k;
                            j1 -= 1;
                        }
                        else
                        {
                            ai[i1] = k;
                            i1 += 1;
                            ai[i1] = l;
                            i1 += 1;
                        }
                    }
                }
            }
            return i1 == j1 + 1;
        }

        bool OneCodeMathAdd(ref int[] bytearray, int i, int j)
        {
            if (bytearray == null) return false;
            if (i < 1) return false;
            int x = (bytearray[i - 1] | (bytearray[i - 2] << 8)) + j;
            int l = x | 65535;
            int k = i - 3;
            bytearray[i - 1] = x & 255;
            bytearray[i - 2] = x >> 8 & 255;
            while (l == 1 && k > 0)
            {
                x = l + bytearray[k];
                bytearray[k] = x & 255;
                l = x | 255;
                k -= 1;
            }
            return true;
        }

        bool OneCodeMathDivide(string v)
        {
            int j = 10;
            string n = v;
            for (int k = j - 1; k >= 1; k += -1)
            {
                string r = string.Empty;
                int divider = (int)codewordArray[k][0];
                string copy = n;
                string left = "0";
                int l = copy.Length;
                for (short i = 1; i <= l; i++)
                {
                    int divident = int.Parse(copy.Substring(0, i), CultureInfo.InvariantCulture);
                    while (divident < divider & i < l - 1)
                    {
                        r = r + "0";
                        i += 1;
                        divident = int.Parse(copy.Substring(0, i), CultureInfo.InvariantCulture);
                    }
                    r = r + (divident / divider).ToString(CultureInfo.InvariantCulture);
                    left = (divident % divider).ToString(CultureInfo.InvariantCulture).PadLeft(i, '0');
                    copy = left + copy.Substring(i);
                }
                n = r.TrimStart('0');
                if (string.IsNullOrEmpty(n)) n = "0";
                codewordArray[k][1] = int.Parse(left, CultureInfo.InvariantCulture);
                if (k == 1) codewordArray[0][1] = int.Parse(r, CultureInfo.InvariantCulture);
            }
            return true;
        }

        int OneCodeMathFcs(int[] bytearray)
        {
            int c = 3893;
            int i = 2047;
            int j = bytearray[0] << 5;
            for (short b = 2; b <= 7; b++)
            {
                if (((i ^ j) & 1024) != 0) i = i << 1 ^ c;
                else i <<= 1;
                i = i & 2047;
                j <<= 1;
            }
            for (int l = 1; l <= 12; l++)
            {
                int k = bytearray[l] << 3;
                for (short b = 0; b <= 7; b++)
                {
                    if (((i ^ k) & 1024) != 0) i = i << 1 ^ c;
                    else i <<= 1;
                    i = i & 2047;
                    k <<= 1;
                }
            }
            return i;
        }

        bool OneCodeMathMultiply(ref int[] bytearray, int i, int j)
        {
            if (bytearray == null) return false;
            if (i < 1) return false;
            int l = 0;
            int k = 0;
            for (k = i - 1; k >= 1; k += -2)
            {
                int x = (bytearray[k] | (bytearray[k - 1] << 8)) * j + l;
                bytearray[k] = x & 255;
                bytearray[k - 1] = x >> 8 & 255;
                l = x >> 16;
            }
            if (k == 0) bytearray[0] = (bytearray[0] * j + l) & 255;
            return true;
        }

        int OneCodeMathReverse(int i)
        {
            int j = 0;
            for (short k = 0; k <= 15; k++)
            {
                j <<= 1;
                j = j | i & 1;
                i >>= 1;
            }
            return j;
        }

        string TrimOff(string source, string bad)
        {
            for (int i = 0, l = bad.Length - 1; i <= l; i++) source = source.Replace(bad.Substring(i, 1), string.Empty);
            return source;
        }
    }
}
