using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using TestChecker.Core.Serialisation;

namespace TestChecker.Core
{    
    public partial class TestCheck<T,TData> : TestCheck where T : class 
                                                where TData : class
    {        
        private Recorder<T> _recorder;
        private T _obj;
        private TData _testData;
        private CoverageMethod _coverageMethod;
        private ILogger _logger;
        private IObjectSerialiser ObjectSerialiser { get; set; }

        //public TData TestData => _testData;

        [JsonIgnore]
        public T Object => _obj;

        public TestCheck(T obj, TData testData, CoverageMethod coverageMethod, ILogger logger, IObjectSerialiser objectSerialiser = null) : base()
        {            
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be an interface in x = new TestCheck<T....");

            ObjectName = typeof(T).Name;
            _testData = testData;
            _coverageMethod = coverageMethod;
            _logger = logger;

            if (objectSerialiser == null)
            {
                ObjectSerialiser = new ObjectSerialiser(logger);
            }
            else
            {
                ObjectSerialiser = objectSerialiser;
            }

            _recorder = new Recorder<T>(coverageMethod);
            _obj = _recorder.CreateProxy(obj);

            _recorder.CoverageChanged += (sender, coverage) => { Coverage = coverage; };
        }

        public async Task<TestCheck<T, TData>> TestIsObjectAsync<TOut>(string description, Func<T, TData, Task<TOut>> functionToTest, int maxDepth = 1)
        {
            try
            {
                var obj = await functionToTest.Invoke(_obj, _testData).ConfigureAwait(false);
                ObjectSerialiser.SetObject(obj);

                var success = ObjectSerialiser.IsObject();

                Add(new TestCheck(success) { ReturnValue = ObjectSerialiser.SerialiseObject(maxDepth), Description = description }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(null, ex) { Description = description }, true);
            }

            return this;
        }


        public async Task<TestCheck<T, TData>> TestIsObjectAsync<TOut>(Expression<Func<T, TData, Task<TOut>>> functionToTest, int maxDepth = 1)
        {
            var methodCallExpression = (functionToTest.Body as MethodCallExpression);
            string method = GetMethodName(methodCallExpression);

            try
            {
                var parameters = GetParamsStringForMethod(methodCallExpression);

                var function = functionToTest.Compile();
                var obj = await function.Invoke(_obj, _testData).ConfigureAwait(false);
                ObjectSerialiser.SetObject(obj);

                var success = ObjectSerialiser.IsObject();

                Add(new TestCheck(method, parameters, success, ObjectSerialiser.SerialiseObject(maxDepth)), true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(method, ex), true);
            }

            return this;
        }

        public TestCheck<T, TData> TestIsObject<TOut>(Expression<Func<T, TData, TOut>> functionToTest, int maxDepth = 1)
        {
            var methodCallExpression = (functionToTest.Body as MethodCallExpression);
            string method = GetMethodName(methodCallExpression);

            try
            {
                var parameters = GetParamsStringForMethod(methodCallExpression);

                var function = functionToTest.Compile();
                var obj = function.Invoke(_obj, _testData);
                ObjectSerialiser.SetObject(obj);

                var success = ObjectSerialiser.IsObject();

                Add(new TestCheck(method, parameters, success, ObjectSerialiser.SerialiseObject(maxDepth)), true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(method, ex), true);
            }

            return this;
        }

        public TestCheck<T, TData> TestIsObject<TOut>(Expression<Func<T, TOut>> functionToTest, int maxDepth = 1)
        {
            var method = GetEndPointName<TOut>(functionToTest);
            var methodCallExpression = (functionToTest.Body as MethodCallExpression);

            try
            {
                var parameters = GetParamsStringForMethod(methodCallExpression);

                var function = functionToTest.Compile();
                var obj = function.Invoke(_obj);
                ObjectSerialiser.SetObject(obj);

                var success = ObjectSerialiser.IsObject();

                Add(new TestCheck(method, parameters, success, ObjectSerialiser.SerialiseObject(maxDepth)), true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(method, ex), true);
            }

            return this;
        }

        public TestCheck<T, TData> TestIsObject<TOut>(string description, Func<T, TData, TOut> functionToTest, int maxDepth = 1)
        {
            try
            {                
                var obj = functionToTest.Invoke(_obj, _testData);
                ObjectSerialiser.SetObject(obj);

                var success = ObjectSerialiser.IsObject();

                Add(new TestCheck(success) { ReturnValue = ObjectSerialiser.SerialiseObject(maxDepth), Description = description }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(null, ex) { Description = description }, true);
            }

            return this;
        }        

        public TestCheck<T, TData> TestIsObject<TOut>(string description, Func<T, TOut> functionToTest, int maxDepth = 1)
        {
            try
            {
                var obj = functionToTest.Invoke(_obj);
                ObjectSerialiser.SetObject(obj);

                var success = ObjectSerialiser.IsObject();

                Add(new TestCheck(success) { ReturnValue = ObjectSerialiser.SerialiseObject(maxDepth), Description = description }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(null, ex) { Description = description }, true);
            }

            return this;
        }

        public TestCheck<T, TData> TestIsTrue(Expression<Func<T, TData, bool>> functionToTest)
        {
            var methodCallExpression = (functionToTest.Body as MethodCallExpression);
            string method = GetMethodName(methodCallExpression);

            try
            {
                var function = functionToTest.Compile();
                var success = function.Invoke(_obj, _testData);

                Add(new TestCheck(method, null, success, null) { ReturnValue = success.ToString() }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(method, ex), true);
            }

            return this;

        }

        public TestCheck<T, TData> TestIsTrue(Expression<Func<T, bool>> functionToTest)
        {
            var methodCallExpression = (functionToTest.Body as MethodCallExpression);
            string method = GetMethodName(methodCallExpression);

            try
            {
                var function = functionToTest.Compile();
                var success = function.Invoke(_obj);
                
                Add(new TestCheck(method, null, success, null) { ReturnValue = success.ToString() }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(method, ex), true);
            }

            return this;
        }

        public async Task<TestCheck<T, TData>> TestIsTrueAsync(Expression<Func<T, Task<bool>>> functionToTest)
        {
            var methodCallExpression = (functionToTest.Body as MethodCallExpression);
            string method = GetMethodName(methodCallExpression);

            try
            {
                var function = functionToTest.Compile();
                var success = await function.Invoke(_obj).ConfigureAwait(false);
                
                Add(new TestCheck(method, null, success, success.ToString()) { ReturnValue = success.ToString() }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(method, ex), true);
            }

            return this;
        }

        public TestCheck<T, TData> TestIsTrue(string description, Func<TData, bool> functionToTest)
        {
            try
            {
                var success = functionToTest(_testData);

                Add(new TestCheck(success) { Description = description, ReturnValue = success.ToString() }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(null, ex) { Description = description }, true);
            }

            return this;
        }

        public TestCheck<T, TData> TestIsTrue(string description, Func<T, TData, bool> functionToTest)
        {
            try
            {
                var success = functionToTest.Invoke(_obj, _testData);

                Add(new TestCheck(success) { Description = description, ReturnValue = success.ToString() }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(null, ex) { Description = description }, true);
            }

            return this;
        }   

        public async Task<TestCheck<T, TData>> TestIsTrueAsync(string description, Func<T, TData, Task<bool>> functionToTest)
        {
            try
            {
                var success = await functionToTest.Invoke(_obj, _testData).ConfigureAwait(false);

                Add(new TestCheck(success) { Description = description, ReturnValue = success.ToString() }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(null, ex) { Description = description }, true);
            }

            return this;
        }               

        public async Task<TestCheck<T, TData>> TestIsTrueAsync(string description, Func<TData, Task<bool>> functionToTest)
        {
            try
            {
                var success = await functionToTest(_testData).ConfigureAwait(false);

                Add(new TestCheck(success) { Description = description, ReturnValue = success.ToString() }, true);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex);
                Add(new TestCheck(null, ex) { Description = description }, true);
            }

            return this;
        }

