using Microsoft.AspNetCore.Http;

namespace TestChecker.Runner.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetUrl(this HttpRequest httpRequest)
        {
            return $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.Path}{httpRequest.QueryString}";
        }
    }
}
