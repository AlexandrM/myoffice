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

namespace Web.MyOffice.Controllers.API
{
    public class MemberController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage Item(Guid? id)
        {
            Project p;
            if (id.HasValue)
                p = db.Projects.FirstOrDefault(x => x.Id == id.Value & (x.AuthorId == UserId | x.Members.Select(z => z.Member.MainMemberId).Contains(UserId)));
            else
                p = new Project { DateTime = DateTime.Now, State = ProjectState.New };

            var s = JsonConvert.SerializeObject(new 
                {
                    Id = p.Id,
                    State = p.State,
                    StateS = p.State.ToDisplayName(),
                    Name = p.Name,
                    Description = p.Description,
                    DateTime = p.DateTime,
                    DateTimeS = p.DateTime.ToStringD(),
                    TasksCount = p.Tasks == null ? 0 : p.Tasks.Count,
                    IsMy = p.AuthorId == UserId,
                    CanApprove = p.AuthorId == UserId,
                    Members = p.Members.Select(m => new {
                        Id = m.Member.Id,
                        FullName = m.Member.FullName,
                        Email = m.Member.Email,
                        MemberType = m.MemberType,
                        MemberTypeS = m.MemberType.ToDisplayName(),
                    }).ToList()
                },
                new JsonSerializerSettings
                {
                });

            return new HttpResponseMessage() { Content = new StringContent(s, Encoding.UTF8, "application/json") };
        }

        [HttpGet]
        public HttpResponseMessage List()
        {
            IQueryable<Member> q;
            q = db.Members.Where(x => x.UserId == UserId);

            var o = q.OrderBy(x => x.FullName).ToList().Select(x =>
            {
                /*var memberRate = x.MemberRates(db).OrderByDescending(z => z.DateTime).Take(1).FirstOrDefault();
                if (memberRate == null)
                {
                    memberRate = db.MemberRates
                    .AsNoTracking()
                    .Include(z => z.Currency)
                    .Where(z => z.UserId == x.MainMemberId & z.Member.MainMemberId == UserId).OrderByDescending(z => z.DateTime).Take(1).FirstOrDefault();
                }
                memberRate = memberRate ?? new MemberRate { Id = Guid.Empty, Value = 0, Currency = new Currency() };*/

                return new
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    FullName = x.FullName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    /*Rate = new
                    {
                        Value = memberRate.Value,
                        CurrencyShortName = memberRate.Currency.ShortName,
                        SetAs = ((memberRate.UserId != UserId) & (memberRate.UserId != Guid.Empty)) ? x.Email : "-",
                        RateType = memberRate.RateType.ToDisplayName()
                    }*/
                };
            }).ToList();

            var s = JsonConvert.SerializeObject(o,
                new JsonSerializerSettings
                {
                });

            return new HttpResponseMessage() { Content = new StringContent(s, Encoding.UTF8, "application/json") };
        }

        [HttpPost]
        public HttpResponseMessage Post(Project model)
        {
            model.Id = Guid.NewGuid();
            model.AuthorId = UserId;
            db.Projects.Add(model);
            db.SaveChanges();

            var s = JsonConvert.SerializeObject(model);
            return new HttpResponseMessage() { Content = new StringContent(s, Encoding.UTF8, "application/json") };
        }

        [HttpPut]
        public HttpResponseMessage Put(Project model)
        {
            db.AttachModel<Project>(model, 
                x => x.Id,
                x => x.Name,
                x => x.DateTime,
                x => x.Description,
                x => x.State
                );
            db.SaveChanges();

            //var s = JsonConvert.SerializeObject(model);
            return new HttpResponseMessage() { Content = new StringContent("", Encoding.UTF8, "application/json") };
        }

        [HttpDelete]
        public HttpResponseMessage Delete(Guid id, [FromUri]Guid? memberId, [FromUri]string mode)
        {
            Project model = db.Projects.FirstOrDefault(x => x.Id == id & (x.AuthorId == UserId | x.Members.Select(z => z.Member.MainMemberId).Contains(UserId)));

            if (mode == "deleteMember")
            {
                db.ProjectMembers.Remove(model.Members.FirstOrDefault(x => x.MemberId == memberId.Value));
            }
            else
            {
                db.Projects.Remove(model);
            }
            db.SaveChanges();

            //var s = JsonConvert.SerializeObject(model);
            return new HttpResponseMessage() { Content = new StringContent("", Encoding.UTF8, "application/json") };
        }
    }
}
