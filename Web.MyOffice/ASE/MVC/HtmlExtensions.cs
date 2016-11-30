using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ASE.MVC
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString NewLine2Br(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrEmpty(text))
                return MvcHtmlString.Create(text);
            else
            {
                StringBuilder builder = new StringBuilder();
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0)
                        builder.Append("<br/>");
                    builder.Append(HttpUtility.HtmlEncode(lines[i]));
                }
                return MvcHtmlString.Create(builder.ToString());
            }
        }

        public static MvcHtmlString RadioButtonForEnum<TModel, TProperty>(
                this HtmlHelper<TModel> htmlHelper,
                Expression<Func<TModel, TProperty>> expression
            )
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var names = Enum.GetNames(metaData.ModelType);
            var sb = new StringBuilder();
            foreach (var name in names)
            {
                var id = string.Format(
                    "{0}_{1}_{2}",
                    htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                    metaData.PropertyName,
                    name
                );

                string name2 = name;
                var memInfo = metaData.ModelType.GetMember(name);
                if (memInfo != null)
                {
                    var attributes = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                        name2 = ((DisplayAttribute)attributes[0]).Name;
                }


                var radio = htmlHelper.RadioButtonFor(expression, name, new { id = id }).ToHtmlString();
                sb.AppendFormat(
                    "<div class=\"radio\"><label for=\"{0}\">{1}</label> {2}</div>",
                    id,
                    HttpUtility.HtmlEncode(name2),
                    radio
                );
            }
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}