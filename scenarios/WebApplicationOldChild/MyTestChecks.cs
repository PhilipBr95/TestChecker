using System.Threading.Tasks;
using TestChecker.Core;
using TestChecker.Runner;

namespace WebApplicationOldChild
{
    public class Employer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface ImyController
    {
        Employer GetEmployer(int employerId);
    }

    public class MyController : ImyController
    {
        public Employer GetEmployer(int employerId)
        {
            return new Employer { Id = employerId, Name = "MyEmployer" };
        }
    }

    internal class MyTestChecks : ITestChecks<MyTestData>
    {
        private readonly ImyController _myController;
        private MyTestData _testData;

        public MyTestChecks(MyController myController, MyTestData testData)
        {
            _myController = myController;
            _testData = testData;
        }

        public MyTestData GetTestData() => _testData;
        public void SetTestData(MyTestData testData) => _testData = testData;

        public async Task<TestCheck> RunReadTestsAsync()
        {
            var allTests = new TestCheck("MyController Tests");

            var tests = new TestCheck<ImyController, MyTestData>(_myController, _testData, CoverageMethod.MethodsOnly, null);
            tests.TestIsObject(x => x.GetEmployer(_testData.EmployerId));

            allTests.Add(tests);

            return await Task.FromResult(allTests);
        }

        public Task<TestCheck> RunWriteTestsAsync()
        {
            throw new System.NotImplementedException();
        }        
    }
}