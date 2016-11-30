using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ASE.MVC;
using MyBank.Models;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Drawing;
using Web.MyOffice.Data;

namespace MyBank.Controllers
{
    [Authorize]
    [RequireHttps]
    [ViewEngineAdv("MyBank/")]
    public class MyBankReportsController : ControllerAdv<DB>
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadReport(string name, Dictionary<string, string> param)
        {
            ViewData["name"] = name;
            foreach (string key in param.Keys)
                ViewData[key] = param[key];

            return new JsonResult
            {
                Data = new {
                    html = this.RenderPartialView("Report" + name, null)
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
