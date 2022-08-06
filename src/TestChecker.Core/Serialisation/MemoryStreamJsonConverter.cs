using Newtonsoft.Json;
using System;
using System.IO;

namespace TestChecker.Core.Serialisation
{
    public class MemoryStreamJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(System.IO.Stream).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var bytes = serializer.Deserialize<byte[]>(reader);
            return bytes != null ? new MemoryStream(bytes) : new MemoryStream();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var bytes = ((MemoryStream)value).ToArray();
            serializer.Serialize(writer, bytes);
        }
    }
}
