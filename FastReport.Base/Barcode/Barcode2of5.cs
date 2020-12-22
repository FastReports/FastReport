using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FastReport.Barcode
{
    /// <summary>
    /// Generates the "2/5 Interleaved" barcode.
    /// </summary>
    public class Barcode2of5Interleaved : LinearBarcodeBase
    {
        internal static int[,] tabelle_2_5 = {
      {0, 0, 1, 1, 0},    // 0
      {1, 0, 0, 0, 1},    // 1
      {0, 1, 0, 0, 1},    // 2
      {1, 1, 0, 0, 0},    // 3
      {0, 0, 1, 0, 1},    // 4
      {1, 0, 1, 0, 0},    // 5
      {0, 1, 1, 0, 0},    // 6
      {0, 0, 0, 1, 1},    // 7
      {1, 0, 0, 1, 0},    // 8
      {0, 1, 0, 1, 0}     // 9
    };

        internal override string GetPattern()
        {
            string text = base.text;
            string result = "5050";   //Startcode
            string c;

            if (CalcCheckSum)
            {
                if (text.Length % 2 == 0)
                    text = text.Substring(1, text.Length - 1);
                text = DoCheckSumming(text);
            }
            else
            {
                if (text.Length % 2 != 0)
                    text = "0" + text;
            }

            for (int i = 0; i < (text.Length / 2); i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (tabelle_2_5[CharToInt(text[i * 2]), j] == 1)
                        c = "6";
                    else
                        c = "5";
                    result += c;

                    if (tabelle_2_5[CharToInt(text[i * 2 + 1]), j] == 1)
                        c = "1";
                    else
                        c = "0";
                    result += c;
                }
            }

            result += "605";    // Stopcode
            return result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Barcode2of5Interleaved"/> class with default settings.
        /// </summary>
        public Barcode2of5Interleaved()
        {
            ratioMin = 2;
            ratioMax = 3;
        }
    }
    /// <summary>
    /// Generates the "Deutsche Identcode" barcode.
    /// </summary>
    public class BarcodeDeutscheIdentcode : Barcode2of5Interleaved
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value that indicates that CheckSum should be printed.
        /// </summary>
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Category("Appearance")]
        public bool PrintCheckSum { get; set; }

        #endregion

        private string CheckSumModulo10(string data)
        {
            int sum = 0;
            int fak = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                if ((fak % 2) == 0)
                    sum += int.Parse(data[i].ToString()) * 9;
                else
                    sum += int.Parse(data[i].ToString()) * 4;
                fak--;
            }

            if ((sum % 10) == 0)
                return data + "0";
            return data + (10 - (sum % 10)).ToString();
        }

        internal override string GetPattern()
        {
            string result = "5050";   //Startcode
            string c;
            string text = base.text.Replace(".", "").Replace(" ", "");

            if(CalcCheckSum)
            {
                if (text.Length == 11)
                    text = CheckSumModulo10(text);
                else if (text.Length != 12)
                    throw new Exception(Res.Get("Messages,BarcodeLengthMismatch"));

            }
            else
            {
                if(text.Length != 12)
                    throw new Exception(Res.Get("Messages,BarcodeLengthMismatch"));
            }

            for (int i = 0; i < (text.Length / 2); i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (tabelle_2_5[CharToInt(text[i * 2]), j] == 1)
                        c = "6";
                    else
                        c = "5";
                    result += c;

                    if (tabelle_2_5[CharToInt(text[i * 2 + 1]), j] == 1)
                        c = "1";
                    else
                        c = "0";
                    result += c;
                }
            }

            result += "605";    // Stopcode

            base.text = text.Insert(2, ".").Insert(6, " ").Insert(10, ".");

            if(!PrintCheckSum)
            {
                base.text = base.text.Substring(0, base.text.Length - 1);
            }
            else
                base.text = base.text.Insert(14, " ");           


            return result;
        }

        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);

            BarcodeDeutscheIdentcode src = source as BarcodeDeutscheIdentcode;
            PrintCheckSum = src.PrintCheckSum;
        }
        internal override void Serialize(FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodeDeutscheIdentcode c = diff as BarcodeDeutscheIdentcode;

            if (c == null || PrintCheckSum != c.PrintCheckSum)
                writer.WriteValue(prefix + "DrawVerticalBearerBars", PrintCheckSum);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeDeutscheIdentcode"/> class with default settings.
        /// </summary>
        public BarcodeDeutscheIdentcode()
        {
            ratioMin = 2.25F;
            ratioMax = 3.5F;
            WideBarRatio = 3F;
            PrintCheckSum = true;
        }
    }

    /// <summary>
    /// Generates the "Deutsche Leitcode" barcode.
    /// </summary>
    public class BarcodeDeutscheLeitcode : Barcode2of5Interleaved
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value that indicates that CheckSum should be printed.
        /// </summary>
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Category("Appearance")]
        public bool PrintCheckSum { get; set; }

        private string CheckSumModulo10(string data)
        {
            int sum = 0;
            int fak = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                if ((fak % 2) == 0)
                    sum += int.Parse(data[i].ToString()) * 9;
                else
                    sum += int.Parse(data[i].ToString()) * 4;
                fak--;
            }

            if ((sum % 10) == 0)
                return data + "0";
            return data + (10 - (sum % 10)).ToString();
        }
		

        #endregion 
        internal override void Serialize(FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodeDeutscheLeitcode c = diff as BarcodeDeutscheLeitcode;

            if (c == null || PrintCheckSum != c.PrintCheckSum)
                writer.WriteValue(prefix + "DrawVerticalBearerBars", PrintCheckSum);
        }
        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);

            BarcodeDeutscheLeitcode src = source as BarcodeDeutscheLeitcode;
            PrintCheckSum = src.PrintCheckSum;
        }

        internal override string GetPattern()
        {
            string result = "5050";   //Startcode
            string c;
            string text = base.text.Replace(".", "").Replace(" ", "");

            if (CalcCheckSum)
            {
                if (text.Length == 13)
                    text = CheckSumModulo10(text);
                else if (text.Length != 14)
                    throw new Exception(Res.Get("Messages,BarcodeLengthMismatch"));

            }
            else
            {
                if (text.Length != 14)
                    throw new Exception(Res.Get("Messages,BarcodeLengthMismatch"));
            }

            for (int i = 0; i < (text.Length / 2); i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (tabelle_2_5[CharToInt(text[i * 2]), j] == 1)
                        c = "6";
                    else
                        c = "5";
                    result += c;

                    if (tabelle_2_5[CharToInt(text[i * 2 + 1]), j] == 1)
                        c = "1";
                    else
                        c = "0";
                    result += c;
                }
            }

            result += "605";    // Stopcode

            base.text = text
                .Insert(5, ".")
                .Insert(6, " ")
                .Insert(10, ".")
                .Insert(11, " ")
                .Insert(15, ".")
                .Insert(16, " ")
                .Insert(19, " ");

       


            return result;
        }

        public BarcodeDeutscheLeitcode()
        {
            ratioMin = 2.25F;
            ratioMax = 3.5F;
            WideBarRatio = 3F;
            CalcCheckSum = true;
        }


    }
    /// <summary>
    /// Generates the "ITF-14" barcode.
    /// </summary>
    public class BarcodeITF14 : Barcode2of5Interleaved
    {
        #region Fields
        private bool drawVerticalBearerBars = true;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value indicating that vertical bearer bars are needed to draw.
        /// </summary>
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Category("Appearance")]
        public bool DrawVerticalBearerBars
        {
            get
            {
                return this.drawVerticalBearerBars;
            }
            set
            {
                this.drawVerticalBearerBars = value;
            }
        }

        #endregion

        #region Internal Methods
        internal override string GetPattern()
        {
            string result = "";   // Startcode
            for (int i = 0; i < 14; i++)//10 for light margin and 4 for vertical bearer bar
            {
                result += "0";
            }
            result += "5050";
            string c;

            if (CalcCheckSum)
            {
                base.text = DoCheckSumming(base.text, 14);
            }
            else base.text = SetLen(14);

            for (int i = 0; i < (base.text.Length / 2); i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (tabelle_2_5[CharToInt(base.text[i * 2]), j] == 1)
                        c = "6";
                    else
                        c = "5";
                    result += c;

                    if (tabelle_2_5[CharToInt(base.text[i * 2 + 1]), j] == 1)
                        c = "1";
                    else
                        c = "0";
                    result += c;
                }
            }

            result += "605";   //Stopcode 
            for (int i = 0; i < 14; i++)//10 for light margin and 4 for vertical bearer bar
            {
                result += "0";
            }
            return result;
        }

        internal override void DrawText(IGraphicsRenderer g, string data)
        {
            data = StripControlCodes(data);
            DrawString(g, 0, drawArea.Width, data.Insert(1, " ").Insert(4, " ").Insert(10, " ").Insert(16, " "));
        }

        internal override void Serialize(FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodeITF14 c = diff as BarcodeITF14;

            if (c == null || DrawVerticalBearerBars != c.DrawVerticalBearerBars)
                writer.WriteValue(prefix + "DrawVerticalBearerBars", DrawVerticalBearerBars);
        }

        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);

            BarcodeITF14 src = source as BarcodeITF14;
            DrawVerticalBearerBars = src.DrawVerticalBearerBars;
        }

        public override void DrawBarcode(IGraphicsRenderer g, RectangleF displayRect)
        {
            base.DrawBarcode(g, displayRect);
            float bearerWidth = WideBarRatio * 2 * zoom;
            using (Pen pen = new Pen(Color, bearerWidth))
            {
                float x0 = displayRect.Left;
                float x01 = displayRect.Left + bearerWidth / 2;
                float y0 = displayRect.Top;
                float y01 = displayRect.Top + bearerWidth / 2;
                float x1 = displayRect.Left + displayRect.Width;
                float x11 = displayRect.Left + displayRect.Width - bearerWidth / 2;
                float y1 = displayRect.Top + barArea.Bottom * zoom;
                float y11 = displayRect.Top + barArea.Bottom * zoom - bearerWidth / 2;

                g.DrawLine(pen, x0, y01 - 0.5F, x1, y01 - 0.5F);
                g.DrawLine(pen, x0, y11, x1, y11);
                if (this.drawVerticalBearerBars)
                {
                    g.DrawLine(pen, x01 - 0.5F, y0, x01 - 0.5F, y1);
                    g.DrawLine(pen, x11, y0, x11, y1);
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeITF14"/> class with default settings.
        /// </summary>
        public BarcodeITF14()
        {
            ratioMin = 2.25F;
            ratioMax = 3.0F;
            WideBarRatio = 2.25F;
        }
    }

    /// <summary>
    /// Generates the "2/5 Industrial" barcode.
    /// </summary>
    public class Barcode2of5Industrial : Barcode2of5Interleaved
    {
        internal override string GetPattern()
        {
            string text = base.text;
            string result = "606050";   // Startcode

            if (CalcCheckSum)
            {
                text = DoCheckSumming(text);
            }

            for (int i = 0; i < text.Length; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (tabelle_2_5[CharToInt(text[i]), j] == 1)
                        result += "60";
                    else
                        result += "50";
                }
            }

            result += "605060";   //Stopcode 
            return result;
        }
    }

    /// <summary>
    /// Generates the "2/5 Matrix" barcode.
    /// </summary>
    public class Barcode2of5Matrix : Barcode2of5Interleaved
    {
        internal override string GetPattern()
        {
            string text = base.text;
            string result = "705050";   // Startcode
            char c;

            if (CalcCheckSum)
            {
                text = DoCheckSumming(text);
            }

            for (int i = 0; i < text.Length; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (tabelle_2_5[CharToInt(text[i]), j] == 1)
                        c = '1';
                    else
                        c = '0';

                    if ((j % 2) == 0)
                        c = (char)((int)c + 5);
                    result += c;
                }
                result += '0';
            }

            result = result + "70505";   // Stopcode

            return result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Barcode2of5Matrix"/> class with default settings.
        /// </summary>
        public Barcode2of5Matrix()
        {
            ratioMin = 2.25f;
            ratioMax = 3;
            WideBarRatio = 2.25f;
        }
    }
}
