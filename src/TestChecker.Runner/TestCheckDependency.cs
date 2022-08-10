using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core;
using TestChecker.Core.Enums;

namespace TestChecker.Runner
{
    public class TestCheckDependency : ITestCheckDependency
    {
        private bool _isTestCheckStatic = false;
        private bool _hasTestCheckRun = false;
        private TestCheckSummary _testCheckResult;

        /// <summary>
        /// Constructs the <see cref="TestCheckDependency"/>.
        /// </summary>
        /// <param name="service">The service to be checked.</param>
        /// <param name="description">A description of the service being checked.</param>
        public TestCheckDependency(ITestCheckable service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private TestCheckDependency(string sericeName)
        {
            _isTestCheckStatic = true;
            _hasTestCheckRun = true;

            _testCheckResult = new TestCheckSummary { System = $"{sericeName} has not been implemented" };
        }

        public static TestCheckDependency NotImplemented(string sericeName)
        {
            return new TestCheckDependency(sericeName);
        }

        /// <summary>
        /// The service to be checked.
        /// </summary>
        public ITestCheckable Service { get; }

        /// <summary>
        /// The result of the diagnostic check.
        /// </summary>
        public TestCheckSummary Result
        {
            get
            {
                return _hasTestCheckRun
                    ? _testCheckResult
                    : throw new InvalidOperationException($"The test must be executed before checking the result.");
            }
        }
       
        public async Task<TestCheckSummary> RunTestAsync(Actions action, string apiKey, string json)
        {
            if (_isTestCheckStatic) return _testCheckResult;

            try
            {
                return await Service.RunTestAsync(action, apiKey, json).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                return new TestCheckSummary { System = TestCheckSummary.GetSystemString(Service.GetType().Assembly, Service.BaseUrl), ReadTestChecks = new TestCheck(null, ex) };
            }
        }

        public async Task<List<NamedTestData>> GetTestDataAsync()
        {
            try
            {
                if (Service == null)
                    return null;

                return await Service.GetTestDataAsync().ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                return new List<NamedTestData> { new NamedTestData { FullName = Service?.GetType().FullName, TestData = ex.ToString() } };
            }
        }
    }
}