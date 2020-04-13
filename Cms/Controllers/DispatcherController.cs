using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    /// <summary>
    /// Điều vận
    /// </summary>
    [Authorize]
    public class DispatcherController : BaseController
    {
        // GET: Dispatcher
        [LogTracker(EnumAction.View, EnumPage.Dispatcher)]
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

            ViewBag.StatesSelectListItem = Enum.GetValues(typeof(DispatcherStatus))
                .Cast<DispatcherStatus>()
                .Select(
                    v =>
                        new SelectListItem
                        {
                            Value = ((byte) v).ToString(),
                            Text = EnumHelper.GetEnumDescription<DispatcherStatus>((int) v),
                            Selected = v == DispatcherStatus.Approved
                        })
                .ToList();

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(DispatcherStatus))
                .Cast<DispatcherStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<DispatcherStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            ViewBag.WalletStatus = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            ViewBag.TransportMethods =
                JsonConvert.SerializeObject(
                    await UnitOfWork.TransportMethodRepo.FindAsync(x => x.IsDelete == false && x.Status == 1),
                    jsonSerializerSettings);

            ViewBag.TransportPartners =
                JsonConvert.SerializeObject(
                    await UnitOfWork.PartnerRepo.FindAsync(x => x.IsDelete == false && x.Status == 1, x => x.OrderBy(y => y.PriorityNo), null),
                    jsonSerializerSettings);

            ViewBag.WalletStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            ViewBag.Entrepots = JsonConvert.SerializeObject(
                    await UnitOfWork.EntrepotRepo.FindAsync(x => x.IsDelete == false && x.Status == 1),
                    jsonSerializerSettings);

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.Dispatcher)]
        public async Task<ActionResult> Search(string warehouseIdPath, int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, string keyword = "",
            int currentPage = 1, int recordPerPage = 20, byte mode = 0)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            long totalRecord = 0;

            // Query bao hàng được tạo trong kho
            Expression<Func<Dispatcher, bool>> queryFrom =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && ((x.FromWarehouseIdPath == warehouseIdPath) ||
                                     x.FromWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.FromWarehouseIdPath == warehouseIdPath))) &&
                     (((fromDate == null) && (toDate == null)) ||
                      ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                      ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                      ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)));

            // Query bao hàng đang trong kho
            Expression<Func<Dispatcher, bool>> queryTo =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete &&
                     ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager &&
                       ((x.ToWarehouseIdPath == warehouseIdPath) ||
                        x.ToWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.ToWarehouseIdPath == warehouseIdPath))) &&
                     (((fromDate == null) && (toDate == null)) ||
                      ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                      ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                      ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)));


            // Query chờ nhập kho
            Expression<Func<Dispatcher, bool>> queryAll =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete &&
                     ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && ((x.ToWarehouseIdPath == warehouseIdPath)
                                     || x.ToWarehouseIdPath.StartsWith(warehouseIdPath + ".")))
                      || (!isManager && (x.ToWarehouseIdPath == warehouseIdPath)) ||
                      (isManager && ((x.FromWarehouseIdPath == warehouseIdPath)
                                     || x.FromWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.FromWarehouseIdPath == warehouseIdPath))) &&
                     (((fromDate == null) && (toDate == null)) ||
                      ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                      ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                      ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)));

            Expression<Func<DispatcherResult, bool>> queryResult =
                result => result.WalletMode == 0 &&
                    (result.UnsignedText.Contains(keyword) || result.WalletUnsignedText.Contains(keyword));

            // Kho tạo bao
            List<DispatcherResult> dispatchers = new List<DispatcherResult>();
            if (mode == 0)
                dispatchers = await UnitOfWork.DispatcherRepo.Search(queryFrom, queryResult,
                    currentPage, recordPerPage, out totalRecord);

            // Bao đang trong kho
            if (mode == 1)
                dispatchers = await UnitOfWork.DispatcherRepo.Search(queryTo, queryResult,
                    currentPage, recordPerPage, out totalRecord);

            // Tất cả
            if (mode == 2)
                dispatchers = await UnitOfWork.DispatcherRepo.Search(queryAll, queryResult,
                    currentPage, recordPerPage, out totalRecord);

            // Count group
            var fromNo = mode == 0 ? totalRecord : await UnitOfWork.DispatcherRepo.LongCount(queryFrom, queryResult);
            var toNo = mode == 1 ? totalRecord : await UnitOfWork.DispatcherRepo.LongCount(queryTo, queryResult);
            var allNo = mode == 2 ? totalRecord : await UnitOfWork.DispatcherRepo.LongCount(queryAll, queryResult);

            return JsonCamelCaseResult(
                new
                {
                    items = dispatchers,
                    totalRecord,
                    mode =
                    new
                    {
                        fromNo,
                        toNo,
                        allNo
                    }
                }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Dispatcher)]
        public async Task<ActionResult> GetDetail(int id)
        {
            var data = await UnitOfWork.DispatcherRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            return JsonCamelCaseResult(data, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Dispatcher)]
        public async Task<ActionResult> GetWallets(int id)
        {
            var dispatcher =
                await UnitOfWork.DispatcherRepo.SingleOrDefaultAsNoTrackingAsync(x => (x.Id == id) && !x.IsDelete);

            if (dispatcher == null)
                return JsonCamelCaseResult(new {Status = -1, Text = "Transfer note does not exist or has been deleted"},
                    JsonRequestBehavior.AllowGet);

            //if (UserState.OfficeId.HasValue && importWarehouse.WarehouseId != UserState.OfficeId.Value)
            //    return JsonCamelCaseResult(new { Status = -2, Text = "Bạn không phải là nhân viên kho này" }, JsonRequestBehavior.AllowGet);

            var items =
                await UnitOfWork.DispatcherDetailRepo.FindAsNoTrackingAsync(x => (x.DispatcherId == id) && !x.IsDelete,
                    x => x.OrderBy(y => y.Id), null);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.Dispatcher)]
        public async Task<ActionResult> Add(DispatcherMeta model)
        {
            if (UserState.OfficeType != 1)
                return JsonCamelCaseResult(
                    new {Status = -2, Text = "Only warehouse staff can perform this action"},
                    JsonRequestBehavior.AllowGet);

            if (!ModelState.IsValid)
                return JsonCamelCaseResult(new {Status = -1, Text = "Data format is incorrect"},
                    JsonRequestBehavior.AllowGet);

            var timeNow = DateTime.Now;

            var codes = new List<string>();
            var codeOrder = new List<string>();

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var transportParner =
                        await
                            UnitOfWork.PartnerRepo.SingleOrDefaultAsync(
                                x => x.Id == model.TransportPartnerId && x.IsDelete == false);

                    if(transportParner == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "The transfer partner does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

                    var transportMethod =
                        await
                            UnitOfWork.TransportMethodRepo.SingleOrDefaultAsync(
                                x => x.Id == model.TransportMethodId && x.IsDelete == false);

                    if (transportMethod == null)
                        return
                            JsonCamelCaseResult(
                                new {Status = -1, Text = "The transfer method does not exist or has been deleted" },
                                JsonRequestBehavior.AllowGet);

                    var dispatcher = new Dispatcher
                    {
                        Created = timeNow,
                        Updated = timeNow,
                        Status = model.Status,
                        UserId = UserState.UserId,
                        UserName = UserState.UserName,
                        UserFullName = UserState.FullName,
                        Note = model.Note,
                        Code = string.Empty,
                        UnsignedText = string.Empty,
                        IsDelete = false,
                        FromWarehouseAddress = UserState.OfficeAddress,
                        FromWarehouseId = UserState.OfficeId,
                        FromWarehouseName = UserState.OfficeName,
                        FromWarehouseIdPath = UserState.OfficeIdPath,
                        ForcastDate = model.ForcastDate,
                        WalletNo = 0,
                        TransportMethodId = transportMethod.Id,
                        TransportMethodName = transportMethod.Name,
                        TransportPartnerId = transportParner.Id,
                        TransportPartnerName = transportParner.Name,
                        ContactPhone = model.ContactPhone,
                        ContactName = model.ContactName,
                        PriceType = model.PriceType,
                        Price = model.Price,
                        Value = model.Value
                    };

                    Office targetWarehouse = null;

                    // Thay đổi kho đích của bao hàng
                    if (model.ToWarehouseId.HasValue)
                    {
                        targetWarehouse = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(
                            x => (x.Id == model.ToWarehouseId.Value) && !x.IsDelete &&
                                (x.Status == (byte) OfficeStatus.Use));

                        if (targetWarehouse == null)
                            return JsonCamelCaseResult(
                                    new { Status = -2, Text = "Receiving warehouse does not exist or has been deleted" },
                                    JsonRequestBehavior.AllowGet);

                        dispatcher.ToWarehouseId = targetWarehouse.Id;
                        dispatcher.ToWarehouseName = targetWarehouse.Name;
                        dispatcher.ToWarehouseIdPath = targetWarehouse.IdPath;
                        dispatcher.ToWarehouseAddress = targetWarehouse.Address;
                    }

                    if (model.EntrepotId.HasValue)
                    {
                        var entrepot = await UnitOfWork.EntrepotRepo.SingleOrDefaultAsync(
                            x => (x.Id == model.EntrepotId.Value) && !x.IsDelete && x.Status == 1);

                        if (entrepot == null)
                            return JsonCamelCaseResult(
                                    new { Status = -2, Text = "Transshipment point does not exist or has been deleted" },
                                    JsonRequestBehavior.AllowGet);

                        dispatcher.EntrepotId = entrepot.Id;
                        dispatcher.EntrepotName = entrepot.Name;
                    }

                    UnitOfWork.DispatcherRepo.Add(dispatcher);

                    await UnitOfWork.DispatcherRepo.SaveAsync();

                    // Cập nhật lại Mã cho Orders và Total money
                    var dispatcherOfDay =
                        UnitOfWork.DispatcherRepo.Count(
                            x => (x.Created.Year == timeNow.Year) && (x.Created.Month == timeNow.Month) &&
                                 (x.Created.Day == timeNow.Day) && (x.Id <= dispatcher.Id));

                    dispatcher.Code = $"{dispatcherOfDay}{timeNow:ddMMyy}";
                    dispatcher.UnsignedText = MyCommon.Ucs2Convert(
                        $"{dispatcher.Code} {dispatcher.UserFullName} {dispatcher.UserName} {dispatcher.ToWarehouseName} {dispatcher.FromWarehouseName}");

                    await UnitOfWork.DispatcherRepo.SaveAsync();

                    var dispatcherDetails = new List<DispatcherDetail>();

                    // Thêm bao hàng
                    foreach (var package in model.Wallets)
                    {
                        // package
                        var w = await UnitOfWork.WalletRepo.SingleOrDefaultAsync(
                            x => (x.Id == package.WalletId) && !x.IsDelete);

                        if (w == null)
                        {
                            codes.Add($"P{package.WalletCode}");
                            continue;
                        }

                        // Có package does not exist
                        if (codes.Any())
                        {
                            continue;
                        }

                        var firstSend = false;
                        // Cập nhật lại trạng thái của bao
                        if (model.Status == (byte) DispatcherStatus.Approved)
                        {
                            w.Status = (byte) WalletStatus.Shipping;

                            
                            // Kho tạo phiếu điều vận lần đàu tiền -> Set Bao hàng đang được đối tác nhận
                            if (w.CurrentWarehouseId != null)
                            {
                                w.PartnerId = dispatcher.TransportPartnerId;
                                w.PartnerName = dispatcher.TransportPartnerName;
                                w.PartnerUpdate = timeNow;

                                firstSend = true;
                            }

                            w.CurrentWarehouseId = null;
                            w.CurrentWarehouseIdPath = null;
                            w.CurrentWarehouseAddress = null;
                            w.CurrentWarehouseName = null;

                            // Cập nhật lại kho đich mới cho bao hàng
                            if (targetWarehouse != null)
                            {
                                w.TargetWarehouseId = targetWarehouse.Id;
                                w.TargetWarehouseIdPath = targetWarehouse.IdPath;
                                w.TargetWarehouseName = targetWarehouse.Name;
                                w.TargetWarehouseAddress = targetWarehouse.Address;
                            }

                            // Cập nhật lại trạng thái của package
                            var packages = await UnitOfWork.OrderPackageRepo.OrderPackageByWalletId(w.Id);

                            foreach (var p in packages)
                            {
                                if (p.Status == (byte) OrderPackageStatus.ChinaExport)
                                {
                                    // Thêm lịch sử cho package
                                    var packageHistory = new PackageHistory()
                                    {
                                        PackageId = p.Id,
                                        PackageCode = p.Code,
                                        OrderId = p.OrderId,
                                        OrderCode = p.OrderCode,
                                        Type = p.OrderType,
                                        Status = (byte)OrderPackageStatus.PartnerDelivery,
                                        Content = $"{UserState.OfficeName } {EnumHelper.GetEnumDescription(OrderPackageStatus.PartnerDelivery)}",
                                        CustomerId = p.CustomerId,
                                        CustomerName = p.CustomerName,
                                        UserId = UserState.UserId,
                                        UserName = UserState.UserName,
                                        UserFullName = UserState.FullName,
                                        CreateDate = DateTime.Now,
                                    };

                                    UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                                    // Thêm note cho package và Orders
                                    await PackageNote(package.Note, p, dispatcher, PackageNoteMode.Dispatch);
                                }

                                p.Status = (byte) OrderPackageStatus.PartnerDelivery;
                                p.CurrentWarehouseId = null;
                                p.CurrentWarehouseIdPath = null;
                                p.CurrentWarehouseAddress = null;
                                p.CurrentWarehouseName = null;
                                p.ForcastDate = timeNow.AddDays(2);

                                // Cập nhật lại Layout
                                p.CurrentLayoutId = null;
                                p.CurrentLayoutIdPath = null;
                                p.CurrentLayoutName = null;

                                // Cập nhật lại kho đich mới package
                                //if (targetWarehouse != null)
                                //{
                                //    p.CustomerWarehouseId = targetWarehouse.Id;
                                //    p.CurrentWarehouseIdPath = targetWarehouse.IdPath;
                                //    p.CurrentWarehouseName = targetWarehouse.Name;
                                //    p.CurrentWarehouseAddress = targetWarehouse.Address;
                                //}

                                // Add Orders code vào danh sách để cập nhật lại thông tin Orders
                                if (!string.IsNullOrWhiteSpace(p.OrderCodes))
                                {
                                    codeOrder.AddRange(p.OrderCodes.Split(',').ToList().Select(x => x.Trim()));
                                }
                                else
                                {
                                    codeOrder.Add(p.OrderCode);
                                }
                            }
                        }

                        var dd = new DispatcherDetail
                        {
                            Created = timeNow,
                            Updated = timeNow,
                            IsDelete = false,
                            Note = package.Note,
                            Status = w.PartnerId == dispatcher.TransportPartnerId ? (byte) 1 : (byte) 0,
                            DispatcherCode = dispatcher.Code,
                            DispatcherId = dispatcher.Id,
                            TransportMethodId = dispatcher.TransportMethodId,
                            TransportMethodName = dispatcher.TransportMethodName,
                            EntrepotId = dispatcher.EntrepotId,
                            EntrepotName = dispatcher.EntrepotName,
                            Weight = w.Weight,
                            WeightActual = w.WeightActual,
                            WeightConverted = w.WeightConverted,
                            WalletId = w.Id,
                            WalletCode = w.Code,
                            Description = package.Description,
                            PackageNo = w.PackageNo,
                            Amount = w.TotalValue,
                            TransportPartnerId = dispatcher.TransportPartnerId,
                            TransportPartnerName = dispatcher.TransportPartnerName,
                            Volume = w.Volume,
                            Size = w.Size,
                        };

                        // Gửi cho nhà VC đầu tiên chốt luôn cân nặng
                        if (firstSend)
                        {
                            dd.Value = package.Value;
                        }

                        // Thêm kiện vào bao hàng
                        dispatcherDetails.Add(dd);
                    }

                    // Có kiên/bao does not exist
                    if (codes.Any())
                    {
                        transaction.Rollback();
                        return JsonCamelCaseResult(new
                            {
                                Status = -2,
                                Text = $"Packages : {string.Join(", ", codes)}  does not exist or has been deleted"
                            },
                            JsonRequestBehavior.AllowGet);
                    }

                    // Lấy tất cả các phiếu điều vận trước của bao hàng
                    var walletCodes = $";{string.Join(";", dispatcherDetails.Select(x => x.WalletCode).ToList())};";

                    var dispatcherDetailOlds = await UnitOfWork.DispatcherRepo.DispatcherDetailOld(DateTime.Now, walletCodes);

                    if (dispatcherDetailOlds.Any())
                    {
                        // Cập nhật lại thông tin cho phiếu vận chuyển trước
                        foreach (var ddOld in dispatcherDetailOlds)
                        {
                            ddOld.ToDispatcherId = dispatcher.Id;
                            ddOld.ToDispatcherCode = dispatcher.Code;
                            ddOld.ToEntrepotId = dispatcher.EntrepotId;
                            ddOld.ToEntrepotName = dispatcher.EntrepotName;
                            ddOld.ToTransportMethodId = dispatcher.TransportMethodId;
                            ddOld.ToTransportMethodName = dispatcher.TransportMethodName;
                            ddOld.ToTransportPartnerId = dispatcher.TransportPartnerId;
                            ddOld.ToTransportPartnerName = dispatcher.TransportPartnerName;
                            ddOld.ToTransportPartnerTime = DateTime.Now;
                        }

                        // Cập nhật thông tin cho phiếu vận chuyển hiện tại
                        foreach (var dd in dispatcherDetails)
                        {
                            var ddOld = dispatcherDetailOlds.SingleOrDefault(x => x.WalletId == dd.WalletId);

                            if (ddOld == null)
                                continue;

                            dd.FromTransportMethodId = ddOld.TransportMethodId;
                            dd.FromTransportMethodName = ddOld.TransportMethodName;
                            dd.FromTransportPartnerId = ddOld.TransportPartnerId;
                            dd.FromTransportPartnerName = ddOld.TransportPartnerName;
                            dd.FromDispatcherId = ddOld.DispatcherId;
                            dd.FromDispatcherCode = ddOld.DispatcherCode;
                            dd.FromEntrepotId = ddOld.EntrepotId;
                            dd.FromEntrepotName = ddOld.FromEntrepotName;
                        }
                    }
                    
                    UnitOfWork.DispatcherDetailRepo.AddRange(dispatcherDetails);

                    await UnitOfWork.DispatcherDetailRepo.SaveAsync();

                    dispatcher.WalletNo = await UnitOfWork.DispatcherDetailRepo.CountAsync(
                        x => !x.IsDelete && (x.DispatcherId == dispatcher.Id));

                    dispatcher.TotalWeight = await UnitOfWork.DispatcherDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.Weight != null) && (x.DispatcherId == dispatcher.Id))
                        .SumAsync(x => x.Weight);

                    dispatcher.TotalWeightActual = await UnitOfWork.DispatcherDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.WeightActual != null) && (x.DispatcherId == dispatcher.Id))
                        .SumAsync(x => x.WeightActual);

                    dispatcher.TotalWeightConverted = await UnitOfWork.DispatcherDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.WeightConverted != null) && (x.DispatcherId == dispatcher.Id))
                        .SumAsync(x => x.WeightConverted);

                    dispatcher.Amount = await UnitOfWork.DispatcherDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.Amount != null) && (x.DispatcherId == dispatcher.Id))
                        .SumAsync(x => x.Amount);

                    dispatcher.TotalVolume = await UnitOfWork.DispatcherDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.Volume != null) && (x.DispatcherId == dispatcher.Id))
                        .SumAsync(x => x.Volume);

                    dispatcher.TotalPackageNo = await UnitOfWork.DispatcherDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.DispatcherId == dispatcher.Id))
                        .SumAsync(x => x.PackageNo);

                    // Cập nhật lại trạng thái của các Orders liên quan
                    if (codeOrders.Any())
                    {
                        var strCodeOrder = $";{string.Join(";", codeOrder.Distinct().ToList())};";

                        var orders = await UnitOfWork.OrderRepo.FindAsync(
                                    x => strCodeOrder.Contains(";" + x.Code + ";") && !x.IsDelete);

                        // Cập nhật trạng thái Orders
                        foreach (var order in orders)
                        {
                            //check Orders ký gửi
                            if (order.Type == (byte) OrderType.Deposit)
                            {
                                if (order.Status < (byte)DepositStatus.Shipping)
                                {
                                    order.Status = (byte)DepositStatus.Shipping;
                                    order.LastUpdate = timeNow;

                                    // Thêm lịch sử cho Orders
                                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                    {
                                        CreateDate = timeNow,
                                        Content = $"Goods in transit",
                                        CustomerId = order.CustomerId ?? 0,
                                        CustomerName = order.CustomerName,
                                        OrderId = order.Id,
                                        Status = order.Status,
                                        UserId = UserState.UserId,
                                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                        Type = order.Type
                                    });
                                    UnitOfWork.OrderHistoryRepo.Save();
                                }
                            }
                            else
                            {
                                if (order.Status < (byte)OrderStatus.Shipping)
                                {
                                    order.Status = (byte)OrderStatus.Shipping;
                                    order.LastUpdate = timeNow;

                                    // Thêm lịch sử cho Orders
                                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                    {
                                        CreateDate = timeNow,
                                        Content = $"Goods in transit",
                                        CustomerId = order.CustomerId ?? 0,
                                        CustomerName = order.CustomerName,
                                        OrderId = order.Id,
                                        Status = order.Status,
                                        UserId = UserState.UserId,
                                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                        Type = order.Type
                                    });
                                    UnitOfWork.OrderHistoryRepo.Save();
                                }
                            }

                            await UnitOfWork.OrderRepo.SaveAsync();
                        }
                    }

                    await UnitOfWork.DispatcherRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new {Status = 1, Text = "Warehousing request created successfully"},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.Dispatcher, EnumPage.WalletTracker)]
        public async Task<ActionResult> UpdateValue(ValueUpdateMeta model)
        {
            var dd = await
                UnitOfWork.DispatcherDetailRepo.SingleOrDefaultAsync(
                    x => x.Id == model.DispatcherDetailId && x.IsDelete == false);

            if (dd == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Transfer note does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var d = await
                        UnitOfWork.DispatcherRepo.SingleOrDefaultAsync(
                            x => x.Id == dd.DispatcherId && x.IsDelete == false);

                    if (d == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Transfer note does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    var w = await UnitOfWork.WalletRepo.SingleOrDefaultAsync(x => x.Id == dd.WalletId);

                    if (w == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Package does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    // Lần đầu cập nhật Giá trị
                    if (dd.Value == null)
                    {
                        var ddOld =
                            await
                                UnitOfWork.DispatcherDetailRepo.Entities.OrderByDescending(x => x.Created)
                                    .FirstOrDefaultAsync(
                                        x =>
                                            x.WalletId == dd.WalletId && x.TransportPartnerId == w.PartnerId &&
                                            x.Created < dd.Created && x.Status == 1);

                        var timeNow = DateTime.Now;

                        // Cập nhật lại Trạng thái điều vận trước là hoàn thành
                        ddOld.Status = 2;
                        ddOld.Updated = timeNow;

                        // Cập nhật lại đối tác vận chuyển trong bao hàng
                        w.PartnerId = d.TransportPartnerId;
                        w.PartnerName = d.TransportPartnerName;
                        w.PartnerUpdate = timeNow;

                        // Cật nhật giá trị điều vận hiện tại
                        dd.Value = model.Value;
                        dd.Status = 1;
                        dd.Created = timeNow;

                        // Cập nhật lại trạng thái của các Orders liên quan
                        var orders = await UnitOfWork.OrderRepo.FindAsync(
                                        x => w.OrderCodesUnsigned.Contains(";" + x.Code + ";") && !x.IsDelete
                                             && x.Status == (byte)OrderStatus.InWarehouse);

                        // Cập nhật trạng thái Orders
                        foreach (var order in orders)
                        {
                            order.Status = (byte)OrderStatus.Shipping;
                            order.LastUpdate = timeNow;

                            // Thêm lịch sử thay đổi trạng thái
                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                            {
                                CreateDate = timeNow,
                                Content = $"Partner {d.TransportPartnerName} receive goods",
                                CustomerId = order.CustomerId ?? 0,
                                CustomerName = order.CustomerName,
                                OrderId = order.Id,
                                Status = order.Status,
                                UserId = UserState.UserId,
                                UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                Type = order.Type
                            });
                        }
                    }
                    else
                    {
                        dd.Value = model.Value;
                    }

                    await UnitOfWork.DispatcherDetailRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { dd.Status, Text = "Updated successfully" },
                    JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.Dispatcher)]
        public async Task<ActionResult> UpdateDescription(DescriptionUpdateMeta model)
        {
            var dd = await
                UnitOfWork.DispatcherDetailRepo.SingleOrDefaultAsync(
                    x => x.Id == model.DispatcherDetailId && x.IsDelete == false);

            if (dd == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Transfer note does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var d = await
                        UnitOfWork.DispatcherRepo.SingleOrDefaultAsync(
                            x => x.Id == dd.DispatcherId && x.IsDelete == false);

                    if (d == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Transfer note does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    var w = await UnitOfWork.WalletRepo.SingleOrDefaultAsync(x => x.Id == dd.WalletId);

                    if (w == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Package does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    // Lần đầu cập nhật Giá trị
                    dd.Description = model.Description;

                    await UnitOfWork.DispatcherDetailRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { dd.Status, Text = "Updated successfully" },
                    JsonRequestBehavior.AllowGet);
        }

        private async Task PackageNote(string strPackageNote, OrderPackage package, Dispatcher dispatcher, PackageNoteMode packageNoteMode)
        {
            //Thêm note cho các Orders trong phiếu nhập
            var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                                x =>
                                    x.PackageId == null && x.OrderId == package.OrderId && x.ObjectId == dispatcher.Id &&
                                    x.Mode == (byte)packageNoteMode);

            if (packageNote == null && !string.IsNullOrWhiteSpace(dispatcher.Note))
            {
                UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                {
                    OrderId = package.OrderId,
                    OrderCode = package.OrderCode,
                    PackageId = null,
                    PackageCode = null,
                    UserId = dispatcher.UserId,
                    UserFullName = dispatcher.UserFullName,
                    Time = DateTime.Now,
                    ObjectId = dispatcher.Id,
                    ObjectCode = dispatcher.Code,
                    Mode = (byte)packageNoteMode,
                    Content = dispatcher.Note
                });
            }
            else if (packageNote != null && !string.IsNullOrWhiteSpace(dispatcher.Note))
            {
                packageNote.Content = dispatcher.Note;
            }
            else if (packageNote != null && string.IsNullOrWhiteSpace(dispatcher.Note))
            {
                UnitOfWork.PackageNoteRepo.Remove(packageNote);
            }

            // Thêm note cho package trong phiếu nhập
            var packageNote2 = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                    x =>
                        x.PackageId == package.Id && x.ObjectId == dispatcher.Id &&
                        x.Mode == (byte)packageNoteMode);

            if (packageNote2 == null && !string.IsNullOrWhiteSpace(strPackageNote))
            {
                UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                {
                    OrderId = package.OrderId,
                    OrderCode = package.OrderCode,
                    PackageId = package.Id,
                    PackageCode = package.Code,
                    UserId = dispatcher.UserId,
                    UserFullName = dispatcher.UserFullName,
                    Time = DateTime.Now,
                    ObjectId = dispatcher.Id,
                    ObjectCode = dispatcher.Code,
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
    }
}