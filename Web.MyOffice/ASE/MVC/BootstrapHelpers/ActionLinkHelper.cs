using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;

namespace ASE.MVC.BootstrapHelpers
{
    public static class ActionLinkHelper
    {
        public static MvcHtmlString ActionLinkBase(this HtmlHelper helper, string type, string text, string action, string controller, object routeValues, object htmlAttributes, string onClick)
        {
            if (helper.ViewBag.RenderToPdf != null)
                return new MvcHtmlString("");

            var attributes = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            object cssClass;
            if (attributes.TryGetValue("class", out cssClass) == false)
                cssClass = "";
            attributes["class"] = cssClass + " btn " + type;
            if (onClick != null)
            {
                if (onClick.ToLower().IndexOf("return") == -1)
                {
                    attributes["onclick"] = String.Format("ASE.ConfirmAction(this.href, '{0}'); return false;", onClick);
                }
                else
                {
                    attributes["onclick"] = onClick;
                }
            }


            var s = helper.ActionLink(String.IsNullOrEmpty(text) ? " " : text, action, controller, new RouteValueDictionary(routeValues), attributes);
            return s;
        }

        #region ActionLinkSuccess

        public static MvcHtmlString ActionLinkSuccess(this HtmlHelper helper, string text, string action, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-success", text, action, null, routeValues, htmlAttributes, null);
        }
        public static MvcHtmlString ActionLinkSuccess(this HtmlHelper helper, string text, string action, object routeValues)
        {
            return ActionLinkBase(helper, "btn-success", text, action, null, routeValues, null, null);
        }
        public static MvcHtmlString ActionLinkSuccess(this HtmlHelper helper, string text, string action, object routeValues, string onClick)
        {
            return ActionLinkBase(helper, "btn-success", text, action, null, routeValues, null, onClick);
        }

        public static MvcHtmlString ActionLinkSuccess(this HtmlHelper helper, string text, string action, string controller, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-success", text, action, controller, routeValues, htmlAttributes, null);
        }

        #endregion ActionLinkSuccess

        #region ActionLinkInfo

        public static MvcHtmlString ActionLinkInfo(this HtmlHelper helper, string text, string action, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-info", text, action, null, routeValues, htmlAttributes, null);
        }
        public static MvcHtmlString ActionLinkInfo(this HtmlHelper helper, string text, string action, object routeValues)
        {
            return ActionLinkBase(helper, "btn-info", text, action, null, routeValues, null, null);
        }
        public static MvcHtmlString ActionLinkInfo(this HtmlHelper helper, string text, string action, object routeValues, string onClick)
        {
            return ActionLinkBase(helper, "btn-info", text, action, null, routeValues, null, onClick);
        }

        public static MvcHtmlString ActionLinkInfo(this HtmlHelper helper, string text, string action, string controller, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-info", text, action, controller, routeValues, htmlAttributes, null);
        }

        #endregion ActionLinkInfo

        #region ActionLinkWarning

        public static MvcHtmlString ActionLinkWarning(this HtmlHelper helper, string text, string action, string controller, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-warning", text, action, controller, routeValues, htmlAttributes, null);
        }
        public static MvcHtmlString ActionLinkWarning(this HtmlHelper helper, string text, string action, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-warning", text, action, null, routeValues, htmlAttributes, null);
        }
        public static MvcHtmlString ActionLinkWarning(this HtmlHelper helper, string text, string action, object routeValues)
        {
            return ActionLinkBase(helper, "btn-warning", text, action, null, routeValues, null, null);
        }
        public static MvcHtmlString ActionLinkWarning(this HtmlHelper helper, string text, string action, object routeValues, string onClick)
        {
            return ActionLinkBase(helper, "btn-warning", text, action, null, routeValues, null, onClick);
        }

        #endregion ActionLinkWarning

        #region ActionLinkDanger

        public static MvcHtmlString ActionLinkDanger(this HtmlHelper helper, string text, string action, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-danger", text, action, null, routeValues, htmlAttributes, null);
        }

        public static MvcHtmlString ActionLinkDanger(this HtmlHelper helper, string text, string action, object routeValues)
        {
            return ActionLinkBase(helper, "btn-danger", text, action, null, routeValues, null, null);
        }

        public static MvcHtmlString ActionLinkDanger(this HtmlHelper helper, string text, string action, object routeValues, string onClick)
        {
            return ActionLinkBase(helper, "btn-danger", text, action, null, routeValues, null, onClick);
        }

        #endregion ActionLinkDanger

        #region ActionLinkPrimary

        public static MvcHtmlString ActionLinkPrimary(this HtmlHelper helper, string text, string action, string controller, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-primary", text, action, controller, routeValues, htmlAttributes, null);
        }
        public static MvcHtmlString ActionLinkPrimary(this HtmlHelper helper, string text, string action, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-primary", text, action, null, routeValues, htmlAttributes, null);
        }
        public static MvcHtmlString ActionLinkPrimary(this HtmlHelper helper, string text, string action, object routeValues)
        {
            return ActionLinkBase(helper, "btn-primary", text, action, null, routeValues, null, null);
        }
        public static MvcHtmlString ActionLinkPrimary(this HtmlHelper helper, string text, string action, object routeValues, string onClick)
        {
            return ActionLinkBase(helper, "btn-primary", text, action, null, routeValues, null, onClick);
        }
        public static MvcHtmlString ActionLinkPrimary(this HtmlHelper helper, string text, string action)
        {
            return ActionLinkBase(helper, "btn-primary", text, action, null, null, null, null);
        }
        public static MvcHtmlString ActionLinkPrimary(this HtmlHelper helper, string text, string action, string controller)
        {
            return ActionLinkBase(helper, "btn-primary", text, action, controller, null, null, null);
        }

        #endregion ActionLinkPrimary

        #region ActionLinkDefault

        public static MvcHtmlString ActionLinkDefault(this HtmlHelper helper, string text, string action, string controller, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-default", text, action, controller, routeValues, htmlAttributes, null);
        }
        public static MvcHtmlString ActionLinkDefault(this HtmlHelper helper, string text, string action, object routeValues, object htmlAttributes)
        {
            return ActionLinkBase(helper, "btn-default", text, action, null, routeValues, htmlAttributes, null);
        }
        public static MvcHtmlString ActionLinkDefault(this HtmlHelper helper, string text, string action, object routeValues)
        {
            return ActionLinkBase(helper, "btn-default", text, action, null, routeValues, null, null);
        }
        public static MvcHtmlString ActionLinkDefault(this HtmlHelper helper, string text, string action, object routeValues, string onClick)
        {
            return ActionLinkBase(helper, "btn-default", text, action, null, routeValues, null, onClick);
        }
        public static MvcHtmlString ActionLinkDefault(this HtmlHelper helper, string text, string action)
        {
            return ActionLinkBase(helper, "btn-default", text, action, null, null, null, null);
        }
        public static MvcHtmlString ActionLinkDefault(this HtmlHelper helper, string text, string action, string controller)
        {
            return ActionLinkBase(helper, "btn-default", text, action, controller, null, null, null);
        }

        #endregion ActionLinkDefault
    }
}