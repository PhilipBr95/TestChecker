using System.Collections.Generic;
using TestChecker.Core;

namespace TestChecker.Runner.Services
{
    public interface IMethodNameExtractorService
    {
        IEnumerable<string> RetrieveMethodNames(TestCheckSummary testCheckSummary);
    }
}