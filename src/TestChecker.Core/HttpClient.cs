using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestChecker.Core
{
    public class HttpClient : IHttpClient
    {
        public async Task<string> GetAsync(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var json = await client.GetStringAsync($"{url}");
                return json;
            }
        }

        public async Task<string> PostAsync(string url, string payload)
        {
            var json = await PostStreamAsync($"{url}", payload);
            return json;
        }

        private static async Task<string> PostStreamAsync(string url, string payload)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Content = new StringContent(payload);

                    using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseContentRead)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to {url}", ex);
            }
        }
    }
}
