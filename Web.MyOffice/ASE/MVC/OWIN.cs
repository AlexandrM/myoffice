using ASE.MVC;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASE.MVC
{
    public class OWIN
    {
        public static UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        public static bool IsUserInRole(string user, string role)
        {
            return userManager.IsInRole(user, role);
        }
    }
}