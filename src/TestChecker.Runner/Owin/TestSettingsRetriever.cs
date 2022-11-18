#if NETFRAMEWORK

using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestChecker.Core;

namespace TestChecker.Runner
{
    internal partial class TestSettingsRetriever
    {
        internal static async Task<TestSettings> GetSettingsAsync(IOwinRequest request)
        {
            var action = request.Query["Action"];
            var apiKey = request.Query["ApiKey"];
            var path = GetTestEndPoint(request.PathBase.Value, request.Path.Value);

            var formData = await request.ReadFormAsync().ConfigureAwait(false);

            var testMethodsData = formData.Get("TestMethods");
            var testMethods = string.IsNullOrWhiteSpace(testMethodsData) ? default : testMethodsData.Split(',');
            var testDataJson = formData.Get("TestData");

            if (string.IsNullOrWhiteSpace(apiKey))
                apiKey = formData.Get("ApiKey");

            if (string.IsNullOrWhiteSpace(action))
                action = formData.Get("Action");

            return new TestSettings(path, apiKey, testDataJson, GetAction(action, request.Path.Value), testMethods);
        }
    }
}

#endif