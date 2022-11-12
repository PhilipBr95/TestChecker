using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestChecker.Core;
using TestChecker.Core.Models;

namespace TestChecker.Runner.Services
{
    internal class MethodNameExtractorService : IMethodNameExtractorService
    {
        public IEnumerable<MethodName> RetrieveMethodNames(TestCheckSummary testCheckSummary)
        {
            var methodNames = new List<MethodName>();

            if(testCheckSummary.ReadTestChecks == null && testCheckSummary.WriteTestChecks == null)
            {
                //Looks like 
            }

            methodNames.AddRange(testCheckSummary?.ReadTestChecks?.TestChecks?.SelectMany(s => RetrieveMethodNames(testCheckSummary.System, s)) ?? Enumerable.Empty<MethodName>());
            methodNames.AddRange(testCheckSummary?.WriteTestChecks?.TestChecks?.SelectMany(s => RetrieveMethodNames(testCheckSummary.System, s)) ?? Enumerable.Empty<MethodName>());

            if (testCheckSummary.DependencyTestChecks != null)
            {
                foreach (var dependency in testCheckSummary.DependencyTestChecks)
                {
                    methodNames.AddRange(RetrieveMethodNames(dependency));
                }
            }

            return methodNames.Distinct()
                        .Where(w => string.IsNullOrWhiteSpace(w.Method) == false);
        }

        private IEnumerable<MethodName> RetrieveMethodNames(SystemInfo systemInfo, TestCheck testCheck)
        {
            if (testCheck == null)
                return Enumerable.Empty<MethodName>();

            if (testCheck.TestChecks.Any())
                return testCheck?.TestChecks?.Select(s => new MethodName { AssemblyName = systemInfo.Name, Method = s.Method ?? s.Description });

            return new List<MethodName> { new MethodName { AssemblyName = systemInfo.Name, Method = testCheck.Method, Description = testCheck.Description ?? null } };
        }
    }
}
