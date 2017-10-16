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
    public class ProjectController : ControllerApiAdv<DB>
    {
        [HttpGet]
        public HttpResponseMessage Item(Guid? id)
        {
            Project p;
            if (id.HasValue)
                p = db.Projects
                    .Include(x => x.Tasks)
                    .Include(x => x.Members)
                    .Include(x => x.Author)
                    .Include(x => x.Members.Select(z => z.Member))
                    .FirstOrDefault(x => x.Id == id.Value & (x.AuthorId == UserId | x.Members.Select(z => z.Member.MainMemberId).Contains(UserId)));
            else
                p = new Project
                {
                    DateTime = DateTime.Now,
                    State = ProjectState.New,
                    Author = db.Members.Find(UserId),
                    AuthorId = UserId,
                    Members = new List<ProjectMember>()
                };

            var model = p.ToDynamic();
            model.TasksCount = p.Tasks == null ? 0 : p.Tasks.Count;
            model.IsMy = p.AuthorId == UserId;
            model.Author = p.Author.LocalMember;
            model.Members = p.Members.Select(m => new
            {
                MemberId = m.MemberId,
                ProjectId = m.ProjectId,
                Member = m.Member.LocalMember,
                MemberType = m.MemberType,
                MemberTypeS = m.MemberType.ToDisplayName(),
            });

            return ResponseObject2Json(model);
        }

        [HttpGet]
        public HttpResponseMessage List(bool notAccepted)
        {
            IQueryable<Project> q;
            if (notAccepted)
                q = db.Projects
                    .Include(x => x.Tasks)
                    .Include(x => x.Members)
                    .Where(x => x.State != ProjectState.Approved & (x.AuthorId == UserId | x.Members.Select(z => z.Member.MainMemberId).Contains(UserId)) 
                    );
            else
                q = db.Projects
                    .Include(x => x.Tasks)
                    .Include(x => x.Members)
                    .Where(x =>(x.AuthorId == UserId | x.Members.Select(z => z.Member.MainMemberId).Contains(UserId))
                );

            var model = q.OrderBy(x => x.DateTime).ThenBy(x => x.Name).ToList().Select(x => 
            {
                var ret = x.ToDynamic();
                ret.TasksCount = x.Tasks.Count;
                ret.IsMy = x.AuthorId == UserId;
                ret.CanApprove = x.AuthorId == UserId;
                return ret;
            }).ToList();

            return ResponseObject2Json(model);
        }

        [HttpPost]
        public HttpResponseMessage Post(Project model)
        {
            model.Id = Guid.NewGuid();
            model.Author = null;
            model.AuthorId = UserId;

            db.Projects.Add(model);
            db.ProjectMembers.Add(new ProjectMember { MemberId = UserId, ProjectId = model.Id });
            db.SaveChanges();

            return ResponseObject2Json(model);
        }

        [HttpPut]
        public HttpResponseMessage Put(Project model)
        {
            if (model.AuthorId == UserId)
            {
                db.AttachModel<Project>(model,
                    x => x.Name,
                    x => x.DateTime,
                    x => x.Description,
                    x => x.State,
                    x => x.RateCurrencyType,
                    x => x.RateType,
                    x => x.RateValue,
                    x => x.BotId,
                    x => x.BotUTC,
                    x => x.Language,
                    x => x.IsArchive
                    );

                db.SaveChanges();
            }

            return ResponseObject2Json(model);
        }

        [HttpDelete]
        public HttpResponseMessage Delete(Guid id, [FromUri]Guid? memberId, [FromUri]string mode)
        {
            Project model = db.Projects.FirstOrDefault(x => x.Id == id & (x.AuthorId == UserId | x.Members.Select(z => z.Member.MainMemberId).Contains(UserId)));
            //
            if (mode == "deleteMember")
            {
                db.ProjectMembers.Remove(model.Members.FirstOrDefault(x => x.MemberId == memberId.Value));
            }
            else
            {
                var delProject = db.Projects.Where(project => project.Id == id).FirstOrDefault();
                if (delProject != null && !delProject.IsArchive)
                {
                    delProject.IsArchive = true;
                    db.AttachModel<Project>(delProject, project => project.IsArchive);
                }
                else
                {
                    if (delProject != null && delProject.IsArchive)
                    {
                        db.Projects.Remove(delProject);
                    }
                }
            }
            db.SaveChanges();

            return new HttpResponseMessage() { Content = new StringContent("", Encoding.UTF8, "application/json") };
        }
    }
}
