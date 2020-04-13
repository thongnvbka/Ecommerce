using System;
using System.Globalization;
using System.Web.Mvc;

namespace Common.Binders
{
    public class DateModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null) 
                return base.BindModel(controllerContext, bindingContext);

            if (string.IsNullOrWhiteSpace(value.AttemptedValue))
                return base.BindModel(controllerContext, bindingContext);

            DateTime date;
            // Attempt to read the culture cookie from Request
            //var cultureCookie = controllerContext.HttpContext.Request.Cookies["_culture"];

            //var cultureName = cultureCookie != null ? cultureCookie.Value : CultureHelper.GetDefaultCulture();

            if (DateTime.TryParse(value.AttemptedValue, CultureInfo.CreateSpecificCulture("vi-VN"), DateTimeStyles.None,
                out date))
            {
                return date;
            }

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"\"{value.AttemptedValue}\" invalid date");

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}