﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core;

namespace TestChecker.Runner
{
    internal class TestCheckDependencyRunner : ITestCheckDependencyRunner
    {        
        public List<ITestCheckDependency> Dependencies { get; private set; }
        private ILogger<TestCheckDependencyRunner> _logger;

        public TestCheckDependencyRunner(List<ITestCheckDependency> dependencies, ILogger<TestCheckDependencyRunner> logger)
        {
            Dependencies = dependencies;
            _logger = logger;
        }

        public async Task<List<T>> RunTestActionAsync<T>(TestSettings settings) where T : new()
        {             
            if (Dependencies == null) return default;

            var taskResults = new List<T>();

            foreach (var dependency in Dependencies)
            {
                _logger?.LogDebug($"{nameof(dependency.RunTestActionAsync)} called on {dependency.Service} with {settings.TestDataJson}");

                try
                {
                    var versionInfo = (await dependency.GetVersionInfoAsync());

                    if (versionInfo.HasAvailableAction(settings.Action))
                    {
                        var taskResult = await dependency.RunTestActionAsync<T>(settings).ConfigureAwait(false);
                        taskResults.Add(taskResult);
                    }
                    else
                        _logger?.LogWarning($"Missing Action {settings.Action} for {dependency.Service.BaseUrl}");
                }
                catch(Exception ex)
                {
                    _logger?.LogError(ex, $"Error hitting {nameof(dependency.RunTestActionAsync)} on {dependency.Service}");
                    throw;
                }
            }
            
            return taskResults;
        }

        public async Task<List<NamedTestData>> GetTestDataAsync()
        {
            var testData = new List<NamedTestData>();

            if (Dependencies == null) return testData;

            foreach (var dependency in Dependencies)
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

        public async Task<List<VersionInfo>> GetVersionInfoAsync()
        {
            var versionInfos = new List<VersionInfo>();

            if (Dependencies == null) return versionInfos;

            foreach (var dependency in Dependencies)
            {
                versionInfos.Add(await dependency.GetVersionInfoAsync());
            }

            return versionInfos;
        }
    }
}