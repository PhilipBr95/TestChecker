#if NETFRAMEWORK
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Reflection;
using TestChecker.Core;
using TestChecker.Runner.Services;

namespace TestChecker.Runner
{
    public static partial class TestEndpointExtensions
    {
        public static void UseTestEndpoint<TData>(this IAppBuilder app, Assembly assembly, List<ITestCheckDependency> dependencies, Func<ITestChecks<TData>> testChecks, ILoggerFactory loggerFactory, IMethodNameExtractorService methodNameExtractorService = null, string readEnvironmentName = READ_ENVIRONMENT_NAME, string readWriteEnvironmentName = READ_WRITE_ENVIRONMENT_NAME) where TData : class
        {
            _methodNameExtractor = methodNameExtractorService ?? new MethodNameExtractorService();

            try
            {
                _logger = loggerFactory?.CreateLogger(typeof(TestEndpointExtensions).FullName);
                var runner = new TestRunner<TData>(assembly, dependencies, testChecks, loggerFactory, GetEnvironmentVariable(readEnvironmentName), GetEnvironmentVariable(readWriteEnvironmentName));

                app.Use(async (context, next) =>
                {
                    if (context.Request.Path.Value.Equals(TESTDATA_END_POINT, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var testData = await runner.GetTestDataAsync(null).ConfigureAwait(false);
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(testData, _jsonSettings);

                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(json).ConfigureAwait(false);
                    }
                    else if (context.Request.Path.Value.Equals(TESTUI_END_POINT, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var settings = await TestSettingsRetriever.GetSettingsAsync(context.Request).ConfigureAwait(false);
                        string html = await GenerateTestUIAsync(settings, assembly, runner, context.Request.Uri.ToString(), testChecks).ConfigureAwait(false);

                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(html).ConfigureAwait(false);
                    }
                    else if (context.Request.Path.Value.Equals(TEST_END_POINT, StringComparison.CurrentCultureIgnoreCase))
                    {
                        CheckTData<TData>();

                        var settings = await TestSettingsRetriever.GetSettingsAsync(context.Request).ConfigureAwait(false);
                        var json = await ExecuteTestsAsync(settings, runner, context.Request.Uri.ToString()).ConfigureAwait(false);

                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(json).ConfigureAwait(false);
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