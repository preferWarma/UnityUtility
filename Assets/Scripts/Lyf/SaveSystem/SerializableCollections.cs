using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Lyf.SaveSystem
{
    public static class AddSerializedJson
    {
        public static void AddAllConverter()
        {
            AddVector3Converter();
        }

        private static void AddVector3Converter()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = { new Vector3Converter() }
            };
        }
    }
    
    public class Vector3Converter : JsonConverter   // 用于将Vector3序列化转换为Json
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (Vector3)value;
            var obj = new JObject
            {
                { "x", vector.x },
                { "y", vector.y },
                { "z", vector.z }
            };
            obj.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var x = (float)obj["x"];
            var y = (float)obj["y"];
            var z = (float)obj["z"];
            return new Vector3(x, y, z);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector3);
        }
    }
}