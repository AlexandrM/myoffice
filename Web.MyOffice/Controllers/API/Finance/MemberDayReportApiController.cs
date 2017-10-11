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

using ASE;
using ASE.EF;
using ASE.MVC;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using ASE.Json;
using System.Data.SqlClient;
using System.Dynamic;

namespace Web.MyOffice.Controllers.API
{
    public class MemberDayReportController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage Get([FromUri]DateTime? dateFrom, [FromUri]DateTime? dateTo)
        {
            dateFrom = dateFrom ?? DateTime.Now.Date;
            dateTo = dateTo ?? DateTime.Now.Date;
            dateTo = dateTo.Value.AddDays(1);

            var model = db.MemberDayReports
                .Include(x => x.Member)
                .Include(x => x.Project)
                .Include(x => x.Project.Members)
                .Include(x => x.Project.Members.Select(z => z.Member))
                .Where(x => x.DateTime >= dateFrom.Value & x.DateTime < dateTo)
                .OrderBy(x => x.DateTime)
                .ToList();

            var r = new {
                Details = model,
                Currencies = db.Currencies.Where(x => x.UserId == UserId).ToList()
            };

            return ResponseObject2Json(r);
        }

        [HttpGet]
        public HttpResponseMessage Get([FromUri]Guid? id, [FromUri]Guid? projectId)
        {
            var model = db.MemberDayReports
                .Include(x => x.Member)
                .Include(x => x.Project)
                .Include(x => x.Project.Members)
                .Include(x => x.Project.Members.Select(z => z.Member))
                .FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                model = new MemberDayReport
                {
                    Id = Guid.Empty,
                    DateTime = DateTime.Now,
                    ProjectId = projectId.Value,
                    Project = db.Projects
                        .Include(x => x.Members)
                        .FirstOrDefault(x => x.Id == projectId.Value),
                };
                model.Member = db.ProjectMembers.Where(x => x.ProjectId == projectId & x.MemberType == ProjectMemberType.Customer).Select(z => z.Member).FirstOrDefault();
                model.MemberId = model.Member == null ? (Guid?)null : model.Member.Id;
                model.RateType = model.Project.RateType;
                model.Value = model.Project.RateValue;
            }

            var r = model.ToDynamic();
            r.Currency = db.Currencies.FirstOrDefault(x => x.UserId == UserId & x.CurrencyType == model.Project.RateCurrencyType);
            r.Members = db.ProjectMembers.Where(x => x.ProjectId == projectId & x.MemberType == ProjectMemberType.Customer).Select(z => z.Member).ToList();

            return ResponseObject2Json(r);
        }

        [HttpPost]
        public HttpResponseMessage Post(MemberDayReport model)
        {
            var p = db.ProjectMembers.Where(x => x.ProjectId == model.ProjectId & x.Member.MainMemberId == UserId & x.MemberType == ProjectMemberType.Implementer).Select(x => x.Project).FirstOrDefault();

            model.Id = Guid.NewGuid();
            model.UserId = UserId;
            model.Member = null;
            model.Project = null;
            model.ProjectId = p.Id;
            db.MemberDayReports.Add(model);
            db.SaveChanges();

            return ResponseObject2Json(null);
        }

        [HttpPut]
        public HttpResponseMessage Put(MemberDayReport model)
        {
            var p = db.ProjectMembers
                .AsNoTracking()
                .Where(x => x.ProjectId == model.ProjectId & x.Member.MainMemberId == UserId & x.MemberType == ProjectMemberType.Implementer).Select(x => x.Project)
                .FirstOrDefault();

            if (p != null)
            {
                model.Member = null;
                model.Project = null;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return ResponseObject2Json(null);
        }

        [HttpDelete]
        public HttpResponseMessage Delete(Guid id)
        {
            db.MemberDayReports.Remove(db.MemberDayReports.FirstOrDefault(x => x.Id == id && x.UserId == UserId));
            db.SaveChanges();

            return ResponseObject2Json(null);
        }
    }
}
