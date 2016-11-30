using ASE.Json;
using ASE.MVC;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyBank.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Web.MyOffice.Data;

namespace Web.MyOffice.Controllers.API
{
    public class BudgetController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage List()
        {
            Guid uid = Guid.Empty;
            Guid.TryParse(this.Request.Headers.FirstOrDefault(x => x.Key == "UID").Value.FirstOrDefault(), out uid);

            var m = db.Budgets
                .Where(x => x.Users.FirstOrDefault(z => z.User.APISessionId == uid) != null | x.Owner.APISessionId == uid)
                .ToList();

            return ResponseObject2Json(m);
        }

        [HttpGet]
        public HttpResponseMessage List(Guid id)
        {
            Guid uid = Guid.Empty;
            Guid.TryParse(this.Request.Headers.FirstOrDefault(x => x.Key == "UID").Value.FirstOrDefault(), out uid);

            var m = db.Budgets
                .Where(x => x.Id == id)
                .Where(x => x.Users.FirstOrDefault(z => z.User.APISessionId == uid) != null | x.Owner.APISessionId == uid)
                .FirstOrDefault();

            return ResponseObject2Json(m);
        }
    }
}
