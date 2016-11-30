using ASE.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ASE
{
    public static class ExtensionsReflection
    {
        public static TAttribute GetAttribute<TAttribute>(this object obj) where TAttribute : Attribute
        {
            var x = obj.GetType().GetCustomAttributes(typeof(TAttribute), false);
            if (x.Length == 0)
                return null;

            return x[0]  as TAttribute ?? null;
        }

        public static string ToDisplayName(this object obj)
        {
            if (obj.GetType().IsEnum)
            {
                var type = obj.GetType();
                var memInfo = type.GetMember(obj.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(LocalizedDisplayAttribute), false);
                if (attributes.Length == 0)
                    return obj.ToString();
                return ((LocalizedDisplayAttribute)attributes[0]).DisplayName;
            }
            var x = obj.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (x.Length == 0)
                return obj.ToString();

            var a = x[0] as DisplayNameAttribute;
            if (a == null)
                return obj.ToString();

            return a.DisplayName;
        }
    }
}