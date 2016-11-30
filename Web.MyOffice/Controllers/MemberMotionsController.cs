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
using System.IO;
using Web.MyOffice.Res;

namespace Web.MyOffice.Controllers.References
{
    [Authorize]
    [RequireHttps]
    public class MemberMotionsController : ControllerAdv<DB>
    {
        public ActionResult List()
        {
            return View();
        }
    }
}
