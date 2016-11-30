using ASE.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.MyOffice.Controllers
{
    public class GlobalSelectController : Controller
    {
        // GET: GlobalSelect
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Select(string value, string type)
        {
            Type tType = Type.GetType(type);
            var attrs = tType.GetCustomAttributes(typeof(SelectAttribute), true);
            SelectAttribute attr = (SelectAttribute)attrs[0];

            return new JsonResult
            {
                Data = new
                {
                    Action = attr.Action,
                    Controller = attr.Controller,
                    Title = ASE.ResourceHelper.GetResourceLookup(attr.ResourceType, attr.ResourceName)
                }
            };
        }
    }
}