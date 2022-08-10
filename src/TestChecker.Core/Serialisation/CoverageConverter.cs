using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestChecker.Core.Serialisation
{
    internal class CoverageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Coverage).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            var c = new Coverage();
            serializer.Populate(jObject.CreateReader(), c);
            return c;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}