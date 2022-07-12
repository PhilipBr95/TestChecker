using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core.Enums;

namespace TestChecker.Core
{
    public interface ITestCheckDependency
    {
        ITestCheckable Service { get; }        
        Task<TestCheckSummary> RunTestAsync(Actions action, string apiKey, string json);
        Task<List<NamedTestData>> GetTestDataAsync();
    }
}