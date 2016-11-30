using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC.Bootstrap3
{
    public static class BS
    {
        /*public static IDisposable Container(HtmlAttribute attrs)
        {
            string ret = "";

            return new MvcHtmlString(ret);
        }*/

        public static class Row
        {

        }

        /*public static FormHLabelEditorValidation<TModel, TProperty> FormHLabelEditorValidation()
        {

        }*/
    }

    public class HtmlAttribute<TModel, TProperty>
    {
        
        public dynamic Class { get; set; }

        public dynamic Style { get; set; }

        public dynamic Attributes { get; set; }

        public Expression<Func<TModel, TProperty>> Property { get; set; }
    }

    public class FormHLabelEditorValidation<TModel, TProperty>
    {
        public HtmlAttribute<TModel, TProperty> Label { get; set; }
        
        public HtmlAttribute<TModel, TProperty> Value { get; set; }
        
        public HtmlAttribute<TModel, TProperty> Validation { get; set; }
    }
}