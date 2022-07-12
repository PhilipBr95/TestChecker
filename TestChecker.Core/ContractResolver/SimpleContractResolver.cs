using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace TestChecker.Core.ContractResolver
{
    internal class SimpleContractResolver : DefaultContractResolver
    {
        public static bool IsNice(System.Type propertyType)
        {
            return propertyType == typeof(string)
                || propertyType.IsValueType
                || propertyType.IsEnum;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var propertyType = property.PropertyType;
            if (IsNice(propertyType))
            {
                property.ShouldSerialize = instance => true;
            }
            else
            {
                property.ShouldSerialize = instance =>
                {
                    return false;
                };
            }
            return property;
        }
    }
}