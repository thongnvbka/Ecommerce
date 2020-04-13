using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Common.KnockoutBinder
{
    public class FromJsonAttribute : CustomModelBinderAttribute
    {
        private readonly static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public override IModelBinder GetBinder()
        {
            return new JsonModelBinder();
        }
        private class JsonModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllercontext, ModelBindingContext bindingcontext)
            {
                var stringified = controllercontext.HttpContext.Request[bindingcontext.ModelName];
                if (string.IsNullOrEmpty(stringified)) return null;
                return serializer.Deserialize(stringified, bindingcontext.ModelType);
            }
        }
    }
}
