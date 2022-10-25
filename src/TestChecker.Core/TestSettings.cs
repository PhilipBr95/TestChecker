using System;
using System.Collections.Generic;
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

        public TestSettings()
        {

        }

        public TestSettings(string path, string apiKey, string testDataJson, Actions actions)
        {
            Path = path;
            ApiKey = apiKey;
            TestDataJson = testDataJson;
            Action = actions;
        }
        public TestSettings(Actions actions)
        {
            Action = actions;
        }

        public bool HasTestData()
        {
            return !string.IsNullOrWhiteSpace(TestDataJson);
        }
    }
}
