using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using ASE.EF;
using ASE.ToolsModels;

namespace ASE.MVC
{
    public class ControllerAdv<DBType> : Controller where DBType : DbContext, new()
    {
        internal DBType db;

        public ControllerAdv()
        {
            db = new DBType();
        }

        private UserManager<ApplicationUser> _UserManager;
        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                if (_UserManager == null)
                {
                    _UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    _UserManager.UserTokenProvider = new EmailTokenProvider<ApplicationUser, string>(); 
                }

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

        public Guid UserId
        {
            get
            {
                return Guid.Parse(this.User.Identity.GetUserId());
            }
        }

        public string UserIdS
        {
            get
            {
                return UserId.ToString();
            }
        }

        /*public void SortOrder(ref string sortOrder, string defSort)
        {
            sortOrder = String.IsNullOrEmpty(sortOrder) ? (this.GetSavedSortOrder("Index") ?? defSort) : sortOrder;
            this.SaveSortOrder("Index", sortOrder);
        }

        public void Filter(ref string filter, string defFilter)
        {
            filter = String.IsNullOrEmpty(filter) ? this.GetSavedFilter("Index") ?? defFilter : filter;
            this.SaveFilter("Index", filter);
        }

        public void SortOrderAndFilter(ref string sortOrder, string defSort, ref string filter, string defFilter)
        {
            SortOrder(ref sortOrder, defSort);

            Filter(ref filter, defFilter);
        }*/

        public void AttachModel<TEntity>(TEntity entity) where TEntity : class
        {
            db.AttachModel(entity, this.Request);
        }

        public HtmlHelper HtmlHelper
        {
            get
            {
                return new HtmlHelper(new ViewContext(ControllerContext, new WebFormView(ControllerContext, "omg"), new ViewDataDictionary(), new TempDataDictionary(), TextWriter.Null), new ViewPage());
            }
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

        public virtual void SessionPush(string name, object value)
        {
            Session.Add(this.GetType().Name + "." + name, value);
        }

        public virtual DateTime SessionPull(string name, DateTime value)
        {
            var r = Session[this.GetType().Name + "." + name];
            if (r is DateTime)
                return (DateTime) r;

            return value;
        }
        public virtual int SessionPull(string name, int value)
        {
            var r = Session[this.GetType().Name + "." + name];
            if (r is int)
                return (int)r;

            return value;
        }

        public DateTime DefaultDateFrom(DateTime? dt)
        {
            return DefaultDateFrom(dt, "DateFrom");
        }

        public DateTime DefaultDateFrom(DateTime? dt, string name)
        {
            var r = dt.HasValue ? dt.Value.Date : SessionPull(name, DateTime.Now.Date);
            SessionPush(name, r);
            return r;
        }

        public DateTime DefaultDateTo(DateTime? dt)
        {
            return DefaultDateTo(dt, "DateTo");
        }

        public DateTime DefaultDateTo(DateTime? dt, string name)
        {
            var r = dt.HasValue ? dt.Value.Date.AddDays(1).AddMilliseconds(-1) : SessionPull(name, DateTime.Now.Date.AddDays(1).AddMilliseconds(-1));
            SessionPush(name, r);
            return r;
        }

        public List<KeyValuePair<string,string>> History()
        {
            List<KeyValuePair<string, string>> history = (Session["ASEHistory"] as List<KeyValuePair<string, string>>);
            if (history == null)
                history = new List<KeyValuePair<string, string>>();

            return history;
        }

        public ActionResult HistoryBack(ActionResult url)
        {
            var h = History();
            if (h.Count != 0)
            {
                var r = h[h.Count - 1].Value;
                if ((r != Request.Url.AbsoluteUri.ToLower()) & (r != Request.UrlReferrer.AbsoluteUri.ToLower()))
                    return Redirect(r);

                if (h.Count != 1)
                {
                    r = h[h.Count - 2].Value;
                    if (r != Request.Url.AbsoluteUri.ToLower())
                        return Redirect(r);
                }
            }

            return url;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                _UserManager.Dispose();
                _UserManager = null;
            }

            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        protected override void Execute(System.Web.Routing.RequestContext requestContext)
        {
            base.Execute(requestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
    }
}