using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC
{
    public class LocalizedRequiredAttribute : RequiredAttribute
    {
        public static ResourceManager ResourceManager { get; set; }
        public static string ResourceName = "RequiredFormat";
        //private static string FormatString = null;

        public LocalizedRequiredAttribute(string name)
        {
            Name = name;
        }

        public LocalizedRequiredAttribute()
        {

        }

        public string Name { get; set; }

        public override string FormatErrorMessage(string name)
        {
            //if ((ResourceManager != null) & (FormatString == null))
            var FormatString = ResourceManager.GetString(ResourceName);

            if ((this.ErrorMessageResourceType == null) & (FormatString != null))
                return String.Format(FormatString, name);
                
            return base.FormatErrorMessage(name);
        }
    }
}