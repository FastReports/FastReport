using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FastReport.Utils;
using System.Globalization;

namespace FastReport.Format
{
    /// <summary>
    /// Defines how numeric values are formatted and displayed.
    /// </summary>
    public class NumberFormat : FormatBase
    {
        #region Fields
        private bool useLocale;
        private int decimalDigits;
        private string decimalSeparator;
        private string groupSeparator;
        private int negativePattern;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value that determines whether to use system locale settings to format a value.
        /// </summary>
        [DefaultValue(true)]
        public bool UseLocale
        {
            get { return useLocale; }
            set { useLocale = value; }
        }

        /// <summary>
        /// Gets or sets the number of decimal places to use in numeric values. 
        /// </summary>
        [DefaultValue(2)]
        public int DecimalDigits
        {
            get { return decimalDigits; }
            set { decimalDigits = value; }
        }

        /// <summary>
        /// Gets or sets the string to use as the decimal separator in numeric values.
        /// </summary>
        public string DecimalSeparator
        {
            get { return decimalSeparator; }
            set { decimalSeparator = value; }
        }

        /// <summary>
        /// Gets or sets the string that separates groups of digits to the left of the decimal in numeric values. 
        /// </summary>
        public string GroupSeparator
        {
            get { return groupSeparator; }
            set { groupSeparator = value; }
        }

        /// <summary>
        /// Gets or sets the format pattern for negative numeric values.
        /// </summary>
        /// <remarks>This property can have one of the values in the following table. 
        /// The symbol <i>n</i> is a number.
        /// <list type="table">
        ///   <listheader><term>Value</term><description>Associated Pattern</description></listheader>
        ///   <item><term>0</term><description>(n)</description></item>
        ///   <item><term>1</term><description>-n</description></item>
        ///   <item><term>2</term><description>- n</description></item>
        ///   <item><term>3</term><description>n-</description></item>
        ///   <item><term>4</term><description>n -</description></item>
        /// </list>
        /// </remarks>
        [DefaultValue(0)]
        public int NegativePattern
        {
            get { return negativePattern; }
            set { negativePattern = value; }
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override FormatBase Clone()
        {
            NumberFormat result = new NumberFormat();
            result.UseLocale = UseLocale;
            result.DecimalDigits = DecimalDigits;
            result.DecimalSeparator = DecimalSeparator;
            result.GroupSeparator = GroupSeparator;
            result.NegativePattern = NegativePattern;
            return result;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            NumberFormat f = obj as NumberFormat;
            return f != null &&
              UseLocale == f.UseLocale &&
              DecimalDigits == f.DecimalDigits &&
              DecimalSeparator == f.DecimalSeparator &&
              GroupSeparator == f.GroupSeparator &&
              NegativePattern == f.NegativePattern;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc/>
        public override string FormatValue(object value)
        {
            if (value is Variant)
                value = ((Variant)value).Value;

            return String.Format(GetNumberFormatInfo(), "{0:n}", value);
        }

        internal NumberFormatInfo GetNumberFormatInfo()
        {
            NumberFormatInfo info;
            if (UseLocale)
            {
                info = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                info.NumberDecimalDigits = DecimalDigits;
            }
            else
            {
                info = new NumberFormatInfo();
                info.NumberDecimalDigits = DecimalDigits;
                info.NumberDecimalSeparator = DecimalSeparator;
                info.NumberGroupSizes = new int[] { 3 };
                info.NumberGroupSeparator = GroupSeparator;
                info.NumberNegativePattern = NegativePattern;
            }
            return info;
        }

        internal override string GetSampleValue()
        {
            return FormatValue(-12345f);
        }

        internal override void Serialize(FRWriter writer, string prefix, FormatBase format)
        {
            base.Serialize(writer, prefix, format);
            NumberFormat c = format as NumberFormat;

            if (c == null || UseLocale != c.UseLocale)
                writer.WriteBool(prefix + "UseLocale", UseLocale);
            if (c == null || DecimalDigits != c.DecimalDigits)
                writer.WriteInt(prefix + "DecimalDigits", DecimalDigits);

            if (!UseLocale)
            {
                if (c == null || DecimalSeparator != c.DecimalSeparator)
                    writer.WriteStr(prefix + "DecimalSeparator", DecimalSeparator);
                if (c == null || GroupSeparator != c.GroupSeparator)
                    writer.WriteStr(prefix + "GroupSeparator", GroupSeparator);
                if (c == null || NegativePattern != c.NegativePattern)
                    writer.WriteInt(prefix + "NegativePattern", NegativePattern);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <b>NumberFormat</b> class with default settings. 
        /// </summary>
        public NumberFormat()
        {
            UseLocale = true;
            DecimalDigits = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
            DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            GroupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
        }
    }
}
