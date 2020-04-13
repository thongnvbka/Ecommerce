using System.Web.Mvc;
using System.Web.Routing;

namespace Cms
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("elmah.axd");

            routes.MapMvcAttributeRoutes();

            //routes.MapRoute(
            //    name: "User-add",
            //    url: "them-moi",
            //    defaults: new { controller = "Home", action = "Add", id = UrlParameter.Optional }
            //);

            // Resize Image
            routes.MapRoute(
                "Images_resize", "upload/resize/{id}_{width}x{height}_{thumType}",
                new {controller = "Upload", action = "ResizeThumbnail"}
            );

            routes.MapRoute(
               "Images", "images/{id}_{width}x{height}_{thumType}",
               new { controller = "Upload", action = "ResizeThumbnail" }
           );

            // Resize Image
            routes.MapRoute(
               "Images_jpg", "images/{id}_{width}x{height}_{thumType}.jpg",
               new { controller = "Upload", action = "ResizeThumbnail" }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
