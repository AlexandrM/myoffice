using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ASE.MVC
{
    public static class Helpers
    {
        public static MvcHtmlString PartialBlock(this HtmlHelper helper, string id, string view, object model)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.GenerateId(id);
            tag.Attributes.Add("class", "partial");
            tag.InnerHtml = helper.Partial(view, model).ToHtmlString();
            return new MvcHtmlString(tag.ToString());
        }
    }
}