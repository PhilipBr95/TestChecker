using System.Threading.Tasks;
using TestChecker.Core;
using TestChecker.Runner;

namespace WebApplicationOldChild
{
    internal class MyTestChecks : ITestChecks<MyTestData>
    {
        private MyTestData _testData { get; set; } = new MyTestData { OldSurname = "OldBriggs" };

        public MyTestData GetTestData()
        {
            return _testData;
        }

        public Task<TestCheck> RunReadTestsAsync()
        {
            var tests = new TestCheck("RunReadTestsAsync Tests");
            tests.TestIsTrue("OldSurname == OldBriggs", () => _testData.OldSurname == "OldBriggs");

            return Task.FromResult(tests);
        }

        public Task<TestCheck> RunWriteTestsAsync()
        {
            throw new System.NotImplementedException();
        }

        public void SetTestData(MyTestData testData)
        {
            _testData = testData;
        }
    }
}