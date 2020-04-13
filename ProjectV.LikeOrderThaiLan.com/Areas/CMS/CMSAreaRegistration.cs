using System.Web.Mvc;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS
{
    public class CMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Cms";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        { 
            context.MapRoute(
                "CMS",
                "{culture}/Cms/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new { culture = @"th|vi" },
                new[] { "ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers" }
                );
        }
    }
}