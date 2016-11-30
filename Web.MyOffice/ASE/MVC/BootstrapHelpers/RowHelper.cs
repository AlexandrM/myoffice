using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC.BootstrapHelpers
{
    public static class RowHelper
    {
        public static IDisposable BeginRow(this HtmlHelper helper)
        {
            helper.ViewContext.Writer.Write("<div class=\"row\">");

            return new TagTerminator(helper, "</div>");
        }
    }
}