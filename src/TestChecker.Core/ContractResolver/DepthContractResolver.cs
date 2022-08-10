using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TestChecker.Core.ContractResolver
{
    public class DepthContractResolver : DefaultContractResolver
    {
        private readonly Func<bool> _includeProperty;

        public DepthContractResolver(Func<bool> includeProperty)
        {
            _includeProperty = includeProperty;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var shouldSerialize = property.ShouldSerialize;
            property.ShouldSerialize = obj =>
            {
                return _includeProperty() ||
                                (shouldSerialize != null &&
                                    SimpleContractResolver.IsNice(obj.GetType()));
            };

            return property;
        }
    }
}
