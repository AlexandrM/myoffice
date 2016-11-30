using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Ajax.Utilities;

namespace ASE.Json
{
    public class DynamicContractResolver : DefaultContractResolver
    {
        private readonly Func<Type, string, bool> filter;
        private readonly Type rootType;

        public DynamicContractResolver(Func<Type, string, bool> filter)
        {
            this.filter = filter;
        }

        public DynamicContractResolver(Func<Type, string, bool> filter, Type rootType)
        {
            this.filter = filter;
            this.rootType = rootType;
        }

        public DynamicContractResolver(Type rootType)
        {
            this.rootType = rootType;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);
            if ((rootType != null) && ((type != rootType) & (type.BaseType != rootType)))
                list.Clear();

            if (filter != null)
                return list.Where(x => filter(type, x.PropertyName)).ToList();
            else
                return list;
        }
    }

    public class ContractResolverWithProcessor : DefaultContractResolver
    {
        private Func<JsonProperty, MemberInfo, MemberSerialization, JsonProperty> processor;

        public ContractResolverWithProcessor(Func<JsonProperty, MemberInfo, MemberSerialization, JsonProperty> processor)
        {
            this.processor = processor;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            return processor(base.CreateProperty(member, memberSerialization), member, memberSerialization);
        }
    }
}