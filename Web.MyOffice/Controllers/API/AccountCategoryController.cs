using ASE.MVC;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web.MyOffice.Data;

namespace Web.MyOffice.Controllers.API
{
    public class AccountCategoryController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage List()
        {
            Guid uid = Guid.Empty;
            Guid.TryParse(this.Request.Headers.FirstOrDefault(x => x.Key == "UID").Value.FirstOrDefault(), out uid);

            var budgets = db.Budgets
                .Where(x => x.Users.FirstOrDefault(z => z.User.APISessionId == uid) != null | x.Owner.APISessionId == uid)
                .Select(x => x.Id)
                .ToList();

            var m = db.CategoryAccounts
                .Where(x => budgets.Contains(x.BudgetId))
                .OrderBy(x => x.Name)
                .ToList();

            return ResponseObject2Json(m);
        }

        [HttpGet]
        public HttpResponseMessage List(Guid id)
        {
            Guid uid = Guid.Empty;
            Guid.TryParse(this.Request.Headers.FirstOrDefault(x => x.Key == "UID").Value.FirstOrDefault(), out uid);

            var budgets = db.Budgets
                .Where(x => x.Users.FirstOrDefault(z => z.User.APISessionId == uid) != null | x.Owner.APISessionId == uid)
                .Select(x => x.Id)
                .ToList();

            var m = db.CategoryAccounts
                .Where(x => x.Id == id)
                .Where(x => budgets.Contains(x.BudgetId))
                .OrderBy(x => x.Name)
                .FirstOrDefault();

            return ResponseObject2Json(m);
        }
    }
}
