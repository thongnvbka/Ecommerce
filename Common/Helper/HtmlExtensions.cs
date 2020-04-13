using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Common.Helper
{
    public class SelectItem : SelectListItem
    {
        public string CssClass { get; set; }
    }

    public enum TooltipPlace
    {
        Top,
        Bottom
    }

    public static class HtmlExtensions
    {
        /* public static IEnumerable<SelectListItem> SelectList(params TEnum[] values)
         {


             foreach (Type t in values)
             {
                 var item = new SelectListItem { Value = t.ToString(), Text = Enum.GetName(t, t) };
             }
         }*/

        /* public static IEnumerable<SelectListItem> ToSelectList<TEnum>(params Type[] fillerValues)
         {
             /*return (Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(
                 enu => new SelectListItem() { Text = Enum.GetName(typeof(TEnum), enu), Value = enu.ToString() })).ToList();#1#

             foreach (Type _fillerValue in fillerValues)
             {

             }

             foreach (var v in Enum.GetValues(typeof(TEnum)))
             {


             }


             return (from int value in Enum.GetValues(typeof(TEnum)) select new SelectListItem { Value = value.ToString(), Text = Enum.GetName(typeof(TEnum), value) }).ToList();


         }*/

        public static IEnumerable<SelectListItem> ToSelectList<TEnum>()
        {
            /*return (Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(
                enu => new SelectListItem() { Text = Enum.GetName(typeof(TEnum), enu), Value = enu.ToString() })).ToList();*/

            return (from int value in System.Enum.GetValues(typeof (TEnum))
                select new SelectListItem {Value = value.ToString(CultureInfo.InvariantCulture), Text = System.Enum.GetName(typeof (TEnum), value)}).ToList
                ();
        }

        public static SelectList ToSelectList<TEnum>(object valuesSelected)
        {
            return new SelectList(ToSelectList<TEnum>(), "Value", "Text", valuesSelected);
        }

        #region Enums DropdownList

        public static MvcHtmlString DropDownList(this HtmlHelper helper, string name, Type type)
        {
            return DropDownList(helper, name, type, null);
        }


        public static MvcHtmlString DropDownList(this HtmlHelper helper, string name, Type type, object selected)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Type is not an enum.");

            if (selected != null && selected.GetType() != type)
                throw new ArgumentException("Selected object is not " + type);

            var enums = new List<SelectListItem>();
            foreach (int value in System.Enum.GetValues(type))
            {
                var item = new SelectListItem
                {
                    Value = value.ToString(CultureInfo.InvariantCulture),
                    // ReSharper disable once PossibleNullReferenceException
                    Text = System.Enum.GetName(type, value).Replace("_", " ")
                };

                if (selected != null)
                    item.Selected = (int) selected == value;

                enums.Add(item);
            }

            return helper.DropDownList(name, enums);
        }
        #endregion

        #region ToSelectList

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified items for data value field.
        /// </summary>
        /// <param name="enumerable">The items.</param>
        /// <param name="value">The data value field.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value)
        {
            return enumerable.ToSelectList(value, value, null);
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified items for data value field and a selected value.
        /// </summary>
        /// <param name="enumerable">The items.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="selectedValue">The selected value.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value,
            object selectedValue)
        {
            return enumerable.ToSelectList(value, value, selectedValue);
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified items for data value field and the data text field.
        /// </summary>
        /// <param name="enumerable">The items.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value,
            Func<T, string> text)
        {
            return enumerable.ToSelectList(value, text, null);
        }

        /// <summary>
        /// Returns an IEnumerable&lt;SelectListItem&gt; by using the specified items for data value field, the data text field, and a selected value.
        /// </summary>
        /// <param name="enumerable">The items.</param>
        /// <param name="value">The data value field.</param>
        /// <param name="text">The data text field.</param>
        /// <param name="selectedValue">The selected value.</param>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> value,
            Func<T, string> text, object selectedValue)
        {
            return enumerable.Select(f => new SelectListItem
            {
                Value = value(f),
                Text = text(f),
                Selected = value(f).Equals(selectedValue)
                // String.Equals(value(f), selectedValue.ToString(), StringComparison.CurrentCultureIgnoreCase)
            });
        }

        #endregion

        public static IHtmlString CheckBox(this HtmlHelper helper, string name, bool isChecked, string label,
            object htmlAttributes)
        {
            var check = new TagBuilder("input");
            if (isChecked)
                check.MergeAttribute("checked", "checked");
            check.MergeAttribute("id", name);
            check.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            check.MergeAttribute("name", name);
            check.MergeAttribute("type", "checkbox");
            var labelTag = new TagBuilder("label");
            labelTag.MergeAttribute("for", name);

            return
                MvcHtmlString.Create(string.Format("{0} {1}", check.ToString(TagRenderMode.SelfClosing),
                    labelTag.ToString(TagRenderMode.SelfClosing)));
        }

        #region CheckBoxGroup

        public static MvcHtmlString CheckBoxGroup(this HtmlHelper helper, string name, IEnumerable<SelectItem> values,
            object htmlAttributes)
        {
            return CheckBoxGroup(helper, name, values, null, htmlAttributes);
        }


        public static MvcHtmlString CheckBoxGroup(this HtmlHelper helper, string name, IEnumerable<SelectItem> values,
            object valuesSelected, object htmlAttributes)
        {
            List<object> selected = null;

            if (valuesSelected != null)
            {
                selected = new List<object> {valuesSelected};
            }

            return CheckBoxGroup(helper, name, values, selected, htmlAttributes);
        }

        public static MvcHtmlString CheckBoxGroup(this HtmlHelper helper, string name, IEnumerable<SelectItem> values,
            IEnumerable<object> valuesSelected, object htmlAttributes)
        {
            var sb = new StringBuilder();

            if (values != null)
            {
                // Create a radio button for each item in the list
                foreach (var item in values)
                {
                    // Generate an id to be given to the radio button field
                    var id =
                        string.Format("{0}_{1}", name,
                            !string.IsNullOrWhiteSpace(item.Value) ? item.Value : DateTime.UtcNow.Ticks.ToString())
                            .ToLower();

                    var selected = valuesSelected != null &&
                                   // ReSharper disable once PossibleMultipleEnumeration
                                    valuesSelected.Any(x => x.ToString().ToLower() == item.Value.ToLower());

                    var checkbox = string.Format(
                        "<input type=\"checkbox\" name=\"{0}\" value=\"{1}\" id=\"{2}\"{3} />", name,
                        HttpUtility.HtmlEncode(item.Value), id, selected ? " checked=\"checked\"" : string.Empty);
                    // helper.CheckBox(name, _selected, new {id = _id, value = item.Value}).ToHtmlString();

                    var label = helper.Label(id, item.Text).ToHtmlString();

                    sb.AppendFormat("<li class=\"{2}\">{0}{1}</li>", checkbox, label, item.CssClass);
                }
            }

            var tag = new TagBuilder("ul");
            tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            tag.InnerHtml = sb.ToString();
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        #endregion

        #region RadioButtonGroup

        public static MvcHtmlString RadioButtonGroup(this HtmlHelper helper, string name, IEnumerable<SelectItem> values,
            object htmlAttributes)
        {
            return RadioButtonGroup(helper, name, values, null, htmlAttributes);
        }

        public static MvcHtmlString RadioButtonGroup(this HtmlHelper helper, string name, IEnumerable<SelectItem> values,
            object valueSelected, object htmlAttributes)
        {
            var sb = new StringBuilder();

            if (values != null)
            {
                // Create a radio button for each item in the list
                foreach (var item in values)
                {
                    // Generate an id to be given to the radio button field
                    var id =
                        string.Format("{0}_{1}", name,
                            !string.IsNullOrWhiteSpace(item.Value) ? item.Value : DateTime.UtcNow.Ticks.ToString())
                            .ToLower();

                    var radioButton =
                        helper.RadioButton(name, item.Value,
                            (valueSelected != null &&
                             string.Compare(item.Value, valueSelected.ToString(),
                                 StringComparison.OrdinalIgnoreCase) == 0), new {id}).ToHtmlString();

                    var label = helper.Label(id, HttpUtility.HtmlEncode(item.Text)).ToHtmlString();

                    sb.AppendFormat(
                        "<li class=\"{2}\">{0}{1}</li>", radioButton, label, item.CssClass);
                }
            }

            var tag = new TagBuilder("ul");
            tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            tag.InnerHtml = sb.ToString();
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }


        public static MvcHtmlString RadioButtonGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> listOfValues)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                // Create a radio button for each item in the list
                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field


                    var id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

                    // Create and populate a radio button using the existing html helpers
                    var label = htmlHelper.Label(id, item.Text);
                    var radio = htmlHelper.RadioButtonFor(expression, item.Value, new {id}).ToHtmlString();

                    // Create the html string that will be returned to the client
                    // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" /><label for="TestRadio_1">Line1</label>
                    sb.AppendFormat("<div class=\"radio-button-group\">{0}{1}</div>", radio, label);
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        #endregion

        #region Link & Image Link

        public static MvcHtmlString ImageLink(this HtmlHelper helper, string href, object hrefAttributes,
            string imageSrc, object imageAttributes)
        {
            var imgHtml = Image(helper, imageSrc, imageAttributes).ToHtmlString();
            var tag = new TagBuilder("a");
            tag.MergeAttributes(new RouteValueDictionary(hrefAttributes));
            tag.MergeAttribute("href", href, true);
            tag.InnerHtml = imgHtml;
            string html = tag.ToString(TagRenderMode.Normal);
            return MvcHtmlString.Create(html);
        }


        public static MvcHtmlString Image(this HtmlHelper helper, string src, object attributes)
        {
            var tag = new TagBuilder("img");
            tag.MergeAttributes(new RouteValueDictionary(attributes));
            tag.MergeAttribute("src", src, true);
            string imgHtml = tag.ToString(TagRenderMode.SelfClosing);
            return MvcHtmlString.Create(imgHtml);
        }


        public static MvcHtmlString Link(this HtmlHelper helper, string href, string text, object attributes)
        {
            var tag = new TagBuilder("a");
            tag.MergeAttributes(new RouteValueDictionary(attributes));
            tag.MergeAttribute("href", href, true);
            tag.InnerHtml = text;
            string html = tag.ToString(TagRenderMode.Normal);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, string href, string text)
        {
            return Link(helper, href, text, new {@title = text});
        }

        #endregion

        public static IHtmlString Title(this HtmlHelper helper, string val)
        {
            return MvcHtmlString.Create(string.Format("<title>{0}</title>", helper.Encode(val)));
        }

        #region Google Script Track

        public static MvcHtmlString GoogleAnalyticsTrackingScript(this HtmlHelper helper, string accountId)
        {
            var html = new StringBuilder("<script type=\"text/javascript\">");
            html.Append(Environment.NewLine);
            html.Append("var _gaq = _gaq || [];");
            html.Append(Environment.NewLine);
            html.Append(string.Format("_gaq.push(['_setAccount', '{0}']);", accountId));
            html.Append(Environment.NewLine);
            html.Append("_gaq.push(['_trackPageview']);");
            html.Append(Environment.NewLine);
            html.Append("(function() {");
            html.Append(Environment.NewLine);
            html.Append("var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;");
            html.Append(Environment.NewLine);
            html.Append("ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') ");
            html.Append(Environment.NewLine);
            html.Append("+ '.google-analytics.com/ga.js';");
            html.Append(Environment.NewLine);
            html.Append("var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);");
            html.Append(Environment.NewLine);
            html.Append("})();");
            html.Append(Environment.NewLine);
            html.Append("</script>");
            return MvcHtmlString.Create(html.ToString());
        }

        #endregion

        #region Label Tooltip For

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, null, TooltipPlace.Top, labelText: null, isRequire: isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, TooltipPlace placement, object htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, null, placement, null, htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, TooltipPlace placement, IDictionary<string, object> htmlAttributes,
        //    bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, null, placement, null, htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, object htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, null, TooltipPlace.Top, htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, null, TooltipPlace.Top, htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, null, TooltipPlace.Top, labelText, htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string labelText, IDictionary<string, object> htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, null, TooltipPlace.Top, labelText, htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string title, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, title, TooltipPlace.Top, labelText: null, isRequire: isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, TooltipPlace placement, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, null, placement, labelText: null, isRequire: isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string title, TooltipPlace placement, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, title, placement, labelText: null, isRequire: isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string title, TooltipPlace placement, string labelText, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, title, placement, labelText, null, isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string title, TooltipPlace placement, object htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, title, placement, null, htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string title, TooltipPlace placement,
        //    IDictionary<string, object> htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, title, placement, null, htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string title, TooltipPlace placement, string labelText,
        //    object htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelFor(html, expression, title, placement, labelText,
        //        HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), isRequire);
        //}

        //public static MvcHtmlString ProviewLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string title, TooltipPlace placement, string labelText,
        //    IDictionary<string, object> htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelHelper(html,
        //        expression,
        //        title, placement,
        //        labelText,
        //        htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabel(this HtmlHelper html, string id, string labelText, bool? isRequire = null)
        //{
        //    return ProviewLabel(html, id, labelText, title: null, isRequire: isRequire);
        //}

        //public static MvcHtmlString ProviewLabel(this HtmlHelper html, string id, string labelText, string title, bool? isRequire = null)
        //{
        //    return ProviewLabel(html, id, labelText, title, TooltipPlace.Top, isRequire);
        //}

        //public static MvcHtmlString ProviewLabel(this HtmlHelper html, string id, string labelText, string title,
        //    TooltipPlace placement, bool? isRequire = null)
        //{
        //    return ProviewLabel(html, id, labelText, title, placement, null, isRequire);
        //}

        //public static MvcHtmlString ProviewLabel(this HtmlHelper html, string id, string labelText, string title,
        //    TooltipPlace placement, object htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabel(html, id, labelText, title, placement,
        //        HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), isRequire);
        //}

        //public static MvcHtmlString ProviewLabel(this HtmlHelper html, string id, string labelText,
        //    object htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabel(html, id, labelText, null, TooltipPlace.Top,
        //        HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), isRequire);
        //}

        //public static MvcHtmlString ProviewLabel(this HtmlHelper html, string id, string labelText,
        //    IDictionary<string, object> htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabel(html, id, labelText, null, TooltipPlace.Top, htmlAttributes, isRequire);
        //}

        //public static MvcHtmlString ProviewLabel(this HtmlHelper html, string id, string labelText, string title,
        //    TooltipPlace placement, IDictionary<string, object> htmlAttributes, bool? isRequire = null)
        //{
        //    return ProviewLabelHelper(html,
        //        id, labelText,
        //        title, placement,
        //        htmlAttributes,
        //        isRequire);
        //}

        //private static MvcHtmlString RenderLabel(HtmlHelper html, string className,
        //    string resolvedLabelText, string htmlFieldName, string title, TooltipPlace placement, bool isRequire,
        //    IDictionary<string, object> htmlAttributes = null)
        //{
        //    var labelTag = new TagBuilder("label");
        //    labelTag.Attributes.Add("for",
        //        TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
        //    labelTag.MergeAttributes(htmlAttributes, true);

        //    string idTooltip;
        //    var pageId = html.ViewContext.Controller.ViewBag.PageId;
        //    if (pageId == null)
        //    {
        //        idTooltip = GenTooltipIdWithControllerAndActionName(html, className, htmlFieldName);
        //    }
        //    else
        //    {
        //        // ReSharper disable once PossibleNullReferenceException
        //        var currentUser = html.ViewContext.Controller.ControllerContext.HttpContext.Session["User"] == null
        //            ? null
        //            : (BriefUser) html.ViewContext.Controller.ControllerContext.HttpContext.Session["User"];
        //        if (currentUser == null)
        //        {
        //            idTooltip = GenTooltipIdWithControllerAndActionName(html, className, htmlFieldName);
        //        }
        //        else
        //        {
        //            var page = currentUser.Pages.FirstOrDefault(item => item.PageId == pageId);
        //            var moduleId = page != null ? page.ModuleId : 0;
        //            idTooltip = string.IsNullOrWhiteSpace(className)
        //                ? string.Format("{0}_{1}_{2}", htmlFieldName.Replace('.', '_'), moduleId,
        //                    pageId)
        //                : string.Format("{0}_{1}_{2}_{3}", className, htmlFieldName.Replace('.', '_'), moduleId,
        //                    pageId);
        //        }
        //    }
        //    var iTag = new TagBuilder("i");
        //    if (!string.IsNullOrWhiteSpace(title))
        //    {
        //        iTag.Attributes.Add("title", title);
        //    }
        //    iTag.Attributes.Add("data-placement", placement == TooltipPlace.Top ? "top" : "bot");
        //    iTag.Attributes.Add("data-toggle", "tooltip");
        //    iTag.Attributes.Add("class", "fa fa-question-circle color-blue tooltips show-help-text");
        //    iTag.Attributes.Add("data-id", idTooltip);

        //    if (isRequire)
        //    {
        //        var spanTag = new TagBuilder("span");
        //        spanTag.Attributes.Add("class", "color-red");
        //        spanTag.SetInnerText("*");

        //        labelTag.InnerHtml = string.Format("{0} {1} {2}", resolvedLabelText,
        //            spanTag.ToString(TagRenderMode.Normal), iTag.ToString(TagRenderMode.Normal));
        //    }
        //    else
        //    {
        //        labelTag.InnerHtml = string.Format("{0} {1}", resolvedLabelText, iTag.ToString(TagRenderMode.Normal));
        //    }

        //    return new MvcHtmlString(labelTag.ToString(TagRenderMode.Normal));
        //}

        //private static MvcHtmlString ProviewLabelHelper<TModel, TValue>(HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression, string title, TooltipPlace placement, string labelText = null,
        //    IDictionary<string, object> htmlAttributes = null, bool? isRequire = null)
        //{
        //    var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
        //    var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
        //    var className = typeof (TModel).Name;

        //    var resolvedLabelText = labelText ??
        //                            metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();

        //    if (!isRequire.HasValue)
        //    {
        //        Func<string, ModelMetadata, IEnumerable<ModelClientValidationRule>> clientValidationRuleFactory =
        //        (name, metaData) => ModelValidatorProviders.Providers.GetValidators(
        //                                metaData ??
        //                                ModelMetadata.FromStringExpression(name, html.ViewData), html.ViewContext)
        //                                    .SelectMany(v => v.GetClientValidationRules());
        //        var clientRules = clientValidationRuleFactory(htmlFieldName, metadata);
        //        isRequire = clientRules.Any(rule => rule.ValidationType.Equals("required"));
        //    }

        //    return RenderLabel(html, className, resolvedLabelText, htmlFieldName, title, placement,
        //        isRequire.Value, htmlAttributes);
        //}

        //private static MvcHtmlString ProviewLabelHelper(HtmlHelper html, string id, string labelText, string title,
        //    TooltipPlace placement, IDictionary<string, object> htmlAttributes = null, bool? isRequire = null)
        //{
        //    if (!isRequire.HasValue)
        //    {
        //        isRequire = false;
        //    }
        //    return RenderLabel(html, null, labelText, id, title, placement, isRequire.Value, htmlAttributes);
        //}

        private static string GenTooltipIdWithControllerAndActionName(HtmlHelper html, string className,
            string htmlFieldName)
        {
            var actionName = html.ViewContext.Controller.ControllerContext.RouteData.Values["action"].ToString();
            var controllerName = html.ViewContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();
            return string.IsNullOrWhiteSpace(className)
                ? string.Format("{0}_{1}_{2}", htmlFieldName.Replace('.', '_'), controllerName,
                    actionName)
                : string.Format("{0}_{1}_{2}_{3}", className, htmlFieldName.Replace('.', '_'), controllerName,
                    actionName);
        }

        #endregion
    }
}