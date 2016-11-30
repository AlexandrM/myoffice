using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace ASE.MVC
{
    public class MvcDateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                var date = value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
                return date;
            }
            catch
            {
            }
            return null;
        }
    }

    public class MvcDecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                string s = value.AttemptedValue;
                s = s.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                s = s.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                if (String.IsNullOrWhiteSpace(s))
                    s = "0";

                var date = Convert.ToDecimal(s, CultureInfo.CurrentCulture);
                return date;
            }
            catch
            {
            }
            return null;
        }
    }

    public class ApiDateTimeModelBinder : System.Web.Http.ModelBinding.IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, System.Web.Http.ModelBinding.ModelBindingContext bindingContext)
        {
            bindingContext.Model = bindingContext.ValueProvider
                        .GetValue(bindingContext.ModelName)
                        .ConvertTo(bindingContext.ModelType, Thread.CurrentThread.CurrentCulture);

            bindingContext.ValidationNode.ValidateAllProperties = true;

            return true;
        }
    }

    public class ApiDecimalModelBinder : System.Web.Http.ModelBinding.IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, System.Web.Http.ModelBinding.ModelBindingContext bindingContext)
        {
            return true;
        }
    }
}