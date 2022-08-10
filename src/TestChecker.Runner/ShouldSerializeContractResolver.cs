using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestChecker.Runner
{
    internal class ShouldSerializeContractResolver : DefaultContractResolver
    {            
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof(string)) return property;

            if (property.PropertyType.GetInterface(nameof(IEnumerable)) != null)
                property.ShouldSerialize = 
                    instance => (instance?.GetType().GetProperty(property.PropertyName).GetValue(instance) as IEnumerable<object>)?.Count() > 0;

            return property;
        }
    }

}