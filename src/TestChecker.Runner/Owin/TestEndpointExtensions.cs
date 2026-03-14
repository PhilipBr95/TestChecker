#if NETFRAMEWORK
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Reflection;
using TestChecker.Core;
using TestChecker.Core.Serialisation;
using TestChecker.Runner.Services;

namespace TestChecker.Runner
{
    public static partial class TestEndpointExtensions
    {
        public static void UseTestEndpoint<TData>(this IAppBuilder app, Assembly assembly, List<ITestCheckDependency> dependencies, Func<ITestChecks<TData>> testChecks, ILoggerFactory loggerFactory, IMethodNameExtractorService methodNameExtractorService = null, string readEnvironmentName = READ_ENVIRONMENT_NAME, string readWriteEnvironmentName = READ_WRITE_ENVIRONMENT_NAME) where TData : class
        {
            _methodNameExtractor = methodNameExtractorService ?? new MethodNameExtractorService();
            _testCheckDependencyRunner = new TestCheckDependencyRunner(dependencies, loggerFactory.CreateLogger<TestCheckDependencyRunner>());

            try
            {
                _logger = loggerFactory?.CreateLogger(typeof(TestEndpointExtensions).FullName);
                var runner = new TestRunner<TData>(assembly, _testCheckDependencyRunner, testChecks, loggerFactory.CreateLogger<ITestChecks<TData>>(), GetEnvironmentVariable(readEnvironmentName), GetEnvironmentVariable(readWriteEnvironmentName));

                app.Use(async (context, next) =>
                {
                    if (await HandleContextAsync(runner, testChecks, context) == false)
                    {
                        await next().ConfigureAwait(false);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                throw;
            }
        }
    }
}

#endif