using System;
using System.ComponentModel;

namespace FastReport.TypeConverters
{
    /// <summary>
    /// Blocks keyboard editing, you need to select a value from the drop-down list for editing
    /// </summary>
    internal class FlagConverter : EnumConverter
    {
        public FlagConverter(Type type) : base(type)
        {
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}