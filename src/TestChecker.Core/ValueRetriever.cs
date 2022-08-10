using System;
using System.Linq;

namespace TestChecker.Core
{
    public class ValueRetriever
    {
        private readonly Func<dynamic, bool?> _isObject;
        private readonly Func<dynamic, object> _getValue;
        public int MinDepth { get; private set; }

        public ValueRetriever(Func<dynamic, object> getValue, int minDepth = 1)
        {
            _isObject = obj => null;
            _getValue = getValue;
            MinDepth = minDepth;
        }

        public ValueRetriever(Func<dynamic, bool?> isObject, Func<dynamic, object> getValue, int minDepth = 1)
        {
            _isObject = isObject;
            _getValue = getValue;
            MinDepth = minDepth;
        }        

        internal object GetValue(dynamic obj)
        {
            return _getValue(obj);
        }

        internal bool? IsObject(dynamic obj)
        {
            return _isObject(obj);
        }

    }
}