using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

using ASE.MVC;
using ASE.EF;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using System.Threading.Tasks;
using Web.MyOffice.Res;


namespace Web.MyOffice.Controllers
{
    [Authorize]
    //[RequireHttps]
    public class FinanceController : ControllerAdv<DB>
    {
        public ActionResult List(string lang)
        {
            if (CultureHelper.Cultures.ContainsKey(lang))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
            }
            return View();
        }
    }
}
