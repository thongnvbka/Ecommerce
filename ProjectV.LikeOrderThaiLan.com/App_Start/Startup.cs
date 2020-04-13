using System;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using ProjectV.LikeOrderThaiLan.com.Helpers;

namespace ProjectV.LikeOrderThaiLan.com
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            var provider = new CookieAuthenticationProvider();

            var originalHandler = provider.OnApplyRedirect;

            //Our logic to dynamically modify the path (maybe needs some fine tuning)
            provider.OnApplyRedirect = context =>
            {
                var mvcContext = new HttpContextWrapper(HttpContext.Current);
                var routeData = RouteTable.Routes.GetRouteData(mvcContext);

                // Attempt to read the culture cookie from Request
                string cultureName;
                if (string.IsNullOrWhiteSpace(routeData?.Values["culture"] as string))
                {
                    cultureName = "vi";
                }
                else
                {
                    cultureName = (string) routeData.Values["culture"];
                }

                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

                cultureName = CultureHelper.GetNeutralCulture(cultureName);

                //Get the current language  
                var routeValues = new RouteValueDictionary {{"culture", cultureName}};

                //Reuse the RetrunUrl
                var uri = new Uri(context.RedirectUri);
                var returnUrl = HttpUtility.ParseQueryString(uri.Query)[context.Options.ReturnUrlParameter];
                routeValues.Add(context.Options.ReturnUrlParameter, returnUrl);

                //Overwrite the redirection uri
                context.RedirectUri = url.Action("login", "account", routeValues);
                originalHandler.Invoke(context);
            };

            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(url.Action("login", "account")),
                //Set the Provider
                Provider = provider
            });

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}