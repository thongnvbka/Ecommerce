using ProjectV.LikeOrderThaiLan.com.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectV.LikeOrderThaiLan.com
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                "Account_Active",
                "{culture}/{controller}/{action}-{url}",
                new { culture = CultureHelper.GetDefaultCulture(), controller = "Account", action = "Active" },
                new[] { "ProjectV.LikeOrderThaiLan.com.Controllers" }
            );

            routes.MapRoute(
             "Account_NewPass",
             "{culture}/{controller}/{action}-{url}",
             new { culture = CultureHelper.GetDefaultCulture(), controller = "Account", action = "NewPass" },
             new[] { "ProjectV.LikeOrderThaiLan.com.Controllers" }
         );

            routes.MapRoute(
               "Account_PassForget",
               "{culture}/{controller}/{action}-{url}",
               new { culture = CultureHelper.GetDefaultCulture(), controller = "Account", action = "PassForget" },
               new[] { "ProjectV.LikeOrderThaiLan.com.Controllers" }
           );

            routes.MapRoute(
                "Default_culture",
                "{culture}/{controller}/{action}/{id}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    culture = UrlParameter.Optional,
                    id = UrlParameter.Optional
                },
                new { culture = @"th|vi" },
                new[] {"ProjectV.LikeOrderThaiLan.com.Controllers"}
                );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] {"ProjectV.LikeOrderThaiLan.com.Controllers"}
            );
        }
    }
}