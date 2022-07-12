namespace TestChecker.Runner.Tests
{
    public class TestData
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public InnerTestData InnerTestData { get; set; }

        public TestData()
        {
            Name = "Tree_1";
            Age = 1;
            InnerTestData = new InnerTestData() { InnerAge = 11, InnerName = "TreeTree_1" };
        }
    }
}
