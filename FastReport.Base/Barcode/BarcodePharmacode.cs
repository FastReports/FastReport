using System;
using System.ComponentModel;
using System.Text;

namespace FastReport.Barcode
{
    /// <summary>
    /// Generates the Pharmacode barcode.
    /// </summary>
    public class BarcodePharmacode : LinearBarcodeBase
    {
        private bool quietZone;

        /// <summary>
        /// Gets or sets the value indicating that quiet zone must be shown.
        /// </summary>
        [DefaultValue(true)]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodePharmacode"/> class with default settings.
        /// </summary>
        public BarcodePharmacode()
        {
            QuietZone = true;
        }

        internal override string GetPattern()
        {
            ulong value = ulong.Parse(text);
            value += 1;
            string binary = Convert.ToString((long)value, 2);

            if (binary.StartsWith("1"))
                binary = binary.Remove(0, 1);

            const string space = "2";
            StringBuilder result = new StringBuilder();

            if (QuietZone)
                result.Append(space);

            foreach (char c in binary)
            {
                switch(c)
                {
                    case '0':
                        result.Append("5");
                        result.Append(space);
                        break;
                    case '1':
                        result.Append("7");
                        result.Append(space);
                        break;
                }
            }

            if (!QuietZone && result.ToString().EndsWith(space))
                result.Remove(result.Length - space.Length, space.Length);

            return result.ToString();
        }

        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);
            BarcodePharmacode src = source as BarcodePharmacode;
            QuietZone = src.QuietZone;
        }

        internal override void Serialize(FastReport.Utils.FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodePharmacode c = diff as BarcodePharmacode;
            if (c == null || QuietZone != c.QuietZone)
                writer.WriteBool(prefix + "QuietZone", QuietZone);
        }
    }
}
