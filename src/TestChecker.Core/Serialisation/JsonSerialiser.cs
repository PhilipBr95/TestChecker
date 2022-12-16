using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TestChecker.Core.Serialisation.Converters;

namespace TestChecker.Core.Serialisation
{
    public static class JsonSerialiser
    {
        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new ShouldSerializeContractResolver(),
            Converters = new List<JsonConverter> { new MemoryStreamJsonConverter() }
        };

        public static string Serialise(object obj) => Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, _jsonSettings);
        
        public static T Deserialise<T>(string json) => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
    }
}
