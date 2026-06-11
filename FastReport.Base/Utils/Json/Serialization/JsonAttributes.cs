using System;

namespace FastReport.Utils.Json.Serialization
{
    /// <summary>
    /// Represents JSON property attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JsonPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Determines whether to ignore null value.
        /// </summary>
        public bool IgnoreNullValue { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="ignoreNullValue">Ignore null value.</param>
        public JsonPropertyAttribute(string propertyName, bool ignoreNullValue = true)
        {
            PropertyName = propertyName;
            IgnoreNullValue = ignoreNullValue;
        }
    }

    /// <summary>
    /// Represents JSON ignore attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonIgnoreAttribute : Attribute
    {

    }
}
