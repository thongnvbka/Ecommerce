using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Emums;
using Cms.Attributes;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    public class TransferController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.Transfer)]
        public async Task<ActionResult> Index()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var allWarehouse = await UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));

            ViewBag.AllWarehouses =
                JsonConvert.SerializeObject(allWarehouse.Select(x => new { x.Id, x.Name, x.IdPath, x.Address }).ToList(),
                    jsonSerializerSettings);

            return View();
        }

         [CheckPermission(EnumAction.View, EnumPage.Transfer)]
        public async Task<ActionResult> Search(string warehouseIdPath, int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, string keyword = "", int currentPage = 1, int recordPerPage = 20, byte mode = 0)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            long totalRecord = 0;

            // Query kho tạo phiếu
            Expression<Func<Transfer, bool>> queryCreated =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && ((userId == null) || (x.FromUserId == userId.Value)) &&
                     ((isManager && ((x.FromWarehouseIdPath == warehouseIdPath) ||
                                     x.FromWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.FromWarehouseIdPath == warehouseIdPath))) &&
                     (((fromDate == null) && (toDate == null)) ||
                      ((fromDate != null) && (toDate != null) && (x.FromTime>= fromDate) && (x.FromTime <= toDate)) ||
                      ((fromDate == null) && toDate.HasValue && (x.FromTime <= toDate)) ||
                      ((toDate == null) && fromDate.HasValue && (x.FromTime >= fromDate)));

            // Query kho chờ nhập kiện
            Expression<Func<Transfer, bool>> queryInStock =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && ((userId == null) || (x.FromUserId == userId.Value)) &&
                     ((isManager && ((x.ToWarehouseIdPath == warehouseIdPath) || x.ToWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.ToWarehouseIdPath == warehouseIdPath))) &&
                     (((fromDate == null) && (toDate == null)) ||
                      ((fromDate != null) && (toDate != null) && (x.FromTime >= fromDate) && (x.FromTime <= toDate)) ||
                      ((fromDate == null) && toDate.HasValue && (x.FromTime <= toDate)) ||
                      ((toDate == null) && fromDate.HasValue && (x.FromTime >= fromDate)));

            // Query tất cả
            Expression<Func<Transfer, bool>> queryAll =
                x => x.UnsignedText.Contains(keyword) && ((status == null) || (x.Status == status.Value)) &&
                     !x.IsDelete && ((userId == null) || (x.FromUserId == userId.Value)) &&
                     ((isManager && ((x.FromWarehouseIdPath == warehouseIdPath)
                                     || x.FromWarehouseIdPath.StartsWith(warehouseIdPath + ".")))
                      || (!isManager && (x.FromWarehouseIdPath == warehouseIdPath)) ||
                      (isManager && ((x.ToWarehouseIdPath == warehouseIdPath)
                                     || x.ToWarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                      (!isManager && (x.ToWarehouseIdPath == warehouseIdPath))) &&
                     (((fromDate == null) && (toDate == null)) ||
                      ((fromDate != null) && (toDate != null) && (x.FromTime >= fromDate) && (x.FromTime <= toDate)) ||
                      ((fromDate == null) && toDate.HasValue && (x.FromTime <= toDate)) ||
                      ((toDate == null) && fromDate.HasValue && (x.FromTime >= fromDate)));

            List<Transfer> wallets = new List<Transfer>();

            // Tất cả
            if (mode == 0)
                wallets = await UnitOfWork.TransferRepo.FindAsNoTrackingAsync(out totalRecord, queryAll,
                    x => x.OrderByDescending(y => y.Id), currentPage, recordPerPage);

            // Kho tạo phiếu
            if (mode == 1)
                wallets = await UnitOfWork.TransferRepo.FindAsNoTrackingAsync(out totalRecord, queryCreated,
                    x => x.OrderByDescending(y => y.Id), currentPage, recordPerPage);

            // Chơ nhập
            if (mode == 2)
                wallets = await UnitOfWork.TransferRepo.FindAsNoTrackingAsync(out totalRecord, queryInStock,
                    x => x.OrderByDescending(y => y.Id), currentPage, recordPerPage);

            // Count group
            var createdNo = await UnitOfWork.TransferRepo.LongCountAsync(queryCreated);
            var inStockNo = await UnitOfWork.TransferRepo.LongCountAsync(queryInStock);
            //var waitImportNo = await UnitOfWork.TransferRepo.LongCountAsync(queryWaitImport);
            var allNo = await UnitOfWork.TransferRepo.LongCountAsync(queryAll);

            return JsonCamelCaseResult(new
            {
                items = wallets,
                totalRecord,
                mode =
                new
                {
                    createdNo,
                    inStockNo,
                    // waitImportNo,
                    allNo
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.Add, EnumPage.Transfer)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Add(TransferMeta model)
        {
            if (UserState.OfficeType != 1)
                return JsonCamelCaseResult(
                        new { Status = -2, Text = "Only warehouse staff can perform this action" },
                        JsonRequestBehavior.AllowGet);

            if (!ModelState.IsValid)
                return JsonCamelCaseResult(new { Status = -1, Text = "Data format is incorrect" },
                    JsonRequestBehavior.AllowGet);

            if (model.ToWarehouseId == UserState.OfficeId)
                return JsonCamelCaseResult(new { Status = -1, Text = "The destination repository is not included with the current repository" },
                        JsonRequestBehavior.AllowGet);

            var toWarehouse =
                await UnitOfWork.OfficeRepo.SingleOrDefaultAsNoTrackingAsync(x => x.IsDelete == false && x.Id == model.ToWarehouseId);

            if(toWarehouse == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Destination warehouse does not exist or has been deleted" },
                        JsonRequestBehavior.AllowGet);

            if(model.TransferDetails == null || !model.TransferDetails.Any())
                return JsonCamelCaseResult(new { Status = -1, Text = "Ticket required  must be packagee" },
                        JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var packageCodes = $";{string.Join(";", model.TransferDetails.Select(x => x.PackageCode).ToList())};";

                    var packages = await UnitOfWork.OrderPackageRepo.FindAsync(
                        x => packageCodes.Contains(";" + x.Code + ";") && x.IsDelete == false &&
                            x.CurrentWarehouseId == UserState.OfficeId);

                    if (packages.Count != model.TransferDetails.Count)
                    {
                        var packageCodesError = model.TransferDetails.Where(x => packages.All(p => p.Code != x.PackageCode)).Select(x=> x.PackageCode).ToList();

                        return JsonCamelCaseResult(
                            new
                            {
                                Status = -1,
                                Text =
                                $"Các package: {string.Join(", ", packageCodesError)}  does not exist or has been deleted"
                            }, JsonRequestBehavior.AllowGet);
                    }

                    if (model.PriceShip != null && model.PriceShip.Value < 0)
                        return JsonCamelCaseResult(new {Status = -1, Text = "Destination warehouse does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    var wallets = await UnitOfWork.WalletRepo.GetWalletByPackageCodes(packageCodes);

                    var timeNow = DateTime.Now;

                    var transfer = new Transfer()
                    {
                        FromTime = timeNow,
                        FromUserId = UserState.UserId,
                        FromUserFullName = UserState.FullName,
                        FromUserTitleId = UserState.TitleId ?? 0,
                        FromUserTitleName = UserState.TitleName,
                        FromUserUserName = UserState.UserName,
                        FromWarehouseId = UserState.OfficeId ?? 0,
                        FromWarehouseIdPath = UserState.OfficeIdPath,
                        FromWarehouseName = UserState.OfficeName,
                        IsDelete = false,
                        Code = string.Empty,
                        Note = model.Note,
                        PriceShip = model.PriceShip,
                        UnsignedText = string.Empty,
                        ToWarehouseId = toWarehouse.Id,
                        ToWarehouseIdPath = toWarehouse.IdPath,
                        ToWarehouseName = toWarehouse.Name,
                        TotalWeight = packages.Sum(x=> x.Weight ?? 0),
                        TotalWeightActual = packages.Sum(x=> x.WeightActual ?? 0),
                        TotalWeightConverted = packages.Sum(x=> x.WeightConverted ?? 0),
                        PackageNo = packages.Count,
                        WalletNo = wallets.Select(x=> x.Id).Distinct().ToList().Count
                    };

                    UnitOfWork.TransferRepo.Add(transfer);

                    await UnitOfWork.TransferRepo.SaveAsync();

                    // Cập nhật lại Mã cho Order và Sum tiền
                    var walletOfDay = UnitOfWork.TransferRepo.Count(x =>
                        x.FromTime.Year == timeNow.Year && x.FromTime.Month == timeNow.Month &&
                        x.FromTime.Day == timeNow.Day && x.Id <= transfer.Id);

                    transfer.Code = $"{walletOfDay}{timeNow:ddMMyy}";
                    transfer.UnsignedText = MyCommon.Ucs2Convert(
                        $"{transfer.Code} {transfer.FromUserFullName} {transfer.FromUserUserName} {transfer.FromWarehouseName} {packageCodes}");

                    await UnitOfWork.TransferRepo.SaveAsync();

                    foreach (var p in packages)
                    {
                        var note = model.TransferDetails.Single(x => x.PackageId == p.Id).Note;
                        var w = wallets.First(x => x.PackageId == p.Id);

                        UnitOfWork.TransferDetailRepo.Add(new TransferDetail()
                        {
                            PackageCode = p.Code,
                            WeightActual = p.WeightActual,
                            Weight = p.Weight,
                            WeightConverted = p.WeightConverted,
                            Created = timeNow,
                            Updated = timeNow,
                            IsDelete = false,
                            Note = note,
                            OrderCode = p.OrderCode,
                            OrderId = p.OrderId,
                            OrderPackageNo = p.PackageNo,
                            OrderServices = p.OrderServices,
                            OrderType = p.OrderType,
                            PackageId = p.Id,
                            Status = 1,
                            TransferCode = transfer.Code,
                            TransferId = transfer.Id,
                            TransportCode = p.TransportCode,
                            WalletCode = w.Code,
                            WalletId = w.Id,
                        });

                        // Cập nhật lại thông tin package
                        p.Status = (byte)OrderPackageStatus.Transferring;
                        p.LastUpdate = DateTime.Now;

                        p.CurrentLayoutId = null;
                        p.CurrentLayoutName = null;
                        p.CurrentLayoutIdPath = null;

                        p.CurrentWarehouseId = null;
                        p.CurrentWarehouseName = null;
                        p.CurrentWarehouseIdPath = null;
                        p.CurrentWarehouseAddress = null;
                        p.ForcastDate = null;

                        p.CustomerWarehouseId = toWarehouse.Id;
                        p.CustomerWarehouseName = toWarehouse.Name;
                        p.CustomerWarehouseAddress = toWarehouse.Address;
                        p.CustomerWarehouseIdPath = toWarehouse.IdPath;

                        // Thêm lịch sử cho package
                        var packageHistory = new PackageHistory()
                        {
                            PackageId = p.Id,
                            PackageCode = p.Code,
                            OrderId = p.OrderId,
                            OrderCode = p.OrderCode,
                            Type = p.OrderType,
                            Status = (byte)OrderPackageStatus.Transferring,
                            Content = $"{EnumHelper.GetEnumDescription(OrderPackageStatus.Transferring)} to warehouse \"{toWarehouse.Name}\"",
                            CustomerId = p.CustomerId,
                            CustomerName = p.CustomerName,
                            UserId = UserState.UserId,
                            UserName = UserState.UserName,
                            UserFullName = UserState.FullName,
                            CreateDate = DateTime.Now,
                        };
                        UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                        var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                            x => x.PackageId == p.Id && x.ObjectId == transfer.Id &&
                                 x.Mode == (byte)PackageNoteMode.Transfer);

                        if (packageNote == null && !string.IsNullOrWhiteSpace(note))
                        {
                            UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                            {
                                OrderId = p.OrderId,
                                OrderCode = p.OrderCode,
                                PackageId = p.Id,
                                PackageCode = p.Code,
                                UserId = transfer.FromUserId,
                                UserFullName = transfer.FromUserFullName,
                                Time = DateTime.Now,
                                ObjectId = transfer.Id,
                                ObjectCode = transfer.Code,
                                Mode = (byte)PackageNoteMode.Transfer,
                                Content = note
                            });
                        }
                        else if (packageNote != null && !string.IsNullOrWhiteSpace(note))
                        {
                            packageNote.Content = note;
                        }
                        else if (packageNote != null && string.IsNullOrWhiteSpace(note))
                        {
                            UnitOfWork.PackageNoteRepo.Remove(packageNote);
                        }
                    }

                    await UnitOfWork.TransferDetailRepo.SaveAsync();

                    transfer.UnsignedText += $"{string.Join(" ", packages.Select(x => x.Code).ToList())} " +
                                             $"{string.Join(" ", packages.Select(x => x.TransportCode).ToList())} " +
                                             $"{string.Join(" ", packages.Select(x => x.OrderCode).Distinct().ToList())} " +
                                             $"{string.Join(" ", packages.Select(x => x.CustomerUserName).Distinct().ToList())} "; 

                    // Cập nhật lại kho đích mới cho Order
                    var orderCodes = $";{string.Join(";", packages.Select(x => x.OrderCode).ToList())};";

                    var orders =
                        await UnitOfWork.OrderRepo.FindAsync(
                            x => x.IsDelete == false && orderCodes.Contains(";" + x.Code + ";"));

                    foreach (var order in orders)
                    {
                        // Note cho order
                        var packageNote2 = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                            x => x.PackageId == null && x.OrderId == order.Id && x.ObjectId == transfer.Id &&
                                 x.Mode == (byte)PackageNoteMode.Transfer);

                        if (packageNote2 == null && !string.IsNullOrWhiteSpace(model.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                            {
                                OrderId = order.Id,
                                OrderCode = order.Code,
                                PackageId = null,
                                PackageCode = null,
                                UserId = transfer.FromUserId,
                                UserFullName = transfer.FromUserFullName,
                                Time = DateTime.Now,
                                ObjectId = transfer.Id,
                                ObjectCode = transfer.Code,
                                Mode = (byte)PackageNoteMode.Transfer,
                                Content = model.Note
                            });
                        }
                        else if (packageNote2 != null && !string.IsNullOrWhiteSpace(model.Note))
                        {
                            packageNote2.Content = model.Note;
                        }
                        else if (packageNote2 != null && string.IsNullOrWhiteSpace(model.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Remove(packageNote2);
                        }

                        // Tiền phát sinh của Order
                        var packagesInTransfer = await UnitOfWork.TransferDetailRepo.FindAsync(
                                x => x.IsDelete == false && x.TransferId == transfer.Id && x.OrderId == order.Id);


                        var totalActualWeight = packagesInTransfer.Sum(x => x.WeightConverted ?? 0);

                        var percent = Math.Round(totalActualWeight * 100 / (transfer.TotalWeightActual ?? 0), 4); 

                        // Thêm tiền phát sinh khi điều chuyển kho
                        var s = new OrderServiceOther()
                        {
                            Value = Math.Round(percent * (transfer.PriceShip ?? 0) / 100, 4) / order.ExchangeRate,
                            Created = DateTime.Now,
                            Mode = 2,
                            CreatedOfficeId = UserState.OfficeId ?? 0,
                            CreatedOfficeIdPath = UserState.OfficeIdPath,
                            CreatedOfficeName = UserState.OfficeName,
                            CreatedUserFullName = UserState.FullName,
                            CreatedUserId = UserState.UserId,
                            CreatedUserTitleId = UserState.TitleId ?? 0,
                            CreatedUserTitleName = UserState.TitleName,
                            CreatedUserUserName = UserState.UserName,
                            Currency = Currency.CNY.ToString(),
                            Note = transfer.Note,
                            ObjectId = transfer.Id,
                            Type = 0,
                            ExchangeRate = order.ExchangeRate,
                            TotalPrice = Math.Round(percent * (transfer.PriceShip ?? 0) / 100, 4),
                            OrderCode = order.Code,
                            OrderId = order.Id,
                            PackageNo = packages.Count,
                            PackageCodes = $";{string.Join(";", packagesInTransfer.Select(x=> x.PackageCode).Distinct().ToList())};",
                            UnsignText = MyCommon.Ucs2Convert($"{order.Code} {order.CustomerEmail}" +
                                                                  $" {order.ContactPhone} {order.CustomerName}")
                        };

                        UnitOfWork.OrderServiceOtherRepo.Add(s);

                        await UnitOfWork.OrderServiceOtherRepo.SaveAsync();

                        // Cập nhật số tiền phát sinh trong Order
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

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật lại kho đích của Order
                        order.WarehouseDeliveryId = toWarehouse.Id;
                        order.WarehouseDeliveryName = toWarehouse.Name;

                        // Cập nhật lại tiền vận chuyển của Order
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

                        // Cập nhật lại Sum tiền của Order
                        var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                                 x.IsDelete == false && x.Checked)
                            .ToList()
                            .Sum(x => x.TotalPrice);

                        order.Total = order.TotalExchange + totalService;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                        order.LastUpdate = DateTime.Now;

                        // Thêm lịch sử cho Order
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = $"\"{UserState.OfficeName}\" Move to warehouse \"{toWarehouse.Name}\"",
                            CustomerId = order.CustomerId ?? 0,
                            CustomerName = order.CustomerName,
                            OrderId = order.Id,
                            Status = order.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = order.Type
                        });

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }

                    // Cập nhật lại kho khách chọn của các package chưa đóng bao của các Order này
                    var packageOld = await UnitOfWork.OrderPackageRepo
                        .FindAsync(x => x.IsDelete == false && x.Status <= (byte) OrderPackageStatus.ChinaInStock
                                        && orderCodes.Contains(";" + x.OrderCode + ";"));

                    foreach (var p in packageOld)
                    {
                        p.CustomerWarehouseId = toWarehouse.Id;
                        p.CustomerWarehouseName = toWarehouse.Name;
                        p.CustomerWarehouseAddress = toWarehouse.Address;
                        p.CustomerWarehouseIdPath = toWarehouse.IdPath;
                    }

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "Created note successfully" },
                JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.Approvel, EnumPage.Transfer)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Approvel(int transferId)
        {
            if (UserState.OfficeType != 1)
                return JsonCamelCaseResult(
                        new { Status = -2, Text = "Only warehouse staff can perform this action" },
                        JsonRequestBehavior.AllowGet);

            if (!ModelState.IsValid)
                return JsonCamelCaseResult(new { Status = -1, Text = "Data format is incorrect" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var transfer =
                        await UnitOfWork.TransferRepo.SingleOrDefaultAsync(
                            x => x.IsDelete == false && x.Id == transferId);

                    if (transfer == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Transport ticket does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    if (transfer.ToWarehouseId != UserState.OfficeId)
                        return JsonCamelCaseResult(new { Status = -1, Text = "You are not an staff of this warehouse destination coupon" },
                            JsonRequestBehavior.AllowGet);

                    if(transfer.Status == 1)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Ticket wait for approval date" },
                            JsonRequestBehavior.AllowGet);

                    transfer.Status = 1;
                    transfer.ToUserId = UserState.UserId;
                    transfer.ToUserTitleId = UserState.TitleId;
                    transfer.ToUserFullName = UserState.FullName;
                    transfer.ToUserUserName = UserState.UserName;
                    transfer.ToTime = DateTime.Now;

                    // Cập nhật lại trạng thái hàng trong kho
                    var packages = await UnitOfWork.TransferDetailRepo.GetByTransferId(transferId);

                    foreach (var p in packages)
                    {
                        // Thêm lịch sử cho package
                        var packageHistory = new PackageHistory()
                        {
                            PackageId = p.Id,
                            PackageCode = p.Code,
                            OrderId = p.OrderId,
                            OrderCode = p.OrderCode,
                            Type = p.OrderType,
                            Status = (byte)OrderPackageStatus.Received,
                            Content = $"[{UserState.OfficeName}] {EnumHelper.GetEnumDescription(OrderPackageStatus.Received)}",
                            CustomerId = p.CustomerId,
                            CustomerName = p.CustomerName,
                            UserId = UserState.UserId,
                            UserName = UserState.UserName,
                            UserFullName = UserState.FullName,
                            CreateDate = DateTime.Now,
                        };

                        UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                        // Cập nhật thông tin kho hiện tại cho package
                        p.Status = (byte)OrderPackageStatus.Received;
                        p.CurrentWarehouseId = UserState.OfficeId;
                        p.CurrentWarehouseName = UserState.OfficeName;
                        p.CurrentWarehouseIdPath = UserState.OfficeIdPath;
                        p.CurrentWarehouseAddress = UserState.OfficeAddress;
                        p.ForcastDate = null;
                    }

                    await UnitOfWork.TransferRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new {Status = 1, Text = "Browse the ticket successfully"},
                JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Transfer)]
        public async Task<ActionResult> GetDetail(int id)
        {
            var data = await UnitOfWork.TransferRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            return JsonCamelCaseResult(data, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Transfer)]
        public async Task<ActionResult> GetPackages(int id)
        {
            var transfer = await UnitOfWork.TransferRepo.SingleOrDefaultAsNoTrackingAsync(x => (x.Id == id) && !x.IsDelete);

            if (transfer == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "packing does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            //if (UserState.OfficeId.HasValue && importWarehouse.WarehouseId != UserState.OfficeId.Value)
            //    return JsonCamelCaseResult(new { Status = -2, Text = "Bạn không phải là nhân viên kho này" }, JsonRequestBehavior.AllowGet);

            var items = await UnitOfWork.TransferDetailRepo.FindAsync(x=> x.TransferId == id);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }
    }
}