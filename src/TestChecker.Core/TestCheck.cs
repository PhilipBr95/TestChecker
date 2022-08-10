using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TestChecker.Core.Serialisation;

namespace TestChecker.Core
{
    [DebuggerDisplay("Object = {ObjectName}, Method = {Method}")]
    public class TestCheck
    {

        [JsonProperty("Object", Order =-20)]
        public string ObjectName { get; set; }
        [JsonProperty(Order = -20)]
        public string Method { get; set; }
        [JsonProperty(Order = -20)]
        public string Description { get; set; }
        [JsonProperty(Order = -12)]
        public bool? Success { get; set; }

        [JsonProperty(Order = -15)]
        public string Parameters { get; set; }

        private string _returnValue = null;
        private bool _hasReturnValue = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Include, Order = -13)]
        public string ReturnValue
        {
            get => _returnValue;
            set
            {
                _returnValue = value;
                _hasReturnValue = true;
            }
        }

        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, Order = -11)]
        public long SuccessCount { get; set; } = 0;

        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, Order = -10)]
        public long ErrorCount { get; set; } = 0;

        public List<TestCheck> TestChecks { get; set; } = new List<TestCheck>();

        public TestCheck()
        {
        }

        public TestCheck(string description)
        {
            Description = description;
        }
        public TestCheck(bool success)
        {
            Success = success;
            
            if (success)
            {
                SuccessCount++;
            }
            else
            {
                ErrorCount++;
            }
        }

        public TestCheck(string method, Exception ex)
        {
            Method = method;
            Error(ex);
        }

        public TestCheck(string method, string parameters, bool success, string message)
        {
            Method = method;
            Success = success;
            ReturnValue = message;
            Parameters = parameters;

            if (success)
            {
                SuccessCount++;
            }
            else
            {
                ErrorCount++;
            }
        }

        public void AddRange(IEnumerable<TestCheck> testChecks, bool updateIsHealthy = true)
        {
            if (testChecks == null) return;

            TestChecks.AddRange(testChecks);

            var newErrorCount = testChecks.Sum(s => s.ErrorCount);
            ErrorCount += newErrorCount;
            SuccessCount += testChecks.Sum(s => s.SuccessCount);

            if (updateIsHealthy)
                Success = Success.GetValueOrDefault(true) && (newErrorCount == 0);
        }

        public void Error(Exception ex)
        {
            Success = false;
            ReturnValue = ex.ToString();

            ErrorCount++;
        }

        public TestCheck Add(TestCheck testCheck, bool updateIsHealthy = true)
        {
            if (testCheck == null)
                return this;

            TestChecks.Add(testCheck);

            ErrorCount += testCheck.ErrorCount;
            SuccessCount += testCheck.SuccessCount;

            if (updateIsHealthy)
            {
                if(Success == null) Success = testCheck.Success;
                else Success &= testCheck.Success;
            }

            return this;
        }

        public TestCheck TestIsTrue(string description, Func<bool> functionToTest)
        {
            try
            {
                var success = functionToTest();

                Add(new TestCheck(success) { Description = description, ReturnValue = success.ToString() }, true);
            }
            catch (Exception ex)
            {
                Add(new TestCheck(null, ex) { Description = description }, true);
            }

            return this;
        }

        public async Task<TestCheck> TestIsTrueAsync(string description, Func<Task<bool>> functionToTest)
        {
            try
            {
                var success = await functionToTest().ConfigureAwait(false);

                Add(new TestCheck(success) { Description = description, ReturnValue = success.ToString() }, true);
            }
            catch (Exception ex)
            {
                Add(new TestCheck(null, ex) { Description = description }, true);
            }

            return this;
        }

        private Coverage _coverage = null;
        
        [JsonProperty(Order = -2)]
        public Coverage Coverage
        {
            get
            {
                if (_coverage != null) return _coverage;

                var hits = TestChecks.Where(w => w.Coverage?.Hits != null).SelectMany(s => s.Coverage.Hits).Distinct().ToList();
                var total = TestChecks.Where(w => w.Coverage?.Hits != null).SelectMany(s => s.Coverage.Total).Distinct().ToList();

                return new Coverage(null, null, hits, total);
            }
            set
            {
                _coverage = value;
            }
        }

        public bool ShouldSerializeCoverage()
        {
            var coverage = Coverage;

            if (coverage == null)
                return false;

            return coverage.Total.Count > 0 || coverage.Percentage > 0;
        }

        public bool ShouldSerializeReturnValue()
        {
            return _hasReturnValue;
        }

        public static TestCheck NotImplemented(string objectName)
        {
            return new TestCheck { ObjectName = objectName, Description = $"Has not yet been implemented!" };
        }

        public async static Task<TestCheck> NotImplementedAsync(string objectName)
        {
            return await Task.FromResult(NotImplemented(objectName)).ConfigureAwait(false);
        }
    }
}