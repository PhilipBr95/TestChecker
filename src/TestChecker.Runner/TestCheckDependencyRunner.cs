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

        internal async Task<List<TestCheckSummary>> RunTestsAsync(Settings settings)
        {             
            if (_dependencies == null) return null;

            var testChecks = new List<TestCheckSummary>();

            foreach (var dependency in _dependencies)
            {
                _logger?.LogDebug($"{nameof(dependency.RunTestAsync)} called on {dependency.Service} with {settings.TestDataJson}");

                var testCheckSummary = await dependency.RunTestAsync(settings.Action, settings.ApiKey, settings.TestDataJson).ConfigureAwait(false);
                testChecks.Add(testCheckSummary);
            }
            
            return testChecks;
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