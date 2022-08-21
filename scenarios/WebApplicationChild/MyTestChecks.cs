using TestChecker.Core;
using TestChecker.Runner;
using WebApplicationChild;
using WebApplicationChild.Models;

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

    public Task<TestCheck> RunReadTestsAsync()
    {
        var tests = new TestCheck(_testData.Surname == "Briggs") { ReturnValue = _testData.Surname };
        
        var testController = new TestCheck<IFakeController, MyTestData>(_fakeController, _testData, CoverageMethod.MethodsOnly, null);
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