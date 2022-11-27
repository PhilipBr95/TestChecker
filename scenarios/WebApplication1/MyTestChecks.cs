using Microsoft.AspNetCore.Mvc;
using TestChecker.Core;
using TestChecker.Core.Enums;
using TestChecker.Runner;
using WebApplication1;
namespace WebApplication1;

internal interface IMyController
{
    bool Test(string surname);
}
internal class MyController : IMyController
{
    public bool Test(string surname)
    {
        return surname == "Smith";
    }
}

internal class MyTestChecks : ITestChecks<MyTestData>
{
    private MyTestData _testData { get; set; } = new MyTestData { Surname = "Smith" };

    public MyTestData GetTestData()
    {
        return _testData;
    }

    public Task<TestCheck> RunReadTestsAsync()
    {
        var readTests = new TestCheck("Read Tests");
        var tests = new TestCheck<IMyController, MyTestData>(new MyController(), _testData, CoverageMethod.MethodsOnly, null);
        tests.TestIsTrue(x => x.Test(_testData.Surname));

        readTests.Add(tests);
        readTests.TestIsTrue("HasName", () => { return _testData.Surname == "Smith"; });
        readTests.TestIsTrue("GetMyName", () => { return _testData.Surname == "Smith"; });

        return Task.FromResult(readTests);
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