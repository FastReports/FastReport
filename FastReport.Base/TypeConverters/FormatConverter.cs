using FastReport.Format;
using System;
using System.ComponentModel;
using System.Globalization;

namespace FastReport.TypeConverters
{
    internal class FormatConverter : TypeConverter
    {
        #region Public Methods

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                FormatBase result = null;
                string className = (string)value;
                switch (className)
                {
                    case "Boolean":
                        result = new BooleanFormat();
                        break;

                    case "Currency":
                        result = new CurrencyFormat();
                        break;

                    case "Custom":
                        result = new CustomFormat();
                        break;

                    case "Date":
                        result = new DateFormat();
                        break;

                    case "General":
                        result = new GeneralFormat();
                        break;

                    case "Number":
                        result = new NumberFormat();
                        break;

                    case "Percent":
                        result = new PercentFormat();
                        break;

                    case "Time":
                        result = new TimeFormat();
                        break;
                }
                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                FormatBase format = value as FormatBase;
                if (format == null)
                    return "";
                return format.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context,
                                      object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        #endregion Public Methods
    }
}