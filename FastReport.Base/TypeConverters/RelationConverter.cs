using FastReport.Data;
using System;
using System.ComponentModel;
using System.Globalization;

namespace FastReport.TypeConverters
{
    internal class RelationConverter : TypeConverter
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
                if (context != null && context.Instance != null && !String.IsNullOrEmpty((string)value))
                {
                    ComponentBase c = context.Instance as ComponentBase;
                    Report report = c.Report;
                    if (report != null)
                        return report.Dictionary.Relations.FindByAlias((string)value);
                }
                return null;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                    return "";
                return (value as Relation).Alias;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Public Methods
    }
}