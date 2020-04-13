using System.Web.Optimization;

namespace ProjectV.LikeOrderThaiLan.com
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/jquery").Include(
                        "~/Scripts/jquery-1.12.4.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Areas/CMS/Scripts/globinfo/globalize.js",
                        "~/Areas/CMS/Scripts/globinfo/globalize.culture.vi-VN.js",
                        "~/Areas/CMS/Scripts/jquery.redirect.js"
                        ));  
            bundles.Add(new StyleBundle("~/css").Include(
                      "~/Content/css/css-login/header_footer.css",
                      "~/Content/css/css-login/reset.css",
                      "~/Content/css/css-home/user.css",
                      "~/Content/css/styles.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/css/css-home/cart.css",
                      "~/Areas/CMS/css/toastr.min.css",
                      "~/Content/custom.css"
                      ));

            //Home
            bundles.Add(new ScriptBundle("~/home").Include(
                       "~/Scripts/jquery.cookie.js",
                       "~/Scripts/custom/home.js"
                       ));

            BundleTable.EnableOptimizations = false;
        }
    }
}