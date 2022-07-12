using Newtonsoft.Json;
using TestChecker.Core;
using TestChecker.Core.ContractResolver;
using TestChecker.Core.Serialisation;
using Xunit;

namespace TestChecker.Runner.Tests
{
    public class TestCheckSummaryTests
    {
        [Fact]
        public void TestCheckSummary_IsBackwardsCompatable_ToVer1()
        {
            //v1 json
            var json = "{\"System\":\"Sys\",\"Success\":true,\"TestCoverage\":{\"Object\":null,\"CoverageMethod\":null,\"Percentage\":10.0,\"Detail\":\"Lots\"},\"ReadTestChecks\":{\"Object\":null,\"Method\":\"Method\",\"Description\":null,\"Parameters\":\"Params\",\"ReturnValue\":\"Message\",\"Success\":true,\"SuccessCount\":1,\"TestChecks\":[]},\"WriteTestChecks\":{\"Object\":null,\"Method\":\"Method\",\"Description\":null,\"Parameters\":\"Params\",\"ReturnValue\":\"Message\",\"Success\":true,\"SuccessCount\":1,\"TestChecks\":[]},\"DependencyTestChecks\":{\"Object\":null,\"Method\":\"Method\",\"Description\":null,\"Parameters\":\"Params\",\"ReturnValue\":\"Message\",\"Success\":true,\"SuccessCount\":1,\"TestChecks\":[]},\"TestData\":null,\"TestDate\":null,\"Environment\":\"Env\",\"Version\":null}";

            var summary = Newtonsoft.Json.JsonConvert.DeserializeObject<TestCheckSummary>(json);

            Assert.Equal(true, summary.Success);
            Assert.NotNull(summary.ReadTestChecks);
            Assert.NotNull(summary.WriteTestChecks);
            Assert.NotNull(summary.TestCoverage);
            Assert.NotNull(summary.DependencyTestChecks);
        }
    }
}
