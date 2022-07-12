using System;
using System.Collections.Generic;
using System.Text;

namespace TestChecker.Core.Extensions
{
    internal static class TypeExtensions
    {
        public static string GetFullName(this Type type)
        {
            return $"{type.Namespace}.{type.Name}";
        }
    }
}
