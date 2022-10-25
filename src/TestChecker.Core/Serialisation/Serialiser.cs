using System;
using System.Collections.Generic;
using System.Text;

namespace TestChecker.Core.Serialisation
{
    public static class Serialiser
    {
        public static string Serialise(object obj) => Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        
        public static T Deserialise<T>(string json) => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
    }
}
