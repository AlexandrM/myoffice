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
using Web.MyOffice.Res;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace Web.MyOffice.Controllers.API
{
    public class EmailDataModel
    {
        public int Action { get; set; }
        public Guid ProjectId { get; set; }
        public Guid MemberId { get; set; }
        public string[] Attachments { get; set; }
    }

    [Authorize]
    public class EmailController : ControllerApiAdv<DB>
    {
        [HttpPost]
        public HttpResponseMessage Post(EmailDataModel model)
        {
            var p = db.ProjectMembers.Where(x => x.ProjectId == model.ProjectId & x.Member.MainMemberId == UserId).Select(x => x.Project).FirstOrDefault();
            var to = db.ProjectMembers.Where(x => x.ProjectId == model.ProjectId & x.MemberId == model.MemberId).Select(x => x.Member).FirstOrDefault();

            if (model.Action == 1)
            {
                var base64Data = Regex.Match(model.Attachments[0], @"data:application/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(binData))
                { 
                    EMail.SendEmail(
                    to.FullName,
                    to.Email,
                    S.Debt,
                    "",
                    new System.Net.Mail.Attachment(stream, "report1.pdf")
                    );
                }
            }

            return new HttpResponseMessage() { Content = new StringContent("123", Encoding.UTF8, "application/json") };
        }
    }
}
