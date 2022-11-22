using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TestChecker.Core.Serialisation.Converters
{
    public class SystemInfoConverter : JsonConverter<SystemInfo>
    {
        public override SystemInfo ReadJson(JsonReader reader, Type objectType, SystemInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                //Old json

                return SystemInfo.CreateFrom(reader.Value.ToString());
            }

            var systemInfo = new SystemInfo();
            serializer.Populate(reader, systemInfo);
            return systemInfo;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, SystemInfo value, JsonSerializer serializer)
        {
            //Not required
            throw new NotImplementedException();
        }
    }
}
