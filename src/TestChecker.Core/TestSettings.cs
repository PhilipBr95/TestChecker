using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestChecker.Core.Enums;

namespace TestChecker.Core
{
    public class TestSettings
    {
        public string ApiKey { get; set; }
        public string TestDataJson { get; set; } = string.Empty;
        public dynamic TestData => Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(TestDataJson ?? String.Empty);
        public Actions Action { get; set; }
        public string Path { get; set; }
        public string[] TestMethods { get; set; }
        public static TestSettings DefaultTestSettings { get; set; }

        public TestSettings()
        {

        }

        public TestSettings(string path, string apiKey, string testDataJson, Actions actions, string[] testMethods)
        {
            Path = path;
            ApiKey = apiKey;
            TestDataJson = testDataJson;
            Action = actions;
            TestMethods = testMethods;
        }
        public TestSettings(string path, Actions actions)
        {
            Path = path;
            Action = actions;
        }

        public bool HasTestData() => !string.IsNullOrWhiteSpace(TestDataJson);
        public bool HasTestMethods(string assemblyName)
        {
            var tests = TestMethods?.Where(w => w.StartsWith(assemblyName));
            return tests?.Any() ?? true;
        }       
    }
}
