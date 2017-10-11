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
    public class DebtsController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage List(int mode)
        {
            var eMode = mode == 1 ? ProjectMemberType.Implementer : ProjectMemberType.Customer;

            var currencies = db.Currencies.Where(x => x.UserId == UserId).ToArray();

            var qr = db.Projects
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Members)
                .Include(x => x.Members.Select(z => z.Member))
                .Where(x => x.Members.FirstOrDefault(z => z.Member.MainMemberId == UserId & z.MemberType == eMode) != null)
                .ToList()
                .Select(x => new {
                    Project = x,
                    Amount = db.MemberDayReports.Where(z => z.ProjectId == x.Id).Select(z => z.Amount * z.Value).DefaultIfEmpty(0).Sum() - db.MemberPayments.Where(z => z.ProjectId == x.Id).Select(z => z.Amount).DefaultIfEmpty(0).Sum(),
                })
                .Where(x => x.Amount != 0)
                .OrderBy(x => x.Project.Name)
                .ToArray(); 

            dynamic model = new object().ToDynamic();
            model.Rests = qr;
            model.Currencies = currencies;

            return ResponseObject2Json(model);
        }
    }
}
