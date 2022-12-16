using System;
using System.Collections.Generic;
using System.Text;
using TestChecker.Core.Serialisation;

namespace TestChecker.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static T ConvertTo<T>(this object obj) where T : new()
        {
            //todo - surely we dont need to do this..just cast
            var json = JsonSerialiser.Serialise(obj);
            return JsonSerialiser.Deserialise<T>(json);
        }

    }
}
