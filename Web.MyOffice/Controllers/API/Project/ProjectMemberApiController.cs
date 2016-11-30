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
    public class ProjectMemberController : ControllerApiAdv<DB>
    {
        [HttpPost]
        public HttpResponseMessage Post(ProjectMember model)
        {
            var r = ResponseObject2Json(model);
            var e = db.ProjectMembers.FirstOrDefault(x => x.MemberId == model.MemberId & x.ProjectId == model.ProjectId);
            if (e == null)
            {
                model.ProjectId = db.Projects.FirstOrDefault(x => x.Id == model.ProjectId & x.AuthorId == UserId).Id;
                db.ProjectMembers.Add(model);
                db.SaveChanges();
            }

            return r;
        }

        [HttpPut]
        public HttpResponseMessage Put(ProjectMember model)
        {
            var m = db.ProjectMembers.FirstOrDefault(x => x.MemberId == model.MemberId & x.ProjectId == model.ProjectId & x.Project.AuthorId == UserId);
            m.MemberType = model.MemberType;
            db.SaveChanges();

            return ResponseObject2Json(model);
        }

        [HttpDelete]
        public HttpResponseMessage Delete([FromUri] ProjectMember model)
        {
            var r = ResponseObject2Json(model);
            model = db.ProjectMembers
                .Include(x => x.Project)
                .FirstOrDefault(x => x.MemberId == model.MemberId & x.ProjectId == model.ProjectId & x.Project.AuthorId == UserId);
            if (model.Project.AuthorId != model.MemberId)
            {
                db.ProjectMembers.Remove(model);
                db.SaveChanges();
            }

            return r;
        }
    }
}
