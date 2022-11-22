using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
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
        private VersionInfo _versionInfo = null;

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

            _testCheckResult = new TestCheckSummary { System = new SystemInfo { Name = $"{sericeName} has not been implemented" } };
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
       
        public async Task<T> RunTestActionAsync<T>(TestSettings testSettings)
        {            
            try
            {
                var versionInfo = testSettings.Action == Actions.GetVersion ? null : await GetVersionInfoAsync();
                return await Service.RunTestAsync<T>(testSettings, versionInfo).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                throw new Exception($"Unknown error with {Service?.GetType().FullName}", ex);
            }
        }

        //public async Task<TestCheckSummary> RunTestAsync(TestSettings testSettings)
        //{
        //    //todo - Has to be dynamic??
        //    //todo - Has to be dynamic??
        //    //todo - Has to be dynamic??
        //    //todo - Has to be dynamic??

        //    //Replace with the one above
        //    //Replace with the one above
        //    //Replace with the one above



        //    if (_isTestCheckStatic) return _testCheckResult;

        //    try
        //    {
        //        return await Service.RunTestAsync<TestCheckSummary>(testSettings, await GetVersionInfoAsync()).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new TestCheckSummary { System = TestCheckSummary.GetSystemString(Service.GetType().Assembly, Service.BaseUrl), ReadTestChecks = new TestCheck(null, ex) };
        //    }
        //}

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

        public async Task<VersionInfo> GetVersionInfoAsync()
        {
            if (_versionInfo == null)
            {
                var versionSettings = new TestSettings(new Uri(new Uri(Service.BaseUrl), TestEndpointExtensions.TEST_END_POINT).ToString(), Core.Enums.Actions.GetVersion);
                _versionInfo = await RunTestActionAsync<VersionInfo>(versionSettings);
            }

            return _versionInfo;
        }
    }
}