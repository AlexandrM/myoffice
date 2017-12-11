using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ASE.MVC
{
    public static class Extensions
    {
        public static string RenderPartialView(this Controller controller, string viewName)
        {
            return RenderPartialView(controller, viewName, null);
        }

        public static string RenderPartialView(this Controller controller, string viewName, object model)
        {
            if (controller.ControllerContext == null)
            {
                controller.ControllerContext = new ControllerContext(new HttpContextWrapper(HttpContext.Current),new RouteData(), controller);
                controller.RouteData.Values.Add("controller", "ChatController");
            }

            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public static string RenderPartialView<DBType>(this ControllerAdv<DBType> controller, string viewName) where DBType : DbContext, new()
        {
            return RenderPartialView(controller, viewName, null);
        }

        public static string RenderPartialView<DBType>(this ControllerAdv<DBType> controller, string viewName, object model) where DBType : DbContext, new()
        {
            if (controller.ControllerContext == null)
            {
                controller.ControllerContext = new ControllerContext(new HttpContextWrapper(HttpContext.Current), new RouteData(), controller);
                controller.RouteData.Values.Add("controller", "ChatController");
            }

            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewEngineAdv = ViewEngines.Engines.ToList().Find(x=> x.GetType() == typeof(ViewEngineAdv));
                var viewResult = viewEngineAdv.FindPartialView(controller.ControllerContext, viewName, true);
                
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                string top = "";
                if (controller.IsSelect)
                {
                    top += "<input type=\"hidden\" id=\"isSelect\" value=\"true\">";
                    top += String.Format("<input type=\"hidden\" id=\"selectResultFieldId\" value=\"{0}\">", controller.SelectResultField.Id);
                    top += String.Format("<input type=\"hidden\" id=\"selectResultField\" value=\"{0}\">", controller.SelectResultField.Value);
                }

                return top + sw.GetStringBuilder().ToString();
            }
        }

        public static string RelativePath(this HttpRequestBase request, string path)
        {
            string approot = request.PhysicalApplicationPath.TrimEnd('\\');
            path = path.Replace(approot, string.Empty).Replace('\\', '/');
            UrlHelper uh = new UrlHelper(request.RequestContext);
            path = uh.Content("~" + path);
            return path;
        }

        public static string ResolveServerUrl(this HttpRequestBase request, string serverUrl, bool forceHttps)
        {
            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;

            string newUrl = serverUrl;
            Uri originalUri = System.Web.HttpContext.Current.Request.Url;
            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                "://" + originalUri.Authority + newUrl;
            return newUrl;
        }

        public static string ResolveFileUrl(this HttpRequestBase request, string file, bool forceHttps)
        {
            return ResolveServerUrl(request, RelativePath(request, file), false);
        }

        public static void WriteToLog(this Controller controller, string info)
        {
            string path = System.IO.Path.Combine(
                               AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
                               "log.txt"
                               );
            System.IO.File.AppendAllText(path, info);
        }

        public static HtmlHelper GetHtmlHelper(this Controller controller)
        {
            var viewContext = new ViewContext(controller.ControllerContext, new FakeView(), controller.ViewData, controller.TempData, TextWriter.Null);
            return new HtmlHelper(viewContext, new ViewPage());
        }

        public class FakeView : IView
        {
            public void Render(ViewContext viewContext, System.IO.TextWriter writer)
            {
                throw new InvalidOperationException();
            }
        }

        public static UrlHelper GetUrlHelper(this Controller controller)
        {
            return new UrlHelper(controller.HttpContext.Request.RequestContext);
        }

        public static ViewContext GetViewContext(this Controller controller)
        {
            return new ViewContext(controller.ControllerContext, new FakeView(), controller.ViewData, controller.TempData, TextWriter.Null);
        }

        public static string JsonSerialize(this HtmlHelper htmlHelper, object value)
        {
            string ret = new JavaScriptSerializer().Serialize(value);
            return ret;
        }

        public static Guid ProviderUserKey(this Controller controller)
        {
            return (Guid)Membership.GetUser().ProviderUserKey;
        }

        public static string DisplayName(this Enum value)
        {
            var type = value.GetType();
            if (!type.IsEnum) throw new ArgumentException(String.Format("Type '{0}' is not Enum", type));

            var members = type.GetMember(value.ToString());
            if (members.Length == 0) throw new ArgumentException(String.Format("Member '{0}' not found in type '{1}'", value, type.Name));

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length == 0) throw new ArgumentException(String.Format("'{0}.{1}' doesn't have DisplayAttribute", type.Name, value));

            var attribute = (DisplayAttribute)attributes[0];
            return attribute.GetName();
        }

        public static RouteValueDictionary Extend(this RouteValueDictionary dest, IEnumerable<KeyValuePair<string, object>> src)
        {
            foreach (var item in src.ToList())
                if (dest[item.Key] == null)
                    dest[item.Key] = item.Value;
                else
                    dest[item.Key] = dest[item.Key] + " " + item.Value;
            return dest;
        }

        public static RouteValueDictionary Merge(this RouteValueDictionary source, IEnumerable<KeyValuePair<string, object>> newData)
        {
            return (new RouteValueDictionary(source)).Extend(newData);
        }
        public static RouteValueDictionary Merge(this RouteValueDictionary source, object newData)
        {
            return (new RouteValueDictionary(source)).Extend(new RouteValueDictionary(newData));
        }

        public static Guid UserId(this HttpContext context)
        {
            return Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
        }

        public static Guid? ValidateUID(this ApiController apiCtrl)
        {
            if (!apiCtrl.Request.Headers.Contains("UID"))
            {
                return null;
            }

            Guid uid = Guid.Empty;
            if (Guid.TryParse(apiCtrl.Request.Headers.FirstOrDefault(x => x.Key == "UID").Value.FirstOrDefault(), out uid))
            {
                return uid;
            }


            return null;
        }
    }
}