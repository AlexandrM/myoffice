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
    public class MemberPaymentController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage Get([FromUri]DateTime? dateFrom, [FromUri]DateTime? dateTo)
        {
            dateFrom = dateFrom ?? DateTime.Now.Date;
            dateTo = dateTo ?? DateTime.Now.Date;
            dateTo = dateTo.Value.AddDays(1);

            var model = db.MemberPayments
                .AsNoTracking()
                .Include(x => x.Member)
                .Include(x => x.Project)
                .Include(x => x.Project.Members)
                .Include(x => x.Project.Members.Select(z => z.Member))
                .Where(x => x.DateTime >= dateFrom.Value & x.DateTime < dateTo)
                .Where(x => x.Project.Members.Any(z => z.MemberId == UserId))
                .OrderBy(x => x.DateTime)
                .ToList();

            var r = new
            {
                Details = model,
                Currencies = db.Currencies.Where(x => x.UserId == UserId).ToList()
            };

            return ResponseObject2Json(r);
        }

        [HttpGet]
        public HttpResponseMessage Get([FromUri]Guid? id, [FromUri]Guid? projectId)
        {
            var model = db.MemberPayments
                .AsNoTracking()
                .Include(x => x.Member)
                .Include(x => x.Project)
                .Where(x => x.Project.Members.Any(z => z.MemberId == UserId))
                .FirstOrDefault(x => x.Id == id);

            if (model == null)
            {
                model = new MemberPayment
                {
                    Id = Guid.Empty,
                    DateTime = DateTime.Now,
                    ProjectId = projectId.Value,
                    Project = db.Projects.FirstOrDefault(x => x.Id == projectId.Value),
                };
                model.Member = db.ProjectMembers.Where(x => x.ProjectId == projectId & x.MemberType == ProjectMemberType.Customer).Select(z => z.Member).FirstOrDefault();
                model.MemberId = model.Member == null ? (Guid?)null : model.Member.Id;
            }

            var r = ExtensionsOject.ToDynamic(model);
            r.Currency = db.Currencies.FirstOrDefault(x => x.UserId == UserId & x.CurrencyType == model.Project.RateCurrencyType);

            return ResponseObject2Json(r);
        }

        [HttpPost]
        public HttpResponseMessage Post(MemberPayment model)
        {
            var p = db.ProjectMembers.FirstOrDefault(x => x.ProjectId == model.ProjectId & x.MemberType == ProjectMemberType.Implementer & x.Member.MainMemberId == UserId);
            if (p != null)
            {
                model.Member = null;
                model.Project = null;
                model.Id = Guid.NewGuid();
                model.UserId = UserId;
                db.MemberPayments.Add(model);
                db.SaveChanges();
            }

            return ResponseObject2Json(null);
        }

        [HttpPut]
        public HttpResponseMessage Put(MemberPayment model)
        {
            var p = db.ProjectMembers.FirstOrDefault(x => x.ProjectId == model.ProjectId & x.MemberType == ProjectMemberType.Implementer & x.Member.MainMemberId == UserId);
            var m = db.MemberPayments.FirstOrDefault(x => x.Id == model.Id & x.UserId == UserId);
            if ((p != null) & (m != null))
            {
                db.AttachModel(model,
                    x => x.DateTime,
                    x => x.Amount
                    );
                
                db.SaveChanges();
            }

            return ResponseObject2Json(null);
        }

        [HttpDelete]
        public HttpResponseMessage Delete(Guid id)
        {
            var model = db.MemberPayments.Find(id);
            if (model.UserId == UserId)
            {
                db.MemberPayments.Remove(model);
                db.SaveChanges();
            }

            return ResponseObject2Json(null);
        }
    }
}
