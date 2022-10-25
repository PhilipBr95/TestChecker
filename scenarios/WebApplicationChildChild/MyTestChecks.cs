using TestChecker.Core;
using TestChecker.Runner;
using WebApplicationChildChild;
namespace WebApplicationChildChild;

internal class MyTestChecks : ITestChecks<MyTestData>
{
    private MyTestData _testData { get; set; } = new MyTestData { Surname2 = "House" };

    public MyTestData GetTestData()
    {
        return _testData;
    }

    public Task<TestCheck> RunReadTestsAsync(bool getNames)
    {
        var tests = new TestCheck("Read Tests", getNames);
        tests.Add(new TestCheck("Surname2 Test", _testData.Surname2, _testData.Surname2 == "House"));

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