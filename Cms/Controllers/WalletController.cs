using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using Library.DbContext.Results;
using Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using Hangfire;
using Library.Jobs;
using Common.FunctionResult;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.Drawing;
using Cms.Jobs;

namespace Cms.Controllers
{
    [Authorize]
    public class WalletController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.Wallet)]
        public async Task<ActionResult> Index()
        {
            var isManager = UserState.Type != null && ((UserState.Type.Value == 2) || (UserState.Type.Value == 1));

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
                        (x.Type == (byte) OfficeType.Warehouse) &&
                        !x.IsDelete && (x.Status == (byte) OfficeStatus.Use));

                ViewBag.Warehouses = JsonConvert.SerializeObject(warehouses.Select(Mapper.Map<OfficeDropdownResult>),
                    jsonSerializerSettings);
            }

            var allWarehouse = await
                UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete && (x.Type == (byte) OfficeType.Warehouse) && (x.Status == (byte) OfficeStatus.Use));

            ViewBag.AllWarehouses =
                JsonConvert.SerializeObject(allWarehouse.Select(x => new {x.Id, x.Name, x.IdPath, x.Address}).ToList(),
                    jsonSerializerSettings);

            ViewBag.OrderServices =
                JsonConvert.SerializeObject(new List<object>()
                    {
                        //new { ServiceId = (byte)OrderServices.FastDelivery, ServiceName = "Chuyển nhanh" },
                        new { ServiceId = (byte)OrderServices.Packing, ServiceName = "packing" },
                        new { ServiceId = (byte)OrderServices.Audit, ServiceName = "Tally" },
                        //new { ServiceId = (byte)OrderServices.Optimal, ServiceName = "Optimal" }
                    },
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

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderPackageStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderType>((int)v)),
                jsonSerializerSettings);

            ViewBag.Entrepots = JsonConvert.SerializeObject(
                await UnitOfWork.EntrepotRepo.FindAsync(x => x.IsDelete == false && x.Status == 1),
                jsonSerializerSettings);

            ViewBag.WalletStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.Wallet)]
        public async Task<ActionResult> Search(string warehouseIdPath, byte? orderServiceId, byte? timeType, byte? type, int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, int? entrepotId, string keyword = "", int currentPage = 1, int recordPerPage = 20, byte mode = 0)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            long totalRecord = 0;

            // Query bao hàng được tạo trong kho
            Expression<Func<Wallet, bool>> queryCreated =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                        (type == null || x.Mode == type) && (entrepotId == null || x.EntrepotId == entrepotId) &&
                     !x.IsDelete && ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && ((x.CreatedWarehouseIdPath == warehouseIdPath) ||
                                     x.CreatedWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.CreatedWarehouseIdPath == warehouseIdPath))) &&
                      (orderServiceId == null || x.OrderServices.Contains(";" + orderServiceId + ";"));

            // Query bao hàng đang trong kho
            Expression<Func<Wallet, bool>> queryInStock =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && (type == null || x.Mode == type) && (entrepotId == null || x.EntrepotId == entrepotId) &&
                     ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager &&
                       ((x.CurrentWarehouseIdPath == warehouseIdPath) ||
                        x.CurrentWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.CurrentWarehouseIdPath == warehouseIdPath))) &&
                      (orderServiceId == null || x.OrderServices.Contains(";" + orderServiceId + ";"));

            // Query chờ nhập kho
            Expression<Func<Wallet, bool>> queryWaitImport =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && (type == null || x.Mode == type) && (entrepotId == null || x.EntrepotId == entrepotId) &&
                     ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && x.Status == (byte)WalletStatus.Shipping && ((x.TargetWarehouseIdPath == warehouseIdPath) || x.TargetWarehouseIdPath.StartsWith(warehouseIdPath + "."))) 
                     || (!isManager && x.Status == (byte)WalletStatus.Shipping && (x.TargetWarehouseIdPath == warehouseIdPath))) &&
                     (orderServiceId == null || x.OrderServices.Contains(";" + orderServiceId + ";"));

            // Query chờ nhập kho
            Expression<Func<Wallet, bool>> queryAll =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && (type == null || x.Mode == type) && (entrepotId == null || x.EntrepotId == entrepotId) &&
                     ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && ((x.CreatedWarehouseIdPath == warehouseIdPath)
                                     || x.CreatedWarehouseIdPath.StartsWith(warehouseIdPath + ".")))
                      || (!isManager && (x.CreatedWarehouseIdPath == warehouseIdPath)) ||
                      (isManager && ((x.TargetWarehouseIdPath == warehouseIdPath)
                                     || x.TargetWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.TargetWarehouseIdPath == warehouseIdPath)) ||
                      (isManager && ((x.CurrentWarehouseIdPath == warehouseIdPath)
                                     || x.CurrentWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.CurrentWarehouseIdPath == warehouseIdPath))) &&
                      (orderServiceId == null || x.OrderServices.Contains(";" + orderServiceId + ";"));

            Expression<Func<WalletResult, bool>> query = x => timeType == null && (
                fromDate == null && toDate == null
                || fromDate != null && toDate != null && x.Created >= fromDate && x.Created <= toDate
                || fromDate == null && toDate.HasValue && x.Created <= toDate
                || toDate == null && fromDate.HasValue && x.Created >= fromDate)
                || timeType != null && 
                (fromDate == null && toDate == null
                || fromDate != null && toDate != null && x.ImportedTime >= fromDate && x.ImportedTime <= toDate
                || fromDate == null && toDate.HasValue && x.ImportedTime <= toDate
                || toDate == null && fromDate.HasValue && x.ImportedTime >= fromDate);

            // Kho tạo bao
            List <WalletResult> wallets = new List<WalletResult>();
            if (mode == 0)
                wallets = await UnitOfWork.WalletRepo.SearchAsync(queryCreated, query, currentPage, recordPerPage, out totalRecord);

            // Bao đang trong kho
            if (mode == 1)
                wallets = await UnitOfWork.WalletRepo.SearchAsync(queryInStock, query, currentPage, recordPerPage, out totalRecord);

            // Chờ nhập kho
            if (mode == 2)
                wallets = await UnitOfWork.WalletRepo.SearchAsync(queryWaitImport, query, currentPage, recordPerPage, out totalRecord);

            // Tất cả
            if (mode == 3)
                wallets = await UnitOfWork.WalletRepo.SearchAsync(queryAll, query, currentPage, recordPerPage, out totalRecord);

            // Count group
            var createdNo = await UnitOfWork.WalletRepo.CountAsync(queryCreated, query);
            var inStockNo = await UnitOfWork.WalletRepo.CountAsync(queryInStock, query);
            var waitImportNo = await UnitOfWork.WalletRepo.CountAsync(queryWaitImport, query);
            var allNo = await UnitOfWork.WalletRepo.CountAsync(queryAll, query);

            return JsonCamelCaseResult(
                new
                {
                    items = wallets,
                    totalRecord,
                    mode =
                    new
                    {
                        createdNo,
                        inStockNo,
                        waitImportNo,
                        allNo
                    }
                }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Package, EnumPage.ImportWarehouse, EnumPage.Wallet, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> GetDetail(int id)
        {
            var data = await UnitOfWork.WalletRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            return JsonCamelCaseResult(data, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Wallet)]
        public async Task<ActionResult> GetPackages(int id)
        {
            var wallets = await UnitOfWork.WalletRepo.SingleOrDefaultAsNoTrackingAsync(x => (x.Id == id) && !x.IsDelete);

            if (wallets == null)
                return JsonCamelCaseResult(new {Status = -1, Text = "Package does not exist or has been deleted"},
                    JsonRequestBehavior.AllowGet);

            //if (UserState.OfficeId.HasValue && importWarehouse.WarehouseId != UserState.OfficeId.Value)
            //    return JsonCamelCaseResult(new { Status = -2, Text = "Bạn không phải là nhân viên kho này" }, JsonRequestBehavior.AllowGet);

            var items = await UnitOfWork.WalletDetailRepo.Search(id);

            var packageIds = $";{string.Join(";", items.Select(x => x.PackageId).ToList())};";

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
                orderPackage.PackageNotes = packageNotes.ContainsKey(orderPackage.PackageId)
                    ? packageNotes[orderPackage.PackageId]
                    : null;
                orderPackage.PackageNoteMode = packageNoteMode;
            }

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Suggetion cho nhập bao
        /// </summary>
        /// <param name="packageCodes"></param>
        /// <param name="term"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.ImportWarehouse, EnumPage.Package, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> Suggetion(string packageCodes, string term, int size = 6, bool isPaste = false)
        {
            // Chỉ để dạng ký tự không dấu
            term = MyCommon.Ucs2Convert(term);

            // Chỉ lấy mã dạng số: (vd: ODR005566 chỉ lấy 005566)
            // term = Regex.Replace(term, "[^0-9]", "");

            if (isPaste)
                term = term.Trim();

            var items = await UnitOfWork.WalletRepo.FindAsNoTrackingAsync(
                x => (x.Status == (byte) WalletStatus.Shipping) && x.TargetWarehouseId.Value == UserState.OfficeId.Value
                     && !x.IsDelete && (isPaste && x.Code == term || isPaste == false && x.Code.Contains(term)) &&
                     ((packageCodes == "") || !packageCodes.Contains(";" + x.Code + ";")),
                x => x.OrderBy(y => y.Id), 1, size);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SuggetionForPutAway(string packageCodes, string term, int size = 6, bool isPaste = false)
        {
            // Chỉ để dạng ký tự không dấu
            term = MyCommon.Ucs2Convert(term);

            // Chỉ lấy mã dạng số: (vd: ODR005566 chỉ lấy 005566)
            // term = Regex.Replace(term, "[^0-9]", "");

            if (isPaste)
                term = term.Trim();

            var items = await UnitOfWork.WalletRepo.SuggetionForPutAway(
                x => (x.Status == (byte) WalletStatus.InStock) && x.TargetWarehouseId.Value == UserState.OfficeId.Value
                     && x.TargetWarehouseId == x.CurrentWarehouseId && x.IsDelete == false &&
                     (isPaste && x.Code == term || isPaste == false && x.Code.Contains(term)),
                packageCodes, size);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Dispatcher, EnumPage.WalletTracker)]
        public async Task<ActionResult> SuggetionToDispatcher(string walletCodes, string term, int size = 6)
        {
            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            var warehouseIdPath = UserState.OfficeIdPath;

            // Chỉ để dạng ký tự không dấu
            term = MyCommon.Ucs2Convert(term);

            // Chỉ lấy mã dạng số: (vd: ODR005566 chỉ lấy 005566)
            // term = Regex.Replace(term, "[^0-9]", "");

            var items = await UnitOfWork.WalletRepo.FindAsNoTrackingAsync(
                x =>
                    x.Status != (byte) WalletStatus.Shipping && x.Status != (byte) WalletStatus.Complete &&
                    ((!isManager && (x.CurrentWarehouseIdPath == warehouseIdPath)) ||
                     (isManager && ((x.CurrentWarehouseIdPath == warehouseIdPath) ||
                       x.CurrentWarehouseIdPath.StartsWith(warehouseIdPath + ".")))) && !x.IsDelete &&
                    x.Code.Contains(term) && ((walletCodes == "") || !walletCodes.Contains(";" + x.Code + ";")),
                x => x.OrderBy(y => y.Id), 1, size);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.WalletTracker)]
        public async Task<ActionResult> GetForWalletTracker(string walletCodes)
        {
            var items =
                await
                    UnitOfWork.WalletRepo.FindAsNoTrackingAsync(x => walletCodes.Contains(";" + x.Code + ";"),
                        x => x.OrderBy(y => y.Id), null);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.Wallet)]
        public async Task<ActionResult> Add(WalletMeta model)
        {
            if (UserState.OfficeType != 1)
                return JsonCamelCaseResult(
                        new {Status = -2, Text = "Only warehouse staff can perform this action"},
                        JsonRequestBehavior.AllowGet);

            if (!ModelState.IsValid)
                return JsonCamelCaseResult(new {Status = -1, Text = "Data format is incorrect" },
                    JsonRequestBehavior.AllowGet);

            var timeNow = DateTime.Now;

            var codes = new List<string>();
            var packageCodeLoses = new List<string>();
            var packages = new List<OrderPackage>();
            FunctionResult result = null;
            List<Order> orders = null;
            Wallet wallet = null;
            Office targetWarehouse;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    wallet = new Wallet
                    {
                        Created = timeNow,
                        Updated = timeNow,
                        Status = model.Status,
                        UserId = UserState.UserId,
                        UserName = UserState.UserName,
                        UserFullName = UserState.FullName,
                        CreatedWarehouseId = UserState.OfficeId ?? 0,
                        CreatedWarehouseName = UserState.OfficeName,
                        CreatedWarehouseIdPath = UserState.OfficeIdPath,
                        CreatedWarehouseAddress = UserState.OfficeAddress,
                        CurrentWarehouseId = UserState.OfficeId ?? 0,
                        CurrentWarehouseName = UserState.OfficeName,
                        CurrentWarehouseIdPath = UserState.OfficeIdPath,
                        CurrentWarehouseAddress = UserState.OfficeAddress,
                        Width = model.Width,
                        Height = model.Height,
                        Length = model.Length,
                        Size = $"{model.Width.ToString("N2", CultureInfo)}x{model.Length.ToString("N2", CultureInfo)}x{model.Height.ToString("N2", CultureInfo)}",
                        Mode = model.Mode,
                        Weight = model.Weight,
                        WeightConverted = Math.Round(model.Width * model.Length * model.Height / 6000, 2),
                        Volume = Math.Round(model.Width * model.Length * model.Height / 1000000, 4),
                        Note = model.Note,
                        Code = string.Empty,
                        UnsignedText = string.Empty,
                    };

                    wallet.WeightActual = wallet.Weight > wallet.WeightConverted
                        ? wallet.Weight
                        : wallet.WeightConverted;

                    // Kho đích là bắt buộc phải chọn
                    if (!model.TargetWarehouseId.HasValue)
                        return JsonCamelCaseResult(
                            new { Status = -2, Text = "Warehouse destination bags forced to choose" },
                            JsonRequestBehavior.AllowGet);

                    // Điểm trung chuyển của bao hàng
                    if (model.Mode == 0 && !model.EntrepotId.HasValue)
                        return JsonCamelCaseResult(new {Status = -2, Text = "Transit point bags forced to select" },
                            JsonRequestBehavior.AllowGet);

                    targetWarehouse = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(
                        x => x.Id == model.TargetWarehouseId && !x.IsDelete &&
                            x.Status == (byte) OfficeStatus.Use);

                    // Kho đích does not exist or has been deleted
                    if (targetWarehouse == null)
                        return
                            JsonCamelCaseResult(
                                new { Status = -2, Text = "destination warehouse does not exist or has been deleted" },
                                JsonRequestBehavior.AllowGet);

                    wallet.TargetWarehouseId = targetWarehouse.Id;
                    wallet.TargetWarehouseName = targetWarehouse.Name;
                    wallet.TargetWarehouseIdPath = targetWarehouse.IdPath;
                    wallet.TargetWarehouseAddress = targetWarehouse.Address;

                    // Đóng bao: Mode == 0
                    if (model.Mode == 0)
                    {
                        var entrepot = await UnitOfWork.EntrepotRepo.SingleOrDefaultAsync(
                            x => x.Id == model.EntrepotId && x.IsDelete == false && x.Status == 1);

                        // Điểm trung chuyển does not exist or has been deleted
                        if (entrepot == null)
                            return JsonCamelCaseResult(
                                new {Status = -2, Text = "Transit point does not exist or has been deleted" },
                                JsonRequestBehavior.AllowGet);

                        wallet.EntrepotId = entrepot.Id;
                        wallet.EntrepotName = entrepot.Name;
                    }

                    UnitOfWork.WalletRepo.Add(wallet);

                    await UnitOfWork.WalletRepo.SaveAsync();

                    // Cập nhật lại Mã cho Order và Sum tiền
                    var walletNoOfWarehouse =
                        UnitOfWork.WalletRepo.Count(x => x.TargetWarehouseId == wallet.TargetWarehouseId && x.Id <= wallet.Id);

                    wallet.Code = $"{targetWarehouse.Code}-{MyCommon.GenCode(walletNoOfWarehouse)}";
                    wallet.UnsignedText = MyCommon.Ucs2Convert(
                        $"{wallet.Code} {wallet.UserFullName} {wallet.UserName} {wallet.CurrentWarehouseName} {wallet.TargetWarehouseName} {wallet.CreatedWarehouseName}");

                    await UnitOfWork.WalletRepo.SaveAsync();

                    // Thêm package
                    foreach (var package in model.Packages)
                    {
                        #region Xử lý thêm package

                        // package
                        var p = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(
                            x => (x.Id == package.PackageId) && !x.IsDelete && x.Status == (byte)OrderPackageStatus.ChinaInStock);

                        // Ds mã kiện does not exist
                        if (p == null)
                        {
                            codes.Add($"P{package.PackageCode}");
                            continue;
                        }

                        // Ds mã kiện đóng sai kho đích
                        if (wallet.TargetWarehouseId.HasValue && p.CustomerWarehouseId != wallet.TargetWarehouseId.Value)
                        {
                            packageCodeLoses.Add($"P{package.PackageCode}");
                        }

                        // Có kiện đóng sai kho đích hoặc kiện does not exist
                        if (codes.Any() || packageCodeLoses.Any())
                            continue;

                        packages.Add(p);

                        // Có kiện hàng khó đích đến khách khác kho đích của bao
                        if (packageCodeLoses.Any())
                        {
                            transaction.Rollback();
                            return
                                JsonCamelCaseResult(
                                    new
                                    {
                                        Status = -2,
                                        Text = $"Các kiện: {string.Join(", ", packageCodeLoses)} Another destination of destination"
                                    },
                                    JsonRequestBehavior.AllowGet);
                        }

                        // Đóng bao hàng
                        if (model.Mode == 0)
                        {
                            p.Status = (byte) OrderPackageStatus.ChinaExport;

                            // Thêm lịch sử cho package
                            var packageHistory = new PackageHistory()
                            {
                                PackageId = p.Id,
                                PackageCode = p.Code,
                                OrderId = p.OrderId,
                                OrderCode = p.OrderCode,
                                Type = p.OrderType,
                                Status = (byte) OrderPackageStatus.ChinaExport,
                                Content =
                                    $"[{UserState.OfficeName}] {EnumHelper.GetEnumDescription(OrderPackageStatus.ChinaExport)}",
                                CustomerId = p.CustomerId,
                                CustomerName = p.CustomerName,
                                UserId = UserState.UserId,
                                UserName = UserState.UserName,
                                UserFullName = UserState.FullName,
                                CreateDate = DateTime.Now,
                            };

                            UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                            // Thêm ghi chú cho package và Order
                            await PackageNote(package.Note, p, wallet, PackageNoteMode.Wallet);
                        }
                        else
                        {
                            // Thêm ghi chú cho package và Order
                            await PackageNote(package.Note, p, wallet, PackageNoteMode.Packing);
                        }

                        // Thêm kiện vào bao hàng
                        UnitOfWork.WalletDetailRepo.Add(new WalletDetail
                        {
                            Created = timeNow,
                            Updated = timeNow,
                            IsDelete = false,
                            Note = package.Note,
                            PackageId = p.Id,
                            PackageCode = p.Code,
                            OrderCode = p.OrderCode,
                            OrderServices = p.OrderServices,
                            OrderId = p.OrderId,
                            OrderType = p.OrderType,
                            Status = 1,
                            TransportCode = p.TransportCode,
                            WalletCode = wallet.Code,
                            WalletId = wallet.Id,
                            Weight = p.Weight,
                            ActualWeight = p.WeightActual,
                            ConvertedWeight = p.WeightConverted,
                            Amount = p.TotalPrice,
                            Customers = p.Customers,
                            CustomersUnsigned = p.CustomersUnsigned,
                            PackageCodes = p.PackageCodes,
                            PackageCodesUnsigned = p.PackageCodesUnsigned,
                            OrderCodes = p.OrderCodes,
                            OrderCodesUnsigned = p.OrderCodesUnsigned,
                            OrderPackageNo = p.PackageNo,
                            Volume = p.Volume
                        });

                        #endregion
                    }

                    // Sum cân nặng của các kiện trong bao không được lớn hơn cân nặng của kiện gỗ/bao gỗ
                    //if (totalWeight > wallet.Weight.Value)
                    //{
                    //    transaction.Rollback();
                    //    return JsonCamelCaseResult(
                    //            new
                    //            {
                    //                Status = -2,
                    //                Text = $"The weight of the bales / sacks must not be less than the bales of bales"
                    //            },
                    //            JsonRequestBehavior.AllowGet);
                    //}

                    // Có kiên/bao does not exist
                    if (codes.Any())
                    {
                        transaction.Rollback();
                        return
                            JsonCamelCaseResult(
                                new
                                {
                                    Status = -2,
                                    Text = $"Các kiện: {string.Join(", ", codes)} does not exist or has been deleted"
                                },
                                JsonRequestBehavior.AllowGet);
                    }

                    // Kiểm tra package nằm trong Order chưa được kiểm đếm
                    var packageCodes = $";{string.Join(";", packages.Select(x => x.Code).ToList())};";
                    var orderCodes = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodes);
                    if (orderCodes.Any())
                    {
                        transaction.Rollback();
                        return JsonCamelCaseResult(
                                new
                                {
                                    Status = -2,
                                    Text = $"Order: {string.Join(", ", orderCodes.Select(MyCommon.ReturnCode).ToList())} No tally "
                                },
                                JsonRequestBehavior.AllowGet);
                    }

                    // Trùng mã vận đơn chưa được xử lý
                    var packageSameCodes = packages.Where(x => x.Mode != null && x.SameCodeStatus == 0).ToList();
                    if (packageSameCodes.Any())
                    {
                        transaction.Rollback();
                        return JsonCamelCaseResult(
                                new
                                {
                                    Status = -2,
                                    Text = $" the transport code: {string.Join(", ", packageSameCodes.Select(x=> x.TransportCode).Distinct().ToList())} overlap untreated"
                                },
                                JsonRequestBehavior.AllowGet);
                    }

                    // Các Order kiểm đếm sai chưa được xử lý
                    var orderCodesLose = await UnitOfWork.OrderRepo.OrderCodeAcountingLose(packageCodes);
                    if (orderCodesLose.Any())
                    {
                        transaction.Rollback();
                        return JsonCamelCaseResult(
                                new
                                {
                                    Status = -2,
                                    Text = $" Order: {string.Join(", ", orderCodesLose.Select(MyCommon.ReturnCode).ToList())} Invalid checksum without processing"
                                },
                                JsonRequestBehavior.AllowGet);
                    }

                    //// Order đi nhanh và đi chậm cùng 1 bao hàng
                    //var orderServiceByPackageCode = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodes, OrderServices.FastDelivery);

                    //if (orderServiceByPackageCode.Any() && orderServiceByPackageCode.Count < packages.Select(x => x.OrderId).Distinct().Count())
                    //{
                    //    transaction.Rollback();
                    //    return JsonCamelCaseResult(
                    //            new
                    //            {
                    //                Status = -2,
                    //                Text = $"Các Order đi nhanh: {string.Join(", ", orderServiceByPackageCode.Select(MyCommon.ReturnCode).ToList())} không được đóng cùng bao hàng này"
                    //            },
                    //            JsonRequestBehavior.AllowGet);
                    //}

                    //// Order tối ưu và đi chậm, đi nhanh cùng 1 bao hàng
                    //var orderOptimalServiceByPackageCode = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodes, OrderServices.Optimal);

                    //if (orderOptimalServiceByPackageCode.Any() && orderOptimalServiceByPackageCode.Count < packages.Select(x => x.OrderId).Distinct().Count())
                    //{
                    //    transaction.Rollback();
                    //    return JsonCamelCaseResult(
                    //            new
                    //            {
                    //                Status = -2,
                    //                Text = $"Các đơn vc tối ưu: {string.Join(", ", orderOptimalServiceByPackageCode.Select(MyCommon.ReturnCode).ToList())} không được đóng cùng bao hàng này"
                    //            },
                    //            JsonRequestBehavior.AllowGet);
                    //}

                    //// Hàng vận chuyển tối ưu không đi line: Đông Hưng
                    //if (orderOptimalServiceByPackageCode.Any() && 
                    //    (model.Mode == 0 || model.Mode == 1 && model.IsSameWallet.HasValue) && 
                    //    model.EntrepotId != 2 && (model.IsConfirm == null || !model.IsConfirm.Value))
                    //{
                    //    transaction.Rollback();
                    //    return JsonCamelCaseResult(
                    //        new
                    //        {
                    //            Status = -1000,
                    //            Text = "Bạn có chắc chắn muốn bao hàng tối ưu này không đi theo line \"Đông Hưng\""
                    //        }, JsonRequestBehavior.AllowGet);
                    //}

                    // Đóng bao
                    if (model.Mode == 0)
                    {
                        // Kiểm tra tạo bao những package chưa được Packing gỗ
                        var orderCodesNoPacking = await UnitOfWork.OrderRepo.OrderCodeByServicePacking(packageCodes);
                        if (orderCodesNoPacking.Any())
                        {
                            transaction.Rollback();
                            return JsonCamelCaseResult(
                                    new
                                    {
                                        Status = -2,
                                        Text = $"Các Order: {string.Join(", ", orderCodesNoPacking.Select(MyCommon.ReturnCode).ToList())} Not packaged wooden package"
                                    },
                                    JsonRequestBehavior.AllowGet);
                        }
                    }

                    // Packing gỗ: Mode == 1
                    if (model.Mode == 1)
                    {
                        // Kiểm tra chứa đơn hàng không sử dụng dịch vụ đóng kiện gỗ
                        var orderPackingCodes = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodes, OrderServices.Packing, false);
                        if (orderPackingCodes.Any())
                        {
                            transaction.Rollback();
                            return
                                JsonCamelCaseResult(
                                    new
                                    {
                                        Status = -2,
                                        Text = $"Order: {string.Join(", ", orderPackingCodes.Select(MyCommon.ReturnCode).ToList())} Not in bales"
                                    },
                                    JsonRequestBehavior.AllowGet);
                        }

                        // Có nhiều hơn 1 khách trong kiện hàng đóng gỗ
                        var orderEmails = await UnitOfWork.OrderRepo.CustomerEmailByPackageCodes(packageCodes);
                        if (orderEmails.Count > 1)
                        {
                            transaction.Rollback();
                            return
                                JsonCamelCaseResult(
                                    new
                                    {
                                        Status = -2,
                                        Text = $"Do not have more than one customer in a wooden package({string.Join(", ", orderEmails)})"
                                    },
                                    JsonRequestBehavior.AllowGet);
                        }

                        // Cập nhật lại cân nặng của kiện (Cập nhật thêm cân nặng phủ bì của kiện gỗ)
                        // Sum cân nặng tính tiền thực của kiện trong kiện gỗ
                        var totalWalletWeight = packages.ToList().Sum(x => 
                            x.Weight > x.WeightConverted ? x.Weight : x.WeightConverted
                        );
                        // Sum thể tích thực tính từ package 
                        var totalWalletVolume = packages.ToList().Sum(x => x.Volume ?? 0);

                        // Thể tích trênh lệnh của bì gỗ 
                        //var walletVolumeWapper = wallet.Volume - totalWalletVolume;
                        // Cộng thêm cân nặng của kiện với cân nặng phủ bì
                        var index = 1;
                        var totalActual = 0M;
                        var totalWapperPercent = 0M;
                        foreach (var orderPackage in packages)
                        {
                            var oldVolumeActual = orderPackage.Volume;
                            
                            // Kiện cuối cùng
                            if (index != packages.Count)
                            {
                                orderPackage.VolumeWapperPercent = Math.Round(orderPackage.Volume.Value * 100 / totalWalletVolume, 4);
                                orderPackage.VolumeActual =
                                   Math.Round(orderPackage.VolumeWapperPercent.Value * wallet.Volume.Value / 100, 4);

                                totalActual += orderPackage.VolumeActual.Value;
                                totalWapperPercent += orderPackage.VolumeWapperPercent.Value;
                            }
                            else
                            {
                                orderPackage.VolumeActual = wallet.Volume.Value - totalActual;
                                orderPackage.VolumeWapperPercent = 100 - totalWapperPercent;
                            }
                            index++;

                            orderPackage.VolumeWapper = orderPackage.VolumeActual - oldVolumeActual;
                        }

                        var ordersWithWeight = new Dictionary<int, decimal>();

                        // Cộng thêm cân nặng của kiện với cân nặng phủ bì
                        index = 1;
                        totalActual = 0M;
                        totalWapperPercent = 0M;
                        foreach (var orderPackage in packages)
                        {
                            var oldActualWeight = orderPackage.WeightActual;
                            
                            // Không phải package cuối cùng trong kiện gỗ
                            if (index != packages.Count)
                            {
                                orderPackage.WeightWapperPercent = Math.Round(orderPackage.WeightActual.Value * 100 / totalWalletWeight.Value, 4);
                                orderPackage.WeightActual = Math.Round(orderPackage.WeightWapperPercent.Value * wallet.WeightActual.Value / 100, 2);
                                totalActual += orderPackage.WeightActual.Value;
                                totalWapperPercent += orderPackage.WeightWapperPercent.Value;
                            }
                            else
                            {
                                orderPackage.WeightActual = wallet.WeightActual.Value - totalActual;
                                orderPackage.WeightWapperPercent = 100 - totalWapperPercent;
                            }

                            index++;

                            // Độ lệch trước và sau Packing gỗ
                            orderPackage.WeightWapper = orderPackage.WeightActual - oldActualWeight;

                            // Add cân nặng phát
                            if (ordersWithWeight.ContainsKey(orderPackage.OrderId))
                            {
                                ordersWithWeight[orderPackage.OrderId] =
                                    ordersWithWeight[orderPackage.OrderId] + orderPackage.WeightWapper ?? 0;
                            }
                            else
                            {
                                ordersWithWeight.Add(orderPackage.OrderId, orderPackage.WeightWapper ?? 0);
                            }
                        }

                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        // Tính Total money Packing gỗ của kiện
                        var packingPriceWallet = OrderRepository.PackingPrice(wallet.WeightActual ?? 0);

                        // Cập nhật lại chi phí Packing gỗ cho các package
                        foreach (var package in packages)
                        {
                            // Tiền chia đều 20 yuan dầu tiên
                            package.TotalPriceWapper = package.WeightWapperPercent * 20 / 100;

                            // Tiền chia đều các yuan tiếp theo
                            if (packingPriceWallet > 20)
                            {
                                package.TotalPriceWapper += (packingPriceWallet - 20) * package.WeightWapperPercent / 100;
                            }
                        }

                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        var orderIds = $";{string.Join(";", ordersWithWeight.Select(x => x.Key).ToList()) };";

                        // Các dịch vụ vận chuyển liên quan
                        var outSideShipServices = await UnitOfWork.OrderServiceRepo.FindAsync(
                                    x => !x.IsDelete && x.Checked &&
                                        x.ServiceId == (byte)OrderServices.OutSideShipping &&
                                        orderIds.Contains(";" + x.OrderId + ";"));

                        //var optimalServices = await
                        //    UnitOfWork.OrderServiceRepo.FindAsync(
                        //        x => !x.IsDelete && orderIds.Contains(";" + x.OrderId + ";") &&
                        //             x.ServiceId == (byte) OrderServices.Optimal && x.Checked);

                        // Các dịch vụ Packing liên quan
                        var packingServices = await UnitOfWork.OrderServiceRepo.FindAsync(
                                    x => !x.IsDelete && x.Checked &&
                                        x.ServiceId == (byte)OrderServices.Packing &&
                                        orderIds.Contains(";" + x.OrderId + ";"));

                        //// Các dịch vụ vận chuyển hàng không liên quan
                        //var fastDeliveryServices = await UnitOfWork.OrderServiceRepo.FindAsync(
                        //            x => !x.IsDelete && x.Checked &&
                        //                x.ServiceId == (byte)OrderServices.FastDelivery &&
                        //                orderIds.Contains(";" + x.OrderId + ";"));

                        // Các order liên quan
                        var ordersForUpdate =
                            await UnitOfWork.OrderRepo.FindAsync(
                                    x => !x.IsDelete && orderIds.Contains(";" + x.Id + ";"));

                        foreach (var o in ordersForUpdate)
                        {
                            // Cập nhật lại giá dịch vụ Packing gỗ
                            var packingService = packingServices.SingleOrDefault(x => x.OrderId == o.Id);
                            if (packingService != null)
                            {
                                var totalOrderPacking = await UnitOfWork.OrderPackageRepo.Entities
                                    .Where(x => x.OrderId == o.Id && !x.IsDelete)
                                    .SumAsync(x => x.TotalPriceWapper ?? 0);

                                packingService.Value = totalOrderPacking;
                                packingService.TotalPrice = totalOrderPacking * o.ExchangeRate;
                            }

                            #region Kiện bị thêm cân nặng từ phủ bì của kiện gỗ -> Cập nhật lại các dịch vụ liên quan đến cân nạng
                            // Kiện bị thêm cân nặng từ phủ bì của kiện gỗ -> Cập nhật lại các dịch vụ liên quan đến cân nạng
                            var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(o.LevelId);

                            var totalOrderWeight = await UnitOfWork.OrderPackageRepo.Entities
                                .Where(x => x.OrderId == o.Id && !x.IsDelete)
                                .SumAsync(x => x.WeightActual ?? 0);

                            // Cập nhật lại giá dịch vụ vận chuyển TQ -> VN của Order
                            var outSideShipService = outSideShipServices.SingleOrDefault(x => x.OrderId == o.Id);

                            // Cân nặng các package được xuất giao tại TQ
                            var orderWeightIgnore = UnitOfWork.OrderPackageRepo.GetTotalActualWeight(o.Id);

                            // Sum cân nặng tính tiền vc của Order
                            var orderWeight = totalOrderWeight - orderWeightIgnore;

                            if (outSideShipService != null)
                            {
                                //var fastDeliveryService = fastDeliveryServices.SingleOrDefault(x => x.OrderId == o.Id);
                                //var optimalService = optimalServices.SingleOrDefault(x => x.OrderId == o.Id);

                                decimal serviceValue;
                                decimal outSideShipping;

                                // Order ký gửi
                                if (o.Type == (byte)OrderType.Deposit)
                                {
                                    serviceValue = o.ApprovelPrice ?? 0;

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
                                    //        o.WarehouseDeliveryId ?? 0);
                                    //}
                                    //else if (fastDeliveryService != null) // VC nhanh
                                    //{
                                    //    serviceValue = OrderRepository.FastDelivery(orderWeight,
                                    //        o.WarehouseDeliveryId ?? 0);
                                    //}
                                    //else // VC bình thường
                                    //{
                                        serviceValue = OrderRepository.ShippingOutSide(o.ServiceType, orderWeight,
                                            o.WarehouseDeliveryId ?? 0);
                                    //}

                                    outSideShipping = orderWeight * serviceValue;
                                }

                                outSideShipService.Value = serviceValue;

                                outSideShipService.TotalPrice = outSideShipping;

                                if (o.Type == (byte)OrderType.Order)
                                {
                                    // Triết khấu VIP
                                    outSideShipService.TotalPrice -= vipLevel.Ship * outSideShipService.TotalPrice / 100;
                                    outSideShipService.Note = $"Shipping service fee to Vietnam {outSideShipService.Value.ToString("N2", CultureInfo)} Baht/1kg" +
                                                                $" And was discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";
                                }
                            }
                            #endregion
                        }
                    }

                    await UnitOfWork.WalletDetailRepo.SaveAsync();

                    #region Cập nhật chi phí phát sinh của package

                    if (model.OrderServiceOthers != null && model.OrderServiceOthers.Any())
                    {
                        foreach (var serviceOther in model.OrderServiceOthers)
                        {
                            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Code == serviceOther.OrderCode);
                            var pOtherService =
                                await UnitOfWork.WalletDetailRepo.FindAsync(x => x.IsDelete == false && x.WalletId == wallet.Id &&
                                        x.OrderId == order.Id);

                            var packageCode = pOtherService.Select(x => x.PackageCode).ToList();

                            var transportCode = pOtherService.Select(x => x.TransportCode).ToList();

                            var s = new OrderServiceOther()
                            {
                                Value = serviceOther.Value,
                                Created = DateTime.Now,
                                Mode = serviceOther.Mode,
                                CreatedOfficeId = UserState.OfficeId ?? 0,
                                CreatedOfficeIdPath = UserState.OfficeIdPath,
                                CreatedOfficeName = UserState.OfficeName,
                                CreatedUserFullName = UserState.FullName,
                                CreatedUserId = UserState.UserId,
                                CreatedUserTitleId = UserState.TitleId ?? 0,
                                CreatedUserTitleName = UserState.TitleName,
                                CreatedUserUserName = UserState.UserName,
                                Currency = Currency.CNY.ToString(),
                                Note = serviceOther.Note,
                                ObjectId = wallet.Id,
                                Type = 0,
                                ExchangeRate = order.ExchangeRate,
                                TotalPrice = order.ExchangeRate * serviceOther.Value,
                                OrderCode = order.Code,
                                OrderId = order.Id,
                                PackageNo = pOtherService.Count,
                                PackageCodes = $";{string.Join(";", packageCode)};",
                                UnsignText = MyCommon.Ucs2Convert($"{order.Code} {order.CustomerEmail}" +
                                                                  $" {order.ContactPhone} {order.CustomerName}" +
                                                                  $" {string.Join(" ", packageCode)}" +
                                                                  $" {string.Join(" ", transportCode)}"),
                                TotalWeightActual = pOtherService.Sum(x=> x.ActualWeight)
                            };

                            UnitOfWork.OrderServiceOtherRepo.Add(s);
                        }

                        await UnitOfWork.OrderServiceOtherRepo.SaveAsync();
                    }

                    #endregion

                    await UnitOfWork.WalletDetailRepo.SaveAsync();

                    // Sum số kiện trong bao
                    wallet.PackageNo = await UnitOfWork.WalletDetailRepo.CountAsync(
                        x => !x.IsDelete && (x.WalletId == wallet.Id));

                    // Sum giá trị của bao
                    wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.Amount != null) && (x.WalletId == wallet.Id))
                        .SumAsync(x => x.Amount);

                    // Sum cân nặng kiện trong bao
                    wallet.TotalWeight = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.Weight != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.Weight);

                    // Sum cân nặng kiện trong bao
                    wallet.TotalWeightActual = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.ActualWeight);

                    // Sum cân nặng chuyển đổi của kiện trong bao
                    wallet.TotalWeightConverted = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.ConvertedWeight != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.ConvertedWeight);

                    // Sum cân nặng tính tiền của kiện trong bao
                    wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.ActualWeight);

                    // Sum thể tích kiện trong bao
                    wallet.TotalVolume = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.Volume != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.Volume);

                    // Dach sách mã các kiện trong bao
                    wallet.PackageCodes = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(
                        x => !x.IsDelete && (x.WalletId == wallet.Id)).Select(x => "P" + x.PackageCode).ToList());

                    var str = string.Join(";", UnitOfWork.WalletDetailRepo.Find(
                        x => !x.IsDelete && (x.WalletId == wallet.Id)).ToList().Select(x => x.PackageCode).ToList());
                    wallet.PackageCodesUnsigned = $";{str};";

                    // Danh sách mã Orders trong bao
                    var strOrdersCode = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(x => !x.IsDelete && (x.WalletId == wallet.Id))
                        .Where(x => !x.IsDelete && x.WalletId == wallet.Id)
                        .ToList()
                        .Select(x => string.IsNullOrWhiteSpace(x.OrderCodes) ? x.OrderCode : x.OrderCodes));

                    // Cập nhật lại các dịch vụ trong bao hàng
                    var orderServices = await UnitOfWork.WalletRepo.GetOrderServiceByWalletId(wallet.Id);

                    wallet.OrderServicesJson = JsonConvert.SerializeObject(orderServices,
                        new JsonSerializerSettings() {ContractResolver = new CamelCasePropertyNamesContractResolver()});

                    wallet.OrderServices = $";{string.Join(";", orderServices.Select(x => x.ServiceId).ToList())};";

                    var ordersCode = strOrderCode.Replace("ORD", "").Split(',').ToList().Select(x => x.Trim()).Distinct().ToList();

                    if (ordersCode.Any())
                    {
                        wallet.OrderCodes = string.Join(", ", ordersCode.Select(x => "ORD" + x));

                        wallet.OrderCodesUnsigned = $";{string.Join(";", ordersCode)};";

                        orders = UnitOfWork.OrderRepo.Find(
                            x => !x.IsDelete && wallet.OrderCodesUnsigned.Contains(";" + x.Code + ";")).ToList();

                        wallet.Customers = string.Join(", ", orders.Select(x => x.CustomerEmail).Distinct().ToList());
                        wallet.CustomersUnsigned = $";{string.Join(";", orders.Select(x => x.CustomerEmail).Distinct().ToList())};";

                        //wallet.Note = "Khách hàng: " + string.Join(", ", orders.Select(x => x.CustomerName + "(" + x.CustomerEmail + ")").Distinct().ToList());

                        // Thêm tiền dịch vụ khác cho đơn  hàng
                        #region Tính lại tiền dịch vụ phát sinh trong Order
                        if (model.OrderServiceOthers != null && model.OrderServiceOthers.Any())
                        {
                            foreach (var order in orders)
                            {
                                // Tính Total money dịch vụ khác
                                var totalServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(x => x.OrderId == order.Id);

                                var otherService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                            x => !x.IsDelete && x.OrderId == order.Id &&
                                                x.ServiceId == (byte)OrderServices.Other && x.Checked);

                                if (otherService == null)
                                {
                                    otherService = new OrderService
                                    {
                                        IsDelete = false,
                                        Checked = true,
                                        Created = timeNow,
                                        LastUpdate = timeNow,
                                        Value = totalServiceOther.Sum(x => x.Value),
                                        Currency = Currency.CNY.ToString(),
                                        ExchangeRate = order.ExchangeRate,
                                        TotalPrice = totalServiceOther.Sum(x => x.TotalPrice),
                                        HashTag = string.Empty,
                                        Mode = (byte)OrderServiceMode.Option,
                                        OrderId = order.Id,
                                        ServiceId = (byte)OrderServices.Other,
                                        ServiceName = (OrderServices.Other).GetAttributeOfType<DescriptionAttribute>()
                                            .Description,
                                        Type = (byte)UnitType.Money,
                                        Note = $"Service fees incurred. (Shop shipment goods after goods into warehouse, rent forklift,...)",
                                    };

                                    UnitOfWork.OrderServiceRepo.Add(otherService);
                                }
                                else
                                {
                                    otherService.LastUpdate = timeNow;
                                    otherService.Value = totalServiceOther.Sum(x => x.Value);
                                    otherService.TotalPrice = totalServiceOther.Sum(x => x.TotalPrice);
                                }
                            }

                            await UnitOfWork.OrderServiceOtherRepo.SaveAsync();
                        }

                        foreach (var order in orders)
                        {
                            var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                               x.IsDelete == false && x.Checked).ToList().Sum(x => x.TotalPrice);

                            order.Total = order.TotalExchange + totalService;
                            order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);
                        }
                        #endregion
                    }

                    await UnitOfWork.WalletRepo.SaveAsync();

                    // Thực hiện đóng bao kiện gỗ
                    if (model.Mode == 1 && model.IsSameWallet.HasValue && model.IsSameWallet.Value)
                    {
                        model.Mode = 0;
                        result = await AddWallet(model);

                        if (result.Status <= 0)
                        {
                            transaction.Rollback();

                            return JsonCamelCaseResult(new { result.Status, result.Text },
                                JsonRequestBehavior.AllowGet);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            // Tính toán chi phí phát sinh cảu Order  
            if (orders != null)
            {
                var jobId = BackgroundJob.Enqueue(() => PackageJob.UpdateOtherServiceOfPackage(orders.Select(x => x.Id).ToList()));

                var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

                // Packing gỗ
                if (model.Mode == 1)
                {
                    // Job cập nhật thông tin package
                    var jobId2 = BackgroundJob.ContinueWith(jobId,
                        () => PackageJob.UpdateWeightActualPercent(orders.Select(x => x.Id).ToList()));

                    BackgroundJob.ContinueWith(jobId2, () => OrderJob.ProcessDebitReport(orderIds));
                }
                else
                {
                    BackgroundJob.ContinueWith(jobId, () => OrderJob.ProcessDebitReport(orderIds));
                }
            }

            if (result != null)
            {
                return JsonCamelCaseResult(new { result.Status, result.Text, data = wallet, wallet = result.Data },
                    JsonRequestBehavior.AllowGet);
            }

            return JsonCamelCaseResult(new {Status = 1, wallet, Text = "Create bales/ package successul" },
                JsonRequestBehavior.AllowGet);
        }

        //Cập nhật note bao hàng
        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.Wallet)]
        public async Task<JsonResult> updateWalletNote(int id, string note)
        {

            if (UserState.OfficeType != (byte)OfficeType.Warehouse)
                return Json(new { status = -1, msg = "Only new warehouse staff has the right to perform this operation!" },
                            JsonRequestBehavior.AllowGet);

            var timeNow = DateTime.Now;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var wallet =
                        await UnitOfWork.WalletRepo.SingleOrDefaultAsync(x => x.Id == id);

                    if (wallet == null)
                        return Json(new { status = -1, msg = "Packing does not exist or has been deleted!" },
                            JsonRequestBehavior.AllowGet);

                    wallet.Note = note;
                    wallet.Updated = timeNow;

                    await UnitOfWork.WalletRepo.SaveAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Update Successful!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thêm bao khi Packing gỗ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<FunctionResult> AddWallet(WalletMeta model)
        {
            if (model.Mode != 0)
            {
                return new FunctionResult
                {
                    Status = -2,
                    Text = "Error function parameters bagging"
                };
            }

            var timeNow = DateTime.Now;

            var codes = new List<string>();
            var packageCodeLoses = new List<string>();
            var packages = new List<OrderPackage>();

            var wallet = new Wallet
            {
                Created = timeNow,
                Updated = timeNow,
                Status = model.Status,
                UserId = UserState.UserId,
                UserName = UserState.UserName,
                UserFullName = UserState.FullName,
                CreatedWarehouseId = UserState.OfficeId ?? 0,
                CreatedWarehouseName = UserState.OfficeName,
                CreatedWarehouseIdPath = UserState.OfficeIdPath,
                CreatedWarehouseAddress = UserState.OfficeAddress,
                CurrentWarehouseId = UserState.OfficeId ?? 0,
                CurrentWarehouseName = UserState.OfficeName,
                CurrentWarehouseIdPath = UserState.OfficeIdPath,
                CurrentWarehouseAddress = UserState.OfficeAddress,
                Width = model.Width,
                Height = model.Height,
                Length = model.Length,
                Size = $"{model.Width.ToString("N2", CultureInfo)}x{model.Length.ToString("N2", CultureInfo)}x{model.Height.ToString("N2", CultureInfo)}",
                Mode = model.Mode,
                Weight = model.Weight,
                WeightConverted = Math.Round(model.Width * model.Length * model.Height / 5000, 2),
                Volume = Math.Round(model.Width * model.Length * model.Height / 1000000, 4),
                Note = model.Note,
                Code = string.Empty,
                UnsignedText = string.Empty,
            };

            wallet.WeightActual = wallet.Weight > wallet.WeightConverted
                ? wallet.Weight
                : wallet.WeightConverted;

            // Kho đích là bắt buộc phải chọn
            if (!model.TargetWarehouseId.HasValue)
                return new FunctionResult
                {
                    Status = -2,
                    Text = "Warehouse destination of bags forced to select"
                };

            // Điểm trung chuyển của bao hàng
            if (!model.EntrepotId.HasValue)
                return new FunctionResult
                {
                    Status = -2,
                    Text = "Transit point of bag forced to select"
                };

            var targetWarehouse = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(
            x =>
                (x.Id == model.TargetWarehouseId) && !x.IsDelete &&
                (x.Status == (byte)OfficeStatus.Use));

            // Kho đích does not exist or has been deleted
            if (targetWarehouse == null)
                return new FunctionResult
                {
                    Status = -2,
                    Text = "Warehouse destination does not exist or has been deleted"
                };

            wallet.TargetWarehouseId = targetWarehouse.Id;
            wallet.TargetWarehouseName = targetWarehouse.Name;
            wallet.TargetWarehouseIdPath = targetWarehouse.IdPath;
            wallet.TargetWarehouseAddress = targetWarehouse.Address;

            var entrepot = await UnitOfWork.EntrepotRepo.SingleOrDefaultAsync(
                x => x.Id == model.EntrepotId && x.IsDelete == false && x.Status == 1);

            // Điểm trung chuyển does not exist or has been deleted
            if (entrepot == null)
                return new FunctionResult
                {
                    Status = -2,
                    Text = "Transit point does not exist or has been deleted"
                };

            wallet.EntrepotId = entrepot.Id;
            wallet.EntrepotName = entrepot.Name;

            UnitOfWork.WalletRepo.Add(wallet);

            await UnitOfWork.WalletRepo.SaveAsync();

            var walletNoOfWarehouse =
                UnitOfWork.WalletRepo.Count(x => x.TargetWarehouseId == wallet.TargetWarehouseId && x.Id <= wallet.Id);
            wallet.Code = $"{targetWarehouse.Code}-{MyCommon.GenCode(walletNoOfWarehouse)}";

            wallet.UnsignedText = MyCommon.Ucs2Convert(
                $"{wallet.Code} {wallet.UserFullName} {wallet.UserName} {wallet.CurrentWarehouseName} {wallet.TargetWarehouseName} {wallet.CreatedWarehouseName}");

            await UnitOfWork.WalletRepo.SaveAsync();

            //decimal totalWeight = 0;

            // Thêm package
            foreach (var package in model.Packages)
            {
                #region Xử lý thêm package

                // package
                var p = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(
                    x => (x.Id == package.PackageId) && !x.IsDelete && x.Status == (byte)OrderPackageStatus.ChinaInStock);

                // Ds mã kiện does not exist
                if (p == null)
                {
                    codes.Add($"P{package.PackageCode}");
                    continue;
                }

                // Ds mã kiện đóng sai kho đích
                if (p.CustomerWarehouseId != wallet.TargetWarehouseId.Value)
                {
                    packageCodeLoses.Add($"P{package.PackageCode}");
                }

                // Có kiện đóng sai kho đích hoặc kiện does not exist
                if (codes.Any() || packageCodeLoses.Any())
                    continue;

                //totalWeight += p.Weight ?? 0;

                packages.Add(p);

                // Có kiện hàng khó đích đến khách khác kho đích của bao
                if (packageCodeLoses.Any())
                {
                    //var textName = packages.Where(x => packageCodeLoses.Any(y => y == x.Code));

                    return new FunctionResult
                    {
                        Status = -2,
                        Text = $"Các kiện: {string.Join(", ", packageCodeLoses)} Another destination warehouse destination "
                    };
                }

                // Đóng bao hàng
                if (model.Mode == 0)
                {
                    p.Status = (byte) OrderPackageStatus.ChinaExport;

                    // Thêm lịch sử cho package
                    var packageHistory = new PackageHistory()
                    {
                        PackageId = p.Id,
                        PackageCode = p.Code,
                        OrderId = p.OrderId,
                        OrderCode = p.OrderCode,
                        Type = p.OrderType,
                        Status = (byte) OrderPackageStatus.ChinaExport,
                        Content =
                            $"[{UserState.OfficeName}] {EnumHelper.GetEnumDescription(OrderPackageStatus.ChinaExport)}",
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        UserId = UserState.UserId,
                        UserName = UserState.UserName,
                        UserFullName = UserState.FullName,
                        CreateDate = DateTime.Now,
                    };

                    UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                    // Thêm ghi chú cho package và Order
                    await PackageNote(package.Note, p, wallet, PackageNoteMode.Wallet);
                }
                else
                {
                    // Thêm ghi chú cho package và Order
                    await PackageNote(package.Note, p, wallet, PackageNoteMode.Packing);
                }

                // Thêm kiện vào bao hàng
                UnitOfWork.WalletDetailRepo.Add(new WalletDetail
                {
                    Created = timeNow,
                    Updated = timeNow,
                    IsDelete = false,
                    Note = package.Note,
                    PackageId = p.Id,
                    PackageCode = p.Code,
                    OrderCode = p.OrderCode,
                    OrderServices = p.OrderServices,
                    OrderId = p.OrderId,
                    OrderType = p.OrderType,
                    Status = 1,
                    TransportCode = p.TransportCode,
                    WalletCode = wallet.Code,
                    WalletId = wallet.Id,
                    Weight = p.Weight,
                    ActualWeight = p.WeightActual,
                    ConvertedWeight = p.WeightConverted,
                    Amount = p.TotalPrice,
                    Customers = p.Customers,
                    CustomersUnsigned = p.CustomersUnsigned,
                    PackageCodes = p.PackageCodes,
                    PackageCodesUnsigned = p.PackageCodesUnsigned,
                    OrderCodes = p.OrderCodes,
                    OrderCodesUnsigned = p.OrderCodesUnsigned,
                    OrderPackageNo = p.PackageNo,
                    Volume = p.Volume
                });

                #endregion
            }

            // Sum cân nặng của các kiện trong bao không được lớn hơn cân nặng của kiện gỗ/bao gỗ
            //if (totalWeight > (wallet.Weight ?? 0))
            //{
            //    return new FunctionResult
            //    {
            //        Status = -2,
            //        Text = $"Cân nặng của kiện gỗ/bao không được nhỏ hơn Sum cân nặng các kiện con"
            //    };
            //}

            // Có kiên/bao does not exist
            if (codes.Any())
            {
                return new FunctionResult
                {
                    Status = -2,
                    Text = $"Package: {string.Join(", ", codes)} does not exist or has been deleted"
                };
            }

            // Kiểm tra package nằm trong Order chưa được kiểm đếm
            var packageCodes = $";{string.Join(";", packages.Select(x => x.Code).ToList())};";
            var orderCodes = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodes);
            if (orderCodes.Any())
            {
                return new FunctionResult
                {
                    Status = -2,
                    Text = $" Order: {string.Join(", ", orderCodes.Select(MyCommon.ReturnCode).ToList())} Not yet tallyed"
                };
            }

            // Trùng mã vận đơn chưa được xử lý
            var packageSameCodes = packages.Where(x => x.Mode != null && x.SameCodeStatus == 0).ToList();
            if (packageSameCodes.Any())
            {
                return new FunctionResult
                {
                    Status = -2,
                    Text = $"Transport code: {string.Join(", ", packageSameCodes.Select(x => x.TransportCode).Distinct().ToList())} Overlap untreated"
                };
            }

            // Các Orders kiểm đếm sai chưa được xử lý
            var orderCodesLose = await UnitOfWork.OrderRepo.OrderCodeAcountingLose(packageCodes);
            if (orderCodesLose.Any())
            {
                return new FunctionResult
                {
                    Status = -2,
                    Text = $" Order: {string.Join(", ", orderCodesLose.Select(MyCommon.ReturnCode).ToList())} wrong tally has not been processed yet"
                };
            }

            //// Order đi nhanh và đi chậm cùng 1 bao hàng
            //var orderServiceByPackageCode = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodes, OrderServices.FastDelivery);

            //if (orderServiceByPackageCode.Any() && orderServiceByPackageCode.Count < packages.Select(x => x.OrderId).Distinct().Count())
            //{
            //    return new FunctionResult
            //    {
            //        Status = -2,
            //        Text = $"Các Order đi nhanh: {string.Join(", ", orderServiceByPackageCode.Select(MyCommon.ReturnCode).ToList())} không được đóng cùng bao hàng này"
            //    };
            //}

            //// Order tối ưu và đi chậm, đi nhanh cùng 1 bao hàng
            //var orderOptimalServiceByPackageCode = await UnitOfWork.OrderRepo.OrderCodeByService(packageCodes, OrderServices.Optimal);

            //if (orderOptimalServiceByPackageCode.Any() && orderOptimalServiceByPackageCode.Count < packages.Select(x => x.OrderId).Distinct().Count())
            //{
            //    return new FunctionResult
            //    {
            //        Status = -2,
            //        Text = $"Các đơn vc tối ưu: {string.Join(", ", orderOptimalServiceByPackageCode.Select(MyCommon.ReturnCode).ToList())} không được đóng cùng bao hàng này"
            //    };
            //}

            // Kiểm tra tạo bao những package chưa được Packing gỗ
            var orderCodesNoPacking = await UnitOfWork.OrderRepo.OrderCodeByServicePacking(packageCodes);
            if (orderCodesNoPacking.Any())
            {
                return new FunctionResult
                {
                    Status = -2,
                    Text = $"Các Order: {string.Join(", ", orderCodesNoPacking.Select(MyCommon.ReturnCode).ToList())} Not yet pack wooden package"
                };
            }

            await UnitOfWork.WalletDetailRepo.SaveAsync();

            // Sum số kiện trong bao
            wallet.PackageNo = await UnitOfWork.WalletDetailRepo.CountAsync(
                x => !x.IsDelete && (x.WalletId == wallet.Id));

            // Sum giá trị của bao
            wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
                    x => !x.IsDelete && (x.Amount != null) && (x.WalletId == wallet.Id))
                .SumAsync(x => x.Amount);

            // Sum cân nặng kiện trong bao
            wallet.TotalWeight = await UnitOfWork.WalletDetailRepo.Entities.Where(
                   x => !x.IsDelete && (x.Weight != null) && (x.WalletId == wallet.Id))
               .SumAsync(x => x.Weight);

            // Sum cân nặng kiện trong bao
            wallet.TotalWeightActual = await UnitOfWork.WalletDetailRepo.Entities.Where(
                   x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
               .SumAsync(x => x.ActualWeight);

            // Sum cân nặng chuyển đổi của kiện trong bao
            wallet.TotalWeightConverted = await UnitOfWork.WalletDetailRepo.Entities.Where(
                   x => !x.IsDelete && (x.ConvertedWeight != null) && (x.WalletId == wallet.Id))
               .SumAsync(x => x.ConvertedWeight);

            // Sum cân nặng tính tiền của kiện trong bao
            wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
                   x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
               .SumAsync(x => x.ActualWeight);

            // Sum thể tích kiện trong bao
            wallet.TotalVolume = await UnitOfWork.WalletDetailRepo.Entities.Where(
                   x => !x.IsDelete && (x.Volume != null) && (x.WalletId == wallet.Id))
               .SumAsync(x => x.Volume);

            // Dach sách mã các kiện trong bao
            wallet.PackageCodes = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(
                x => !x.IsDelete && (x.WalletId == wallet.Id)).Select(x => "P" + x.PackageCode).ToList());

            // Cập nhật lại các dịch vụ trong bao hàng
            var orderServices = await UnitOfWork.WalletRepo.GetOrderServiceByWalletId(wallet.Id);

            wallet.OrderServicesJson = JsonConvert.SerializeObject(orderServices,
                new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            wallet.OrderServices = $";{string.Join(";", orderServices.Select(x => x.ServiceId).ToList())};";

            await UnitOfWork.WalletRepo.SaveAsync();

            var str = string.Join(";", UnitOfWork.WalletDetailRepo.Find(
                x => !x.IsDelete && (x.WalletId == wallet.Id)).ToList().Select(x => x.PackageCode).ToList());
            wallet.PackageCodesUnsigned = $";{str};";

            // Danh sách mã Orders trong bao
            var strOrdersCode = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(x => !x.IsDelete && (x.WalletId == wallet.Id))
                .Where(x => !x.IsDelete && x.WalletId == wallet.Id)
                .ToList()
                .Select(x => string.IsNullOrWhiteSpace(x.OrderCodes) ? x.OrderCode : x.OrderCodes));

            var ordersCode = strOrderCode.Replace("ORD", "").Split(',').ToList().Select(x => x.Trim()).Distinct().ToList();

            if (ordersCode.Any())
            {
                wallet.OrderCodes = string.Join(", ", ordersCode.Select(x => "ORD" + x));

                wallet.OrderCodesUnsigned = $";{string.Join(";", ordersCode)};";

                var orders = UnitOfWork.OrderRepo.Find(
                    x => !x.IsDelete && wallet.OrderCodesUnsigned.Contains(";" + x.Code + ";")).ToList();

                wallet.Customers = string.Join(", ", orders.Select(x => x.CustomerEmail).Distinct().ToList());
                wallet.CustomersUnsigned = $";{string.Join(";", orders.Select(x => x.CustomerEmail).Distinct().ToList())};";

                //wallet.Note = "Khách hàng: " + string.Join(", ", orders.Select(x => x.CustomerName + "(" + x.CustomerEmail + ")").Distinct().ToList());

                // Thêm tiền dịch vụ khác cho đơn  hàng
                #region Tính lại tiền dịch vụ phát sinh trong Order
                if (model.OrderServiceOthers != null && model.OrderServiceOthers.Any())
                {
                    foreach (var order in orders)
                    {
                        // Tính Total money dịch vụ khác
                        var totalServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(x => x.OrderId == order.Id);

                        var otherService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                    x => !x.IsDelete && x.OrderId == order.Id &&
                                        x.ServiceId == (byte)OrderServices.Other && x.Checked);

                        if (otherService == null)
                        {
                            otherService = new OrderService
                            {
                                IsDelete = false,
                                Checked = true,
                                Created = timeNow,
                                LastUpdate = timeNow,
                                Value = totalServiceOther.Sum(x => x.Value),
                                Currency = Currency.CNY.ToString(),
                                ExchangeRate = order.ExchangeRate,
                                TotalPrice = totalServiceOther.Sum(x => x.TotalPrice),
                                HashTag = string.Empty,
                                Mode = (byte)OrderServiceMode.Option,
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.Other,
                                ServiceName = (OrderServices.Other).GetAttributeOfType<DescriptionAttribute>()
                                    .Description,
                                Type = (byte)UnitType.Money,
                                Note = $"Service fees incurred. (Shop shipment goods after goods into warehouse, rent forklift,...)",
                            };

                            UnitOfWork.OrderServiceRepo.Add(otherService);
                        }
                        else
                        {
                            otherService.LastUpdate = timeNow;
                            otherService.Value = totalServiceOther.Sum(x => x.Value);
                            otherService.TotalPrice = totalServiceOther.Sum(x => x.TotalPrice);
                        }
                    }

                    await UnitOfWork.OrderServiceOtherRepo.SaveAsync();
                }

                foreach (var order in orders)
                {
                    var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                       x.IsDelete == false && x.Checked).ToList().Sum(x => x.TotalPrice);

                    order.Total = order.TotalExchange + totalService;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);
                }
                #endregion
            }

            await UnitOfWork.WalletRepo.SaveAsync();

            return new FunctionResult
            {
                Status = 1,
                Text = "Tạo bao thành công",
                Data = wallet
            };
        }

        private async Task PackageNote(string strPackageNote, OrderPackage package, Wallet wallet, PackageNoteMode packageNoteMode)
        {
            //Thêm ghi chú cho các Orders trong phiếu nhập
            var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                                x =>
                                    x.PackageId == null && x.OrderId == package.OrderId && x.ObjectId == wallet.Id &&
                                    x.Mode == (byte)packageNoteMode);

            if (packageNote == null && !string.IsNullOrWhiteSpace(wallet.Note))
            {
                UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                {
                    OrderId = package.OrderId,
                    OrderCode = package.OrderCode,
                    PackageId = null,
                    PackageCode = null,
                    UserId = wallet.UserId,
                    UserFullName = wallet.UserFullName,
                    Time = DateTime.Now,
                    ObjectId = wallet.Id,
                    ObjectCode = wallet.Code,
                    Mode = (byte)packageNoteMode,
                    Content = wallet.Note
                });
            }
            else if (packageNote != null && !string.IsNullOrWhiteSpace(wallet.Note))
            {
                packageNote.Content = wallet.Note;
            }
            else if (packageNote != null && string.IsNullOrWhiteSpace(wallet.Note))
            {
                UnitOfWork.PackageNoteRepo.Remove(packageNote);
            }

            // Thêm note cho package trong phiếu nhập
            var packageNote2 = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                    x =>
                        x.PackageId == package.Id && x.ObjectId == wallet.Id &&
                        x.Mode == (byte)packageNoteMode);

            if (packageNote2 == null && !string.IsNullOrWhiteSpace(strPackageNote))
            {
                UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                {
                    OrderId = package.OrderId,
                    OrderCode = package.OrderCode,
                    PackageId = package.Id,
                    PackageCode = package.Code,
                    UserId = wallet.UserId,
                    UserFullName = wallet.UserFullName,
                    Time = DateTime.Now,
                    ObjectId = wallet.Id,
                    ObjectCode = wallet.Code,
                    Mode = (byte)packageNoteMode,
                    Content = strPackageNote
                });
            }
            else if (packageNote2 != null && !string.IsNullOrWhiteSpace(strPackageNote))
            {
                packageNote2.Content = strPackageNote;
            }
            else if (packageNote2 != null && string.IsNullOrWhiteSpace(strPackageNote))
            {
                UnitOfWork.PackageNoteRepo.Remove(packageNote2);
            }

            await UnitOfWork.PackageNoteRepo.SaveAsync();
        }

        /// <summary>
        /// Cập nhật lại cân nặng của package
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.Wallet)]
        public async Task<ActionResult> UpdateWeight(WalletUpdateInfoMeta model)
        {
            Wallet wallet;
            List<Order> orders;
            List<OrderPackage> packages = new List<OrderPackage>();

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    wallet = await UnitOfWork.WalletRepo.SingleOrDefaultAsync(
                    x => x.Id == model.WalletId && x.IsDelete == false);

                    if (wallet == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Package does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    // Kiểm tra kiện bao hàng có package đã được giao cho khách hàng
                    if (UnitOfWork.WalletRepo.HasPackageGoDelivery(wallet.Id))
                        return JsonCamelCaseResult(new
                        {
                            Status = -1,
                            Text = "Can not edit how the package generated the delivery ticket"
                        }, JsonRequestBehavior.AllowGet);

                    //if (wallet.Status >= (byte)OrderPackageStatus.WaitDelivery)
                    //    return JsonCamelCaseResult(
                    //            new { Status = -2, Text = "Can not edit the edit package created" },
                    //            JsonRequestBehavior.AllowGet);


                    var weightConverted = Math.Round(model.Length * model.Width * model.Height / 5000, 2);
                    var size = $"{wallet.Length}x{wallet.Width:N2}x{wallet.Height:N2}";

                    string note = " (";
                    // Bổ xung note kích thước trước sau cập nhật
                    if (wallet.Size != size)
                    {
                        note += $"Size: {wallet.Size} -> {size}";
                    }

                    // Bổ xung note kích cân nặng trước và sau cập nhật
                    if (wallet.Weight != model.Weight)
                    {
                        note += $"Weight: {wallet.Weight} -> {model.Weight}";
                    }

                    // Bổ xung note kích cân nặng trước và sau cập nhật
                    if (wallet.WeightConverted != weightConverted)
                    {
                        note += $"WeightConverted: {wallet.WeightConverted} -> {weightConverted}";
                    }
                    note += ")";

                    var timeNow = DateTime.Now;

                    wallet.Weight = model.Weight;
                    wallet.Length = model.Length;
                    wallet.Width = model.Width;
                    wallet.Height = model.Height;
                    wallet.WeightConverted = weightConverted;
                    wallet.WeightActual = wallet.Weight > wallet.WeightConverted ? wallet.Weight : wallet.WeightConverted;
                    wallet.Size = size;
                    wallet.Volume = Math.Round(wallet.Length.Value * wallet.Width.Value * wallet.Height.Value / 1000000, 4);
                    wallet.Updated = timeNow;
                    wallet.Note += note;


                    await UnitOfWork.WalletRepo.SaveAsync();

                    // Wallet là bao gỗ -> Cập nhật lại các kiện trong bao
                    if (wallet.Mode == 1)
                    {
                        packages = await UnitOfWork.OrderPackageRepo.GetByWalletId(wallet.Id);

                        // Cập nhật lại cân nặng của kiện (Cập nhật thêm cân nặng phủ bì của kiện gỗ)
                        // Sum cân nặng tính tiền thực của kiện trong kiện gỗ
                        var totalWalletWeight = packages.ToList().Sum(x =>
                            x.Weight > x.WeightConverted ? x.Weight : x.WeightConverted
                        );
                        // Sum thể tích thực tính từ package 
                        var totalWalletVolume = packages.ToList().Sum(x => x.Volume ?? 0);

                        // Thể tích trênh lệnh của bì gỗ 
                        //var walletVolumeWapper = wallet.Volume - totalWalletVolume;
                        // Cộng thêm cân nặng của kiện với cân nặng phủ bì
                        var index = 1;
                        var totalActual = 0M;
                        var totalWapperPercent = 0M;
                        foreach (var orderPackage in packages)
                        {
                            var oldVolumeActual = orderPackage.Volume;

                            // Kiện cuối cùng
                            if (index != packages.Count)
                            {
                                orderPackage.VolumeWapperPercent = Math.Round(orderPackage.Volume.Value * 100 / totalWalletVolume, 4);
                                orderPackage.VolumeActual =
                                   Math.Round(orderPackage.VolumeWapperPercent.Value * wallet.Volume.Value / 100, 4);

                                totalActual += orderPackage.VolumeActual.Value;
                                totalWapperPercent += orderPackage.VolumeWapperPercent.Value;
                            }
                            else
                            {
                                orderPackage.VolumeActual = wallet.Volume.Value - totalActual;
                                orderPackage.VolumeWapperPercent = 100 - totalWapperPercent;
                            }
                            index++;

                            orderPackage.VolumeWapper = orderPackage.VolumeActual - oldVolumeActual;
                        }

                        var ordersWithWeight = new Dictionary<int, decimal>();

                        // Cộng thêm cân nặng của kiện với cân nặng phủ bì
                        index = 1;
                        totalActual = 0M;
                        totalWapperPercent = 0M;

                        foreach (var orderPackage in packages)
                        {
                            var oldActualWeight = orderPackage.WeightActual;

                            // Không phải package cuối cùng trong kiện gỗ
                            if (index != packages.Count)
                            {
                                orderPackage.WeightWapperPercent = Math.Round(orderPackage.WeightActual.Value * 100 / totalWalletWeight.Value, 4);
                                orderPackage.WeightActual = Math.Round(orderPackage.WeightWapperPercent.Value * wallet.WeightActual.Value / 100, 2);
                                totalActual += orderPackage.WeightActual.Value;
                                totalWapperPercent += orderPackage.WeightWapperPercent.Value;
                            }
                            else
                            {
                                orderPackage.WeightActual = wallet.WeightActual.Value - totalActual;
                                orderPackage.WeightWapperPercent = 100 - totalWapperPercent;
                            }

                            index++;

                            // Độ lệch trước và sau Packing gỗ
                            orderPackage.WeightWapper = orderPackage.WeightActual - oldActualWeight;

                            // Add cân nặng phát
                            if (ordersWithWeight.ContainsKey(orderPackage.OrderId))
                            {
                                ordersWithWeight[orderPackage.OrderId] =
                                    ordersWithWeight[orderPackage.OrderId] + orderPackage.WeightWapper ?? 0;
                            }
                            else
                            {
                                ordersWithWeight.Add(orderPackage.OrderId, orderPackage.WeightWapper ?? 0);
                            }
                        }

                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        // Tính Total money Packing gỗ của kiện
                        var packingPriceWallet = OrderRepository.PackingPrice(wallet.WeightActual ?? 0);

                        // Cập nhật lại chi phí Packing gỗ cho các package
                        foreach (var package in packages)
                        {
                            // Tiền chia đều 20 yuan dầu tiên
                            package.TotalPriceWapper = package.WeightWapperPercent * 20 / 100;

                            // Tiền chia đều các yuan tiếp theo
                            if (packingPriceWallet > 20)
                            {
                                package.TotalPriceWapper += (packingPriceWallet - 20) * package.WeightWapperPercent / 100;
                            }
                        }

                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        var orderIds = $";{string.Join(";", ordersWithWeight.Select(x => x.Key).ToList()) };";

                        // Các dịch vụ vận chuyển liên quan
                        var outSideShipServices = await UnitOfWork.OrderServiceRepo.FindAsync(
                                    x => !x.IsDelete && x.Checked &&
                                        x.ServiceId == (byte)OrderServices.OutSideShipping &&
                                        orderIds.Contains(";" + x.OrderId + ";"));

                        // Các dịch vụ Packing liên quan
                        var packingServices = await UnitOfWork.OrderServiceRepo.FindAsync(
                                    x => !x.IsDelete && x.Checked &&
                                        x.ServiceId == (byte)OrderServices.Packing &&
                                        orderIds.Contains(";" + x.OrderId + ";"));

                        //// Các dịch vụ vận chuyển hàng không liên quan
                        //var fastDeliveryServices = await UnitOfWork.OrderServiceRepo.FindAsync(
                        //            x => !x.IsDelete && x.Checked &&
                        //                x.ServiceId == (byte)OrderServices.FastDelivery &&
                        //                orderIds.Contains(";" + x.OrderId + ";"));

                        //// Các dịch vụ vận chuyển hàng không liên quan
                        //var optimalServices = await UnitOfWork.OrderServiceRepo.FindAsync(
                        //    x => !x.IsDelete && x.Checked &&
                        //         x.ServiceId == (byte) OrderServices.Optimal &&
                        //         orderIds.Contains(";" + x.OrderId + ";"));
                        

                        // Các order liên quan
                        var ordersForUpdate =
                            await UnitOfWork.OrderRepo.FindAsync(
                                    x => !x.IsDelete && orderIds.Contains(";" + x.Id + ";"));

                        foreach (var o in ordersForUpdate)
                        {
                            // Cập nhật lại giá dịch vụ Packing gỗ
                            var packingService = packingServices.SingleOrDefault(x => x.OrderId == o.Id);
                            if (packingService != null)
                            {
                                var totalOrderPacking = await UnitOfWork.OrderPackageRepo.Entities
                                    .Where(x => x.OrderId == o.Id && !x.IsDelete)
                                    .SumAsync(x => x.TotalPriceWapper ?? 0);

                                packingService.Value = totalOrderPacking;
                                packingService.TotalPrice = totalOrderPacking * o.ExchangeRate;
                            }

                            #region Kiện bị thêm cân nặng từ phủ bì của kiện gỗ -> Cập nhật lại các dịch vụ liên quan đến cân nạng
                            // Kiện bị thêm cân nặng từ phủ bì của kiện gỗ -> Cập nhật lại các dịch vụ liên quan đến cân nạng
                            var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(o.LevelId);

                            var totalOrderWeight = await UnitOfWork.OrderPackageRepo.Entities
                                .Where(x => x.OrderId == o.Id && !x.IsDelete)
                                .SumAsync(x => x.WeightActual ?? 0);

                            // Cập nhật lại giá dịch vụ vận chuyển TQ -> VN của Order
                            var outSideShipService = outSideShipServices.SingleOrDefault(x => x.OrderId == o.Id);

                            // Cân nặng các package được xuất giao tại TQ
                            var orderWeightIgnore = UnitOfWork.OrderPackageRepo.GetTotalActualWeight(o.Id);

                            // Sum cân nặng tính tiền vc của Order
                            var orderWeight = totalOrderWeight - orderWeightIgnore;

                            if (outSideShipService != null)
                            {
                                //var fastDeliveryService = fastDeliveryServices.SingleOrDefault(x => x.OrderId == o.Id);
                                //var optimalService = optimalServices.SingleOrDefault(x => x.OrderId == o.Id);

                                decimal serviceValue;
                                decimal outSideShipping;

                                // Order ký gửi
                                if (o.Type == (byte)OrderType.Deposit)
                                {
                                    serviceValue = o.ApprovelPrice ?? 0;

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
                                    //        o.WarehouseDeliveryId ?? 0);
                                    //}
                                    //else if (fastDeliveryService != null) // VC nhanh
                                    //{
                                    //    serviceValue = OrderRepository.FastDelivery(orderWeight,
                                    //        o.WarehouseDeliveryId ?? 0);
                                    //}
                                    //else // VC bình thường
                                    //{
                                        serviceValue = OrderRepository.ShippingOutSide(o.ServiceType, orderWeight,
                                            o.WarehouseDeliveryId ?? 0);
                                    //}

                                    outSideShipping = orderWeight * serviceValue;
                                }

                                outSideShipService.Value = serviceValue;

                                outSideShipService.TotalPrice = outSideShipping;

                                if (o.Type == (byte)OrderType.Order)
                                {
                                    // Triết khấu VIP
                                    outSideShipService.TotalPrice -= vipLevel.Ship * outSideShipService.TotalPrice / 100;
                                    outSideShipService.Note = $"Shipping service fee to Vietnam {outSideShipService.Value.ToString("N2", CultureInfo)} Baht/1kg" +
                                                                $" And was discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";
                                }
                            }
                            #endregion
                        }
                    }

                    var strCodeOrder = $";{string.Join(";", packages.Select(x => x.OrderCode).Distinct().ToList())};";

                    orders = await UnitOfWork.OrderRepo.FindAsync(x => strCodeOrder.Contains(";" + x.Code + ";") && !x.IsDelete);

                    var weight = await UnitOfWork.OrderPackageRepo.SumWeightByOrderCodes(strCodeOrder);

                    foreach (var order in orders)
                    {
                        if (weight.ContainsKey(order.Code))
                        {
                            order.TotalWeight = weight[order.Code];
                        }

                        #region Chia tiền dịch vụ phát sinh cho các package theo % cân nạng
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
                                var packages2 = UnitOfWork.OrderPackageRepo.GetByOrderIdAndImportWarehouseId(order.Id,
                                        serviceOther.ObjectId);

                                serviceOther.TotalWeightActual = packages2.Sum(x => x.WeightActual);

                                // Tính lại cân nặng trong kiên hàng
                                foreach (var p in packages2)
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
                                    $"Shipping service fee to Vietnam {serviceValue.ToString("N2", CultureInfo)} Baht/1kg" +
                                    $" And was discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";

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
                                    $"Shipping service fee to Vietnam {serviceValue.ToString("N2", CultureInfo)} Baht/1kg" +
                                    $" And was discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";
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

                        await UnitOfWork.OrderRepo.SaveAsync();
                    }

                    await UnitOfWork.WalletDetailRepo.SaveAsync();

                    // Sum số kiện trong bao
                    wallet.PackageNo = await UnitOfWork.WalletDetailRepo.CountAsync(
                        x => !x.IsDelete && (x.WalletId == wallet.Id));

                    // Sum giá trị của bao
                    wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.Amount != null) && (x.WalletId == wallet.Id))
                        .SumAsync(x => x.Amount);

                    // Sum cân nặng kiện trong bao
                    wallet.TotalWeight = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.Weight != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.Weight);

                    // Sum cân nặng kiện trong bao
                    wallet.TotalWeightActual = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.ActualWeight);

                    // Sum cân nặng chuyển đổi của kiện trong bao
                    wallet.TotalWeightConverted = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.ConvertedWeight != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.ConvertedWeight);

                    // Sum cân nặng tính tiền của kiện trong bao
                    wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.ActualWeight);

                    // Sum thể tích kiện trong bao
                    wallet.TotalVolume = await UnitOfWork.WalletDetailRepo.Entities.Where(
                           x => !x.IsDelete && (x.Volume != null) && (x.WalletId == wallet.Id))
                       .SumAsync(x => x.Volume);

                    // Dach sách mã các kiện trong bao
                    wallet.PackageCodes = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(
                        x => !x.IsDelete && (x.WalletId == wallet.Id)).Select(x => "P" + x.PackageCode).ToList());

                    var str = string.Join(";", UnitOfWork.WalletDetailRepo.Find(
                        x => !x.IsDelete && (x.WalletId == wallet.Id)).ToList().Select(x => x.PackageCode).ToList());
                    wallet.PackageCodesUnsigned = $";{str};";

                    // Danh sách mã Orders trong bao
                    var strOrdersCode = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(x => !x.IsDelete && (x.WalletId == wallet.Id))
                        .Where(x => !x.IsDelete && x.WalletId == wallet.Id)
                        .ToList()
                        .Select(x => string.IsNullOrWhiteSpace(x.OrderCodes) ? x.OrderCode : x.OrderCodes));

                    // Cập nhật lại các dịch vụ trong bao hàng
                    var orderServices = await UnitOfWork.WalletRepo.GetOrderServiceByWalletId(wallet.Id);

                    wallet.OrderServicesJson = JsonConvert.SerializeObject(orderServices,
                        new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                    wallet.OrderServices = $";{string.Join(";", orderServices.Select(x => x.ServiceId).ToList())};";

                    var ordersCode = strOrderCode.Replace("ORD", "").Split(',').ToList().Select(x => x.Trim()).Distinct().ToList();

                    if (ordersCode.Any())
                    {
                        wallet.OrderCodes = string.Join(", ", ordersCode.Select(x => "ORD" + x));

                        wallet.OrderCodesUnsigned = $";{string.Join(";", ordersCode)};";

                        orders = UnitOfWork.OrderRepo.Find(
                            x => !x.IsDelete && wallet.OrderCodesUnsigned.Contains(";" + x.Code + ";")).ToList();

                        wallet.Customers = string.Join(", ", orders.Select(x => x.CustomerEmail).Distinct().ToList());
                        wallet.CustomersUnsigned = $";{string.Join(";", orders.Select(x => x.CustomerEmail).Distinct().ToList())};";
                    }

                    await UnitOfWork.WalletRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            // Cập nhật lại tiền phải thu nếu là bao gỗ
            if (wallet.Mode == 1)
            {
                var orderIds2 = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

                BackgroundJob.Enqueue(() => OrderJob.ProcessDebitReport(orderIds2));
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "Update weight successfully" }, JsonRequestBehavior.AllowGet);
        }

        //public async Task<int> FixErrorPackage()
        //{
        //    using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var wallets = await UnitOfWork.WalletRepo.FindAsync(x => x.IsDelete == false && x.TotalWeight == null);

        //            foreach (var wallet in wallets)
        //            {
        //                // Sum số kiện trong bao
        //                wallet.PackageNo = await UnitOfWork.WalletDetailRepo.CountAsync(
        //                    x => !x.IsDelete && (x.WalletId == wallet.Id));

        //                // Sum giá trị của bao
        //                wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                        x => !x.IsDelete && (x.Amount != null) && (x.WalletId == wallet.Id))
        //                    .SumAsync(x => x.Amount);

        //                // Sum cân nặng kiện trong bao
        //                wallet.TotalWeight = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                        x => !x.IsDelete && (x.Weight != null) && (x.WalletId == wallet.Id))
        //                    .SumAsync(x => x.Weight);

        //                // Sum cân nặng kiện trong bao
        //                wallet.TotalWeightActual = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                        x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
        //                    .SumAsync(x => x.ActualWeight);

        //                // Sum cân nặng chuyển đổi của kiện trong bao
        //                wallet.TotalWeightConverted = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                        x => !x.IsDelete && (x.ConvertedWeight != null) && (x.WalletId == wallet.Id))
        //                    .SumAsync(x => x.ConvertedWeight);

        //                // Sum cân nặng tính tiền của kiện trong bao
        //                wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                        x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
        //                    .SumAsync(x => x.ActualWeight);

        //                // Sum thể tích kiện trong bao
        //                wallet.TotalVolume = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                        x => !x.IsDelete && (x.Volume != null) && (x.WalletId == wallet.Id))
        //                    .SumAsync(x => x.Volume);

        //                // Dach sách mã các kiện trong bao
        //                wallet.PackageCodes = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(
        //                    x => !x.IsDelete && (x.WalletId == wallet.Id)).Select(x => "P" + x.PackageCode).ToList());

        //                await UnitOfWork.WalletRepo.SaveAsync();

        //                var str = string.Join(";", UnitOfWork.WalletDetailRepo.Find(
        //                    x => !x.IsDelete && (x.WalletId == wallet.Id)).ToList().Select(x => x.PackageCode).ToList());
        //                wallet.PackageCodesUnsigned = $";{str};";

        //                // Danh sách mã Orders trong bao
        //                var strOrdersCode = string.Join(", ",
        //                    UnitOfWork.WalletDetailRepo.Find(x => !x.IsDelete && (x.WalletId == wallet.Id))
        //                        .Where(x => !x.IsDelete && x.WalletId == wallet.Id)
        //                        .ToList()
        //                        .Select(x => string.IsNullOrWhiteSpace(x.OrderCodes) ? x.OrderCode : x.OrderCodes));

        //                var ordersCode =
        //                    strOrderCode.Replace("ORD", "").Split(',').ToList().Select(x => x.Trim()).Distinct().ToList();

        //                var timeNow = DateTime.Now;

        //                if (ordersCode.Any())
        //                {
        //                    wallet.OrderCodes = string.Join(", ", ordersCode.Select(x => "ORD" + x));

        //                    wallet.OrderCodesUnsigned = $";{string.Join(";", ordersCode)};";

        //                    var orders = UnitOfWork.OrderRepo.Find(
        //                        x => !x.IsDelete && wallet.OrderCodesUnsigned.Contains(";" + x.Code + ";")).ToList();

        //                    wallet.Customers = string.Join(", ", orders.Select(x => x.CustomerEmail).Distinct().ToList());
        //                    wallet.CustomersUnsigned =
        //                        $";{string.Join(";", orders.Select(x => x.CustomerEmail).Distinct().ToList())};";

        //                    //wallet.Note = "Khách hàng: " + string.Join(", ", orders.Select(x => x.CustomerName + "(" + x.CustomerEmail + ")").Distinct().ToList());

        //                    // Thêm tiền dịch vụ khác cho đơn  hàng
        //                    //Tính lại tiền dịch vụ phát sinh trong Order
        //                    foreach (var order in orders)
        //                    {
        //                        // Tính Total money dịch vụ khác
        //                        var totalServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(x => x.OrderId == order.Id);

        //                        var otherService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
        //                            x => !x.IsDelete && x.OrderId == order.Id &&
        //                                 x.ServiceId == (byte)OrderServices.Other && x.Checked);

        //                        if (otherService == null)
        //                        {
        //                            otherService = new OrderService
        //                            {
        //                                IsDelete = false,
        //                                Checked = true,
        //                                Created = timeNow,
        //                                LastUpdate = timeNow,
        //                                Value = totalServiceOther.Sum(x => x.Value),
        //                                Currency = Currency.CNY.ToString(),
        //                                ExchangeRate = order.ExchangeRate,
        //                                TotalPrice = totalServiceOther.Sum(x => x.TotalPrice),
        //                                HashTag = string.Empty,
        //                                Mode = (byte)OrderServiceMode.Option,
        //                                OrderId = order.Id,
        //                                ServiceId = (byte)OrderServices.Other,
        //                                ServiceName = (OrderServices.Other).GetAttributeOfType<DescriptionAttribute>()
        //                                    .Description,
        //                                Type = (byte)UnitType.Money,
        //                                Note =
        //                                    $"Service fees incurred. (Shop shipment goods after goods into warehouse, rent forklift,...)",
        //                            };

        //                            UnitOfWork.OrderServiceRepo.Add(otherService);
        //                        }
        //                        else
        //                        {
        //                            otherService.LastUpdate = timeNow;
        //                            otherService.Value = totalServiceOther.Sum(x => x.Value);
        //                            otherService.TotalPrice = totalServiceOther.Sum(x => x.TotalPrice);
        //                        }
        //                    }

        //                    await UnitOfWork.OrderServiceOtherRepo.SaveAsync();

        //                    foreach (var order in orders)
        //                    {
        //                        var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
        //                                                                                 x.IsDelete == false && x.Checked)
        //                            .ToList()
        //                            .Sum(x => x.TotalPrice);

        //                        order.Total = order.TotalExchange + totalService;
        //                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);
        //                    }
        //                }
        //            }

        //            transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            ExceptionDispatchInfo.Capture(ex).Throw();
        //        }
        //    }

        //    var wallets2 = await UnitOfWork.WalletRepo.FindAsync(x => x.IsDelete == false && x.TotalWeight == null);

        //    foreach (var wallet in wallets2)
        //    {
        //        var ordersCodes =
        //            $";{string.Join(";", UnitOfWork.WalletDetailRepo.Find(x => x.IsDelete == false && x.WalletId == wallet.Id).Select(x => x.OrderCode).Distinct().ToList())};";

        //        var orders =
        //            UnitOfWork.OrderRepo.Find(x => x.IsDelete == false && ordersCodes.Contains(";" + x.Code + ";"));


        //        // Tính toán chi phí phát sinh cảu Order  
        //        if (orders != null)
        //            BackgroundJob.Enqueue(() => PackageJob.UpdateOtherServiceOfPackage(orders.Select(x => x.Id).ToList()));

        //        // Packing gỗ
        //        if (orders != null && wallet.Mode == 1)
        //        {
        //            // Job cập nhật thông tin package
        //            BackgroundJob.Enqueue(() => PackageJob.UpdateWeightActualPercent(orders.Select(x => x.Id).ToList()));
        //        }
        //    }

        //    return 1;
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CheckPermission(EnumAction.Update, EnumPage.Wallet)]
        //public async Task<ActionResult> Update(WalletMeta model)
        //{
        //    ModelState.Remove("Packages");

        //    if (!ModelState.IsValid)
        //        return JsonCamelCaseResult(new {Status = -1, Text = "Data format is incorrect"},
        //            JsonRequestBehavior.AllowGet);


        //    var wallet = await UnitOfWork.WalletRepo.SingleOrDefaultAsync(x => (x.Id == model.Id) && !x.IsDelete);

        //    if (wallet == null)
        //        return JsonCamelCaseResult(new {Status = -2, Text = "Goods received note does not exist or has been deleted"},
        //            JsonRequestBehavior.AllowGet);

        //    var targetWarehouse = await UnitOfWork.OfficeRepo.SingleOrDefaultAsNoTrackingAsync(
        //        x =>
        //            (x.Id == model.TargetWarehouseId) && !x.IsDelete &&
        //            (x.Status == (byte)OfficeStatus.Use));

        //    if (targetWarehouse == null)
        //        return
        //            JsonCamelCaseResult(
        //                new { Status = -3, Text = "Kho hàng đến does not exist or has been deleted" },
        //                JsonRequestBehavior.AllowGet);


        //    wallet.TargetWarehouseId = targetWarehouse.Id;
        //    wallet.TargetWarehouseName = targetWarehouse.Name;
        //    wallet.TargetWarehouseIdPath = targetWarehouse.IdPath;
        //    wallet.TargetWarehouseAddress = targetWarehouse.Address;

        //    wallet.Note = model.Note;
        //    wallet.Updated = DateTime.Now;
        //    wallet.Status = model.Status;
        //    wallet.Width = model.Width;
        //    wallet.Height = model.Height;
        //    wallet.Length = model.Length;
        //    wallet.Size =
        //        $"{model.Width.ToString("N2", CultureInfo)}x{model.Length.ToString("N2", CultureInfo)}x{model.Height.ToString("N2", CultureInfo)}";
        //    wallet.Weight = model.Weight;
        //    wallet.WeightConverted = model.Width * model.Length * model.Height / 5000;
        //    wallet.Volume = model.Width * model.Length * model.Height / 1000000;
        //    wallet.WeightActual = wallet.Weight > wallet.WeightConverted
        //        ? wallet.Weight
        //        : wallet.WeightConverted;

        //    await UnitOfWork.WalletRepo.SaveAsync();

        //    return JsonCamelCaseResult(new {Status = 1, Text = "Goods received note updated successfully"},
        //        JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CheckPermission(EnumAction.Update, EnumPage.Wallet)]
        //public async Task<ActionResult> AddPackage(WalletDetailMeta model)
        //{
        //    if (!ModelState.IsValid)
        //        return JsonCamelCaseResult(new {Status = -1, Text = "Data format is incorrect"},
        //            JsonRequestBehavior.AllowGet);


        //    if (!model.WalletId.HasValue)
        //        return JsonCamelCaseResult(
        //            new {Status = -1, Text = "Yêu cầu nhập kho này does not exist or has been deleted"},
        //            JsonRequestBehavior.AllowGet);

        //    using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var wallet =
        //                await UnitOfWork.WalletRepo.SingleOrDefaultAsync(
        //                    x => (x.Id == model.WalletId.Value) && !x.IsDelete);

        //            if (wallet == null)
        //                return JsonCamelCaseResult(
        //                    new {Status = -1, Text = "Yêu cầu nhập kho này does not exist or has been deleted"},
        //                    JsonRequestBehavior.AllowGet);

        //            var timeNow = DateTime.Now;

        //            var p =
        //                await
        //                    UnitOfWork.OrderPackageRepo.SingleOrDefaultAsNoTrackingAsync(
        //                        x => (x.Id == model.PackageId) && !x.IsDelete);

        //            if (p == null)
        //                return JsonCamelCaseResult(
        //                    new {Status = -2, Text = "package does not exist or has been deleted"},
        //                    JsonRequestBehavior.AllowGet);

        //            UnitOfWork.WalletDetailRepo.Add(new WalletDetail
        //            {
        //                Created = timeNow,
        //                Updated = timeNow,
        //                IsDelete = false,
        //                Note = model.Note,
        //                PackageId = p.Id,
        //                PackageCode = p.Code,
        //                OrderCode = p.OrderCode,
        //                OrderId = p.OrderId,
        //                OrderServices = p.OrderServices,
        //                Status = 1,
        //                TransportCode = p.TransportCode,
        //                WalletCode = wallet.Code,
        //                WalletId = wallet.Id,
        //                Weight = p.Weight,
        //                ConvertedWeight = p.WeightConverted,
        //                ActualWeight = p.WeightActual,
        //                Amount = p.TotalPrice,
        //                Customers = p.Customers ?? p.CustomerUserName,
        //                CustomersUnsigned = p.CustomersUnsigned ?? $";{p.CustomerUserName};",
        //                PackageCodes = p.PackageCodes,
        //                PackageCodesUnsigned = p.PackageCodesUnsigned,
        //                OrderCodes = p.OrderCodes,
        //                OrderCodesUnsigned = p.OrderCodesUnsigned,
        //                OrderPackageNo = p.PackageNo,
        //                Volume = p.Volume,
        //                OrderType = p.OrderType
        //            });

        //            // Thêm kiện || Bao vào yêu cầu nhập kho
        //            await UnitOfWork.WalletDetailRepo.SaveAsync();

        //            // Sum số kiện trong bao
        //            wallet.PackageNo = await UnitOfWork.WalletDetailRepo.CountAsync(
        //                x => !x.IsDelete && (x.WalletId == wallet.Id));

        //            // Sum giá trị của bao
        //            wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                    x => !x.IsDelete && (x.Amount != null) && (x.WalletId == wallet.Id))
        //                .SumAsync(x => x.Amount);

        //            // Sum cân nặng kiện trong bao
        //            wallet.TotalWeight = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.Weight != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.Weight);

        //            // Sum cân nặng kiện trong bao
        //            wallet.TotalWeightActual = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.ActualWeight);

        //            // Sum cân nặng chuyển đổi của kiện trong bao
        //            wallet.TotalWeightConverted = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.ConvertedWeight != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.ConvertedWeight);

        //            // Sum cân nặng tính tiền của kiện trong bao
        //            wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.ActualWeight);

        //            // Sum thể tích kiện trong bao
        //            wallet.TotalVolume = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.Volume != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.Volume);

        //            wallet.PackageCodes = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(
        //                x => !x.IsDelete && (x.WalletId == wallet.Id)).Select(x => "P" + x.PackageCode).ToList());

        //            var str = string.Join(";", UnitOfWork.WalletDetailRepo.Find(
        //                x => !x.IsDelete && (x.WalletId == wallet.Id)).ToList().Select(x => x.PackageCode).ToList());
        //            wallet.PackageCodesUnsigned = $";{str};";

        //            var strOrderCode = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(
        //                    x => !x.IsDelete && (x.WalletId == wallet.Id))
        //                .Where(x => !x.IsDelete && x.WalletId == 1)
        //                .ToList()
        //                .Select(x => string.IsNullOrWhiteSpace(x.OrderCodes) ? x.OrderCode : x.OrderCodes));

        //            var ordersCode = strOrderCode.Replace("ORD", "").Split(',').ToList().Select(x => x.Trim()).Distinct();

        //            wallet.OrderCodes = string.Join(", ", ordersCode.Select(x => "ORD" + x));

        //            wallet.OrderCodesUnsigned = $";{string.Join(";", ordersCode)};";

        //            var orders = UnitOfWork.OrderRepo.Find(
        //                x => !x.IsDelete && wallet.OrderCodesUnsigned.Contains(";" + x.Code + ";")).ToList();

        //            wallet.Customers = string.Join(", ", orders.Select(x => x.CustomerEmail).Distinct().ToList());
        //            wallet.CustomersUnsigned = $";{string.Join(";", orders.Select(x => x.CustomerEmail).Distinct().ToList())};";

        //            wallet.Note = "Khách hàng: " + string.Join(", ", orders.Select(x => x.CustomerName + "(" + x.CustomerEmail + ")").Distinct().ToList());

        //            await UnitOfWork.WalletRepo.SaveAsync();

        //            transaction.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }

        //    return JsonCamelCaseResult(
        //        new {Status = 1, Text = $"Thêm package thành công"},
        //        JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CheckPermission(EnumAction.Update, EnumPage.Wallet)]
        //public async Task<ActionResult> UpdatePackage(WalletDetailMeta model)
        //{
        //    if (!ModelState.IsValid)
        //        return JsonCamelCaseResult(new {Status = -1, Text = "Data format is incorrect"},
        //            JsonRequestBehavior.AllowGet);

        //    var p = await UnitOfWork.WalletDetailRepo.SingleOrDefaultAsync(x => (x.Id == model.Id) && !x.IsDelete);

        //    if (p == null)
        //        return JsonCamelCaseResult(
        //            new { Status = -2, Text = "package does not exist or has been deleted" },
        //            JsonRequestBehavior.AllowGet);

        //    // Cập nhật thông tin kho hiện tại cho package
        //    p.Note = model.Note;
        //    p.Updated = DateTime.Now;

        //    await UnitOfWork.WalletDetailRepo.SaveAsync();

        //    return JsonCamelCaseResult(
        //        new {Status = 1, Text = "Updated successfully"},
        //        JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CheckPermission(EnumAction.Update, EnumPage.Wallet)]
        //public async Task<ActionResult> DeletePackage(WalletDetailMeta model)
        //{
        //    if (!ModelState.IsValid)
        //        return JsonCamelCaseResult(new { Status = -1, Text = "Data format is incorrect" },
        //            JsonRequestBehavior.AllowGet);


        //    if (!model.WalletId.HasValue)
        //        return JsonCamelCaseResult(
        //            new { Status = -1, Text = "Bao hàng này does not exist or has been deleted" },
        //            JsonRequestBehavior.AllowGet);

        //    using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var p =
        //                await UnitOfWork.WalletDetailRepo.SingleOrDefaultAsync(x => (x.Id == model.Id) && !x.IsDelete);

        //            if (p == null)
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = "package does not exist or has been deleted" },
        //                    JsonRequestBehavior.AllowGet);

        //            var wallet =
        //                await UnitOfWork.WalletRepo.SingleOrDefaultAsync(
        //                    x => (x.Id == p.WalletId) && !x.IsDelete);

        //            if (wallet == null)
        //                return JsonCamelCaseResult(
        //                    new { Status = -1, Text = "Bao hàng này does not exist or has been deleted" },
        //                    JsonRequestBehavior.AllowGet);


        //            // Cập nhật thông tin kho hiện tại cho package
        //            p.IsDelete = true;
        //            p.Updated = DateTime.Now;

        //            await UnitOfWork.WalletDetailRepo.SaveAsync();

        //            // Sum số kiện trong bao
        //            wallet.PackageNo = await UnitOfWork.WalletDetailRepo.CountAsync(
        //                x => !x.IsDelete && (x.WalletId == wallet.Id));

        //            // Sum giá trị của bao
        //            wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                    x => !x.IsDelete && (x.Amount != null) && (x.WalletId == wallet.Id))
        //                .SumAsync(x => x.Amount);

        //            // Sum cân nặng kiện trong bao
        //            wallet.TotalWeight = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.Weight != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.Weight);

        //            // Sum cân nặng kiện trong bao
        //            wallet.TotalWeightActual = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.ActualWeight);

        //            // Sum cân nặng chuyển đổi của kiện trong bao
        //            wallet.TotalWeightConverted = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.ConvertedWeight != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.ConvertedWeight);

        //            // Sum cân nặng tính tiền của kiện trong bao
        //            wallet.TotalValue = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.ActualWeight != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.ActualWeight);

        //            // Sum thể tích kiện trong bao
        //            wallet.TotalVolume = await UnitOfWork.WalletDetailRepo.Entities.Where(
        //                   x => !x.IsDelete && (x.Volume != null) && (x.WalletId == wallet.Id))
        //               .SumAsync(x => x.Volume);

        //            wallet.PackageCodes = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(
        //                x => !x.IsDelete && (x.WalletId == wallet.Id)).Select(x => "P" + x.PackageCode).ToList());

        //            var str = string.Join(";", UnitOfWork.WalletDetailRepo.Find(
        //                x => !x.IsDelete && (x.WalletId == wallet.Id)).ToList().Select(x => x.PackageCode).ToList());
        //            wallet.PackageCodesUnsigned = $";{str};";

        //            var strOrderCode = string.Join(", ", UnitOfWork.WalletDetailRepo.Find(
        //                    x => !x.IsDelete && (x.WalletId == wallet.Id))
        //                .Where(x => !x.IsDelete && x.WalletId == 1)
        //                .ToList()
        //                .Select(x => string.IsNullOrWhiteSpace(x.OrderCodes) ? x.OrderCode : x.OrderCodes));

        //            var ordersCode = strOrderCode.Replace("ORD", "").Split(',').ToList().Select(x => x.Trim()).Distinct();

        //            wallet.OrderCodes = string.Join(", ", ordersCode.Select(x => "ORD" + x));

        //            wallet.OrderCodesUnsigned = $";{string.Join(";", ordersCode)};";

        //            var orders = UnitOfWork.OrderRepo.Find(
        //                x => !x.IsDelete && wallet.OrderCodesUnsigned.Contains(";" + x.Code + ";")).ToList();

        //            wallet.Customers = string.Join(", ", orders.Select(x => x.CustomerEmail).Distinct().ToList());
        //            wallet.CustomersUnsigned = $";{string.Join(";", orders.Select(x => x.CustomerEmail).Distinct().ToList())};";

        //            wallet.Note = "Khách hàng: " + string.Join(", ", orders.Select(x => x.CustomerName + "(" + x.CustomerEmail + ")").Distinct().ToList());

        //            await UnitOfWork.WalletRepo.SaveAsync();

        //            transaction.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //    return JsonCamelCaseResult(
        //        new { Status = 1, Text = "Xóa package thành công" },
        //        JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public async Task<ActionResult> ExcelTrackingPackageWallet(string warehouseIdPath, byte? orderServiceId, byte? timeType, 
            byte? type, int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, int? entrepotId, string keyword = "", byte mode = 0)
        {
            var ngay = "";
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            // Query bao hàng được tạo trong kho
            Expression<Func<Wallet, bool>> queryCreated =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                        (type == null || x.Mode == type) && (entrepotId == null || x.EntrepotId == entrepotId) &&
                     !x.IsDelete && ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && ((x.CreatedWarehouseIdPath == warehouseIdPath) ||
                                     x.CreatedWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.CreatedWarehouseIdPath == warehouseIdPath))) &&
                      (orderServiceId == null || x.OrderServices.Contains(";" + orderServiceId + ";"));

            // Query bao hàng đang trong kho
            Expression<Func<Wallet, bool>> queryInStock =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && (type == null || x.Mode == type) && (entrepotId == null || x.EntrepotId == entrepotId) &&
                     ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager &&
                       ((x.CurrentWarehouseIdPath == warehouseIdPath) ||
                        x.CurrentWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.CurrentWarehouseIdPath == warehouseIdPath))) &&
                      (orderServiceId == null || x.OrderServices.Contains(";" + orderServiceId + ";"));

            // Query chờ nhập kho
            Expression<Func<Wallet, bool>> queryWaitImport =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && (type == null || x.Mode == type) && (entrepotId == null || x.EntrepotId == entrepotId) &&
                     ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && x.Status == (byte)WalletStatus.Shipping && ((x.TargetWarehouseIdPath == warehouseIdPath) || x.TargetWarehouseIdPath.StartsWith(warehouseIdPath + ".")))
                     || (!isManager && x.Status == (byte)WalletStatus.Shipping && (x.TargetWarehouseIdPath == warehouseIdPath))) &&
                     (orderServiceId == null || x.OrderServices.Contains(";" + orderServiceId + ";"));

            // Query chờ nhập kho
            Expression<Func<Wallet, bool>> queryAll =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && (type == null || x.Mode == type) && (entrepotId == null || x.EntrepotId == entrepotId) &&
                     ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && ((x.CreatedWarehouseIdPath == warehouseIdPath)
                                     || x.CreatedWarehouseIdPath.StartsWith(warehouseIdPath + ".")))
                      || (!isManager && (x.CreatedWarehouseIdPath == warehouseIdPath)) ||
                      (isManager && ((x.TargetWarehouseIdPath == warehouseIdPath)
                                     || x.TargetWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.TargetWarehouseIdPath == warehouseIdPath)) ||
                      (isManager && ((x.CurrentWarehouseIdPath == warehouseIdPath)
                                     || x.CurrentWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.CurrentWarehouseIdPath == warehouseIdPath))) &&
                      (orderServiceId == null || x.OrderServices.Contains(";" + orderServiceId + ";"));

            Expression<Func<WalletResult, bool>> query = x => timeType == null && (
                fromDate == null && toDate == null
                || fromDate != null && toDate != null && x.Created >= fromDate && x.Created <= toDate
                || fromDate == null && toDate.HasValue && x.Created <= toDate
                || toDate == null && fromDate.HasValue && x.Created >= fromDate)
                || timeType != null &&
                (fromDate == null && toDate == null
                || fromDate != null && toDate != null && x.ImportedTime >= fromDate && x.ImportedTime <= toDate
                || fromDate == null && toDate.HasValue && x.ImportedTime <= toDate
                || toDate == null && fromDate.HasValue && x.ImportedTime >= fromDate);

            // Kho tạo bao
            List<WalletResult> wallets = new List<WalletResult>();

            if (mode == 0)
                wallets = await UnitOfWork.WalletRepo.SearchForExportAsync(queryCreated, query);

            // Bao đang trong kho
            if (mode == 1)
                wallets = await UnitOfWork.WalletRepo.SearchForExportAsync(queryInStock, query);

            // Chờ nhập kho
            if (mode == 2)
                wallets = await UnitOfWork.WalletRepo.SearchForExportAsync(queryWaitImport, query);

            // Tất cả
            if (mode == 3)
                wallets = await UnitOfWork.WalletRepo.SearchForExportAsync(queryAll, query);


            var orderService = new List<dynamic>()
            {
                //new {ServiceId = (byte) OrderServices.Optimal, ServiceName = "Chuyển tiết kiệm"},
                //new {ServiceId = (byte) OrderServices.FastDelivery, ServiceName = "Chuyển nhanh"},
                new {ServiceId = (byte) OrderServices.Packing, ServiceName = "packing"},
                new {ServiceId = (byte) OrderServices.Audit, ServiceName = "tally"}
            };

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col++, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Package code", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "WEIGHT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Weight conversion", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Weigh current", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Created date", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "INPUT DAY", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Warehouse create bag", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Transit point", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Service", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Destination warehouse", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "The number of package", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "DANH SÁCH BAO", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                if(fromDate == null || toDate == null)
                {
                    ngay = "";
                }
                else
                {
                    ngay = "Từ: " + fromDate.Value.ToShortDateString() + " đến " + toDate.Value.ToShortDateString();
                }

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"{ngay}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (wallets.Any())
                {
                    foreach (var package in wallets)
                    {

                        col = 1;

                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col++, no - row, ExcelHorizontalAlignment.Center, true);
                        //Ma bao
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.Code);
                        //cân nặng
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.Weight, ExcelHorizontalAlignment.Right, true);
                        //quy đổi
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.WeightConverted, ExcelHorizontalAlignment.Left, true);
                        // Cân nặng tính tiền
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.WeightActual, ExcelHorizontalAlignment.Left, true);
                        //ngày tạo
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        //ngày nhập bao
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.ImportedTime?.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        //kho tạo bao
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.CreatedWarehouseName);
                        //điểm trung chuyển
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.EntrepotName);

                        // Dịch vụ
                        var services =
                            orderService.Where(x => package.OrderServices.Contains(";" + x.ServiceId + ";"))
                                .Select(x => x.ServiceName)
                                .ToList();

                        ExcelHelper.CreateCellTable(sheet, no, col++,
                            services.Any() ? string.Join(", ", services) : string.Empty);

                        //kho đích
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.TargetWarehouseName);
                        //Số lượng kiện
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.PackageNo, ExcelHorizontalAlignment.Right, true);
                        //Ghi chú
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.Note);

                        no++;
                    }
                }

                ExcelHelper.CreateColumnAutofit(sheet, 1, col);
                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"DanhSachBao_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        public async Task<ActionResult> FixOrderService()
        {

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var wallets = await UnitOfWork.WalletRepo.FindAsync(x => x.IsDelete == false);

                    foreach (var wallet in wallets)
                    {
                        // Cập nhật lại các dịch vụ trong bao hàng
                        var orderServices = await UnitOfWork.WalletRepo.GetOrderServiceByWalletId(wallet.Id);

                        wallet.OrderServicesJson = JsonConvert.SerializeObject(orderServices,
                            new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                        wallet.OrderServices = $";{string.Join(";", orderServices.Select(x => x.ServiceId).ToList())};";
                    }

                    await UnitOfWork.WalletRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        }
    }
}