using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASE.MVC;
using Web.MyOffice.Data;

namespace Web.MyOffice.Controllers.MyBank
{
    [ViewEngineAdv("MyBank/")]
    public class ExpenditureController : ControllerAdv<DB>
    {
        public ActionResult Items()
        {
            return View();
        }
    }
}