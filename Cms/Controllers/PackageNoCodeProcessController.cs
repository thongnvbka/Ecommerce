using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using Common.Helper;
using Common.Emums;
using AutoMapper;
using Cms.Attributes;
using Library.DbContext.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    [Authorize]
    public class PackageNoCodeProcessController : BaseController
    {
        // GET: PackageNoCodeProcess
        [LogTracker(EnumAction.View, EnumPage.PackageNoCodeProcess)]
        public async Task<ActionResult> Index()
        {
            var isManager = UserState.Type != null && (UserState.Type.Value == 2 || UserState.Type.Value == 1);

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (isManager)
            {
                var warehouses = await UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x => (isManager && (x.IdPath == UserState.OfficeIdPath || x.IdPath.StartsWith(UserState.OfficeIdPath + ".")) ||
                         (!isManager && x.IdPath == UserState.OfficeIdPath)) && x.Type == (byte)OfficeType.Warehouse &&
                        !x.IsDelete && x.Status == (byte)OfficeStatus.Use);

                ViewBag.Warehouses = JsonConvert.SerializeObject(warehouses.Select(Mapper.Map<OfficeDropdownResult>),
                    jsonSerializerSettings);
            }

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);
            return View();
        }
        
        public async Task<ActionResult> Search(string warehouseIdPath, DateTime? fromDate, DateTime? toDate,
            string keyword = "", int currentPage = 1, int recordPerPage = 20, byte mode = 0)
        {
            var isManager = UserState.Type != null && (UserState.Type.Value == 2 || UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            keyword = MyCommon.Ucs2Convert(keyword);

            long totalRecord;

            var packages = await UnitOfWork.OrderPackageRepo.FindAsync(out totalRecord,
                x =>
                    x.IsDelete == false && x.HashTag == ";packagelose;" && (x.TransportCode == keyword 
                    || x.UnsignedText.Contains(keyword)) &&
                    (isManager && (x.WarehouseIdPath == warehouseIdPath ||
                                   x.WarehouseIdPath.StartsWith(warehouseIdPath + ".")) ||
                     !isManager && x.WarehouseIdPath == warehouseIdPath) &&
                    (fromDate == null && toDate == null ||
                     fromDate != null && toDate != null && x.Created >= fromDate && x.Created <= toDate ||
                     fromDate == null && toDate.HasValue && x.Created <= toDate ||
                     toDate == null && fromDate.HasValue && x.Created >= fromDate), x => x.OrderBy(y => y.Id),
                currentPage, recordPerPage);

            return JsonCamelCaseResult(new { packages, totalRecord }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.PackageNoCodeProcess)]
        public async Task<ActionResult> Delete(int packageId)
        {
            var package =
                await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(x => x.Id == packageId && x.IsDelete == false && x.HashTag == ";packagelose;");

            if(package == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Lost customer information does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);

            package.IsDelete = true;
            package.LastUpdate = DateTime.Now;

            await UnitOfWork.OrderPackageRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = 1, Text = "Delete successful code loss information" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.PackageNoCodeProcess)]
        public async Task<ActionResult> DeleteAll(string packageCodes)
        {
            var package = await UnitOfWork.OrderPackageRepo.FindAsync(
                    x => packageCodes.Contains(";" + x.Code + ";") && x.IsDelete == false && x.HashTag == ";packagelose;");

            if (!package.Any())
                return JsonCamelCaseResult(new { Status = -1, Text = "Lost customer information does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);

            package.ForEach(x =>
            {
                x.IsDelete = true;
                x.LastUpdate = DateTime.Now;
            });

            await UnitOfWork.OrderPackageRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = 1, Text = "Delete successful code loss information" }, JsonRequestBehavior.AllowGet);
        }
    }
}