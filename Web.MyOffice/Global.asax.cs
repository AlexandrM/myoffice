using ASE.MVC;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Web.MyOffice
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ru-RU");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("ru-RU");

            System.Data.Entity.Database.SetInitializer<Web.MyOffice.Data.DB>(new Web.MyOffice.Data.InitData());
            System.Data.Entity.Database.SetInitializer<ApplicationDbContext>(new ASE.EF.AutoMigrationInit<ApplicationDbContext>());

            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            GlobalFilters.Filters.Add(new ASE.MVC.HistoryActionFilterAttribute());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ASE.MVC.BootstrapHelpers.Settings.LocalizeResourceManager = Web.MyOffice.Res.S.ResourceManager;
            
            ASE.MVC.LocalizedRequiredAttribute.ResourceManager = Web.MyOffice.Res.S.ResourceManager;
            ASE.MVC.LocalizedDisplayAttribute.ResourceManager = Web.MyOffice.Res.S.ResourceManager;
            ASE.MVC.LocalizedMinLengthAttribute.ResourceManager = Web.MyOffice.Res.S.ResourceManager;
            ASE.MVC.LocalizedCompareAttribute.ResourceManager = Web.MyOffice.Res.S.ResourceManager;

            ASE.MVC.IdentinityExtension.ResourceManager = Web.MyOffice.Res.S.ResourceManager;

            ModelBinders.Binders.Add(typeof(decimal), new MvcDecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new MvcDecimalModelBinder());

            ModelBinders.Binders.Add(typeof(DateTime), new MvcDateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new MvcDateTimeModelBinder());

            //ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ViewEngineAdv());

            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalizedRequiredAttribute), typeof(RequiredAttributeAdapter));
        }

        protected void Application_BeginRequest()
        {
            string cultureName = CultureHelper.GetCultureFromCookies(new HttpRequestWrapper(Request));

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);
        }
    }
}
