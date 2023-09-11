using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FastReport.Utils.Json.Serialization
{

    public static partial class JsonConverter
    {

        public static T Deserialize<T>(string json)
        {
            return JsonDeserializer.Deserialize<T>(json);
        }

        public static object Deserialize(string json, Type type)
        {
            return JsonDeserializer.Deserialize(json, type);
        }

        public static string Serialize<T>(T instance)
        {
            return JsonSerializer.Serialize(instance);
        }

        public static byte[] SerializeToBytes<T>(T instance)
        {
            var strContent = Serialize(instance);
            byte[] bytes = Encoding.UTF8.GetBytes(strContent);
            return bytes;
        }
    }
}