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


namespace Web.MyOffice.Controllers
{
    [Authorize]
    //[RequireHttps]
    public class ProjectController : ControllerAdv<DB>
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Members()
        {
            return View();
        }
    }
}
