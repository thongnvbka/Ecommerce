using System;
using System.Web.Mvc;

namespace ProjectV.LikeOrderThaiLan.com.Binders
{
    public class DoubleArrayBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (string.IsNullOrWhiteSpace(result.AttemptedValue)) return null;

            var words = result.AttemptedValue.Trim().Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var ints = new double[words.Length];
            for (var i = 0; i < words.Length; i++)
            {
                ints[i] = double.Parse(words[i].Trim());
            }

            return ints;
        }
    }
}
