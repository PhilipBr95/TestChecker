using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core.Serialisation;
using TestChecker.Core.Enums;
using TestChecker.Core.ContractResolver;

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

        public async Task<TestCheckSummary> RunTestAsync(Actions action, string apiKey, string payload)
        {

            var jsonResult = await _httpClient.PostAsync($"{BaseUrl}/test?action={action}&apikey={apiKey}", payload).ConfigureAwait(false);

            var settings = new JsonSerializerSettings
            {                
                Converters = new List<JsonConverter> { new CoverageConverter() }
            };

            var result = JsonConvert.DeserializeObject<TestCheckSummary>(jsonResult, settings);
            //return result?.GetTestChecks();
            return result;
        }
    }
}
