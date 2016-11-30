using ASE.ToolsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.MyOffice.Res;

namespace ASE.MVC.BootstrapHelpers
{
    public class Settings
    {
        public static ResourceManager LocalizeResourceManager { get; set; }
    }

    public static class ButtonHelper
    {
        public static string DefaultClasses = "btn btn-primary";

        public static MvcHtmlString ButtonSubmit(this HtmlHelper helper, string text)
        {
            return Button(helper, text, "submit", new { @class = DefaultClasses });
        }

        public static MvcHtmlString ButtonSubmit(this HtmlHelper helper, string text, dynamic attributes)
        {
            return Button(helper, text, "submit", attributes);
        }
        public static MvcHtmlString Button(this HtmlHelper helper, string text, dynamic attributes)
        {
            return Button(helper, text, "", attributes);
        }
        public static MvcHtmlString ButtonSuccess(this HtmlHelper helper, string text, dynamic attributes)
        {
            return Button(helper, text, "", attributes);
        }

        public static MvcHtmlString ButtonPrimary(this HtmlHelper helper, string text, dynamic attributes)
        {
            RouteValueDictionary hal = new RouteValueDictionary(attributes);
            hal = hal.Merge(new
            {
                @class = "btn btn-primary"
            });
            
            return Button(helper, text, "", hal);
        }

        public static MvcHtmlString ButtonPrimarySelect(this HtmlHelper helper, ToolModelIdValue resultField, ToolModelIdValue value)
        {
            return ButtonPrimarySelect(helper, resultField, value, new { });
        }

        public static MvcHtmlString ButtonPrimarySelect(this HtmlHelper helper, ToolModelIdValue resultField, ToolModelIdValue value, dynamic attributes)
        {
            RouteValueDictionary hal = new RouteValueDictionary(attributes);
            hal = hal.Merge(new
            {
                @class = "btn btn-primary glyphicon glyphicon-ok",
                onclick = String.Format("ASE.SelectDo(this, '{0}', '{1}', '{2}', '{3}'); return false;", resultField.Id, resultField.Value, value.Id, value.Value)
            });

            return Button(helper, "", "", hal);
        }

        public static MvcHtmlString ButtonPrimaryRefresh(this HtmlHelper helper, string name)
        {
            return ButtonPrimary(helper, 
                Settings.LocalizeResourceManager.GetString("Refresh") + @" <span class=""glyphicon glyphicon-refresh""></span>"
                , new { @class = "", onclick = String.Format("{0}_Refresh('{0}')", name) });
        }

        public static MvcHtmlString ButtonSubmitPrimaryRefresh(this HtmlHelper helper)
        {
            if ((helper.ViewBag.RenderToPdf != null) && (helper.ViewBag.RenderIfPdf == null))
                return new MvcHtmlString("");

            return Button(helper
                , Settings.LocalizeResourceManager.GetString("Refresh") + @" <span class=""glyphicon glyphicon-refresh""></span>"
                , ""
                , new { @class = "btn btn-primary", onclick = "ASE.Refresh(this); return false;" });
        }

        public static MvcHtmlString ButtonDelete(this HtmlHelper helper, string action, string controller, object id, string message, dynamic attributes)
        {
            if ((helper.ViewBag.RenderToPdf != null) && (helper.ViewBag.RenderIfPdf == null))
                return new MvcHtmlString("");

            RouteValueDictionary hal = new RouteValueDictionary(attributes);
            hal = hal.Merge(new
            {
                @class = "btn btn-danger",
                onclick = String.Format("ASE.Delete(this, '{0}', '{1}', '{2}', '{3}'); return false;", message, action, controller, id.ToString())
            });

            return Button(helper
                , S.Delete + @" <span class=""glyphicon glyphicon-remove""></span>"
                , ""
                , hal);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, string text, string type, dynamic attributes)
        {
            TagBuilder tb = new TagBuilder("button");
            RouteValueDictionary hal = new RouteValueDictionary(attributes);
            tb.InnerHtml = text;
            if (!String.IsNullOrEmpty(type))
                tb.Attributes.Add("type", type);
            foreach(var item in hal)
                tb.Attributes.Add(item.Key, item.Value.ToString());

            return new MvcHtmlString(tb.ToString());
        }
        
        public static MvcHtmlString ButtonCreate(this HtmlHelper helper)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<div class=""form-group"">");
            sb.Append(@"<div class=""col-md-offset-2 col-md-10"">");
            sb.Append(ButtonSubmit(helper, S.Add));
            sb.Append("</div>");
            sb.Append("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString ButtonSave(this HtmlHelper helper, bool visible = true)
        {
            if (!visible)
                return new MvcHtmlString("");

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<div class=""form-group"">");
            sb.Append(@"<div class=""col-md-offset-2 col-md-10"">");
            sb.Append(ButtonSubmit(helper, S.Save));
            sb.Append("</div>");
            sb.Append("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString ButtonUpload(this HtmlHelper helper, bool visible = true)
        {
            if (!visible)
                return new MvcHtmlString("");

            StringBuilder sb = new StringBuilder();
            //sb.Append(@"<div class=""form-group"">");
            //sb.Append(@"<div class=""col-md-offset-2 col-md-10"">");
            sb.Append(Button(helper, S.AddFile, "file", new { @class = "btn btn-primary"}));
            //sb.Append("</div>");
            //sb.Append("</div>");

            return new MvcHtmlString(sb.ToString());
        }
    }
}