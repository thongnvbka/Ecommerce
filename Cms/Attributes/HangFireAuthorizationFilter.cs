using System.Web;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Cms.Attributes
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.Identity.Name == "admin";
        }
    }
}