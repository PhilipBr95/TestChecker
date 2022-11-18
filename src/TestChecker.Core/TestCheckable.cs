using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core.Serialisation;
using TestChecker.Core.Enums;
using System;
using System.IO;
using TestChecker.Core.Serialisation.Converters;

namespace TestChecker.Core
{
    public class TestCheckable : ITestCheckable
    {
        public string BaseUrl { get; private set; }
        private readonly IHttpClient _httpClient;

        public TestCheckable(string baseUrl)
        {
            BaseUrl = baseUrl;
            _httpClient = new HttpClient();
        }

        public TestCheckable(string baseUrl, IHttpClient httpClient)
        {
            BaseUrl = baseUrl;
            _httpClient = httpClient;
        }

        public async Task<List<NamedTestData>> GetTestDataAsync()
        {
            var json = await _httpClient.GetAsync($"{BaseUrl}/testdata").ConfigureAwait(false);
            
            var result = JsonConvert.DeserializeObject<List<NamedTestData>>(json);
            return result;
        }

        public async Task<T> RunTestAsync<T>(TestSettings testSettings, VersionInfo versionInfo)
        {
            try
            {
                string payload = GetPayload(testSettings, versionInfo);

                var jsonResult = await _httpClient.PostAsync($"{BaseUrl}/test?action={testSettings.Action}&apikey={testSettings.ApiKey}", payload).ConfigureAwait(false);

                var jsonSettings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new CoverageConverter() }
                };

                var result = JsonConvert.DeserializeObject<T>(jsonResult, jsonSettings);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static string GetPayload(TestSettings testSettings, VersionInfo versionInfo)
        {
            if(versionInfo?.HasVersion() == true)
                return JsonSerialiser.Serialise(testSettings);

            //For backwards compat...
            if (testSettings.Action == Actions.GetVersion)
                return String.Empty;

            //More backwards compat...
            return testSettings.TestDataJson;
        }
    }
}
