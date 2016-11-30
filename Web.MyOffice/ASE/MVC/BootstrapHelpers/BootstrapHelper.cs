using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace ASE.MVC.BootstrapHelpers
{
    public static class BootstrapHelper
    {
        public static IDisposable Row(this HtmlHelper helper)
        {
            helper.ViewContext.Writer.Write(@"<div class=""row"">");

            return new TagTerminator(helper, @"</div>");
        }

        public static MvcHtmlString Row(this HtmlHelper helper, params MvcHtmlString[] columns)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.Attributes.Add("class", "row");

            tag.InnerHtml = "";
            foreach (var item in columns)
                tag.InnerHtml += item.ToString();

            return new MvcHtmlString(tag.ToString());
        }

        public static IDisposable Row(this HtmlHelper helper, string name)
        {
            helper.ViewContext.Writer.Write(String.Format("<div id=\"{0}\" class=\"row\">", name));

            return new TagTerminator(helper, @"</div>");
        }

        public static MvcHtmlString RowDivider(this HtmlHelper helper, int size)
        {
            return new MvcHtmlString(String.Format(@"<div class=""row"" style=""min-height: {0}px""></div>", size));
        }

        public static string GlobalCoumn = "col-md-";

        public static IDisposable Col(this HtmlHelper helper, int size)
        {
            return Col(helper, size, new { });
        }

        public static IDisposable Col(this HtmlHelper helper, int size, object attrs)
        {
            TagBuilder tag = new TagBuilder("div");
            RouteValueDictionary dict = new RouteValueDictionary(attrs);
            tag.Attributes.Add("class", dict["class"] + " " + String.Format(@"{0}{1}", GlobalCoumn, size));

            helper.ViewContext.Writer.Write(tag.ToString(TagRenderMode.StartTag));

            return new TagTerminator(helper, @"</div>");
        }

        public static MvcHtmlString Col(this HtmlHelper helper, int size, params MvcHtmlString[] items)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.Attributes.Add("class", String.Format(@"{0}{1}", GlobalCoumn, size));
            
            tag.InnerHtml = "";
            if (items != null)
                foreach (var item in items)
                    if (item != null)
                        tag.InnerHtml += item.ToString();

            return new MvcHtmlString(tag.ToString());;
        }

        public static IDisposable ColForm(this HtmlHelper helper, int size)
        {
            helper.ViewContext.Writer.Write(String.Format(@"<div class=""{0}{1}""><form method=""post"">", GlobalCoumn, size));

            return new TagTerminator(helper, @"</div></form>");
        }

        public static IDisposable Table<T>(this HtmlHelper helper, SearchablePagedList<T> model)
        {
            return Table<T>(helper, "", null);
        }

        public static IDisposable Table<T>(this HtmlHelper helper, IEnumerable<T> model)
        {
            return Table<T>(helper, "", null);
        }

        public static IDisposable Table<T>(this HtmlHelper helper, string type, SearchablePagedList<T> model)
        {
            helper.ViewContext.Writer.Write(String.Format(@"<table class=""table table-hover {0}"">", type));

            string pager = "";

            if ((model != null) && (String.IsNullOrEmpty(model.SearchString)))
                pager = helper.PagerForString(model, "Index", helper.ViewContext.Controller.GetType().Name.Replace("Controller", ""), "Gett()");

            return new TagTerminator(helper, @"</table>" + pager);
        }

        public static IDisposable Table<T>(this HtmlHelper helper, string type, IEnumerable<T> model)
        {
            helper.ViewContext.Writer.Write(String.Format(@"<table class=""table {0}"">", type));

            return new TagTerminator(helper, @"</table>");
        }

        public static IDisposable TableStripped<T>(this HtmlHelper helper)
        {
            return Table<T>(helper, "table-striped", null);
        }

        public static IDisposable TableStripped<T>(this HtmlHelper helper, SearchablePagedList<T> model)
        {
            return Table(helper, "table-striped", model);
        }

        public static MvcHtmlString TextBoxSearchFilter(this HtmlHelper helper, string name, string placeholder,  object routeValues)
        {
            return TextBoxSearchFilter(helper, name, placeholder, null, null, routeValues, null);
        }
        public static MvcHtmlString TextBoxSearchFilter(this HtmlHelper helper, string name, string placeholder, string action, string controllerName, object routeValues, string resultDiv)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            RouteValueDictionary rv = new RouteValueDictionary(routeValues);
            rv = rv.Merge(new
                {
                    @class = "pull-right clearable",
                    onkeyup = "ASE.SearchTextBox(this)",
                    onkeypress = "ASE.SearchTextBox(this)",
                    placeholder = placeholder,
                    data = String.Format("#{0}", resultDiv ?? "data"),
                    onfocus = "this.value = this.value;",
                    isSelect = helper.ViewContext.RequestContext.HttpContext.Request.Params["isSelect"],
                    href = urlHelper.Action(action, controllerName, helper.ViewContext.RouteData.Values)
                });

            return helper.TextBox(
                "searchTB_" + name, 
                helper.ViewContext.HttpContext.Request.Params.AllKeys.Contains("searchTB_" + name) ? helper.ViewContext.HttpContext.Request.Params["searchTB_" + name] : "",
                rv
                );
        }
        public static MvcHtmlString TextBoxSearch(this HtmlHelper helper, string name, object routeValues)
        {
            
            return helper.TextBoxSearchFilter( name, "Поиск",
                helper.ViewContext.Controller.ValueProvider.GetValue("action").RawValue.ToString(),
                helper.ViewContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString(),
                routeValues,
                null);
        }
        public static MvcHtmlString TextBoxFilter(this HtmlHelper helper, string name, object routeValues)
        {
            return helper.TextBoxSearchFilter(name, "Фильтр",
                helper.ViewContext.Controller.ValueProvider.GetValue("action").RawValue.ToString(),
                helper.ViewContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString(),
                routeValues,
                null);
        }

        public static IDisposable FormInline(this HtmlHelper helper)
        {
            helper.ViewContext.Writer.Write(@"<div class=""form-inline"">");

            return new TagTerminator(helper, @"</div>");
        }

        public static IDisposable BeginFormInline(this HtmlHelper helper)
        {
            helper.ViewContext.Writer.Write(@"<form class=""form-inline"" method=""post"">");

            return new TagTerminator(helper, @"</form>");
        }

        public static IDisposable BeginFormInline(this HtmlHelper helper, bool get)
        {
            helper.ViewContext.Writer.Write(@"<form class=""form-inline"" method=""{0}"">", get ? "get" : "post");

            return new TagTerminator(helper, @"</form>");
        }

        public static Bootstrap Bootstrap(this HtmlHelper helper)
        {
            return new Bootstrap();
        }
    }

    public class Bootstrap
    {
    }

    public static class BootstrapExtensions
    {
        public static string Test(this Bootstrap bs)
        {
            return "";
        }
    }
}