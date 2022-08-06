using System;
using System.Collections.Generic;
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

        public async Task<dynamic> HandleRequestAsync(Settings settings, string url)
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

            TestCheck readTestChecks = null;
            TestCheck writeTestChecks = null;            

            if (AreTestsAllowed(settings.ApiKey, _readApiKey, _readWriteApiKey))
            {
                if (settings.Action.HasFlag(Actions.RunReadTests))
                {
                    _logger?.LogDebug($"{nameof(testChecks.RunReadTestsAsync)} called");
                    readTestChecks = await testChecks.RunReadTestsAsync().ConfigureAwait(false);
                }
            }
            else
            {
                readTestChecks = new TestCheck($"Your ApiKey does not match the Read or Write ApiKey!");
            }

            if (AreTestsAllowed(settings.ApiKey, _readWriteApiKey) && settings.Action.HasFlag(Actions.RunWriteTests))
            {
                _logger?.LogDebug($"{nameof(testChecks.RunWriteTestsAsync)} called");
                writeTestChecks = await testChecks.RunWriteTestsAsync().ConfigureAwait(false);
            }

            List<TestCheckSummary> dependencyTestChecks = null;

            if (_dependencies != null)
            {                
                dependencyTestChecks = await new TestCheckDependencyRunner(_dependencies, _loggerFactory?.CreateLogger<TestCheckDependencyRunner>()).RunTestsAsync(settings).ConfigureAwait(false);
            }

            var coverages = new List<Coverage> { readTestChecks?.Coverage, writeTestChecks?.Coverage };

            if(dependencyTestChecks != null)
                coverages.AddRange(dependencyTestChecks.GetCoverages());

            var coverage = new Coverage(coverages);
            bool? success = null;

            if (readTestChecks?.Success != null || writeTestChecks?.Success != null || dependencyTestChecks?.IsSuccess() != null)
                success = (readTestChecks?.Success ?? true) && (writeTestChecks?.Success ?? true) && (dependencyTestChecks?.IsSuccess() ?? true);

            return new TestCheckSummary
            {
                System = GetService(url),
                Success = success,
                TestCoverage = coverage,
                ReadTestChecks = readTestChecks,
                WriteTestChecks = writeTestChecks,
                DependencyTestChecks = dependencyTestChecks,
                TestData = await GetTestDataAsync(),
                TestDate = DateTime.Now.ToShortDateString(),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            };
        }

        internal TData RetrieveTestData(dynamic testData)
        {
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

        private bool AreTestsAllowed(string apiKey, params string[] serverApiKeys)
        {
            foreach (var serverApiKey in serverApiKeys)
            {
                if (string.IsNullOrWhiteSpace(serverApiKey))
                    break;

                if (serverApiKey.Equals(apiKey, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        private string GetService(string url)
        {
            var name = _assembly?.GetName();

            if (name == null)
                return "Unkown";

            return $"{name.Name}, Version={name.Version}, Url={url}";
        }

        public async Task<List<NamedTestData>> GetTestDataAsync()
        {
            var testData = new List<NamedTestData>();
            var testChecks = _testChecks();
            
            testData.Add(new NamedTestData<TData>(testChecks.GetTestData()));

            if(_dependencies != null)
                testData.AddRange(await new TestCheckDependencyRunner(_dependencies, _loggerFactory?.CreateLogger<TestCheckDependencyRunner>()).GetTestDataAsync());

            return testData;
        }
    }
}