using Newtonsoft.Json;
using System;

namespace TestChecker.Core
{
    public class NamedTestData
    {
        [JsonProperty(Order = -20)]
        public string FullName { get; set; }

        [JsonProperty(Order = -10)]
        public dynamic TestData { get; set; }

        public static string GetFullName(object testData)
        {
            return GetFullName(testData.GetType());
        }

        public static string GetFullName(Type testData)
        {
            return testData.FullName;
        }
    }

    public class NamedTestData<T> : NamedTestData where T : class
    {
        [JsonProperty(Order = -10)]
        public new T TestData { get; set; }

        public NamedTestData()
        {
        }

        public NamedTestData(T testData)
        {
            FullName = GetFullName(testData);
            TestData = testData;
        }

        //public NamedTestData(string fullName, T testData)
        //{
        //    FullName = fullName;
        //    TestData = testData;
        //}        
    }
}
