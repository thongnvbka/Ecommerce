using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    /// <summary>
    /// Đóng kiện gỗ
    /// </summary>
    [Authorize]
    public class PackingController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.Packing)]
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

                var systems = await UnitOfWork.SystemRepo.FindAsNoTrackingAsync(x => x.Status == 1);
                ViewBag.Systems = JsonConvert.SerializeObject(systems.Select(x => new { x.Id, x.Domain, x.Name }),
                    jsonSerializerSettings);
            }

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderTypes = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                .Cast<OrderType>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderType>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderType>((int)v)),
                jsonSerializerSettings);

            ViewBag.StatesSelectListItem = Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(
                    v =>
                        new SelectListItem
                        {
                            Value = ((byte) v).ToString(),
                            Text = EnumHelper.GetEnumDescription<WalletStatus>((int) v),
                            Selected = v == WalletStatus.Approved
                        })
                .ToList();

            var allWarehouse = await UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                x => !x.IsDelete && (x.Type == (byte) OfficeType.Warehouse) && (x.Status == (byte) OfficeStatus.Use));

            ViewBag.AllWarehouses =
                JsonConvert.SerializeObject(allWarehouse.Select(x => new {x.Id, x.Name, x.IdPath, x.Address}).ToList(),
                    jsonSerializerSettings);

            ViewBag.OrderPackageStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.WalletStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            ViewBag.Entrepots = JsonConvert.SerializeObject(
                await UnitOfWork.EntrepotRepo.FindAsync(x => x.IsDelete == false && x.Status == 1),
                jsonSerializerSettings);

            return View();
        }

        /// <param name="warehouseIdPath"></param>
        /// <param name="systemId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="orderType"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="keyword"></param>
        /// <param name="currentPage"></param>
        /// <param name="recordPerPage"></param>
        /// <param name="mode">0: All, 1: Wait for packing wooden , 2: Have packing wooden </param>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.Packing)]
        public async Task<ActionResult> Search(string warehouseIdPath, int? systemId, int? userId, 
            byte? status, byte? orderType, DateTime? fromDate, DateTime? toDate, string keyword = "",
            int currentPage = 1, int recordPerPage = 20, byte mode = 0)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = UserState.Type.Value == 2 || UserState.Type.Value == 1;

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            int totalRecord;
            IQueryable<PackingResult> queryOut;

            Expression<Func<OrderPackage, bool>> predicate1 =
                x => x.UnsignedText.Contains(keyword) && x.IsDelete == false && x.OrderId != 0 && x.Weight != null &&
                    (status == null || x.Status == status.Value) &&
                    (orderType == null || x.OrderType == orderType.Value) &&
                     (systemId == null || x.SystemId == systemId.Value) &&
                     (userId == null || x.UserId == userId.Value) &&
                     ((isManager && (x.CurrentWarehouseIdPath == warehouseIdPath || x.CurrentWarehouseIdPath.StartsWith(warehouseIdPath + ".") 
                     || x.WarehouseIdPath == warehouseIdPath || x.WarehouseIdPath.StartsWith(warehouseIdPath + ".") || x.CustomerWarehouseIdPath == warehouseIdPath 
                     || x.CustomerWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.CurrentWarehouseIdPath == warehouseIdPath || x.WarehouseIdPath == warehouseIdPath || x.CustomerWarehouseIdPath == warehouseIdPath)));

            Expression<Func<PackingResult, bool>> predicate2;

            int all;
            int wait;
            int complete;

            // Tất cả
            if (mode == 0)
            {
                predicate2 = x => ((x.WalletId == null && x.CurrentWarehouseIdPath == warehouseIdPath) || x.WalletId != null) && (fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate);
            }
            // Chờ đóng kiện gỗ
            else if (mode == 1)
            {
                predicate2 = x => x.WalletId == null && x.CurrentWarehouseIdPath == warehouseIdPath && ((fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate));
            }
            // Đã đóng kiện gỗ
            else
            {
                predicate2 = x => x.WalletId != null && (fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate);
            }

            var items = await UnitOfWork.OrderPackageRepo.SearchPacking(out totalRecord, out queryOut, predicate1, predicate2,
                currentPage, recordPerPage);

            // Tất cả
            if (mode == 0)
            {
                all = totalRecord;

                wait = await queryOut.CountAsync(x => x.WalletId == null && x.CurrentWarehouseIdPath == warehouseIdPath && (fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate));

                complete = await queryOut.CountAsync(x => x.WalletId != null && (fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate));
            }

            // Chờ đóng kiện gỗ
            else if (mode == 1)
            {
                all = await queryOut.CountAsync(x=> ((x.WalletId == null && x.CurrentWarehouseIdPath == warehouseIdPath) || x.WalletId != null) && (fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate));

                wait = totalRecord;

                complete = await queryOut.CountAsync(x => x.WalletId != null && (fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate));
            }
            // Đã đóng kiện gỗ
            else
            {
                all = await queryOut.CountAsync(x=> ((x.WalletId == null && x.CurrentWarehouseIdPath == warehouseIdPath) || x.WalletId != null) && 
                
                (fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate));
                wait = await queryOut.CountAsync(x => x.WalletId == null && x.CurrentWarehouseIdPath == warehouseIdPath && (fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate));
                complete = totalRecord;
            }

            return JsonCamelCaseResult(
                new {items, totalRecord, mode = new {all, wait, complete}},
                JsonRequestBehavior.AllowGet);
        }
    }
}