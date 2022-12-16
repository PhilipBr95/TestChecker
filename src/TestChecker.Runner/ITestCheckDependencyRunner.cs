using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core;

namespace TestChecker.Runner
{
    internal interface ITestCheckDependencyRunner
    {
        List<ITestCheckDependency> Dependencies { get; }
        Task<List<T>> RunTestActionAsync<T>(TestSettings settings) where T : new();
        Task<List<NamedTestData>> GetTestDataAsync();
        Task<List<VersionInfo>> GetVersionInfoAsync();
    }
}