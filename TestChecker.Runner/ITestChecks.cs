using System.Threading.Tasks;
using TestChecker.Core;

namespace TestChecker.Runner
{
    public interface ITestChecks<TData>
    {
        Task<TestCheck> RunReadTestsAsync();
        Task<TestCheck> RunWriteTestsAsync();
        TData GetTestData();        
        void SetTestData(TData testData);
    }
}