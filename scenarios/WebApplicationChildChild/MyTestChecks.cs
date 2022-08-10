using TestChecker.Core;
using TestChecker.Runner;
using WebApplicationChildChild;

internal class MyTestChecks : ITestChecks<MyTestData>
{
    private MyTestData _testData { get; set; } = new MyTestData { Surname2 = "House" };

    public MyTestData GetTestData()
    {
        return _testData;
    }

    public Task<TestCheck> RunReadTestsAsync()
    {
        return Task.FromResult(new TestCheck(_testData.Surname2 == "House") {  ReturnValue= _testData.Surname2 } );
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