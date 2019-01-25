using System;
using System.ComponentModel;
using System.Globalization;

namespace FastReport.TypeConverters
{
    internal class FillConverter : TypeConverter
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
                FillBase result = null;
                string className = (string)value;
                switch (className)
                {
                    case "Solid":
                        result = new SolidFill();
                        break;

                    case "LinearGradient":
                        result = new LinearGradientFill();
                        break;

                    case "PathGradient":
                        result = new PathGradientFill();
                        break;

                    case "Hatch":
                        result = new HatchFill();
                        break;

                    case "Glass":
                        result = new GlassFill();
                        break;
                    case "Texture":
                        result = new TextureFill();
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
                FillBase fill = value as FillBase;
                if (fill == null)
                    return "";
                return fill.Name;
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