using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ASE.MVC
{
    public static class BasicHelpers
    {
        public static MvcHtmlString Div(this HtmlHelper helper, object attrs)
        {
            return Tag(helper, "div", attrs, null);
        }

        public static MvcHtmlString Div(this HtmlHelper helper, object attrs, params dynamic[] inner)
        {
            return Tag(helper, "div", attrs, inner);
        }

        public static MvcHtmlString Tag(this HtmlHelper helper, string tag, object attrs, params dynamic[] inner)
        {
            TagBuilder tagAll = new TagBuilder(tag);
            RouteValueDictionary rvdAttrs = new RouteValueDictionary(attrs);
            foreach (var key in rvdAttrs.Keys)
                tagAll.Attributes.Add(key, rvdAttrs[key].ToString());

            if (inner != null)
                foreach (var item in inner)
                    tagAll.InnerHtml += item.ToString();

            return new MvcHtmlString(tagAll.ToString());
        }

        public static IDisposable TagBegin(this HtmlHelper helper, string tag, object attrs, params dynamic[] inner)
        {
            TagBuilder tagAll = new TagBuilder(tag);
            RouteValueDictionary rvdAttrs = new RouteValueDictionary(attrs);
            foreach (var key in rvdAttrs.Keys)
                tagAll.Attributes.Add(key, rvdAttrs[key].ToString());

            helper.ViewContext.Writer.Write(tagAll.ToString(TagRenderMode.StartTag));

            if (inner != null)
                foreach (var item in inner)
                    tagAll.InnerHtml += item.ToString();

            return new TagTerminator(helper, tag);
        }
    }

    public class TagTerminator : IDisposable
    {
        readonly HtmlHelper helper;
        readonly string terminator;

        public TagTerminator(HtmlHelper helper, string terminator)
        {
            this.helper = helper;
            this.terminator = terminator;
        }

        public void Dispose()
        {
            helper.ViewContext.Writer.Write(terminator);
        }
    }
}