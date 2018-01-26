using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using ASE.MVC;

using Web.MyOffice.Data;
using Web.MyOffice.Models;
using ASE;
using Web.MyOffice.Res;
using BotDetect.Web.Mvc;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity.EntityFramework;
using MyBank.Models;

namespace Web.MyOffice.Controllers
{
    [Authorize]
    //[RequireHttps]
    public class AccountController : ControllerAdv<DB>
    {
        private ApplicationSignInManager _signInManager;

        public AccountController()
            : this(
                new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())),
                new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext())))
        {
            ApplicationDbContext ac = new ApplicationDbContext();
            if (ac.Users.Count() == 0)
            {
                RoleManager.Create(new ApplicationRole() { Name = "Admin" });
            }
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }


        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string email, string error)
        {
            ViewBag.ReturnUrl = returnUrl;
                ModelState.AddModelError("", error);

            return View(new LoginViewModel { Email = email });
        }

        public ActionResult PasswordReset(string email, string action, string bodyView, string message)
        {
            var user = UserManager.FindByName(email);

            ViewBag.Code = UserManager.GeneratePasswordResetToken(user.Id);
            ViewBag.User = user.Email;

            EMail.SendEmail(
                new string[] { user.UserName, user.Email },
                new string[] { user.UserName, user.Email },
                System.Configuration.ConfigurationManager.AppSettings["ASE.SiteName"] + " - " + action,
                this.RenderPartialView(bodyView));

            return RedirectToAction("RestorePassword", new ResetPasswordViewModel { Email = email, Message = message });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string register, string formMode)
        {
            if ((formMode != null) & (formMode == "restore"))
            {
                var user = UserManager.FindByName(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", S.UserNotFound);
                    return View(model);
                }

                return PasswordReset(model.Email, S.RestoringPassword, "MT_RestorePassword", S.ResetPassword);
            }

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    SignInAsync(user, true);

                    if (user.LockoutEnabled)
                    {
                        user.LockoutEnabled = false;
                        UserManager.Update(user);
                        return RedirectToAction("Index", "Currency");
                    }

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", S.InvalidUsernameOrPassword);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaValidation("CaptchaCode", "RegisterCaptcha", "")]
        public async Task<ActionResult> Register(string email, string captchaCode, bool captchaValid)
        {
            RegisterViewModel model = new RegisterViewModel { Email = email, Password = Guid.NewGuid().ToString() };
            if (!captchaValid)
                ModelState.AddModelError("IncorrectCAPTCHA", S.IncorrectCAPTCHA);

            if (ModelState.IsValid)
            {
                //Exists - restore password
                var user = UserManager.FindByEmail(email);
                if (user != null)
                    return PasswordReset(model.Email, S.RestoringPassword, "MT_RestorePassword", S.ResetPassword);

                //Member
                Member member = db.Members.FirstOrDefault(x => x.Email == model.Email & x.Id == x.MainMemberId & x.Id == x.UserId);
                //Create if exits
                member = member ?? Member.NewMember(model.Email);
                //Create user
                user = new ApplicationUser() { UserName = model.Email, Id = member.Id.ToString(), Email = model.Email };

                //Exits in users
                try
                {
                    user.LockoutEnabled = true;
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        //SignInAsync(user, isPersistent: false);
                        //return RedirectToAction("Index", "Home");

                        return PasswordReset(model.Email, S.AssignPassword, "MT_AssignPassword", S.AssignPassword);
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError("", validationError.ErrorMessage);
                        }
                    }
                    //throw ex;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            //ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                //message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                //message = ManageMessageId.Error;
            }
            return RedirectToAction("MyProfile", "Member");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            var r = new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            //External login info 
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            //Try login local
            var loginResult = await SignInManager.ExternalSignInAsync(loginInfo, true);
            if (loginResult == SignInStatus.Success)
                return RedirectToLocal(returnUrl);

            //No associate but authenticated - add login
            if (Request.IsAuthenticated)
            {
                await UserManager.AddLoginAsync(UserIdS, loginInfo.Login);
                return RedirectToLocal(returnUrl);
            }

            //It`s registration
            string extUser = loginInfo.Email;
            if (extUser == null)
            {
                //Not registre without email
                return RedirectToAction("Login", new { error = String.Format("{0} - {1}", loginInfo.Login.LoginProvider, S.LoginProviderNoEmail) });
            }

            var user = await UserManager.FindAsync(loginInfo.Login);

            if (user != null)
            {
                if (user.LockoutEnabled)
                    return View("LockoutEnabled");

                SignInAsync(user, isPersistent: false);

                return RedirectToLocal(returnUrl);
            }
            else
            {
                //Member
                Member member = null;
                //Seek member
                var eMember = db.Members.FirstOrDefault(x => x.Email == loginInfo.Email & x.Id == x.MainMemberId & x.Id == x.UserId);
                //Create if exits
                member = eMember ?? Member.NewMember(loginInfo.Email);
                //Create user
                user = new ApplicationUser() { UserName = extUser, Id = member.Id.ToString(), Email = loginInfo.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    //Add member if exits
                    if (eMember == null)
                    {
                        db.Members.Add(member);
                        db.SaveChanges();
                    }
                    SignInAsync(user, isPersistent: false);
                    return RedirectToLocal("~/Project#/Currencies");
                }

                return RedirectToLocal(returnUrl);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region Helpers

        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);

            Response.Cookies.Add(new HttpCookie("UserEmail") { Value = user.Email, Expires = DateTime.Now.AddYears(1) });

            Guid userId = Guid.Parse(user.Id);

            //Add member
            var currentMember = db.Members.FirstOrDefault(x => x.Id == userId);
            if (currentMember == null)
            {
                currentMember = new Member();
                currentMember.Id = userId;
                currentMember.Email = user.UserName;
                currentMember.FullName = user.UserName;
                currentMember.UserId = userId;
                currentMember.MainMemberId = userId;
                db.Members.Add(currentMember);
                db.SaveChanges();
            }

            //Add default budget
            var budget = db.Budgets.FirstOrDefault(x => x.OwnerId == userId);
            if (budget == null)
            {
                budget = new Budget
                {
                    Id = Guid.NewGuid(),
                    Name = R.R.MainBudget,
                    OwnerId = userId,
                };
                db.Budgets.Add(budget);
                db.SaveChanges();
            }

            var currency = ExtensionsDB.DefaultCurrency(db, userId);

            if (!db.CategoryAccounts.Any())
            {
                var accCat1 = new CategoryAccount
                {
                    BudgetId = budget.Id,
                    Id = Guid.NewGuid(),
                    Name = R.R.CategoryCash,
                    Description = R.R.CategoryCash,
                };
                var accCat2 = new CategoryAccount
                {
                    BudgetId = budget.Id,
                    Id = Guid.NewGuid(),
                    Name = R.R.CategoryCards,
                    Description = R.R.CategoryCards,
                };
                db.CategoryAccounts.Add(accCat1);
                db.CategoryAccounts.Add(accCat2);
                db.SaveChanges();

                db.Accounts.Add(new Account
                {
                    Id = Guid.NewGuid(),
                    CategoryId = accCat1.Id,
                    CurrencyId = currency.Id,
                    Name = R.R.AccountCash,
                    ShowInRest = true,                    
                });
                db.Accounts.Add(new Account
                {
                    Id = Guid.NewGuid(),
                    CategoryId = accCat2.Id,
                    CurrencyId = currency.Id,
                    Name = R.R.AccountCard,
                    ShowInRest = true,
                });
                db.SaveChanges();
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                properties.Dictionary["redirect_uri"] = "QQQQQQQQQQQQ"; 
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion       

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult RestorePassword(ResetPasswordViewModel model)
        {
            if ((model.Code == null) || (Request.HttpMethod.ToLower()  != "post"))
            {
                ModelState.Clear();
                return View(model);
            }

            var user = UserManager.FindByName(model.Email);
            var result = UserManager.ResetPassword(user.Id, model.Code, model.NewPassword);

            if (result.Succeeded)
            {
                if (Request.IsAuthenticated)
                {
                    return RedirectToAction("MyProfile", "Member");
                }
                else
                {
                    return RedirectToAction("Login", new { email = model.Email });
                }
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);


            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model, string formMode)
        {
            if ((formMode != null) & (formMode == "restore"))
            {
                var user = UserManager.FindById(UserIdS);
                if (user == null)
                {
                    ModelState.AddModelError("", S.UserNotFound);
                    return View(model);
                }

                ViewBag.Code = UserManager.GeneratePasswordResetToken(user.Id);
                ViewBag.User = user.Email;

                EMail.SendEmail(
                    user.Email,
                    user.Email,
                    System.Configuration.ConfigurationManager.AppSettings["ASE.SiteName"] + " - " + S.RestoringPassword,
                    this.RenderPartialView("MT_RestorePassword"));

                return RedirectToAction("RestorePassword", new { email = user.Email });
            }

            if (!ModelState.IsValid)
                return View(model);

            var result = UserManager.ChangePassword(UserIdS, model.OldPassword, model.NewPassword);
            
            if (result.Succeeded)
                return RedirectToAction("MyProfile", "Member");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }
    }
}