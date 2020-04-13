using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Library.AutoMappers;
using Library.Models;

namespace Cms
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ModelBinderConfig.Register(ModelBinders.Binders);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Đăng ký mapper
            AutoMapperConfig.RegisterMappings();
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            UserState userState = new UserState();
            if (User is ClaimsPrincipal)
            {
                var user = User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                var userStateString = GetClaim(claims, "userState");

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(userState.Culture ?? "vi-VN");
            //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(userState.Culture ?? "vi-VN");
        }

        public static string GetClaim(List<Claim> claims, string key)
        {
            var claim = claims.FirstOrDefault(c => c.Type == key);
            if (claim == null)
                return null;

            return claim.Value;
        }
    }
}
