using TestChecker.Core;
using TestChecker.Runner;
using WebApplicationChild;
using WebApplicationChild.Models;

namespace WebApplicationChild;
internal class MyTestChecks : ITestChecks<MyTestData>
{
    private IFakeController _fakeController;
    private MyTestData _testData { get; set; } = new MyTestData { Surname = "Briggs" };

    public MyTestChecks()
    {
        _fakeController = new FakeController();
    }

    public MyTestData GetTestData()
    {
        return _testData;
    }

    public Task<TestCheck> RunReadTestsAsync(bool getNames)
    {
        var tests = new TestCheck("RunReadTestsAsync Tests", getNames);
        
        var testController = new TestCheck<IFakeController, MyTestData>(_fakeController, _testData, CoverageMethod.MethodsOnly, null, getNames);
        testController.TestIsTrue((obj, data) => obj.GetData(data.Town));
        testController.TestIsObject((obj, data) => obj.GetData(data.Town));
        testController.TestIsObject((obj, data) => obj.GetDateTime(data.City));

        tests.Add(testController);

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

public class FakeController : IFakeController
{
    public bool GetData(Town town)
    {
        return true;
    }

    public DateTime GetDateTime(string city)
    {
        return DateTime.Now;
    }
}