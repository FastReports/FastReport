using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using FastReport.Utils;

namespace FastReport.Barcode
{
    /// <summary>
    /// Generates the Codabar barcode.
    /// </summary>
    public class BarcodeCodabar : LinearBarcodeBase
    {
#if READONLY_STRUCTS
        private readonly struct Codabar
#else
        private struct Codabar
#endif
        {
#pragma warning disable FR0006 // Field name of struct must be longer than 2 characters.
            public readonly string c;
#pragma warning restore FR0006 // Field name of struct must be longer than 2 characters.
            public readonly string data;

            public Codabar(string c, string data)
            {
                this.c = c;
                this.data = data;
            }
        }

        /// <inheritdoc/>
        public enum CodabarChar { A, B, C, D };

        private static Codabar[] tabelle_cb = {
      new Codabar("1", "5050615"),
      new Codabar("2", "5051506"),
      new Codabar("3", "6150505"),
      new Codabar("4", "5060515"),
      new Codabar("5", "6050515"),
      new Codabar("6", "5150506"),
      new Codabar("7", "5150605"),
      new Codabar("8", "5160505"),
      new Codabar("9", "6051505"),
      new Codabar("0", "5050516"),
      new Codabar("-", "5051605"),
      new Codabar("$", "5061505"),
      new Codabar(":", "6050606"),
      new Codabar("/", "6060506"),
      new Codabar(".", "6060605"),
      new Codabar("+", "5060606"),
      new Codabar("A", "5061515"),
      new Codabar("B", "5151506"),
      new Codabar("C", "5051516"),
      new Codabar("D", "5051615") };

        /// <summary>
        /// Specifies start character (A, B, C or D).
        /// </summary>
        /// <inheritdoc/>
        [Category("Barcode")]
        [DefaultValue(CodabarChar.A)]
        public CodabarChar StartChar { get; set; } = CodabarChar.A;

        /// <summary>
        /// Specifies stop character (A, B, C or D)
        /// </summary>
        /// <inheritdoc/>
        [Category("Barcode")]
        [DefaultValue(CodabarChar.B)]
        public CodabarChar StopChar { get; set; } = CodabarChar.B;

        /// <inheritdoc/>
        public override bool IsNumeric
        {
            get { return false; }
        }

        private int FindBarItem(string c)
        {
            for (int i = 0; i < tabelle_cb.Length; i++)
            {
                if (c == tabelle_cb[i].c)
                    return i;
            }
            return -1;
        }

        internal override string GetPattern()
        {
            string result = "";
            int index = FindBarItem(StartChar.ToString());
            if (index >= 0)
            {
                result = tabelle_cb[index].data + "0";
            }

            foreach (char c in text)
            {
                index = FindBarItem(c.ToString());
                if (index >= 0)
                {
                    result += tabelle_cb[index].data + "0";
                }
            }

            index = FindBarItem(StopChar.ToString());
            if (index >= 0)
            {
                result += tabelle_cb[index].data;
            }
            return result;
        }

        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);
            StartChar = (source as BarcodeCodabar).StartChar;
            StopChar = (source as BarcodeCodabar).StopChar;
        }

        /// <inheritdoc/>
        internal override void Serialize(FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodeCodabar c = diff as BarcodeCodabar;

            if (c == null || StartChar != c.StartChar)
                writer.WriteStr(prefix + "StartChar", StartChar.ToString());

            if (c == null || StopChar != c.StopChar)
                writer.WriteStr(prefix + "StopChar", StopChar.ToString());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeCodabar"/> class with default settings.
        /// </summary>
        public BarcodeCodabar()
        {
            ratioMin = 2;
            ratioMax = 3;
        }
    }
}
