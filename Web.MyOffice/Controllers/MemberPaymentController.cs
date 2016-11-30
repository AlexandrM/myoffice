using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using ASE;
using ASE.MVC;
using ASE.EF;

using Web.MyOffice.Data;
using Web.MyOffice.Models;

namespace Web.MyOffice.Controllers
{
    public class MemberPaymentController : ControllerAdv<DB>
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