using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ASE.MVC.BootstrapHelpers
{
    public static class EditorHelper
    {
        public class DynamicDictionary : DynamicObject
        {
            IDictionary<string, object> dict;

            public DynamicDictionary(IDictionary<string, object> dict)
            {
                this.dict = dict;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                dict[binder.Name] = value;
                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                return dict.TryGetValue(binder.Name, out result);
            }
        }
        public static MvcHtmlString EditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool disabled, object additionalViewData)
        {
            var str = html.EditorFor(expression, additionalViewData).ToHtmlString();
            if (disabled)
                str = str.Insert(6, " disabled=\"\" ");
            
            return new MvcHtmlString(str);
        }
    }
}