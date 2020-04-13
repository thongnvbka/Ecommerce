using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    [Authorize]
    public class WalletTrackerController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.WalletTracker)]
        public async Task<ActionResult> Index()
        {
            var isManager = UserState.Type.HasValue && ((UserState.Type.Value == 2) || (UserState.Type.Value == 1));

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (isManager)
            {
                var warehouses = await UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x =>
                        ((isManager &&
                          ((x.IdPath == UserState.OfficeIdPath) || x.IdPath.StartsWith(UserState.OfficeIdPath + "."))) ||
                         (!isManager && (x.IdPath == UserState.OfficeIdPath))) &&
                        (x.Type == (byte)OfficeType.Warehouse) &&
                        !x.IsDelete && (x.Status == (byte)OfficeStatus.Use));

                ViewBag.Warehouses = JsonConvert.SerializeObject(warehouses.Select(Mapper.Map<OfficeDropdownResult>),
                    jsonSerializerSettings);
            }

            var allWarehouse = await
                UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));

            ViewBag.AllWarehouses =
                JsonConvert.SerializeObject(allWarehouse.Select(x => new { x.Id, x.Name, x.IdPath, x.Address }).ToList(),
                    jsonSerializerSettings);

            ViewBag.StatesSelectListItem = Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(
                    v =>
                        new SelectListItem
                        {
                            Value = ((byte)v).ToString(),
                            Text = EnumHelper.GetEnumDescription<WalletStatus>((int)v),
                            Selected = v == WalletStatus.Approved
                        })
                .ToList();

            ViewBag.TransportPartners =
                JsonConvert.SerializeObject(
                    await UnitOfWork.PartnerRepo.FindAsync(x => x.IsDelete == false && x.Status == 1, x => x.OrderBy(y => y.PriorityNo), null),
                    jsonSerializerSettings);

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderPackageStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderType>((int)v)),
                jsonSerializerSettings);

            ViewBag.TransportMethods =
                JsonConvert.SerializeObject(
                    await UnitOfWork.TransportMethodRepo.FindAsync(x => x.IsDelete == false && x.Status == 1),
                    jsonSerializerSettings);

            ViewBag.WalletStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.Entrepots =
                JsonConvert.SerializeObject(
                    await UnitOfWork.EntrepotRepo.FindAsync(x => x.IsDelete == false && x.Status == 1),
                    jsonSerializerSettings);

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.Wallet)]
        public async Task<ActionResult> Search(string warehouseIdPath, int? targetWarehouseId, int? userId,
            byte? status, DateTime? fromDate, DateTime? toDate, int? partnerId, byte? entrepotId, string keyword = "",
            int currentPage = 1, int recordPerPage = 20, byte mode = 0, int tabId = 0)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            Expression<Func<Wallet, bool>> wQuery;
            Expression<Func<DispatcherDetail, bool>> ddQuery;
            Expression<Func<Dispatcher, bool>> dQuery;
            Expression<Func<WalletTrackerResult, bool>> query;
            IQueryable<WalletTrackerResult> outQuery;
            List<WalletTrackerResult> items = new List<WalletTrackerResult>();
            int totalRecord;
            long totalWallet = 0;
            long totalWalletReceived = 0;
            long totalWalletCompleted = 0;

            // Tab Kho hàng (Lấy ra các bao hàng đang trong kho chưa điều chuyển)
            if (tabId == 0)
            {
                wQuery = x => x.UnsignedText.Contains(keyword) && (targetWarehouseId == null || x.TargetWarehouseId == targetWarehouseId) &&
                    x.IsDelete == false && ((userId == null) || (x.UserId == userId.Value)) && x.Mode == 0 &&
                    (entrepotId == null || x.EntrepotId == entrepotId) &&
                    ((isManager && ((x.CreatedWarehouseIdPath == warehouseIdPath) ||
                       x.CreatedWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                     (!isManager && (x.CreatedWarehouseIdPath == warehouseIdPath))) &&
                    (((fromDate == null) && (toDate == null)) ||
                     ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                     ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                     ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)));

                ddQuery = x => x.IsDelete == false;
                dQuery = x => x.IsDelete == false;

                query = x => (status == null || (status == 0 && x.DispatcherId == null) ||
                             (status == 1 && x.WalletPartnerId != null) ||
                             (status == 2 && x.WalletTargetWarehouseId == x.WalletCurrentWarehouseId));

                items = await UnitOfWork.WalletRepo.WalletTracker(wQuery, ddQuery, dQuery, query, currentPage, recordPerPage,
                out totalRecord, out outQuery);
            }
            else if (tabId == -1) // Tab chờ nhập kho (Lấy ra các bao hàng đang chờ nhập kho)
            {
                wQuery =
                    x =>
                        x.UnsignedText.Contains(keyword) && x.PartnerUpdate != null && x.Mode == 0 &&
                        (entrepotId == null || x.EntrepotId == entrepotId) &&
                        (status == null || status == 0 || (status == 1 && x.TargetWarehouseId == x.CurrentWarehouseId)) &&
                        x.IsDelete == false && ((isManager && ((x.TargetWarehouseIdPath == warehouseIdPath) ||
                                                               x.TargetWarehouseIdPath.StartsWith(
                                                                   warehouseIdPath + "."))) ||
                                                (!isManager && (x.TargetWarehouseIdPath == warehouseIdPath)));

                ddQuery = x => x.IsDelete == false;

                dQuery = x => x.IsDelete == false && ((userId == null) || (x.UserId == userId.Value)) &&
                    (((fromDate == null) && (toDate == null)) ||
                     ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                     ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                     ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)));

                query = x => (status == null || (status == 0 && x.WalletPartnerId != null));

                items = await UnitOfWork.WalletRepo.WalletTracker(wQuery, ddQuery, dQuery, query, currentPage, recordPerPage,
                            out totalRecord, out outQuery);

                totalWallet = totalRecord;
                totalWalletReceived = items.Any() ? await outQuery.Where(x => x.WalletPartnerId != null).LongCountAsync() : 0;
                totalWalletCompleted = items.Any() ? await outQuery.Where(x => x.WalletPartnerId == null).LongCountAsync() : 0;
            }
            else // Tab nhà vận chuyển. (Lấy ra các bao đã gửi đến nhà vận chuyển)
            {
                wQuery = x =>
                    x.UnsignedText.Contains(keyword) && x.Mode == 0 && x.IsDelete == false
                    && (entrepotId == null || x.EntrepotId == entrepotId) &&
                    (targetWarehouseId == null || x.TargetWarehouseId == targetWarehouseId)
                    && ((isManager && ((x.CreatedWarehouseIdPath == warehouseIdPath) ||
                                    x.CreatedWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                     (!isManager && (x.CreatedWarehouseIdPath == warehouseIdPath)) ||
                     (isManager && ((x.TargetWarehouseIdPath == warehouseIdPath) ||
                                    x.TargetWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                     (!isManager && (x.TargetWarehouseIdPath == warehouseIdPath)));

                ddQuery = x => x.IsDelete == false;

                dQuery = x => x.IsDelete == false && ((userId == null) || (x.UserId == userId.Value)) &&
                    (((fromDate == null) && (toDate == null)) ||
                     ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                     ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                     ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)));

                query = x => (status == null || x.DispatcherDetailStatus == status) && x.TransportPartnerId == tabId;

                items = await UnitOfWork.WalletRepo.WalletTrackerByPartner(wQuery, ddQuery, dQuery, query, currentPage, recordPerPage,
                out totalRecord, out outQuery);

                totalWallet = totalRecord;
                totalWalletReceived = items.Any() ? await outQuery.Where(x => x.WalletPartnerId == tabId).LongCountAsync() : 0;
                totalWalletCompleted = items.Any() ? await outQuery.Where(x => x.DispatcherDetailStatus == 2).LongCountAsync() : 0;
            }

            var totalWeight = items.Any() ? await outQuery.Where(x => x.WalletWeight.HasValue).SumAsync(x => x.WalletWeight) : 0;
            var totalWeightConverted = items.Any() ? await outQuery.Where(x => x.WalletWeightConverted.HasValue).SumAsync(x => x.WalletWeightConverted) : 0;
            var totalVolume = items.Any() ? await outQuery.Where(x => x.WalletVolume.HasValue).SumAsync(x => x.WalletVolume) : 0;
            var totalPackNo = items.Any() ? await outQuery.SumAsync(x => x.WalletPackageNo) : 0;

            return JsonCamelCaseResult(
                new
                {
                    items,
                    totalRecord,
                    tab =
                    new
                    {
                        totalWeight,
                        totalWeightConverted,
                        totalVolume,
                        totalPackNo,
                        totalWallet,
                        totalWalletReceived,
                        totalWalletCompleted,
                    }
                }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.WalletTracker, EnumPage.Dispatcher)]
        public async Task<ActionResult> Route(int walletId)
        {
            var items = await UnitOfWork.WalletRepo.DispatcherDetailByWalletId(walletId);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.View, EnumPage.WalletTracker)]
        public async Task<FileResult> Export(string warehouseIdPath, int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, int? partnerId, byte? entrepotId, string keyword = "",
            int currentPage = 1, int recordPerPage = 20, byte mode = 0, int tabId = 0)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            Expression<Func<Wallet, bool>> wQuery;
            Expression<Func<DispatcherDetail, bool>> ddQuery;
            Expression<Func<Dispatcher, bool>> dQuery;
            Expression<Func<WalletTrackerResult, bool>> query;
            IQueryable<WalletTrackerResult> outQuery;
            List<WalletTrackerResult> items;
            int totalRecord;

            // Tab Kho hàng (Lấy ra các bao hàng đang trong kho chưa điều chuyển)
            if (tabId == 0)
            {
                wQuery = x => x.UnsignedText.Contains(keyword) && x.Mode == 0 &&
                (entrepotId == null || x.EntrepotId == entrepotId) &&
                    x.IsDelete == false && ((userId == null) || (x.UserId == userId.Value)) &&
                    ((isManager && ((x.CreatedWarehouseIdPath == warehouseIdPath) ||
                       x.CreatedWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                     (!isManager && (x.CreatedWarehouseIdPath == warehouseIdPath))) &&
                    (((fromDate == null) && (toDate == null)) ||
                     ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                     ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                     ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)));

                ddQuery = x => x.IsDelete == false;
                dQuery = x => x.IsDelete == false;

                query = x => (status == null || (status == 0 && x.DispatcherId == null) ||
                             (status == 1 && x.WalletPartnerId != null) ||
                             (status == 2 && x.WalletTargetWarehouseId == x.WalletCurrentWarehouseId));

                await UnitOfWork.WalletRepo.WalletTracker(wQuery, ddQuery, dQuery, query, currentPage, recordPerPage,
                out totalRecord, out outQuery);
            }
            else if (tabId == -1) // Tab chờ nhập kho (Lấy ra các bao hàng đang chờ nhập kho)
            {
                wQuery =
                    x =>
                        x.UnsignedText.Contains(keyword) && x.PartnerUpdate != null && x.Mode == 0 &&
                        (entrepotId == null || x.EntrepotId == entrepotId) &&
                        (status == null || status == 0 || (status == 1 && x.TargetWarehouseId == x.CurrentWarehouseId)) &&
                        x.IsDelete == false && ((isManager && ((x.TargetWarehouseIdPath == warehouseIdPath) ||
                                                               x.TargetWarehouseIdPath.StartsWith(
                                                                   warehouseIdPath + "."))) ||
                                                (!isManager && (x.TargetWarehouseIdPath == warehouseIdPath)));

                ddQuery = x => x.IsDelete == false;

                dQuery = x => x.IsDelete == false && ((userId == null) || (x.UserId == userId.Value)) &&
                    (((fromDate == null) && (toDate == null)) ||
                     ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                     ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                     ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate))) &&
                     (entrepotId == null || x.EntrepotId.Value == entrepotId.Value);

                query = x => (status == null || (status == 0 && x.WalletPartnerId != null));

                await UnitOfWork.WalletRepo.WalletTracker(wQuery, ddQuery, dQuery, query, currentPage, recordPerPage,
                            out totalRecord, out outQuery);
            }
            else // Tab nhà vận chuyển. (Lấy ra các bao đã gửi đến nhà vận chuyển)
            {
                wQuery = x =>
                    x.UnsignedText.Contains(keyword) && x.IsDelete == false && x.Mode == 0 &&
                    (entrepotId == null || x.EntrepotId == entrepotId) &&
                    ((isManager && ((x.CreatedWarehouseIdPath == warehouseIdPath) ||
                                    x.CreatedWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                     (!isManager && (x.CreatedWarehouseIdPath == warehouseIdPath)) ||
                     (isManager && ((x.TargetWarehouseIdPath == warehouseIdPath) ||
                                    x.TargetWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                     (!isManager && (x.TargetWarehouseIdPath == warehouseIdPath)));

                ddQuery = x => x.IsDelete == false;

                dQuery = x => x.IsDelete == false && ((userId == null) || (x.UserId == userId.Value)) &&
                    (((fromDate == null) && (toDate == null)) ||
                     ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                     ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                     ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)))
                     && (entrepotId == null || x.EntrepotId.Value == entrepotId.Value);

                query = x => (status == null || x.DispatcherDetailStatus == status) && x.TransportPartnerId == tabId;

                await UnitOfWork.WalletRepo.WalletTrackerByPartner(wQuery, ddQuery, dQuery, query, currentPage, recordPerPage,
                out totalRecord, out outQuery);
            }

            items = await outQuery.OrderBy(x => new { x.DispatcherId, x.DispatcherCreatedUserId, x.WalletUserId }).ToListAsync();

            using (var xls = new ExcelPackage())
            {
                var sheetName = fromDate.HasValue && toDate.HasValue &&
                                Math.Abs(fromDate.Value.Subtract(toDate.Value).TotalDays) <= 0
                    ? toDate.Value.Day.ToString() : "Product";

                var sheet = xls.Workbook.Worksheets.Add(sheetName);

                ExcelHelper.CreateHeaderTable(sheet, 1, 1, "STT");
                ExcelHelper.CreateHeaderTable(sheet, 1, 2, "Packaging code");//"Mã bao"
                ExcelHelper.CreateHeaderTable(sheet, 1, 3, "Transit point");
                ExcelHelper.CreateHeaderTable(sheet, 1, 4, "Actual weight (kg)");
                ExcelHelper.CreateHeaderTable(sheet, 1, 5, "Weight conversion (kg)");
                ExcelHelper.CreateHeaderTable(sheet, 1, 6, "Package number");//Số kiện
                ExcelHelper.CreateHeaderTable(sheet, 1, 7, "Sent date ");//Ngày gửi
                ExcelHelper.CreateHeaderTable(sheet, 1, 8, "Warehouse staff");// NV kho
                ExcelHelper.CreateHeaderTable(sheet, 1, 9, "Warehouse");
                ExcelHelper.CreateHeaderTable(sheet, 1, 10, "Destination warehouse");//Kho đích
                ExcelHelper.CreateHeaderTable(sheet, 1, 11, "Volume (m3)");
                ExcelHelper.CreateHeaderTable(sheet, 1, 12, "Size (cm)");//Size 
                ExcelHelper.CreateHeaderTable(sheet, 1, 13, "Description");//Mo ta
                ExcelHelper.CreateHeaderTable(sheet, 1, 14, "Note");

                var stt = 1;
                foreach (var w in items)
                {
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 1, stt, ExcelHorizontalAlignment.Center);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 2, w.WalletCode);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 3, w.EntrepotName);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 4, w.WalletWeight, ExcelHorizontalAlignment.Right);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 5, w.WalletWeightConverted, ExcelHorizontalAlignment.Right);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 6, w.WalletPackageNo, ExcelHorizontalAlignment.Right);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 7, w.WalletCreated.ToString("dd/MM/yyyy"), ExcelHorizontalAlignment.Right);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 8, w.DispatcherCreatedUserFullName);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 9, w.WalletCreatedWarehouseName);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 10, w.WalletTargetWarehouseName);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 11, w.WalletVolume, ExcelHorizontalAlignment.Right);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 12, w.WalletSize);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 13, w.DispatcherDetailDescription);
                    ExcelHelper.CreateCellTable(sheet, stt + 1, 14, w.WalletNote, true);
                    stt++;
                }

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "TheoDoiBaoHang.xlsx");
            }
        }
    }
}