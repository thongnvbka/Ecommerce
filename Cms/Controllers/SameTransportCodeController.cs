using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using System.Linq;
using Common.Helper;
using Common.Emums;
using Cms.Attributes;

namespace Cms.Controllers
{
    public class SameTransportCodeController : BaseController
    {
        // GET: PackageNoCode
        [LogTracker(EnumAction.View, EnumPage.SameTransportCode)]
        public ActionResult Index()
        {
            //var jsonSerializerSettings = new JsonSerializerSettings
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};

            //ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
            //    .Cast<OrderPackageStatus>()
            //    .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
            //    .ToList(), jsonSerializerSettings);

            //ViewBag.OrderTypes = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
            //   .Cast<OrderType>()
            //   .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderType>((int)v) })
            //   .ToList(), jsonSerializerSettings);

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.SameTransportCode)]
        public async Task<ActionResult> Search(byte? status, DateTime? fromDate, DateTime? toDate, 
            byte? orderType, string keyword = "", int currentPage = 1, int recordPerPage = 20, byte mode = 0)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            long totalRecord;

            var transportCodes = await UnitOfWork.OrderPackageRepo.GetPackageSameCode(out totalRecord, status, fromDate, toDate,
            orderType, keyword, currentPage, recordPerPage);

            var transportCodesStr = $";{string.Join(";", transportCodes)};";

            var packages = await UnitOfWork.OrderPackageRepo.GetPackageSameCode(transportCodesStr);

            // Object Javascript: Trạng thái package
            var packageStatus = Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderStatus>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v));

            return JsonCamelCaseResult(new {packages, transportCodes, packageStatus, totalRecord}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.SameTransportCode)]
        public async Task<ActionResult> UpdateStatus(string transportCode, byte status)
        {
            var packages =
                await UnitOfWork.OrderPackageRepo.FindAsync(
                    x => x.IsDelete == false && x.OrderId > 0 && x.Mode != null && x.TransportCode == transportCode);

            if (!packages.Any())
                return JsonCamelCaseResult(new {Status = -1, Text = $"No package has transport code \"{transportCode}\""},
                    JsonRequestBehavior.AllowGet);

            var timeNow = DateTime.Now;

            packages.ForEach(x =>
            {
                x.SameCodeStatus = status;
                x.LastUpdate = timeNow;
            });

            await UnitOfWork.OrderPackageRepo.SaveAsync();

            return JsonCamelCaseResult(new {Status = 1, Text = "Status update successful"},
                JsonRequestBehavior.AllowGet);
        }
    }
}