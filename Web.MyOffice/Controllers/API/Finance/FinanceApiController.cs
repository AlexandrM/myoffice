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
    public class FinanceController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage List(Guid id)
        {
            Project p = db.Projects
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Tasks)
                .Include(x => x.Members)
                .Include(x => x.Members.Select(z => z.Member))
                .FirstOrDefault(x => x.Id == id & (x.AuthorId == UserId | x.Members.Select(z => z.Member.MainMemberId).Contains(UserId)));

            var rests = new List<dynamic>();
            if (p.Members.FirstOrDefault(x => x.Member.MainMemberId == UserId & x.MemberType != ProjectMemberType.Member) != null)
            {
                var q = (from rep in db.MemberDayReports.Where(x => x.ProjectId == p.Id)
                         group rep by new { rep.ProjectId } into g
                         select new
                         {
                             Amount = g.Sum(x => x.Amount * x.Value),
                             ProjectId = g.Key.ProjectId,
                         })
                        .Concat(from rep in db.MemberPayments.Where(x => x.ProjectId == p.Id)
                                group rep by new { rep.ProjectId } into g
                                select new
                                {
                                    Amount = g.Sum(x => -x.Amount),
                                    ProjectId = g.Key.ProjectId,
                                })
                         .GroupBy(x => new { x.ProjectId })
                         .Select(z => new
                         {
                             Amount = z.Sum(x => x.Amount),
                             ProjectId = z.Key.ProjectId,
                         })
                         .Where(x => x.Amount != 0)
                         //.OrderBy(x => x.Project.Name)
                         .ToList();

                foreach (var item in q)
                {
                    dynamic rest = item.ToDynamic();
                    rest.Details = new List<dynamic>();
                    rest.Currency = db.Currencies.FirstOrDefault(x => x.UserId == UserId).LocalCurrency(db);
                    rest.CurrencyId = rest.Currency.Id;
                    rests.Add(rest);
                    var dt = DateTime.Now;
                    decimal amount = item.Amount;
                    while (true)
                    {
                        if (amount <= 0)
                            break;

                        var reports = db.MemberDayReports
                            .AsNoTracking()
                            .Where(x => x.ProjectId == item.ProjectId & x.DateTime < dt).OrderByDescending(x => x.DateTime).Take(10).ToList();
                        if (reports.Count == 0)
                            break;
                        foreach (var report in reports)
                        {
                            dt = report.DateTime;
                            amount = amount - (report.Amount * report.Value);

                            dynamic rep = report.ToDynamic();
                            rep.IsMy = report.UserId == UserId;
                            rest.Details.Add(rep);

                            rep.Rest = 0;
                            if (amount < 0)
                                rep.Rest = (report.Amount * report.Value) + amount;
                            if (amount <= 0)
                                break;
                        }
                    }
                }
            }

            var model = p.ToDynamic();
            model.TasksCount = p.Tasks == null ? 0 : p.Tasks.Count;
            model.IsMy = p.AuthorId == UserId;
            model.Author = p.Author.LocalMember;
            model.Currencies = db.Currencies.Where(x => x.UserId == UserId).ToList();
            model.Rests = rests;
            model.Members = p.Members.Select(m => new
            {
                MemberId = m.MemberId,
                ProjectId = m.ProjectId,
                Member = m.Member.LocalMember,
                MemberType = m.MemberType,
                MemberTypeS = m.MemberTypeS,
            });

            return ResponseObject2Json(model);
        }
    }
}
