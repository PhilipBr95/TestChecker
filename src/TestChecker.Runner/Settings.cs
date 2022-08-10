using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using TestChecker.Core.Enums;

namespace TestChecker.Runner
{
    internal partial class Settings
    {
        public string ApiKey { get; private set; }
        public string TestDataJson { get; private set; } = string.Empty;
        public dynamic TestData => Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(TestDataJson ?? String.Empty);
        public Actions Action { get; private set; }
        public string Path { get; private set; }

        public Settings(string path, string apiKey, string testDataJson, Actions actions)
        {
            Path = path;
            ApiKey = apiKey;
            TestDataJson = testDataJson;
            Action = actions;
        }
       
        internal static async Task<Settings> GetSettingsAsync(HttpRequest request)
        {
            var action = request.Query["Action"];
            var apiKey = request.Query["ApiKey"];
            var path = GetTestEndPoint(request.PathBase, request.Path);

            StringValues testDataJson;

            if (request.HasFormContentType)
            {
                request.Form.TryGetValue("TestData", out testDataJson);

                if (string.IsNullOrWhiteSpace(apiKey))
                    request.Form.TryGetValue("ApiKey", out apiKey);
            }
            else
            {
                using (StreamReader stream = new StreamReader(request.Body))
                {
                    testDataJson = await stream.ReadToEndAsync().ConfigureAwait(false);
                }
            }
            
            return new Settings(path, apiKey, testDataJson, GetAction(action));
        }

        internal bool HasTestData()
        {
            return !string.IsNullOrWhiteSpace(TestDataJson);
        }

        private static string GetTestEndPoint(string pathBase, string path)
        {
            var url = $"{pathBase}{path}";
            return Regex.Replace(url, TestEndpointExtensions.REGRESSIONUI_END_POINT, TestEndpointExtensions.REGRESSION_END_POINT, RegexOptions.IgnoreCase);
        }

        private static Actions GetAction(string actionText)
        {
            if(Enum.TryParse<Actions>(actionText, true, out Actions action))
            {
                return action;
            }

            return Actions.RunTests;
        }
    }
}
