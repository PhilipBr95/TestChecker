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
                    if (IsTestRequest(context.Request.Path.Value))
                    {
                        var settings = await TestSettingsRetriever.GetSettingsAsync(context.Request).ConfigureAwait(false);
                        var response = await HandleContextAsync(context.Request.Path.Value, context.Request.GetUrl(), settings, runner, testChecks);

                        context.Response.ContentType = response.ContentType;
                        await context.Response.WriteAsync(response.Content).ConfigureAwait(false);
                    }
                    else
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