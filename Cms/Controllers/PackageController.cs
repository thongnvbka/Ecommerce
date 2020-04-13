using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Library.Models;
using System;
using System.Collections.Generic;
using Library.ViewModels;
using Common.Helper;
using Common.Emums;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using AutoMapper;
using Cms.Attributes;
using Cms.Helpers;
using Cms.Jobs;
using Hangfire;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using Library.DbContext.Results;
using Library.Jobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    [Authorize]
    public class PackageController : BaseController
    {
        public ActionResult Test()
        {
            //NotifyHelper.CreateAndSendNotifySystemToClient(26, "Tieu de", EnumNotifyType.Info, "Noi dung");

            //return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);

            MessageHelper.SystemSendMessage(new List<int>() { 26 }, "Order has not had waybill code yet", "Content");

            StringUtil.RenderPartialViewToString(new BaseController(), "~/Views/Shared/_OrderImportOvertime.cshtml", "");

            return View();
        }

        [LogTracker(EnumAction.View, EnumPage.Package)]
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

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.Package)]
        public async Task<ActionResult> Search(string warehouseIdPath, byte? day, int? systemId, int? userId, byte? status,
            byte? orderType, DateTime? fromDate, DateTime? toDate, string keyword = "", int currentPage = 1,
            int recordPerPage = 20, byte mode = 0)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            var isManager = UserState.Type != null && (UserState.Type.Value == 2 || UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            long totalRecord = 0;

            DateTime? timeNow = null;

            if (day.HasValue && day != 0)
            {
                timeNow = DateTime.Now.AddDays(day.Value * -1);
            }

            // Query đang trong kho
            Expression<Func<OrderPackage, bool>> queryInStock =
                x => x.UnsignedText.Contains(keyword) && !x.IsDelete && x.OrderId != 0 &&
                    (status == null || x.Status == status.Value) &&
                    (orderType == null || x.OrderType == orderType.Value) &&
                     (systemId == null || x.SystemId == systemId.Value) &&
                     (userId == null || x.UserId == userId.Value) &&
                     ((isManager && (x.CurrentWarehouseIdPath == warehouseIdPath || x.CurrentWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && x.CurrentWarehouseIdPath == warehouseIdPath)) &&
                     ((fromDate == null && toDate == null) ||
                      (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate)) &&
                      (timeNow == null || x.Created <= timeNow.Value);

            // Query các kiện chờ nhập kho
            Expression<Func<OrderPackage, bool>> queryWaitImport =
                x => x.UnsignedText.Contains(keyword) && !x.IsDelete && x.OrderId != 0 &&
                    (orderType == null || x.OrderType == orderType.Value) &&
                    (status == null || x.Status == status.Value) &&
                     (systemId == null || x.SystemId == systemId.Value) &&
                     (userId == null || x.UserId == userId.Value) &&
                     (((isManager && x.CurrentWarehouseId == null && x.Status == (byte)OrderPackageStatus.ShopDelivery && (x.WarehouseIdPath == warehouseIdPath || x.WarehouseIdPath.StartsWith(warehouseIdPath + ".")))
                     || (!isManager && x.CurrentWarehouseId == null && x.Status == (byte)OrderPackageStatus.ShopDelivery && x.WarehouseIdPath == warehouseIdPath)) ||
                     ((isManager && x.CurrentWarehouseId == null && x.Status == (byte)OrderPackageStatus.PartnerDelivery && (x.CustomerWarehouseIdPath == warehouseIdPath || x.CustomerWarehouseIdPath.StartsWith(warehouseIdPath + ".")))
                     || (!isManager && x.CurrentWarehouseId == null && x.Status == (byte)OrderPackageStatus.PartnerDelivery && x.CustomerWarehouseIdPath == warehouseIdPath))) &&
                     ((fromDate == null && toDate == null) || (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                      (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                      (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate)) &&
                      (timeNow == null || x.Created <= timeNow.Value);

            // Query tất cả các kiện
            Expression<Func<OrderPackage, bool>> queryAll = x => x.UnsignedText.Contains(keyword) && !x.IsDelete && x.OrderId != 0 &&
                   (orderType == null || x.OrderType == orderType.Value) &&
                   (status == null || x.Status == status.Value) &&
                    (systemId == null || x.SystemId == systemId.Value) &&
                    (userId == null || x.UserId == userId.Value) && ((((isManager && (x.CurrentWarehouseIdPath == warehouseIdPath || x.CurrentWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                     (!isManager && x.CurrentWarehouseIdPath == warehouseIdPath))) ||
                     ((isManager && (x.CustomerWarehouseIdPath == warehouseIdPath || x.CustomerWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                     (!isManager && x.CustomerWarehouseIdPath == warehouseIdPath)) ||
                     ((isManager && (x.WarehouseIdPath == warehouseIdPath || x.WarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                     (!isManager && x.WarehouseIdPath == warehouseIdPath))) &&
                    ((fromDate == null && toDate == null) || (fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate) ||
                     (fromDate == null && toDate.HasValue && x.ForcastDate <= toDate) ||
                     (toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate)) &&
                     (timeNow == null || x.Created <= timeNow.Value);


            List<OrderPackage> packages = new List<OrderPackage>();
            // Chờ nhập kho 
            if (mode == 0)
                packages = await UnitOfWork.OrderPackageRepo.FindAsNoTrackingAsync(out totalRecord, queryWaitImport,
                    x => x.OrderBy(y => y.OrderCode), currentPage, recordPerPage);

            // Đang trong kho
            if (mode == 1)
                packages = await UnitOfWork.OrderPackageRepo.FindAsNoTrackingAsync(out totalRecord, queryInStock,
                    x => x.OrderBy(y => y.OrderCode), currentPage, recordPerPage);

            // Tất cả 
            if (mode == 2)
                packages = await UnitOfWork.OrderPackageRepo.FindAsNoTrackingAsync(out totalRecord, queryAll,
                    x => x.OrderBy(y => y.OrderCode), currentPage, recordPerPage);

            // Count group
            var inStockNo = await UnitOfWork.OrderPackageRepo.LongCountAsync(queryInStock);
            var waitImportNo = await UnitOfWork.OrderPackageRepo.LongCountAsync(queryWaitImport);
            var allNo = await UnitOfWork.OrderPackageRepo.LongCountAsync(queryAll);

            return JsonCamelCaseResult(
                new { items = packages, totalRecord, mode = new { inStockNo, waitImportNo, allNo } },
                JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Package, EnumPage.ImportWarehouse, EnumPage.Wallet, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> GetDetail(int id)
        {
            var package = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            var packageHistories = await UnitOfWork.PackageHistoryRepo.FindAsync(x => x.PackageId == id);

            var packageNotes = await UnitOfWork.PackageNoteRepo.FindAsNoTrackingAsync(x => x.PackageId == id);

            var packageNoteMode = Enum.GetValues(typeof(PackageNoteMode))
                .Cast<PackageNoteMode>()
                .ToDictionary(v => (byte) v, v => EnumHelper.GetEnumDescription<PackageNoteMode>((int) v));

            return JsonCamelCaseResult(new { package, packageHistories, packageNotes, packageNoteMode }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Tìm package để nhập kho
        /// </summary>
        /// <param name="packageCodes"></param>
        /// <param name="term"></param>
        /// <param name="size"></param>
        /// <param name="isPaste"></param>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.ImportWarehouse, EnumPage.Package, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> Suggetion(string packageCodes, string term, int size = 6, bool isPaste = false)
        {
            // Chỉ để dạng ký tự không dấu
            term = MyCommon.Ucs2Convert(term);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            var warehouseIdPath = UserState.OfficeIdPath;

            // Chỉ lấy mã dạng số: (vd: ODR005566 chỉ lấy 005566)
            // term = Regex.Replace(term, "[^0-9]", "");

            var items = await UnitOfWork.OrderPackageRepo.SuggetionForImportWarehouse(packageCodes, warehouseIdPath, term, size, isPaste);

            var packageIds = $";{string.Join(";", items.Select(x => x.Id).ToList())};";

            var packageNotesData =
                await UnitOfWork.PackageNoteRepo.FindAsNoTrackingAsync(
                    x => x.PackageId != null && packageIds.Contains(";" + x.PackageId + ";"));

            var packageNotes = packageNotesData.GroupBy(x => x.PackageId)
                .ToDictionary(x => x.Key, x => x.OrderBy(o => o.Time).ToList());

            var packageNoteMode = Enum.GetValues(typeof(PackageNoteMode))
                .Cast<PackageNoteMode>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<PackageNoteMode>((int)v));

            foreach (var orderPackage in items)
            {
                orderPackage.PackageNotes = packageNotes.ContainsKey(orderPackage.Id)
                    ? packageNotes[orderPackage.Id]
                    : null;
                orderPackage.PackageNoteMode = packageNoteMode;
                orderPackage.Note = "";
            }

            //foreach (var orderPackage in items)
            //{
            //    // Lấy ra tất cả các package trùng mã vận đơn
            //    var packageSameTransportCode = await UnitOfWork.OrderPackageRepo.FindAsync(
            //                x => x.TransportCode == orderPackage.TransportCode && !x.IsDelete && x.Status != (byte)OrderPackageStatus.Completed);

            //    // Có package trùng mã vận đơn
            //    if (packageSameTransportCode.Any(x => x.Id != orderPackage.Id))
            //    {
            //        // Một Order có nhiều kiện trùng mã vận đơn
            //        if (packageSameTransportCode.Count(x => x.OrderId == orderPackage.OrderId) == packageSameTransportCode.Count())
            //        {
            //            var packagesCode = string.Join(", ", packageSameTransportCode.Select(x => "P" + x.Code).ToList());

            //            foreach (var p in packageSameTransportCode)
            //            {
            //                // Add cảnh báo cho package
            //                p.Note = $" Các kiện liên quan: {packagesCode} In the same Order";
            //                //p.PackageCodes = $"{packagesCode}";
            //                //p.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
            //            }

            //            orderPackage.Note = $" Related packages: {packagesCode} in the same order";
            //            //orderPackage.PackageCodes = $"{packagesCode}";
            //            //orderPackage.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
            //        }
            //        else if (packageSameTransportCode.Any(x => x.OrderId != orderPackage.OrderId)) // Có Order trùng mã vận đơn
            //        {
            //            var orderCode = string.Join(", ", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList());
            //            //var customers = string.Join(", ", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList());

            //            // Kiểm tra khác kho đích
            //            var diffWarehouse = packageSameTransportCode.Any(x => x.CustomerWarehouseId != orderPackage.CustomerWarehouseId);

            //            foreach (var p in packageSameTransportCode)
            //            {
            //                // Add cảnh báo cho kiện hàng
            //                p.Note = $" Order with coincided waybill code: {orderCode}";
            //                //p.OrderCodes = $"{orderCode}";
            //                //p.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
            //                //p.Customers = $"{customers}";
            //                //p.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

            //                if (diffWarehouse)
            //                    p.Note += ", The destination package is different";
            //            }

            //            // Add cảnh báo cho kiện hàng
            //            orderPackage.Note = $" Order with coincided waybill code: {orderCode}";
            //            //orderPackage.OrderCodes = $"{orderCode}";
            //            //orderPackage.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
            //            //orderPackage.Customers = $"{customers}";
            //            //orderPackage.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

            //            if (diffWarehouse)
            //                orderPackage.Note += ", The destination package is different";
            //        }

            //        //await UnitOfWork.OrderPackageRepo.SaveAsync();
            //    }
            //}

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Tìm package để đóng bao
        /// </summary>
        /// <param name="packageCodes"></param>
        /// <param name="term"></param>
        /// <param name="targetWarehouseId"></param>
        /// <param name="size"></param>
        /// <param name="isPaste"></param>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.Wallet, EnumPage.Package)]
        public async Task<ActionResult> Suggetion2(string packageCodes, string term, int? targetWarehouseId, int size = 6, bool isPaste = false)
        {
            // Chỉ để dạng ký tự không dấu
            term = MyCommon.Ucs2Convert(term);

            // Chỉ lấy mã dạng số: (vd: ODR005566 chỉ lấy 005566)
            // term = Regex.Replace(term, "[^0-9]", "");

            var packages = await UnitOfWork.OrderPackageRepo.Suggetion(term, packageCodes, size, targetWarehouseId, isPaste);

            var packageCodesStr = $";{string.Join(";", packages.Select(x => x.Code).ToList())};";

            // Kiểm tra package nằm trong Order chưa được kiểm đếm
            var orderCodesNoAudit = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodesStr);

            // Các Order wrong tally chưa được xử lý
            var orderCodesAuditLose = await UnitOfWork.OrderRepo.OrderCodeAcountingLose(packageCodesStr);

            // Kiểm tra tạo bao những package chưa được Packing gỗ
            var orderCodesNoPacking = await UnitOfWork.OrderRepo.OrderCodeByServicePacking(packageCodesStr);

            // Kiểm tra chứa đơn hàng không sử dụng dịch vụ đóng kiện gỗ
            var orderCodesNoPackingService = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodesStr, OrderServices.Packing, false);

            //// Order đi nhanh và đi chậm cùng 1 bao hàng
            //var orderCodesFastDelivery = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodesStr, OrderServices.FastDelivery);
            var orderCodesFastDelivery = new OrderService();
            var orderCodesOptimal = new OrderService();
            //// Order tối ưu cùng 1 bao hàng
            //var orderCodesOptimal = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodesStr, OrderServices.Optimal);

            var packageIds = $";{string.Join(";", packages.Select(x => x.Id).ToList())};";

            var packageNotesData =
                await UnitOfWork.PackageNoteRepo.FindAsNoTrackingAsync(
                    x => x.PackageId != null && packageIds.Contains(";" + x.PackageId + ";"));

            var packageNotes = packageNotesData.GroupBy(x => x.PackageId)
                .ToDictionary(x => x.Key, x => x.OrderBy(o => o.Time).ToList());

            var packageNoteMode = Enum.GetValues(typeof(PackageNoteMode))
                .Cast<PackageNoteMode>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<PackageNoteMode>((int)v));

            foreach (var orderPackage in packages)
            {
                orderPackage.PackageNotes = packageNotes.ContainsKey(orderPackage.Id)
                    ? packageNotes[orderPackage.Id]
                    : null;
                orderPackage.PackageNoteMode = packageNoteMode;

                orderPackage.Note = null;
            }

            foreach (var orderPackage in packages)
            {
                // Lấy ra tất cả các package trùng mã vận đơn
                var packageSameTransportCode = await UnitOfWork.OrderPackageRepo.FindAsync(
                            x => x.TransportCode == orderPackage.TransportCode && !x.IsDelete && x.Status != (byte)OrderPackageStatus.Completed);

                // Có package trùng mã vận đơn
                if (packageSameTransportCode.Any(x => x.Id != orderPackage.Id))
                {
                    // Một Order có nhiều kiện trùng mã vận đơn
                    if (packageSameTransportCode.Count(x => x.OrderId == orderPackage.OrderId) == packageSameTransportCode.Count())
                    {
                        var packagesCode = string.Join(", ", packageSameTransportCode.Select(x => "P" + x.Code).ToList());

                        foreach (var p in packageSameTransportCode)
                        {
                            // Add cảnh báo cho kiện hàng
                            p.Note = $" Related packages: {packagesCode} in the same order";
                            //p.PackageCodes = $"{packagesCode}";
                            //p.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
                        }

                        orderPackage.Note = $" Related packages: {packagesCode} in the same order";
                        //orderPackage.PackageCodes = $"{packagesCode}";
                        //orderPackage.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
                    }
                    else if (packageSameTransportCode.Any(x => x.OrderId != orderPackage.OrderId)) // Có Order trùng mã vận đơn
                    {
                        var orderCode = string.Join(", ", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList());
                        //var customers = string.Join(", ", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList());

                        // Kiểm tra khác kho đích
                        var diffWarehouse = packageSameTransportCode.Any(x => x.CustomerWarehouseId != orderPackage.CustomerWarehouseId);

                        foreach (var p in packageSameTransportCode)
                        {
                            // Add cảnh báo cho kiện hàng
                            p.Note = $" Order with coincided waybill code: {orderCode}";
                            //p.OrderCodes = $"{orderCode}";
                            //p.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
                            //p.Customers = $"{customers}";
                            //p.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

                            if (diffWarehouse)
                                p.Note += ", Destination warehouses of packages are different";
                        }

                        // Add cảnh báo cho kiện hàng
                        orderPackage.Note = $" Order with coincided waybill code: {orderCode}";
                        //orderPackage.OrderCodes = $"{orderCode}";
                        //orderPackage.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
                        //orderPackage.Customers = $"{customers}";
                        //orderPackage.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

                        if (diffWarehouse)
                            orderPackage.Note += ", Destination warehouses of packages are different";
                    }

                    //await UnitOfWork.OrderPackageRepo.SaveAsync();
                }
            }

            return JsonCamelCaseResult(new { packages, orderCodesNoAudit, orderCodesAuditLose, orderCodesNoPacking, orderCodesFastDelivery, orderCodesOptimal, orderCodesNoPackingService }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Transfer, EnumPage.Package)]
        public async Task<ActionResult> SuggetionForTransfer(string packageCodes, string term, int size = 6, bool isPaste = false)
        {
            term = MyCommon.Ucs2Convert(term);

            var items = await UnitOfWork.OrderPackageRepo.SuggetionForTransfer(term, packageCodes, size, UserState.OfficeId ?? 0, isPaste);

            var packageIds = $";{string.Join(";", items.Select(x => x.Id).ToList())};";

            var packageNotesData =
                await UnitOfWork.PackageNoteRepo.FindAsNoTrackingAsync(
                    x => x.PackageId != null && packageIds.Contains(";" + x.PackageId + ";"));

            var packageNotes = packageNotesData.GroupBy(x => x.PackageId)
                .ToDictionary(x => x.Key, x => x.OrderBy(o => o.Time).ToList());

            var packageNoteMode = Enum.GetValues(typeof(PackageNoteMode))
                .Cast<PackageNoteMode>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<PackageNoteMode>((int)v));

            foreach (var orderPackage in items)
            {
                orderPackage.PackageNotes = packageNotes.ContainsKey(orderPackage.Id)
                    ? packageNotes[orderPackage.Id]
                    : null;
                orderPackage.PackageNoteMode = packageNoteMode;

                orderPackage.Note = null;
            }

            //foreach (var orderPackage in items)
            //{
            //    // Lấy ra tất cả các package trùng mã vận đơn
            //    var packageSameTransportCode = await UnitOfWork.OrderPackageRepo.FindAsync(
            //                x => x.TransportCode == orderPackage.TransportCode && !x.IsDelete && x.Status != (byte)OrderPackageStatus.Completed);

            //    // Có package trùng mã vận đơn
            //    if (packageSameTransportCode.Any(x => x.Id != orderPackage.Id))
            //    {
            //        // Một Order có nhiều kiện trùng mã vận đơn
            //        if (packageSameTransportCode.Count(x => x.OrderId == orderPackage.OrderId) == packageSameTransportCode.Count())
            //        {
            //            var packagesCode = string.Join(", ", packageSameTransportCode.Select(x => "P" + x.Code).ToList());

            //            foreach (var p in packageSameTransportCode)
            //            {
            //                // Add cảnh báo cho kiện hàng
            //                p.Note = $" Related packages: {packagesCode} in the same order";
            //                //p.PackageCodes = $"{packagesCode}";
            //                //p.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
            //            }

            //            orderPackage.Note = $" Related packages: {packagesCode} in the same order";
            //            //orderPackage.PackageCodes = $"{packagesCode}";
            //            //orderPackage.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
            //        }
            //        else if (packageSameTransportCode.Any(x => x.OrderId != orderPackage.OrderId)) // Có Order trùng mã vận đơn
            //        {
            //            var orderCode = string.Join(", ", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList());
            //            //var customers = string.Join(", ", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList());

            //            // Kiểm tra khác kho đích
            //            var diffWarehouse = packageSameTransportCode.Any(x => x.CustomerWarehouseId != orderPackage.CustomerWarehouseId);

            //            foreach (var p in packageSameTransportCode)
            //            {
            //                // Add cảnh báo cho kiện hàng
            //                p.Note = $" Order with coincided waybill code: {orderCode}";
            //                //p.OrderCodes = $"{orderCode}";
            //                //p.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
            //                //p.Customers = $"{customers}";
            //                //p.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

            //                if (diffWarehouse)
            //                    p.Note += ", The destination package is different";
            //            }

            //            // Add cảnh báo cho kiện hàng
            //            orderPackage.Note = $" Order with coincided waybill code: {orderCode}";
            //            //orderPackage.OrderCodes = $"{orderCode}";
            //            //orderPackage.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
            //            //orderPackage.Customers = $"{customers}";
            //            //orderPackage.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

            //            if (diffWarehouse)
            //                orderPackage.Note += ", The destination package is different";
            //        }

            //        //await UnitOfWork.OrderPackageRepo.SaveAsync();
            //    }
            //}

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Packing)]
        public async Task<ActionResult> GetForPacking(string packageCodes)
        {
            var items = await UnitOfWork.OrderPackageRepo.Suggetion2(packageCodes);

            var packageIds = $";{string.Join(";", items.Select(x => x.Id).ToList())};";

            var packageNotesData =
                await UnitOfWork.PackageNoteRepo.FindAsNoTrackingAsync(
                    x => x.PackageId != null && packageIds.Contains(";" + x.PackageId + ";"));

            var packageNotes = packageNotesData.GroupBy(x => x.PackageId)
                .ToDictionary(x => x.Key, x => x.OrderBy(o => o.Time).ToList());

            var packageNoteMode = Enum.GetValues(typeof(PackageNoteMode))
                .Cast<PackageNoteMode>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<PackageNoteMode>((int)v));

            foreach (var orderPackage in items)
            {
                orderPackage.PackageNotes = packageNotes.ContainsKey(orderPackage.Id)
                    ? packageNotes[orderPackage.Id]
                    : null;
                orderPackage.PackageNoteMode = packageNoteMode;

                orderPackage.Note = null;
            }

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Tìm kiếm kiện cho PutAway
        /// </summary>
        /// <param name="packageCodes"></param>
        /// <param name="term"></param>
        /// <param name="size"></param>
        /// <param name="isPaste"></param>
        /// <returns></returns>
        // Suggetion packages for putAway
        [CheckPermission(EnumAction.View, EnumPage.PutAway, EnumPage.Package)]
        public async Task<ActionResult> SuggetionForPutAway(string packageCodes, string term, int size = 6, bool isPaste = false)
        {
            // Chỉ để dạng ký tự không dấu
            term = MyCommon.Ucs2Convert(term);

            // Chỉ lấy mã dạng số: (vd: ODR005566 chỉ lấy 005566)
            // term = Regex.Replace(term, "[^0-9]", "");

            var items = await UnitOfWork.OrderPackageRepo.SuggetionForPutAway(term, packageCodes, size, UserState.OfficeId ?? 0, isPaste);

            var packageIds = $";{string.Join(";", items.Select(x => x.Id).ToList())};";

            var packageNotesData =
                await UnitOfWork.PackageNoteRepo.FindAsNoTrackingAsync(
                    x => x.PackageId != null && packageIds.Contains(";" + x.PackageId + ";"));

            var packageNotes = packageNotesData.GroupBy(x => x.PackageId)
                .ToDictionary(x => x.Key, x => x.OrderBy(o => o.Time).ToList());

            var packageNoteMode = Enum.GetValues(typeof(PackageNoteMode))
                .Cast<PackageNoteMode>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<PackageNoteMode>((int)v));

            foreach (var orderPackage in items)
            {
                orderPackage.PackageNotes = packageNotes.ContainsKey(orderPackage.Id)
                    ? packageNotes[orderPackage.Id]
                    : null;
                orderPackage.PackageNoteMode = packageNoteMode;
                orderPackage.Note = null;
            }

            //foreach (var orderPackage in items)
            //{
            //    // Lấy ra tất cả các package trùng mã vận đơn
            //    var packageSameTransportCode = await UnitOfWork.OrderPackageRepo.FindAsync(
            //                x => x.TransportCode == orderPackage.TransportCode && !x.IsDelete && x.Status != (byte)OrderPackageStatus.Completed);

            //    // Có package trùng mã vận đơn
            //    if (packageSameTransportCode.Any(x => x.Id != orderPackage.Id))
            //    {
            //        // Một Order có nhiều kiện trùng mã vận đơn
            //        if (packageSameTransportCode.Count(x => x.OrderId == orderPackage.OrderId) == packageSameTransportCode.Count())
            //        {
            //            var packagesCode = string.Join(", ", packageSameTransportCode.Select(x => "P" + x.Code).ToList());

            //            foreach (var p in packageSameTransportCode)
            //            {
            //                // Add cảnh báo cho kiện hàng
            //                p.Note = $" Related packages: {packagesCode} in the same order";
            //                //p.PackageCodes = $"{packagesCode}";
            //                //p.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
            //            }

            //            orderPackage.Note = $" Related packages: {packagesCode} in the same order";
            //            //orderPackage.PackageCodes = $"{packagesCode}";
            //            //orderPackage.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
            //        }
            //        else if (packageSameTransportCode.Any(x => x.OrderId != orderPackage.OrderId)) // Có Order trùng mã vận đơn
            //        {
            //            var orderCode = string.Join(", ", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList());
            //            //var customers = string.Join(", ", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList());

            //            // Kiểm tra khác kho đích
            //            var diffWarehouse = packageSameTransportCode.Any(x => x.CustomerWarehouseId != orderPackage.CustomerWarehouseId);

            //            foreach (var p in packageSameTransportCode)
            //            {
            //                // Add cảnh báo cho kiện hàng
            //                p.Note = $" Order with coincided waybill code: {orderCode}";
            //                //p.OrderCodes = $"{orderCode}";
            //                //p.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
            //                //p.Customers = $"{customers}";
            //                //p.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

            //                if (diffWarehouse)
            //                    p.Note += ", The destination package is different";
            //            }

            //            // Add cảnh báo cho kiện hàng
            //            orderPackage.Note = $" Order with coincided waybill code: {orderCode}";
            //            //orderPackage.OrderCodes = $"{orderCode}";
            //            //orderPackage.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
            //            //orderPackage.Customers = $"{customers}";
            //            //orderPackage.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

            //            if (diffWarehouse)
            //                orderPackage.Note += ", The destination package is different";
            //        }

            //        //await UnitOfWork.OrderPackageRepo.SaveAsync();
            //    }
            //}

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.PutAway, EnumPage.Package)]
        public async Task<ActionResult> SuggetionForPutAwayByWalletId(int walletId)
        {
            var items = await UnitOfWork.OrderPackageRepo.SuggetionForPutAway(walletId, UserState.OfficeId ?? 0);

            //foreach (var orderPackage in items)
            //{
            //    // Lấy ra tất cả các package trùng mã vận đơn
            //    var packageSameTransportCode = await UnitOfWork.OrderPackageRepo.FindAsync(
            //                x => x.TransportCode == orderPackage.TransportCode && !x.IsDelete && x.Status != (byte)OrderPackageStatus.Completed);

            //    // Có package trùng mã vận đơn
            //    if (packageSameTransportCode.Any(x => x.Id != orderPackage.Id))
            //    {
            //        // Một Order có nhiều kiện trùng mã vận đơn
            //        if (packageSameTransportCode.Count(x => x.OrderId == orderPackage.OrderId) == packageSameTransportCode.Count())
            //        {
            //            var packagesCode = string.Join(", ", packageSameTransportCode.Select(x => "P" + x.Code).ToList());

            //            foreach (var p in packageSameTransportCode)
            //            {
            //                // Add cảnh báo cho kiện hàng
            //                p.Note = $" Related packages: {packagesCode} in the same order";
            //                //p.PackageCodes = $"{packagesCode}";
            //                //p.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
            //            }

            //            orderPackage.Note = $" Related packages: {packagesCode} in the same order";
            //            //orderPackage.PackageCodes = $"{packagesCode}";
            //            //orderPackage.PackageCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => "P" + x.Code).ToList())};";
            //        }
            //        else if (packageSameTransportCode.Any(x => x.OrderId != orderPackage.OrderId)) // Có Order trùng mã vận đơn
            //        {
            //            var orderCode = string.Join(", ", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList());
            //            //var customers = string.Join(", ", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList());

            //            // Kiểm tra khác kho đích
            //            var diffWarehouse = packageSameTransportCode.Any(x => x.CustomerWarehouseId != orderPackage.CustomerWarehouseId);

            //            foreach (var p in packageSameTransportCode)
            //            {
            //                // Add cảnh báo cho kiện hàng
            //                p.Note = $" Order with coincided waybill code: {orderCode}";
            //                //p.OrderCodes = $"{orderCode}";
            //                //p.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
            //                //p.Customers = $"{customers}";
            //                //p.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

            //                if (diffWarehouse)
            //                    p.Note += ", The destination package is different";
            //            }

            //            // Add cảnh báo cho kiện hàng
            //            orderPackage.Note = $" Order with coincided waybill code: {orderCode}";
            //            //orderPackage.OrderCodes = $"{orderCode}";
            //            //orderPackage.OrderCodesUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList())};";
            //            //orderPackage.Customers = $"{customers}";
            //            //orderPackage.CustomersUnsigned = $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

            //            if (diffWarehouse)
            //                orderPackage.Note += ", The destination package is different";
            //        }

            //        //await UnitOfWork.OrderPackageRepo.SaveAsync();
            //    }
            //}

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Delivery)]
        public async Task<ActionResult> SuggetionForDelivery(string packageCodes, string term, byte type = 0, int size = 6)
        {
            // Chỉ để dạng ký tự không dấu
            term = MyCommon.Ucs2Convert(term);

            // Chỉ lấy mã dạng số: (vd: ODR005566 chỉ lấy 005566)
            // term = Regex.Replace(term, "[^0-9]", "");

            var warehouseIdPath = UserState.OfficeIdPath;

            var items = await UnitOfWork.OrderPackageRepo.FindAsync(
                x => x.Status == (byte)OrderPackageStatus.InStock && !x.IsDelete && x.OrderId != 0 &&
                     (x.WarehouseIdPath == warehouseIdPath || x.WarehouseIdPath.StartsWith(warehouseIdPath + ".")) &&
                     x.TransportCode != "" && ((type == 0 && x.OrderCode.Contains(term)) || (type == 1 && x.Code.Contains(term))
                      || (type == 2 && x.TransportCode.Contains(term)) || (type == 3 && x.CustomerUserName.Contains(term))
                      || (type == 4 && x.UnsignedText.Contains(term))) &&
                     (packageCodes == "" || !packageCodes.Contains(";" + x.Code + ";")), x => x.OrderBy(y => y.Id), 1,
                size);

            var packagesResults = items.Select(Mapper.Map<OrderPackageForDeliveryResult>).ToList();

            foreach (var package in packagesResults)
            {
                var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsNoTrackingAsync(x => x.Id == package.OrderId);
                var orderAddress = await UnitOfWork.OrderAddressRepo.SingleOrDefaultAsNoTrackingAsync(x => x.Id == order.ToAddressId);
                var orderExchange = await
                        UnitOfWork.OrderExchangeRepo.SingleOrDefaultAsNoTrackingAsync(
                            x =>
                                x.OrderId == package.OrderId && !x.IsDelete &&
                                x.Status == (byte)OrderExchangeStatus.Approved &&
                                x.Type == (byte)OrderExchangeType.Product && x.Mode == (byte)OrderExchangeMode.Export);


                package.CustomerWeight = order.TotalWeight;
                package.CustomerDebt = order.Total - orderExchange.TotalPrice ?? 0;
                package.CustomerFullName = order.CustomerName;
                package.CustomerPhone = order.CustomerPhone;
                package.CustomerAddress = orderAddress.Address;
            }

            return JsonCamelCaseResult(packagesResults, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Delivery)]
        public async Task<ActionResult> GetByOrderCodeForDelivery(string codeOrder)
        {
            var items = await UnitOfWork.OrderPackageRepo.GetForDelivery(codeOrder);

            var packagesResults = items.Select(Mapper.Map<OrderPackageForDeliveryResult>).ToList();

            foreach (var package in packagesResults)
            {
                var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsNoTrackingAsync(x => x.Id == package.OrderId);
                var orderAddress = await UnitOfWork.OrderAddressRepo.SingleOrDefaultAsNoTrackingAsync(x => x.Id == order.ToAddressId);

                // Tiền khách đã thanh toán cho Order
                var payedMoney = await
                    UnitOfWork.OrderExchangeRepo.Entities.Where(
                            x =>
                                x.OrderId == package.OrderId && !x.IsDelete &&
                                x.Status == (byte)OrderExchangeStatus.Approved && x.Mode == (byte)OrderExchangeMode.Export)
                        .SumAsync(x => x.TotalPrice ?? 0);


                package.CustomerWeight = order.TotalWeight;
                package.CustomerDebt = order.Total - payedMoney;
                // Xử lý giá trị nhỏ hơn 1 Baht
                package.CustomerDebt = package.CustomerDebt < 1 ? 0 : package.CustomerDebt;
                package.CustomerFullName = order.CustomerName;
                package.CustomerPhone = order.CustomerPhone;
                package.CustomerAddress = orderAddress.Address;
            }

            return JsonCamelCaseResult(packagesResults, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy ra tất các package theo Order code
        /// </summary>
        /// <param name="codeOrder"></param>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.ExportWarehouse)]
        public async Task<ActionResult> GetByOrderCodeForExportWarehouse(string codeOrder)
        {
            var items = await UnitOfWork.OrderPackageRepo.OrderPackageByOrderCode(codeOrder);

            var packagesResults = items.Select(Mapper.Map<OrderPackageForDeliveryResult>).ToList();

            foreach (var package in packagesResults)
            {
                var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsNoTrackingAsync(x => x.Id == package.OrderId);
                var orderAddress = await UnitOfWork.OrderAddressRepo.SingleOrDefaultAsNoTrackingAsync(x => x.Id == order.ToAddressId);

                package.CustomerWeight = order.TotalWeight;
                package.CustomerFullName = order.CustomerName;
                package.CustomerPhone = order.CustomerPhone;
                package.CustomerAddress = orderAddress.Address;
            }

            return JsonCamelCaseResult(packagesResults, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchModal"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAllPackageList(int page, int pageSize, OrderProcessItemSearchModel searchModal)
        {
            var packageModal = new List<OrderProcessItemSearchModel>();

            if (searchModal == null)
            {
                searchModal = new OrderProcessItemSearchModel();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);

            //if (!string.IsNullOrEmpty(searchModal.DateStart))
            //{
            //    var DateStart = DateTime.Parse(searchModal.DateStart);
            //    var DateEnd = DateTime.Parse(searchModal.DateEnd);

            //    packageModal = await UnitOfWork.OrderPackageRepo.FindAsync(
            //        out totalRecord,
            //        x => x.Code.Contains(searchModal.Keyword) && (searchModal.Status == -1 || x.Status == searchModal.Status) && (searchModal.CustomerId == -1 || x.UserId == searchModal.CustomerId) && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId) && x.Created >= DateStart && x.Created <= DateEnd,
            //        x => x.OrderByDescending(y => y.Created),
            //        page,
            //        pageSize
            //    );
            //}
            //else
            //{
            //    packageModal = await UnitOfWork.ImportWarehouseRepo.FindAsync(
            //        out totalRecord,
            //        x => x.Code.Contains(searchModal.Keyword) && (searchModal.Status == -1 || x.Status == searchModal.Status) && (searchModal.UserId == -1 || x.UserId == searchModal.UserId) && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId),
            //        x => x.OrderByDescending(y => y.Created),
            //        page,
            //        pageSize
            //    );
            //}

            return Json(new { packageModal }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPackageSearchData()
        {
            var listStatus = new List<dynamic>() { new { Text ="All", Value = -1 } };
            var listWarehouse = new List<SearchMeta>();
            var listCustomer = new List<SearchMeta>();

            // Lấy các trạng thái Status
            foreach (ImportWarehouseStatus importWarehouseStatus in Enum.GetValues(typeof(ImportWarehouseStatus)))
            {
                if (importWarehouseStatus >= 0)
                {
                    listStatus.Add(new { Text = importWarehouseStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)importWarehouseStatus });
                }
            }

            // Lấy danh sách các kho
            var warehouse = UnitOfWork.WarehouseRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempWarehouseList = from p in warehouse
                                    select new SearchMeta() { Text = p.Name, Value = p.Id };
            listWarehouse.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listWarehouse.AddRange(tempWarehouseList.ToList());

            // Lấy danh sách người tạo phiếu - Danh sách nhân viên trong công ty
            var customer = UnitOfWork.CustomerRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempCustomerList = from p in customer
                                   select new SearchMeta() { Text = p.FullName, Value = p.Id };

            listCustomer.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listCustomer.AddRange(tempCustomerList.ToList());

            return Json(new { listStatus, listWarehouse, listCustomer }, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> GetPackageDetail(int packageId)
        {
            var result = true;

            var packageModal = await UnitOfWork.ImportWarehouseRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == packageId);
            if (packageModal == null)
            {
                result = false;
            }

            return Json(new { result, packageModal }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatus()
        {
            var status = Enum.GetValues(typeof(OrderPackageStatus)).Cast<OrderPackageStatus>().Select(v => new { id = (byte)v, name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) }).ToList();

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Approvel, EnumPage.TrackingPackage)]
        public async Task<ActionResult> Transfer(int packageId, int orderId, string note)
        {
            OrderPackage package;
            List<Order> orders;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    package = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(
                    x => x.Id == packageId && x.IsDelete == false);

                    if (package == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "package does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);

                    if (package.Status >= (byte)OrderPackageStatus.WaitDelivery)
                        return JsonCamelCaseResult(new { Status = -2, Text = "Unable to edit the package of which delivery note has been created" }, JsonRequestBehavior.AllowGet);

                    if(package.OrderId == orderId)
                        return JsonCamelCaseResult(new { Status = -2, Text = "The package has already been in this order" }, JsonRequestBehavior.AllowGet);

                    // Chỉ có đặt hàng mới có quyền thực hiện thao tác này
                    if (package.OrderType == (byte) OrderType.Order && UserState.OfficeType != (byte) OfficeType.Order
                        || package.OrderType == (byte) OrderType.Deposit &&
                        UserState.OfficeType != (byte) OfficeType.Deposit)
                        return Json(new {status = -1, text = "Only ordering employee has permission to perform this action"},
                            JsonRequestBehavior.AllowGet);

                    var orderTaget =
                        await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == orderId);

                    if(orderTaget == null)
                        return JsonCamelCaseResult(new { Status = -2, Text = "Destination order does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);

                    var orderFrom = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == package.OrderId);

                    if (orderFrom == null)
                        return JsonCamelCaseResult(new { Status = -2, Text = "Old order does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);

                    var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == orderTaget.CustomerId);

                    // Customer does not exist
                    if (customer == null)
                        return Json(new { status = -1, text = "Customer does not exist!" },
                                JsonRequestBehavior.AllowGet);

                    var warehouse = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == orderTaget.WarehouseDeliveryId);

                    // Warehouse does not exist
                    if (warehouse == null)
                        return Json(new { status = -1, text = "Warehouse does not exist" },
                                JsonRequestBehavior.AllowGet);

                    var warehouseOutSide = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == orderTaget.WarehouseId);

                    // Warehouse does not exist
                    if (warehouseOutSide == null)
                        return Json(new { status = -1, text = "Warehouse does not exist" },
                                JsonRequestBehavior.AllowGet);

                    orderTaget.PackageNo += 1;

                    package.OrderId = orderTaget.Id;
                    package.OrderCode = orderTaget.Code;
                    package.OrderType = orderTaget.Type;
                    package.SystemId = orderTaget.SystemId;
                    package.SystemName = orderTaget.SystemName;

                    // Update Package
                    package.CustomerId = customer.Id;
                    package.CustomerName = customer.FullName;
                    package.CustomerUserName = customer.Email;
                    package.CustomerLevelId = customer.LevelId;
                    package.CustomerLevelName = customer.LevelName;
                    package.CustomerWarehouseId = warehouse.Id;
                    package.CustomerWarehouseName = warehouse.Name;
                    package.CustomerWarehouseIdPath = warehouse.IdPath;
                    package.CustomerWarehouseAddress = warehouse.Address;
                    package.LastUpdate = DateTime.Now;
                    package.PackageNo = orderTaget.PackageNo;
                    package.UnsignedText =
                        MyCommon.Ucs2Convert(
                            $"{orderTaget.Code} {MyCommon.ReturnCode(orderTaget.Code)} {orderTaget.UserFullName} {package.Code} P{package.Code} {package.TransportCode}" +
                            $"{orderTaget.CustomerName} {orderTaget.CustomerEmail} {orderTaget.CustomerPhone} {orderTaget.ShopName}");

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    note += $" (transferred from order: \"{orderTaget.Code}\")";

                    // Cập nhật ghi chú khi câp nhật Order của package
                    UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                    {
                        OrderId = package.OrderId,
                        OrderCode = package.OrderCode,
                        PackageId = package.Id,
                        PackageCode = package.Code,
                        UserId = UserState.UserId,
                        UserFullName = UserState.FullName,
                        Time = DateTime.Now,
                        ObjectId = null,
                        ObjectCode = null,
                        Mode = (byte)PackageNoteMode.UpdateOrder,
                        Content = note
                    });

                    await UnitOfWork.ImportWarehouseDetailRepo.SaveAsync();

                    // Cập nhật lại tất các các chỗ khác thông tin của package này

                    var timeNow = DateTime.Now;

                    var strCodeOrder = $";{orderFrom.Code};{orderTaget.Code};";

                    orders = await UnitOfWork.OrderRepo.FindAsync(x => strCodeOrder.Contains(";" + x.Code + ";") && !x.IsDelete);

                    var weight = await UnitOfWork.OrderPackageRepo.SumWeightByOrderCodes(strCodeOrder);

                    foreach (var order in orders)
                    {
                        if (weight.ContainsKey(order.Code))
                        {
                            order.TotalWeight = weight[order.Code];
                        }

                        #region Divide incurred service fees to packages by weight
                        // Tính toán chi phí phát sinh cảu đơn hàng
                        var serviceOthers =
                            await UnitOfWork.OrderServiceOtherRepo.FindAsync(
                                x => x.OrderId == order.Id && x.Type == 0);

                        if (serviceOthers.Any())
                        {
                            var packageFirst = new List<int>();
                            // Cập nhật lại Sum cân nặng của đơn
                            foreach (var serviceOther in serviceOthers)
                            {
                                var packages = UnitOfWork.OrderPackageRepo.GetByOrderIdAndImportWarehouseId(order.Id,
                                        serviceOther.ObjectId);

                                serviceOther.TotalWeightActual = packages.Sum(x => x.WeightActual);

                                // Tính lại cân nặng trong kiên hàng
                                foreach (var p in packages)
                                {
                                    var percent = p.WeightActual * 100 / serviceOther.TotalWeightActual;

                                    if (packageFirst.Any(x => x == p.Id))
                                    {
                                        p.OtherService += percent * serviceOther.TotalPrice / 100;
                                    }
                                    else
                                    {
                                        p.OtherService = percent * serviceOther.TotalPrice / 100;
                                        packageFirst.Add(p.Id);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Update Goods shipping to Vietnam service
                        //var fastDeliveryService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte)OrderServices.FastDelivery && x.Checked);

                        //var optimalService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte) OrderServices.Optimal && x.Checked);

                        var outSideShippingService = await
                                UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                    x => !x.IsDelete && x.OrderId == order.Id &&
                                        x.ServiceId == (byte)OrderServices.OutSideShipping && x.Checked);

                        decimal serviceValue;

                        var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId);

                        // Cân nặng các package được xuất giao tại TQ
                        var orderWeightIgnore = UnitOfWork.OrderPackageRepo.GetTotalActualWeight(order.Id);

                        // Sum cân nặng tính tiền vc của Order
                        var orderWeight = order.TotalWeight - orderWeightIgnore;

                        decimal outSideShipping;

                        // Order ký gửi
                        if (order.Type == (byte)OrderType.Deposit)
                        {
                            serviceValue = order.ApprovelPrice ?? 0;

                            if (orderWeight >= 50)
                            {
                                outSideShipping = orderWeight * serviceValue;
                            }
                            else
                            {
                                outSideShipping = (orderWeight - 1) * serviceValue + 100000;
                            }
                        }
                        else
                        { // Order Order
                          // VC tiết kiệm
                            //if (optimalService != null)
                            //{
                            //    serviceValue = OrderRepository.OptimalDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else if (fastDeliveryService != null) // VC nhanh
                            //{
                            //    serviceValue = OrderRepository.FastDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else // VC bình thường
                            //{
                                serviceValue = OrderRepository.ShippingOutSide(order.ServiceType, orderWeight,
                                    order.WarehouseDeliveryId ?? 0);
                            //}
                            outSideShipping = orderWeight * serviceValue;
                        }

                        if (outSideShippingService == null)
                        {
                            outSideShippingService = new OrderService()
                            {
                                IsDelete = false,
                                Checked = true,
                                Created = timeNow,
                                LastUpdate = timeNow,
                                Value = serviceValue,
                                Currency = Currency.VND.ToString(),
                                ExchangeRate = 0,
                                TotalPrice = outSideShipping,
                                HashTag = string.Empty,
                                Mode = (byte)OrderServiceMode.Required,
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.OutSideShipping,
                                ServiceName =
                                    (OrderServices.OutSideShipping).GetAttributeOfType<DescriptionAttribute>()
                                        .Description,
                                Type = (byte)UnitType.Money,

                            };

                            // Triết khấu Vip cho Order Order
                            if (order.Type == (byte)OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee to Vietnam {serviceValue.ToString("N2", CultureInfo)} Baht/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";

                                UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                            }
                        }
                        else
                        {
                            outSideShippingService.LastUpdate = timeNow;
                            outSideShippingService.Value = serviceValue;
                            outSideShippingService.TotalPrice = outSideShipping;

                            // Triết khấu Vip cho Order Order
                            if (order.Type == (byte)OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee to Vietnam {serviceValue.ToString("N2", CultureInfo)} Baht/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";
                            }
                        }
                        #endregion

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật lại Sum tiền của Order
                        var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                                 x.IsDelete == false && x.Checked)
                            .ToList()
                            .Sum(x => x.TotalPrice);

                        order.Total = order.TotalExchange + totalService;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                        order.LastUpdate = DateTime.Now;

                        await UnitOfWork.OrderRepo.SaveAsync();
                    }

                    //Thêm thông tin kiện đã nhập kho cho Order vừa gắn Order code
                    if (package.OrderId > 0)
                    {
                        var oT = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderTaget.Id);
                        oT.PackageNoInStock = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.OrderId == oT.Id);

                        //Thêm thông tin kiện đã nhập kho cho Order vừa bỏ gắn Order code
                        var oF = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderFrom.Id);
                        oF.PackageNoInStock = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.OrderId == oF.Id);

                        await UnitOfWork.OrderRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            var jobId1 = "";
            // Cập nhật thông tin các bảng liên quan trong đến cân nặng của package
            if (package != null)
            {
                BackgroundJob.Enqueue(() => PackageJob.UpdateOrder(package.Id));
            }

            // Job cập nhật thông tin package
            if (orders != null)
            {
                var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

                var jobId2 = "";

                if (string.IsNullOrWhiteSpace(jobId1))
                {
                    BackgroundJob.Enqueue(() => PackageJob.UpdateWeightActualPercent(orders.Select(x => x.Id).ToList()));
                }
                else
                {
                    jobId2 = BackgroundJob.ContinueWith(jobId1, () => PackageJob.UpdateWeightActualPercent(orders.Select(x => x.Id).ToList()));
                }

                if (!string.IsNullOrWhiteSpace(jobId2))
                {
                    BackgroundJob.ContinueWith(jobId2, () => OrderJob.ProcessDebitReport(orderIds));
                }
                else if (!string.IsNullOrWhiteSpace(jobId1))
                {
                    BackgroundJob.ContinueWith(jobId1, () => OrderJob.ProcessDebitReport(orderIds));
                }
                else
                {
                    BackgroundJob.Enqueue(() => OrderJob.ProcessDebitReport(orderIds));
                }

            }

            return JsonCamelCaseResult(new { Status = 1, Text = "Package transferred successfully" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhật lại cân nặng của package
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.TrackingPackage)]
        public async Task<ActionResult> UpdateWeight(PackageUpdateInfoMeta model)
        {
            OrderPackage package;
            List<Order> orders;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    package = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(
                    x => x.Id == model.PackageId && x.IsDelete == false);

                    if (package == null)
                        return JsonCamelCaseResult(new {Status = -1, Text = "Goods package does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    if (package.Status >= (byte) OrderPackageStatus.Completed)
                        return JsonCamelCaseResult(
                                new {Status = -2, Text = "Unable to edit the package of which delivery note has been created"},
                                JsonRequestBehavior.AllowGet);


                    var weightConverted = Math.Round((package.Length ?? 0) * (package.Width ?? 0) * (package.Height ?? 0) / 5000, 2);
                    var size = $"{package.Length.Value:N2}x{package.Width.Value:N2}x{package.Height.Value:N2}";

                    string note = "(";
                    // Bổ xung note kích thước trước sau cập nhật
                    if (package.Size != size)
                    {
                        note += $"Size: {package.Size} -> {size}";
                    }

                    // Bổ xung note kích cân nặng trước và sau cập nhật
                    if (package.Weight != model.Weight)
                    {
                        note += $"weight: {package.Weight} -> {model.Weight}";
                    }

                    // Bổ xung note kích cân nặng trước và sau cập nhật
                    if (package.WeightConverted != weightConverted)
                    {
                        note += $"Weight converted: {package.WeightConverted} -> {weightConverted}";
                    }
                    note += ")";

                    package.Weight = model.Weight;
                    package.Length = model.Length;
                    package.Width = model.Width;
                    package.Height = model.Height;
                    package.WeightConverted = weightConverted;
                    package.WeightActual = package.Weight > package.WeightConverted ? package.Weight : package.WeightConverted;
                    package.Size = size;
                    package.Volume = Math.Round(package.Length.Value * package.Width.Value * package.Height.Value / 1000000, 4);
                    package.VolumeActual = package.Volume;

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    var timeNow = DateTime.Now;

                    var strCodeOrder = $";{package.OrderCode};";

                    orders = await UnitOfWork.OrderRepo.FindAsync(x => strCodeOrder.Contains(";" + x.Code + ";") && !x.IsDelete);

                    var weight = await UnitOfWork.OrderPackageRepo.SumWeightByOrderCodes(strCodeOrder);

                    foreach (var order in orders)
                    {
                        if (weight.ContainsKey(order.Code))
                        {
                            order.TotalWeight = weight[order.Code];
                        }

                        #region Divide incurred service fees to packages by weight
                        // Tính toán chi phí phát sinh cảu đơn hàng
                        var serviceOthers =
                            await UnitOfWork.OrderServiceOtherRepo.FindAsync(
                                x => x.OrderId == order.Id && x.Type == 0);

                        if (serviceOthers.Any())
                        {
                            var packageFirst = new List<int>();
                            // Cập nhật lại Sum cân nặng của đơn
                            foreach (var serviceOther in serviceOthers)
                            {
                                var packages = UnitOfWork.OrderPackageRepo.GetByOrderIdAndImportWarehouseId(order.Id,
                                        serviceOther.ObjectId);

                                serviceOther.TotalWeightActual = packages.Sum(x => x.WeightActual);

                                // Tính lại cân nặng trong kiên hàng
                                foreach (var p in packages)
                                {
                                    var percent = p.WeightActual * 100 / serviceOther.TotalWeightActual;

                                    if (packageFirst.Any(x => x == p.Id))
                                    {
                                        p.OtherService += percent * serviceOther.TotalPrice / 100;
                                    }
                                    else
                                    {
                                        p.OtherService = percent * serviceOther.TotalPrice / 100;
                                        packageFirst.Add(p.Id);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Update Goods shipping to Vietnam service
                        //var fastDeliveryService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte)OrderServices.FastDelivery && x.Checked);

                        //var optimalService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte) OrderServices.Optimal && x.Checked);

                        var outSideShippingService = await
                                UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                    x => !x.IsDelete && x.OrderId == order.Id &&
                                        x.ServiceId == (byte)OrderServices.OutSideShipping && x.Checked);

                        decimal serviceValue;

                        var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId);

                        // Cân nặng các package được xuất giao tại TQ
                        var orderWeightIgnore = UnitOfWork.OrderPackageRepo.GetTotalActualWeight(order.Id);

                        // Sum cân nặng tính tiền vc của Order
                        var orderWeight = order.TotalWeight - orderWeightIgnore;

                        decimal outSideShipping;

                        // Order ký gửi
                        if (order.Type == (byte)OrderType.Deposit)
                        {
                            serviceValue = order.ApprovelPrice ?? 0;

                            if (orderWeight >= 50)
                            {
                                outSideShipping = orderWeight * serviceValue;
                            }
                            else
                            {
                                outSideShipping = (orderWeight - 1) * serviceValue + 100000;
                            }
                        }
                        else
                        { // Order Order
                          // VC tiết kiệm
                            //if (optimalService != null)
                            //{
                            //    serviceValue = OrderRepository.OptimalDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else if (fastDeliveryService != null) // VC nhanh
                            //{
                            //    serviceValue = OrderRepository.FastDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else // VC bình thường
                            //{
                                serviceValue = OrderRepository.ShippingOutSide(order.ServiceType, orderWeight,
                                    order.WarehouseDeliveryId ?? 0);
                            //}

                            outSideShipping = orderWeight * serviceValue;
                        }

                        if (outSideShippingService == null)
                        {
                            outSideShippingService = new OrderService()
                            {
                                IsDelete = false,
                                Checked = true,
                                Created = timeNow,
                                LastUpdate = timeNow,
                                Value = serviceValue,
                                Currency = Currency.VND.ToString(),
                                ExchangeRate = 0,
                                TotalPrice = outSideShipping,
                                HashTag = string.Empty,
                                Mode = (byte)OrderServiceMode.Required,
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.OutSideShipping,
                                ServiceName =
                                    (OrderServices.OutSideShipping).GetAttributeOfType<DescriptionAttribute>()
                                        .Description,
                                Type = (byte)UnitType.Money,

                            };

                            // Triết khấu Vip cho Order Order
                            if (order.Type == (byte)OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee to Vietnam {serviceValue.ToString("N2", CultureInfo)} Baht/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";

                                UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                            }
                        }
                        else
                        {
                            outSideShippingService.LastUpdate = timeNow;
                            outSideShippingService.Value = serviceValue;
                            outSideShippingService.TotalPrice = outSideShipping;

                            // Triết khấu Vip cho Order Order
                            if (order.Type == (byte)OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee to Vietnam {serviceValue.ToString("N2", CultureInfo)} Baht/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";
                            }
                        }
                        #endregion

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật lại Sum tiền của Orders
                        var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                                 x.IsDelete == false && x.Checked)
                            .ToList()
                            .Sum(x => x.TotalPrice);

                        order.Total = order.TotalExchange + totalService;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                        order.LastUpdate = DateTime.Now;

                        // Cập nhật ghi chú khi câp nhật Orders của package
                        UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                        {
                            OrderId = package.OrderId,
                            OrderCode = package.OrderCode,
                            PackageId = package.Id,
                            PackageCode = package.Code,
                            UserId = UserState.UserId,
                            UserFullName = UserState.FullName,
                            Time = DateTime.Now,
                            ObjectId = null,
                            ObjectCode = null,
                            Mode = (byte)PackageNoteMode.UpdateWeight,
                            Content = model.Note + note
                        });

                        await UnitOfWork.OrderRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            // Cập nhật thông tin các bảng liên quan trong đến cân nặng của package
            var jobId1 = BackgroundJob.Enqueue(() => PackageJob.UpdateWeight(package.Id));

            // Job cập nhật thông tin package
            var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

            var jobId2 = BackgroundJob.ContinueWith(jobId1, () => PackageJob.UpdateWeightActualPercent(orders.Select(x => x.Id).ToList()));

            BackgroundJob.ContinueWith(jobId2, () => OrderJob.ProcessDebitReport(orderIds));

            // Cập nhật thông tin các bảng liên quan trong đến cân nặng của package
            //if (package != null)
            //BackgroundJob.Enqueue(() => PackageJob.UpdateWeight(package.Id));

            //// Job cập nhật thông tin package
            //if(orders != null)
            //BackgroundJob.Enqueue(() => PackageJob.UpdateWeightActualPercent(orders.Select(x => x.Id).ToList()));

            return JsonCamelCaseResult(new {Status = 1, Text = "Weight updated successfully" }, JsonRequestBehavior.AllowGet);
        }

        // Cập nhật lại tiền cân nặng trong các Order - Cập nhật lại tiền dịch vụ VC về VN
        public async Task<ActionResult> FixService()
        {
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {


                    // lấy ra tất cả các Order đã có cân nặng
                    var orders =
                        await UnitOfWork.OrderRepo.FindAsync(
                            x =>
                                x.IsDelete == false && ";913;1052;1053;1490;1492;1505;1637;1685;1704;1743;2178;4878;4908;".Contains(";" + x.Id + ";") &&
                                (x.Type != (byte) OrderType.Deposit && x.Status >= (byte) OrderStatus.InWarehouse ||
                                x.Type == (byte) OrderType.Deposit && x.Status >= (byte) DepositStatus.InWarehouse));

                    var timeNow = DateTime.Now;

                    foreach (var order in orders)
                    {
                        var totalWeight =
                            UnitOfWork.OrderPackageRepo.Entities.Where(
                                    x => x.IsDelete == false && x.OrderId == order.Id && x.WeightActual != null)
                                .Select(x => x.WeightActual)
                                .ToList()
                                .Sum(x => x);

                        order.TotalWeight = totalWeight ?? 0;

                        // Tính lại tiền dịch vụ VC hàng về VN

                        #region Update Goods shipping to Vietnam service
                        //var fastDeliveryService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte)OrderServices.FastDelivery && x.Checked);

                        //var optimalService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte) OrderServices.Optimal && x.Checked);

                        var outSideShippingService = await
                                UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                    x => !x.IsDelete && x.OrderId == order.Id &&
                                        x.ServiceId == (byte)OrderServices.OutSideShipping && x.Checked);

                        decimal serviceValue;

                        var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId);

                        // Cân nặng các package được xuất giao tại TQ
                        var orderWeightIgnore = UnitOfWork.OrderPackageRepo.GetTotalActualWeight(order.Id);

                        // Sum cân nặng tính tiền vc của Order
                        var orderWeight = order.TotalWeight - orderWeightIgnore;

                        decimal outSideShipping;

                        // Order ký gửi
                        if (order.Type == (byte)OrderType.Deposit)
                        {
                            serviceValue = order.ApprovelPrice ?? 0;

                            if (orderWeight >= 50)
                            {
                                outSideShipping = orderWeight * serviceValue;
                            }
                            else
                            {
                                outSideShipping = (orderWeight - 1) * serviceValue + 100000;
                            }
                        }
                        else
                        { // Order Order
                          // VC tiết kiệm
                            //if (optimalService != null)
                            //{
                            //    serviceValue = OrderRepository.OptimalDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else if (fastDeliveryService != null) // VC nhanh
                            //{
                            //    serviceValue = OrderRepository.FastDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else // VC bình thường
                            //{
                                serviceValue = OrderRepository.ShippingOutSide(order.ServiceType, orderWeight,
                                    order.WarehouseDeliveryId ?? 0);
                            //}

                            outSideShipping = orderWeight * serviceValue;
                        }

                        if (outSideShippingService == null)
                        {
                            outSideShippingService = new OrderService()
                            {
                                IsDelete = false,
                                Checked = true,
                                Created = timeNow,
                                LastUpdate = timeNow,
                                Value = serviceValue,
                                Currency = Currency.VND.ToString(),
                                ExchangeRate = 0,
                                TotalPrice = outSideShipping,
                                HashTag = string.Empty,
                                Mode = (byte)OrderServiceMode.Required,
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.OutSideShipping,
                                ServiceName =
                                    (OrderServices.OutSideShipping).GetAttributeOfType<DescriptionAttribute>()
                                        .Description,
                                Type = (byte)UnitType.Money,

                            };

                            // Triết khấu Vip cho Order Order
                            if (order.Type == (byte)OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee to Vietnam {serviceValue.ToString("N2", CultureInfo)} Baht/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";

                                UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                            }
                        }
                        else
                        {
                            outSideShippingService.LastUpdate = timeNow;
                            outSideShippingService.Value = serviceValue;
                            outSideShippingService.TotalPrice = outSideShipping;

                            // Triết khấu Vip cho Order Order
                            if (order.Type == (byte)OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee to Vietnam {serviceValue.ToString("N2", CultureInfo)} Baht/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";
                            }
                        }
                        #endregion

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật lại Sum tiền của Order
                        var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                                 x.IsDelete == false && x.Checked)
                            .ToList()
                            .Sum(x => x.TotalPrice);

                        order.Total = order.TotalExchange + totalService;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                        order.LastUpdate = DateTime.Now;
                    }

                    await UnitOfWork.OrderRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> FixSameTransportCode()
        {
            var packages = await UnitOfWork.OrderPackageRepo.Entities.Where(
                        x => x.IsDelete == false && x.OrderId > 0)
                    .GroupBy(x => x.TransportCode)
                    .Select(x => x.OrderByDescending(p => p.Id).FirstOrDefault())
                    .ToListAsync();

            foreach (var p in packages)
            {
                PackageJob.UpdateSameTransportCode(p.TransportCode, p.Code, UnitOfWork, UserState);
            }

            return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ReProcessWeightOfOrder()
        {
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction()) { 
                try
                {
                    var timeNow = DateTime.Now;
                    var packageCodes = ";186160317;492140317;229160317;";

                    var listPackages = await UnitOfWork.OrderPackageRepo.FindAsync(
                            x => x.IsDelete == false && packageCodes.Contains(";" + x.Code + ";"));

                    foreach (var p in listPackages)
                    {
                        p.IsDelete = true;
                        p.LastUpdate = timeNow;
                    }

                    var strCodeOrder = $";{string.Join(";", listPackages.Select(x => x.OrderCode).Distinct().ToList())};";

                    //Cập nhật lại tiền khi xóa kiện
                    //var strCodeOrder = $";361002171;";

                    var orders =
                        await UnitOfWork.OrderRepo.FindAsync(
                            x => strCodeOrder.Contains(";" + x.Code + ";") && !x.IsDelete);

                    var weight = await UnitOfWork.OrderPackageRepo.SumWeightByOrderCodes(strCodeOrder);
                    

                    foreach (var order in orders)
                    {
                        if (weight.ContainsKey(order.Code))
                        {
                            order.TotalWeight = weight[order.Code];
                        }

                        #region Divide incurred service fees to packages by weight

                        // Tính toán chi phí phát sinh cảu Order
                        var serviceOthers =
                            await UnitOfWork.OrderServiceOtherRepo.FindAsync(
                                x => x.OrderId == order.Id && x.Type == 0);

                        if (serviceOthers.Any())
                        {
                            var packageFirst = new List<int>();
                            // Cập nhật lại Sum cân nặng của đơn
                            foreach (var serviceOther in serviceOthers)
                            {
                                var packages = UnitOfWork.OrderPackageRepo.GetByOrderIdAndImportWarehouseId(order.Id,
                                    serviceOther.ObjectId);

                                serviceOther.TotalWeightActual = packages.Sum(x => x.WeightActual);

                                // Tính lại cân nặng trong kiên hàng
                                foreach (var p in packages)
                                {
                                    var percent = p.WeightActual * 100 / serviceOther.TotalWeightActual;

                                    if (packageFirst.Any(x => x == p.Id))
                                    {
                                        p.OtherService += percent * serviceOther.TotalPrice / 100;
                                    }
                                    else
                                    {
                                        p.OtherService = percent * serviceOther.TotalPrice / 100;
                                        packageFirst.Add(p.Id);
                                    }
                                }
                            }
                        }

                        #endregion

                        #region Update Goods shipping to Vietnam service

                        //var fastDeliveryService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte) OrderServices.FastDelivery && x.Checked);

                        //var optimalService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte) OrderServices.Optimal && x.Checked);

                        var outSideShippingService = await
                            UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => !x.IsDelete && x.OrderId == order.Id &&
                                     x.ServiceId == (byte) OrderServices.OutSideShipping && x.Checked);

                        decimal serviceValue;

                        var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId);

                        // Cân nặng các package được xuất giao tại TQ
                        var orderWeightIgnore = UnitOfWork.OrderPackageRepo.GetTotalActualWeight(order.Id);

                        // Sum cân nặng tính tiền vc của Order
                        var orderWeight = order.TotalWeight - orderWeightIgnore;

                        decimal outSideShipping;

                        // Order ký gửi
                        if (order.Type == (byte) OrderType.Deposit)
                        {
                            serviceValue = order.ApprovelPrice ?? 0;

                            if (orderWeight >= 50)
                            {
                                outSideShipping = orderWeight * serviceValue;
                            }
                            else
                            {
                                outSideShipping = (orderWeight - 1) * serviceValue + 100000;
                            }
                        }
                        else // Order Order
                        {
                            // VC tiết kiệm
                            //if (optimalService != null)
                            //{
                            //    serviceValue = OrderRepository.OptimalDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else if (fastDeliveryService != null) // VC nhanh
                            //{
                            //    serviceValue = OrderRepository.FastDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else // VC bình thường
                            //{
                                serviceValue = OrderRepository.ShippingOutSide(order.ServiceType, orderWeight,
                                    order.WarehouseDeliveryId ?? 0);
                            //}

                            outSideShipping = orderWeight * serviceValue;
                        }

                        if (outSideShippingService == null)
                        {
                            outSideShippingService = new OrderService()
                            {
                                IsDelete = false,
                                Checked = true,
                                Created = timeNow,
                                LastUpdate = timeNow,
                                Value = serviceValue,
                                Currency = Currency.VND.ToString(),
                                ExchangeRate = 0,
                                TotalPrice = outSideShipping,
                                HashTag = string.Empty,
                                Mode = (byte) OrderServiceMode.Required,
                                OrderId = order.Id,
                                ServiceId = (byte) OrderServices.OutSideShipping,
                                ServiceName =
                                    (OrderServices.OutSideShipping).GetAttributeOfType<DescriptionAttribute>()
                                    .Description,
                                Type = (byte) UnitType.Money,

                            };

                            // Triết khấu Vip cho Order Order
                            if (order.Type == (byte) OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee to Vietnam {serviceValue.ToString("N0", CultureInfo)} vnd/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";

                                UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                            }
                        }
                        else
                        {
                            outSideShippingService.LastUpdate = timeNow;
                            outSideShippingService.Value = serviceValue;
                            outSideShippingService.TotalPrice = outSideShipping;

                            // Triết khấu Vip cho Order Order
                            if (order.Type == (byte) OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee to Vietnam {serviceValue.ToString("N0", CultureInfo)} vnd/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";
                            }
                        }

                        #endregion

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật lại Sum tiền của Orders
                        var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                                 x.IsDelete == false && x.Checked)
                            .ToList()
                            .Sum(x => x.TotalPrice);

                        order.PackageNo = await UnitOfWork.OrderPackageRepo.CountAsync(x => x.IsDelete == false && x.OrderId == order.Id);
                        order.PackageNoInStock = await UnitOfWork.OrderPackageRepo.CountAsync(x => x.IsDelete == false && x.OrderId == order.Id && x.Status >= (byte)OrderPackageStatus.ChinaReceived);

                        order.Total = order.TotalExchange + totalService;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                        order.LastUpdate = DateTime.Now;

                        await UnitOfWork.OrderRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return JsonCamelCaseResult("Successful", JsonRequestBehavior.AllowGet);
        }
    }
}