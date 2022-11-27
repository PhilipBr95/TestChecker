using System.Threading.Tasks;
using TestChecker.Core;
using TestChecker.Runner;

namespace WebApplicationOldChild
{
    public interface IMyTests
    {
        bool TestName(string name);
    }

    public class MyTests : IMyTests
    {
        public bool TestName(string name)
        {
            return "OldBriggs" == name;
        }
    }

    internal class MyTestChecks : ITestChecks<MyTestData>
    {
        private MyTestData _testData { get; set; } = new MyTestData { OldSurname = "OldBriggs" };

        public MyTestData GetTestData()
        {
            return _testData;
        }

        public Task<TestCheck> RunReadTestsAsync()
        {
            var controller = new MyTests();
            var allTests = new TestCheck("All Tests");
            var tests = new TestCheck<IMyTests, MyTestData>(controller, _testData, CoverageMethod.MethodsOnly, null);
            tests.TestIsTrue(x => x.TestName(_testData.OldSurname));
            allTests.Add(tests);

            return Task.FromResult(allTests);
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