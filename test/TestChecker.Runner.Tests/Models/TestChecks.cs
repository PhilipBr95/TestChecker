using System.Threading.Tasks;
using TestChecker.Core;
using TestChecker.Runner;

namespace TestChecker.Runner.Tests
{
    public class TestChecks<T> : ITestChecks<T> where T : new()
    {
        private T _testData;

        public TestChecks(T testData)
        {
            _testData = testData;
        }

        public T GetTestData()
        {
            return _testData;
        }

        public Task<TestCheck> RunReadTestsAsync(bool getNames)
        {
            throw new System.NotImplementedException();
        }

        public Task<TestCheck> RunWriteTestsAsync()
        {
            throw new System.NotImplementedException();
        }

        public void SetTestData(T testData)
        {
            _testData = testData;
        }
    }
}
