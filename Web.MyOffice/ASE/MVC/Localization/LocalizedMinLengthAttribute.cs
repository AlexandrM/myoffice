using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC
{
    public class LocalizedMinLengthAttribute : MinLengthAttribute
    {
        public static ResourceManager ResourceManager { get; set; }
        public static string ResourceName = "MinLength";
        //private static string FormatString = null;

        public LocalizedMinLengthAttribute(int length): base(length)
        {
            //if ((ResourceManager != null) & (FormatString == null))
            var FormatString = ResourceManager.GetString(ResourceName);

            if ((this.ErrorMessageResourceType == null) & (FormatString != null))
                base.ErrorMessage = String.Format(FormatString, length);

            
        }
    }
}