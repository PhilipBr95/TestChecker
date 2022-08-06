using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core.Enums;

namespace TestChecker.Core
{
    public interface ITestCheckable
    {
        Task<TestCheckSummary> RunTestAsync(Actions action, string apiKey, string json);

        Task<List<NamedTestData>> GetTestDataAsync();
    }
}
