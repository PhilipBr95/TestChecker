using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core;

namespace TestChecker.Runner
{
    internal class TestCheckDependencyRunner
    {
        private List<ITestCheckDependency> _dependencies;
        private ILogger<TestCheckDependencyRunner> _logger;

        public TestCheckDependencyRunner(List<ITestCheckDependency> dependencies, ILogger<TestCheckDependencyRunner> logger)
        {
            _dependencies = dependencies;
            _logger = logger;
        }

        internal async Task<List<T>> RunTestActionAsync<T>(TestSettings settings) where T : new()
        {             
            if (_dependencies == null) return default;

            var taskResults = new List<T>();

            foreach (var dependency in _dependencies)
            {
                _logger?.LogDebug($"{nameof(dependency.RunTestAsync)} called on {dependency.Service} with {settings.TestDataJson}");

                var taskResult = await dependency.RunTestActionAsync<T>(settings).ConfigureAwait(false);
                taskResults.Add(taskResult);
            }
            
            return taskResults;
        }

        internal async Task<List<NamedTestData>> GetTestDataAsync()
        {
            var testData = new List<NamedTestData>();

            if (_dependencies == null) return testData;

            foreach (var dependency in _dependencies)
            {
                _logger?.LogDebug($"{nameof(dependency.GetTestDataAsync)} called on {dependency.Service}");

                var dependencyTestData = await dependency.GetTestDataAsync().ConfigureAwait(false);

                if (dependencyTestData != null)
                {
                    testData.AddRange(dependencyTestData);
                }
            }

            return testData;
        }
    }
}