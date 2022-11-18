using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TestChecker.Core.Serialisation.Converters
{
    internal class TestCheckSummaryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(TestCheckSummary).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            
            var depChecks = jObject.SelectToken("$.DependencyTestChecks");

            if(depChecks != null && depChecks.Type != JTokenType.Array)
            {
                var deps = new List<TestCheckSummary>();
                var dep = jObject["DependencyTestChecks"].ToObject<TestCheck>();
                deps.Add(new TestCheckSummary { System = new SystemInfo { Name = "Old Tests" }, ReadTestChecks = dep });

                //v1 - Not the best
                var summary = new TestCheckSummary
                {
                    DependencyTestChecks = deps,
                    ReadTestChecks = jObject["ReadTestChecks"].ToObject<TestCheck>(),
                    Success = jObject["Success"].Value<bool>(),
                    System = SystemInfo.GenerateFrom(jObject["System"].Value<string>()),
                    TestCoverage = jObject["TestCoverage"].ToObject<Coverage>(),
                    TestData = null,    //Not the best
                    TestDate = jObject["TestDate"].Value<string>(),
                    WriteTestChecks = jObject["WriteTestChecks"].ToObject<TestCheck>(),
                };

                return summary;
            }
            else
            {
                //v2                
                var summary = new TestCheckSummary();
                serializer.Populate(jObject.CreateReader(), summary);
                return summary;
            }
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
