using TestChecker.Core;
using TestChecker.Runner;
using WebApplication1;
namespace WebApplication1;

internal class MyTestChecks : ITestChecks<MyTestData>
{
    private MyTestData _testData { get; set; } = new MyTestData { Surname = "Smith" };

    public MyTestData GetTestData()
    {
        return _testData;
    }

    public Task<TestCheck> RunReadTestsAsync()
    {
        var tests = new TestCheck("Read Tests");
        tests.TestIsTrue("Test Surname", () => { return _testData.Surname == "Smith"; });
        tests.TestIsTrue("HasName", () => { return _testData.Surname == "Smith"; });
        tests.TestIsTrue("GetMyName", () => { return _testData.Surname == "Smith"; });

        return Task.FromResult(tests);
    }

    public Task<TestCheck> RunWriteTestsAsync()
    {
        throw new NotImplementedException();
    }

    public void SetTestData(MyTestData testData)
    {
        _testData = testData;
    }
}