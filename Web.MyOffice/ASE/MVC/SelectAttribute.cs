using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;

namespace ASE.MVC
{
    public class SelectAttribute: Attribute
    {
        public string Controller { get; set; }

        public Type ControllerType { get; set; }
        
        public string Action { get; set; }

        public Type ResourceType { get; set; }

        public string ResourceName  { get; set; }

        public SelectAttribute()
        {

        }
    }
}