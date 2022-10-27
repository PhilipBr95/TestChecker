using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestChecker.Core;
using TestChecker.Core.Enums;
using TestChecker.Runner.Extensions;

//[assembly: InternalsVisibleTo("TestChecker.Runner.Tests, PublicKey=4c452e3bb169baa8")]
namespace TestChecker.Runner
{
    internal class TestRunner<TData> where TData : class
    {
        private readonly Assembly _assembly;
        private readonly List<ITestCheckDependency> _dependencies;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly string _readApiKey;
        private readonly string _readWriteApiKey;
        private readonly Func<ITestChecks<TData>> _testChecks;
        
        public TestRunner(Assembly assembly, List<ITestCheckDependency> dependencies, Func<ITestChecks<TData>> testChecks, ILoggerFactory loggerFactory, string readApiKey, string readWriteApiKey)
        {
            _assembly = assembly;
            _dependencies = dependencies;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory?.CreateLogger<TestRunner<TData>>();
            _readApiKey = readApiKey;
            _readWriteApiKey = readWriteApiKey;

            _testChecks = testChecks;            
        }

        public async Task<dynamic> HandleRequestAsync(TestSettings settings, string url)
        {
            ITestChecks<TData> testChecks = _testChecks();            

            if (settings.HasTestData())
            {
                var data = RetrieveTestData(settings.TestData);
                _logger?.LogDebug($"{nameof(testChecks.SetTestData)} called with {data}");

                testChecks.SetTestData(data);
            }

            if (settings.Action.HasFlag(Actions.GetTestData))
            {
                _logger?.LogDebug($"{nameof(testChecks.GetTestData)} called");
                return testChecks.GetTestData();
            }

            if (settings.Action.HasFlag(Actions.GetVersion))
            {
                _logger?.LogDebug($"{nameof(VersionInfo)} called");

                var myVersion = new VersionInfo();

                if (_dependencies != null)
                {
                    var dependencyVersions = await new TestCheckDependencyRunner(_dependencies, _loggerFactory?.CreateLogger<TestCheckDependencyRunner>())
                        .RunTestActionAsync<VersionInfo>(settings)
                        .ConfigureAwait(false);

                    myVersion.AddDependencies(dependencyVersions);
                }
                                
                return myVersion;
            }

            TestCheck readTestChecks = null;
            TestCheck writeTestChecks = null;            

            if (AreTestsAllowed(settings, _readApiKey, _readWriteApiKey))
            {
                if (settings.Action.HasRunReadTests())
                {
                    _logger?.LogDebug($"{nameof(testChecks.RunReadTestsAsync)} called");

                    try
                    {
                        readTestChecks = await testChecks.RunReadTestsAsync(settings.Action.HasGetNames()).ConfigureAwait(false);
                    }
                    catch (NotImplementedException notEx)
                    {
                        var message = $"{nameof(testChecks.RunReadTestsAsync)} not implemented by {testChecks.GetType().FullName}";
                        _logger?.LogDebug(message);
                    }
                }
            }
            else
            {
                readTestChecks = new TestCheck($"Your ApiKey does not match the Read or Write ApiKey!", null, false);
            }

            if (AreTestsAllowed(settings, _readWriteApiKey) && settings.Action.HasFlag(Actions.RunWriteTests))
            {
                _logger?.LogDebug($"{nameof(testChecks.RunWriteTestsAsync)} called");

                try
                {
                    writeTestChecks = await testChecks.RunWriteTestsAsync().ConfigureAwait(false);
                }
                catch(NotImplementedException notEx)
                {
                    var message = $"{nameof(testChecks.RunWriteTestsAsync)} not implemented by {testChecks.GetType().FullName}";
                    _logger?.LogDebug(message);

                    writeTestChecks = null;
                }
            }

            List<TestCheckSummary> dependencyTestChecks = null;

            if (_dependencies != null)             
                dependencyTestChecks = await new TestCheckDependencyRunner(_dependencies, _loggerFactory?.CreateLogger<TestCheckDependencyRunner>())
                    .RunTestActionAsync<TestCheckSummary>(settings)
                    .ConfigureAwait(false);

            var coverages = new List<Coverage> { readTestChecks?.Coverage, writeTestChecks?.Coverage };

            if(dependencyTestChecks != null)
                coverages.AddRange(dependencyTestChecks.GetCoverages());

            var coverage = new Coverage(coverages);
            bool? success = null;

            if (readTestChecks?.Success != null || writeTestChecks?.Success != null || dependencyTestChecks?.IsSuccess() != null)
                success = (readTestChecks?.Success ?? true) && (writeTestChecks?.Success ?? true) && (dependencyTestChecks?.IsSuccess() ?? true);

            return new TestCheckSummary
            {
                System = TestCheckSummary.GetSystemString(_assembly, url),
                Success = success,
                TestCoverage = coverage,
                ReadTestChecks = readTestChecks,
                WriteTestChecks = writeTestChecks,
                DependencyTestChecks = dependencyTestChecks,
                TestData = await GetTestDataAsync(settings),
                TestDate = DateTime.Now.ToShortDateString()
            };
        }

