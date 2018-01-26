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
using System.Data.Entity;


using Web.MyOffice.Data;
using Web.MyOffice.Models;

namespace Web.MyOffice.Controllers.API
{
    public class MotionController : ControllerApiAdv<DB>
    {
        public class Filter
        {
            public Guid AccountId { get; set; }
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }

        [HttpGet]
        public HttpResponseMessage List([FromUri] Filter filter)
        {
            Guid uid = Guid.Empty;
            Guid.TryParse(this.Request.Headers.FirstOrDefault(x => x.Key == "UID").Value.FirstOrDefault(), out uid);

            var budgets = db.Budgets
                .Where(x => x.Users.FirstOrDefault(z => z.User.APISessionId == uid) != null | x.Owner.APISessionId == uid)
                .Select(x => x.Id)
                .ToList();

            filter.DateFrom = filter.DateFrom.Date;
            filter.DateTo = filter.DateTo.Date.AddDays(1);

            var m = db.Motions
                .Include(x => x.Item)
                .Where(x => budgets.Contains(x.Account.Category.BudgetId))
                .Where(x => x.AccountId == filter.AccountId)
                .Where(x => x.DateTime >= filter.DateFrom & x.DateTime < filter.DateTo)
                .ToList();

            return ResponseObject2Json(m);
        }
    }
}
