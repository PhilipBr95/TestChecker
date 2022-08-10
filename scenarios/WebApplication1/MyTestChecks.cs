using TestChecker.Core;
using TestChecker.Runner;
using WebApplication1;

internal class MyTestChecks : ITestChecks<MyTestData>
{
    private MyTestData _testData { get; set; } = new MyTestData { Surname = "Smith" };

    public MyTestData GetTestData()
    {
        return _testData;
    }

    public Task<TestCheck> RunReadTestsAsync()
    {
        return Task.FromResult(new TestCheck(_testData.Surname == "Smith") { ReturnValue = _testData.Surname });
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