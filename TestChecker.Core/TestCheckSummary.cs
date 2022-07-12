﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TestChecker.Core.Serialisation;

namespace TestChecker.Core
{

    [JsonConverter(typeof(TestCheckSummaryConverter))]
    public class TestCheckSummary
    {
        public string System { get; set; }
        public bool? Success { get; set; }
        public Coverage TestCoverage { get; set; }
        public TestCheck ReadTestChecks { get; set; }
        public TestCheck WriteTestChecks { get; set; }
        public List<TestCheckSummary> DependencyTestChecks { get; set; }
        public object TestData { get; set; }
        public string TestDate { get; set; }
        public string Environment { get; set; }
        public string Version { get; set; }
    }


}
