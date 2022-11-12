using System.Collections.Generic;
using TestChecker.Core;
using TestChecker.Core.Models;

namespace TestChecker.Runner.Services
{
    public interface IMethodNameExtractorService
    {
        IEnumerable<MethodName> RetrieveMethodNames(TestCheckSummary testCheckSummary);
    }
}