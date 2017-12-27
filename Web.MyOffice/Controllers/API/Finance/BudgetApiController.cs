using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;
using System.Data.Entity;
using System.Dynamic;

using ASE;
using ASE.EF;
using ASE.MVC;
using ASE.Json;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using System.Data.SqlClient;

namespace Web.MyOffice.Controllers.API
{
    /*public class BxxudgetController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage List()
        {
            var model = db.BudgetUsers
                .Include(x => x.Budget)
                .Include(x => x.Budget.CategoryAccounts)
                .Include(x => x.Budget.CategoryAccounts.Select(z => z.Accounts))
                .Where(x => x.Budget.OwnerId == UserId || x.UserId == UserId)
                .ToList();

            return ResponseObject2Json(model);
        }
    }*/
}
