using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace TestChecker.Core
{
    [DebuggerDisplay("NamedTestData: {FullName}")]
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

    [DebuggerDisplay("NamedTestData: {FullName}")]
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
    }
}
