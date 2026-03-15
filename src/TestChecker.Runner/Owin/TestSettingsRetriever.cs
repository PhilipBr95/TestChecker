#if NETFRAMEWORK

using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestChecker.Core;
using System.Linq;

namespace TestChecker.Runner
{
    internal partial class TestSettingsRetriever
    {
        internal static async Task<TestSettings> GetSettingsAsync(IOwinRequest request)
        {
            var action = request.Query["Action"];
            var apiKey = request.Query["ApiKey"];
            var useUIString = request.Query["UseUI"];            

            var path = GetTestEndPoint(request.PathBase.Value, request.Path.Value);

            var formData = await request.ReadFormAsync().ConfigureAwait(false);

            var testMethods = formData.GetValues("TestMethods");
            var testDataJson = formData.Get("TestData");
            
            if (string.IsNullOrWhiteSpace(apiKey))
                apiKey = formData.Get("ApiKey");

            if (string.IsNullOrWhiteSpace(action))
                action = formData.Get("Action");

            if (string.IsNullOrWhiteSpace(useUIString))
                useUIString = formData.Get("UseUI");

            bool.TryParse(useUIString, out bool useUI);
            return new TestSettings(path, apiKey, testDataJson, GetAction(action, request.Path.Value), testMethods?.ToArray()) { UseUI = useUI };
        }
    }
}

#endif