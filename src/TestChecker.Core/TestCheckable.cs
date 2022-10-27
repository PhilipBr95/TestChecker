using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core.Serialisation;
using TestChecker.Core.Enums;
using TestChecker.Core.ContractResolver;
using System;
using System.IO;

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

        public async Task<T> RunTestAsync<T>(TestSettings testSettings)
        {
            var jsonResult = await _httpClient.PostAsync($"{BaseUrl}/test?action={testSettings.Action}&apikey={testSettings.ApiKey}", JsonSerialiser.Serialise(testSettings)).ConfigureAwait(false);

            var jsonSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new CoverageConverter() }
            };

            var result = JsonConvert.DeserializeObject<T>(jsonResult, jsonSettings);
            return result;
        }

        public static List<T> DeserializeSingleOrList<T>(JsonReader jsonReader)
        {
            if (jsonReader.Read())
            {
                switch (jsonReader.TokenType)
                {
                    case JsonToken.StartArray:
                        return new JsonSerializer().Deserialize<List<T>>(jsonReader);

                    case JsonToken.StartObject:
                        var instance = new JsonSerializer().Deserialize<T>(jsonReader);
                        return new List<T> { instance };
                }
            }

            throw new InvalidOperationException("Unexpected JSON input");
        }
    }
}
