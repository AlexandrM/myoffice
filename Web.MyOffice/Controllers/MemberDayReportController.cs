using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using ASE;
using ASE.EF;
using ASE.MVC;

using Web.MyOffice.Data;
using Web.MyOffice.Models;

namespace Web.MyOffice.Controllers.References
{
    [Authorize]
    [RequireHttps]
    public class MemberDayReportController : ControllerAdv<DB>
    {
        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }
    }
}
