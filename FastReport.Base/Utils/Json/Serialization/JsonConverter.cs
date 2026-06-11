using System;
using System.Text;

namespace FastReport.Utils.Json.Serialization
{
    /// <summary>
    /// Contains JSON converter utility methods.
    /// </summary>
    public static partial class JsonConverter
    {
        /// <summary>
        /// Deserializes JSON string.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>The T instance.</returns>
        public static T Deserialize<T>(string json)
        {
            return JsonDeserializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Deserializes JSON string.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <param name="type">The type.</param>
        /// <returns>The type instance.</returns>
        public static object Deserialize(string json, Type type)
        {
            return JsonDeserializer.Deserialize(json, type);
        }

        /// <summary>
        /// Serializes the type instance to a string.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="instance">The type instance.</param>
        /// <returns>JSON string.</returns>
        public static string Serialize<T>(T instance)
        {
            return JsonSerializer.Serialize(instance);
        }

        /// <summary>
        /// Serializes the type instance to a byte array.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="instance">The type instance.</param>
        /// <returns>The byte array.</returns>
        public static byte[] SerializeToBytes<T>(T instance)
        {
            var strContent = Serialize(instance);
            byte[] bytes = Encoding.UTF8.GetBytes(strContent);
            return bytes;
        }
    }
}