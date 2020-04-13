using Common.ActionResult;
using Common.Host;
using Library.Models;
using Library.UnitOfWork;
using Newtonsoft.Json;
using ProjectV.LikeOrderThaiLan.com.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectV.LikeOrderThaiLan.com.Controllers
{
    public class BaseController : Controller
    {
        public CustomerState CustomerState { get; set; }

        public CultureInfo CultureInfo = new CultureInfo("vi-VN", false);
        public UnitOfWork UnitOfWork { get; }

        public byte SystemId => byte.Parse(ConfigurationManager.AppSettings["SystemId"]);
        public string SystemName => ConfigurationManager.AppSettings["SystemName"];

        public BaseController()
        {
            UnitOfWork = new UnitOfWork();
            ViewData["CultureInfo"] = CultureInfo;
        }

        public decimal ExchangeRate()
        {
            return decimal.Parse(ConfigurationManager.AppSettings["CnyToVnd"]);
        }

        // GET: Base
        protected override void OnException(ExceptionContext filterContext)
        {
            //Log Exception e
            OutputLog.WriteOutputLog(filterContext.Exception);
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            // Grab the user's login information from Identity
            CustomerState customerState = new CustomerState();
            if (User is ClaimsPrincipal)
            {
                var user = User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                var customerStateString = GetClaim(claims, "customerStateString");
                //var name = GetClaim(claims, ClaimTypes.Name);
                //var id = GetClaim(claims, ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(customerStateString))
                    customerState.FromString(customerStateString);
            }
            CustomerState = customerState;

            ViewData["CustomerState"] = CustomerState;
            ViewData["ExchangeRate"] = ExchangeRate();
        }

        public static string GetClaim(List<Claim> claims, string key)
        {
            var claim = claims.FirstOrDefault(c => c.Type == key);
            if (claim == null)
                return null;

            return claim.Value;
        }

        /// <summary>
        /// Allow external initialization of this controller by explicitly
        /// passing in a request context
        /// </summary>
        /// <param name="requestContext"></param>
        public void InitializeForced(RequestContext requestContext)
        {
            Initialize(requestContext);
        }

        public ActionResult JsonCamelCaseResult(object obj, JsonRequestBehavior jsonAllwoBehavior)
        {
            return new JsonCamelCaseResult(obj, jsonAllwoBehavior);
        }

        public string GetCatetgoryJsTree()
        {
            var offices = UnitOfWork.CategoryRepo.Entities.Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToList();

            var listJsTree = offices.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId == 0 ? "#" : o.ParentId.ToString(),
                idPath = o.IdPath,
            });

            return JsonConvert.SerializeObject(listJsTree);
        }

        public string GetCatetgoryDepositJsTree()
        {
            var offices = UnitOfWork.CategoryRepo.Entities.Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToList();

            var list = new List<dynamic>();

            list.Add(new
            {
                id = -1,
                text = "เลือกประเภทสินค้า",
                parent = "#",
                idPath = 0,
            });

            foreach(var item in offices)
            {
                list.Add(new
                {
                    id = item.Id.ToString(),
                    text = item.Name,
                    parent = item.ParentId == 0 ? "#" : item.ParentId.ToString(),
                    idPath = item.IdPath,
                });
            }
            
            return JsonConvert.SerializeObject(list);
        }

        public string GetWardJsTree()
        {
            var list = UnitOfWork.WarehouseRepo.Entities.ToList();
           
            var listJsTree = list.Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.Id.ToString()
            });

            return JsonConvert.SerializeObject(listJsTree);
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            var cultureName = RouteData.Values["culture"] as string;

            if (Request.Url != null)
            {
                var array = Request.Url.Segments;

                if (array.Length > 1)
                {
                    cultureName = array[1].Replace("/", "");
                }

                // Attempt to read the culture cookie from Request
                if (string.IsNullOrWhiteSpace(cultureName))
                {
                    cultureName = "th";
                }
                    //cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

                cultureName = CultureHelper.GetNeutralCulture(cultureName);

                RouteData.Values["culture"] = cultureName.ToLowerInvariant();
            }

            // Modify current thread's cultures
            // ReSharper disable once AssignNullToNotNullAttribute
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }
    }
}