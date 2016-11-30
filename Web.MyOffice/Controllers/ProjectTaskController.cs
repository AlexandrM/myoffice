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
using System.Threading.Tasks;


namespace Web.MyOffice.Controllers
{
    [Authorize]
    [RequireHttps]
    public class ProjectTaskController : ControllerAdv<DB>
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }
    }
}
