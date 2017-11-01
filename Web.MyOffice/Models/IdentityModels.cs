using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ASE.MVC
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole
    {
    }

    public class ExternalLoginViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ConnectionString")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (System.Configuration.ConfigurationManager.AppSettings["TablePrefix"] != null)
            {
                modelBuilder.Entity<ApplicationUser>().ToTable(System.Configuration.ConfigurationManager.AppSettings["TablePrefix"] + "_AspNetUsers"); //AspNetUsers
                modelBuilder.Entity<IdentityRole>().ToTable(System.Configuration.ConfigurationManager.AppSettings["TablePrefix"] + "_AspNetRoles"); //AspNetRoles
                modelBuilder.Entity<IdentityUserRole>().ToTable(System.Configuration.ConfigurationManager.AppSettings["TablePrefix"] + "_AspNetUserRoles"); //AspNetUserRoles
                modelBuilder.Entity<IdentityUserClaim>().ToTable(System.Configuration.ConfigurationManager.AppSettings["TablePrefix"] + "_AspNetUserClaims"); //AspNetUserClaims
                modelBuilder.Entity<IdentityUserLogin>().ToTable(System.Configuration.ConfigurationManager.AppSettings["TablePrefix"] + "_AspNetUserLogins"); //AspNetUserLogins
            }
        }
    }
}