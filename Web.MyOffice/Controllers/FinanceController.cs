using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
    [RequireHttps]
    public class FinanceController : ControllerAdv<DB>
    {
        public ActionResult List()
        {
            return View();
        }
    }
}
