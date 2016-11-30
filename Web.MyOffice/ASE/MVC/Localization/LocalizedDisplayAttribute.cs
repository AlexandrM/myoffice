using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Web;

namespace ASE.MVC
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class LocalizedDisplayAttribute : DisplayNameAttribute
    {
        public static ResourceManager ResourceManager { get; set; }

        private string resourceName = "";
        
        public LocalizedDisplayAttribute(string resourceName): base()
        {
            this.resourceName = resourceName;
        }

        public LocalizedDisplayAttribute(ResourceManager resourceManager)
        {
            ResourceManager = resourceManager;
        }

        public override string DisplayName {
            get 
            {
                return ResourceManager.GetString(this.resourceName); 
            }
        }

        public override int GetHashCode()
        {
            int r = 0;
            try
            {
                r = base.GetHashCode();
            }
            catch
            {
                throw new Exception("LocalizedDisplayAttribute.GetHashCode() for resource: " + resourceName);
            }
            return r;
        }
    }
}