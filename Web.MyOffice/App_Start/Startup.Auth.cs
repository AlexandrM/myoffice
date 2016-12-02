using ASE.MVC;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Twitter;
using Owin;
using System;

namespace Web.MyOffice
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieName = System.Configuration.ConfigurationManager.AppSettings["Auth.AuthCookieName"],
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Microsoft.Auth.ClientId"]))
                app.UseMicrosoftAccountAuthentication(
                    clientId: System.Configuration.ConfigurationManager.AppSettings["Microsoft.Auth.ClientId"],
                    clientSecret: System.Configuration.ConfigurationManager.AppSettings["Microsoft.Auth.ClientSecret"]);

            if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Twitter.Auth.ConsumerKey"]))
                app.UseTwitterAuthentication(new TwitterAuthenticationOptions
                {
                    ConsumerKey = System.Configuration.ConfigurationManager.AppSettings["Twitter.Auth.ConsumerKey"],
                    ConsumerSecret = System.Configuration.ConfigurationManager.AppSettings["Twitter.Auth.ConsumerSecret"],
                    BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(new[]
                    {
                        "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                        "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                        "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                        "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                        "5168FF90AF0207753CCCD9656462A212B859723B", //DigiCert SHA2 High Assurance Server C‎A 
                        "B13EC36903F8BF4701D498261A0802EF63642BC3" //DigiCert High Assurance EV Root CA
                    })
                });

            if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Facebook.Auth.AppId"]))
                app.UseFacebookAuthentication(
                    appId: System.Configuration.ConfigurationManager.AppSettings["Facebook.Auth.AppId"],
                    appSecret: System.Configuration.ConfigurationManager.AppSettings["Facebook.Auth.AppSecret"]);

            if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Google.Auth.ClientId"]))
                app.UseGoogleAuthentication(
                    System.Configuration.ConfigurationManager.AppSettings["Google.Auth.ClientId"],
                    System.Configuration.ConfigurationManager.AppSettings["Google.Auth.ClientSecret"]);
        }
    }
}