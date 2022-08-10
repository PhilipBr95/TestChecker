using Xunit;

namespace TestChecker.Runner.Tests
{
    public class TestRunnerTests
    {
        //[Fact]
        //public async void RetrieveTestData_Succeeds()
        //{
        //    var expectedTestData = new TestData() { Name = "New Tree" };
        //    var expectedTestDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(expectedTestData);

        //    var runner = new TestRunner<TestData>(null, null, () => new TestChecks<TestData>(expectedTestData), null, null, null);

        //    var data = await runner.GetTestDataAsync().ConfigureAwait(false);
        //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        //    var dynamicJson = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

        //    var testData = runner.RetrieveTestData(dynamicJson);
        //    var testDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(testData);

        //    Assert.Equal(expectedTestDataJson, testDataJson);
        //}

        //[Fact]
        //public async void RetrieveTestData_FailToFindTestData()
        //{
        //    var expectedTestData = new TestData() { Name = "New Tree" };
        //    var expectedTestData2 = new TestData2() { Name2 = "New Tree2" };

        //    var runner = new TestRunner<TestData>(null, null, () => new TestChecks<TestData>(expectedTestData), null, null, null);
        //    var runner2 = new TestRunner<TestData2>(null, null, () => new TestChecks<TestData2>(expectedTestData2), null, null, null);

        //    var data = await runner.GetTestDataAsync().ConfigureAwait(false);
        //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        //    var dynamicJson = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

        //    var testData = runner2.RetrieveTestData(dynamicJson);

        //    Assert.Null(testData);
        //}
    }
}
