using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestChecker.Core.Enums;

namespace TestChecker.Core
{
    public class Recorder<T> : DispatchProxy
    {
        private T _decorated;
        private Recorder<T> _recorder;
        private CoverageMethod _coverageMethod;

        public Recorder()
        {
        }

        public Recorder(CoverageMethod coverageMethod)
        {
            _coverageMethod = coverageMethod;
        }

        private List<string> MethodCalls { get; set; } = new List<string>();
        public IEnumerable<string> Methods { get; private set; }

        public event EventHandler<Coverage> CoverageChanged;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod == null) throw new ArgumentException(nameof(targetMethod));            

            var result = targetMethod.Invoke(_decorated, args);
            var method = $"{typeof(T)} - {targetMethod}";
            
            if (_recorder?.Methods?.Contains(method) ?? false)
                _recorder?.MethodCalls.Add(method);

            OnCoverageChanged();

            return result;
        }

        public T CreateProxy(T decorated)
        {
            object proxy = Create<T, Recorder<T>>();
            ((Recorder<T>)proxy).SetParameters(decorated, this, _coverageMethod);

            return (T)proxy;
        }        

        private void SetParameters(T decorated, Recorder<T> recorder, CoverageMethod coverageMethod)
        {
            _decorated = decorated;
            _recorder = recorder;
            _coverageMethod = coverageMethod;

            Methods = GetEndPoints().Select(s => $"{typeof(T)} - {s}");
            _recorder.Methods = Methods;
        }

        private Coverage _coverage = null;

        private void OnCoverageChanged()
        {
            if (_recorder != null) _recorder.OnCoverageChanged();
            if (CoverageChanged == null) return;

            CoverageChanged(this, new Coverage(typeof(T).Name, _coverageMethod, MethodCalls.Distinct().ToList(), Methods.ToList()));
        }

        private IEnumerable<string> GetEndPoints()
        {
            switch(_coverageMethod)
            {
                case CoverageMethod.PropertiesOnly:
                    return typeof(T).GetProperties().Select(s => s.GetGetMethod().ToString());
                case CoverageMethod.MethodsOnly:
                    return typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(s => s.ToString());
                case CoverageMethod.MethodsAndProperties:
                    var methods = typeof(T).GetProperties().Select(s => s.GetGetMethod().ToString());
                    methods = methods.Union(typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(s => s.ToString()));

                    return methods;
                case CoverageMethod.UseTestableAttribute:
                    var testableMethods = typeof(T).GetProperties().Where(w => w.GetCustomAttributes(typeof(TestableAttribute), true).Length > 0).Select(s => s.GetGetMethod().ToString());
                    testableMethods = testableMethods.Union(typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(w => w.GetCustomAttributes(typeof(TestableAttribute), true).Length > 0).Select(s => s.ToString()));

                    return testableMethods;
            }

            return null;
        }
    }
}


/*
 * 
 * WebAPI contains:
 *      Controllers
 *      +
 *      Services/Clients
 *      
 * TestChecks should contain a ref to all of them!!
 * FakeItEasy needs to look for all the classes that implement ITestable and check they've been hit
 * 
 * 
 * Do we need a separate TestCheck class?
 */
