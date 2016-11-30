using ASE.MVC;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web.MyOffice.Data;

namespace Web.MyOffice.Controllers.API
{
    public class AuthController : ControllerApiAdv<DB>
    {
        [HttpGet]
        [HttpPost]
        public Guid? Login(LoginViewModel model)
        {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var usr = um.Find(model.Email, model.Password);
            if (usr == null)
                return Guid.Empty;

            Guid gid = Guid.Parse(usr.Id);
            var u = db.Members.FirstOrDefault(x => x.Id == gid);
            if ((!u.APISessionDT.HasValue) || (u.APISessionDT.Value.AddHours(3) < DateTime.Now))
            {
                u.APISessionId = Guid.NewGuid();
                u.APISessionDT = DateTime.Now;
            }
            db.SaveChanges();

            return u.APISessionId.Value;
        }

        [HttpDelete]
        public string Logout(Guid id)
        {
            var u = db.Members.FirstOrDefault(x => x.APISessionId == id);
            if (u == null)
                return "null";

            u.APISessionId = null;
            u.APISessionDT = null;
            db.SaveChanges();

            return "ok";
        }

        [HttpPut]
        public Guid Renew(Guid id)
        {
            var u = db.Members.FirstOrDefault(x => x.APISessionId == id);
            if (u == null)
                return Guid.Empty;

            u.APISessionDT = DateTime.Now;
            db.SaveChanges();

            return id;
        }
    }
}
