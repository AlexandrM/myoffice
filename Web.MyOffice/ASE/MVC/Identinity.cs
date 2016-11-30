using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;

namespace ASE.MVC
{
    public static class IdentinityExtension
    {
        public static ResourceManager ResourceManager { get; set; }

        public static IEnumerable<string> LocalizedErrors(this IdentityResult identityResult)
        {
            var ret = new List<string>();
            foreach(var item in identityResult.Errors)
            {
                if ((ResourceManager != null))
                {
                    var s = ResourceManager.GetString(item.Replace(" ", "").Replace(".", ""));
                    if (s != null)
                    {
                        ret.Add(s);
                        continue;
                    }
                }
                ret.Add(item);
            }

            return ret;
        }
    }
}