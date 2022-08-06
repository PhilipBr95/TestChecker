using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TestChecker.Core;

namespace TestChecker.Runner.Extensions
{
    public static class ListTestCheckSummaryExtensions
    {
        public static bool IsSuccess(this List<TestCheckSummary> summaries)
        {
            return summaries.All(a => a.Success.GetValueOrDefault(true));
        }

        public static IEnumerable<Coverage> GetCoverages(this List<TestCheckSummary> summaries)
        {
            return summaries.Select(s => s.TestCoverage);
        }        
    }
}