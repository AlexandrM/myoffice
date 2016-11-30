using ASE.MVC.BootstrapHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.MyOffice.Res;

namespace Web.MyOffice
{
    public static class Helpers
    {
        public static MvcHtmlString Top(this HtmlHelper helper)
        {
            return new MvcHtmlString(String.Format("<h2>{0}</h2><hr />", helper.ViewBag.Title));
        }

        public static IDisposable TopDetials(this HtmlHelper helper)
        {
            helper.ViewContext.Writer.Write(String.Format("<h2>{0}</h2>", helper.ViewBag.Title));

            return new TagTerminator(helper, @"<hr />");
        }

        public static MvcHtmlString BackToIndex(this HtmlHelper helper)
        {
            if (helper.ViewBag.RenderToPdf != null)
                return new MvcHtmlString("");

            return new MvcHtmlString(String.Format("<div>{0}</div>", ASE.MVC.BootstrapHelpers.ActionLinkHelper.ActionLinkDefault(helper, S.ToIndex, "Index")));
        }

        public static MvcHtmlString BackToIndex(this HtmlHelper helper, object roureValues)
        {
            return new MvcHtmlString(String.Format("<div>{0}</div>", ASE.MVC.BootstrapHelpers.ActionLinkHelper.ActionLinkDefault(helper, S.ToIndex, "Index", roureValues)));
        }

        public static MvcHtmlString BackToIndex(this HtmlHelper helper, string controller, object roureValues)
        {
            return new MvcHtmlString(String.Format("<div>{0}</div>", ASE.MVC.BootstrapHelpers.ActionLinkHelper.ActionLinkDefault(helper, S.ToIndex, "Index", controller, roureValues, null)));
        }
    }
}