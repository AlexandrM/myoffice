using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;

using ASE;
using ASE.EF;
using ASE.MVC;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using Web.MyOffice.Res;

namespace Web.MyOffice.Controllers.API
{
    public class ProjectTaskCommentController : ControllerApiAdv<DB>
    {
        [HttpPost]
        public HttpResponseMessage Post(ProjectTaskComment model)
        {
            var task = db.ProjectTasks.FirstOrDefault(x => x.Id == model.TaskId & x.Project.Members.Select(z => z.Member.MainMember.MainMemberId).Contains(UserId));

            model.Id = Guid.NewGuid();
            model.TaskId = task.Id;
            model.DateTime = DateTime.Now;
            model.AuthorId = UserId;
            var r = ResponseObject2Json(model);

            db.ProjectTaskComments.Add(model);
            db.SaveChanges();

            return r;
        }

        [HttpPut]
        public HttpResponseMessage Put(ProjectTaskComment model)
        {
            var r = ResponseObject2Json(model);

            var m = db.ProjectTaskComments.FirstOrDefault(x => x.Id == model.Id & x.AuthorId == UserId);
            m.History = String.Format("{0}[{1}]<br>{2}: {3}<br><br><br>", 
                m.History,
                m.Updated
                , S.Comment, m.Comment);
            m.Comment = model.Comment;
            m.Updated = DateTime.Now;
            db.SaveChanges();

            return r;
        }

        [HttpDelete]
        public HttpResponseMessage Delete(Guid id)
        {
            db.ProjectTaskComments.Remove(db.ProjectTaskComments.FirstOrDefault(x => x.Id == id & x.AuthorId == UserId));
            db.SaveChanges();

            return ResponseObject2Json(new { id = id});
        }
    }
}
