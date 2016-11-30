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
using Web.MyOffice.Res;

namespace Web.MyOffice.Controllers.API
{
    public class ProjectTaskController : ControllerApiAdv<DB>
    {
        /// <summary>
        /// Get ProjectTask by id or new
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Get(Guid id, Guid projectId)
        {
            ProjectTask p;
            dynamic newComment = null;

            p = db.ProjectTasks
                .Include(x => x.Author)
                .Include(x => x.Comments)
                .Include(x => x.Comments.Select(z => z.Author))
                .FirstOrDefault(x => x.Id == id);
            if (p != null)
            { 
                newComment = new
                {
                    Id = Guid.Empty,
                    AuthorId = UserId,
                    AuthorName = db.Members.Find(UserId).LocalMember.FullName,
                    IsMy = true,
                    Comment = "",
                    DateTime = DateTime.Now,
                    TaskId =  id
                };
            }
            else
            {
                p = new ProjectTask
                {
                    AuthorId = UserId,
                    Author = db.Members.Find(UserId),
                    DateTime = DateTime.Now,
                    State = ProjectTaskState.New,
                    ProjectId = projectId
                };
            }

            dynamic comments = null;
            if (p.Comments != null)
                comments = p.Comments.OrderBy(x => x.DateTime).ToList().Select(x => new
                {
                    Id = x.Id,
                    AuthorId = x.AuthorId,
                    AuthorName = x.Author.LocalMember.FullName,
                    IsMy = x.AuthorId == UserId,
                    Comment = x.Comment,
                    History = x.History,
                    DateTime = x.DateTime,
                    TaskId = x.TaskId
                });


            var o = new
            {
                Id = p.Id,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.LocalMember.FullName,
                IsMy = p.AuthorId == UserId,
                DateTime = p.DateTime,
                Description = p.Description,
                History = p.History,
                Limitation = p.Limitation,
                Name = p.Name,
                ProjectId = p.ProjectId,
                State = p.State,
                StateS = p.State.ToDisplayName(),
                Comments = comments,
                NewComment = newComment
            };

            var s = JsonConvert.SerializeObject(o, new JsonSerializerSettings { });

            return new HttpResponseMessage() { Content = new StringContent(s, Encoding.UTF8, "application/json") };
        }

        /// <summary>
        /// Get list of ProjectTask by id with period
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="notAccepted"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Query(Guid projectId, DateTime dateFrom, DateTime dateTo, bool notAccepted)
        {
            IQueryable<ProjectTask> q;
            if (notAccepted)
                q = db.ProjectTasks.Where(x =>
                    x.ProjectId == projectId
                    & ((x.DateTime >= dateFrom & x.DateTime <= dateTo) | (x.Limitation >= dateFrom & x.Limitation <= dateTo) | (x.State != ProjectTaskState.Approved))
                    & (x.Project.AuthorId == UserId | x.Project.Members.Select(z => z.Member.MainMemberId).Contains(UserId))
                    );
            else
                q = db.ProjectTasks.Where(x =>
                    x.ProjectId == projectId
                    & ((x.DateTime >= dateFrom & x.DateTime <= dateTo) | (x.Limitation >= dateFrom & x.Limitation <= dateTo))
                    & (x.Project.AuthorId == UserId | x.Project.Members.Select(z => z.Member.MainMemberId).Contains(UserId))
                );

            var user = db.ProjectMembers.FirstOrDefault(x => x.ProjectId == projectId & x.Member.MainMemberId == UserId);

            var o = q.OrderBy(x => x.DateTime).ThenBy(x => x.Name).ToList().Select(x => new
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                State = x.State,
                StateS = x.State.ToDisplayName(),
                Name = x.Name,
                Description = x.Description,
                History = x.History,
                DateTime = x.DateTime,
                Limitation = x.Limitation,
                LimitationS = x.Limitation.ToStringD(),
                IsMy = x.AuthorId == UserId,
                CanApprove = x.AuthorId == UserId | user.MemberType == ProjectMemberType.Customer,
                Expired = DateTime.Now.Date == x.Limitation.Date ? 1 : DateTime.Now.Date > x.Limitation.Date ? 2 : 0
            }).ToList();

            var s = JsonConvert.SerializeObject(o, new JsonSerializerSettings { });

            return new HttpResponseMessage() { Content = new StringContent(s, Encoding.UTF8, "application/json") };
        }

        /// <summary>
        /// Add ProjectTask
        /// Update ProjectTask
        /// Add days to ProjectTask.Limitation
        /// Change ProjectTask.State
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Post(ProjectTask model, [FromUri] string mode)
        {
            var task = db.ProjectTasks.FirstOrDefault(x => x.Id == model.Id & (x.Project.AuthorId == UserId | x.Project.Members.Select(z => z.Member.MainMemberId).Contains(UserId)));

            if (mode == "addDay")
            {
                task.Limitation = model.Limitation;
            }
            else if (mode == "setState")
            {
                var user = db.ProjectMembers.FirstOrDefault(x => x.ProjectId == task.ProjectId & x.Member.MainMemberId == UserId);

                if (task.AuthorId == UserId)
                    task.State = model.State;
                else if ((model.State != ProjectTaskState.Approved) | (user.MemberType == ProjectMemberType.Customer))
                    task.State = model.State;
            }
            else if ((task == null))
            {
                if (db.ProjectMembers.FirstOrDefault(x => x.ProjectId == model.ProjectId & x.Member.MainMemberId == UserId) != null)
                {
                    model.Id = Guid.NewGuid();
                    model.DateTime = DateTime.Now;
                    model.AuthorId = UserId;
                    db.ProjectTasks.Add(model);
                }
            }
            else if ((task != null) && (task.AuthorId == UserId))
            {
                db.Entry(task).State = System.Data.Entity.EntityState.Detached;

                model.History = String.Format("{0}[{1}]<br>{2}: {3}<br>{4}: {5}<br>{6}: {7}<br><br><br>", 
                    task.History,
                    task.Updated, 
                    S.Name, task.Name, 
                    S.Limitation, task.Limitation, 
                    S.Description, task.Description);

                model.Updated = DateTime.Now;
                db.AttachModel<ProjectTask>(model,
                    x => x.Id,
                    x => x.Name,
                    x => x.DateTime,
                    x => x.Updated,
                    x => x.Limitation,
                    x => x.Description,
                    x => x.History
                    );
            }

            db.SaveChanges();

            var s = JsonConvert.SerializeObject(model, new JsonSerializerSettings { });
            return new HttpResponseMessage() { Content = new StringContent(s, Encoding.UTF8, "application/json") };
        }

        /// <summary>
        /// Delete ProjectTask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public HttpResponseMessage Delete(Guid id)
        {
            db.ProjectTasks.Remove(db.ProjectTasks.FirstOrDefault(x => x.Id == id & x.AuthorId == UserId));

            db.SaveChanges();


            return new HttpResponseMessage() { Content = new StringContent("", Encoding.UTF8, "application/json") };
        }

        /// <summary>
        /// Add comment to ProjectTask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage PostComment(ProjectTaskComment model)
        {
            var task = db.ProjectTasks.FirstOrDefault(x => x.Id == model.TaskId & (x.Project.AuthorId == UserId | x.Project.Members.Select(z => z.Member.MainMemberId).Contains(UserId)));

            model.Id = Guid.NewGuid();
            model.DateTime = DateTime.Now;
            model.AuthorId = UserId;
            model.TaskId = task.Id;
            db.ProjectTaskComments.Add(model);
            db.SaveChanges();


            return new HttpResponseMessage() { Content = new StringContent("", Encoding.UTF8, "application/json") };
        }
    }
}