        private string GetEndPointName<TOut>(Expression<Func<T, TOut>> expression)
        {
            var methodCallExpression = (expression.Body as MethodCallExpression);
            var memberExpression = (expression.Body as MemberExpression);

            return methodCallExpression == null ? GetMethodName(memberExpression) : GetMethodName(methodCallExpression);
        }

        private string GetMethodName(MemberExpression memberExpression)
        {
            return $"{ObjectName}.{memberExpression.Member.Name}";
        }

        private string GetMethodNameFallback(MethodCallExpression methodCallExpression)
        {
            var method = methodCallExpression.ToString();

            if (methodCallExpression.Object != null)
            {
                method = method.Replace(methodCallExpression.Object.ToString(), ObjectName);
            }

            return method;
        }

        private string GetMethodName(MethodCallExpression methodCallExpression)
        {
            try
            {
                if (methodCallExpression.Object == null && methodCallExpression.Arguments.Count == 1 && methodCallExpression.Arguments[0].NodeType == ExpressionType.MemberAccess)
                {
                    var method = (methodCallExpression.Arguments[0] as MemberExpression).Member.Name;

                    return $"{ObjectName}.{method}";
                }

                if (methodCallExpression.Arguments.Count == 1 && methodCallExpression.Arguments[0].NodeType == ExpressionType.Call)
                {
                    var expression = methodCallExpression.Arguments[0] as MethodCallExpression;

                    if(expression != null)
                    {
                        methodCallExpression = expression;
                    }
                }

                if (methodCallExpression.Arguments.Count == 2 && methodCallExpression.Arguments[1].NodeType == ExpressionType.Quote)
                {
                    var method = (methodCallExpression.Arguments[0] as MemberExpression).Member.Name;

                    return $"{ObjectName}.{method}";
                }

                if(methodCallExpression.Arguments.Count == 0)
                {
                    return GetMethodNameFallback(methodCallExpression);
                }

                var paramTypes = methodCallExpression.Arguments.Select(s => s.Type).ToArray();
                var parameters = typeof(T).GetMethod(methodCallExpression.Method.Name, paramTypes)?.GetParameters();

                return $"{ObjectName}.{methodCallExpression.Method.Name}({string.Join(",", parameters.Select(s => $"{s.ParameterType.Name} {s.Name}"))})";
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex);
            }

