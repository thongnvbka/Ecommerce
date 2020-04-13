using System.Globalization;
using System.Web.Mvc;

namespace ProjectV.LikeOrderThaiLan.com.Binders
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null)
                return base.BindModel(controllerContext, bindingContext);

            if (string.IsNullOrWhiteSpace(value.AttemptedValue))
                return base.BindModel(controllerContext, bindingContext);

            decimal result;
            // Attempt to read the culture cookie from Request
            //var cultureCookie = controllerContext.HttpContext.Request.Cookies["_culture"];

            //var cultureName = cultureCookie != null ? cultureCookie.Value : CultureHelper.GetDefaultCulture();

            if (decimal.TryParse(value.AttemptedValue, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out result))
            {
                return result;
            }

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"\"{value.AttemptedValue}\" invalid decimal");

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}