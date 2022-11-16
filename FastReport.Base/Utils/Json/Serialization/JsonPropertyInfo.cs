using System;
using System.Collections;
using System.Reflection;

namespace FastReport.Utils.Json.Serialization
{
    internal class JsonPropertyInfo
    {
        public PropertyInfo Info { get; }

        public string Name { get; }

        public bool IgnoreNullValue { get; }

        public bool IsPrimitive =>
            Info.PropertyType.IsPrimitive;

        public bool IsDateTime =>
            Info.PropertyType == typeof(DateTime);

        public bool IsCollection
        {
            get
            {
                var type = Info.PropertyType;
                var result = type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
                return result;
            }
        }

        public bool IsEnum => Info.PropertyType.IsEnum;

        internal static JsonPropertyInfo Parse(PropertyInfo propertyInfo)
        {
            var attr = Attribute.GetCustomAttribute(propertyInfo, typeof(JsonPropertyAttribute)) as JsonPropertyAttribute;
            string propName = attr?.PropertyName ?? propertyInfo.Name;
            bool ignoreNull = attr?.IgnoreNullValue ?? true;
            var propInfo = new JsonPropertyInfo(propertyInfo, propName, ignoreNull);
            return propInfo;
        }

        public JsonPropertyInfo(PropertyInfo propertyInfo,
            string propertyName,
            bool ignoreNullValue = true)
        {
            Info = propertyInfo;
            Name = propertyName;
            IgnoreNullValue = ignoreNullValue;
        }
    }
}