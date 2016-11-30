using ASE.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Web.MyOffice
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            /*config.Routes.MapHttpRoute(
                name: "DefaultApiWithAction",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );*/

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.BindParameter(typeof(decimal), new ApiDecimalModelBinder());
            config.BindParameter(typeof(decimal?), new ApiDecimalModelBinder());

            config.BindParameter(typeof(DateTime), new ApiDateTimeModelBinder());
            config.BindParameter(typeof(DateTime?), new ApiDateTimeModelBinder());
            //config.MessageHandlers.Add(new RequestRawHandler());
        }
    }
}
