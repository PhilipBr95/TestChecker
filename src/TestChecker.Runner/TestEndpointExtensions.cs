using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using TestChecker.Core;
using Newtonsoft.Json.Linq;
using TestChecker.Runner.Extensions;
using TestChecker.Core.Serialisation;

namespace TestChecker.Runner
{
    public static partial class TestEndpointExtensions
    {
        private static ILogger _logger;

        public const string REGRESSION_END_POINT = "/test";
        public const string REGRESSIONTESTDATA_END_POINT = "/testdata";
        public const string REGRESSIONUI_END_POINT = "/testui";

        const string READ_ENVIRONMENT_NAME = "TEST_READ_SECURITY_TOKEN";
        const string READ_WRITE_ENVIRONMENT_NAME = "TEST_READWRITE_SECURITY_TOKEN";

        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new ShouldSerializeContractResolver(),
            Converters = new List<JsonConverter> { new MemoryStreamJsonConverter() }
        };

        public static void UseTestEndpoint<TData>(this IApplicationBuilder app, List<ITestCheckDependency> dependencies, Func<ITestChecks<TData>> testChecks, string readEnvironmentName = READ_ENVIRONMENT_NAME, string readWriteEnvironmentName = READ_WRITE_ENVIRONMENT_NAME) where TData : class, new()
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var runner = new TestRunner<TData>(Assembly.GetEntryAssembly(), dependencies, testChecks, loggerFactory, GetEnvironmentVariable(readEnvironmentName), GetEnvironmentVariable(readWriteEnvironmentName));

            try
            {
                _logger = loggerFactory?.CreateLogger(typeof(TestEndpointExtensions).FullName);

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
                        string html = await GenerateTestUIAsync(settings, Assembly.GetEntryAssembly(), runner, testChecks).ConfigureAwait(false);

                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(html).ConfigureAwait(false);
                    }
                    else if (context.Request.Path.Value.Equals(REGRESSION_END_POINT, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var settings = await Settings.GetSettingsAsync(context.Request).ConfigureAwait(false);
                        string json = await ExecuteTestsAsync(settings, runner, context.Request.GetUrl()).ConfigureAwait(false);

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

        private static void CheckTData<TData>()
        {
            var type = typeof(TData);

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic).ToList();
            properties.ForEach(fe => _logger?.LogWarning($"{type.FullName}.{fe.Name} isn't public"));
        }

        private static TData PopulateObject<TData>(string json) where TData : class, new()
        {            
            JArray jsonArray = JArray.Parse(json);

            foreach (JObject x in jsonArray)
            {                
                var tData = x.ToObject<TData>();

                if(tData != null)
                {
                    return tData;
                }
            }

            return null;
        }

        private async static Task<string> ExecuteTestsAsync<TData>(Settings settings, TestRunner<TData> runner, string url) where TData : class
        {
            var response = await runner.HandleRequestAsync(settings, url).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(response, _jsonSettings);

            return json;
        }

        private async static Task<string> GenerateTestUIAsync<TData>(Settings settings, Assembly callingAssembly, TestRunner<TData> runner, Func<ITestChecks<TData>> testChecks) where TData : class
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                                          .Single(str => str.EndsWith("TestUI.cshtml"));

            var html = new StreamReader(assembly.GetManifestResourceStream(resourceName)).ReadToEnd();
            
            var datas = await runner.GetTestDataAsync(null).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(datas, _jsonSettings);

            string result = GenerateHtml(settings, html, json);
            return result;
        }

        private static string GenerateHtml(Settings settings, string html, string json)
        {
            html = html.Replace("@Model.Action", settings.Path);
            html = html.Replace("@Model.TestData", json);
            return html.Replace("@Model.ApiKey", settings.ApiKey);            
        }

        private static string GetEnvironmentVariable(string environmentVariable)
        {
            try
            {
                return System.Environment.GetEnvironmentVariable(environmentVariable);
            }
            catch(Exception)
            {
                _logger?.LogWarning($"The env var '{environmentVariable}' could not be found");
            }

            //Not enabled
            return string.Empty;
        }
    }
}