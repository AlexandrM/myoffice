using ASE.EF;
using ASE.MVC.Bootstrap3;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace ASE.MVC.BootstrapHelpers
{
    public static class FormHelper
    {
        public static MvcForm FormHorizontal(this HtmlHelper helper)
        {
            return helper.BeginForm(null, null, FormMethod.Post, new { @class = "form-horizontal container" });
        }

        public static MvcForm FormHorizontal(this HtmlHelper helper, string action, string controller, bool multipart)
        {
            return helper.BeginForm(action, controller, FormMethod.Post, new { @class = "form-horizontal container", enctype = "multipart/form-data" });
        }

        public static MvcForm FormHorizontal(this HtmlHelper helper, string action, string controller, bool multipart, object attrs)
        {
            RouteValueDictionary adv = new RouteValueDictionary(attrs);
            if (!adv.Keys.Contains("class"))
                adv["class"] = "";
            adv["class"] = adv["class"] + " form-horizontal container";
            if (multipart)
                adv["enctype"] = "multipart/form-data";

            return helper.BeginForm(action, controller, FormMethod.Post, adv);
        }

        public static MvcHtmlString RowLabelInputButton<TModel, TValue, TProperty> (this HtmlHelper<TModel> helper, 
            Expression<Func<TModel, TValue>> label,
            Expression<Func<TModel, TProperty>> value,
            string action, string controller,
            string toAction, string toController)
        {
            string ret = "<div class=\"row\">";
            ret += "<div class=\"form-group\">";
            ret += helper.LabelFor(label, new { @class = "control-label col-md-2" });
            ret += "<div class=\"col-md-4\">";
            ret += "<div class=\"input-group\">";
            ret += helper.TextBoxFor(value, new { @class = "form-control", disabled = "" });
            ret += "<span class=\"input-group-btn\">";
            ret += helper.ActionLinkPrimary("...", action, controller, new { toAction = toAction, toController = toController });
            ret += "</span>";
            ret += "</div>";
            ret += "</div>";
            ret += "</div>";
            ret += "</div>";

            return new MvcHtmlString(ret);
        }

        public class MyVisitor: ExpressionVisitor
        {
            public Expression Modify(Expression expression)
            {
                Expression expr = Expression.Property(expression, "Displayable");
                return Visit(expr);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                return base.VisitBinary(node);
            }
        }

        public static MvcHtmlString RowLabelEditor(this HtmlHelper helper, string id, string label, object value, object attributes)
        {
            RouteValueDictionary hal = new RouteValueDictionary(attributes);

            string ret = "<div class=\"row\">";
            ret += "<div class=\"form-group\">";
            ret += helper.Label(label, new { @class = "control-label col-md-2" });
            ret += "<div class=\"col-md-4\">";
            hal = hal.Merge(new { @class = "form-control " });
            ret += helper.TextBox(id, value, hal);
            ret += "</div>";
            ret += "</div>";
            ret += "</div>";

            return new MvcHtmlString(ret);
        }

        public static MvcHtmlString RowLabelEditorValidation<TModel, TValue, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> label,
            Expression<Func<TModel, TProperty>> value,
            Expression<Func<TModel, TProperty>> validation)
        {
            string ret = "<div class=\"row\">";
            ret += LabelEditorValidation(helper, label, value, validation).ToString();
            ret += "</div>";

            return new MvcHtmlString(ret);
        }
        public static MvcHtmlString LabelEditorValidation<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property)
        {
            return helper.LabelEditorValidation(property, property, null);
        }

        public static MvcHtmlString LabelEditorValidation<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property, int columns)
        {
            return helper.LabelEditorValidation(property, property, null, columns, null);
        }
        public static MvcHtmlString LabelEditorValidation<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property, object additionalViewData)
        {
            return helper.LabelEditorValidation(property, property, property, 2, additionalViewData);
        }
        

        public static MvcHtmlString LabelEditorValidation<TModel, TValue, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> label,
            Expression<Func<TModel, TProperty>> value,
            Expression<Func<TModel, TProperty>> validation)
        {
            return helper.LabelEditorValidation(label, value, validation, 2, null);
        }

        public static MvcHtmlString LabelEditorValidation<TModel, TValue, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> label,
            Expression<Func<TModel, TProperty>> value,
            Expression<Func<TModel, TProperty>> validation,
            int columnDivider,
            object additionalViewData)
        {
            RouteValueDictionary adv = new RouteValueDictionary(additionalViewData);
            TagBuilder tagAll = new TagBuilder("div");
            
            #region label

            RouteValueDictionary advLabel = new RouteValueDictionary();
            //label params
            if (adv.Keys.Contains("label"))
                advLabel = new RouteValueDictionary(adv["label"]);

            //label "class"
            if (!advLabel.Keys.Contains("class"))
                advLabel.Add("class", "control-label");
            else
                advLabel["class"] = advLabel["class"] + " control-label";

            //is label "size" as "col-sm-8"
            var fullSize = advLabel.FirstOrDefault(x => x.Key.ToLower().IndexOf("col-") == 0);

            if (adv.Keys.Contains("size48")) //size48 = col-sm-4
                advLabel["class"] = advLabel["class"] + " col-sm-4";
            else if (fullSize.Key != null) //size48 = col-sm-4
            {
                advLabel["class"] = advLabel["class"] + fullSize.Value.ToString();
                advLabel.Remove(fullSize.Key);
            }
            else if (advLabel.Keys.Contains("size"))
            {
                advLabel["class"] = advLabel["class"] + String.Format(" col-md-{0}", advLabel["size"]);
                advLabel.Remove("size");
            }
            else
                advLabel["class"] = advLabel["class"] + " col-md-2";

            tagAll.Attributes.Add("class", "form-group");
            tagAll.InnerHtml = helper.LabelFor(label, advLabel).ToHtmlString();

            #endregion label 

            #region group

            RouteValueDictionary advGroup = new RouteValueDictionary();
            if (adv.Keys.Contains("group"))
                advGroup = new RouteValueDictionary(adv["group"]);

            if (!advGroup.Keys.Contains("class"))
                advGroup.Add("class", "");
            else
                advGroup["class"] = advGroup["class"] + " ";

            fullSize = advGroup.FirstOrDefault(x => x.Key.ToLower().IndexOf("col-") == 0);
            if (adv.Keys.Contains("size48"))
                advGroup["class"] = advGroup["class"] + " col-md-8";
            else if (fullSize.Key != null)
            {
                advGroup["class"] = advGroup["class"] + fullSize.Value.ToString();
                advGroup.Remove(fullSize.Key);
            }
            else if (advGroup.Keys.Contains("size"))
            {
                advGroup["class"] = advGroup["class"] + String.Format(" col-md-{0}", advGroup["size"]);
                advGroup.Remove("size");
            }
            else
                advGroup["class"] = advGroup["class"] + " col-md-4";

            TagBuilder tagInputs = new TagBuilder("div");
            foreach (var key in advGroup.Keys)
                tagInputs.Attributes.Add(key, advGroup[key].ToString());

            #endregion group

            #region edit

            RouteValueDictionary advEdit = new RouteValueDictionary();
            if (adv.Keys.Contains("edit"))
                advEdit = new RouteValueDictionary(adv["edit"]);
            if (!advEdit.Keys.Contains("class"))
                advEdit.Add("class", " form-control");
            else
                advEdit["class"] = advEdit["class"] + " form-control";

            if ((advEdit.Keys.Contains("disabled")) && (advEdit["disabled"] is bool) && (!(bool)advEdit["disabled"]))
                advEdit.Remove("disabled");

            var dataTypeAttr = value.GetAttribute<DataTypeAttribute>();
            if (dataTypeAttr != null)
            {
                if (dataTypeAttr.DataType == DataType.Date)
                    advEdit["class"] = advEdit["class"] + " datepicker";
                if (dataTypeAttr.DataType == DataType.DateTime)
                    advEdit["class"] = advEdit["class"] + " datetimepicker";
            }
            else if (value.Body.Type == typeof(DateTime))
            {
                advEdit["class"] = advEdit["class"] + " datepicker";
            }

            #endregion edit

            //editor + button
            if ((!value.Body.Type.IsSealed) & (value.Body.Type.IsClass))
            {
                TagBuilder tagInputGroup = new TagBuilder("div");
                tagInputGroup.Attributes.Add("class", "input-group");

                //ID
                var propId = (value.Body as MemberExpression).Member.Name + "Id";
                var expr = Expression.Property((value.Body as MemberExpression).Expression, propId);
                var expression = Expression.Lambda<Func<TModel, Guid>>(expr, value.Parameters[0]);
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
                tagInputGroup.InnerHtml = helper.Hidden((value.Body as MemberExpression).Member.Name + "Id", metadata.Model, new { objectType = value.Body.Type.ToString() }).ToHtmlString();
                
                //TEXT
                expr = Expression.Property((value.Body as MemberExpression).Expression, (value.Body as MemberExpression).Member.Name);
                var attr = value.ReturnType.GetAttribute<DisplaybleAttribute>();
                expr = Expression.Property(expr, attr.Displayble);

                object modelValue = ModelMetadata.FromLambdaExpression(Expression.Lambda<Func<TModel, string>>(expr, value.Parameters[0]), helper.ViewData).Model;
                if (!advEdit.Keys.Contains("disabled"))
                    advEdit.Add("disabled", "");
                tagInputGroup.InnerHtml += helper.TextBox(
                    (value.Body as MemberExpression).Member.Name,
                    modelValue ?? "",
                    advEdit);
                
                //BUTTON
                TagBuilder span = new TagBuilder("span");
                span.Attributes.Add("class", "input-group-btn");
                RouteValueDictionary btn = new RouteValueDictionary();
                btn.Add("class", "btn btn-primary");
                btn.Add("onclick", "ASE.Select(this); return false;");
                if (advEdit.Keys.Contains("btndisabled"))
                    btn["class"] = btn["class"] + " disabled";

                span.InnerHtml = helper.Button("...", btn).ToHtmlString();
                tagInputGroup.InnerHtml += span.ToString();
                tagInputs.InnerHtml = tagInputGroup.ToString();
            }
            else
            {
                if (value.GetAttribute<LocalizedRequiredAttribute>() != null)
                    advEdit["class"] = advEdit["class"] + " required";
                advEdit["id"] = (value.Body as MemberExpression).Member.Name;
                advEdit["name"] = (value.Body as MemberExpression).Member.Name;
                tagInputs.InnerHtml = helper.EditorFor(value, "", (value.Body as MemberExpression).Member.Name, new { htmlAttributes = advEdit }).ToHtmlString();
            }

            if (validation != null)
                tagAll.InnerHtml += helper.ValidationMessage((value.Body as MemberExpression).Member.Name);

            tagAll.InnerHtml += tagInputs;

            return new MvcHtmlString(tagAll.ToString());
        }

        public static MvcHtmlString RowLabelEditorAndValidation<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property)
        {
            return helper.RowLabelEditorValidation(property, property, property);
        }

        public static MvcHtmlString RowLabelTextAreaAndValidation<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property)
        {
            return helper.RowLabelEditorValidation(property, property, property);
        }

        public static MvcHtmlString RowLabelEditorValidation<TModel, TValue, TProperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, TValue>> label,
           Expression<Func<TModel, TProperty>> valueValidation)
        {
            return helper.RowLabelEditorValidation(label, valueValidation, valueValidation);
        }

        public static MvcHtmlString RowLabelDropBoxValidation<TModel, TValue, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> label,
            Expression<Func<TModel, TProperty>> validation,
            string name,
            IEnumerable<SelectListItem> selectList)
        {
            string ret = "<div class=\"row\">";
            ret += "<div class=\"form-group\">";
            ret += helper.LabelFor(label, new { @class = "control-label col-md-2" });
            ret += "<div class=\"col-md-4\">";
            ret += helper.DropDownList(name, selectList, htmlAttributes: new { @class = "form-control" });
            ret += helper.ValidationMessageFor(validation, "", new { @class = "text-danger" });
            ret += "</div>";
            ret += "</div>";
            ret += "</div>";

            return new MvcHtmlString(ret);
        }

        public static MvcHtmlString RowLabelEditorValidation(this HtmlHelper helper,
            string label,
            string valueId)
        {
            string ret = "<div class=\"row\">";
            ret += "<div class=\"form-group\">";
            ret += helper.Label(label, new { @class = "control-label col-md-2" });
            ret += "<div class=\"col-md-4\">";

            ret += helper.Editor(valueId, new { htmlAttributes = new { @class = "form-control " } });
            ret += "</div>";
            ret += "</div>";
            ret += "</div>";

            return new MvcHtmlString(ret);
        }

        public static MvcHtmlString LabelEditorValidation(this HtmlHelper helper,
            string label,
            string valueId,
            object value,
            object htmlAttributes)
        {
            return LabelEditorValidation(helper, label, valueId, value, htmlAttributes, null);
        }

        public static MvcHtmlString LabelEditorValidation(this HtmlHelper helper,
            string label,
            string valueId,
            object value,
            object htmlAttributes,
            object htmlAttributesLabel)
        {
            IDictionary<string, object> hal = new RouteValueDictionary(htmlAttributesLabel);

            var key = hal.Keys.FirstOrDefault(x => x.ToLower() == "class");
            if (key == null)
                hal["class"] = "";
            hal["class"] = "padr10 " + hal["class"];

            IDictionary<string, object> ha = new RouteValueDictionary(htmlAttributes);

            key = ha.Keys.FirstOrDefault(x => x.ToLower() == "class");
            if (key == null)
                ha["class"] = "";
            ha["class"] = "form-control " + ha["class"];

            string ret = "";
            ret += "<div class=\"form-group\">";
            ret += helper.Label(label, hal);
            ret += helper.TextBox(valueId, value, ha);
            ret += "</div>";

            return new MvcHtmlString(ret);
        }

        public static MvcHtmlString IsReadonly<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property)
        {
            var spanBuilder = new TagBuilder("span");
            spanBuilder.Attributes["make-readonly"] = "true";
            spanBuilder.Attributes["for"] = helper.ViewData.TemplateInfo.GetFullHtmlFieldId(property);
            return new MvcHtmlString(spanBuilder.ToString());
        }
        public static string GetFullHtmlFieldName<TModel, TProperty>(this TemplateInfo templateInfo, Expression<Func<TModel, TProperty>> expression)
        {
            return templateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }

        public static string GetFullHtmlFieldId<TModel, TProperty>(this TemplateInfo templateInfo, Expression<Func<TModel, TProperty>> expression)
        {
            return templateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
        }

        public static string LabelEditorHSizeL = "col-sm-4";
        public static string LabelEditorHSizeIG = "col-sm-8";
        public static MvcHtmlString LabelEditorH<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property, object attrs)
        {
            return LabelEditorH(helper, property, LabelEditorHSizeL, LabelEditorHSizeIG, attrs);
        }

        public static MvcHtmlString LabelEditorH<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property)
        {
            return LabelEditorH(helper, property, LabelEditorHSizeL, LabelEditorHSizeIG, null);
        }

        public static MvcHtmlString LabelEditorH<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> property, string sizeL, string sizeIG, object attrs)
        {
            /*
            new {
                hide = true | false,
                disabled = true | false,
                all = new 
                    {
                        @class = "col-sm-4"
                        attr = "attr1"
                    },
                label = new 
                    {
                        label = "<label class="control-label">property</label>"
                        |
                        caption = "caption" //helper.Label(rvdLabel["caption"].ToString(), rvdLabel)
                        |
                        helper.LabelFor(property, label);
                    },
                group = new 
                    {
                        @class = "col-sm-8"
                    },
                editor = new 
                    {
                        
                    }
            }
            <div class="form-group col-sm-4" attr = "attr1">
                <label class="control-label"></label>
                <div class="input-group col-sm-8">
                </div>
            <div>             
            */

            RouteValueDictionary rvdAttrs = new RouteValueDictionary(attrs);
            if ((rvdAttrs["hide"] != null) && ((bool)rvdAttrs["hide"]))
                return new MvcHtmlString("");

            #region all

            TagBuilder tagAll = new TagBuilder("div");
            RouteValueDictionary rvdAll = new RouteValueDictionary(rvdAttrs["all"]);
            rvdAll["class"] = rvdAll["class"] + " form-group";
            foreach (var item in rvdAll.Keys)
                tagAll.Attributes.Add(item, rvdAll[item].ToString());

            #endregion all

            #region label

            RouteValueDictionary rvdLabel = new RouteValueDictionary(rvdAttrs["label"]);
            rvdLabel["class"] = rvdLabel["class"] + sizeL + " control-label";

            MvcHtmlString label = null;
            if (rvdLabel["label"] != null)
                label = new MvcHtmlString(rvdLabel["label"].ToString());
            if (rvdLabel["caption"] != null)
                label = helper.Label(rvdLabel["caption"].ToString(), rvdLabel);
            else
                label = helper.LabelFor(property, rvdLabel);
            
            #endregion label

            #region input-group

            RouteValueDictionary rvdGroup = new RouteValueDictionary(rvdAttrs["group"]);
            TagBuilder tagFormGroup = new TagBuilder("div");
            rvdGroup["class"] = rvdGroup["class"] + " " + sizeIG;
            foreach (var item in rvdGroup.Keys)
                tagFormGroup.Attributes.Add(item, rvdGroup[item].ToString());

            #endregion input-group

            RouteValueDictionary rvdEditor = new RouteValueDictionary(rvdAttrs["editor"]);
            rvdEditor["class"] = rvdEditor["class"] + " form-control";

            if ((!property.Body.Type.IsSealed) & (property.Body.Type.IsClass) & ((rvdEditor["editor"] == null) & rvdEditor["dropdown"] == null))
            {
                TagBuilder tagInputGroup = new TagBuilder("div");
                tagInputGroup.Attributes.Add("class", "input-group");
                //ID
                var propId = (property.Body as MemberExpression).Member.Name + "Id";
                var expr = Expression.Property((property.Body as MemberExpression).Expression, propId);
                var expression = Expression.Lambda<Func<TModel, Guid>>(expr, property.Parameters[0]);
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
                tagInputGroup.InnerHtml = helper.Hidden((property.Body as MemberExpression).Member.Name + "Id", metadata.Model, new { objectType = property.Body.Type.ToString() }).ToHtmlString();

                //TEXT
                expr = Expression.Property((property.Body as MemberExpression).Expression, (property.Body as MemberExpression).Member.Name);
                var attr = property.ReturnType.GetAttribute<DisplaybleAttribute>();
                expr = Expression.Property(expr, attr.Displayble);

                object modelValue = ModelMetadata.FromLambdaExpression(Expression.Lambda<Func<TModel, string>>(expr, property.Parameters[0]), helper.ViewData).Model;

                if (!rvdEditor.Keys.Contains("disabled"))
                    rvdEditor.Add("disabled", "");

                tagInputGroup.InnerHtml += helper.TextBox(
                    (property.Body as MemberExpression).Member.Name,
                    modelValue ?? "",
                    rvdEditor);

                //BUTTON
                TagBuilder span = new TagBuilder("span");
                span.Attributes.Add("class", "input-group-btn");
                RouteValueDictionary btn = new RouteValueDictionary();
                btn.Add("class", "btn btn-primary");
                btn.Add("onclick", "ASE.Select(this); return false;");
                if (rvdEditor.Keys.Contains("btndisabled"))
                    btn["class"] = btn["class"] + " disabled";

                span.InnerHtml = helper.Button("...", btn).ToHtmlString();
                tagInputGroup.InnerHtml += span.ToString();
                tagFormGroup.InnerHtml = tagInputGroup.ToString();
            }
            else
            {
                string editor = "";
                string format = "";
                if ((rvdEditor["format"] != null) && (rvdEditor["format"].ToString() == "Date"))
                {
                    rvdEditor["class"] = rvdEditor["class"] + " datepicker";
                    format = String.Format("{{0:{0}}}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                }
                if ((rvdEditor["format"] != null) && (rvdEditor["format"].ToString() == "DateTime"))
                {
                    rvdEditor["class"] = rvdEditor["class"] + " datetimepicker";
                    format = String.Format("{{0:{0} {1}}}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
                }

                if ((rvdEditor["readonly"] is bool) && ((bool)rvdEditor["readonly"] == false))
                    rvdEditor.Remove("readonly");

                if (rvdEditor["editor"] != null)
                    editor = rvdEditor["editor"].ToString();
                else if (rvdEditor["value"] != null)
                    editor = helper.TextBox(rvdEditor["name"].ToString(), rvdEditor["value"], format, rvdEditor).ToString();
                else
                {
                    var dataTypeAttr = property.GetAttribute<DataTypeAttribute>();
                    if (dataTypeAttr != null)
                    {
                        if (dataTypeAttr.DataType == DataType.Date)
                        {
                            rvdEditor["class"] = rvdEditor["class"] + " datepicker";
                            format = String.Format("{{0:{0}}}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                        }
                        if (dataTypeAttr.DataType == DataType.DateTime)
                        {
                            rvdEditor["class"] = rvdEditor["class"] + " datetimepicker";
                            format = String.Format("{{0:{0} {1}}}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
                        }
                    }
                    else if (property.Body.Type == typeof(DateTime))
                    {
                        rvdEditor["class"] = rvdEditor["class"] + " datepicker";
                        format = String.Format("{{0:{0}}}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                    }
                    if ((property.Body.Type == typeof(int)) | (property.Body.Type == typeof(long)))
                    {
                        if (rvdEditor["type"] == null)
                            rvdEditor["type"] = "number";
                    }
                    else if (rvdEditor["dropdown"] != null)
                    {
                        if (rvdEditor["dropdownEmpty"] == null)
                            editor = helper.DropDownListFor(property, (SelectList)rvdEditor["dropdown"], rvdEditor).ToHtmlString();
                        else
                            editor = helper.DropDownListFor(property, (SelectList)rvdEditor["dropdown"], rvdEditor["dropboxEmpty"].ToString(), rvdEditor).ToHtmlString();
                    }
                    else if (rvdEditor["textarea"] != null)
                    {
                        int rows = 10;
                        int cols = 10;
                        int.TryParse("" + rvdEditor["textarearows"], out rows);
                        int.TryParse("" + rvdEditor["textareacols"], out cols);
                        editor = helper.TextAreaFor(property, rows, cols, rvdEditor["htmlAttributes"]).ToHtmlString();
                    }
                    else
                        editor = helper.TextBoxFor(property, format, rvdEditor).ToHtmlString();
                }
                if ((rvdEditor["hide"] == null) || (!(bool)rvdEditor["hide"]))
                    tagFormGroup.InnerHtml = editor;
            }

            tagAll.InnerHtml = label.ToHtmlString();
            tagAll.InnerHtml += tagFormGroup.ToString();

            return new MvcHtmlString(tagAll.ToString());
        }
    }
}