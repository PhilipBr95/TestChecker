using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

//[assembly: InternalsVisibleTo("TestChecker.Runner.Tests")]
namespace TestChecker.Core.Serialisation
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
                deps.Add(new TestCheckSummary { System = "Old Tests", ReadTestChecks = dep });

                //v1 - Not the best
                var summary = new TestCheckSummary
                {
                    DependencyTestChecks = deps,
                    Environment = jObject["Environment"].Value<string>(),
                    ReadTestChecks = jObject["ReadTestChecks"].ToObject<TestCheck>(),
                    Success = jObject["Success"].Value<bool>(),
                    System = jObject["System"].Value<string>(),
                    TestCoverage = jObject["TestCoverage"].ToObject<Coverage>(),
                    TestData = null,    //Not the best
                    TestDate = jObject["TestDate"].Value<string>(),
                    Version = jObject["Version"].Value<string>(),
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
