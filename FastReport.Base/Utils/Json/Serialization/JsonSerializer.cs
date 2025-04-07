using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace FastReport.Utils.Json.Serialization
{
    internal static class JsonSerializer
    {
        private static readonly Dictionary<Type, JsonPropertyInfo[]> _readablePropertiesCache =
            new Dictionary<Type, JsonPropertyInfo[]>();


        public static string Serialize<T>(T instance)
        {
            StringBuilder sb = new StringBuilder(32);
            SerializeProperties(instance, sb);
            return sb.ToString();
        }

        private static void SerializeProperties<T>(T instance, StringBuilder sb)
        {
            var properties = GetReadableProperties<T>();

            SerializeProperties(instance, sb, properties);

        }

        private static void SerializeProperties(object instance, StringBuilder sb, JsonPropertyInfo[] properties)
        {
            var json = ParseToJsonObject(instance, properties);
            json.WriteTo(sb, 0);
        }

        private static JsonObject ParseToJsonObject(object instance, JsonPropertyInfo[] properties)
        {
            if (instance == null)
                return null;

            JsonObject obj = new JsonObject();
            foreach (var property in properties)
            {
                var propInfo = property.Info;
                var propName = property.Name;

                var value = propInfo.GetValue(instance);
                if (value == null)
                {
                    if (property.IgnoreNullValue)
                        continue;
                }
                else
                {
                    var actualType = value.GetType();

                    if (IsCollection(actualType))
                    {
                        value = ParseToArray(value, actualType);
                    }
                    else if (IsCustomType(actualType))
                    {
                        value = ParseToJsonObject(value, actualType);
                    }
                }

                obj[propName] = value;
            }
            return obj;
        }

        private static object ParseToArray(object value)
        {
            var type = value.GetType();
            return ParseToArray(value, type);
        }

        private static object ParseToArray(object value, Type valueType)
        {
            // BASE 64
            if (valueType.IsArray && valueType.GetElementType() == typeof(byte))
            {
                return Convert.ToBase64String((byte[])value);
            }
            else
                return ParseToJsonArray(value);
        }

        private static JsonArray ParseToJsonArray(object value)
        {
            if (value == null)
                return null;

            var array = new JsonArray();
            var collectionType = value.GetType();
            var isArray = collectionType.IsArray;
            Type elementType;
            if (isArray)
            {
                elementType = collectionType.GetElementType();
            }
            else
            {
                var genericTypes = collectionType.GetGenericArguments();
                elementType = genericTypes[0];
            }

            var enumerable = value as IEnumerable;
            if (IsCustomType(elementType))
            {
                foreach (var item in enumerable)
                {
                    var jsonObject = ParseToJsonObject(item);
                    array.Add(jsonObject);
                }
            }
            else if (IsCollection(elementType))
            {
                foreach (var item in enumerable)
                {
                    var jsonArray = ParseToArray(item);
                    array.Add(jsonArray);
                }
            }
            else
            {
                foreach (var item in enumerable)
                    array.Add(item);
            }

            return array;
        }

        private static JsonObject ParseToJsonObject(object value)
        {
            var type = value.GetType();
            return ParseToJsonObject(value, type);
        }

        private static JsonObject ParseToJsonObject(object value, Type actualType)
        {
            var properties = GetReadableProperties(actualType);
            return ParseToJsonObject(value, properties);
        }

        private static bool IsCustomType(Type type)
        {
            return !(type.IsPrimitive
                || type == typeof(string)
                || type == typeof(DateTime)
                || type.IsEnum);
        }

        private static bool IsCollection(Type type)
        {
            return type.IsArray
                || (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string));
        }


        private static JsonPropertyInfo[] GetReadableProperties<T>()
        {
            var type = typeof(T);
            return GetReadableProperties(type);
        }

        private static JsonPropertyInfo[] GetReadableProperties(Type type)
        {
            if (_readablePropertiesCache.ContainsKey(type))
                return _readablePropertiesCache[type];

            var findProps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var readableProps = findProps
                .Where(prop => Attribute.GetCustomAttribute(prop, typeof(JsonIgnoreAttribute)) == null)
                .Where(prop => prop.CanRead);

            var propInfoList = new List<JsonPropertyInfo>();
            foreach (var readableProp in readableProps)
            {
                var propInfo = JsonPropertyInfo.Parse(readableProp);
                propInfoList.Add(propInfo);
            }

            var propInfos = propInfoList.ToArray();
#if COREWIN
            _readablePropertiesCache.TryAdd(type, propInfos);
#else
            _readablePropertiesCache.Add(type, propInfos);
#endif
            return propInfos;
        }
    }
}
