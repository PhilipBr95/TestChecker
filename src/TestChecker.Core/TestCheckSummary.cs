using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using TestChecker.Core.Serialisation;

namespace TestChecker.Core
{

    [JsonConverter(typeof(TestCheckSummaryConverter))]
    [DebuggerDisplay("{System?.Name}")]
    public class TestCheckSummary : object
    {
        public SystemInfo System { get; set; }
        public bool? Success { get; set; }
        public Coverage TestCoverage { get; set; }
        public TestCheck ReadTestChecks { get; set; }
        public TestCheck WriteTestChecks { get; set; }
        public List<TestCheckSummary> DependencyTestChecks { get; set; }
        public object TestData { get; set; }
        public string TestDate { get; set; }
        public object Data { get; set; }

        public static SystemInfo GetSystemString(Assembly assembly, string url)
        {
            var name = assembly?.GetName();
            return new SystemInfo { Name = name?.Name ?? "Unknown", Version = name?.Version, Url = url };
        }

        public void Add(TestCheckSummary obj)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<TestCheckSummary> list)
        {
            throw new NotImplementedException();
        }
    }
}