            return "N/A";
        }

        private string GetParamsStringForMethod(MethodCallExpression methodCallExpression)
        {
            try
            {
                if (methodCallExpression == null)
                    return null;

                if (methodCallExpression.Object == null && methodCallExpression.Arguments.Count == 1 && methodCallExpression.NodeType == ExpressionType.Call)
                {

                    var expression = methodCallExpression.Arguments[0] as MethodCallExpression;

                    if (expression != null)
                    {
                        methodCallExpression = expression;
                    }
                }

                if (methodCallExpression.Arguments.Count == 1 && methodCallExpression.Arguments[0].NodeType == ExpressionType.Call)
                {
                    var expression = methodCallExpression.Arguments[0] as MethodCallExpression;

                    if (expression != null)
                    {
                        methodCallExpression = expression;
                    }
                }

                if (methodCallExpression.Arguments.Count == 2 && methodCallExpression.Arguments[1].NodeType == ExpressionType.Quote)
                {
                    return methodCallExpression.Arguments[1].ToString();
                }

                if (methodCallExpression.Arguments.Count == 0)
                    return null;
                    
                var parameters = methodCallExpression.Arguments;
                var paramTypes = methodCallExpression.Arguments.Select(s => s.Type).ToArray();
                var methodParameters = typeof(T).GetMethod(methodCallExpression.Method.Name, paramTypes).GetParameters();

                var i = 0;
                var parameterParts = new List<string>();

                foreach (var p in parameters)
                {
                    System.Linq.Expressions.MemberExpression memberExpression = null;
                    object value;
                    string paramName;

                    memberExpression = p as System.Linq.Expressions.MemberExpression;
                    var propertyName = memberExpression?.Member?.Name;

                    try
                    { 
                        value = _testData.GetType().GetProperty(propertyName).GetValue(_testData);
                        paramName = $"({propertyName})";
                    }
                    catch(Exception ex)
                    {
                        //Might be done on purpose

                        _logger?.LogWarning(ex, $"Error getting a value for param: {i}({propertyName}) from {_testData}");
                        value = "???";
                        paramName = $"({propertyName})";
                    }

                    parameterParts.Add($"{methodParameters[i].Name}={value}{paramName}");
                    i++;
                }

                return String.Join(",", parameterParts);
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex);
            }

            return $"{methodCallExpression}";
        }
    }
}