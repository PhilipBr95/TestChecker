using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestChecker.Core.ContractResolver;

namespace TestChecker.Core.Serialisation
{
    public class Serialiser
    {
        private int _maxDepth;

        public Serialiser(int maxDepth)
        {
            _maxDepth = maxDepth;
        }

        public string SerializeObject(object obj, JsonSerializerSettings jsonSerializerSettings)
        {
            using (var strWriter = new StringWriter())
            {
                using (var jsonWriter = new DepthJsonTextWriter(strWriter))
                {
                    bool include() => jsonWriter.CurrentDepth <= _maxDepth;
                    var resolver = new DepthContractResolver(include);                    

                    var serializer = new JsonSerializer { ContractResolver = resolver, PreserveReferencesHandling = jsonSerializerSettings.PreserveReferencesHandling, NullValueHandling = jsonSerializerSettings.NullValueHandling, ReferenceLoopHandling = jsonSerializerSettings.ReferenceLoopHandling };
                    serializer.Serialize(jsonWriter, obj);
                }
                return strWriter.ToString();
            }
        }
    }
}
