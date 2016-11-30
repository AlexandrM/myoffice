using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASE
{
    public class DynamicTools
    {
        public static object GetValue(dynamic obj, string property)
        {
            return obj.GetType().GetProperty(property).GetValue(obj, null);
        }
    }
}