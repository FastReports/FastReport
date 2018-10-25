using System;
using System.ComponentModel;
using System.Globalization;

namespace FastReport.TypeConverters
{
    /// <summary>
    /// Provides a type converter for a property representing a data type.
    /// </summary>
    public class DataTypeConverter : TypeConverter
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string val = (string)value;
                if (!val.StartsWith("System."))
                    val = "System." + val;
                return Type.GetType(val);
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return (value as Type).Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Public Methods
    }
}