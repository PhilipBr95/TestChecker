#if NETFRAMEWORK

using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestChecker.Runner
{
    internal partial class Settings
    {
        internal static async Task<Settings> GetSettingsAsync(IOwinRequest request)
        {
            var action = request.Query["Action"];
            var apiKey = request.Query["ApiKey"];
            var path = GetTestEndPoint(request.PathBase.Value, request.Path.Value);

            var formData = await request.ReadFormAsync().ConfigureAwait(false);
            var testDataJson = formData.Get("TestData");

            if (string.IsNullOrWhiteSpace(apiKey))
                apiKey = formData.Get("ApiKey");

            return new Settings(path, apiKey, testDataJson, GetAction(action));
        }
    }
}

#endif