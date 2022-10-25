using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestChecker.Core;

namespace TestChecker.Runner.Services
{
    internal class MethodNameExtractorService : IMethodNameExtractorService
    {
        public IEnumerable<string> RetrieveMethodNames(TestCheckSummary testCheckSummary)
        {
            var names = new List<string>();

            names.AddRange(testCheckSummary?.ReadTestChecks?.TestChecks?.SelectMany(s => RetrieveMethodNames(s)) ?? Enumerable.Empty<string>());
            names.AddRange(testCheckSummary?.WriteTestChecks?.TestChecks?.SelectMany(s => RetrieveMethodNames(s)) ?? Enumerable.Empty<string>());

            if (testCheckSummary.DependencyTestChecks != null)
            {
                foreach (var dependency in testCheckSummary.DependencyTestChecks)
                {
                    names.AddRange(RetrieveMethodNames(dependency));
                }
            }

            return names.Distinct()
                        .Where(w => string.IsNullOrWhiteSpace(w) == false);
        }

        private IEnumerable<string> RetrieveMethodNames(TestCheck testCheck)
        {
            if (testCheck == null)
                return Enumerable.Empty<string>();

            if (testCheck.TestChecks.Any())
                return testCheck?.TestChecks?.Select(s => s.Method ?? s.Description ?? null);

            return new List<string> { $"{testCheck.Method ?? testCheck.Description ?? null}" };
        }
    }
}
