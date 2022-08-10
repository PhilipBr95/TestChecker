using System.Threading.Tasks;

namespace TestChecker.Core
{
    public interface IHttpClient
    {
        Task<string> GetAsync(string url);
        Task<string> PostAsync(string url, string payload);
    }
}