using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FastReport.Utils.Json.Serialization
{
    internal static class JsonDeserializer
    {
        private static readonly Dictionary<Type, ConstructorInfo> _ctorsCache = new Dictionary<Type, ConstructorInfo>();
        private static readonly Dictionary<Type, JsonPropertyInfo[]> _writablePropertiesCache =
            new Dictionary<Type, JsonPropertyInfo[]>();


        public static T Deserialize<T>(string json)
        {
            var instance = CreateInstance<T>();
            DeserializeProperties(instance, json);
            return instance;
        }

        public static object Deserialize(string json, Type type)
        {
            var instance = CreateInstance(type);
            DeserializeProperties(instance, json);
            return instance;
        }

        private static object Deserialize(JsonBase jsonBase, Type type)
        {
            var instance = CreateInstance(type);

            DeserializeProperties(instance, jsonBase, type);
            return instance;
        }

        private static void DeserializeProperties<T>(T instance, string json)
        {
            var jsonBase = JsonBase.FromString(json);
            DeserializeProperties(instance, jsonBase);
        }

        private static void DeserializeProperties<T>(T instance, JsonBase jsonBase)
        {
            var properties = GetWritableProperties<T>();

            DeserializeProperties(instance, jsonBase, properties);
        }

        private static void DeserializeProperties(object instance, JsonBase jsonBase, Type type)
        {
            var properties = GetWritableProperties(type);

            DeserializeProperties(instance, jsonBase, properties);
        }

        private static void DeserializeProperties(object instance, JsonBase jsonBase, JsonPropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                var propInfo = property.Info;
                var propName = property.Name;
                var obj = jsonBase[propName];

                obj = CastToType(obj, propInfo.PropertyType);

#if COREWIN || CROSSPLATFORM || MONO // .Net 4.5 or greater
                propInfo.SetValue(instance, obj);
#else
                propInfo.GetSetMethod(true)
                    .Invoke(instance, BindingFlags.SetProperty, null, new[] { obj }, null);
#endif
            }
        }

        private static object ConvertCollection(object obj, Type collectionType)
        {
            if (obj is JsonArray jsonArray)
            {
                bool isArray;
                Type elementType = GetElementType(collectionType, out isArray);
                if (isArray)
                {
                    IList collection = Array.CreateInstance(elementType, jsonArray.Count);
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        var item = jsonArray[i];
                        var converted = CastToType(item, elementType);
                        collection[i] = converted;
                    }
                    return collection;
                }
                else
                {
                    var genericTypes = collectionType.GetGenericArguments();
                    var listType = typeof(List<>).MakeGenericType(genericTypes);
                    var collection = (IList)Activator.CreateInstance(listType);

                    foreach (var item in jsonArray)
                    {
                        var converted = CastToType(item, elementType);
                        collection.Add(converted);
                    }
                    return collection;
                }

            }
            throw new Exception("Unknown array type " + obj.GetType().FullName);
        }

        private static object CastToType(object value, Type expectedType)
        {
            if (value == null)
                return null;

            object convertedValue;
            if (expectedType.IsPrimitive)
            {
                convertedValue = Convert.ChangeType(value, expectedType);
            }
            else if (expectedType == typeof(string))
            {
                convertedValue = value.ToString();
            }
            else if (expectedType == typeof(DateTime))
            {
                convertedValue = DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture);
            }
            else if (expectedType.IsArray || typeof(IEnumerable).IsAssignableFrom(expectedType))
            {
                var actualType = value.GetType();
                var elemType = GetElementType(expectedType, out bool isArray);
                // BASE 64 check
                if (elemType == typeof(byte) && actualType == typeof(string))
                    convertedValue = Convert.FromBase64String(value.ToString());
                else
                    convertedValue = ConvertCollection(value, expectedType);
            }
            else if (expectedType.IsEnum)
            {
                convertedValue = Enum.Parse(expectedType, value.ToString(), true);
            }
            else // Custom type
            {
                if (value is JsonBase jsonBase)
                {
                    convertedValue = Deserialize(jsonBase, expectedType);
                }
                else
                    throw new Exception("Unknown type" + expectedType.FullName);
            }

            return convertedValue;
        }

        private static Type GetElementType(Type collection, out bool isArray)
        {
            isArray = collection.IsArray;
            if (isArray)
            {
                return collection.GetElementType();
            }
            else // IEnumerable, IEnumerable<>, ICollection ...
            {
                var genArgs = collection.GetGenericArguments();
                if (genArgs == null)
                    return typeof(object);

                return genArgs[0];
            }
        }

        private static T CreateInstance<T>()
        {
            var type = typeof(T);

            var instance = (T)CreateInstance(type);
            return instance;
        }

        private static object CreateInstance(Type type)
        {
            var ctor = GetConstructor(type);
            var instance = ctor.Invoke(null);
            return instance;
        }


        private static ConstructorInfo GetConstructor(Type type)
        {
            if (_ctorsCache.ContainsKey(type))
                return _ctorsCache[type];

            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            // Trying to find a parameterless ctor
            var availableCtor = ctors.Where(_ctor => _ctor.GetParameters().Length == 0).ToArray();
            if (availableCtor.Length == 0)
                throw new Exception("There are no available public parameterless constructors for this type");

            var ctor = availableCtor[0];
#if COREWIN
            _ctorsCache.TryAdd(type, ctor);
#else
            _ctorsCache.Add(type, ctor);
#endif
            return ctor;
        }

        private static JsonPropertyInfo[] GetWritableProperties<T>()
        {
            var type = typeof(T);
            return GetWritableProperties(type);
        }

        private static JsonPropertyInfo[] GetWritableProperties(Type type)
        {
            if (_writablePropertiesCache.ContainsKey(type))
                return _writablePropertiesCache[type];

            var findProps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var writableProps = findProps
                .Where(prop => Attribute.GetCustomAttribute(prop, typeof(JsonIgnoreAttribute)) == null)
                .Where(prop => prop.CanWrite).ToArray();

            var propInfoList = new List<JsonPropertyInfo>();
            foreach (var writableProp in writableProps)
            {
                var propInfo = JsonPropertyInfo.Parse(writableProp);
                propInfoList.Add(propInfo);
            }

            var propInfos = propInfoList.ToArray();
#if COREWIN
            _writablePropertiesCache.TryAdd(type, propInfos);
#else
            _writablePropertiesCache.Add(type, propInfos);
#endif
            return propInfos;
        }

    }
}
