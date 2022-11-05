using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace TestChecker.Runner.Extensions
{
    public static class HttpRequestExtensions
    {
        [DebuggerStepThrough]
        public static string GetUrl(this HttpRequest httpRequest)
        {
            return $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.Path}{httpRequest.QueryString}";
        }
    }
}
