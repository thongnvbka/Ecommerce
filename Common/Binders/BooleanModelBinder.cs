using System.Web.Mvc;

namespace Common.Binders
{
    public class BooleanModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null)
                return base.BindModel(controllerContext, bindingContext);

            if (string.IsNullOrWhiteSpace(value.AttemptedValue))
                return base.BindModel(controllerContext, bindingContext);

            bool rs;
            if (bool.TryParse(value.AttemptedValue, out rs))
            {
                return rs;
            }

            if (value.AttemptedValue == "1" || value.AttemptedValue == "True" 
                || value.AttemptedValue == "true" || value.AttemptedValue == "On" 
                || value.AttemptedValue == "on")
            {
                return true;
            }

            if (value.AttemptedValue == "0" || value.AttemptedValue == "False" 
                || value.AttemptedValue == "false" || value.AttemptedValue == "Off"
                || value.AttemptedValue == "off")
            {
                return false;
            }

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"\"{value.AttemptedValue}\" invalid boolean");

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}