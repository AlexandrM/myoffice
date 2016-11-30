using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Web.Mvc.Ajax;

namespace ASE.MVC
{
    public class SearchablePagedList<T> : PagedList<T>
    {
        public string SearchString { get; set; }
        public string SortOrder { get; set; }
        public string Filter { get; set; }

        public SearchablePagedList(IEnumerable<T> items, int? pageNumber, int pageSize)
            : base(items, pageNumber ?? 1, pageSize)
        {
        }

        public SearchablePagedList(IEnumerable<T> items, int? pageNumber, int pageSize, string searchString)
            : base(items, pageNumber ?? 1, pageSize)
        {
            SearchString = searchString;
        }

        public static SearchablePagedList<T> Create(IEnumerable<T> items, int? pageNumber, int pageSize, string searchString)
        {
            pageNumber = pageNumber ?? 1;
            if (!String.IsNullOrEmpty(searchString))
                pageNumber = 1;
            return new SearchablePagedList<T>(items, pageNumber, pageSize, searchString);
        }
    }

    public static class PagerHelper
    {
        public static void PagerFor<T>(this System.Web.Mvc.HtmlHelper helper, SearchablePagedList<T> model, string action, string controller, string ajaxTarget = null)
        {
            var pageNumber = (model.PageCount < model.PageNumber) ? 0 : model.PageNumber;
            var pageCount = model.PageCount;

            var builder = new TagBuilder("p");
            builder.SetInnerText(string.Format("Страница {0} из {1}", pageNumber, pageCount));

            helper.ViewContext.Writer.Write(builder);

            var ajax = !string.IsNullOrEmpty(ajaxTarget);
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            var pager = helper.PagedListPager(
                model,
                page => urlHelper.Action(action, controller, new { page, sortOrder = model.SortOrder, searchString = model.SearchString, filter = model.Filter, isSelect = helper.ViewContext.HttpContext.Request["isSelect"] }),
                ajax ? PagedListRenderOptions.EnableUnobtrusiveAjaxReplacing(new AjaxOptions { HttpMethod = "POST", OnSuccess = "ASE.DataAjaxOnSuccess", OnBegin = "ASE.DataAjaxOnBegin" }) : PagedListRenderOptions.Classic);

            helper.ViewContext.Writer.Write(pager.ToHtmlString());
        }

        public static string PagerForString<T>(this System.Web.Mvc.HtmlHelper helper, SearchablePagedList<T> model, string action, string controller, string ajaxTarget = null)
        {
            var pageNumber = (model.PageCount < model.PageNumber) ? 0 : model.PageNumber;
            var pageCount = model.PageCount;

            var builder = new TagBuilder("p");
            builder.SetInnerText(string.Format("Страница {0} из {1}", pageNumber, pageCount));

            helper.ViewContext.Writer.Write(builder);

            var ajax = !string.IsNullOrEmpty(ajaxTarget);
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            var pager = helper.PagedListPager(
                model,
                page => urlHelper.Action(action, controller, new { page, sortOrder = model.SortOrder, searchString = model.SearchString, filter = model.Filter, isSelect = helper.ViewContext.HttpContext.Request["isSelect"] }),
                ajax ? PagedListRenderOptions.EnableUnobtrusiveAjaxReplacing(new AjaxOptions { HttpMethod = "POST", OnSuccess = "ASE.DataAjaxOnSuccess", OnBegin = "ASE.DataAjaxOnBegin" }) : PagedListRenderOptions.Classic);

            return pager.ToHtmlString();
        }
    }
}