using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC.BootstrapHelpers
{
    public static class TableHelper
    {
        public static string DefaultClasses = "table-striped";

        public static IDisposable BeginTable(this HtmlHelper helper)
        {
            return BeginTable(helper, DefaultClasses);
        }

        public static IDisposable BeginTable(this HtmlHelper helper, string type)
        {
            helper.ViewContext.Writer.Write(String.Format("<table class=\"table {0}\">", type));

            return new TagTerminator(helper, "</table>");
        }

        public static void TableHeader(this HtmlHelper helper, params dynamic[] columns)
        {
            helper.ViewContext.Writer.Write("<tr>");

            string name = "";
            foreach(var item in columns)
            {
                PropertyInfo[] pis = item.GetType().GetProperties();

                helper.ViewContext.Writer.Write("<th ");
                foreach(var pi in pis)
                {
                    if (pi.Name == "name")
                        name = pi.GetValue(item);
                    else
                        helper.ViewContext.Writer.Write(String.Format("{0}=\"{1}\"", pi.Name, pi.GetValue(item)));
                }
                helper.ViewContext.Writer.Write(">");
                helper.ViewContext.Writer.Write(name);
                helper.ViewContext.Writer.Write("</th>");
            }
        }

        public static void TableRow(this HtmlHelper helper,  dynamic tr, params dynamic[] td)
        {
            helper.ViewContext.Writer.Write("<tr");
            foreach (var item in tr)
            {
                PropertyInfo[] pis = item.GetType().GetProperties();
                foreach (var pi in pis)
                    helper.ViewContext.Writer.Write(String.Format("{0}=\"{1}\"", pi.Name, pi.GetValue(item)));
            }
            helper.ViewContext.Writer.Write(">");

            string value = "";
            foreach (var item in td)
            {
                PropertyInfo[] pis = item.GetType().GetProperties();

                helper.ViewContext.Writer.Write("<td ");
                foreach (var pi in pis)
                {
                    if (pi.Name == "value")
                        value = pi.GetValue(item);
                    else
                        helper.ViewContext.Writer.Write(String.Format("{0}=\"{1}\"", pi.Name, pi.GetValue(item)));
                }
                helper.ViewContext.Writer.Write(">");
                helper.ViewContext.Writer.Write(value);
                helper.ViewContext.Writer.Write("</td>");
            }
        }
    }
}