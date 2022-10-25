using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core.Enums;

namespace TestChecker.Core
{
    public interface ITestCheckDependency
    {
        ITestCheckable Service { get; }        
        Task<T> RunTestActionAsync<T>(TestSettings testSettings);
        Task<TestCheckSummary> RunTestAsync(TestSettings testSettings);
        Task<List<NamedTestData>> GetTestDataAsync();
    }
}