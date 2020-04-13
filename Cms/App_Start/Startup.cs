using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using Cms.Attributes;
using Cms.Jobs;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Cms
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // Hangfire job config
            GlobalConfiguration.Configuration.UseSqlServerStorage("FinGroupContext",
                new SqlServerStorageOptions {QueuePollInterval = TimeSpan.FromMinutes(1)});

            //this call placement is important
            var options = new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            };
            app.UseHangfireDashboard("/hangfire", options);
            app.UseHangfireServer();

            // Quan ly Jobs
            //var manager = new RecurringJobManager();
            //manager.AddOrUpdate("notify_order_package", Job.FromExpression(() => Method()), Cron.Daily(12, 20));
            //RecurringJob.AddOrUpdate(() => Console.Write("Easy!"), Cron.Daily(12, 20));

            // Thông báo đặt hàng package quá 3 ngày Not entered kho
            RecurringJob.RemoveIfExists("OrderWarningByEmail");
            RecurringJob.AddOrUpdate("OrderWarningByEmail", () => OrderJob.OrderWarningByEmail(3), Cron.Daily(1, 0));

            //// Thông báo đặt hàng Orders quá 3 ngày chưa có mã vận đơn
            //RecurringJob.AddOrUpdate("order_no_code_three_days", () => OrderJob.OrderNoCodeOfLadingOverDays(3), Cron.Daily(17, 14));

            //// Thông báo đặt hàng Orders quá 4 ngày chưa đủ kiện về kho
            //RecurringJob.AddOrUpdate("order_not_enough_inventory_four_days", () => OrderJob.OrderNotEnoughInventoryOverDays(4), Cron.Daily(17, 14));


            // Thông báo đặt hàng package quá 3 ngày Not entered kho
            RecurringJob.RemoveIfExists("UpdateOrderToComplete");
            RecurringJob.AddOrUpdate("UpdateOrderToComplete", () => OrderJob.UpdateOrderToComplete(2), Cron.Daily(2, 0));

            app.MapSignalR();
        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie, 
                LoginPath = new PathString("/Account/LogOn"),
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromHours(1),
                Provider = new CookieAuthenticationProvider
                {
                    OnResponseSignIn = ctx =>
                    {
                        //if (ctx.Properties.IsPersistent)
                        //    return;

                        // Tạo thời gian timeout cho coocki
                        //var ticks = ctx.Options.SystemClock.UtcNow.AddHours(1).UtcTicks;
                        //ctx.Properties.Dictionary.Add("absolute", ticks.ToString());
                    },
                    OnValidateIdentity = ctx =>
                    {
                        if(ctx.Properties.IsPersistent)
                            return Task.FromResult(0);

                        //var reject = true;
                        //string value;
                        //if (ctx.Properties.Dictionary.TryGetValue("absolute", out value))
                        //{
                        //    long ticks;
                        //    if (Int64.TryParse(value, out ticks))
                        //    {
                        //        reject = ctx.Options.SystemClock.UtcNow.UtcTicks > ticks;
                        //    }
                        //}

                        //if (reject)
                        //{
                        //    ctx.RejectIdentity();
                        //    // optionally clear cookie
                        //    //ctx.OwinContext.Authentication.SignOut(ctx.Options.AuthenticationType);
                        //}

                        return  Task.FromResult(0);
                    },

                    OnApplyRedirect = ctx =>
                    {
                        if (!IsAjaxRequest(ctx.Request))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                }
            });

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }

        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }
            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }
    }
}