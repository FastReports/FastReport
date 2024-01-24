using System;

namespace FastReport.Utils.Json.Serialization
{
    [AttributeUsage(AttributeTargets.Property,
        AllowMultiple = false)]
    public class JsonPropertyAttribute : Attribute
    {

        public string PropertyName { get; }

        public bool IgnoreNullValue { get; }

        public JsonPropertyAttribute(string propertyName, bool ignoreNullValue = true)
        {
            PropertyName = propertyName;
            IgnoreNullValue = ignoreNullValue;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonIgnoreAttribute : Attribute
    {

    }
}
