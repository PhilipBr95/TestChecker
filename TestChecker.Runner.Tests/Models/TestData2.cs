namespace TestChecker.Runner.Tests
{
    public class TestData2
    {
        public string Name2 { get; set; }
        public int Age2 { get; set; }

        public InnerTestData InnerTestData2 { get; set; }

        public TestData2()
        {
            Name2 = "Tree_2";
            Age2 = 2;
            InnerTestData2 = new InnerTestData() { InnerAge = 22, InnerName = "TreeTree_2" };
        }
    }
}
