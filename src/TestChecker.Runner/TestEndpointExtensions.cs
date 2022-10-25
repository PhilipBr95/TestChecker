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
using TestChecker.Runner.Services;

namespace TestChecker.Runner
{
    public static partial class TestEndpointExtensions
    {
        private static ILogger _logger;

        public const string TEST_END_POINT = "/test";
        public const string TESTDATA_END_POINT = "/testdata";
        public const string TESTUI_END_POINT = "/testui";

        const string READ_ENVIRONMENT_NAME = "TEST_READ_SECURITY_TOKEN";
        const string READ_WRITE_ENVIRONMENT_NAME = "TEST_READWRITE_SECURITY_TOKEN";

        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new ShouldSerializeContractResolver(),
            Converters = new List<JsonConverter> { new MemoryStreamJsonConverter() }
        };
        private static IMethodNameExtractorService _methodNameExtractor;

        public static void AddTestEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IMethodNameExtractorService, MethodNameExtractorService>();
        }

        public static void UseTestEndpoint<TData>(this IApplicationBuilder app, List<ITestCheckDependency> dependencies, Func<ITestChecks<TData>> testChecks, string readEnvironmentName = READ_ENVIRONMENT_NAME, string readWriteEnvironmentName = READ_WRITE_ENVIRONMENT_NAME) where TData : class, new()
        {
            _methodNameExtractor = app.ApplicationServices.GetService<IMethodNameExtractorService>();
            if (_methodNameExtractor == null) throw new InvalidOperationException($"Call ServiceCollection.AppTestEndpoint() first");

            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var runner = new TestRunner<TData>(Assembly.GetEntryAssembly(), dependencies, testChecks, loggerFactory, GetEnvironmentVariable(readEnvironmentName), GetEnvironmentVariable(readWriteEnvironmentName));

            try
            {
                _logger = loggerFactory?.CreateLogger(typeof(TestEndpointExtensions).FullName);

                //use Actions, not a new endpoint
                //use Actions, not a new endpoint
                //use Actions, not a new endpoint

                //app.Map(TESTINFO_END_POINT, appBuilder =>
                //{
                //    appBuilder.Run(async (context) =>
                //    {
                //        string json = JsonConvert.SerializeObject(new Info(), _jsonSettings);

                //        context.Response.ContentType = "application/json";
                //        await context.Response.WriteAsync(json).ConfigureAwait(false);
                //    });
                //});

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
                        string html = await GenerateTestUIAsync(settings, Assembly.GetEntryAssembly(), runner, context.Request.GetUrl(), testChecks).ConfigureAwait(false);

                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(html).ConfigureAwait(false);
                    }
                    else if (context.Request.Path.Value.Equals(TEST_END_POINT, StringComparison.CurrentCultureIgnoreCase))
                    {
                        CheckTData<TData>();

                        var settings = await TestSettingsRetriever.GetSettingsAsync(context.Request).ConfigureAwait(false);
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

        private async static Task<string> ExecuteTestsAsync<TData>(TestSettings settings, TestRunner<TData> runner, string url) where TData : class
        {
            var response = await runner.HandleRequestAsync(settings, url).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(response, _jsonSettings);

            return json;
        }

        private async static Task<string> GenerateTestUIAsync<TData>(TestSettings settings, Assembly callingAssembly, TestRunner<TData> runner, string url, Func<ITestChecks<TData>> testChecks) where TData : class
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                                          .Single(str => str.EndsWith("TestUI.cshtml"));

            var html = new StreamReader(assembly.GetManifestResourceStream(resourceName)).ReadToEnd();

            var datas = await runner.GetTestDataAsync(null).ConfigureAwait(false);

            //Future-proofing
            var versionSettings = new TestSettings(Core.Enums.Actions.GetVersion);            
            var versionsJson = await ExecuteTestsAsync<TData>(versionSettings, runner, url);
            var versionsSummary = JsonConvert.DeserializeObject<VersionInfoSummary>(versionsJson);
            //var versionInfos = (versionsSummary.Data as JArray).ToObject<IEnumerable<VersionInfo>>();

            var names = await ExecuteTestsAsync<TData>(settings, runner, url);
            var namesSummary = JsonConvert.DeserializeObject<TestCheckSummary>(names);
            IEnumerable<string> allNames = _methodNameExtractor.RetrieveMethodNames(namesSummary);

            var json = JsonConvert.SerializeObject(datas, _jsonSettings);

            string result = GenerateHtml(settings, html, json, allNames, versionsSummary);
            return result;
        }

        private static string GenerateHtml(TestSettings settings, string html, string json, IEnumerable<string> methodNames, VersionInfoSummary versionsSummary)
        {
            html = html.Replace("@Model.VersionInfos", Serialiser.Serialise(versionsSummary));
            html = html.Replace("@Model.Action", settings.Path);
            html = html.Replace("@Model.TestData", json);
            html = html.Replace("@Model.MethodNames", string.Join("<br />", methodNames.Select((s, i) => $"<input type='checkbox' name='testMethods' id='method{i}' value='{s}' checked /><label for='method{i}'>{s}</label>")));

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