using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC
{
    public class ViewEngineAdv: RazorViewEngine
    {
        public ViewEngineAdv()
        {
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var attr = controllerContext.Controller.GetAttribute<ViewEngineAdvAttribute>();
            if (attr != null)
                return new ViewEngineResult(CreateView(controllerContext, String.Format("~/Views/{0}{1}/{2}.cshtml", attr.SubPath, controllerContext.Controller.GetType().Name.Replace("Controller", ""), viewName), ""), this);
            
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            var attr = controllerContext.Controller.GetAttribute<ViewEngineAdvAttribute>();
            if (attr != null)
            {
                var virtualPath = String.Format("~/Views/{0}{1}/{2}.cshtml", attr.SubPath, controllerContext.Controller.GetType().Name.Replace("Controller", ""), partialViewName);
                if (FileExists(controllerContext, virtualPath))
                return new ViewEngineResult(CreatePartialView(controllerContext, virtualPath), this);
            }

            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }
    }

    public class ViewEngineAdvAttribute: Attribute
    {
        public string SubPath { get; set; }
        
        public ViewEngineAdvAttribute(string subPath)
        {
            SubPath = subPath;
        }
    }
}