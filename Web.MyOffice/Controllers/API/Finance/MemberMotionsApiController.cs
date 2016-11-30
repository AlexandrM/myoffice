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
using System.Data.SqlClient;
using System.Dynamic;

using ASE;
using ASE.EF;
using ASE.MVC;
using ASE.Json;

using Web.MyOffice.Data;
using Web.MyOffice.Models;

namespace Web.MyOffice.Controllers.API
{
    public class MemberMotionsController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage List(Guid projectId, DateTime dateFrom, DateTime dateTo)
        {
            dateFrom = dateFrom.Date;
            dateTo = dateTo.Date.AddDays(1).AddMilliseconds(-1);
            Project p = db.Projects
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Members)
                .Include(x => x.Members.Select(z => z.Member))
                .FirstOrDefault(x => x.Id == projectId & (x.AuthorId == UserId | x.Members.Select(z => z.Member.MainMemberId).Contains(UserId)));


            List<MemberMotion> motions = new List<MemberMotion>();
            motions.AddRange(
                db.MemberDayReports
                .AsNoTracking()
                .Where(x => 
                    x.DateTime >= dateFrom 
                    & x.DateTime <= dateTo 
                    & x.ProjectId == projectId
                    ).ToList());
            motions.AddRange(db.MemberPayments
                .AsNoTracking()
                .Where(x =>
                    x.DateTime >= dateFrom
                    & x.DateTime <= dateTo
                    & x.ProjectId == projectId
                    ).ToList());
            motions = motions.OrderBy(x => x.DateTime).ToList();

            var debtStart = db.MemberDayReports.Where(x => x.ProjectId == projectId & x.DateTime < dateFrom).Sum(x => (decimal?)x.Amount * (decimal?)x.Value) ?? 0;
            debtStart -= db.MemberPayments.Where(x => x.ProjectId == projectId & x.DateTime < dateFrom).Sum(x => (decimal?)x.Amount) ?? 0;

            var debtEnd = db.MemberDayReports.Where(x => x.ProjectId == projectId & x.DateTime < dateTo).Sum(x => (decimal?)x.Amount * (decimal?)x.Value) ?? 0;
            debtEnd -= db.MemberPayments.Where(x => x.ProjectId == projectId & x.DateTime < dateTo).Sum(x => (decimal?)x.Amount) ?? 0;

            var model = p.ToDynamic();
            model.Motions = motions;
            model.IsMy = p.AuthorId == UserId;
            model.Currencies = db.Currencies.Where(x => x.UserId == UserId).ToList();
            model.AuthorFullName = p.Author.FullName;
            model.DebtStart = debtStart;
            model.DebtEnd = debtEnd;
            model.UserId = UserId;

            return ResponseObject2Json(model);
        }
    }
}
