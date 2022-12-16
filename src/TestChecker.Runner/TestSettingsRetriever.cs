using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using TestChecker.Core;
using TestChecker.Core.Enums;

namespace TestChecker.Runner
{
    internal partial class TestSettingsRetriever
    {      
        internal static async Task<TestSettings> GetSettingsAsync(HttpRequest request)
        {
            var action = request.Query["Action"];
            var apiKey = request.Query["ApiKey"];
            var useUIString = request.Query["UseUI"];            
            bool useUI = false;
            var path = GetTestEndPoint(request.PathBase, request.Path);          

            if (request.HasFormContentType)
            {
                //Used on the TestUI form
                
                request.Form.TryGetValue("TestMethods", out StringValues testMethods);
                request.Form.TryGetValue("TestData", out StringValues testDataJson);
                request.Form.TryGetValue("UseUI", out useUIString);

                if (string.IsNullOrWhiteSpace(apiKey))
                    request.Form.TryGetValue("ApiKey", out apiKey);

                if (string.IsNullOrWhiteSpace(action))
                    request.Form.TryGetValue("Action", out action);

                bool.TryParse(useUIString, out useUI);
                return new TestSettings(path, apiKey, testDataJson, GetAction(action, request.Path), testMethods) { UseUI = useUI };
            }
            else
            {
                using (StreamReader stream = new StreamReader(request.Body))
                {
                    var payloadSettings = await stream.ReadToEndAsync().ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(payloadSettings))
                        return JsonConvert.DeserializeObject<TestSettings>(payloadSettings);
                }
            }

            bool.TryParse(useUIString, out useUI);
            return new TestSettings(path, GetAction(action, request.Path)) { ApiKey = apiKey, UseUI = useUI };
        }        

        private static string GetTestEndPoint(string pathBase, string path)
        {
            var url = $"{pathBase}{path}";
            return Regex.Replace(url, TestEndpointExtensions.TESTUI_END_POINT, TestEndpointExtensions.TEST_END_POINT, RegexOptions.IgnoreCase);
        }

        private static Actions GetAction(string actionText, string path)
        {
            if(Enum.TryParse<Actions>(actionText, true, out Actions action))
            {
                return action;
            }
            else
            {
                //Backwards compatability

                if(path.IndexOf(TestEndpointExtensions.TESTDATA_END_POINT, StringComparison.OrdinalIgnoreCase) >= 0)
                    return Actions.GetTestData;

                if (path.IndexOf(TestEndpointExtensions.TESTUI_END_POINT, StringComparison.OrdinalIgnoreCase) >= 0)
                    return Actions.RunTests;
            }

            return Actions.RunTests;
        }
    }
}
