using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Library.AutoMappers;

namespace ProjectV.LikeOrderThaiLan.com
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            // Đăng ký custom binding
            ModelBinderConfig.Register(ModelBinders.Binders);

            // Đăng ký https
            GlobalFilters.Filters.Add(new RequireHttpsAttribute());

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Đăng ký mapper
            AutoMapperConfig.RegisterMappings();
        }
    }
}
