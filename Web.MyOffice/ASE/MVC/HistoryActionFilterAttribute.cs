using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC
{
    public class HistoryActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            /*
            if (filterContext.RequestContext.HttpContext.Request.HttpMethod.ToLower() == "post")
                return;

            List<string> history = (List<string>)filterContext.RequestContext.HttpContext.Session["ASEHistory"];
            if (history == null)
            {
                history = new List<string>();
                filterContext.RequestContext.HttpContext.Session.Add("ASEHistory", history);
            }
            var url = filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri.ToLower();
            if (history.Count == 0)
                history.Add(url);
            else if (history[history.Count - 1] != url)
                history.Add(url);

            if (history.Count > 25)
                history.RemoveRange(20, 4);
            */
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }
    }
}