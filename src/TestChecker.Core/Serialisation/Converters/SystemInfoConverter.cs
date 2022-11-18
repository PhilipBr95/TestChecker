using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TestChecker.Core.Serialisation.Converters
{
    public class SystemInfoConverter : JsonConverter<SystemInfo>
    {
        public override SystemInfo ReadJson(JsonReader reader, Type objectType, SystemInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = (reader as JTokenReader).CurrentToken;            

            if(token.HasValues)
            { 
                JObject jObject = JObject.Load(reader);
                
                var systemInfo = new SystemInfo();
                serializer.Populate(jObject.CreateReader(), systemInfo);
                return systemInfo;
            }
            else
            {
                //Old json
                return new SystemInfo { Name = token.ToString() };
            }
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, SystemInfo value, JsonSerializer serializer)
        {
            //Not required
            throw new NotImplementedException();
        }
    }
}
