using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using Hangfire;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using Library.DbContext.Results;
using Library.Jobs;
using Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.ExceptionServices;
using Cms.Jobs;

namespace Cms.Controllers
{
    [Authorize]
    public class PutAwayController : BaseController
    {
        // GET: PutAway
        [LogTracker(EnumAction.View, EnumPage.PutAway)]
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
                        (x.Type == (byte)OfficeType.Warehouse) &&
                        !x.IsDelete && (x.Status == (byte)OfficeStatus.Use));

                ViewBag.Warehouses = JsonConvert.SerializeObject(warehouses.Select(Mapper.Map<OfficeDropdownResult>),
                    jsonSerializerSettings);
            }

            var allWarehouse = await UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));

            ViewBag.AllWarehouses =
                JsonConvert.SerializeObject(allWarehouse.Select(x => new { x.Id, x.Name, x.IdPath, x.Address }).ToList(),
                    jsonSerializerSettings);

            ViewBag.StatesSelectListItem = Enum.GetValues(typeof(PutAwayStatus))
                .Cast<PutAwayStatus>()
                .Select(
                    v =>
                        new SelectListItem
                        {
                            Value = ((byte)v).ToString(),
                            Text = EnumHelper.GetEnumDescription<PutAwayStatus>((int)v),
                            Selected = v == PutAwayStatus.Complete
                        })
                .ToList();

            ViewBag.PutAwayStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(PutAwayStatus))
                .Cast<PutAwayStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<PutAwayStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderPackageStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            // todo: comment where by status (await UnitOfWork.LayoutRepo.FindAsNoTrackingAsync)
            var layout = await UnitOfWork.LayoutRepo.FindAsNoTrackingAsync(
                x => x.WarehouseId == UserState.OfficeId.Value && !x.IsDelete);

            ViewBag.WalletStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            var layoutJstree = layout.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                childNo = o.ChildNo,
                namePath = o.NamePath,
                value = MyCommon.Ucs2Convert(o.Name),
                parent = o.ParentLayoutId?.ToString() ?? "#",
                state = new { opened = !o.ParentLayoutId.HasValue },
                idPath = o.IdPath,
                code = o.Code
            });

            ViewBag.Layouts = JsonConvert.SerializeObject(layoutJstree);

            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderType>((int)v)),
                jsonSerializerSettings);

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.PutAway)]
        public async Task<ActionResult> Search(string warehouseIdPath, int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, string keyword = "",
            int currentPage = 1, int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            long totalRecord;

            // Query bao hàng được tạo trong kho
            Expression<Func<PutAway, bool>> queryCreated =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && ((x.WarehouseIdPath == warehouseIdPath) ||
                                     x.WarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.WarehouseIdPath == warehouseIdPath))) &&
                     (((fromDate == null) && (toDate == null)) ||
                      ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                      ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                      ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)));

            //var wallets = await UnitOfWork.PutAwayRepo.FindAsNoTrackingAsync(out totalRecord, queryCreated,
            //        x => x.OrderByDescending(y => y.Id), currentPage, recordPerPage);


            Expression<Func<PutAway, bool>> queryI =
                x => ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && ((userId == null) || (x.UserId == userId.Value)) &&
                     ((isManager && ((x.WarehouseIdPath == warehouseIdPath) ||
                                     x.WarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.WarehouseIdPath == warehouseIdPath))) &&
                     (((fromDate == null) && (toDate == null)) ||
                      ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) && (x.Created <= toDate)) ||
                      ((fromDate == null) && toDate.HasValue && (x.Created <= toDate)) ||
                      ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate)));

            Expression <Func<PutAwayResult, bool>> queryR =
                x => x.UnsignedText.Contains(keyword) || x.PackageUnsignedText.Contains(keyword);

            var wallets =
                await UnitOfWork.PutAwayRepo.Search(queryI, queryR, currentPage, recordPerPage, out totalRecord);

            var states = UnitOfWork.PutAwayRepo.CountByStatus(queryCreated);

            return JsonCamelCaseResult(
                new
                {
                    items = wallets,
                    totalRecord,
                    states
                }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.PutAway)]
        public async Task<ActionResult> GetDetail(int id)
        {
            var data = await UnitOfWork.PutAwayRepo.SingleOrDefaultAsNoTrackingAsync(x => x.Id == id && !x.IsDelete);

            return JsonCamelCaseResult(data, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.PutAway)]
        public async Task<ActionResult> GetPackages(int id)
        {
            var putAway = await UnitOfWork.PutAwayRepo.SingleOrDefaultAsNoTrackingAsync(x => (x.Id == id) && !x.IsDelete);

            if (putAway == null)
                return JsonCamelCaseResult(new { Status = -1, Text = " PutAway Ticket does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            //if (UserState.OfficeId.HasValue && importWarehouse.WarehouseId != UserState.OfficeId.Value)
            //    return JsonCamelCaseResult(new { Status = -2, Text = "Bạn không phải là nhân viên kho này" }, JsonRequestBehavior.AllowGet);

            var layout = await UnitOfWork.LayoutRepo.FindAsNoTrackingAsync(
                x => x.WarehouseId == putAway.WarehouseId && !x.IsDelete);

            var layoutJstree = layout.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentLayoutId?.ToString() ?? "#",
                state = new { opened = !o.ParentLayoutId.HasValue },
                idPath = o.IdPath,
                code = o.Code
            });

            var items = await UnitOfWork.PutAwayDetailRepo.GetByPutawayId(id);

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


            return JsonCamelCaseResult(new { layoutJstree, items}, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.PutAway)]
        public async Task<ActionResult> Add(PutAwayMeta model)
        {
            if (UserState.OfficeType != 1)
                return JsonCamelCaseResult(
                        new { Status = -2, Text = "Only warehouse staff can perform this operation" },
                        JsonRequestBehavior.AllowGet);

            if (!ModelState.IsValid)
            {
                var errorMessage =
                    string.Join(", ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage));

                return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
                    JsonRequestBehavior.AllowGet);
            }

            var timeNow = DateTime.Now;
            var codes = new List<string>();
            var putAwayCodes = new List<string>();
            var codeOrder = new List<string>();
            List<Order> orders = null;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var putAway = new PutAway()
                    {
                        Created = timeNow,
                        Updated = timeNow,
                        Status = model.Status,
                        UserId = UserState.UserId,
                        UserName = UserState.UserName,
                        UserFullName = UserState.FullName,
                        WarehouseId = UserState.OfficeId ?? 0,
                        WarehouseName = UserState.OfficeName,
                        WarehouseIdPath = UserState.OfficeIdPath,
                        WarehouseAddress = UserState.OfficeAddress,
                        Note = model.Note,
                        Code = string.Empty,
                        UnsignedText = string.Empty,
                        PackageNo = 0,
                        IsDelete = false
                    };

                    UnitOfWork.PutAwayRepo.Add(putAway);

                    await UnitOfWork.PutAwayRepo.SaveAsync();

                    // Cập nhật lại Mã cho Order và Sum tiền
                    var putAwayOfDay = UnitOfWork.PutAwayRepo.Count(x =>
                        (x.Created.Year == timeNow.Year) && (x.Created.Month == timeNow.Month) &&
                        (x.Created.Day == timeNow.Day) && (x.Id <= putAway.Id));

                    putAway.Code = $"{putAwayOfDay}{timeNow:ddMMyy}";
                    putAway.UnsignedText = MyCommon.Ucs2Convert(
                        $"{putAway.Code} {putAway.UserFullName} {putAway.UserName} {putAway.WarehouseName}");

                    await UnitOfWork.PutAwayRepo.SaveAsync();

                    List<OrderPackage> listPackage = new List<OrderPackage>();

                    // Thêm package
                    foreach (var package in model.Packages)
                    {
                        #region Xử lý thông tin liên quan đến package
                        // package
                        var p = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(
                            x => x.Id == package.PackageId && !x.IsDelete &&
                                (x.Status == (byte) OrderPackageStatus.Received
                                 || x.Status == (byte) OrderPackageStatus.ChinaReceived || x.Status == (byte)OrderPackageStatus.Lost) && x.CurrentLayoutId == null);

                        if (p == null)
                        {
                            codes.Add($"P{package.PackageCode}");
                            continue;
                        }

                        if (p.CurrentLayoutId != null)
                        {
                            putAwayCodes.Add($"P{package.PackageCode}");
                            continue;
                        }

                        if (p.CustomerWarehouseId == UserState.OfficeId && package.IsLose == false)
                        {
                            p.Status = (byte)OrderPackageStatus.Lost;
                            p.LastUpdate = DateTime.Now;
                            continue;
                        }

                        Layout layout = null;
                        if (p.WarehouseId != putAway.WarehouseId)
                        {
                            // todo: comment where by status (await UnitOfWork.LayoutRepo.FindAsNoTrackingAsync)
                            layout = await UnitOfWork.LayoutRepo.SingleOrDefaultAsync(
                                        x => x.Id == package.LayoutId && !x.IsDelete /*&& x.Status == 1*/);

                            if (layout == null)
                            {
                                transaction.Rollback();
                                return JsonCamelCaseResult(new
                                {
                                    Status = -2,
                                    Text =
                                            $"Layout of package \"{package.PackageCode}\" does not exist or has been deleted"
                                },
                                        JsonRequestBehavior.AllowGet);
                            }

                            p.CurrentLayoutId = layout.Id;
                            p.CurrentLayoutName = layout.Name;
                            p.CurrentLayoutIdPath = layout.IdPath;
                        }

                        if (p.CurrentWarehouseId != UserState.OfficeId)
                        {
                            transaction.Rollback();
                            return JsonCamelCaseResult(new
                            {
                                Status = -2,
                                Text =
                                        $"You are not an staff in warehouse: \"{p.CurrentWarehouseName}\" Can not enter into warehouse!"
                            },
                                    JsonRequestBehavior.AllowGet);
                        }

                        if (p.CurrentLayoutId == null)
                        {
                            p.CurrentLayoutId = 0;
                            p.CurrentLayoutName = string.Empty;
                            p.CurrentLayoutIdPath = string.Empty;
                        }

                        // Kho đích của khách hàng
                        if (p.CustomerWarehouseId == UserState.OfficeId)
                        {
                            // Putaway kho VN
                            p.Status = (byte) OrderPackageStatus.InStock;

                            // Thêm lịch sử cho package
                            var packageHistory = new PackageHistory()
                            {
                                PackageId = p.Id,
                                PackageCode = p.Code,
                                OrderId = p.OrderId,
                                OrderCode = p.OrderCode,
                                Type = p.OrderType,
                                Status = (byte)OrderPackageStatus.InStock,
                                Content = $"[{UserState.OfficeName}] {EnumHelper.GetEnumDescription(OrderPackageStatus.InStock)}",
                                JsonData = JsonConvert.SerializeObject(new
                                {
                                    layoutName = layout.Name,
                                    layoutIdPath = layout.IdPath,
                                    layoutNamePath = layout.NamePath,
                                    warehouseId = UserState.OfficeId,
                                    warehouseName = UserState.OfficeName,
                                    warehouseIdPath = UserState.OfficeIdPath
                                }),
                                CustomerId = p.CustomerId,
                                CustomerName = p.CustomerName,
                                UserId = UserState.UserId,
                                UserName = UserState.UserName,
                                UserFullName = UserState.FullName,
                                CreateDate = DateTime.Now,
                            };

                            UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                            // Thêm ghi chú cho package và Order
                            await PackageNote(package.Note, p, putAway, PackageNoteMode.Putaway);
                        }
                        else
                        {
                            // Cập nhật lại trong lượng cho package
                            p.Weight = package.Weight;
                            p.WeightConverted = Math.Round(package.Length * package.Width * package.Height / 5000, 2);
                            p.WeightActual = p.Weight > p.WeightConverted ? p.Weight : p.WeightConverted;
                            p.Length = package.Length;
                            p.Width = package.Width;
                            p.Height = package.Height;
                            p.Size = $"{package.Length:N2}x{package.Width:N2}x{package.Height:N2}";
                            p.Volume = Math.Round(package.Length * package.Width * package.Height / 1000000, 4);
                            p.VolumeActual = p.Volume;

                            // Putaway kho TQ
                            p.Status = (byte) OrderPackageStatus.ChinaInStock;

                            // Thêm lịch sử cho package
                            var packageHistory = new PackageHistory()
                            {
                                PackageId = p.Id,
                                PackageCode = p.Code,
                                OrderId = p.OrderId,
                                OrderCode = p.OrderCode,
                                Type = p.OrderType,
                                Status = (byte)OrderPackageStatus.ChinaInStock,
                                Content = $"[{UserState.OfficeName}] {EnumHelper.GetEnumDescription(OrderPackageStatus.ChinaInStock)}",
                                CustomerId = p.CustomerId,
                                CustomerName = p.CustomerName,
                                UserId = UserState.UserId,
                                UserName = UserState.UserName,
                                UserFullName = UserState.FullName,
                                CreateDate = DateTime.Now,
                            };

                            UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                            // Thêm ghi chú cho package và Order
                            await PackageNote(package.Note, p, putAway, PackageNoteMode.ChinaPutaway);
                        }

                        var putAwayDetail = new PutAwayDetail()
                        {
                            Created = timeNow,
                            Updated = timeNow,
                            IsDelete = false,
                            Note = package.Note,
                            PackageId = p.Id,
                            OrderCode = p.OrderCode,
                            OrderId = p.OrderId,
                            OrderType = p.OrderType,
                            Status = 1,
                            TransportCode = p.TransportCode,
                            PutAwayId = putAway.Id,
                            PutAwayCode = putAway.Code,
                            Weight = p.Weight.Value,
                            ActualWeight = p.WeightActual.Value,
                            ConvertedWeight = p.WeightConverted.Value,
                            OrderPackageNo = p.PackageNo,
                            Length = package.Length,
                            Width = package.Width,
                            Height = package.Height,
                            Size = p.Size,

                            PackageCode = p.Code,
                            CustomerId = p.CustomerId,
                            CustomerName = p.CustomerName,
                            CustomerUserName = p.CustomerUserName,
                            OrderServices = p.OrderServices,
                        };

                        if (layout != null)
                        {
                            putAwayDetail.LayoutId = layout.Id;
                            putAwayDetail.LayoutIdPath = layout.IdPath;
                            putAwayDetail.LayoutName = layout.Name;
                            putAwayDetail.LayoutNamePath = layout.NamePath;
                        }
                        else
                        {
                            putAwayDetail.LayoutId = 0;
                            putAwayDetail.LayoutIdPath = string.Empty;
                            putAwayDetail.LayoutName = string.Empty;
                            putAwayDetail.LayoutNamePath = string.Empty;
                        }

                        // Thêm kiện vào bao hàng
                        UnitOfWork.PutAwayDetailRepo.Add(putAwayDetail);

                        // Add OrderCode to update cân nặng
                        codeOrder.Add(p.OrderCode);
                        #endregion
                    }

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

                    // Trùng mã vận đơn chưa được xử lý
                    var packageSameCodes = listPackage.Where(x => x.Mode != null && x.SameCodeStatus == 0).ToList();
                    if (packageSameCodes.Any())
                    {
                        transaction.Rollback();
                        return JsonCamelCaseResult(
                            new
                            {
                                Status = -2,
                                Text =
                                $"Transport code: {string.Join(", ", packageSameCodes.Select(x => x.TransportCode).Distinct().ToList())} overlap untreated"
                            },
                            JsonRequestBehavior.AllowGet);
                    }

                    // Các kiện đã được putAway
                    if (putAwayCodes.Any())
                    {
                        transaction.Rollback();
                        return
                            JsonCamelCaseResult(
                                new
                                {
                                    Status = -2,
                                    Text = $"Package: {string.Join(", ", putAwayCodes)} get putAway"
                                },
                                JsonRequestBehavior.AllowGet);
                    }

                    //packages.Select(x=> x.Cu)

                    await UnitOfWork.PutAwayDetailRepo.SaveAsync();

                    putAway.PackageNo = await UnitOfWork.PutAwayDetailRepo.CountAsync(
                        x => !x.IsDelete && (x.PutAwayId == putAway.Id));

                    putAway.TotalWeight = UnitOfWork.PutAwayDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.PutAwayId == putAway.Id))
                        .Select(x=> x.Weight)
                        .ToList()
                        .Sum(x => x);

                    putAway.TotalActualWeight = UnitOfWork.PutAwayDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.PutAwayId == putAway.Id))
                        .Select(x=> x.ActualWeight)
                        .ToList()
                        .Sum(x => x);

                    putAway.TotalConversionWeight = UnitOfWork.PutAwayDetailRepo.Entities.Where(
                            x => !x.IsDelete && (x.PutAwayId == putAway.Id))
                        .Select(x=> x.ConvertedWeight)
                        .ToList()
                        .Sum(x => x);

                    #region Xử lý các Order liên quan cần cập nhật

                    if (codeOrder.Any())
                    {
                        var strCodeOrder = $";{string.Join(";", codeOrder.Distinct())};";

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
                                    var packages = UnitOfWork.OrderPackageRepo.GetByOrderIdAndImportWarehouseId(order.Id,
                                            serviceOther.ObjectId);

                                    serviceOther.TotalWeightActual = packages.Sum(x => x.WeightActual);

                                    var totalPercent = 0M;
                                    var index = 1;
                                    // Tính lại cân nặng trong kiên hàng
                                    foreach (var p in packages)
                                    {
                                        decimal percent;
                                        // Không phải kiện cuối cùng
                                        if (index != packages.Count)
                                        {
                                            percent = Math.Round((p.WeightActual * 100 / serviceOther.TotalWeightActual) ?? 0, 4);
                                            totalPercent += percent;
                                        }
                                        else
                                        {
                                            percent = 100 - totalPercent;
                                        }

                                        if (packageFirst.Any(x => x == p.Id))
                                        {
                                            p.OtherService += percent * serviceOther.TotalPrice / 100;
                                        }
                                        else
                                        {
                                            p.OtherService = percent * serviceOther.TotalPrice / 100;
                                            packageFirst.Add(p.Id);
                                        }
                                        index++;
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
                                            x.ServiceId == (byte)OrderServices.OutSideShipping && x.Checked);

                            //var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == order.CustomerId.Value);

                            // Cập nhật số lượng Sum
                            //var hasOrderService =
                            //    await UnitOfWork.OrderServiceRepo.AnyAsync(
                            //            x => x.OrderId == order.Id && x.IsDelete == false && x.Checked);

                            //var totalService = hasOrderService
                            //    ? await UnitOfWork.OrderServiceRepo.Entities.Where(
                            //            x => x.OrderId == order.Id && x.IsDelete == false && x.Checked)
                            //        .SumAsync(x => x.TotalPrice)
                            //    : 0.0M;

                            decimal serviceValue;

                            var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId);

                            decimal outSideShipping;

                            // Cân nặng các package được xuất giao tại TQ
                            var orderWeightIgnore = UnitOfWork.OrderPackageRepo.GetTotalActualWeight(order.Id);

                            // Sum cân nặng tính tiền vc của Order
                            var orderWeight = order.TotalWeight - orderWeightIgnore;

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
                            else
                            { // Order Order
                                // VC tiết kiệm
                                //if (optimalService != null)
                                //{
                                //    serviceValue = OrderRepository.OptimalDelivery(orderWeight,
                                //        order.WarehouseDeliveryId ?? 0);
                                //}else if (fastDeliveryService != null) // VC nhanh
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
                                if (order.Type == (byte) OrderType.Order)
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
                                if (order.Type == (byte) OrderType.Order)
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

                            //Check Order ký gửi
                            if (order.Type == (byte) OrderType.Deposit)
                            {
                                // PutAway tại kho VN
                                if (order.WarehouseDeliveryId != UserState.OfficeId && 
                                    order.Status < (byte)DepositStatus.InWarehouse)
                                {
                                    order.Status = (byte)DepositStatus.InWarehouse;

                                    // Thêm lịch sử cho Order
                                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                    {
                                        CreateDate = timeNow,
                                        Content = $"Kho {UserState.OfficeName} Receive goods",
                                        CustomerId = order.CustomerId ?? 0,
                                        CustomerName = order.CustomerName,
                                        OrderId = order.Id,
                                        Status = order.Status,
                                        UserId = UserState.UserId,
                                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                        Type = order.Type
                                    });
                                } // PutAway tại kho TQ
                                else if (order.WarehouseDeliveryId == UserState.OfficeId &&
                                         order.Status < (byte)DepositStatus.Pending)
                                {
                                    order.Status = (byte)DepositStatus.Pending;

                                    // Thêm lịch sử cho Order
                                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                    {
                                        CreateDate = timeNow,
                                        Content = $"Wait for delivery",
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
                                // PutAway tại kho VN
                                if (order.WarehouseDeliveryId != UserState.OfficeId &&
                                    order.Status < (byte)OrderStatus.InWarehouse)
                                {
                                    order.Status = (byte)OrderStatus.InWarehouse;

                                    // Thêm lịch sử cho Order
                                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                    {
                                        CreateDate = timeNow,
                                        Content = $"Warehouse {UserState.OfficeName} Receive goods",
                                        CustomerId = order.CustomerId ?? 0,
                                        CustomerName = order.CustomerName,
                                        OrderId = order.Id,
                                        Status = order.Status,
                                        UserId = UserState.UserId,
                                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                        Type = order.Type
                                    });
                                } // PutAway tại kho TQ
                                else if (order.WarehouseDeliveryId == UserState.OfficeId &&
                                         order.Status < (byte)OrderStatus.Pending)
                                {
                                    order.Status = (byte)OrderStatus.Pending;

                                    // Thêm lịch sử cho Order
                                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                    {
                                        CreateDate = timeNow,
                                        Content = $"Wait for delivery",
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

                            await UnitOfWork.OrderRepo.SaveAsync();
                        }

                        
                    }
                    #endregion

                    await UnitOfWork.PutAwayRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            // Job cập nhật thông tin package
            if (orders != null)
            {
                var jobId = BackgroundJob.Enqueue(() => PackageJob.UpdateWeightActualPercent(orders.Select(x => x.Id).ToList()));

                var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

                BackgroundJob.ContinueWith(jobId, () => OrderJob.ProcessDebitReport(orderIds));
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "Create a successful store requisition" },
                JsonRequestBehavior.AllowGet);
        }

        private async Task PackageNote(string strPackageNote, OrderPackage package, PutAway putaway, PackageNoteMode packageNoteMode)
        {
            //Thêm ghi chú cho các Order trong phiếu nhập
            var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                                x =>
                                    x.PackageId == null && x.OrderId == package.OrderId && x.ObjectId == putaway.Id &&
                                    x.Mode == (byte)packageNoteMode);

            if (packageNote == null && !string.IsNullOrWhiteSpace(putaway.Note))
            {
                UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                {
                    OrderId = package.OrderId,
                    OrderCode = package.OrderCode,
                    PackageId = null,
                    PackageCode = null,
                    UserId = putaway.UserId,
                    UserFullName = putaway.UserFullName,
                    Time = DateTime.Now,
                    ObjectId = putaway.Id,
                    ObjectCode = putaway.Code,
                    Mode = (byte)packageNoteMode,
                    Content = putaway.Note
                });
            }
            else if (packageNote != null && !string.IsNullOrWhiteSpace(putaway.Note))
            {
                packageNote.Content = putaway.Note;
            }
            else if (packageNote != null && string.IsNullOrWhiteSpace(putaway.Note))
            {
                UnitOfWork.PackageNoteRepo.Remove(packageNote);
            }

            // Thêm note cho package trong phiếu nhập
            var packageNote2 = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                    x =>
                        x.PackageId == package.Id && x.ObjectId == putaway.Id &&
                        x.Mode == (byte)packageNoteMode);

            if (packageNote2 == null && !string.IsNullOrWhiteSpace(strPackageNote))
            {
                UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                {
                    OrderId = package.OrderId,
                    OrderCode = package.OrderCode,
                    PackageId = package.Id,
                    PackageCode = package.Code,
                    UserId = putaway.UserId,
                    UserFullName = putaway.UserFullName,
                    Time = DateTime.Now,
                    ObjectId = putaway.Id,
                    ObjectCode = putaway.Code,
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CheckPermission(EnumAction.Update, EnumPage.PutAway)]
        //public async Task<ActionResult> Update(PutAwayMeta model)
        //{
        //    ModelState.Remove("Packages");

        //    if (UserState.OfficeType != 1)
        //        return JsonCamelCaseResult(
        //                new { Status = -2, Text = "Chỉ có nhân viên kho mới được thực hiện thao tác này" },
        //                JsonRequestBehavior.AllowGet);

        //    if (!ModelState.IsValid)
        //    {
        //        var errorMessage =
        //            string.Join(", ", ModelState.Values
        //                .SelectMany(x => x.Errors)
        //                .Select(x => x.ErrorMessage));

        //        return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
        //            JsonRequestBehavior.AllowGet);
        //    }

        //    var putAway = await UnitOfWork.PutAwayRepo.SingleOrDefaultAsync(x => (x.Id == model.Id) && !x.IsDelete);

        //    if (putAway == null)
        //        return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu nhập kho không tồn tại hoặc đã bị xóa" },
        //            JsonRequestBehavior.AllowGet);

        //    // Không thể Edit phiếu PutAway quá 3 ngày
        //    if (putAway.Created > DateTime.Now.AddDays(-2))
        //    {
        //        return JsonCamelCaseResult(new { Status = -2, Text = "Không thể cập nhật phiếu PutAway đã quá 2 ngày" },
        //            JsonRequestBehavior.AllowGet);
        //    }

        //    putAway.Note = model.Note;
        //    putAway.Updated = DateTime.Now;
        //    putAway.Status = model.Status;

        //    await UnitOfWork.PutAwayRepo.SaveAsync();

        //    return JsonCamelCaseResult(new { Status = 1, Text = "Goods received note updated successfully" },
        //        JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CheckPermission(EnumAction.Update, EnumPage.PutAway)]
        //public async Task<ActionResult> AddPackage(PutAwayMeta1 model)
        //{
        //    if (UserState.OfficeType != 1)
        //        return JsonCamelCaseResult(
        //                new { Status = -2, Text = "Chỉ có nhân viên kho mới được thực hiện thao tác này" },
        //                JsonRequestBehavior.AllowGet);

        //    if (!ModelState.IsValid)
        //    {
        //        var errorMessage =
        //            string.Join(", ", ModelState.Values
        //                .SelectMany(x => x.Errors)
        //                .Select(x => x.ErrorMessage));

        //        return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
        //            JsonRequestBehavior.AllowGet);
        //    }

        //    using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var putAway = await UnitOfWork.PutAwayRepo.SingleOrDefaultAsync(
        //                    x => (x.Id == model.PutAwayId) && !x.IsDelete);

        //            if (putAway == null)
        //                return JsonCamelCaseResult(
        //                    new { Status = -1, Text = "Yêu cầu nhập kho này không tồn tại hoặc đã bị xóa" },
        //                    JsonRequestBehavior.AllowGet);

        //            // Không thể Edit phiếu PutAway quá 3 ngày
        //            if (putAway.Created > DateTime.Now.AddDays(-2))
        //            {
        //                return JsonCamelCaseResult(new { Status = -2, Text = "Không thể cập nhật phiếu PutAway đã quá 2 ngày" },
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            if (!model.Packages.Any())
        //            {
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = "No package để thêm" },
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            var packageIds = $";{string.Join(";", model.Packages.Select(x => x.PackageId))};";

        //            var timeNow = DateTime.Now;

        //            var packages =
        //                await
        //                    UnitOfWork.OrderPackageRepo.FindAsync(
        //                        x => x.IsDelete == false && packageIds.Contains(";" + x.Id + ";"));

        //            if (!packages.Any())
        //            {
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = "No package để thêm" },
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            var codesNotExist = new List<string>();
        //            var codesPutawayed = new List<string>();

        //            foreach (var package in model.Packages)
        //            {
        //                var p = packages.SingleOrDefault(x => (x.Id == package.PackageId) && !x.IsDelete);

        //                if (p == null)
        //                {
        //                    codesNotExist.Add("P" + package.PackageCode);
        //                    continue;
        //                }

        //                if (p.CurrentLayoutId != null)
        //                {
        //                    codesPutawayed.Add("P" + p.Code);
        //                    continue;
        //                }

        //                UnitOfWork.PutAwayDetailRepo.Add(new PutAwayDetail()
        //                {
        //                    Created = timeNow,
        //                    Updated = timeNow,
        //                    IsDelete = false,
        //                    Note = string.Empty,
        //                    PackageId = p.Id,
        //                    OrderCode = p.OrderCode,
        //                    OrderId = p.OrderId,
        //                    OrderType = p.OrderType,
        //                    Status = 1,
        //                    TransportCode = p.TransportCode,
        //                    PutAwayCode = putAway.Code,
        //                    PutAwayId = putAway.Id,
        //                    Length = p.Length ?? 0,
        //                    Width = p.Width ?? 0,
        //                    Height = p.Height ?? 0,
        //                    Weight = p.Weight ?? 0,
        //                    ConvertedWeight = p.WeightConverted ?? 0,
        //                    ActualWeight = p.WeightActual ?? 0,
        //                    OrderPackageNo = p.PackageNo,
        //                    PackageCode = p.Code,
        //                    LayoutId = p.CurrentLayoutId,
        //                    LayoutIdPath = p.CurrentLayoutIdPath,
        //                    LayoutName = p.CurrentLayoutName,
        //                    LayoutNamePath = p.CurrentLayoutIdPath,
        //                    Size = p.Size,
        //                    OrderServices = p.OrderServices
        //                });
        //            }

        //            if (codesNotExist.Any())
        //            {
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = $"Các kiện: {string.Join(", ", codesNotExist)} không tồn tại hoặc đã bị xóa" },
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            if (codesPutawayed.Any())
        //            {
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = $"Các kiện: {string.Join(", ", codesPutawayed)} Đã được PutAway" },
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            // Thêm kiện || Bao vào yêu cầu nhập kho
        //            await UnitOfWork.PutAwayDetailRepo.SaveAsync();

        //            // Cập nhật lại số lượng kiên, bao trong yêu cầu nhập kho
        //            putAway.PackageNo = await UnitOfWork.PutAwayDetailRepo.CountAsync(
        //                x => !x.IsDelete && (x.PutAwayId == putAway.Id));

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
        //        new { Status = 1, Text = $"Thêm package thành công" },
        //        JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CheckPermission(EnumAction.Update, EnumPage.PutAway)]
        //public async Task<ActionResult> UpdatePackage(PutAwayDetailMeta model)
        //{
        //    if (UserState.OfficeType != 1)
        //        return JsonCamelCaseResult(
        //                new { Status = -2, Text = "Chỉ có nhân viên kho mới được thực hiện thao tác này" },
        //                JsonRequestBehavior.AllowGet);

        //    if (!ModelState.IsValid)
        //    {
        //        var errorMessage =
        //            string.Join(", ", ModelState.Values
        //                .SelectMany(x => x.Errors)
        //                .Select(x => x.ErrorMessage));

        //        return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
        //            JsonRequestBehavior.AllowGet);
        //    }

        //    if (!model.PutAwayId.HasValue)
        //        return JsonCamelCaseResult(
        //            new { Status = -1, Text = "Phiếu PutAway này không tồn tại hoặc đã bị xóa" },
        //            JsonRequestBehavior.AllowGet);

        //    using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var putAway = await UnitOfWork.PutAwayRepo.SingleOrDefaultAsync(
        //                x => (x.Id == model.PutAwayId.Value) && !x.IsDelete);

        //            if (putAway == null)
        //                return JsonCamelCaseResult(
        //                    new { Status = -1, Text = "Phiếu PutAway này không tồn tại hoặc đã bị xóa" },
        //                    JsonRequestBehavior.AllowGet);

        //            var putAwayDetail = await UnitOfWork.PutAwayDetailRepo.SingleOrDefaultAsync(x => (x.Id == model.Id) && !x.IsDelete);

        //            if (putAwayDetail == null)
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = "Kiện hàng không tồn tại hoặc đã bị xóa" },
        //                    JsonRequestBehavior.AllowGet);

        //            var package = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(
        //                        x => (x.Id == model.PackageId) && !x.IsDelete);

        //            if (package == null)
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = "Kiện hàng không tồn tại hoặc đã bị xóa" },
        //                    JsonRequestBehavior.AllowGet);

        //            // Packge of order
        //            package.Length = model.Length;
        //            package.Width = model.Width;
        //            package.Height = model.Height;
        //            package.Size = $"{package.Length:N2}x{package.Width:N2}x{package.Height:N2}";

        //            package.Weight = model.Weight;
        //            package.WeightConverted = package.Length * package.Width * package.Height * 5000;
        //            package.WeightActual = package.Weight > package.WeightConverted ? package.Weight : package.WeightConverted;
        //            package.Volume = package.Length * package.Width * package.Height / 1000000;


        //            // Put Away Detail
        //            putAwayDetail.Note = model.Note;
        //            putAwayDetail.Length = package.Length ?? 0;
        //            putAwayDetail.Width = package.Width ?? 0;
        //            putAwayDetail.Height = package.Height ?? 0;
        //            putAwayDetail.Weight = package.Weight ?? 0;
        //            putAwayDetail.ConvertedWeight = package.WeightConverted ?? 0;
        //            putAwayDetail.ActualWeight = package.WeightActual ?? 0;

        //            await UnitOfWork.PutAwayDetailRepo.SaveAsync();

        //            putAway.PackageNo = await UnitOfWork.PutAwayDetailRepo.CountAsync(
        //                 x => !x.IsDelete && (x.PutAwayId == putAway.Id));

        //            putAway.TotalWeight = UnitOfWork.PutAwayDetailRepo.Entities.Where(
        //                    x => !x.IsDelete && (x.PutAwayId == putAway.Id))
        //                .Sum(x => x.Weight);

        //            putAway.TotalActualWeight = UnitOfWork.PutAwayDetailRepo.Entities.Where(
        //                    x => !x.IsDelete && (x.PutAwayId == putAway.Id))
        //                .Sum(x => x.ActualWeight);

        //            putAway.TotalConversionWeight = UnitOfWork.PutAwayDetailRepo.Entities.Where(
        //                    x => !x.IsDelete && (x.PutAwayId == putAway.Id))
        //                .Sum(x => x.ConvertedWeight);

        //            await UnitOfWork.PutAwayRepo.SaveAsync();

        //            transaction.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }

        //    return JsonCamelCaseResult(
        //        new { Status = 1, Text = "Updated successfully" },
        //        JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[CheckPermission(EnumAction.Update, EnumPage.PutAway)]
        //public async Task<ActionResult> DeletePackage(PutAwayDetailMeta model)
        //{
        //    if (UserState.OfficeType != 1)
        //        return JsonCamelCaseResult(
        //                new { Status = -2, Text = "Chỉ có nhân viên kho mới được thực hiện thao tác này" },
        //                JsonRequestBehavior.AllowGet);

        //    if (!ModelState.IsValid)
        //    {
        //        var errorMessage =
        //            string.Join(", ", ModelState.Values
        //                .SelectMany(x => x.Errors)
        //                .Select(x => x.ErrorMessage));

        //        return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
        //            JsonRequestBehavior.AllowGet);
        //    }

        //    if (!model.PutAwayId.HasValue)
        //        return JsonCamelCaseResult(
        //            new { Status = -1, Text = "Bao hàng này không tồn tại hoặc đã bị xóa" },
        //            JsonRequestBehavior.AllowGet);

        //    using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var putAwayDetail =
        //                await UnitOfWork.PutAwayDetailRepo.SingleOrDefaultAsync(x => (x.Id == model.Id) && !x.IsDelete);

        //            if (putAwayDetail == null)
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = "Kiện hàng không tồn tại hoặc đã bị xóa" },
        //                    JsonRequestBehavior.AllowGet);

        //            var putAway =
        //                await UnitOfWork.WalletRepo.SingleOrDefaultAsync(
        //                    x => (x.Id == putAwayDetail.PutAwayId) && !x.IsDelete);

        //            if (putAway == null)
        //                return JsonCamelCaseResult(
        //                    new { Status = -1, Text = "Phiếu PutAway này không tồn tại hoặc đã bị xóa" },
        //                    JsonRequestBehavior.AllowGet);

        //            // Không thể Edit phiếu PutAway quá 3 ngày
        //            if (putAway.Created > DateTime.Now.AddDays(-2))
        //            {
        //                return JsonCamelCaseResult(new { Status = -2, Text = "Không thể cập nhật phiếu PutAway đã quá 2 ngày" },
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            var p = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(x => x.Id == model.PackageId && !x.IsDelete);

        //            // Kiện không tồn tại hoặc đã bị xóa
        //            if (p == null)
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = "Kiện hàng không tồn tại hoặc đã bị xóa" },
        //                    JsonRequestBehavior.AllowGet);

        //            // Kiện không còn trong kho
        //            if (p.CurrentWarehouseId != UserState.OfficeId)
        //            {
        //                return JsonCamelCaseResult(
        //                    new { Status = -2, Text = "Không thể xóa kiện đã không còn trong kho" },
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            p.CurrentLayoutId = null;
        //            p.CurrentLayoutIdPath = null;
        //            p.CurrentLayoutName = null;
        //            p.LastUpdate = DateTime.Now;

        //            // Cập nhật thông tin kho hiện tại cho package
        //            putAwayDetail.IsDelete = true;
        //            putAwayDetail.Updated = DateTime.Now;

        //            await UnitOfWork.PutAwayDetailRepo.SaveAsync();

        //            // Cập nhật lại thông tin cho phiếu PutAway
        //            putAway.PackageNo = await UnitOfWork.PutAwayDetailRepo.CountAsync(
        //                 x => !x.IsDelete && (x.PutAwayId == putAway.Id));

        //            putAway.TotalWeight = UnitOfWork.PutAwayDetailRepo.Entities.Where(
        //                    x => !x.IsDelete && (x.PutAwayId == putAway.Id))
        //                .Sum(x => x.Weight);

        //            putAway.TotalWeightActual = UnitOfWork.PutAwayDetailRepo.Entities.Where(
        //                    x => !x.IsDelete && (x.PutAwayId == putAway.Id))
        //                .Sum(x => x.ActualWeight);

        //            putAway.TotalWeightConverted = UnitOfWork.PutAwayDetailRepo.Entities.Where(
        //                    x => !x.IsDelete && (x.PutAwayId == putAway.Id))
        //                .Sum(x => x.ConvertedWeight);

        //            await UnitOfWork.PutAwayRepo.SaveAsync();

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
    }
}