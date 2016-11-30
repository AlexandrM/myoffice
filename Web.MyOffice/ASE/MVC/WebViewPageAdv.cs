using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using ASE.ToolsModels;

using Web.MyOffice.Models;
using Web.MyOffice.Data;
using Web.MyOffice.Res;

namespace ASE.MVC
{
    public abstract class WebViewPageAdv<T> : WebViewPage<T>
    {
        //public DB db { get; set; }

        public WebViewPageAdv()
        {
            //db = new DB();
        }

        public Guid UserId
        {
            get
            {
                if (this.User.Identity.GetUserId() == null)
                    return Guid.Empty;

                return Guid.Parse(this.User.Identity.GetUserId());
            }
        }

        public string UserIdS
        {
            get
            {
                return this.User.Identity.GetUserId();
            }
        }

        public string CurrentAction
        {
            get
            {
                return this.ViewContext.Controller.ValueProvider.GetValue("action").RawValue.ToString();
            }
        }
        public string CurrentController
        {
            get
            {
                return this.ViewContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString();
            }
        }

        public string IsCurrentAction(string result, params string[] actions)
        {
            foreach (var item in actions)
                if (item == CurrentAction)
                    return result;

            return "";
        }
        public string IsCurrentController(string result, params string[] controllers)
        {
            foreach (var item in controllers)
                if (item == CurrentController)
                    return result;

            return "";
        }

        public string IsCurrentActionAndController(string action, string controller, string result)
        {
            if ((CurrentAction == action) & (CurrentController == controller))
                return result;

            return "";
        }

        private UserManager<ApplicationUser> _UserManager;
        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                if (_UserManager == null)
                    _UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                return _UserManager;
            }
        }

        private RoleManager<ApplicationRole> _RoleManager;
        public RoleManager<ApplicationRole> RoleManager
        {
            get
            {
                if (_RoleManager == null)
                    _RoleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));

                return _RoleManager;
            }
        }

        public string GetCookieValue(string name)
        {
            if ((Response.Cookies.AllKeys.FirstOrDefault(x => x == name) != null))
                return Response.Cookies[name].Value;

            //if ((Request.Cookies[name] != null) && Request.Cookies[name].Value != null)
            if ((Request.Cookies.AllKeys.FirstOrDefault(x => x == name) != null))
                return Request.Cookies[name].Value;

            return null;
        }

        public bool IsSelect
        {
            get
            {
                var ret = false;
                if (Request.Params.AllKeys.Contains("isSelect"))
                    bool.TryParse(Request.Params["isSelect"], out ret);

                return ret;
            }
        }

        public ToolModelIdValue SelectResultField
        {
            get
            {
                return new ToolModelIdValue
                {
                    Id = Request.Params["selectResultFieldId"],
                    Value = Request.Params["selectResultField"],
                };
            }
        }

        public override System.Web.WebPages.HelperResult RenderPage(string path, params object[] data)
        {
            return base.RenderPage(path, data);
        }

        public string Title
        {
            get
            {
                var title = String.Format("{0} - {1}", ViewBag.Title, S.AppName);

                List<KeyValuePair<string, string>> history = (Session["ASEHistory"] as List<KeyValuePair<string, string>>);
                if (history == null)
                {
                    history = new List<KeyValuePair<string, string>>();
                    Session.Add("ASEHistory", history);
                }
                var url = new KeyValuePair<string, string>(title, Request.Url.AbsoluteUri.ToLower());
                if (history.Count == 0)
                    history.Add(url);
                else if (history[history.Count - 1].Value != url.Value)
                    history.Add(url);

                if (history.Count > 25)
                    history.RemoveRange(20, 4);

                return title;
            }
        }

        public DB db
        {
            get
            {
                return ContextPerRequest<DB>.Db;
            }
        }
    }
}