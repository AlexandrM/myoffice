using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ASE.Json
{
    public static class ExtensionsJson
    {
        public static Dictionary<string, object> GetFiltredObject<T>(this object obj, params Expression<Func<T, object>>[] properties) where T : class
        {
            var ret = new Dictionary<string, object>();

            return ret;
        }
    }
}