        internal TData RetrieveTestData(dynamic testData)
        {
            if (testData == null)
                return null;

            var fullName = NamedTestData.GetFullName(typeof(TData));

            try
            {
                foreach (JObject x in testData as JArray)
                {
                    if (x.TryGetValue("FullName", out JToken name))
                    {
                        _logger?.LogDebug($"Found the FullName {name}");

                        if (name.ToString() == fullName)
                        {
                            if (x.TryGetValue("TestData", out JToken value))
                            {
                                _logger?.LogDebug($"Found TestData {value.ToString()}");

                                var serialiser = new JsonSerializer();
                                serialiser.Converters.Add(new Core.Serialisation.MemoryStreamJsonConverter());

                                var tData = (TData)value.ToObject(typeof(TData), serialiser);
                                return tData;
                            }
                            else
                            {
                                _logger?.LogDebug($"Failed to find TestData");
                            }
                        }
                        else
                        {
                            _logger?.LogDebug($"Failed to match name {name.ToString()} != {fullName}");
                        }
                    }
                    else
                    {
                        _logger?.LogDebug($"Failed to find FullName in {x.ToString()}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
            }

            var testDataJson = testData?.ToString();
            _logger?.LogError($"Failed to find {fullName} in {testDataJson}");

            return null;
        }

        private bool AreTestsAllowed(TestSettings testSettings, params string[] serverApiKeys)
        {
            if (testSettings.Action.IsValid() == false)
            {
                _logger?.LogWarning($"{nameof(testSettings.Action)} of {testSettings.Action} is not recognised");
                return false;
            }

            if (testSettings.Action.HasGetNames())
            {
                //We're not going to actually run the tests!!
                return true;
            }

            foreach (var serverApiKey in serverApiKeys)
            {
                if (string.IsNullOrWhiteSpace(serverApiKey))
                    return true;

                if (serverApiKey.Equals(testSettings.ApiKey, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        public async Task<List<NamedTestData>> GetTestDataAsync(TestSettings settings)
        {
            var myTestData = RetrieveTestData(settings?.TestData);            
            var testData = new List<NamedTestData>() { new NamedTestData<TData>(myTestData ?? _testChecks().GetTestData()) };

            if (_dependencies != null)
                testData.AddRange(await new TestCheckDependencyRunner(_dependencies, _loggerFactory?.CreateLogger<TestCheckDependencyRunner>()).GetTestDataAsync());

            if (_dependencies != null && settings != null)
            { 
                var overridingTestData = JsonConvert.DeserializeObject<List<NamedTestData>>(settings.TestDataJson);
                if (overridingTestData == null)
                    return testData;

                overridingTestData.RemoveAll(r => testData.Select(s => s.FullName).Contains(r.FullName) == false);
                return overridingTestData;
            }

            return testData;
        }
    }
}