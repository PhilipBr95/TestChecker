#if NETFRAMEWORK
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Reflection;
using TestChecker.Core;

namespace TestChecker.Runner
{
    public static partial class TestEndpointExtensions
    {
        public static void UseTestEndpoint<TData>(this IAppBuilder app, Assembly assembly, List<ITestCheckDependency> dependencies, Func<ITestChecks<TData>> testChecks, ILoggerFactory loggerFactory, string readEnvironmentName = READ_ENVIRONMENT_NAME, string readWriteEnvironmentName = READ_WRITE_ENVIRONMENT_NAME) where TData : class
        {
            try
            {
                _logger = loggerFactory?.CreateLogger(typeof(TestEndpointExtensions).FullName);
                var runner = new TestRunner<TData>(assembly, dependencies, testChecks, loggerFactory, GetEnvironmentVariable(readEnvironmentName), GetEnvironmentVariable(readWriteEnvironmentName));

                CheckTData<TData>();

                app.Use(async (context, next) =>
                {
                    if (context.Request.Path.Value.Equals(REGRESSIONTESTDATA_END_POINT, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var testData = await runner.GetTestDataAsync(null).ConfigureAwait(false);
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(testData, _jsonSettings);

                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(json).ConfigureAwait(false);
                    }
                    else if (context.Request.Path.Value.Equals(REGRESSIONUI_END_POINT, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var settings = await Settings.GetSettingsAsync(context.Request).ConfigureAwait(false);
                        string html = await GenerateTestUIAsync(settings, assembly, runner, testChecks).ConfigureAwait(false);

                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(html).ConfigureAwait(false);
                    }
                    else if (context.Request.Path.Value.Equals(REGRESSION_END_POINT, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var settings = await Settings.GetSettingsAsync(context.Request).ConfigureAwait(false);
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